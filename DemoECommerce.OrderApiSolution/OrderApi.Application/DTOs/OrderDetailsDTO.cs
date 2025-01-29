using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderApi.Application.DTOs
{
    public record OrderDetailsDTO(
            [Required]
            int OrderId,

            [Required]
            int ProductId,

            [Required]
            int UserId,

            [Required, EmailAddress]
            string EmailAddress,

            [Required]
            string TelephoneNumber,

            [Required]
            string ProductName,

            [Required]
            int PurchaseQuantity,

            [Required, DataType(DataType.Currency)]
            decimal UnitPrice,

            [Required, DataType(DataType.Currency)]
            decimal TotalPrice,

            [Required]
            DateTime OrderDate
        );
}
