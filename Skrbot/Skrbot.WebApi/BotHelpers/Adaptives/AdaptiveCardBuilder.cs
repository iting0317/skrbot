using AdaptiveCards;
using Microsoft.Bot.Connector;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Skrbot.WebApi.BotHelpers.Adaptives
{
    public enum AdaptiveListCardImageLayout { None, Right, Left }

    [Serializable]
    public static class AdaptiveCardBuilder
    {
        #region Rich Card To Adaptive Card

        // Create AdaptiveCard from HeroCard
        public static AdaptiveCard CreateHeroCard(HeroCard card, string version = "1.0")
        {
            var adaptiveCard = new AdaptiveCard();
            var body = new AdaptiveContainer() { Items = new List<AdaptiveElement>() };

            // Add Image
            var image = AdaptiveElementBuilder.CreateImage(card.Images, AdaptiveImageSize.Large);
            if (image != null)
            {
                body.Items.Add(image);
            }

            // Add Title, SubTitle and Text
            body.Items.AddRange(AdaptiveElementBuilder.CreateTitle(card.Title, card.Subtitle, card.Text));

            // Set Tap Action
            if (card.Tap != null)
            {
                body.SelectAction = AdaptiveElementBuilder.CreateAction(card.Tap);
            }

            // Set Body and Actions
            adaptiveCard.Body = new List<AdaptiveElement>() { body };
            adaptiveCard.Actions = AdaptiveElementBuilder.CreateActions(card.Buttons);
            adaptiveCard.Version = version;
            return adaptiveCard;
        }

        // Create AdaptiveCard from ThumbnailCard
        public static AdaptiveCard CreateThumbnailCard(ThumbnailCard card, string version = "1.0")
        {
            var adaptiveCard = new AdaptiveCard();
            var body = new AdaptiveContainer() { Items = new List<AdaptiveElement>() };

            // Add Image
            var image = AdaptiveElementBuilder.CreateImage(card.Images, AdaptiveImageSize.Medium);
            if (image != null)
            {
                body.Items.Add(image);
            }

            // Add Title, SubTitle and Text
            body.Items.AddRange(AdaptiveElementBuilder.CreateTitle(card.Title, card.Subtitle, card.Text));

            // Set Tap Action
            if (card.Tap != null)
            {
                body.SelectAction = AdaptiveElementBuilder.CreateAction(card.Tap);
            }

            // Set Body and Actions
            adaptiveCard.Body = new List<AdaptiveElement>() { body };
            adaptiveCard.Actions = AdaptiveElementBuilder.CreateActions(card.Buttons);
            adaptiveCard.Version = version;
            return adaptiveCard;
        }

        // Create AdaptiveCard from SigninCard
        public static AdaptiveCard CreateSigninCard(SigninCard card, string version = "1.0")
        {
            var adaptiveCard = new AdaptiveCard();

            var body = new AdaptiveContainer() { Items = new List<AdaptiveElement>() };

            // Add Title
            body.Items.AddRange(AdaptiveElementBuilder.CreateTitle(card.Text));

            // Set Body and Actions
            adaptiveCard.Body = new List<AdaptiveElement>() { body };
            adaptiveCard.Actions = AdaptiveElementBuilder.CreateActions(card.Buttons);
            adaptiveCard.Version = version;
            return adaptiveCard;
        }

        // Create AdaptiveCard from ReceiptCard
        public static AdaptiveCard CreateReceiptCard(ReceiptCard card, string totalTitle = "Total", string vatTitle = "VAT", string taxTitle = "Tax", string version = "1.0")
        {
            var adaptiveCard = new AdaptiveCard();
            var body = new AdaptiveContainer() { Items = new List<AdaptiveElement>() };

            // Add Title
            body.Items.AddRange(AdaptiveElementBuilder.CreateTitle(card.Title));

            // Add Receipt Fact
            if (card.Facts != null && card.Facts.Count != 0)
            {
                body.Items.AddRange(CreateReceiptFactSet(card.Facts));
            }

            // Add Separator
            body.Items.Add(CreateReceiptSeparator());

            // Add Receipt Item
            if (card.Items != null && card.Items.Count != 0)
            {
                body.Items.AddRange(CreateReceiptItems(card.Items.ToList()));
            }

            // Add Separator
            body.Items.Add(CreateReceiptSeparator());

            // Add VAT
            if (!string.IsNullOrEmpty(card.Vat))
            {
                body.Items.Add(CreateReceiptOtherInfo(vatTitle, card.Vat, "Vat"));
            }

            // Add Tax
            if (!string.IsNullOrEmpty(card.Tax))
            {
                body.Items.Add(CreateReceiptOtherInfo(taxTitle, card.Tax, "Tax"));
            }

            // Add Total
            if (!string.IsNullOrEmpty(card.Total))
            {
                body.Items.Add(CreateReceiptTotalInfo(totalTitle, card.Total));
            }

            // Set Tap Action
            if (card.Tap != null)
            {
                body.SelectAction = AdaptiveElementBuilder.CreateAction(card.Tap);
            }

            // Set Body and Actions
            adaptiveCard.Body = new List<AdaptiveElement>() { body };
            adaptiveCard.Actions = AdaptiveElementBuilder.CreateActions(card.Buttons);
            adaptiveCard.Version = version;

            return adaptiveCard;
        }

        // Create AdaptiveCard from AnimationCard
        public static AdaptiveCard CreateAnimationCard(AnimationCard card, string version = "1.0")
        {
            var adaptiveCard = new AdaptiveCard();
            var body = new AdaptiveContainer() { Items = new List<AdaptiveElement>() };

            // Add Image
            var images = AdaptiveElementBuilder.CreateImage(card.Media, AdaptiveImageSize.Large);
            if (images != null)
            {
                body.Items.Add(images);
            }

            // Add Title, SubTitle and Text
            body.Items.AddRange(AdaptiveElementBuilder.CreateTitle(card.Title, card.Subtitle, card.Text));

            // Set Body and Actions
            adaptiveCard.Body = new List<AdaptiveElement>() { body };
            adaptiveCard.Actions = AdaptiveElementBuilder.CreateActions(card.Buttons);
            adaptiveCard.Version = version;
            return adaptiveCard;
        }

        #endregion


        #region Create Common Adaptive Card

        // Create Confirm Card
        public static AdaptiveCard CreateConfirmCard(string title, CardAction yesAction = null, CardAction noAction = null, CardImage image = null, string version = "1.0")
        {
            var adaptiveCard = new AdaptiveCard();
            var body = new AdaptiveContainer() { Items = new List<AdaptiveElement>() };

            // Add Image
            if (image != null)
            {
                // Add Image
                body.Items.Add(AdaptiveElementBuilder.CreateImage(image, AdaptiveImageSize.Medium));
            }

            // Add Title
            body.Items.AddRange(AdaptiveElementBuilder.CreateTitle(title));

            // Add Action
            var actions = AdaptiveElementBuilder.CreateActions(new List<CardAction>() {
                yesAction ?? GetYesAction(),
                noAction ?? GetNoAction(),
            });

            // Set Body and Actions
            adaptiveCard.Body = new List<AdaptiveElement>() { body };
            adaptiveCard.Actions = actions;
            adaptiveCard.Version = version;
            return adaptiveCard;
        }
        // Create Confirm Action
        public static CardAction GetYesAction(string title = "Yes", string value = "yes")
        {
            return new CardAction(ActionTypes.ImBack, title: title, value: value);
        }
        public static CardAction GetNoAction(string title = "No", string value = "no")
        {
            return new CardAction(ActionTypes.ImBack, title: title, value: value);
        }

        // Create Choice Card
        public static AdaptiveCard CreateChoiceCard(string title, IList<CardAction> choices, CardImage image = null, string version = "1.0")
        {
            var adaptiveCard = new AdaptiveCard();
            var body = new AdaptiveContainer() { Items = new List<AdaptiveElement>() };

            // Add Image
            if (image != null)
            {
                // Add Image
                body.Items.Add(AdaptiveElementBuilder.CreateImage(image, AdaptiveImageSize.Medium));
            }

            // Add Title
            body.Items.AddRange(AdaptiveElementBuilder.CreateTitle(title));

            // Set Body and Actions
            adaptiveCard.Body = new List<AdaptiveElement>() { body };
            adaptiveCard.Actions = AdaptiveElementBuilder.CreateActions(choices);
            adaptiveCard.Version = version;
            return adaptiveCard;
        }

        // Create Fact Card
        public static AdaptiveCard CreateFactListCard(string title, IList<Fact> facts, IList<CardAction> buttons = null, CardImage image = null, string version = "1.0")
        {
            var adaptiveCard = new AdaptiveCard();
            var body = new AdaptiveContainer() { Items = new List<AdaptiveElement>() };

            // Add Image
            if (image != null)
            {
                // Add Image
                body.Items.Add(AdaptiveElementBuilder.CreateImage(image, AdaptiveImageSize.Medium));
            }

            // Add Title
            body.Items.AddRange(AdaptiveElementBuilder.CreateTitle(title));

            // Add FaceSet
            body.Items.Add(AdaptiveElementBuilder.CreateFactSet(facts));

            // Set Body and Actions
            adaptiveCard.Body = new List<AdaptiveElement>() { body };
            adaptiveCard.Actions = AdaptiveElementBuilder.CreateActions(buttons);
            adaptiveCard.Version = version;
            return adaptiveCard;
        }

        // Create Image Card
        public static AdaptiveCard CreateImageCard(CardImage image, IList<CardAction> buttons = null, string version = "1.0")
        {
            var adaptiveCard = new AdaptiveCard();
            var body = new AdaptiveContainer() { Items = new List<AdaptiveElement>() };

            // Add Image
            if (image != null)
            {
                // Add Image
                body.Items.Add(AdaptiveElementBuilder.CreateImage(image, AdaptiveImageSize.Medium));
            }

            // Set Body and Actions
            adaptiveCard.Body = new List<AdaptiveElement>() { body };
            adaptiveCard.Actions = AdaptiveElementBuilder.CreateActions(buttons);
            adaptiveCard.Version = version;
            return adaptiveCard;
        }

        // Create ImageSet Card
        public static AdaptiveCard CreateImageSetCard(IList<CardImage> images, IList<CardAction> buttons = null, string version = "1.0")
        {
            var adaptiveCard = new AdaptiveCard();
            var body = new AdaptiveContainer() { Items = new List<AdaptiveElement>() };

            // Add Image
            if (images != null)
            {
                // Add Image
                body.Items.Add(AdaptiveElementBuilder.CreateImageSet(images, AdaptiveImageSize.Medium));
            }

            // Set Body and Actions
            adaptiveCard.Body = new List<AdaptiveElement>() { body };
            adaptiveCard.Actions = AdaptiveElementBuilder.CreateActions(buttons);
            adaptiveCard.Version = version;
            return adaptiveCard;
        }

        // Create List Card
        public static AdaptiveCard CreateListCard(IList<AdaptiveListCardItem> listItem, string title = null, IList<CardAction> buttons = null, AdaptiveListCardImageLayout imageLayout = AdaptiveListCardImageLayout.Right, AdaptiveImageSize imageSize = AdaptiveImageSize.Small, string version = "1.0")
        {
            var adaptiveCard = new AdaptiveCard();
            var body = new AdaptiveContainer() { Items = new List<AdaptiveElement>() };

            // Add Title
            if (!string.IsNullOrEmpty(title))
            {
                body.Items.AddRange(AdaptiveElementBuilder.CreateTitle(title, AdaptiveTextSize.Large));
            }

            // Add List Item
            body.Items.AddRange(CreateListItems(listItem, imageLayout, imageSize));

            // Set Body and Actions
            adaptiveCard.Body = new List<AdaptiveElement>() { body };
            adaptiveCard.Actions = AdaptiveElementBuilder.CreateActions(buttons);
            adaptiveCard.Version = version;
            return adaptiveCard;
        }

        #endregion


        #region Create Adaptive Card From Json String

        // Create AdaptiveCard from Json String
        public static AdaptiveCard CreateAdaptiveCard(string cardJson, Dictionary<string, object> data = null)
        {
            var card = AdaptiveCard.FromJson(cardJson).Card;
            if (data != null)
            {
                return card.BindData(data);
            }
            return card;
        }
        // Create AdaptiveCard from Json String
        public static AdaptiveCard CreateAdaptiveCard(string cardJson, JObject data)
        {
            var card = AdaptiveCard.FromJson(cardJson).Card;
            if (data != null)
            {
                return card.BindData(data);
            }
            return card;
        }
        // Create AdaptiveCard from Json String
        public static AdaptiveCard CreateAdaptiveCard<T>(string cardJson, T data)
        {
            var card = AdaptiveCard.FromJson(cardJson).Card;
            if (data != null)
            {
                return card.BindData(data);
            }
            return card;
        }

        // Read AdaptiveCard json file and create AdaptiveCard (Relative Path)
        public static AdaptiveCard CreateAdaptiveCardFromFilePath(string filePath, Dictionary<string, object> data = null)
        {
            var relativePath = HttpContext.Current.Server.MapPath(filePath);
            return CreateAdaptiveCardFromAbsoluteFilePath(relativePath, data);
        }
        // Read AdaptiveCard json file and create AdaptiveCard (Relative Path)
        public static AdaptiveCard CreateAdaptiveCardFromFilePath(string filePath, JObject data)
        {
            var relativePath = HttpContext.Current.Server.MapPath(filePath);
            return CreateAdaptiveCardFromAbsoluteFilePath(relativePath, data);
        }
        // Read AdaptiveCard json file and create AdaptiveCard (Relative Path)
        public static AdaptiveCard CreateAdaptiveCardFromFilePath<T>(string filePath, T data)
        {
            var relativePath = HttpContext.Current.Server.MapPath(filePath);
            return CreateAdaptiveCardFromAbsoluteFilePath(relativePath, data);
        }

        // Read AdaptiveCard json file and create AdaptiveCard (Absolute Path)
        public static AdaptiveCard CreateAdaptiveCardFromAbsoluteFilePath(string filePath, Dictionary<string, object> data = null)
        {
            using (StreamReader r = new StreamReader(filePath))
            {
                string cardJson = r.ReadToEnd();
                return CreateAdaptiveCard(cardJson, data);
            }
        }
        // Read AdaptiveCard json file and create AdaptiveCard (Absolute Path)
        public static AdaptiveCard CreateAdaptiveCardFromAbsoluteFilePath(string filePath, JObject data)
        {
            using (StreamReader r = new StreamReader(filePath))
            {
                string cardJson = r.ReadToEnd();
                return CreateAdaptiveCard(cardJson, data);
            }
        }
        // Read AdaptiveCard json file and create AdaptiveCard (Absolute Path)
        public static AdaptiveCard CreateAdaptiveCardFromAbsoluteFilePath<T>(string filePath, T data)
        {
            using (StreamReader r = new StreamReader(filePath))
            {
                string cardJson = r.ReadToEnd();
                return CreateAdaptiveCard(cardJson, data);
            }
        }

        #endregion


        #region Private Method - ReceiptCard only

        // Add Receipt Facts
        private static AdaptiveElement CreateReceiptFact(Fact fact, int factIndex)
        {
            var columns = new List<AdaptiveColumn>();

            if (!string.IsNullOrEmpty(fact.Key))
            {
                columns.Add(new AdaptiveColumn()
                {
                    Width = AdaptiveColumnWidth.Stretch.ToLower(),
                    Items = new List<AdaptiveElement>()
                    {
                        new AdaptiveTextBlock()
                        {
                            Id = $"ReceiptFact-{factIndex}-Label",
                            Text = fact.Key,
                            Size = AdaptiveTextSize.Medium,
                            Weight = AdaptiveTextWeight.Bolder,
                            HorizontalAlignment = AdaptiveHorizontalAlignment.Left
                        }
                    }
                });
            }

            if (!string.IsNullOrEmpty(fact.Value))
            {
                columns.Add(new AdaptiveColumn()
                {
                    Width = AdaptiveColumnWidth.Stretch.ToLower(),
                    Items = new List<AdaptiveElement>()
                    {
                        new AdaptiveTextBlock()
                        {
                            Id = $"ReceiptFact-{factIndex}",
                            Text = fact.Value,
                            Size = AdaptiveTextSize.Medium,
                            HorizontalAlignment = AdaptiveHorizontalAlignment.Right
                        }
                    }
                });
            }

            return new AdaptiveColumnSet()
            {
                Columns = columns
            };
        }
        private static List<AdaptiveElement> CreateReceiptFactSet(IList<Fact> facts)
        {
            var content = new List<AdaptiveElement>();

            for (var i = 0; i < facts.Count; i++)
            {
                var fact = facts[i];
                if (!string.IsNullOrEmpty(fact.Key) || !string.IsNullOrEmpty(fact.Value))
                {
                    content.Add(CreateReceiptFact(fact, i + 1));
                }
            }

            return content;
        }

        // Add Receipt Items
        private static AdaptiveElement CreateReceiptItem(ReceiptItem item, int itemIndex, AdaptiveImageSize imageSize = AdaptiveImageSize.Small)
        {
            var columns = new List<AdaptiveColumn>();
            // Add Receipt Item Image
            if (item.Image != null && item.Image.Url != null)
            {
                columns.Add(new AdaptiveColumn()
                {
                    Width = AdaptiveColumnWidth.Auto.ToLower(),
                    Items = new List<AdaptiveElement>()
                    {
                        AdaptiveElementBuilder.CreateImage(item.Image, imageSize, id: $"Item-{itemIndex}-Image")
                    }
                });
            }

            // Add Receipt Item Title and Subtitle
            if (!string.IsNullOrEmpty(item.Title) || !string.IsNullOrEmpty(item.Subtitle))
            {
                var title = (!string.IsNullOrEmpty(item.Title)) ? item.Title : string.Empty;
                var subtitle = (!string.IsNullOrEmpty(item.Subtitle)) ? item.Subtitle : string.Empty;
                columns.Add(new AdaptiveColumn()
                {
                    Width = AdaptiveColumnWidth.Auto.ToLower(),
                    Items = new List<AdaptiveElement>()
                    {
                        new AdaptiveTextBlock()
                        {
                            Text = title,
                            Id = $"Item-{itemIndex}-Title",
                            Size = AdaptiveTextSize.Medium,
                            Spacing = AdaptiveSpacing.None,
                            HorizontalAlignment = AdaptiveHorizontalAlignment.Left
                        },
                        new AdaptiveTextBlock()
                        {
                            Text = subtitle,
                            Id = $"Item-{itemIndex}-Subtitle",
                            Size = AdaptiveTextSize.Default,
                            Spacing = AdaptiveSpacing.None,
                            HorizontalAlignment = AdaptiveHorizontalAlignment.Left
                        }
                    }
                });
            }

            // Add Price
            if (!string.IsNullOrEmpty(item.Price))
            {
                columns.Add(new AdaptiveColumn()
                {
                    Width = AdaptiveColumnWidth.Stretch.ToLower(),
                    Items = new List<AdaptiveElement>()
                    {
                        new AdaptiveTextBlock()
                        {
                            Text = item.Price,
                            Id = $"Item-{itemIndex}-Price",
                            Size = AdaptiveTextSize.Medium,
                            HorizontalAlignment = AdaptiveHorizontalAlignment.Right
                        }
                    }
                });
            }

            return new AdaptiveColumnSet()
            {
                Columns = columns,
                SelectAction = AdaptiveElementBuilder.CreateAction(item.Tap)
            };
        }
        private static List<AdaptiveElement> CreateReceiptItems(List<ReceiptItem> items, AdaptiveImageSize imageSize = AdaptiveImageSize.Small)
        {
            var content = new List<AdaptiveElement>();

            for (int i = 0; i < items.Count; i++)
            {
                var item = items[i];
                content.Add(CreateReceiptItem(item, i + 1));
            }

            return content;
        }

        // Add Receipt Total
        private static AdaptiveElement CreateReceiptTotalInfo(string title, string value)
        {
            var columns = new List<AdaptiveColumn>();

            if (!string.IsNullOrEmpty(value))
            {
                columns.Add(new AdaptiveColumn()
                {
                    Width = AdaptiveColumnWidth.Stretch.ToLower(),
                    Items = new List<AdaptiveElement>()
                    {
                        new AdaptiveTextBlock()
                        {
                            Text = title,
                            Id = "Total-Label",
                            Size = AdaptiveTextSize.Medium,
                            Weight = AdaptiveTextWeight.Bolder,
                            HorizontalAlignment = AdaptiveHorizontalAlignment.Left
                        }
                    }
                });

                columns.Add(new AdaptiveColumn()
                {
                    Width = AdaptiveColumnWidth.Stretch.ToLower(),
                    Items = new List<AdaptiveElement>()
                    {
                        new AdaptiveTextBlock()
                        {
                            Text = value,
                            Id = "Total",
                            Size = AdaptiveTextSize.Medium,
                            Weight = AdaptiveTextWeight.Bolder,
                            HorizontalAlignment = AdaptiveHorizontalAlignment.Right
                        }
                    }
                });
            }

            return new AdaptiveColumnSet()
            {
                Columns = columns
            };
        }
        // Add Receipt VAT or Tax
        private static AdaptiveElement CreateReceiptOtherInfo(string title, string value, string type = "")
        {
            var columns = new List<AdaptiveColumn>();

            if (!string.IsNullOrEmpty(value))
            {
                columns.Add(new AdaptiveColumn()
                {
                    Width = AdaptiveColumnWidth.Stretch.ToLower(),
                    Items = new List<AdaptiveElement>()
                    {
                        new AdaptiveTextBlock()
                        {
                            Text = title,
                            Id = $"{type}-Label",
                            Size = AdaptiveTextSize.Medium,
                            Weight = AdaptiveTextWeight.Bolder,
                            HorizontalAlignment = AdaptiveHorizontalAlignment.Left
                        }
                    }
                });

                columns.Add(new AdaptiveColumn()
                {
                    Width = AdaptiveColumnWidth.Stretch.ToLower(),
                    Items = new List<AdaptiveElement>()
                    {
                        new AdaptiveTextBlock()
                        {
                            Text = value,
                            Id = type,
                            Size = AdaptiveTextSize.Medium,
                            HorizontalAlignment = AdaptiveHorizontalAlignment.Right
                        }
                    }
                });
            }

            return new AdaptiveColumnSet()
            {
                Columns = columns
            };
        }

        // Add Separator
        private static AdaptiveElement CreateReceiptSeparator()
        {
            return new AdaptiveColumnSet()
            {
                Columns = new List<AdaptiveColumn>()
                {
                    new AdaptiveColumn()
                    {
                        Items = new List<AdaptiveElement>()
                    }
                }
            };
        }

        #endregion


        #region Private Method - ListCard only

        // Add List Item
        private static AdaptiveElement CreateListItem(AdaptiveListCardItem item, int itemIndex, AdaptiveListCardImageLayout imageLayout, AdaptiveImageSize imageSize = AdaptiveImageSize.Small)
        {
            var columns = new List<AdaptiveColumn>();

            // Add Title, subtitile, text
            var itemContent = new AdaptiveColumn();

            if (!string.IsNullOrEmpty(item.Title))
            {
                itemContent.Items.Add(new AdaptiveTextBlock()
                {
                    Text = item.Title,
                    Id = $"Item-{itemIndex}-Title",
                    Size = AdaptiveTextSize.Medium,
                    Weight = AdaptiveTextWeight.Bolder,
                    Spacing = AdaptiveSpacing.None,
                    HorizontalAlignment = AdaptiveHorizontalAlignment.Left
                });
            }

            if (!string.IsNullOrEmpty(item.Subtitle))
            {
                itemContent.Items.Add(new AdaptiveTextBlock()
                {
                    Text = item.Subtitle,
                    Id = $"Item-{itemIndex}-Subtitle",
                    Size = AdaptiveTextSize.Default,
                    Spacing = AdaptiveSpacing.None,
                    IsSubtle = true,
                    HorizontalAlignment = AdaptiveHorizontalAlignment.Left
                });
            }

            if (!string.IsNullOrEmpty(item.Text))
            {
                itemContent.Items.Add(new AdaptiveTextBlock()
                {
                    Text = item.Text,
                    Id = $"Item-{itemIndex}-Text",
                    Size = AdaptiveTextSize.Small,
                    Spacing = AdaptiveSpacing.None,
                    Weight = AdaptiveTextWeight.Lighter,
                    IsSubtle = true,
                    HorizontalAlignment = AdaptiveHorizontalAlignment.Left
                });
            }

            // Add List Image
            var itemImage = new AdaptiveColumn()
            {
                Items = new List<AdaptiveElement>(),
                Width = AdaptiveColumnWidth.Auto.ToLower()
            };
            if (item.Image != null && !string.IsNullOrEmpty(item.Image.Url))
            {
                itemImage.Items.Add(AdaptiveElementBuilder.CreateImage(item.Image, imageSize));
            }

            // Set List Item
            switch (imageLayout)
            {
                case AdaptiveListCardImageLayout.Right:
                    columns.Add(itemContent);
                    columns.Add(itemImage);
                    break;
                case AdaptiveListCardImageLayout.Left:
                    columns.Add(itemImage);
                    columns.Add(itemContent);
                    break;
                case AdaptiveListCardImageLayout.None:
                default:
                    columns.Add(itemContent);
                    break;
            }

            return new AdaptiveColumnSet()
            {
                Separator = true,
                Columns = columns,
                SelectAction = AdaptiveElementBuilder.CreateAction(item.Tap)
            };
        }
        private static List<AdaptiveElement> CreateListItems(IList<AdaptiveListCardItem> items, AdaptiveListCardImageLayout imageLayout, AdaptiveImageSize imageSize = AdaptiveImageSize.Small)
        {
            var content = new List<AdaptiveElement>();

            for (int i = 0; i < items.Count; i++)
            {
                var item = items[i];
                content.Add(CreateListItem(item, i + 1, imageLayout, imageSize));
            }

            return content;
        }

        #endregion
    }

    [Serializable]
    public class AdaptiveElementBuilder
    {
        #region Title & Subtitle & Text

        // Create TextBlock
        public static List<AdaptiveElement> CreateTitle(string title, string subtitle, string text)
        {
            var content = new List<AdaptiveElement>();

            if (!string.IsNullOrEmpty(title))
            {
                content.Add(new AdaptiveTextBlock()
                {
                    Id = "Title",
                    Size = AdaptiveTextSize.Medium,
                    Wrap = true,
                    Weight = AdaptiveTextWeight.Bolder,
                    Text = title ?? string.Empty
                });
            }

            if (!string.IsNullOrEmpty(subtitle))
            {
                content.Add(new AdaptiveTextBlock()
                {
                    Id = "Subtitle",
                    Size = AdaptiveTextSize.Default,
                    Wrap = true,
                    Text = subtitle ?? string.Empty,
                    IsSubtle = true
                });
            }

            if (!string.IsNullOrEmpty(text))
            {
                content.Add(new AdaptiveTextBlock()
                {
                    Id = "Text",
                    Size = AdaptiveTextSize.Default,
                    Wrap = true,
                    Text = text ?? string.Empty
                });
            }
            return content;
        }
        // Create TextBlock
        public static List<AdaptiveElement> CreateTitle(string title, AdaptiveTextSize size = AdaptiveTextSize.Medium)
        {
            var content = new List<AdaptiveElement>();

            if (!string.IsNullOrEmpty(title))
            {
                content.Add(new AdaptiveTextBlock()
                {
                    Size = size,
                    Id = "Title",
                    Wrap = true,
                    Weight = AdaptiveTextWeight.Bolder,
                    Text = title ?? string.Empty
                });
            }

            return content;
        }


        #endregion


        #region Image & ImageSet

        // Create Image
        public static AdaptiveImage CreateImage(CardImage image, AdaptiveImageSize size = AdaptiveImageSize.Medium, string id = "Image")
        {
            if (image != null)
            {
                return new AdaptiveImage()
                {
                    Id = id ?? string.Empty,
                    Url = new Uri(image.Url),
                    Size = size,
                    AltText = image.Alt,
                    SelectAction = CreateAction(image.Tap)
                };
            }
            return null;
        }
        // Create Image
        public static AdaptiveImage CreateImage(MediaUrl image, AdaptiveImageSize size = AdaptiveImageSize.Medium, string id = "Image")
        {
            if (image != null)
            {
                return new AdaptiveImage()
                {
                    Id = id ?? string.Empty,
                    Url = new Uri(image.Url),
                    Size = size
                };
            }
            return null;
        }
        // Create Image
        public static AdaptiveImage CreateImage(IList<CardImage> cardImages, AdaptiveImageSize size = AdaptiveImageSize.Medium)
        {
            if (cardImages == null || cardImages.FirstOrDefault() == null)
            {
                return null;
            }

            var image = cardImages.FirstOrDefault();

            return CreateImage(image, size);
        }
        // Create Image
        public static AdaptiveImage CreateImage(IList<MediaUrl> cardImages, AdaptiveImageSize size = AdaptiveImageSize.Medium)
        {
            if (cardImages == null || cardImages.FirstOrDefault() == null)
            {
                return null;
            }

            var image = cardImages.FirstOrDefault();

            return CreateImage(image, size);
        }

        // Create ImageSet
        public static AdaptiveElement CreateImageSet(IList<CardImage> cardImages, AdaptiveImageSize size = AdaptiveImageSize.Medium)
        {
            var imageSet = new AdaptiveImageSet()
            {
                Id = "ImageSet",
                Images = new List<AdaptiveImage>()
            };

            if (cardImages == null || cardImages.Count == 0)
            {
                return null;
            }

            for (var i = 0; i < cardImages.Count; i++)
            {
                var cardImage = cardImages[i];
                imageSet.Images.Add(CreateImage(cardImage, size, $"Image-{i + 1}"));
            }

            return imageSet;
        }
        // Create ImageSet
        public static AdaptiveElement CreateImageSet(IList<MediaUrl> cardImages, AdaptiveImageSize size = AdaptiveImageSize.Medium)
        {
            var imageSet = new AdaptiveImageSet()
            {
                Id = "ImageSet",
                Images = new List<AdaptiveImage>()
            };

            if (cardImages == null || cardImages.Count == 0)
            {
                return null;
            }

            for (var i = 0; i < cardImages.Count; i++)
            {
                var cardImage = cardImages[i];
                imageSet.Images.Add(CreateImage(cardImage, size, $"Image-{i + 1}"));
            }

            return imageSet;
        }


        #endregion


        #region FactSet

        // Create Fact
        public static AdaptiveFact CreateFact(Fact fact)
        {
            if (fact == null)
            {
                return null;
            }
            return new AdaptiveFact(fact.Key, fact.Value);
        }

        // Create FactSet
        public static AdaptiveElement CreateFactSet(IList<Fact> facts)
        {
            var factSet = new AdaptiveFactSet()
            {
                Id = "FactSet",
                Facts = new List<AdaptiveFact>()
            };

            if (facts == null || facts.Count == 0)
            {
                return null;
            }

            foreach (var fact in facts)
            {
                factSet.Facts.Add(CreateFact(fact));
            }

            return factSet;
        }

        #endregion


        #region Actions

        // Create Action
        public static AdaptiveAction CreateAction(CardAction action)
        {
            if (action == null)
            {
                return null;
            }

            if (string.IsNullOrEmpty(action.DisplayText) &&
                string.IsNullOrEmpty(action.Title) &&
                string.IsNullOrEmpty(action.Text) &&
                action.Value == null)
            {
                return null;
            }

            var title = action.DisplayText ?? action.Title ?? action.Text;
            var value = (action.Value ?? action.DisplayText ?? action.Title ?? action.Text).ToString();

            switch (action.Type)
            {
                case ActionTypes.OpenUrl:
                case ActionTypes.Signin:
                case ActionTypes.DownloadFile:
                case ActionTypes.PlayAudio:
                case ActionTypes.PlayVideo:
                case ActionTypes.ShowImage:
                    return new AdaptiveOpenUrlAction()
                    {
                        Title = title,
                        Url = new Uri(value)
                    };
                case ActionTypes.ImBack:
                case ActionTypes.PostBack:
                case ActionTypes.MessageBack:
                    return new AdaptiveSubmitAction()
                    {
                        Title = title,
                        Data = value
                    };

                default:
                    return null;
            }
        }

        // Create Actions
        public static List<AdaptiveAction> CreateActions(IList<CardAction> buttons)
        {
            var actions = new List<AdaptiveAction>();

            if (buttons == null)
            {
                return actions;
            }

            foreach (var button in buttons)
            {
                var action = CreateAction(button);
                if (action != null)
                {
                    actions.Add(action);
                }
            }

            return actions;
        }

        #endregion
    }
}