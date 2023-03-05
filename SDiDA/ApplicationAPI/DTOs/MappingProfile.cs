using ApplicationAPI.Entities;
using AutoMapper;
using MongoDB.Bson;

namespace ApplicationAPI.DTOs
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<UserProfile, UserProfileDto>();
            CreateMap<UserProfileCreationDto, UserProfile>();
            CreateMap<UserProfileUpdateDto, UserProfile>();
            CreateMap<Post, PostDto>();
            CreateMap<PostCreationDto, Post>()
                .ForMember(dest => dest.AuthorId, opt => opt.MapFrom(src => new ObjectId(src.AuthorId)));
            CreateMap<PostUpdateDto, Post>()
                .ForMember(dest => dest.AuthorId, opt => opt.MapFrom(src => new ObjectId(src.AuthorId)));
            CreateMap<Comment, CommentDto>();
            CreateMap<CommentCreationDto, Comment>()
                .ForMember(dest => dest.AuthorId, opt => opt.MapFrom(src => new ObjectId(src.AuthorId)));
            CreateMap<CommentUpdateDto, Comment>()
                .ForMember(dest => dest.AuthorId, opt => opt.MapFrom(src => new ObjectId(src.AuthorId)));
            CreateMap<UserProfileCreationDto, UserProfile>();
            CreateMap<UserProfileUpdateDto, UserProfile>();
            CreateMap<UserAccountRegistrationDto, UserAccount>();
            CreateMap<UserAccount, UserAccountDto>();
            CreateMap<UserAccountRegistrationDto, UserProfileCreationDto>();
        }
    }
}
