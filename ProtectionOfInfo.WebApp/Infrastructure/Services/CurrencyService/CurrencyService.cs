using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Hosting;
using ProtectionOfInfo.WebApp.Data.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace ProtectionOfInfo.WebApp.Infrastructure.Services.CurrencyService
{
    public class CurrencyService : BackgroundService, ICurrencyService
    {
        private readonly IMemoryCache _memoryCache;
        private readonly IWebHostEnvironment _env;

        private readonly string Url = "http://www.cbr.ru/scripts/XML_daily.asp";
        public readonly string KeyMemoryCache = "KeyCurrency";
        public readonly string path;

        public CurrencyService(IMemoryCache memoryCache, IWebHostEnvironment env)
        {
            _memoryCache = memoryCache;
            _env = env;
            path = Path.Combine(_env.ContentRootPath, "wwwroot", "documents");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var fileName = Path.Combine(path, Guid.NewGuid().ToString() + ".xml");

                try
                {
                    Thread.CurrentThread.CurrentCulture = new CultureInfo("ru-Ru");
                    Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

                    XDocument xdoc = XDocument.Load(Url);
                    XElement? ValCurs = xdoc.Element("ValCurs");
                    if(ValCurs == null)
                    {
                        throw new Exception();
                    }

                    List<Valute>? currencies = new List<Valute>();
                    foreach (var valute in ValCurs.Elements("Valute"))
                    {
                        var val = new Valute()
                        {
                            ID = valute.Attribute("ID")!.Value,
                            NumCode = int.Parse(valute.Element("NumCode")!.Value),
                            CharCode = valute.Element("CharCode")!.Value,
                            Nominal = decimal.Parse(valute.Element("Nominal")!.Value),
                            Name = valute.Element("Name")!.Value,
                            Value = decimal.Parse(valute.Element("Value")!.Value)
                        };
                        
                        currencies.Add(val);
                    }

                    _memoryCache.Set(KeyMemoryCache, currencies, TimeSpan.FromSeconds(1400));
                    
                }
                catch
                {

                }

                await Task.Delay(3600000, stoppingToken);
            }
        }

        public List<Valute>? GetAllValute()
        {
            if(!_memoryCache.TryGetValue(KeyMemoryCache, out List<Valute> valuteName))
            {
                throw new Exception("Значения не найдены");
            }

            return valuteName;
        }

        public Valute GetValute(string Name)
        {
            if (!_memoryCache.TryGetValue(KeyMemoryCache, out List<Valute> valuteName))
            {
                throw new Exception("Значения не найдены");
            }

            var valute = valuteName.FirstOrDefault(x => x.Name == Name);
            if (valute == null)
            {
                throw new Exception("Нет совпадений");
            }

            return valute;
        }
    }
}
