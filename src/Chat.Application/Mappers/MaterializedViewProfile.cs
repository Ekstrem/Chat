using AutoMapper;
using Chat.Contracts.Views;
using Chat.InternalContracts;

namespace Chat.Application.Mappers
{
    internal class MaterializedViewProfile : Profile
    {
        public MaterializedViewProfile()
        {
            CreateMap<DialogView, ChatInfoView>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.AggregateId))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Messages, opt => opt.Ignore());
        }
    }
}
