using System;
using System.Collections.Generic;

namespace ProtectionOfInfo.WebApp.Infrastructure.Services.ConvertToWordService
{
    public class Check
    {
        public Check(string type, int number, DateTime date, string supplier, string supplierDetails, string сustomer, string deliveryAddress, string deliveryType, List<Product> products)
        {
            Type = type;
            Number = number;
            Date = date;
            Supplier = supplier;
            SupplierDetails = supplierDetails;
            Сustomer = сustomer;
            DeliveryAddress = deliveryAddress;
            DeliveryType = deliveryType;
            Products = products;
        }

        public Check()
        {

        }

        /// <summary>
        /// Тип договора
        /// </summary>
        public string Type { get; set; } = null!;

        /// <summary>
        /// номер договора
        /// </summary>
        public int Number { get; set; }

        /// <summary>
        /// Дата создания договора
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// данные поставщика
        /// </summary>
        public string Supplier { get; set; } = null!;

        /// <summary>
        /// Платежные реквизиты поставщика
        /// </summary>
        public string SupplierDetails { get; set; } = null!;

        /// <summary>
        /// Покупатель
        /// </summary>
        public string Сustomer { get; set; } = null!;

        /// <summary>
        /// Адрес доставки
        /// </summary>
        public string DeliveryAddress { get; set; } = null!;

        /// <summary>
        /// Способ доставки
        /// </summary>
        public string DeliveryType { get; set; } = null!;

        /// <summary>
        /// Сумма счета без скидки
        /// </summary>
        public decimal Total{ 
            get
            {
                decimal sum = 0;
                foreach(var item in Products)
                {
                    sum += item.Total;
                }

                return sum;
            } 
        }

        /// <summary>
        /// НДС
        /// </summary>
        public decimal VAT
        {
            get
            {
                return Total * 0.2m;
            }
        }

        /// <summary>
        /// Образец заполнения назначения платежа
        /// </summary>
        public string PaymentInformation
        {
            get
            {
                return $"Оплата по счёту №{Type}{Number} от {Date.ToString("dd.MM.yyyy")} за товары, сумма {Total.ToString("C")} руб, в т.ч. НДС (20%): {VAT.ToString("C")}";
            }
        }

        /// <summary>
        /// Список товаров
        /// </summary>
        public List<Product> Products { get; set; } = null!;

        /// <summary>
        /// Количество позиций в заказе
        /// </summary>
        private int NumberOfPositionsProducts
        { 
            get
            {
                int num = 0;
                for(int i = 0; i < Products.Count; i++)
                {
                    num += Products[i].Amount;
                }

                return num;
            } 
        }

        /// <summary>
        /// Итог по заказу
        /// </summary>
        public string ResultInfoCheck
        {
            get
            {
                return $"Всего наименований {Products.Count}, количество позиций {NumberOfPositionsProducts}, на сумму {Total.ToString("C")} руб.";
            }
        }
    }
}
