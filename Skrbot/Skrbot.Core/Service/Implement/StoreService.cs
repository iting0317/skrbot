using System;
using System.Collections.Generic;
using System.Linq;
using Skrbot.Dao;
using Skrbot.Domain;
using Skrbot.Domain.Condition;
using Skrbot.Domain.Enum;
using Skrbot.Domain.ServiceModel;

namespace Skrbot.Core.Service.Implement
{
    public class StoreService : IStoreService
    {
        private readonly IRecordDao _recordDao;
        private readonly IStoreDao _storeDao;

        public StoreService(IRecordDao recordDao, IStoreDao storeDao)
        {
            _recordDao = recordDao;
            _storeDao = storeDao;
        }

        public IList<ScoreStore> GetRandomList(User user, EnumSkrType type)
        {
            IList<Store> stores = _storeDao.GetList(user, type);
            stores = _recordDao.GetPositiveRecordNum(stores);
            IList<ScoreStore> scoreStores = CalculateScore(stores, EnumSkrType.Random, user);

            IList<ScoreStore> result = new List<ScoreStore>();
            int totalFinalScore = 0;

            scoreStores.ToList()
                .ForEach(s =>
                {
                    int count = Convert.ToInt32(s.FinalScore);
                    totalFinalScore += count;
                });

            scoreStores.ToList()
                .ForEach(s =>
                {
                    int count = Convert.ToInt32(s.FinalScore / totalFinalScore * 500);
                    IList<ScoreStore> sameScoreStores = Enumerable.Repeat(s, count).ToList();
                    result = result.Concat(sameScoreStores).ToList();
                });

            // Shuffle list
            Random random = new Random();
            int index = scoreStores.Count;

            while (index > 1)
            {
                index--;
                int swapIndex = random.Next(index + 1);
                ScoreStore value = scoreStores[swapIndex];
                scoreStores[swapIndex] = scoreStores[index];
                scoreStores[index] = value;
            }

            return result;
        }
        
        public IList<ScoreStore> GetList(SimpleFilterCondition condition, User user)
        {
            IList<Store> stores = _storeDao.GetList(condition, user);
            stores = _recordDao.GetPositiveRecordNum(stores);

            return CalculateScore(stores, EnumSkrType.SimpleFilter, user);
        }

        private IList<ScoreStore> CalculateScore(IList<Store> stores, EnumSkrType type, User user)
        {
            ScoreRatio scoreRatio = GetScoreRatio(type);

            return stores.Select(s => CalculateScore(s, scoreRatio, user))
                .Where(s => s.FinalScore > 0)
                .OrderByDescending(s => s.FinalScore)
                .ToList();
        }

        private ScoreStore CalculateScore(Store store, ScoreRatio scoreRatio, User user)
        {
            ScoreStore scoreStore = new ScoreStore(store);
            int eatInTenDayRecord = 0;

            double currentStoreScore = CalculateFirstScore(store, scoreRatio.CurrentStoreScoreRatio);
            
            double currentStoreScoreWithOthers = CalculateSecondScore(store, scoreRatio.CurrentStoreScoreWithOthersRatio);

            double distanceScore = CalculateThirdScore(store, scoreRatio.DistanceScoreRatio);

            IDictionary<string, double> fourthResult = CalculateFourthScore(store, scoreRatio.TenDayNoEatScoreRatio, user);
            double tenDayNoEatScore = fourthResult["tenDayNoEatScore"];
            eatInTenDayRecord = Convert.ToInt32(fourthResult["eatInTenDayRecord"]);

            double priceScore = CalculateFifthScore(store, scoreRatio.PriceRatio);

            double notAlwaysEatScore = CalculateSixthScore(store, scoreRatio.NotAlwaysEatScoreRatio, eatInTenDayRecord);

            scoreStore.FinalScore = currentStoreScore + currentStoreScoreWithOthers + distanceScore + tenDayNoEatScore + priceScore + notAlwaysEatScore;

            return scoreStore;
        }

