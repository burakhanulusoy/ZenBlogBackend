using AutoMapper;
using ZenBlog.Application.Features.Commands.CategoryCommands;
using ZenBlog.Application.Features.Results.CategoryResults;
using ZenBlog.Domain.Entities;

namespace ZenBlog.Application.Mappings.CategoryMappings
{
    public class CategoryMapping :Profile
    {
        public CategoryMapping()
        {
            CreateMap<Category, GetCategoryByIdQueryResult>().ReverseMap();
            CreateMap<Category, GetCategoriesQueryResult>().ReverseMap();
            CreateMap<CreateCategoryCommand, Category>().ReverseMap();
            CreateMap<UpdateCategoryCommand, Category>().ReverseMap();


        }
    }
}
