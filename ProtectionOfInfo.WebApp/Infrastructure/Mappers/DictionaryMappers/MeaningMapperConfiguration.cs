using ProtectionOfInfo.WebApp.Data.Entities.DictionaryEntities;
using ProtectionOfInfo.WebApp.Infrastructure.Mappers.Base;
using ProtectionOfInfo.WebApp.ViewModels.DictionaryViewModels;

namespace ProtectionOfInfo.WebApp.Infrastructure.Mappers.DictionaryMappers
{
    public class MeaningMapperConfiguration : MapperConfigurationBase
    {
        public MeaningMapperConfiguration()
        {
            CreateMap<Meaning, MeaningViewModel>()
                .ForMember(x => x.Definitions, o => o.MapFrom(x => x.Definitions));
        }
    }
}
