using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Skrbot.Core.Service;
using Skrbot.Domain;
using Skrbot.Domain.Condition;
using Skrbot.Domain.Enum;
using Skrbot.WebApi.Models;

namespace Skrbot.WebApi.Dialogs
{
    [Serializable]
    public class FavoriteDialog : IDialog<object>
    {
        private readonly IFavoriteService _favoriteService;
        private readonly ITypeService _typeService;

        private FavotiteCondition _condition { get; set; } = new FavotiteCondition();
        private DialogResponse _dialogResponse { get; set; }
        private IList<Domain.Type> _types { get; set; }
        private int _page { get; set; } = 1;

        public FavoriteDialog(IFavoriteService favoriteService, ITypeService typeService)
        {
            _favoriteService = favoriteService;
            _typeService = typeService;
        }

        public async Task StartAsync(IDialogContext context)
        {
            _types = GetAllTypes();
            AskFavotieSetting(context);
        }

        private void AskFavotieSetting(IDialogContext context)
        {
            PromptDialog.Choice(context,
                AfterAskFavotieSettingAsync,
                SkrConstants.FavoriteMenuMap.Select(o => o.Name),
                "請選擇設定項目",
                retry: "無此選項，請重新選擇");
        }

        private async Task AfterAskFavotieSettingAsync(IDialogContext context, IAwaitable<string> response)
        {
            string result = await response;
            DialogOption selectedFavoriteSetting = SkrConstants.FavoriteMenuMap.FirstOrDefault(o => o.Name == result);

            if (selectedFavoriteSetting.Id == EnumFavoriteSetting.Favorite.ToString())
            {
                _condition.FavoriteSetting = EnumFavoriteSetting.Favorite;
                AskType(context);
            }
            else if (selectedFavoriteSetting.Id == EnumFavoriteSetting.Hate.ToString())
            {
                _condition.FavoriteSetting = EnumFavoriteSetting.Hate;
                AskType(context);
            }
            else if (selectedFavoriteSetting.Id == EnumFavoriteSetting.ResetFavorite.ToString())
            {
                _condition.FavoriteSetting = EnumFavoriteSetting.ResetFavorite;
                ResetFavorite(context);
            }
            else if (selectedFavoriteSetting.Id == EnumFavoriteSetting.ResetHate.ToString())
            {
                _condition.FavoriteSetting = EnumFavoriteSetting.ResetHate;
                ResetHate(context);
            }
            else
            {
                AskFavotieSetting(context);
            }
        }

        private void ResetFavorite(IDialogContext context)
        {
            User user = context.UserData.GetValue<User>("User");

            _dialogResponse = new DialogResponse()
            {
                Result = true
            };

            _dialogResponse.Result = _favoriteService.ResetUserLikeTypeList(user.Id);

             context.Done<object>(_dialogResponse);
        }

        private void ResetHate(IDialogContext context)
        {
            User user = context.UserData.GetValue<User>("User");

            _dialogResponse = new DialogResponse()
            {
                Result = true
            };

            _dialogResponse.Result = _favoriteService.ResetUserHateTypeList(user.Id);

            context.Done<object>(_dialogResponse);
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
                SaveFavoriteHateTypes(context);
            }
        }

        private void SaveFavoriteHateTypes(IDialogContext context)
        {
            User user = context.UserData.GetValue<User>("User");

            _condition.UserSeqNum = user.Id;

            if (_condition.FavoriteSetting == EnumFavoriteSetting.Favorite)
            {
                _condition.LikeType = _condition.Types.Select(t => t.Id).ToList();
                _favoriteService.SaveUserLikeTypeList(_condition);
            }
            else
            {
                _condition.HateType = _condition.Types.Select(t => t.Id).ToList();
                _favoriteService.SaveUserHateTypeList(_condition);
            }

            _dialogResponse = new DialogResponse()
            {
                Result = true
            };

            context.Done<object>(_dialogResponse);
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

        private IList<Domain.Type> GetAllTypes()
        {
            return _typeService.GetList(new TypeCondition());
        }
    }
}
