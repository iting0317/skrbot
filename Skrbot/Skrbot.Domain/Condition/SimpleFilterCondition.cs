using System.Collections.Generic;
using Skrbot.Domain.Enum;

namespace Skrbot.Domain.Condition
{
    public class SimpleFilterCondition
    {
        public IList<Type> Types { get; set; }

        public EnumPriceRange PriceRange { get; set; }

        public EnumDistance Distance { get; set; }

        public SimpleFilterCondition()
        {
            Types = new List<Type>();
        }
    }
}
