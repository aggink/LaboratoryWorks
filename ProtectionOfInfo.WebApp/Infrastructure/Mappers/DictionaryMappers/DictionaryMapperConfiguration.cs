using ProtectionOfInfo.WebApp.Data.Entities.DictionaryEntities;
using ProtectionOfInfo.WebApp.Infrastructure.Mappers.Base;
using ProtectionOfInfo.WebApp.ViewModels.DictionaryViewModels;

namespace ProtectionOfInfo.WebApp.Infrastructure.Mappers.DictionaryMappers
{
    public class DictionaryMapperConfiguration : MapperConfigurationBase
    {
        public DictionaryMapperConfiguration()
        {
            CreateMap<Dictionary, DictionaryViewModel>()
                .ForMember(x => x.Word, o => o.MapFrom(y => y.Word))
                .ForMember(x => x.Origin, o => o.MapFrom(y => y.Origin))
                .ForMember(x => x.Meanings, o => o.MapFrom(y => y.Meanings));
        }
    }
}
