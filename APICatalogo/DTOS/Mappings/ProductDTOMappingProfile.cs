using APICatalogo.Models;
using AutoMapper;

namespace APICatalogo.DTOS.Mappings
{
    public class ProductDTOMappingProfile : Profile
    {
        public ProductDTOMappingProfile()
        {
            CreateMap<Product, ProductDTO>().ReverseMap();
            CreateMap<Category, CategoryDTO>().ReverseMap();            
            CreateMap<Product, ProductDTOUpdateRequest>().ReverseMap();
            CreateMap<Product, ProductDTOUpdateResponse>().ReverseMap();
        }
    }
}
