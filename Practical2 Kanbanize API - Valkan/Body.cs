namespace Practical2_Kanbanize_API___Valkan
{
    public class Body
    {
        public int? lane_id { get; set; }
        public int? card_id { get; set; }
        public int? column_id { get; set; }
        public int? workflow_id { get; set; }
        public int? position { get; set; }
        public int? priority { get; set; }
        public int? size { get; set; }
        public int? subtask_id { get; set; }
        public string link_type { get; set; }
        public string title { get; set; }
        public string color { get; set; }
        public string description { get; set; }
        public string deadline { get; set; }
       
    }
    public class BodySizeDouble
    {
        public int? card_id { get; set; }
        public double size { get; set; }
        public int? lane_id { get; set; }
        public int? column_id { get; set; }
        public string title { get; set; }
    }
}