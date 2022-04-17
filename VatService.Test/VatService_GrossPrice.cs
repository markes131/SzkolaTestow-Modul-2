using NUnit.Framework;
using VatServices;
using System;
using FluentAssertions;
using Vat.Services;
using Moq;
using System.Collections.Generic;
using NSubstitute;
using Microsoft.Extensions.Logging;

namespace VatService.Test;

[TestFixture]
public class Tests
{

    VatServ _vatService;
    Dictionary<string, int> productTypes;
    double result;
    Mock<ILoggingService> mockLogger;

    [SetUp]
    public void Setup()
    {
        productTypes = new Dictionary<string, int>()
        {

            { "Food", 0},
            { "Electronic", 1},
            { "Books", 0},
            { "Furniture", 1}

        };

        mockLogger = new Mock<ILoggingService>();
    }


    [TestCase(0, 100, 0.23, /*Expected = */ 123d)]
    [TestCase(0, 200, 0.23, /*Expected = */ 246d)]
    [TestCase(0, 100, 0.08, /*Expected = */ 108d)]

    [Test]
    public void GivingProductWithNetPriceAndVatValue_ShouldReturnGrossPriceOfProduct(int index, double productNetPrice, double vatValue, double expected)
    {
        Product product = new(index, productNetPrice, "Books");
        
        this._vatService = new VatServ(vatValue, mockLogger.Object);

        result = _vatService.GrossPriceForDefaultVat(product);

        result.Should().Be(expected);
    }


    readonly Random random = new Random();

    [TestCase(100, "Food", 108d)]
    [TestCase(100, "Electronic", 123d)]
    [TestCase(100, "Books", 108d)]
    [TestCase(100, "Furniture", 123d)]

    [Test]
    public void GivingProductWithNetPriceViaVatProvider_ShouldReturnGrossPriceOfProductBasedOnProductType(double netPrice, string productType, double expected)
    {

        // Dla wersji z NSubstitute
        // IVatProvider _vatProvider;
        // _vatProvider = Substitute.For<IVatProvider>();

        //foreach (KeyValuePair<string, int> typePair in productTypes)
        //{
        //    if (typePair.Value.Equals(0))
        //    {
        //        _vatProvider.VatForType(typePair.Key).Returns(0.08);
        //    }
        //    else if (typePair.Value.Equals(1))
        //    {
        //        _vatProvider.VatForType(typePair.Key).Returns(0.23);
        //    }
        //    else
        //    {
        //        throw new ArgumentException("Wrong Value in Dictionary");
        //    }
        //}

        //given
        Product product = new Product(random.Next(), netPrice, productType);

        Mock<IVatProvider> mockVatProvider = new Mock<IVatProvider>();

        if (productTypes.GetValueOrDefault(productType) == 0)
            {
                mockVatProvider.Setup(x => x.VatForType(productType)).Returns(0.08);
            }
            else if (productTypes.GetValueOrDefault(productType) == 1)
            {
                mockVatProvider.Setup(x => x.VatForType(productType)).Returns(0.23);
            }
            else
            {
                throw new ArgumentException("Wrong Value in Dictionary");
            }

        // Dla wersji z list¹ zamiast Dictionary
        //List<string> productTypeWithLowVat = new List<string> { "Food", "Books" };

        //mockVatProvider.Setup(x => x.VatForType(productType)).Returns(() =>
        //{
        //    if (productTypeWithLowVat.Contains(product.ProductType))
        //    {
        //        return 0.08;
        //    }
        //    else
        //    {
        //        return 0.23;
        //    }
        //    return 0;
        //});

        // Dla wersji bez mockowania ILoggera
        //VatServ vatServWithMock = new VatServ(mockVatProvider.Object);

        mockLogger.Setup(log => log.Info(It.IsAny<string>())).Verifiable();

        _vatService = new VatServ(mockVatProvider.Object, mockLogger.Object);

        // dla wersji z NSubstitute
        //VatServ vatServWithMock = new VatServ(_vatProvider);

        //when 
        result = _vatService.GrossPriceForVatProvider(product);

        //assert przy wykorzystaniu FluentAssertion
        result.Should().Be(expected);

        // dla zwyk³ej assercji z NUnit
        // Assert.AreEqual(result, expected);

        // sprawdzenie czy logger wywo³a³ metodê Info z odpowiedni¹ wiadomoœci¹ dok³adnie jeden raz
        mockLogger.Verify(x => x.Info("Recieved VatValue"));
        mockLogger.Verify(x => x.Info("Calculated Gross Price for product"));
        mockLogger.Verify(x => x.Info("I am here"));
    }


    [TestCase(1.01)]
    [TestCase(1.05)]
    [TestCase(1.1)]
    [TestCase(3)]

    [Test]
    public void GivingProductWithVatAboveAcceptedValue_ShouldThrowArgumentOutOfRangeException(double vatValue)
    {
        Product product = new(0, 100);

        // dla wersji z Assert NUnit
        // Assert.Throws<ArgumentOutOfRangeException>(() => _vatService.GrossPriceForDefaultVat(product));

        mockLogger.Setup(log => log.Info(It.IsAny<string>())).Verifiable();
        mockLogger.Setup(log => log.Error(It.IsAny<string>())).Verifiable();

        _vatService = new VatServ(vatValue, mockLogger.Object);

        Action act = () => _vatService.GrossPriceForDefaultVat(product);


        // Wersja assert dla FluentAssertion

        act.Should().Throw<ArgumentOutOfRangeException>();

        mockLogger.Verify(log => log.Info("Gross Price For Default Vat"));
        mockLogger.Verify(log => log.Info("I am here"));
        mockLogger.Verify(log => log.Error("Jakis blad"));

    }


    [TestCase(-1)]
    [TestCase(-1000)]
    [TestCase(int.MinValue)]

    [Test]
    public void CreatingProductWithIdOutOfRange(int id)
    {
        Random randomId = new();
        Double randomValue = randomId.NextDouble();
        Product product;

        Action act = () => product = new Product(id, randomValue);

        // Wersja assert dla FluentAssertion
        act.Should().Throw<ArgumentOutOfRangeException>();

        // dla wersji z Assert NUnit
        // Assert.Throws<ArgumentOutOfRangeException>(() => product = new Product(id, randomValue));
    }
}