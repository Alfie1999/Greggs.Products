using Greggs.Products.Api.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Greggs.Products.Api.Business;

public interface IProductService
{
  Task<IEnumerable<Product>> ProductList(int? pageStart, int? pageSize);
  Task<IEnumerable<Product>> ProductPiceList(ProductExchangeRate productExchangeRate, int? pageStart, int? pageSize);
}