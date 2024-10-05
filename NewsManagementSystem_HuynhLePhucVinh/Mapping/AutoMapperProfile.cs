using AutoMapper;
using BusinessObject;
using DTO.Account;
using DTO.Category;
using DTO.News;
using DTO.Tag;


namespace ProductManagementAPI.Mapping
{
    public class AutoMapperProfile : Profile
    {

        public AutoMapperProfile()
        {
            CreateMap<Category, ResponseCategoryDTO>().ForMember(dest => dest.ParentCategoryName, o => o.MapFrom(src => src.ParentCategory.CategoryName)).ReverseMap();
            CreateMap<Category, CreateCategoryDTO>().ReverseMap();
            CreateMap<Category, UpdateCategoryDTO>().ReverseMap();
            CreateMap<Tag, CreateTagDTO>().ReverseMap();
            CreateMap<Tag, UpdateTagDTO>().ReverseMap();
            CreateMap<NewsArticle, CreateNewsDTO>().ReverseMap();
            CreateMap<NewsArticle, UpdateNewsDTO>().ReverseMap();
            CreateMap<SystemAccount, CreateAccountDTO>().ReverseMap();
            CreateMap<SystemAccount, UpdateAccountDTO>().ReverseMap();
            CreateMap<SystemAccount, UpdateProfileDTO>().ReverseMap();
        }
    }
}
