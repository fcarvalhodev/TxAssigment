using AutoMapper;
using TxAssignmentInfra.Entities;
using TxAssignmentServices.Models;

namespace TxAssignmentServices.Profiles
{
    public class ProfileProduct : Profile
    {
        public ProfileProduct()
        {
            CreateMap<Product, ModelProduct>();
            CreateMap<Size, ModelSize>();
            CreateMap<ModelProduct, Product>();
            CreateMap<ModelSize, Size>();
        }
    }
}
