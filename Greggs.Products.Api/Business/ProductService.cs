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
  /// <summary>
  /// returns a list of Product items and their prices in Pounds
  /// </summary>
  /// <param name="pageStart"></param>
  /// <param name="pageSize"></param>
  /// <returns></returns>
  public async Task<IEnumerable<Product>> ProductList(int? pageStart, int? pageSize)
  {
    _logger.LogInformation("ProductService:List");
    return await Task.Run(() => _productAccess.List(pageStart, pageSize));
  }
  /// <summary>
  /// returns a list of Product items and their prices in Pounds and Euros.
  /// DISCLAIMER: doesn't format to any decimal places as front end can display for it's culture etc
  /// </summary>
  /// <param name="productExchangeRate"></param>
  /// <param name="pageStart"></param>
  /// <param name="pageSize"></param>
  /// <returns></returns>
  public async Task<IEnumerable<ProductPrice>> ProductPiceList(ProductExchangeRate productExchangeRate, int? pageStart, int? pageSize)
  {
    if(productExchangeRate is null || productExchangeRate.Euros == 0)
    {
      _logger.LogCritical("ProductPiceList:List productExchangeRate is null or set to 0");
      return null;
    }
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

