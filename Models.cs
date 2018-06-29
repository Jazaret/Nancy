namespace NancyApplication {
    
    using Newtonsoft.Json;

    public class Account 
    {
        [JsonProperty(PropertyName = "id")]
        public string ID {get; set;}
        public string AccountName {get;set;}
        /// <summary>
        /// Ignore password field when returned to JSON
        /// </summary>
        [JsonIgnore]
        public string AccountPassword {get;set;}
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
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
        public string ID { get; set; }
        public string TopicID {get;set;}
        public string AccountID {get;set;}
        public string ConfirmationToken { get; set; }

        public bool SubscriptionConfirmed { get; set; }
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }        
    }


}