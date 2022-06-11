using FluentAssertions;
using Greggs.Products.Api.JWT;
using System;
using Xunit;

namespace Greggs.Products.UnitTests;

public class AuthenticationUnitTests
{
  [Fact]
  public void Pass_Authentication_Returns_Token()
  {
    //Arrange
    var username = "test1";
    var password = "password";
    var key = "Test Key One is going to work just fine";

    //Act
    var authenticate = new JwtAuthenticationManager(key);
    var token = authenticate.Authenticate(username, password);

    //Assert
    token.Should().NotBeNull();
    token.Should().BeOfType<string>();
    token.Length.Should().BeGreaterThan(10);
  }

  [Theory]
  [InlineData(16)]
  [InlineData(50)]
  [InlineData(100)]
  [InlineData(200)]
  public void Pass_Authentication_Test_Key_size_Returns_Token(int keyLength)
  {
    //Arrange
    var username = "test1";
    var password = "password";
    var key =  new string('*', keyLength); ;

    //Act
    var authenticate = new JwtAuthenticationManager(key);
    var token = authenticate.Authenticate(username, password);

    //Assert
    token.Should().NotBeNull();
    token.Should().BeOfType<string>();
    token.Length.Should().BeGreaterThan(10);
  }

  [Theory]
  [InlineData(1)]
  [InlineData(5)]
  [InlineData(10)]
  [InlineData(15)]
  public void Fail_Authentication_Inline_Key_sizs_Throws_ArgumentOutOfRange_Exception(int keyLength)
  {
    //Arrange
    var username = "test1";
    var password = "password";
    var key = new string('*', keyLength); ;

    //Act & assert
    var authenticate = new JwtAuthenticationManager(key);
    Assert.Throws<ArgumentOutOfRangeException>(() => authenticate.Authenticate(username, password));
  }

  [Fact]
  public void Fail_Authentication_Empty_Key_Returns_Null()
  {
    //Arrange
    var username = "test1";
    var password = "password";
    var key = string.Empty;

    //Act 
    var authenticate = new JwtAuthenticationManager(key);
    var token = authenticate.Authenticate(username, password);

    //Assert
    token.Should().BeNull();

  }

  [Fact]
  public void Fail_Authentication_Key_Size_Throws_ArgumentOutOfRange_Exception()
  {
    //Arrange
    var username = "test1";
    var password = "password";
    var key = "Test";

    //Act & assert
    var authenticate = new JwtAuthenticationManager(key);
    Action act = () => authenticate.Authenticate(username, password);
    act.Should().Throw<ArgumentOutOfRangeException>();
    //.WithMessage("The encryption");
  }

  [Theory]
  [InlineData("John", "pword")]
  [InlineData("John", "password")]
  [InlineData("user1", "password2")]
  [InlineData("user2", "password")]
  public void Fail_Authentication_With_Bad_User_Credentials
    (string username, string password)
  {
    //Arrange
    var key = "Test Key One is going to work just fine";

    //Act
    var authenticate = new JwtAuthenticationManager(key);
    var token = authenticate.Authenticate(username, password);

    //Assert
    token.Should().BeNull();
  }
}