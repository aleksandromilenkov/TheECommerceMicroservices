using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AuthenticationApi.Application.DTOs;
using AuthenticationApi.Domain.Entities;
using ECommerce.SharedLibrary.Responses;

namespace AuthenticationApi.Application.Interfaces
{
    public interface IAppUser
    {
        Task<Response> Register(AppUserCreateDTO appUserCreateDTO);
        Task<Response> Login(LoginDTO loginDTO);
        Task<Response> UpdateUser(AppUser appUser);
        Task<Response> DeleteUser(int userId);
        Task<AppUser> GetUser(int userId);
        Task<List<AppUser>> GetAllUsers();
    }
}
