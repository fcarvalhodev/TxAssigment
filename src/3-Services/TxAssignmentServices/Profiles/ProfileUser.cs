using AutoMapper;
using TxAssignmentInfra.Entities;
using TxAssignmentServices.Models;

namespace TxAssignmentServices.Profiles
{
    public class ProfileUser : Profile
    {
        public ProfileUser()
        {
            CreateMap<ModelUser, User>()
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore());

            CreateMap<User, ModelUser>()
                .ForMember(dest => dest.Password, opt => opt.Ignore());
        }
    }
}
