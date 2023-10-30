using AutoMapper;
using ReactGramAPI.Data.Dtos;
using ReactGramAPI.Models;

namespace ReactGramAPI.Profiles;

public class CommentProfile : Profile
{
    public CommentProfile()
    {
        CreateMap<CreateCommentDto, Comment>();
        CreateMap<Comment, ReadCommentDto>();
        CreateMap<UpdateCommentDto, Comment>();
    }
}
