using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.Core.Models
{
    public class AdaptiveCard
    {
        public string Type { get; set; } = "message";
        public List<Attachment> Attachments { get; } = new List<Attachment>();

        public Attachment AddAttachment()
        {
            var attachment = new Attachment();
            Attachments.Add(attachment);
            return attachment;
        }

        public class Attachment
        {
            public string ContentType { get; set; } = "application/vnd.microsoft.card.adaptive";
            public CardContent Content { get; set; } = new CardContent();
        }

        public class Recipient
        {
            public string Name { get; set; }
            public string Email { get; set; }
        }

        public class CardContent
        {
            public string Schema { get; set; } = "http://adaptivecards.io/schemas/adaptive-card.json";
            public string Version { get; set; } = "1.0";
            public string Type { get; set; } = "AdaptiveCard";
            public List<AdaptiveCardBlock> Body { get; set; } = new List<AdaptiveCardBlock>();
            public MSTeams Msteams { get; set; } = new MSTeams();

            private AdaptiveCardTextBlock _recipientBlock;

            public void AddRecipient(string name, string email)
            {
                if (_recipientBlock == null)
                {
                    _recipientBlock = new AdaptiveCardTextBlock();
                    Body.Insert(0, _recipientBlock);
                    _recipientBlock.Type = "TextBlock";
                    _recipientBlock.Wrap = true;
                    _recipientBlock.Text = $"<at>{name}</at>";
                }
                else
                {
                    _recipientBlock.Text += $", <at>{name}</at>";
                }

                Msteams.Add(email, name);
            }

            public void AddTextBlock(string text)
            {
                Body.Add(new AdaptiveCardTextBlock()
                {
                    Type = "TextBlock",
                    Text = text,
                    Wrap = true
                }); ;
            }

            public void AddLink(string text, string link)
            {
                Body.Add(new AdaptiveCardTextBlock()
                {
                    Type = "TextBlock",
                    Text = $"[{text}]({link})",
                    Wrap = true
                }); ;
            }
        }

        public class MSTeams
        {
            public List<Entity> Entities { get; set; } = new List<Entity>();

            public void Add(string email, string name)
            {
                var entity = new Entity();
                entity.Text = $"<at>{name}</at>";
                entity.Mentioned.Id = email;
                entity.Mentioned.Name = name;
                Entities.Add(entity);
            }

            public class Entity
            {
                public string Type { get; set; } = "mention";
                public string Text { get; set; }
                public MentionedEntity Mentioned { get; } = new MentionedEntity();

                public class MentionedEntity
                {
                    public string Id { get; set; }
                    public string Name { get; set; }
                }
            }
        }

        public class AdaptiveCardBlock
        {
            public string Type { get; set; }
        }

        public class AdaptiveCardTextBlock : AdaptiveCardBlock
        {
            public string Size { get; set; }
            public string Weight { get; set; }
            public string Text { get; set; }
            public bool Wrap { get; set; }
        }

        public string ToJSON()
        {
            return JsonConvert.SerializeObject(this, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore
            });
        }
    }
}
