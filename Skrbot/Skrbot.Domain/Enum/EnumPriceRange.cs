namespace Skrbot.Domain.Enum
{
    public enum EnumPriceRange
    {
        /// <summary>
        /// less than 50
        /// </summary>
        Cheapest,

        /// <summary>
        /// between 50 to 80
        /// </summary>
        Low,

        /// <summary>
        /// between 80 to 100
        /// </summary>
        Midddle,

        /// <summary>
        /// between 100 to 150
        /// </summary>
        High,

        /// <summary>
        /// more than 150
        /// </summary>
        MostExpensive,

        None
    }
}
