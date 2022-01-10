using ProtectionOfInfo.WebApp.Data.CatalogEntities;
using ProtectionOfInfo.WebApp.Infrastructure.Mappers.Base;
using ProtectionOfInfo.WebApp.ViewModels.CatalogViewModels.AuthorViewModels;
using ProtectionOfInfo.WebApp.ViewModels.CatalogViewModels.BookViewModels;
using ProtectionOfInfo.WebApp.ViewModels.CatalogViewModels.CategoryViewModels;
using ProtectionOfInfo.WebApp.ViewModels.CatalogViewModels.PublisherViewModels;

namespace ProtectionOfInfo.WebApp.Infrastructure.CatalogMappers
{
    public class BookMapperConfiguration : MapperConfigurationBase
    {
        public BookMapperConfiguration()
        {
            CreateMap<BookCreateViewModel, Book>()
                .ForMember(x => x.Id, o => o.Ignore())
                .ForMember(x => x.Name, o => o.MapFrom(y => y.Name))
                .ForMember(x => x.Price, o => o.MapFrom(y => y.Price))
                .ForMember(x => x.AuthorId, o => o.MapFrom(y => y.AuthorId))
                .ForMember(x => x.CategoryId, o => o.MapFrom(y => y.CategoryId))
                .ForMember(x => x.PublisherId, o => o.MapFrom(y => y.PublisherId))
                .ForMember(x => x.Author, o => o.Ignore())
                .ForMember(x => x.Category, o => o.Ignore())
                .ForMember(x => x.Publisher, o => o.Ignore())
                .ForMember(x => x.CreatedAt, o => o.Ignore())
                .ForMember(x => x.CreatedBy, o => o.Ignore())
                .ForMember(x => x.UpdatedAt, o => o.Ignore())
                .ForMember(x => x.UpdatedBy, o => o.Ignore());

            CreateMap<BookUpdateViewModel, Book>()
                .ForMember(x => x.Id, o => o.MapFrom(y => y.Id))
                .ForMember(x => x.Name, o => o.MapFrom(y => y.Name))
                .ForMember(x => x.Price, o => o.MapFrom(y => y.Price))
                .ForMember(x => x.AuthorId, o => o.MapFrom(y => y.AuthorId))
                .ForMember(x => x.CategoryId, o => o.MapFrom(y => y.CategoryId))
                .ForMember(x => x.PublisherId, o => o.MapFrom(y => y.PublisherId))
                .ForMember(x => x.Author, o => o.Ignore())
                .ForMember(x => x.Category, o => o.Ignore())
                .ForMember(x => x.Publisher, o => o.Ignore())
                .ForMember(x => x.CreatedAt, o => o.Ignore())
                .ForMember(x => x.CreatedBy, o => o.Ignore())
                .ForMember(x => x.UpdatedAt, o => o.Ignore())
                .ForMember(x => x.UpdatedBy, o => o.Ignore());

            CreateMap<Book, BookUpdateViewModel>()
                .ForMember(x => x.Id, o => o.MapFrom(y => y.Id))
                .ForMember(x => x.Name, o => o.MapFrom(y => y.Name))
                .ForMember(x => x.Price, o => o.MapFrom(y => y.Price))
                .ForMember(x => x.AuthorId, o => o.MapFrom(y => y.AuthorId))
                .ForMember(x => x.CategoryId, o => o.MapFrom(y => y.CategoryId))
                .ForMember(x => x.PublisherId, o => o.MapFrom(y => y.PublisherId))
                .ForMember(x => x.Authors, o => o.Ignore())
                .ForMember(x => x.Categories, o => o.Ignore())
                .ForMember(x => x.Publishers, o => o.Ignore());

            CreateMap<Book, BookViewModel>()
                .ForMember(x => x.Id, o => o.MapFrom(y => y.Id))
                .ForMember(x => x.Name, o => o.MapFrom(y => y.Name))
                .ForMember(x => x.Price, o => o.MapFrom(y => y.Price))
                .AfterMap((x, y, context) => y.CategoryViewModel = context.Mapper.Map<Category, CategoryViewModel>(x.Category!))
                .AfterMap((x, y, context) => y.AuthorViewModel = context.Mapper.Map<Author, AuthorViewModel>(x.Author!))
                .AfterMap((x, y, context) => y.PublisherViewModel = context.Mapper.Map<Publisher, PublisherViewModel>(x.Publisher!)); ;
        }
    }
}
