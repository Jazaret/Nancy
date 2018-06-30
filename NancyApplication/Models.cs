namespace NancyApplication {
    using System;
    using Newtonsoft.Json;

    public class Account 
    {
        [JsonProperty(PropertyName = "id")]
        public string Id {get; set;}
        public string Name {get;set;}
        public string Password {get;set;}
        public Account() {}
        public Account(string id, string name, string password)
        {
            this.Id = id;
            this.Name = name;
            this.Password = password;
        }
        public Account(string name, string password) {
            this.Id = Guid.NewGuid().ToString();
            this.Name = name;
            this.Password = password;
        }
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }        
        public bool IsValid() {
            if (string.IsNullOrWhiteSpace(Id)) {
                return false;
            }
            if (string.IsNullOrWhiteSpace(Name)) {
                return false;
            }
            if (string.IsNullOrWhiteSpace(Password)) {
                return false;
            }
            return true;
        }
        public void ReplaceWith(Account newAccount) {
            this.Name = newAccount.Name;
            this.Password = newAccount.Password;
        }

    }

    public class Topic
    {
        [JsonProperty(PropertyName = "id")]
        public string ID { get; set; }
        public string Name { get; set; }
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

    public class Subscription
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        public string TopicID {get;set;}
        public string AccountID {get;set;}
        public string ConfirmationToken { get; set; }

        public bool SubscriptionConfirmed { get; set; }
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
        public Subscription() {}
        public Subscription(string id, string topicID, string accountID, string confirmationToken, bool confirmed)
        {
            this.Id = id;
            this.TopicID = topicID;
            this.AccountID = accountID;
            this.ConfirmationToken = confirmationToken;
            this.SubscriptionConfirmed = confirmed;
        }
        public Subscription(string topicID, string accountID)
        {
            this.Id = Guid.NewGuid().ToString();
            this.TopicID = topicID;
            this.AccountID = accountID;
            this.ConfirmationToken = Guid.NewGuid().ToString();
            this.SubscriptionConfirmed = false;
        }
        /// <summary>
        /// Updates the data from existing subscription to a new one. 
        /// Currently only reason is to change the subscription confirmed status.
        /// Keeps busines logic outside of storage repository
        /// </summary>
        public void ReplaceWith(Subscription newSubscription) {
            this.SubscriptionConfirmed = newSubscription.SubscriptionConfirmed;
        }
    }

    public enum ActionResult {
        Success,
        DoesNotExist,
        ActionNotRequired,
        ConcurrencyViolation,
        Error
    }


}