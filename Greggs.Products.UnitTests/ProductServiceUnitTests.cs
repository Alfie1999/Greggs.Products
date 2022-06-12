using FluentAssertions;
using Greggs.Products.Api.Business;
using Greggs.Products.Api.DataAccess;
using Greggs.Products.Api.Models;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Greggs.Products.UnitTests;

public class ProductServiceUnitTests
{


  [Theory]
  [InlineData(0.5)] 
  [InlineData(1.09)]
  [InlineData(1.11)]
  [InlineData(2.22)]
  [InlineData(3.35)]
  public void Pass_GetProductPrices_Returns_Price_In_Euros(decimal eurosExchangeRate)
  {
    //Arrange
    Mock<ILogger<ProductService>> mockLogger = new();
    Mock<IDataAccess<Product>> mockProductAccess = new();

    IEnumerable<Product> ProductDatabase = new List<Product>()
          {
              new() { Name = "Sausage Roll", PriceInPounds = 1m },
              new() { Name = "Vegan Sausage Roll", PriceInPounds = 1.1m },
              new() { Name = "Steak Bake", PriceInPounds = 1.2m },
              new() { Name = "Yum Yum", PriceInPounds = 0.7m },
              new() { Name = "Pink Jammie", PriceInPounds = 0.5m },
              new() { Name = "Mexican Baguette", PriceInPounds = 2.1m },
              new() { Name = "Bacon Sandwich", PriceInPounds = 1.95m },
              new() { Name = "Coca Cola", PriceInPounds = 1.2m }
          };

    mockProductAccess.Setup(d => d.List(It.IsAny<int>(), It.IsAny<int>()))
      .ReturnsAsync(ProductDatabase);

    //Act
    var productSercice = new ProductService(mockProductAccess.Object, mockLogger.Object);
    ProductExchangeRate produceListExchangeRate = new ProductExchangeRate
    {
      Euros = eurosExchangeRate
    };
    var items = productSercice.ProductPiceList(produceListExchangeRate, 8, 8)
      .Result.ToList();
    //Assert

    items.Should().NotBeNull();
    items.Count.Should().Be(8);

    items[0].Name.Should().Be("Sausage Roll");
    items[0].PriceInPounds.Should().Be(1m);

    items[3].Name.Should().Be("Yum Yum");
    items[3].PriceInPounds.Should().Be(0.7m);

    items[6].Name.Should().Be("Bacon Sandwich");
    items[6].PriceInPounds.Should().Be(1.95m);


    foreach (ProductPrice item in items)
    {
      item.PriceInEuros.Should().Be(item.PriceInPounds * eurosExchangeRate);
    }


    mockLogger.Verify(
              m => m.Log(
                  LogLevel.Information,
                  It.IsAny<EventId>(),
                  It.IsAny<It.IsAnyType>(),
                  null,
                  It.IsAny<Func<It.IsAnyType, Exception, string>>()),
              Times.Once);

  }
  [Fact]
  public void Fail_GetProductPrices_Exchangerate_Is_0()
  {
    //Arrange
    Mock<ILogger<ProductService>> mockLogger = new();
    Mock<IDataAccess<Product>> mockProductAccess = new();

    //Act
    var productSercice = new ProductService(mockProductAccess.Object, mockLogger.Object);
    ProductExchangeRate produceListExchangeRate = new ProductExchangeRate
    {
      Euros = 0
    };
    var items = productSercice.ProductPiceList(produceListExchangeRate, 8, 8)
      .Result;
    //Assert

    items.Should().BeNull();

    mockLogger.Verify(
              m => m.Log(
                  LogLevel.Critical,
                  It.IsAny<EventId>(),
                  It.IsAny<It.IsAnyType>(),
                  null,
                  It.IsAny<Func<It.IsAnyType, Exception, string>>()),
              Times.Once);

  }

  [Fact]
  public void Fail_GetProductPrices_Exchangerate_Is_Null()
  {
    //Arrange
    Mock<ILogger<ProductService>> mockLogger = new();
    Mock<IDataAccess<Product>> mockProductAccess = new();

    //Act
    var productSercice = new ProductService(mockProductAccess.Object, mockLogger.Object);    
    var items = productSercice.ProductPiceList(null, 8, 8)
      .Result;
    //Assert

    items.Should().BeNull();

    mockLogger.Verify(
              m => m.Log(
                  LogLevel.Critical,
                  It.IsAny<EventId>(),
                  It.IsAny<It.IsAnyType>(),
                  null,
                  It.IsAny<Func<It.IsAnyType, Exception, string>>()),
              Times.Once);

  }

}
