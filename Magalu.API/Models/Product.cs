using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Magalu.API.Models
{
    [Serializable]
    public class Product
    {
        public Product()
        {
        }

        public string Id { get; set; }
        public double Price { get; set; }
        public string Image { get; set; }
        public string Brand { get; set; }
        public string Title { get; set; }
    }
}
