using AutoMapper;
using BlocshopTest.Domain.Events.Models;
using BlocshopTest.Web.Models.Events;

namespace BlocshopTest.Web;

public class WebAutoMapperProfile : Profile
{
    public WebAutoMapperProfile()
    {
        CreateEventMappings();
    }

    private void CreateEventMappings()
    {
        CreateMap<Event, EventSimpleDto>();
        CreateMap<Event, EventDto>();
        CreateMap<CreateEventDto, Event>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());
    }
}
