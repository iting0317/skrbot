namespace Skrbot.Domain.ServiceModel
{
    public class ScoreRatio
    {
        public int CurrentStoreScoreRatio { get; set; }

        public int CurrentStoreScoreWithOthersRatio { get; set; }

        public int DistanceScoreRatio { get; set; }

        public int TenDayNoEatScoreRatio { get; set; }

        public int PriceRatio { get; set; }

        public int NotAlwaysEatScoreRatio { get; set; }
    }
}
