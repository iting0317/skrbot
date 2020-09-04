using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Skrbot.Core.Service;
using Skrbot.WebApi.Models;
using System.Collections.Generic;

namespace Skrbot.WebApi.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        private readonly IStoreService _storeService;
        private readonly IFavoriteService _favoriteService;
        private readonly ITypeService _typeService;

        public RootDialog(IStoreService storeService, IFavoriteService favoriteService, ITypeService typeService)
        {
            _storeService = storeService;
            _favoriteService = favoriteService;
            _typeService = typeService;
        }

        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            IMessageActivity message = await result;

            switch (message.Text)
            {
                case "Random":
                    context.Call(new RandomDialog(_storeService, _favoriteService), AfterSimpleFilterAsync);
                    break;
                case "SimpleFilter":
                    context.Call(new SimpleFilterDialog(_favoriteService, _storeService, _typeService), AfterSimpleFilterAsync);
                    break;
                case "Favorite":
                    context.Call(new FavoriteDialog(_favoriteService, _typeService), AfterFavoriteAsync);
                    break;
                default:
                    await context.PostAsync(GetMainMenu(context));
                    break;
            }
        }

        private IMessageActivity GetMainMenu(IDialogContext context)
        {
            IMessageActivity rootActivity = context.MakeMessage();
            rootActivity.AddHeroCard("skrbot",
                SkrConstants.MainMenuMap.Select(o => o.Id),
                SkrConstants.MainMenuMap.Select(o => o.Name));
            //rootActivity.ChannelData = JsonConvert.SerializeObject(new ChannelData()
            //{
            //    Payload = new ChannelDataMessage()
            //    {
            //        Message = new LocationMessage()
            //        {
            //            Title = "ALL PASTA 義大利麵美式漢堡",
            //            Address = "台北市中山區德惠街52號",
            //            Latitude = 25.0665599,
            //            Longitude = 121.5249113
            //        }
            //    }
            //});
            return rootActivity;
        }

        private async Task AfterSimpleFilterAsync(IDialogContext context, IAwaitable<object> result)
        {
            object condition = await result;
            DialogResponse dialogResponse = condition as DialogResponse;

            if (dialogResponse == null)
            {
                await context.PostAsync("skr 找不到您要的餐廳QQ");
            }
            else if (dialogResponse.Result)
            {
                await context.PostAsync("祝你有個 skrrrr 的午餐！");
            }
            else
            {
                await context.PostAsync("看來你不喜歡 skr 推薦給你的餐廳QQ");
            }
            await Task.Delay(TimeSpan.FromSeconds(2));
            await context.PostAsync(GetMainMenu(context));
        }

        private async Task AfterFavoriteAsync(IDialogContext context, IAwaitable<object> result)
        {
            object condition = await result;
            DialogResponse dialogResponse = condition as DialogResponse;

            if (dialogResponse != null && dialogResponse.Result)
            {
                await context.PostAsync("設定完成！");
            }
            else
            {
                await context.PostAsync("設定怪怪DER~");
            }
            await Task.Delay(TimeSpan.FromSeconds(2));
            await context.PostAsync(GetMainMenu(context));
        }
    }
}
