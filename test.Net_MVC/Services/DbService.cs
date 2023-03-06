using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System.Text.Json;
using System.Text.Json.Nodes;
using test.Net_MVC.Models;

namespace test.Net_MVC.Services
{
    public class DbService
    {
        private IMongoDatabase dataBase { get; set; } = null!;

        public List<string> collections { get; private set; } = null!;


        public DbService() {
            connect("CreatingDigitalImages");
        }
        public bool connect (string db, string connectionString = "mongodb://localhost:27017")
        {
            try
            {
                var mongoClient = new MongoClient(connectionString);
                dataBase = mongoClient.GetDatabase(db);
                collections = dataBase.ListCollectionNames().ToList();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public List<Document> getDocuments (string collectionStr)
        {
            IMongoCollection<Document> collection = dataBase.GetCollection<Document>(collectionStr);
            List<Document> documents = collection.Find(_ => true).ToList<Document>();
            return documents;

        }
        public bool createCollection(string name, List <Document> documents)
        {
            try
            {
                dataBase.CreateCollection(name);
                IMongoCollection<Document> collection = dataBase.GetCollection<Document>(name);
                collection.InsertMany(documents);
                return true;
            }

            catch (Exception ex) { return false; }
            
        }

        public void exportDB (string fileForExport)
        {
            Dictionary<string, List<Document>> d = new Dictionary<string, List<Document>>();

            foreach (string collection in collections)
            {
                List<Document> documents = getDocuments(collection);
                d.Add(collection, documents);
                dataBase.DropCollection(collection);
            }
            
            
            File.WriteAllText($"C:\\Users\\AdministratoR\\Desktop\\{fileForExport}.json", JsonSerializer.Serialize(d));
        }
    }
}
