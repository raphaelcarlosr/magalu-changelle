using Magalu.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http;

namespace Magalu.API.Services
{
    public class ProductService
    {
        private readonly string BaseUrl = "http://challenge-api.luizalabs.com/api/product";
        public async Task<IEnumerable<Product>> GetAll(int page = 1)
        {
            try
            {
                return await BaseUrl
                    .SetQueryParams(new { page = page })
                    .GetJsonAsync<Product[]>();
            }
            catch (Exception ex)
            {
                throw new InvalidProgramException("Erro ao consutar o produto", ex);
            }
        }

        public async Task<Product> Get(string id)
        {
            try
            {
                return await $"{BaseUrl}/{id}".GetJsonAsync<Product>();
            }
            catch (Exception ex)
            {
                throw new InvalidProgramException("Erro ao consutar o produto", ex);
            }
            
        }
    }
}
