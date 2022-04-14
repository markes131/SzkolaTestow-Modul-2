using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VatServices
{
    public class Product
    {
        private int Id { get; set; }

        public double NetPrice { get; set; }

        public string ProductType { get; set; }


        public Product(int id, double netPrice)
        {
            if (id < 0) {
                throw new ArgumentOutOfRangeException(nameof(id), "Id value should be above 0");
            }
            else {
                Id = id;
            }


            if (netPrice < 0) {
                throw new ArgumentOutOfRangeException(nameof(netPrice), "Netto Price value should be above 0");
            }
            else {
                NetPrice = netPrice;
            }
        }

        public Product(int id, double netPrice, string productType) : base()
        {
            if (productType != null)
            {
                ProductType = productType;
            }
            else {
                throw new ArgumentNullException(nameof(productType), "Product has to has a type");
            }
            if (id < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id), "Id value should be above 0");
            }
            else
            {
                Id = id;
            }


            if (netPrice < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(netPrice), "Netto Price value should be above 0");
            }
            else
            {
                NetPrice = netPrice;
            }
        }

        //public enum ProductType
        //{
        //    Furniture,
        //    Food,
        //    Electronic,
        //    Books
        //}

    }
}
