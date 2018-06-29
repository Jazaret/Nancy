namespace NancyApplication {
    
    using Newtonsoft.Json;

    public class Account 
    {
        [JsonProperty(PropertyName = "id")]
        public string ID {get; set;}
        public string AccountName {get;set;}
        public string DisplayName {get;set;}
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
        public string TopicID {get;set;}
        public string AccountID {get;set;}
    }


}