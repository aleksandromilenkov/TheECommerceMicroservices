using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using OrderApi.Application.DTOs;
using OrderApi.Application.Interfaces;
using Polly;
using Polly.Registry;

namespace OrderApi.Application.Services
{
    public class OrderService(IOrder orderRepository, HttpClient httpClient, ResiliencePipelineProvider<string> resiliencePipeline, IMapper mapper) : IOrderService
    {

        // GET PRODUCT
        private async Task<ProductDTO> GetProduct(int productId)
        {
            // Call ProductAPI using HttpClient
            // Redirect this call to the API Gateway since ProductAPI is not responding to outsiders
            var getProduct = await httpClient.GetAsync($"/api/products/{productId}");
            if (!getProduct.IsSuccessStatusCode)
            {
                return null!;
            }
            var product = await getProduct.Content.ReadFromJsonAsync<ProductDTO>();
            return product!;
        }

        // GET USER
        private async Task<AppUserDTO> GetUser(int userId)
        {
            // Call ProductAPI using HttpClient
            // Redirect this call to the API Gateway since ProductAPI is not responding to outsiders
            var getUser = await httpClient.GetAsync($"http://localhost:5000/api/Authentication/{userId}");
            if (!getUser.IsSuccessStatusCode)
            {
                return null!;
            }
            var user = await getUser.Content.ReadFromJsonAsync<AppUserDTO>();
            return user!;
        }

        // GET ORDER DETAILS By ID
        public async Task<OrderDetailsDTO?> GetOrderDetails(int orderId)
        {
            // Prepare Order
            var order = await orderRepository.FindByIdAsync(orderId);
            if (order == null || order!.Id <= 0) return null;

            // Get Retry pipeline
            var retryPipeline = resiliencePipeline.GetPipeline("my-retry-pipeline");

            // Prepare Product
            var productDTO = await retryPipeline.ExecuteAsync(async token => await GetProduct(order.ProductId));

            // Prepare User
            var appUserDTO = await retryPipeline.ExecuteAsync(async token => await GetUser(order.UserId));

            // Populate OrderDetails
            return new OrderDetailsDTO(
                orderId,
                productDTO.Id,
                appUserDTO.Id,
                appUserDTO.Email,
                appUserDTO.TelephoneNumber,
                productDTO.ProductName,
                order.PurchaseQuantity,
                productDTO.Price,
                order.PurchaseQuantity * productDTO.Price,
                order.OrderedDate
                );
        }

        // GET ORDERS BY USER ID
        public async Task<IEnumerable<OrderDTO>> GetOrdersByUserId(int userId)
        {
            var orders = await orderRepository.GetOrdersAsync((o=> o.UserId == userId));
            var mappedOrders = mapper.Map<List<OrderDTO>>(orders);
            return mappedOrders;
        }
    }
}
