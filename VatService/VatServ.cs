
using Vat.Services;

namespace VatServices;

public class VatServ
{
    public double VatValue { get; set; }
    readonly IVatProvider _vatProvider;

    public VatServ()
    {
        this.VatValue = 0.23;
    }

    public VatServ(double vatValue)
    {
        this.VatValue = vatValue;
    }

    public VatServ(IVatProvider vatProvider)
    {
        this._vatProvider = vatProvider;
    }

    public double GrossPriceForDefaultVat(Product product)
    {
        return CalculateGrossPrice(product.NetPrice, VatValue);
    }

    public double GrossPriceForVatProvider(Product product)
    {
        VatValue = _vatProvider.VatForType(product.ProductType);

        return CalculateGrossPrice(product.NetPrice, VatValue);
    }

    public double CalculateGrossPrice(double netPrice, double vatValue)
    {
        if (vatValue > 1)
        {
            throw new ArgumentOutOfRangeException(nameof(vatValue), "Vat Value is too high! Should be lower than 1.");
        }

        return netPrice * (1 + vatValue);
    }

    public double GrossPrice(double netPrice, string productType)
    {
        VatValue = _vatProvider.VatForType(productType);
        return CalculateGrossPrice(netPrice, VatValue);
    }

}
