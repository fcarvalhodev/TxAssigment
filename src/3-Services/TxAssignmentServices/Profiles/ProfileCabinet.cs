using AutoMapper;
using TxAssignmentInfra.Entities;
using TxAssignmentServices.Models;

namespace TxAssignmentServices.Profiles
{
    public class ProfileCabinet : Profile
    {
        public ProfileCabinet()
        {
            CreateMap<Cabinet, ModelCabinet>();
            CreateMap<Row, ModelRow>();
            CreateMap<Lane, ModelLane>();
            CreateMap<Position, ModelPosition>();
            CreateMap<Size, ModelSize>();

            CreateMap<ModelCabinet, Cabinet>();
            CreateMap<ModelRow, Row>();
            CreateMap<ModelLane, Lane>();
            CreateMap<ModelPosition, Position>();
            CreateMap<ModelSize, Size>();
        }
    }
}
