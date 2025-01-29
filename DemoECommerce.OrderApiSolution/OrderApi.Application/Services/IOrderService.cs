using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrderApi.Application.DTOs;

namespace OrderApi.Application.Services
{
    public interface IOrderService
    {
        Task<IEnumerable<OrderDTO>> GetOrdersByUserId(int userId);
        Task<OrderDetailsDTO?> GetOrderDetails(int orderId);
    }
}
