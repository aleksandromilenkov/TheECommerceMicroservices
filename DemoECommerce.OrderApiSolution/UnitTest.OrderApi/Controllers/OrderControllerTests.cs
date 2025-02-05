using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using ECommerce.SharedLibrary.Responses;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using OrderApi.Application.DTOs;
using OrderApi.Application.Interfaces;
using OrderApi.Application.Services;
using OrderApi.Domain.Entities;
using OrderApi.Presentation.Controllers;
using Xunit;
using static System.Net.Mime.MediaTypeNames;

namespace UnitTest.OrderApi.Controllers
{
    public class OrderControllerTests
    {
        private readonly IOrder orderInterface;
        private readonly IMapper mapper;
        private readonly OrdersController orderController;
        private readonly IOrderService orderService;

        public OrderControllerTests()
        {
            // Set up dependencies
            orderInterface = A.Fake<IOrder>();
            orderService = A.Fake<IOrderService>();
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Order, OrderDTO>();
                cfg.CreateMap<OrderDTO, Order>();
                cfg.CreateMap<OrderCreateDTO, Order>()
                    .ForMember(dest => dest.OrderedDate, opt => opt.MapFrom(src => src.OrderedDate ?? DateTime.UtcNow));
            });
            mapper = config.CreateMapper();

            // Set up System Under Test - SUT
            orderController = new OrdersController(orderInterface, orderService, mapper);
        }

        [Fact]
        public async Task GetOrders_WhenOrdersExist_ReturnsListOfOrders()
        {
            // Arrange
            var orders = new List<Order>
            {
                new Order { Id = 1, ProductId = 101, UserId = 1001, PurchaseQuantity = 2, OrderedDate = DateTime.UtcNow },
                new Order { Id = 2, ProductId = 102, UserId = 1002, PurchaseQuantity = 1, OrderedDate = DateTime.UtcNow }
            };
           A.CallTo(()=> orderInterface.GetAllAsync()).Returns(orders);

            // Act
            var result = await orderController.GetOrders();

            // Assert
            var actionResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<List<OrderDTO>>(actionResult.Value);
            returnValue.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetOrderById_WhenOrderExists_ReturnsOrder()
        {
            // Arrange
            var order = new Order { Id = 1, ProductId = 101, UserId = 1001, PurchaseQuantity = 2, OrderedDate = DateTime.UtcNow };
            A.CallTo(() => orderInterface.FindByIdAsync(1)).Returns(Task.FromResult(order));

            // Act
            var result = await orderController.GetOrderById(1);

            // Assert
            var actionResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<OrderDTO>(actionResult.Value);
            returnValue.Id.Should().Be(1);
        }

        [Fact]
        public async Task GetOrderById_WhenOrderDoesNotExist_ReturnsNotFound()
        {
            // Arrange
            A.CallTo(() => orderInterface.FindByIdAsync(99)).Returns(Task.FromResult<Order>(null));

            // Act
            var result = await orderController.GetOrderById(99);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task CreateOrder_WhenValidOrder_ReturnsSuccessResponse()
        {
            // Arrange
            var orderCreateDTO = new OrderCreateDTO(101, 1001, 2, null);
            var order = mapper.Map<Order>(orderCreateDTO);
            var response = new Response
            {
                Flag = true,
                Message = "Order successfully created"
            };

            A.CallTo(() => orderInterface.CreateAsync(A<Order>.Ignored)).Returns(Task.FromResult(response));

            // Act
            var result = await orderController.CreateOrder(orderCreateDTO);

            // Assert
            var actionResult = result.Result as OkObjectResult;
            actionResult.Should().NotBeNull();
            actionResult.StatusCode.Should().Be(StatusCodes.Status200OK);
            var returnedResponse = actionResult.Value as Response;
            returnedResponse.Should().NotBeNull();
            returnedResponse.Flag.Should().BeTrue();
            returnedResponse.Message.Should().Be("Order successfully created");
        }

        [Fact]
        public async Task CreateOrder_WhenInvalidOrder_ReturnsBadRequest()
        {
            // Arrange
            orderController.ModelState.AddModelError("ProductId", "Required");

            // Act
            var result = await orderController.CreateOrder(new OrderCreateDTO(0, 1001, 2, null));

            // Assert
            var actionResult = result.Result as BadRequestObjectResult;
            actionResult.Should().NotBeNull();
            actionResult.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        }

        [Fact]
        public async Task UpdateOrder_WhenOrderExists_ReturnsSuccessResponse()
        {
            // Arrange
            var orderDTO = new OrderDTO(1, 101, 1001, 2, DateTime.UtcNow);
            var response = new Response
            {
                Flag = true,
                Message = "Order updated successfully."
            };

            A.CallTo(() => orderInterface.UpdateAsync(A<Order>._)).Returns(response);

            // Act
            var result = await orderController.UpdateOrder(orderDTO);

            // Assert
            var actionResult = result.Result as OkObjectResult;
            actionResult.Should().NotBeNull();
            actionResult.StatusCode.Should().Be(StatusCodes.Status200OK);
            var returnedResponse = actionResult.Value as Response;
            returnedResponse.Should().NotBeNull();
            returnedResponse.Flag.Should().BeTrue();
            returnedResponse.Message.Should().Be("Order updated successfully.");
        }

        [Fact]
        public async Task UpdateOrder_WhenOrderDoesNotExist_ReturnsBadRequest()
        {
            // Arrange
            var orderDTO = new OrderDTO(99, 101, 1001, 2, DateTime.UtcNow);
            var order = mapper.Map<Order>(orderDTO);
            var response = new Response
            {
                Flag = false,
                Message = "Order not found."
            };

            A.CallTo(() => orderInterface.UpdateAsync(order)).Returns(response);

            // Act
            var result = await orderController.UpdateOrder(orderDTO);

            // Assert
            var actionResult = result.Result as BadRequestObjectResult;
            actionResult.Should().NotBeNull();
            actionResult.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
            var returnedResponse = actionResult.Value as Response;
            returnedResponse.Should().NotBeNull();
            returnedResponse.Flag.Should().BeFalse();
        }

        [Fact]
        public async Task DeleteOrder_WhenOrderExists_ReturnsSuccessResponse()
        {
            // Arrange
            var order = new Order { Id = 1, ProductId = 101, UserId = 1001, PurchaseQuantity = 2, OrderedDate = DateTime.UtcNow };
            var response = new Response
            {
                Flag = true,
                Message = "Order deleted successfully."
            };

            A.CallTo(() => orderInterface.FindByIdAsync(1)).Returns(Task.FromResult(order));
            A.CallTo(() => orderInterface.DeleteAsync(order)).Returns(Task.FromResult(response));

            // Act
            var result = await orderController.DeleteOrder(1);

            // Assert
            var actionResult = result.Result as OkObjectResult;
            actionResult.Should().NotBeNull();
            actionResult.StatusCode.Should().Be(StatusCodes.Status200OK);
            var returnedResponse = actionResult.Value as Response;
            returnedResponse.Should().NotBeNull();
            returnedResponse.Flag.Should().BeTrue();
            returnedResponse.Message.Should().Be("Order deleted successfully.");
        }

        [Fact]
        public async Task DeleteOrder_WhenOrderDoesNotExist_ReturnsNotFound()
        {
            // Arrange
            A.CallTo(() => orderInterface.FindByIdAsync(99)).Returns(Task.FromResult<Order>(null));

            // Act
            var result = await orderController.DeleteOrder(99);

            // Assert
            var actionResult = result.Result as NotFoundObjectResult;
            actionResult.Should().NotBeNull();
            actionResult.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        }
    }
}
