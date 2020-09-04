using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using Skrbot.Common;
using Skrbot.Core.Service;
using Skrbot.Domain;
using Skrbot.Domain.Condition;
using Skrbot.Domain.Enum;
using Skrbot.Domain.ServiceModel;
using Skrbot.WebApi.Models;

namespace Skrbot.WebApi.Dialogs
{
    [Serializable]
    public class SimpleFilterDialog : IDialog<object>
    {
        private readonly IFavoriteService _favoriteService;
        private readonly IStoreService _storeService;
        private readonly ITypeService _typeService;

        private SimpleFilterCondition _condition { get; set; } = new SimpleFilterCondition();
        private DialogResponse _dialogResponse { get; set; } = null;
        private IList<Domain.Type> _types { get; set; }
        private int _page { get; set; } = 1;

        public SimpleFilterDialog(IFavoriteService favoriteService, IStoreService storeService, ITypeService typeService)
        {
            _favoriteService = favoriteService;
            _storeService = storeService;
            _typeService = typeService;
        }

        public async Task StartAsync(IDialogContext context)
        {
            _types = GetAllTypes();
            AskTypeMenu(context);
        }

        private void AskTypeMenu(IDialogContext context)
        {
            PromptDialog.Choice(context,
                AfterAskTypeMenuAsync,
                SkrConstants.FilterMenuMap.Select(o => o.Name),
                "請選擇午餐類型",
                retry: "你回覆了錯誤的選項，請選擇午餐類型。");
        }

        private async Task AfterAskTypeMenuAsync(IDialogContext context, IAwaitable<string> response)
        {
            string result = await response;
            DialogOption selectedFilterMenu = SkrConstants.FilterMenuMap.FirstOrDefault(o => o.Name == result);
            User user = context.UserData.GetValue<User>("User");

            if (selectedFilterMenu.Id == "FavoriteType")
            {
                _condition.Types = _typeService.GetFavoriteList(user.Id);
                AskPriceRange(context);
            }
            else
            {
                AskType(context);
            }
        }

        private void AskType(IDialogContext context)
        {
            PromptDialog.Choice(context,
                AfterAskTypeAsync,
                GetTypeList(),
                "請選擇午餐類型",
                retry: "你回覆了錯誤的選項，請選擇午餐類型。");
        }

        private async Task AfterAskTypeAsync(IDialogContext context, IAwaitable<string> response)
        {
            string result = await response;

            if (result == "更多")
            {
                _page++;
                AskType(context);
            }
            else if (result == "回首頁")
            {
                _page = 1;
                AskType(context);
            }
            else
            {
                Domain.Type selectedType = _types.FirstOrDefault(t => t.Name == result);

                _condition.Types.Add(selectedType);

                AskNeedMoreType(context);
            }
        }

        private void AskNeedMoreType(IDialogContext context)
        {
            PromptDialog.Choice(context,
                AfterAskNeedMoreTypeAsync,
                SkrConstants.ConfirmMap.Select(o => o.Name),
                "想要選擇更多類型嗎？",
                retry: "你回覆了錯誤的選項，想要選擇更多類型嗎？");
        }

        private async Task AfterAskNeedMoreTypeAsync(IDialogContext context, IAwaitable<string> response)
        {
            string result = await response;
            DialogOption selectedOption = SkrConstants.ConfirmMap.FirstOrDefault(o => o.Name == result);

            if (selectedOption.Id == true.ToString())
            {
                AskType(context);
            }
            else
            {
                AskPriceRange(context);
            }
        }

        private void AskPriceRange(IDialogContext context)
        {
            PromptDialog.Choice(context,
                AfterAskPriceRangeAsync,
                SkrConstants.PriceRangeMap.Select(o => o.Name),
                "請選擇可接受的價格範圍。",
                retry: "你回覆了錯誤的選項。請選擇可接受的價格範圍。");
        }

        private async Task AfterAskPriceRangeAsync(IDialogContext context, IAwaitable<string> response)
        {
            string result = await response;
            string priceRangeString = SkrConstants.PriceRangeMap.FirstOrDefault(o => result == o.Name)?.Id;
            EnumPriceRange priceRange;

            if (!Enum.TryParse(priceRangeString, out priceRange))
            {
                priceRange = EnumPriceRange.Midddle;
            }

            _condition.PriceRange = priceRange;

            AskDistance(context);
        }

        private void AskDistance(IDialogContext context)
        {
            PromptDialog.Choice(context,
                AfterAskDistanceAsync,
                SkrConstants.DistanceMap.Select(o => o.Name),
                "請問你可以接受多遠的餐廳？",
                retry: "你回覆了錯誤的選項。請問你可以接受多遠的餐廳？");
        }

        private async Task AfterAskDistanceAsync(IDialogContext context, IAwaitable<string> response)
        {
            string result = await response;
            DialogOption option = SkrConstants.DistanceMap.FirstOrDefault(o => o.Name == result);
            EnumDistance distance;

            if (!Enum.TryParse(result, out distance))
            {
                distance = EnumDistance.Near;
            }

            _condition.Distance = distance;

            SearchAndRecommendStores(context);
        }

        private IList<string> GetTypeList()
        {
            // Types are not selected
            IList<string> result = _types.Where(t => _condition.Types.FirstOrDefault(ct => ct.Name == t.Name) == null)
                .Skip((_page - 1) * 3)
                .Take(3)
                .Select(t => t.Name)
                .ToList();

            if (result.Count < 3 && _page > 1)
            {
                // No more types
                result.Add("回首頁");
            }
            else if (result.Count == 3 && _page >= _types.Count / 3)
            {
                // Last page
                result.Add("回首頁");
            }
            else if (result.Count == 3)
            {
                // No more types
                result.Add("更多");
            }

            return result;
        }

        private void SearchAndRecommendStores(IDialogContext context)
        {
            User user = context.UserData.GetValue<User>("User");

            IList<ScoreStore> stores = _storeService.GetList(_condition, user);

            context.UserData.SetValue("_stores", stores);

            RecommendStore(context);
        }

        private void RecommendStore(IDialogContext context)
        {
            IList<ScoreStore> stores = context.UserData.GetValue<IList<ScoreStore>>("_stores");
            ScoreStore store = stores.FirstOrDefault();

            if (store != null)
            {
                context.UserData.SetValue("_storeNow", store);

                IMessageActivity heroCard = ShowRecommend(context, store);
                if (heroCard == null)
                {
                    RecommendStore(context);
                }
                else
                {
                    stores.Remove(store);
                    context.UserData.SetValue("_stores", stores);

                    context.PostAsync(heroCard);
                    context.Wait(AfterRecommendStoreAsync);
                }
            }
            else
            {
                context.Done<object>(_dialogResponse);
            }
        }

        private async Task AfterRecommendStoreAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
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

                        IMessageActivity heroCard = ShowRecommend(context, storeNow);
                        context.PostAsync(heroCard);
                        context.Wait(AfterRecommendStoreAsync);
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

        private IList<Domain.Type> GetAllTypes()
        {
            return _typeService.GetList(new TypeCondition());
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
    }
}
