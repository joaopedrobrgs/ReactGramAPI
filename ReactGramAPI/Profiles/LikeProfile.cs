using AutoMapper;
using ReactGramAPI.Data.Dtos;
using ReactGramAPI.Models;

namespace ReactGramAPI.Profiles;

public class LikeProfile : Profile
{
    public LikeProfile()
    {
        CreateMap<Like, ReadLikeDto>();
    }
}
