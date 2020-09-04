using System.Collections.Generic;
using Skrbot.Domain.Enum;

namespace Skrbot.Domain.Condition
{
    public class FavotiteCondition
    {
        public EnumFavoriteSetting FavoriteSetting { get; set; }

        public IList<Type> Types { get; set; }

        public List<string> LikeType { get; set; }

        public List<string> HateType { get; set; }

        public string UserSeqNum { get; set; }

        public FavotiteCondition()
        {
            Types = new List<Type>();
        }
    }
}
