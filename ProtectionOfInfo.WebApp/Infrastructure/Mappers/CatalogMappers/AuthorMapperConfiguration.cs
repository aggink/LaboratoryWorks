using ProtectionOfInfo.WebApp.Data.CatalogEntities;
using ProtectionOfInfo.WebApp.Infrastructure.Mappers.Base;
using ProtectionOfInfo.WebApp.ViewModels.CatalogViewModels.AuthorViewModels;

namespace ProtectionOfInfo.WebApp.Infrastructure.CatalogMappers
{
    public class AuthorMapperConfiguration : MapperConfigurationBase
    {
        public AuthorMapperConfiguration()
        {
            CreateMap<AuthorCreateViewModel, Author>()
                .ForMember(x => x.Id, o => o.Ignore())
                .ForMember(x => x.Name, o => o.MapFrom(y => y.Name))
                .ForMember(x => x.Biography, o => o.MapFrom(y => y.Biography))
                .ForMember(x => x.CreatedAt, o => o.Ignore())
                .ForMember(x => x.CreatedBy, o => o.Ignore())
                .ForMember(x => x.UpdatedAt, o => o.Ignore())
                .ForMember(x => x.UpdatedBy, o => o.Ignore())
                .ForMember(x => x.Books, o => o.Ignore());

            CreateMap<AuthorUpdateViewModel, Author>()
                .ForMember(x => x.Id, o => o.MapFrom(y => y.Id))
                .ForMember(x => x.Name, o => o.MapFrom(y => y.Name))
                .ForMember(x => x.Biography, o => o.MapFrom(y => y.Biography))
                .ForMember(x => x.CreatedAt, o => o.Ignore())
                .ForMember(x => x.CreatedBy, o => o.Ignore())
                .ForMember(x => x.UpdatedAt, o => o.Ignore())
                .ForMember(x => x.UpdatedBy, o => o.Ignore())
                .ForMember(x => x.Books, o => o.Ignore());

            CreateMap<Author, AuthorUpdateViewModel>()
                .ForMember(x => x.Id, o => o.MapFrom(y => y.Id))
                .ForMember(x => x.Name, o => o.MapFrom(y => y.Name))
                .ForMember(x => x.Biography, o => o.MapFrom(y => y.Biography));

            CreateMap<Author, AuthorViewModel>()
                .ForMember(x => x.Id, o => o.MapFrom(y => y.Id))
                .ForMember(x => x.Name, o => o.MapFrom(y => y.Name))
                .ForMember(x => x.Biography, o => o.MapFrom(y => y.Biography)); 
        }
    }
}
