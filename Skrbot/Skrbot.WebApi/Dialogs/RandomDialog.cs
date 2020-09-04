using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Skrbot.Core.Service;
using Skrbot.Domain;
using Skrbot.Domain.Condition;
using Skrbot.Domain.Enum;
using Skrbot.Domain.ServiceModel;
using Skrbot.WebApi.Models;
using Microsoft.Bot.Connector;
using Skrbot.Common;
using System.Web.Script.Serialization;
using Newtonsoft.Json;

namespace Skrbot.WebApi.Dialogs
{
    [Serializable]
    public class RandomDialog : IDialog<object>
    {
        private readonly IStoreService _storeService;
        private readonly IFavoriteService _favoriteService;

        private SimpleFilterCondition _condition { get; set; } = new SimpleFilterCondition();
        private DialogResponse _dialogResponse { get; set; }

        public RandomDialog(IStoreService storeService, IFavoriteService favoriteService)
        {
            _storeService = storeService;
            _favoriteService = favoriteService;
        }

        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync("歡迎使用「隨機」推薦！");
            AskRandomMenu(context);
        }

        private void AskRandomMenu(IDialogContext context)
        {
            PromptDialog.Choice(context,
                AfterAskRandomMenuAsync,
                SkrConstants.RandomMenuMap.Select(o => o.Name),
                "請選擇推薦方式",
                retry: "你回覆了錯誤的選項。請選擇推薦方式");
        }

        private async Task AfterAskRandomMenuAsync(IDialogContext context, IAwaitable<string> response)
        {
            string result = await response;
            DialogOption selectedType = SkrConstants.RandomMenuMap.FirstOrDefault(o => o.Name == result);
            EnumSkrType randomType;

            if (!Enum.TryParse(selectedType.Id, out randomType))
            {
                randomType = EnumSkrType.Random;
            }

            randomType = randomType == EnumSkrType.NotEatenYetRandom ? EnumSkrType.NotEatenYetRandom : EnumSkrType.Random;

            SearchAndRecommendStores(context, randomType);
        }

        private void SearchAndRecommendStores(IDialogContext context, EnumSkrType type)
        {
            User user = context.UserData.GetValue<User>("User");
            IList<ScoreStore> stores = _storeService.GetRandomList(user, type);
            context.UserData.SetValue("_stores", stores);

            RecommendStore(context);
        }

        private void RecommendStore(IDialogContext context)
        {
            ScoreStore store = RandomOneStore(context);

            if (store != null)
            {
				context.UserData.SetValue("_storeNow", store);

                var heroCard = ShowRecommend(context, store);
                if (heroCard == null)
                {
                    RecommendStore(context);
                }
                else
                {
                    context.PostAsync(heroCard);
                    context.Wait(AfterRecommendStore);
                }
            }
            else
            {
                context.Done<object>(_dialogResponse);
            }
        }

        private async Task  AfterRecommendStore( IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            IMessageActivity message = await result;
            
            User user = context.UserData.GetValue<User>("User");
            ScoreStore storeNow = context.UserData.GetValue<ScoreStore>("_storeNow");

            switch (message.Text)
            {
                case "想知道店家地點～":
                    var storeaddress = ShowStoreAddress(context, storeNow);
                    if (storeaddress == null)
                    {
                        RecommendStore(context);
                    }
                    else
                    { 
                        context.PostAsync(storeaddress);
                        await Task.Delay(TimeSpan.FromSeconds(2));

                        var heroCard = ShowRecommend(context, storeNow);
                        context.PostAsync(heroCard);
                        context.Wait(AfterRecommendStore);
                    }
                    break;
                case "這間我想吃！":
                    _dialogResponse = new DialogResponse()
                    {
                        Result = true
                    };
                    _storeService.SaveRecord(storeNow.Id, user.Id, 1);
                    //去過的Score -1
                    _storeService.InsertOrUpdateScore(storeNow.Id, user.Id, -1);
                    context.Done<object>(_dialogResponse);
                    break;
                case "這間好雷...下一間！":
                    //SaveHateList
                    _favoriteService.SaveUserHateList(storeNow.Id, user.Id);
                    _storeService.SaveRecord(storeNow.Id, user.Id, -5);
                    RecommendStore(context);
                    break;
                case "推薦我下一間！":
                    _storeService.SaveRecord(storeNow.Id, user.Id, 0);
                    //沒去過的Score + 1
                    _storeService.InsertOrUpdateScore(storeNow.Id, user.Id, 1);
                    RecommendStore(context);
                    break;
                default:
                    RecommendStore(context);
                    break;
            }
        }

