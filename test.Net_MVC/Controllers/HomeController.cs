using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Diagnostics;
using test.Net_MVC.Models;
using MongoDB.Driver;
using test.Net_MVC.Services;
using MongoDB.Bson.Serialization;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using MongoDB.Bson.Serialization.Serializers;

namespace test.Net_MVC.Controllers
{
    
    public class HomeController : Controller
    {

        private readonly DbService _dbService;

        public HomeController(DbService dbService) =>
        _dbService = dbService;

        [HttpGet]
        /*public async Task<List<Document>> Get() =>
        await _dbService.GetAsync();*/


        public ViewResult Index(string folder = "Creating Digital Images")
        {

            ViewBag.folder = folder;
            ViewBag.references = new List<Reference>();

            
            foreach (string collection in _dbService.collections)
            {
                ViewBag.references.Add(new Reference() { name = collection, href =$"/Home/GetDocuments?collection={collection}" }) ; 
            }


            return View();
        }

        public ViewResult GetDocuments(string collection )
        {
            ViewBag.folder = collection;
            ViewBag.references=new List<Reference>();
            foreach(Document d in _dbService.getDocuments(collection))
            {
                ViewBag.references.Add(new Reference() { name = $"{d.name}", href = $"/Home/GetInnerDocuments?name={d.name}&inside={d.inside}" });
            }
            return View("Index");
        }

        public ViewResult GetInnerDocuments(Document d)
        {

            ViewBag.folder = d.name;
            ViewBag.references = new List<Reference>();
            Document[] inside = d.inside != null ? d.inside : new Document[0];
            if (inside.Length == 0)
            {
                return View("Index");
            }

            foreach (Document i in inside)
            {
                ViewBag.references.Add(new Reference() { name = $"{i.name}", href = $"/Home/GetInnerDocuments?name={i.name}&inside={i.inside}" });
            }
            return View("Index");
        }

        public RedirectResult Import(string file, string fileForExport)
        {
            file = file.Split(".")[0];
            _dbService.exportDB(fileForExport);

            string text = System.IO.File.ReadAllText($"C:\\Users\\AdministratoR\\Desktop\\{file}.json");
            BsonDocument dbObj = BsonSerializer.Deserialize<BsonDocument>(text);
            foreach(BsonElement collection in dbObj.ToList())
            {
                List<Document> documents = BsonSerializer.Deserialize<List<Document>>(collection.Value.ToJson());
                Console.Write(documents.ToJson());
                _dbService.createCollection(collection.Name, documents);
            }
           

            return Redirect($"~/Home/Index?folder={file}");

        }

        

    }
}