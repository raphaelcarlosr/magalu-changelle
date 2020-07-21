using Magalu.API.Data;
using Magalu.API.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Magalu.API.Services
{
    public class ClientService
    {
        private ApplicationDbContext DbContext { get; set; }
        private IMongoCollection<Client> Catalog { get; set; }
        private ProductService ProductService { get; set; }
        public ClientService(ApplicationDbContext applicationDbContext, ProductService productService)
        {
            DbContext = applicationDbContext;
            Catalog = DbContext.Collection<Client>();
            ProductService = productService ?? throw new ArgumentException("Serviço do produto não disponível.");
        }

        public Task<Client> GetByEmail(string email)
        {
            var filter = Builders<Client>.Filter.Eq(nameof(Client.Email), email);
            return Catalog.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<List<Client>> GetAll()
        {
            return await Catalog.Find<Client>(Builders<Client>.Filter.Empty).ToListAsync();
        }
        
        public async Task Create(string email, string name)
        {
            bool exists = await GetByEmail(email) != null;
            if (exists) throw new InvalidProgramException("O Email informado já existe");

            await Catalog.InsertOneAsync(new Client(email, name));
        }
        public async Task Update(Client client)
        {
            Client originalEntity = await GetByEmail(client.Email) ?? throw new InvalidOperationException("Cliente inexistente");

            client.Id = originalEntity.Id;
            var filter = Builders<Client>.Filter.Eq(nameof(Client.Id), originalEntity.Id);
            await Catalog.ReplaceOneAsync(filter, client);
        }
        public async Task Delete(string email)
        {
            var filter = Builders<Client>.Filter.Eq(nameof(Client.Email), email);
            await Catalog.DeleteOneAsync(filter);
        }

        public async Task<Product> AddProduct(string email, string productId )
        {
            //TODO product api dont return product by id
            IEnumerable<Product> products = await ProductService.GetAll(1) ?? throw new InvalidOperationException("Produto não encontrado");

            //Um produto não pode ser adicionado em uma lista caso ele não exista            
            Product product = products.FirstOrDefault(p => p.Id == productId) ?? throw new InvalidOperationException("Produto não encontrado");

            //Um produto não pode estar duplicado na lista de produtos favoritos de um cliente
            Client client = await GetByEmail(email) ?? throw new InvalidOperationException("Cliente não encontrado");
            if (client.Favorites?.Any(favorite => favorite.Id == product.Id) == true)
                throw new InvalidOperationException("O produto ja é um favorito");

            List<Product> favorites = new List<Product>() { product };
            if (client.Favorites == null) client.Favorites = favorites;
            else client.Favorites.Union(favorites);

            var filter = Builders<Client>.Filter.Eq(nameof(Client.Id), client.Id);
            await Catalog.ReplaceOneAsync(filter, client);

            return product;
        }

        public async Task<Product> RemoveProduct(string email, string productId)
        {
            //TODO product api dont return product by id
            IEnumerable<Product> products = await ProductService.GetAll(1) ?? throw new InvalidOperationException("Produto não encontrado");

            //Um produto não pode ser adicionado em uma lista caso ele não exista            
            Product product = products.FirstOrDefault(p => p.Id == productId) ?? throw new InvalidOperationException("Produto não encontrado");

            //Um produto não pode estar duplicado na lista de produtos favoritos de um cliente
            Client client = await GetByEmail(email);
            if (client.Favorites?.Any(favorite => favorite.Id == product.Id) == false)
                throw new InvalidOperationException("O produto não nos favoritos");

            client.Favorites = client.Favorites ?? new List<Product>().AsEnumerable();
            client.Favorites = client.Favorites.Where(p => p.Id != product.Id);

            var filter = Builders<Client>.Filter.Eq(nameof(Client.Id), client.Id);
            await Catalog.ReplaceOneAsync(filter, client);

            return product;
        }
    }
}
