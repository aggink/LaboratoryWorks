using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace ProtectionOfInfo.WebApp.Infrastructure.Services.PasswordValidatorsService
{
    public interface IMyPasswordValidatorService
    {
        public IdentityResult Validate(string password);
    }
}
