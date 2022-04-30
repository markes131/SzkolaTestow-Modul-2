
using Microsoft.Extensions.Logging;
using Vat.Services;

namespace VatServices;

public class VatServ
{
    public double VatValue { get; set; }
    private double _defaultVatValue = 0.23;

    readonly IVatProvider _vatProvider;

    private ILoggingService _logger;

    public VatServ(ILoggingService logger)
    {
        this.VatValue = _defaultVatValue;
        this._logger = logger;
    }

    public VatServ(double vatValue, ILoggingService logger)
    {
        this.VatValue = vatValue;
        this._logger = logger;
    }

    /*    public VatServ(IVatProvider vatProvider)
        {
            this._vatProvider = vatProvider;
        }*/

    public VatServ(IVatProvider vatProvider, ILoggingService logger)
    {
        this._vatProvider = vatProvider;
        this._logger = logger;
    }

    public double GrossPriceForDefaultVat(Product product)
    {
        _logger.Info("Gross Price For Default Vat");
        return CalculateGrossPrice(product.NetPrice, VatValue);
    }

    public double GrossPriceForVatProvider(Product product)
    {
        VatValue = _vatProvider.VatForType(product.ProductType);
       _logger.Info("Recieved VatValue");

        return CalculateGrossPrice(product.NetPrice, VatValue);
    }

    public double CalculateGrossPrice(double netPrice, double vatValue)
    {
        if (vatValue > 1)
        {
            _logger.Error("Jakis blad");

            throw new ArgumentOutOfRangeException(nameof(vatValue), "Vat Value is too high! Should be lower than 1.");
        }
        else
        {
            double result = netPrice * (1 + vatValue);

            _logger.Info("Calculated Gross Price for product");
            _logger.Debug($"Result value = {result}");

            return result;
        }
    }

/*    public double GrossPrice(double netPrice, string productType)
    {
        VatValue = _vatProvider.VatForType(productType);
        _logger.Info("Recieved VatValue");

        return CalculateGrossPrice(netPrice, VatValue);
    }*/

}