        private ScoreRatio GetScoreRatio(EnumSkrType type)
        {
            ScoreRatio result = new ScoreRatio();

            switch (type)
            {
                case EnumSkrType.Random:
                    result.CurrentStoreScoreRatio = 1;
                    result.CurrentStoreScoreWithOthersRatio = 5;
                    result.DistanceScoreRatio = 3;
                    result.TenDayNoEatScoreRatio = 5;
                    result.PriceRatio = 2;
                    result.NotAlwaysEatScoreRatio = 4;
                    break;
                case EnumSkrType.NotEatenYetRandom:
                    result.CurrentStoreScoreRatio = 1;
                    result.CurrentStoreScoreWithOthersRatio = 5;
                    result.DistanceScoreRatio = 3;
                    result.TenDayNoEatScoreRatio = 5;
                    result.PriceRatio = 2;
                    result.NotAlwaysEatScoreRatio = 5;
                    break;
                case EnumSkrType.SimpleFilter:
                    result.CurrentStoreScoreRatio = 5;
                    result.CurrentStoreScoreWithOthersRatio = 1;
                    result.DistanceScoreRatio = 3;
                    result.TenDayNoEatScoreRatio = 3;
                    result.PriceRatio = 2;
                    result.NotAlwaysEatScoreRatio = 1;
                    break;
                case EnumSkrType.Favorite:
                    result.CurrentStoreScoreRatio = 5;
                    result.CurrentStoreScoreWithOthersRatio = 1;
                    result.DistanceScoreRatio = 1;
                    result.TenDayNoEatScoreRatio = 1;
                    result.PriceRatio = 1;
                    result.NotAlwaysEatScoreRatio = 1;
                    break;
                default:
                    result.CurrentStoreScoreRatio = 1;
                    result.CurrentStoreScoreWithOthersRatio = 1;
                    result.DistanceScoreRatio = 1;
                    result.TenDayNoEatScoreRatio = 1;
                    result.PriceRatio = 1;
                    result.NotAlwaysEatScoreRatio = 1;
                    break;
            }

            return result;
        }

        private double CalculateFirstScore(Store store, int ratio)
        {
            return store.Score * ratio;
        }

        private double CalculateSecondScore(Store store, int ratio)
        {
            RecordCondition recordCondition = new RecordCondition()
            {
                CreateDate = DateTime.UtcNow.AddDays(-7)
            };
            int totalRecord = _recordDao.GetCount(recordCondition);

            recordCondition.StoreId = store.Id;
            int storeRecord = _recordDao.GetCount(recordCondition);
            return (totalRecord == 0 ? 1 : storeRecord / totalRecord) * 100 * ratio;
        }

        private double CalculateThirdScore(Store store, int ratio)
        {
            double currentLatitude = 25.0668195;
            double currentLongitude = 121.5228352;

            // 將兩點距離換算成公尺(1度距離約111000公尺)，除上100 (以100公尺做為一個級距)
            double distance = CalculateDistance(currentLatitude, currentLongitude, store.Latitude, store.Longitude) * 111000 / 100;

            if (distance >= 10)
            {
                distance = 10;
            }

            return (10 - distance) * 10 * ratio;
        }

        private double CalculateDistance(double latitudeOne, double longitudeOne, double latitudeTwo, double longitudeTwo)
        {
            return Math.Sqrt((latitudeOne - latitudeTwo) * (latitudeOne - latitudeTwo) + (longitudeOne - longitudeTwo) * (longitudeOne - longitudeTwo));
        }

        private IDictionary<string, double> CalculateFourthScore(Store store, int ratio, User user)
        {
            RecordCondition condition = new RecordCondition()
            {
                CreateDate = DateTime.UtcNow.AddDays(-10),
                StoreId = store.Id,
                UserId = user.Id
            };
            int eatInTenDayRecord = _recordDao.GetCount(condition);

            return new Dictionary<string, double>()
            {
                { "tenDayNoEatScore", eatInTenDayRecord > 0 ? 0 : 100 * ratio },
                { "eatInTenDayRecord", eatInTenDayRecord }
            };
        }

        private double CalculateFifthScore(Store store, int ratio)
        {
            double priceLevel = store.Price;
            double result = 0;

            if (priceLevel > 15)
            {
                result = (0 - 100) * ratio;
            }
            else if (priceLevel > 7)
            {
                result = 0;
            }
            else
            {
                result = (7 - priceLevel) * 100 / 7 * ratio;
            }

            return result;
        }

        private double CalculateSixthScore(Store store, int ratio, int eatInTenDayRecord)
        {
            double minusPoint = Math.Pow(1.5, eatInTenDayRecord);
            double result = 0;

            if (minusPoint > 57)
            {
                result = (0 - 100) * ratio;
            }
            else if (minusPoint > 10)
            {
                result = 0;
            }
            else
            {
                result = (10 - minusPoint) * 10 * ratio;
            }

            return result;
        }

        public void SaveRecord(string storeNo, string userNo, int score)
        {
            Dao.Implement.RecordDao recordDao = new Dao.Implement.RecordDao();
            recordDao.SaveRecord(storeNo, userNo, score);
        }

        public void InsertOrUpdateScore(string storeNo, string userNo, int score)
        {
            Dao.Implement.ScoreDao scoreDao = new Dao.Implement.ScoreDao();
            scoreDao.InsertOrUpdateScore(storeNo, userNo, score);
        }
    }
}
