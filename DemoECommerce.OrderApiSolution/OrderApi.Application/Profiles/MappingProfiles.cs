using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using OrderApi.Application.DTOs;
using OrderApi.Domain.Entities;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace OrderApi.Application.Profiles
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<Order, OrderDTO>().ReverseMap();
            CreateMap<Order, OrderCreateDTO>().ReverseMap();
        }
    }
}
