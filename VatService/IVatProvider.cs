using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vat.Services
{
    public interface IVatProvider
    {
        double DefaultVat();

        double VatForType(string type);
    }
}
