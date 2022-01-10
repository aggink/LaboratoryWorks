using ProtectionOfInfo.WebApp.Data.CatalogEntities;
using ProtectionOfInfo.WebApp.Infrastructure.Mappers.Base;
using ProtectionOfInfo.WebApp.ViewModels.CatalogViewModels.PublisherViewModels;

namespace ProtectionOfInfo.WebApp.Infrastructure.Mappers
{
    public class PublisherMapperConfiguration : MapperConfigurationBase
    {
        public PublisherMapperConfiguration()
        {
            CreateMap<PublisherCreateViewModel, Publisher>()
                .ForMember(x => x.Id, o => o.Ignore())
                .ForMember(x => x.Name, o => o.MapFrom(y => y.Name))
                .ForMember(x => x.Description, o => o.MapFrom(y => y.Description))
                .ForMember(x => x.CreatedAt, o => o.Ignore())
                .ForMember(x => x.CreatedBy, o => o.Ignore())
                .ForMember(x => x.UpdatedAt, o => o.Ignore())
                .ForMember(x => x.UpdatedBy, o => o.Ignore())
                .ForMember(x => x.Books, o => o.Ignore());

            CreateMap<PublisherUpdateViewModel, Publisher>()
                .ForMember(x => x.Id, o => o.MapFrom(y => y.Id))
                .ForMember(x => x.Name, o => o.MapFrom(y => y.Name))
                .ForMember(x => x.Description, o => o.MapFrom(y => y.Description))
                .ForMember(x => x.CreatedAt, o => o.Ignore())
                .ForMember(x => x.CreatedBy, o => o.Ignore())
                .ForMember(x => x.UpdatedAt, o => o.Ignore())
                .ForMember(x => x.UpdatedBy, o => o.Ignore())
                .ForMember(x => x.Books, o => o.Ignore());

            CreateMap<Publisher, PublisherUpdateViewModel>()
                .ForMember(x => x.Id, o => o.MapFrom(y => y.Id))
                .ForMember(x => x.Name, o => o.MapFrom(y => y.Name))
                .ForMember(x => x.Description, o => o.MapFrom(y => y.Description));

            CreateMap<Publisher, PublisherViewModel>()
                .ForMember(x => x.Id, o => o.MapFrom(y => y.Id))
                .ForMember(x => x.Name, o => o.MapFrom(y => y.Name))
                .ForMember(x => x.Description, o => o.MapFrom(y => y.Description));

        }
    }
}
