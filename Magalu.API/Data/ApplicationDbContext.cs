using System.Linq;
using MongoDB.Driver;
using Microsoft.Extensions.Configuration;
using System.Security.Policy;
using System.Collections.Generic;

namespace Magalu.API.Data
{
    public class ApplicationDbContext
    {
        private readonly IConfiguration Configuration;

        private string ConnectionString => Configuration.GetConnectionString("ConexaoCatalogo");
        private MongoClient ClientInstance { get; set; } = null;
        public MongoClient Client => ClientInstance ?? (ClientInstance = new MongoClient(new MongoUrl(ConnectionString)));
        public IMongoDatabase Database => Client.GetDatabase("magalu-changelle");
        private MongoClientSettings Settings => new MongoClientSettings()
        {
            Server = new MongoServerAddress(ConnectionString),
            AllowInsecureTls = true,
            ConnectionMode = ConnectionMode.Automatic,
            RetryReads = false,
            RetryWrites = false
        };

        public ApplicationDbContext(IConfiguration config)
        {
            Configuration = config;
        }

        public IMongoCollection<T> Collection<T>(string name = null)
        {
            return Database.GetCollection<T>(name ?? typeof(T).Name);
        }
    }
}
