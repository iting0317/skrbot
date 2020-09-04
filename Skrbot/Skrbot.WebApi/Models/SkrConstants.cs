using System.Collections.Generic;
using Skrbot.Domain.Enum;

namespace Skrbot.WebApi.Models
{
    public static class SkrConstants
    {
        public static IList<DialogOption> MainMenuMap = new List<DialogOption>()
        {
            new DialogOption()
            {
                Id = EnumSkrType.Random.ToString(),
                Name = "隨機"
            },
            new DialogOption()
            {
                Id = EnumSkrType.SimpleFilter.ToString(),
                Name = "簡單過濾"
            },
            new DialogOption()
            {
                Id = EnumSkrType.Favorite.ToString(),
                Name = "偏好設定"
            }
        };

        public static IList<DialogOption> RandomMenuMap = new List<DialogOption>()
        {
            new DialogOption()
            {
                Id = EnumSkrType.Random.ToString(),
                Name = "完全隨機"
            },
            new DialogOption()
            {
                Id = EnumSkrType.NotEatenYetRandom.ToString(),
                Name = "未吃過的"
            }
        };

        public static IList<DialogOption> FilterMenuMap = new List<DialogOption>()
        {
            new DialogOption()
            {
                Id = "FavoriteType",
                Name = "使用我的最愛類別"
            },
            new DialogOption()
            {
                Id = "OtherType",
                Name = "選擇其他類別"
            }
        };

        public static IList<DialogOption> FavoriteMenuMap = new List<DialogOption>()
        {
            new DialogOption()
            {
                Id = EnumFavoriteSetting.Favorite.ToString(),
                Name = "新增最愛類別"
            },
            new DialogOption()
            {
                Id = EnumFavoriteSetting.Hate.ToString(),
                Name = "新增討厭類別"
            },
            new DialogOption()
            {
                Id = EnumFavoriteSetting.ResetFavorite.ToString(),
                Name = "重設最愛類別"
            }
            ,
            new DialogOption()
            {
                Id = EnumFavoriteSetting.ResetHate.ToString(),
                Name = "重設討厭類別"
            }
        };

        public static IList<DialogOption> PriceRangeMap = new List<DialogOption>()
        {
            new DialogOption()
            {
                Id = EnumPriceRange.Low.ToString(),
                Name = "低（< 80元）"
            },
            new DialogOption()
            {
                Id = EnumPriceRange.Midddle.ToString(),
                Name = "中（80 ~ 150元）"
            },
            new DialogOption()
            {
                Id = EnumPriceRange.High.ToString(),
                Name = "高（> 150元）"
            },
            new DialogOption()
            {
                Id = EnumPriceRange.None.ToString(),
                Name = "不限"
            }
        };

        public static IList<DialogOption> DistanceMap = new List<DialogOption>()
        {
            new DialogOption()
            {
                Id = EnumDistance.Near.ToString(),
                Name = "近（< 250公尺）"
            },
            new DialogOption()
            {
                Id = EnumDistance.Middle.ToString(),
                Name = "中（250 ~ 500公尺）"
            },
            new DialogOption()
            {
                Id = EnumDistance.Far.ToString(),
                Name = "遠（> 500公尺）"
            },
            new DialogOption()
            {
                Id = EnumDistance.None.ToString(),
                Name = "不限"
            }
        };

        public static IList<DialogOption> ConfirmMap = new List<DialogOption>()
        {
            new DialogOption()
            {
                Id = true.ToString(),
                Name = "是"
            },
            new DialogOption()
            {
                Id = false.ToString(),
                Name = "否"
            }
        };

        public static IList<DialogOption> RecommandActionMap = new List<DialogOption>()
        {
            new DialogOption()
            {
                Id = "Address",
                Name = "想知道店家地點～"
            },
            new DialogOption()
            {
                Id = "Yes",
                Name = "這間我想吃！"
            },
            new DialogOption()
            {
                Id = "Hate",
                Name = "這間好雷...下一間！"
            },
            new DialogOption()
            {
                Id = "Skip",
                Name = "推薦我下一間！"
            }
        };
    }
}
