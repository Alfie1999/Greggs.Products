using Greggs.Products.Api.Business;
using Greggs.Products.Api.JWT;
using Greggs.Products.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Greggs.Products.Api.Controllers;
[Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)] 
[ApiController]
[Route("[controller]")]
public class ProductController : ControllerBase
{


  private readonly ILogger<ProductController> _logger;
  private readonly IProductService _productService;
  private readonly ProductExchangeRate _productExchangeRate;


  public ProductController(IProductService productService,
    IOptions<ProductExchangeRate> options, ILogger<ProductController> logger)
  {
    _productService = productService;
    _productExchangeRate = options.Value;
    _logger = logger;

  }
  /// <summary>
  /// Get Product list
  /// </summary>
  /// <param name="pageStart"></param>
  /// <param name="pageSize"></param>
  /// <returns></returns>
  [HttpGet]
  [AllowAnonymous]
  public async Task<ActionResult<IEnumerable<Product>>> Get(int pageStart = 0, int pageSize = 5) 
  {
    var items = await _productService.ProductList(pageStart, pageSize);
    _logger.LogInformation("ProductController Get called");
    return Ok(items);
  }

 /// <summary>
 /// Get Product Prices in Euros
 /// </summary>
 /// <param name="pageStart"></param>
 /// <param name="pageSize"></param>
 /// <returns></returns>
  [HttpGet("GetProductPrices")]
  public async Task<ActionResult<IEnumerable<ProductPrice>>> GetProductPrices(int pageStart = 0, int pageSize = 5)
  {

    var items = await _productService.ProductPiceList(_productExchangeRate, pageStart, pageSize);
    return Ok(items);
  }


}