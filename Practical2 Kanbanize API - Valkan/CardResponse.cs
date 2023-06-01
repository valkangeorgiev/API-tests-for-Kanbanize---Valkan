using Newtonsoft.Json;

namespace Practical2_Kanbanize_API___Valkan
{ 
        public class CardResponse
        {
            [JsonProperty("data")]
            public List<Body> data { get; set; }
  
        }

        public class SizeResponse
        { 
            [JsonProperty("data")]
             public List<BodySizeDouble> data { get; set; }
        }

        public class CardResponseObject
        {
            [JsonProperty("data")]
            public Body data { get; set; }
        }
    
}