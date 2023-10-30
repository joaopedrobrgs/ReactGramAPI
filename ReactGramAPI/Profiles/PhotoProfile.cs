using AutoMapper;
using ReactGramAPI.Data.Dtos;
using ReactGramAPI.Models;

namespace ReactGramAPI.Profiles;

public class PhotoProfile : Profile
{
    public PhotoProfile()
    {
        CreateMap<CreatePhotoDto, Photo>();
        CreateMap<Photo, ReadPhotoDto>()
            .ForMember(photoDto => photoDto.User, option => option.MapFrom(photo => photo.User))
            .ForMember(photoDto => photoDto.Likes, option => option.MapFrom(photo => photo.Likes))
            .ForMember(photoDto => photoDto.Comments, option => option.MapFrom(photo => photo.Comments));
        CreateMap<UpdatePhotoDto, Photo>();
    }
}
