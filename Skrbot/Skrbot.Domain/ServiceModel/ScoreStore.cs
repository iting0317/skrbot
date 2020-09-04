namespace Skrbot.Domain.ServiceModel
{
    public class ScoreStore : Store
    {
        public double FinalScore { get; set; }

        public ScoreStore()
        {
        }

        public ScoreStore(Store store)
        {
            Id = store.Id;
            Name = store.Name;
            Price = store.Price;
            Address = store.Address;
            Latitude = store.Latitude;
            Longitude = store.Longitude;
            Distance = store.Distance;
            Seat = store.Seat;
            Url = store.Url;
            CreateDate = store.CreateDate;
            CreateUser = store.CreateUser;
            Score = store.Score;
            StoreUrl = store.StoreUrl;
            PictureUrl = store.PictureUrl;
            PictureType = store.PictureType;
            PositiveRecordList = store.PositiveRecordList;
        }
    }
}
