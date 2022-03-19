using System;

namespace ProtectionOfInfo.WebApp.Infrastructure.Services.ConvertToWordService
{
    public class Product
    {
        public Product(Guid guid, string vendorCode, string name, int amount, string unit, decimal price, decimal discount)
        {
            Id = guid;
            VendorCode = vendorCode;
            Name = name;
            Amount = amount;
            Unit = unit;
            Price = price;
            Discount = discount;
        }

        public Product()
        {

        }

        public Guid Id { get; set; }

        /// <summary>
        /// Артикул
        /// </summary>
        public string VendorCode { get; set; }

        /// <summary>
        /// Наименование товара
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Количество товара
        /// </summary>
        public int Amount { get; set; }

        /// <summary>
        /// Единица измерения
        /// </summary>
        public string Unit { get; set; }

        /// <summary>
        /// Цена
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Сумма счета без скидки
        /// </summary>
        public decimal SummaWithOutDiscount
        {
            get
            {
                return Price * Amount;
            }
        }

        /// <summary>
        /// Скидка
        /// </summary>
        public decimal Discount { get; set; }

        /// <summary>
        /// Cумма со скидкой
        /// </summary>
        public decimal Total
        {
            get
            {
                return SummaWithOutDiscount * ((100.0m - Discount) / 100.0m);
            }
        }

    }
}
