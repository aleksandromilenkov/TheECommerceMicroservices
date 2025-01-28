using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using ProductApi.Application.DTOs;
using ProductApi.Application.Entites;

namespace ProductApi.Application.Profiles
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles() { 
            CreateMap<Product, ProductDTO>().ReverseMap();
            CreateMap<Product, ProductCreateDTO>().ReverseMap();
        }
    }
}
