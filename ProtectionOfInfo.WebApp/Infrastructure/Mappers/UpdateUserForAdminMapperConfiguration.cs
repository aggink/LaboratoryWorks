using ProtectionOfInfo.WebApp.Data.Entities;
using ProtectionOfInfo.WebApp.Infrastructure.Mappers.Base;
using ProtectionOfInfo.WebApp.ViewModels.AccountViewModels;

namespace ProtectionOfInfo.WebApp.Infrastructure.Mappers
{
    public class UpdateUserForAdminMapperConfiguration : MapperConfigurationBase
    {
        public UpdateUserForAdminMapperConfiguration()
        {
            CreateMap<MyIdentityUser, UpdateUserForAdminViewModel>()
                .ForMember(x => x.BlockedUser, o => o.MapFrom(src => src.BlockedUser))
                .ForMember(x=> x.UserName, o => o.MapFrom(src => src.UserName))
                .ForMember(x => x.Id, o => o.MapFrom(src => src.Id))
                .ForMember(x => x.PasswordValidation, o => o.MapFrom(src => src.PasswordValidation));

            CreateMap<UpdateUserForAdminViewModel, MyIdentityUser>()
                .ForMember(x => x.UserName, o => o.MapFrom(src => src.UserName))
                .ForMember(x => x.Id, o => o.MapFrom(src => src.Id))
                .ForMember(x => x.BlockedUser, o => o.MapFrom(src => src.BlockedUser))
                .ForMember(x => x.PasswordValidation, o => o.MapFrom(src => src.PasswordValidation))
                .ForMember(x => x.Email, o => o.Ignore())
                .ForMember(x => x.EmailConfirmed, o => o.Ignore())
                .ForMember(x => x.AccessFailedCount, o => o.Ignore())
                .ForMember(x => x.ConcurrencyStamp, o => o.Ignore())
                .ForMember(x => x.LockoutEnabled, o => o.Ignore())
                .ForMember(x => x.LockoutEnd, o => o.Ignore())
                .ForMember(x => x.NormalizedEmail, o => o.Ignore())
                .ForMember(x => x.NormalizedUserName, o => o.Ignore())
                .ForMember(x => x.PasswordHash, o => o.Ignore())
                .ForMember(x => x.PhoneNumber, o => o.Ignore())
                .ForMember(x => x.PhoneNumberConfirmed, o => o.Ignore())
                .ForMember(x => x.SecurityStamp, o => o.Ignore())
                .ForMember(x => x.FirstAccess, o => o.Ignore());
            
        }
    }
}
