using Greggs.Products.Api.JWT;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Greggs.Products.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class HomeController : ControllerBase
{
  private readonly ILogger<ProductController> _logger;
  private readonly IJwtAuthenticationManager _jwtAuthenticationManager;


  public HomeController(IJwtAuthenticationManager jwtAuthenticationManager, ILogger<ProductController> logger)
  {
    _jwtAuthenticationManager = jwtAuthenticationManager;
    _logger = logger;

  }

  /// <summary>
  /// Authenticate Greggs Entrepreneur
  /// </summary>
  /// <param name="userCred"></param>
  /// <returns></returns>
  [AllowAnonymous]
  [HttpPost("authenticate")]
  public IActionResult Authenticate([FromBody] UserCred userCred)
  {
    var token = _jwtAuthenticationManager.Authenticate(userCred.Username, userCred.Password);
    if (token == null)
    {
      _logger.LogInformation("token not generated");
      return Unauthorized();
    }
    return Ok(token);
  }

}