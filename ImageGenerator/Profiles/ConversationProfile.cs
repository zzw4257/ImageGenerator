using AutoMapper;
using ImageGenerator.Dtos;
using ImageGenerator.Enums;
using ImageGenerator.Models;

namespace ImageGenerator.Profiles;

public class ConversationProfile : Profile
{
    public ConversationProfile()
    {
        CreateMap<Conversation, ConversationDto>();
        
        CreateMap<GenerationRecord, GenerationRecordDto>()
            .ForMember(dest => dest.OutputImage, opt => opt.MapFrom(src => src.OutputImages));
        
        CreateMap<Image, ImageDto>();
        
        CreateMap<GenerateImageDto, GenerationRecord>()
            .ForMember(dest => dest.GenerationParams, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => GenerationStatus.Pending));
    }
}
