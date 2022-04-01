using ProtectionOfInfo.WebApp.Data.Entities;
using System.Collections.Generic;

namespace ProtectionOfInfo.WebApp.Infrastructure.Services.CurrencyService
{
    public interface ICurrencyService
    {
        public List<Valute>? GetAllValute();
        public Valute GetValute(string Name);
    }
}
