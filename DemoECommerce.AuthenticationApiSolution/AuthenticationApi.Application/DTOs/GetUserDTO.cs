using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace AuthenticationApi.Application.DTOs
{
    public record GetUserDTO(
        int Id,
        [Required] string Name,
        [Required] string TelephoneNumber,
        [Required] string Address,
        [Required, EmailAddress] string Email,
        [Required] string Role
        );
    
}
