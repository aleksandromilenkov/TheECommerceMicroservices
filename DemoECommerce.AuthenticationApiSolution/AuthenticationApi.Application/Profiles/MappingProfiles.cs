using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AuthenticationApi.Application.DTOs;
using AuthenticationApi.Domain.Entities;
using AutoMapper;

namespace AuthenticationApi.Application.Profiles
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<AppUser, AppUserDTO>().ReverseMap();
            CreateMap<AppUser, AppUserCreateDTO>().ReverseMap();
            CreateMap<AppUser, GetUserDTO>().ReverseMap();
        }
    }
}
