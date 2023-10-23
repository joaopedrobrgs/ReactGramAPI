using AutoMapper;
using ReactGramAPI.Data.Dtos;
using ReactGramAPI.Models;

namespace ReactGramAPI.Profiles;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<CreateUserDto, User>();
    }
}
