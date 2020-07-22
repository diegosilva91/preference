namespace preference.Models
{
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;    
    public class Todo
    {
        [BsonId]
        public ObjectId InternalId { get; set; }
        public long Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Severity { get; set; }
        public string Status { get; set; }
    }
}