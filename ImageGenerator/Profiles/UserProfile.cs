using AutoMapper;
using ImageGenerator.Dtos;
using ImageGenerator.Models;

namespace ImageGenerator.Profiles;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<User, UserDto>();
    }
}
