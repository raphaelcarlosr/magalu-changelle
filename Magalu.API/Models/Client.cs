using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Magalu.API.Models
{
    [Serializable]
    public class Client
    {
        public Client()
        {
        }

        public Client(string email, string name)
        {
            Email = email ?? throw new ArgumentException("informe o email");
            Name = name ?? throw new ArgumentException("informe o nome");
        }

        [BsonRepresentation(BsonType.ObjectId)]
        [BsonId]
        public string Id { get; set; }

        public string Email { get; set; }
        public string Name { get; set; }

        public IEnumerable<Product> Favorites {get; set;}
    }
}
