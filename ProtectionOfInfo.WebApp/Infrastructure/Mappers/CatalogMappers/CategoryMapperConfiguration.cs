using ProtectionOfInfo.WebApp.Data.CatalogEntities;
using ProtectionOfInfo.WebApp.Infrastructure.Mappers.Base;
using ProtectionOfInfo.WebApp.ViewModels.CatalogViewModels.CategoryViewModels;

namespace ProtectionOfInfo.WebApp.Infrastructure.CatalogMappers
{
    public class CategoryMapperConfiguration : MapperConfigurationBase
    {
        public CategoryMapperConfiguration()
        {
            CreateMap<CategoryCreateViewModel, Category>()
                .ForMember(x => x.Id, o => o.Ignore())
                .ForMember(x => x.Name, o => o.MapFrom(y => y.Name))
                .ForMember(x => x.Synopsis, o => o.MapFrom(y => y.Synopsis))
                .ForMember(x => x.CreatedAt, o => o.Ignore())
                .ForMember(x => x.CreatedBy, o => o.Ignore())
                .ForMember(x => x.UpdatedAt, o => o.Ignore())
                .ForMember(x => x.UpdatedBy, o => o.Ignore())
                .ForMember(x => x.Books, o => o.Ignore());

            CreateMap<CategoryUpdateViewModel, Category>()
                .ForMember(x => x.Id, o => o.MapFrom(y => y.Id))
                .ForMember(x => x.Name, o => o.MapFrom(y => y.Name))
                .ForMember(x => x.Synopsis, o => o.MapFrom(y => y.Synopsis))
                .ForMember(x => x.CreatedAt, o => o.Ignore())
                .ForMember(x => x.CreatedBy, o => o.Ignore())
                .ForMember(x => x.UpdatedAt, o => o.Ignore())
                .ForMember(x => x.UpdatedBy, o => o.Ignore())
                .ForMember(x => x.Books, o => o.Ignore());

            CreateMap<Category, CategoryUpdateViewModel>()
                .ForMember(x => x.Id, o => o.MapFrom(y => y.Id))
                .ForMember(x => x.Name, o => o.MapFrom(y => y.Name))
                .ForMember(x => x.Synopsis, o => o.MapFrom(y => y.Synopsis));

            CreateMap<Category, CategoryViewModel>()
                .ForMember(x => x.Id, o => o.MapFrom(y => y.Id))
                .ForMember(x => x.Name, o => o.MapFrom(y => y.Name))
                .ForMember(x => x.Synopsis, o => o.MapFrom(y => y.Synopsis));
        }
    }
}
