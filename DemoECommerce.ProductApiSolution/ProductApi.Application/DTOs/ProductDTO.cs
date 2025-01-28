using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductApi.Application.DTOs
{
    public record ProductDTO
        (
        int Id,
        [Required] 
        string ProductName,

        [Required, Range(1, int.MaxValue)]
        int Quantity,

        [Required, DataType(DataType.Currency)] decimal Price
        );
}
