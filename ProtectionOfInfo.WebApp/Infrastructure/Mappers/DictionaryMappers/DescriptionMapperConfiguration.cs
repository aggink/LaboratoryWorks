using ProtectionOfInfo.WebApp.Data.Entities.DictionaryEntities;
using ProtectionOfInfo.WebApp.Infrastructure.Mappers.Base;
using ProtectionOfInfo.WebApp.ViewModels.DictionaryViewModels;

namespace ProtectionOfInfo.WebApp.Infrastructure.Mappers.DictionaryMappers
{
    public class DescriptionMapperConfiguration : MapperConfigurationBase
    {
        public DescriptionMapperConfiguration()
        {
            CreateMap<Description, DescriptionViewModel>()
                .ForMember(x => x.Antonyms, o => o.MapFrom(y => y.Antonyms))
                .ForMember(x => x.Synonyms, o => o.MapFrom(y => y.Synonyms))
                .ForMember(x => x.Definition, o => o.MapFrom(y => y.Definition))
                .ForMember(x => x.Example, o => o.MapFrom(y => y.Example));
        }
    }
}
