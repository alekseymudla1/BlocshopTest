using AutoMapper;
using BlocshopTest.Domain.Events.Models;
using BlocshopTest.Domain.Holds.Models;
using BlocshopTest.Domain.Shared.Models;
using BlocshopTest.Web.Models.Events;
using BlocshopTest.Web.Models.Holds;
using BlocshopTest.Web.Models.Shared;

namespace BlocshopTest.Web;

public class WebAutoMapperProfile : Profile
{
    public WebAutoMapperProfile()
    {
        CreateCommonMaps();
        CreateEventMappings();
        CreataHoldMappings();
    }

    private void CreateCommonMaps()
    {
        CreateMap(typeof(Page<>), typeof(PageDto<>));
    }
    private void CreateEventMappings()
    {
        CreateMap<Event, EventSimpleDto>();
        CreateMap<Event, EventDto>();
        CreateMap<CreateEventDto, Event>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());
    }

    private void CreataHoldMappings()
    {
        CreateMap<Hold, CreatedHoldDto>()
            .ForMember(dst => dst.HoldId, opt => opt.MapFrom(src => src.Id));
    }
}
