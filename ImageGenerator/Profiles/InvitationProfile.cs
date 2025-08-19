using AutoMapper;
using ImageGenerator.Dtos;
using ImageGenerator.Models;

namespace ImageGenerator.Profiles;

public class InvitationProfile : Profile
{
    public InvitationProfile()
    {
        CreateMap<Invitation, InvitationDto>();
        // CreateMap<InvitationDto, Invitation>();
    }
}
