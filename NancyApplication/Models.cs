namespace NancyApplication {
    using System;
    using Newtonsoft.Json;

    public class Account 
    {
        [JsonProperty(PropertyName = "id")]
        public string Id {get; set;}
        public string Name {get;set;}
        [JsonProperty(PropertyName = "_etag")]
        public string ETag { get; set; }
        public string Password {get;set;}
        public Account(string id, string name, string password)
        {
            Id = id;
            Name = name;
            Password = password;
        }
        public Account(string name, string password) {
            Id = Guid.NewGuid().ToString();
            Name = name;
            Password = password;
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
        public string ETag { get; set; }
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
        public Subscription(string id, string topicID, string accountID, string confirmationToken, bool confirmed)
        {
            Id = id;
            TopicID = topicID;
            AccountID = accountID;
            ConfirmationToken = confirmationToken;
            SubscriptionConfirmed = confirmed;
        }
        public Subscription(string topicID, string accountID)
        {
            Id = Guid.NewGuid().ToString();
            TopicID = topicID;
            AccountID = accountID;
            ConfirmationToken = Guid.NewGuid().ToString();
            SubscriptionConfirmed = false;
        }        
    }


}