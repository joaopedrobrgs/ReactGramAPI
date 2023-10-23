using AutoMapper;
using ReactGramAPI.Data.Dtos;
using ReactGramAPI.Models;

namespace ReactGramAPI.Profiles;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<RegisterUserDto, User>();
        CreateMap<UpdateUserDto, User>();
        CreateMap<User, UpdateUserDto>();
        CreateMap<User, ReadAuthenticationUserDto>();
        CreateMap<User, ReadLoggedUserDto>();
        CreateMap<User, ReadUserDto>();
    }
}
