using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AuthenticationApi.Application.DTOs;
using AuthenticationApi.Application.Interfaces;
using AuthenticationApi.Domain.Entities;
using AuthenticationApi.Infrastructure.Data;
using AutoMapper;
using ECommerce.SharedLibrary.Logs;
using ECommerce.SharedLibrary.Responses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace AuthenticationApi.Infrastructure.Repositories
{
    public class AppUserRepository(IMapper mapper, AuthenticationDbContext context, IConfiguration config) : IAppUser
    {

        public async Task<List<AppUser>> GetAllUsers()
        {
            try
            {
                var users = await context.Users.ToListAsync();
                return users;
            }
            catch (Exception ex)
            {
                LogException.LogExceptions(ex);
                throw new Exception("Error occurred retreiving users");
            }

        }
        public async Task<AppUser> GetUser(int userId)
        {
            try
            {
                var user = await context.Users.FindAsync(userId);
                return user ?? null!;
            }
            catch (Exception ex) { 
                LogException.LogExceptions(ex);
                throw new Exception("Error occurred retreiving user");
            }
            
        }

        public async Task<AppUser> GetUserByEmail(string email)
        {
            try
            {
                var user = await context.Users.AsNoTracking().FirstOrDefaultAsync(u=> u.Email == email);
                return user ?? null!;
            }
            catch (Exception ex)
            {
                LogException.LogExceptions(ex);
                throw new Exception("Error occurred retreiving user");
            }

        }

        public async Task<Response> Login(LoginDTO loginDTO)
        {
            var getUser = await GetUserByEmail(loginDTO.Email);
            if (getUser == null) {
                return new Response(false, "Invalid credentials");
            }
            bool verifyPassword = BCrypt.Net.BCrypt.Verify(loginDTO.Password, getUser.Password);
            if (!verifyPassword)
            {
                return new Response() { Flag = false, Message = "Invalid credentials" };
            }
            string token = GenerateToken(getUser);
            return new Response() { Flag = true, Message = token };
        }

        private string GenerateToken(AppUser user)
        {
            var key = Encoding.UTF8.GetBytes(config.GetSection("Authentication:Key").Value!);
            var symmetricSecurityKey = new SymmetricSecurityKey(key);
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Email, user.Email),
            };

            if(!string.IsNullOrEmpty(user.Role) || !Equals("string", user.Role))
            {
                claims.Add(new Claim(ClaimTypes.Role, user.Role));
            }

            var jwtSecurityToken = new JwtSecurityToken(
               issuer: config.GetSection("Authentication:Issuer").Value,
               audience: config.GetSection("Authentication:Audience").Value,
               claims: claims,
               expires: DateTime.UtcNow.AddMinutes(10),
               signingCredentials: signingCredentials);
            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            return jwtSecurityTokenHandler.WriteToken(jwtSecurityToken);
        }

        public async Task<Response> Register(AppUserCreateDTO appUserDTO)
        {
            try
            {

            if (await GetUserByEmail(appUserDTO.Email) != null) {
                return new Response() { Flag = false, Message = $"User with email: {appUserDTO.Email} already exists." };
                }
            var userToAdd = mapper.Map<AppUser>(appUserDTO);
            userToAdd.Password = BCrypt.Net.BCrypt.HashPassword(appUserDTO.Password);
            var result = await context.AddAsync(userToAdd);
            await context.SaveChangesAsync();
            return result.Entity.Id > 0
                    ? new Response() { Flag = true, Message = $"User registered successfully." }
                    : new Response() { Flag = false, Message = $"Failed to register the user." };
            }
            catch (Exception ex)
            {
                LogException.LogExceptions(ex);
                throw new Exception("Something went wrong trying to register a user");
            }
        }

        public async Task<Response> UpdateUser(AppUser appUser)
        {
            try
            {
                var user = await GetUserByEmail(appUser.Email);
                if (user == null) { 
                    return new Response() { Flag = false, Message = "User does not exists." };
                }
                appUser.Password = BCrypt.Net.BCrypt.HashPassword(appUser.Password);
                context.Update(appUser);
                await context.SaveChangesAsync();
                return new Response() { Flag = true, Message="User updated successfully." };
            }catch(Exception ex)
            {
                LogException.LogExceptions(ex);
                throw new Exception("Something went wrong trying to update a user");
            }
        }

        public async Task<Response> DeleteUser(int userId)
        {
            try
            {
                var user = await GetUser(userId);
                if (user == null)
                {
                    return new Response() { Flag = false, Message = "User does not exists." };
                }
                context.Remove(user);
                await context.SaveChangesAsync();
                return new Response() { Flag = true, Message = "User removed successfully." };
            }
            catch (Exception ex)
            {
                // Log Original Exception
                LogException.LogExceptions(ex);

                //display user-friendly message to the client
                throw new Exception("Error occurred deliting user");
            }
        }
    }
}
