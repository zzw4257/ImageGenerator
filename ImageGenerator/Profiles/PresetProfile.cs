using AutoMapper;
using ImageGenerator.Dtos;
using ImageGenerator.Models;

namespace ImageGenerator.Profiles;

public class PresetProfile : Profile
{
    public PresetProfile()
    {
        CreateMap<Preset, PresetDto>();
        CreateMap<Preset, PresetDetailedDto>();
        CreateMap<PresetFavorite, PresetFavoriteDto>();
        CreateMap<PresetLike, PresetLikeDto>();
    }
}
