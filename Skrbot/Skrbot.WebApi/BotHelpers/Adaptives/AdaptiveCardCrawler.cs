using AdaptiveCards;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Skrbot.WebApi.BotHelpers.Adaptives
{
    [Serializable]
    public class AdaptiveCardCrawler
    {
        private AdaptiveCard Card { get; set; }

        public AdaptiveCardCrawler(AdaptiveCard card)
        {
            Card = card;
        }

        // 透過ID取得Adaptive Card Element
        public AdaptiveElement GetCardElementById(string id)
        {
            AdaptiveElement target;

            if (SelectCardElementById(Card.Body, id, out target))
            {
                return target;
            }
            return null;
        }

        // 以Id尋找Adaptive Element，並設定樣式
        public AdaptiveCard SetElementPropertyValueById(string id, string propertyName, object value)
        {
            var element = GetCardElementById(id);

            if (element != null)
            {
                // 檢查Property是否符合
                var prop = element.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
                if (prop != null && prop.CanWrite && prop.PropertyType == value.GetType())
                {
                    prop.SetValue(element, value, null);
                }
            }
            return Card;
        }

        // 以Id尋找Adaptive Element，並設定Select Action
        public AdaptiveCard SetSelectActionById(string id, CardAction tap)
        {
            var element = GetCardElementById(id);

            if (element == null)
            {
                return Card;
            }

            var selectAction = AdaptiveElementBuilder.CreateAction(tap);
            switch (element.Type)
            {
                case AdaptiveImage.TypeName:
                    (element as AdaptiveImage).SelectAction = selectAction;
                    break;
                case AdaptiveContainer.TypeName:
                    (element as AdaptiveContainer).SelectAction = selectAction;
                    break;
                case AdaptiveColumnSet.TypeName:
                    (element as AdaptiveColumnSet).SelectAction = selectAction;
                    break;
                case AdaptiveColumn.TypeName:
                    (element as AdaptiveColumn).SelectAction = selectAction;
                    break;
            }
            return Card;
        }


        #region Bind Data

        // 以Id尋找Adaptive Element，並設定Adaptive Element Data
        public AdaptiveCard BindDataById(string id, object value)
        {
            var element = GetCardElementById(id);

            if (element == null)
            {
                return Card;
            }

            switch (element.Type)
            {
                case AdaptiveTextBlock.TypeName:
                    SetValue(element as AdaptiveTextBlock, value);
                    break;
                case AdaptiveImage.TypeName:
                    SetValue(element as AdaptiveImage, value);
                    break;
                case AdaptiveFactSet.TypeName:
                    SetValue(element as AdaptiveFactSet, value);
                    break;
                case AdaptiveImageSet.TypeName:
                    SetValue(element as AdaptiveImageSet, value);
                    break;
                case AdaptiveTextInput.TypeName:
                    SetValue(element as AdaptiveTextInput, value);
                    break;
                case AdaptiveNumberInput.TypeName:
                    SetValue(element as AdaptiveNumberInput, value);
                    break;
                case AdaptiveDateInput.TypeName:
                    SetValue(element as AdaptiveDateInput, value);
                    break;
                case AdaptiveTimeInput.TypeName:
                    SetValue(element as AdaptiveTimeInput, value);
                    break;
                case AdaptiveToggleInput.TypeName:
                    SetValue(element as AdaptiveToggleInput, value);
                    break;
                case AdaptiveChoiceSetInput.TypeName:
                    SetValue(element as AdaptiveChoiceSetInput, value);
                    break;
            }
            return Card;
        }

        private void SetValue(AdaptiveTextBlock textBlock, object value)
        {
            textBlock.Text = Convert.ToString(value);
        }
        private void SetValue(AdaptiveTextInput textInput, object value)
        {
            textInput.Value = Convert.ToString(value);
        }
        private void SetValue(AdaptiveNumberInput numberInput, object value)
        {
            numberInput.Value = Convert.ToDouble(value);
        }
        private void SetValue(AdaptiveDateInput dataInput, object value)
        {
            if (value is DateTime)
            {
                dataInput.Value = ((DateTime)value).ToString("yyyy-MM-dd");
            }
            if (value is string)
            {
                DateTime date;
                if (DateTime.TryParse(value as string, out date))
                {
                    dataInput.Value = date.ToString("yyyy-MM-dd");
                }
            }
        }
        private void SetValue(AdaptiveTimeInput timeInput, object value)
        {
            if (value is DateTime)
            {
                timeInput.Value = ((DateTime)value).ToString("HH:mm:ss");
            }
            if (value is string)
            {
                DateTime time;
                if (DateTime.TryParse(value as string, out time))
                {
                    timeInput.Value = time.ToString("HH:mm:ss");
                }
            }
        }
        private void SetValue(AdaptiveToggleInput toggleInput, object value)
        {
            toggleInput.Value = Convert.ToString(value);
        }
        private void SetValue(AdaptiveChoiceSetInput choiceSet, object value)
        {
            choiceSet.Value = Convert.ToString(value);
        }
        private void SetValue(AdaptiveImage image, object value)
        {
            if (value is Uri)
            {
                image.Url = value as Uri;
            }
            else if (value is string)
            {
                image.Url = new Uri(Convert.ToString(value ?? ""));
            }
        }
        private void SetValue(AdaptiveFactSet factSet, object value)
        {
            if (value is List<AdaptiveFact>)
            {
                factSet.Facts = value as List<AdaptiveFact>;
            }
            else if (value is List<Fact>)
            {
                factSet = AdaptiveElementBuilder.CreateFactSet(value as List<Fact>) as AdaptiveFactSet;
            }
            else if (value is Dictionary<string, object>)
            {
                factSet.Facts = new List<AdaptiveFact>();
                var dictionary = value as Dictionary<string, object>;
                var keys = dictionary.Keys;
                foreach (var key in keys)
                {
                    factSet.Facts.Add(new AdaptiveFact(key, Convert.ToString(dictionary[key])));
                }
            }
        }
        private void SetValue(AdaptiveImageSet imageSet, object value)
        {
            if (value is List<AdaptiveImage>)
            {
                imageSet.Images = value as List<AdaptiveImage>;
            }
            else if (value is List<CardImage>)
            {
                imageSet = AdaptiveElementBuilder.CreateImageSet(value as List<CardImage>) as AdaptiveImageSet;
            }
            else if (value is List<MediaUrl>)
            {
                imageSet = AdaptiveElementBuilder.CreateImageSet(value as List<MediaUrl>) as AdaptiveImageSet;
            }
        }

        #endregion


        #region Common Method

        // 尋找ID
        private bool SelectCardElementById<T>(List<T> elements, string id, out AdaptiveElement target)
        {
            foreach (var element in elements)
            {
                var isFound = SelectCardElementById(element as AdaptiveElement, id, out target);
                if (isFound)
                {
                    return isFound;
                }
            }
            target = null;
            return false;
        }
        // 尋找ID
        private bool SelectCardElementById(AdaptiveElement element, string id, out AdaptiveElement target)
        {
            bool isFound = false;
            if (element.Id != null && element.Id == id)
            {   // ID符合的物件
                target = element;
                return true;
            }

            switch (element.Type)
            {   // 如果是Container
                case AdaptiveContainer.TypeName:
                    isFound = SelectCardElementById((element as AdaptiveContainer).Items, id, out target);
                    break;
                case AdaptiveColumnSet.TypeName:
                    isFound = SelectCardElementById((element as AdaptiveColumnSet).Columns, id, out target);
                    break;
                case AdaptiveColumn.TypeName:
                    isFound = SelectCardElementById((element as AdaptiveColumn).Items, id, out target);
                    break;
                case AdaptiveImageSet.TypeName:
                    isFound = SelectCardElementById((element as AdaptiveImageSet).Images, id, out target);
                    break;
                default:
                    target = null;
                    break;
            }
            return isFound;
        }

        #endregion
    }
}