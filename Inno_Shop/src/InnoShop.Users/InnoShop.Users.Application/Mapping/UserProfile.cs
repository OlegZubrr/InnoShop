using AutoMapper;
using InnoShop.Users.Application.DTOs;
using InnoShop.Users.Domain.Entities;

namespace InnoShop.Users.Application.Mapping;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<UserCreateDto, User>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.PasswordHash, opt => opt.MapFrom(src => src.Password));

        CreateMap<UserUpdateDto, User>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore());

        CreateMap<User, UserResponseDto>();
    }
}