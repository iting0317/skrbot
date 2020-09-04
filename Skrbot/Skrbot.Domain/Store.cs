using System;
using System.Collections.Generic;

namespace Skrbot.Domain
{
    public class Store
    {
        public string Id { get; set; }
        
        public string Name { get; set; }

        public double Price { get; set; }

        public string Address { get; set; }

        public double Latitude { get; set; }
        
        public double Longitude { get; set; }

        public double Distance { get; set; }

        public string Seat { get; set; }

        public string Url { get; set; }

        public DateTime? CreateDate { get; set; }

        public string CreateUser { get; set; }

        public double Score { get; set; }

        public IList<string> PositiveRecordList { get; set; }

        public string StoreUrl { get; set; }

        public string PictureUrl { get; set; }

        public string PictureType { get; set; }
    }
}
