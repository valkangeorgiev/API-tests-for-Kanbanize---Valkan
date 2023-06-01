using Newtonsoft.Json;

namespace Practical2_Kanbanize_API___Valkan
{
    public class ErrorResponse<T>
    {
        [JsonProperty("erorr")]
        public ErrorBox<T> error { get; set; }
    }
    public class ErrorBox<T>
    {
        public string message { get; set; }

        public Details<T>details { get; set; }
    }
    public class Details<T>
    {

        [JsonProperty("lane_id")]
        public T lane_id { get; set; }

        [JsonProperty("column_id")]
        public T column_id { get; set; }

        [JsonProperty("workflow_id")]
        public T workflow_id { get; set; }

        [JsonProperty("position")]
        public T position { get; set; }

        [JsonProperty("title")]
        public T title { get; set; }

        [JsonProperty("color")]
        public T color { get; set; }

        [JsonProperty("priority")]
        public T priority { get; set; }

        [JsonProperty("description")]
        public T description { get; set; }

        [JsonProperty("owner_user_id")]
        public T owner_user_id { get; set; }

        [JsonProperty("deadline")]
        public T deadline { get; set; }

        [JsonProperty("size")]
        public T size { get; set; }

        [JsonProperty("subtasks_to_convert_into_cards")]
        public Items<T>subtasks_to_convert_into_cards { get; set; }
    }

    public class Items<T>
    {
        [JsonProperty("items")]
        public Zero<T> items { get; set; }

    }

    public class Zero<T>
    {

        [JsonProperty("0")]
        public LinkToExistingCard<T> zero { get; set; }

    }
    public class LinkToExistingCard<T>
    {

        [JsonProperty("links_to_existing_cards_to_add_or_update")]
        public linkPropertyItem <T> links_to_existing_cards_to_add_or_update { get; set; }
    }
    public class linkPropertyItem<T>
    {
        [JsonProperty("items")]
        public linkPropertyZero<T> items { get; set; }

    }
    public class linkPropertyZero<T>
    {

        [JsonProperty("0")]
        public linkPropertyLinkType <T> zero { get; set; }

    }
    public class linkPropertyLinkType<T>
    {

        [JsonProperty("link_type")]
        public T link_type { get; set; }

    }


}