        private IMessageActivity ShowStoreAddress(IDialogContext context, ScoreStore store)
        {
            try
            {
                IMessageActivity rootActivity = context.MakeMessage();

                rootActivity.ChannelData = JsonConvert.SerializeObject(new ChannelData()
                {
                    Payload = new ChannelDataMessage()
                    {
                        Message = new LocationMessage()
                        {
                            Title = store.Name,
                            Address = store.Address,
                            Latitude = store.Latitude,
                            Longitude = store.Longitude
                        }
                    }
                });
                return rootActivity;
            }
            catch (Exception ex)
            {
                Logger.Write(EnumLogCategory.Information, ex.ToString());
                return null;
            }
        }

        private IMessageActivity ShowRecommend(IDialogContext context, ScoreStore store)
        {
            try
            {
                IMessageActivity rootActivity = context.MakeMessage();

                rootActivity.Attachments = new List<Attachment>();
                // AttachmentLayout options are list or carousel
                rootActivity.AttachmentLayout = "carousel";
                List<CardImage> cardImages1 = new List<CardImage>();
                cardImages1.Add(new CardImage(url: store.PictureUrl));

                string PositiveRecordString = "這間還沒有其他人吃過唷，歡迎嘗鮮!";
                if (store.PositiveRecordList != null)
                {
                    PositiveRecordString = $"☆{store.PositiveRecordList.Count}人覺得這間不錯!";
                }

                HeroCard card1 = new HeroCard()
                {
                    Title = $"skr 推薦「{store.Name}」給你",
                    Subtitle = PositiveRecordString,
                    Images = cardImages1,
                    Buttons = GenerateButtons(SkrConstants.RecommandActionMap.Select(o => o.Name))
                };

                Attachment plAttachment1 = card1.ToAttachment();
                // Add the Attachment to the reply message
                rootActivity.Attachments.Add(plAttachment1);

                var json = new JavaScriptSerializer().Serialize(rootActivity);
                Logger.Write(EnumLogCategory.Error, json);

                return rootActivity;
            }
            catch (Exception ex)
            {
                Logger.Write(EnumLogCategory.Information, ex.ToString());
                return null;
            }
        }

        private ScoreStore RandomOneStore(IDialogContext context)
        {
            // Candidate
            IList<ScoreStore> stores = context.UserData.GetValue<IList<ScoreStore>>("_stores");

            ScoreStore result = null;

            if (stores != null && stores.Count > 0)
            {
                // Random one store
                Random random = new Random();
                int index = random.Next(stores.Count - 1);
                result = stores[index];

                // Remove the same store from store pool
                stores = stores.Where(s => s.Id != result.Id).ToList();
                context.UserData.SetValue("_stores", stores);
            }

            return result;
        }

        private IList<CardAction> GenerateButtons<T>(IEnumerable<T> options, IEnumerable<string> descriptions = null)
        {
            var actions = new List<CardAction>();
            int i = 0;
            var adescriptions = descriptions?.ToArray();
            foreach (var option in options)
            {
                var title = (adescriptions == null ? option.ToString() : adescriptions[i]);
                actions.Add(new CardAction
                {
                    Title = title,
                    Type = ActionTypes.ImBack,
                    Value = option.ToString()
                });
                ++i;
            }
            return actions;
        }
    }
}
