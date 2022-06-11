namespace Greggs.Products.Api.JWT;

public interface IJwtAuthenticationManager
{
  string Authenticate(string username, string password);
}
