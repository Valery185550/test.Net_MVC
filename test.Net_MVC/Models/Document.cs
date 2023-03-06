using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;
using ThirdParty.Json.LitJson;

namespace test.Net_MVC.Models

{
    [BsonIgnoreExtraElements]
    public class Document
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        
        public string name { get; set; } = null!;

       
        public Document[] inside { get; set; } = null!;
    }
}
