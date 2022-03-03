using System.Threading.Tasks;

namespace ProtectionOfInfo.WebApp.Infrastructure.Services.ConvertToExcelService
{
    public interface IConvertToExcel
    { 
        public Task<byte[]> ConvertBookToExcel();
        public Task<byte[]> ConvertDbToExcel();
    }
}
