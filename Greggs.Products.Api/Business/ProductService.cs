using Greggs.Products.Api.DataAccess;
using Greggs.Products.Api.Models;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Greggs.Products.Api.Business;

/// <summary>
/// Products business class
/// </summary>
public class ProductService : IProductService
{
  private readonly ILogger<ProductService> _logger;
  private readonly IDataAccess<Product> _productAccess;

  public ProductService(IDataAccess<Product> productAccess, ILogger<ProductService> logger)
  {
    _productAccess = productAccess;
    _logger = logger;
  }

  public async Task<IEnumerable<Product>> ProductList(int? pageStart, int? pageSize)
  {
    _logger.LogInformation("ProductService:List");
    return await Task.Run(() => _productAccess.List(pageStart, pageSize));
  }

  public async Task<IEnumerable<Product>> ProductPiceList(ProductExchangeRate productExchangeRate, int? pageStart, int? pageSize)
  {
    _logger.LogInformation("ProductPiceList:List");
    var priceList = new List<ProductPrice>();
    var productList = await Task.Run(() => _productAccess.List(pageStart, pageSize));
    foreach (var record in productList)
    {
      priceList.Add(new ProductPrice
      {
        Name = record.Name,
        PriceInPounds = record.PriceInPounds,
        PriceInEuros = (record.PriceInPounds * productExchangeRate.Euros)
      });
    }
    return priceList;
  }
}

