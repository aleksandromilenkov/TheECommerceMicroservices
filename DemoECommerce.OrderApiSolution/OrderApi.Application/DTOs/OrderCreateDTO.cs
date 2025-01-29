using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderApi.Application.DTOs
{
    public record OrderCreateDTO(
            [Required, Range(1, int.MaxValue)]
            int ProductId,

            [Required, Range(1, int.MaxValue)]
            int UserId,

            [Required, Range(1, int.MaxValue)]
            int PurchaseQuantity,

            DateTime? OrderedDate
        );
}
