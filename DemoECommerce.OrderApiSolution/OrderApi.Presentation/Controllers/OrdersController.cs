using AutoMapper;
using ECommerce.SharedLibrary.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrderApi.Application.DTOs;
using OrderApi.Application.Interfaces;
using OrderApi.Application.Services;
using OrderApi.Domain.Entities;

namespace OrderApi.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrdersController(IOrder orderRepository, IOrderService orderService, IMapper mapper) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderDTO>>> GetOrders()
        {
            var orders = await orderRepository.GetAllAsync();
            var mappedOrders = mapper.Map<List<OrderDTO>>(orders);
            return Ok(mappedOrders);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<OrderDTO>> GetOrderById([FromRoute] int id)
        {
            var order = await orderRepository.FindByIdAsync(id);
            if (order == null)
            {
                return NotFound("Order not found.");
            }
            var mappedOrder = mapper.Map<OrderDTO>(order);
            return Ok(mappedOrder);
        }

        [HttpGet("client/{userId:int}")]
        public async Task<ActionResult<List<OrderDTO>>> GetUserOrders([FromRoute] int userId)
        {
            if(userId < 0) { return BadRequest("Invalid userId"); }
            var orders = await orderService.GetOrdersByUserId(userId);
            return Ok(orders);
        }

        [HttpGet("details/{orderId:int}")]
        public async Task<ActionResult<OrderDetailsDTO>> GetOrderDetails([FromRoute] int orderId)
        {
            if (orderId < 0) { return BadRequest("Invalid userId"); }
            var orderDetails = await orderService.GetOrderDetails(orderId);
            return orderDetails == null ? NotFound("Order with that Id does not exists.") : Ok(orderDetails);
        }

        [HttpPost]
        public async Task<ActionResult<Response>> CreateOrder([FromBody] OrderCreateDTO orderToCreate)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Bad Request");
            }
            var order = mapper.Map<Order>(orderToCreate);
            var resp = await orderRepository.CreateAsync(order);
            return resp.Flag is true ? Ok(resp) : BadRequest(resp);
        }

        [HttpPut]
        public async Task<ActionResult<Response>> UpdateOrder([FromBody] OrderDTO orderToUpdate)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Bad Request");
            }
            var order = mapper.Map<Order>(orderToUpdate);
            var resp = await orderRepository.UpdateAsync(order);
            return resp.Flag is true ? Ok(resp) : BadRequest(resp);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<Response>> DeleteOrder([FromRoute] int id)
        {
            if (id < 0)
            {
                return BadRequest("Bad Request");
            }
            var order = await orderRepository.FindByIdAsync(id);
            if (order == null)
            {
                return NotFound("Order not found.");
            }
            var resp = await orderRepository.DeleteAsync(order);
            return resp.Flag is true ? Ok(resp) : BadRequest(resp);
        }
    }
}
