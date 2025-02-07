using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using FakeItEasy;
using FluentAssertions;
using OrderApi.Application.DTOs;
using OrderApi.Application.Interfaces;
using OrderApi.Application.Services;
using OrderApi.Domain.Entities;

namespace UnitTest.OrderApi.Services
{
    public class OrderServiceTests
    {
        private readonly IOrderService orderService;
        private readonly IOrder orderRepository;
        public OrderServiceTests()
        {
            orderService = A.Fake<IOrderService>();
            orderRepository = A.Fake<IOrder>();
        }


        // Creating a fake HTTP message Handler
        public class FakeHttpMessageHandler(HttpResponseMessage response): HttpMessageHandler
        {
            private readonly HttpResponseMessage _response = response;

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                return Task.FromResult(_response);
            }
        }

        // Create fake http client using 
        private static HttpClient CreateFakeHttpClient(object o)
        {
            var httpResponseMessage = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
            {
                Content = JsonContent.Create(o),
            };
            var fakeHttpMessageHandler = new FakeHttpMessageHandler(httpResponseMessage);
            var _httpClient = new HttpClient(fakeHttpMessageHandler)
            {
                BaseAddress = new Uri("http://localhost")
            };
            return _httpClient;
        }

        // Get Product
        [Fact]
        public async Task GetProduct_ValidProductId_ReturnProduct()
        {
            // Arrange
            int productId = 1;
            var productDto = new ProductDTO(productId, "Pr1", 2, 10);
            var _httpClient = CreateFakeHttpClient(productDto);

            // System under test
            // only need httpClient to make calls
            // specify only httpClient and null for the rest(we dont need them right here)
            var _orderService = new OrderService(null, _httpClient, null, null);

            // Act
            var result = await _orderService.GetProduct(productId);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(productId);
            result.ProductName.Should().Be("Pr1");
            result.Quantity.Should().Be(2);
            result.Price.Should().Be(10);
        }
        [Fact]
        public async Task GetProduct_InvalidProductId_ReturnNull()
        {
            // Arrange
            int productId = 10;
            var _httpClient = CreateFakeHttpClient(null!);
            var _orderService = new OrderService(null, _httpClient, null, null);

            // Act
            var result = await _orderService.GetProduct(productId);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetUser_ValidUserId_ReturnUser()
        {
            // Arrange
            int userId = 1;
            var userDTO = new AppUserDTO(userId, "NovUser", "123345677", "adresa", "asd@asd.com", "test12345678", "admin");
            var httpClient = CreateFakeHttpClient(userDTO);
            var orderService = new OrderService(null, httpClient, null, null);

            // Act
            var result = await orderService.GetUser(userId);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(userId);
            result.Name.Should().Be("NovUser");
            result.TelephoneNumber.Should().Be("123345677");
            result.Address.Should().Be("adresa");
            result.Email.Should().Be("asd@asd.com");
            result.Password.Should().Be("test12345678");
            result.Role.Should().Be("admin");
        }

        [Fact]
        public async Task GetUser_InvalidUserId_ReturnNull()
        {
            // Arrange
            int userId = 1;
            var httpClient = CreateFakeHttpClient(null);
            var orderService = new OrderService(null, httpClient, null, null);

            // Act
            var result = await orderService.GetUser(userId);

            // Assert
            result.Should().BeNull();
        }

        // Get User Orders By Id
        [Fact]
        public async Task GetOrdersByUserId_OrderExists_ReturnOrderDetails()
        {
            // Arrange
            int userId = 1;

            var orders = new List<Order>
            {
                new() { Id = 1, ProductId = 1, UserId = userId, PurchaseQuantity = 100, OrderedDate= new DateTime(2025, 01, 05, 00, 10, 50) },
                new() { Id = 2, ProductId= 2, UserId = userId, PurchaseQuantity = 200, OrderedDate = new DateTime(2025, 01, 05, 00, 10, 52)}
            };

            var mappedOrders = new List<OrderDTO>
            {
                new(1, 1, userId, 100, new DateTime(2025, 01, 05, 00, 10, 50)),
                new(2, 2, userId, 200, new DateTime(2025, 01, 05, 00, 10, 52))
            };

            // fake repository to return orders when queried by userId
            A.CallTo(() => orderRepository.GetOrdersAsync(A<Expression<Func<Order, bool>>>.Ignored))
            .Returns(orders);

            var mockMapper = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Order, OrderDTO>();
            }).CreateMapper();

            var realOrderService = new OrderService(orderRepository, null, null, mockMapper);
            // Act
            var result = await realOrderService.GetOrdersByUserId(userId);

            // Assert
            result.Should().NotBeNull().And.HaveCount(2);
            result.Should().BeEquivalentTo(mappedOrders);
        }

        [Fact]
        public async Task GetOrdersByUserId_OrderEmpty_ReturnOrderDetails()
        {
            // Arrange
            int userId = 1;

            var orders = new List<Order>
            {
            };

            var mappedOrders = new List<OrderDTO>
            {
            };

            // fake repository to return orders when queried by userId
            A.CallTo(() => orderRepository.GetOrdersAsync(A<Expression<Func<Order, bool>>>.Ignored))
            .Returns(orders);

            var mockMapper = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Order, OrderDTO>();
            }).CreateMapper();

            var realOrderService = new OrderService(orderRepository, null, null, mockMapper);
            // Act
            var result = await realOrderService.GetOrdersByUserId(userId);

            // Assert
            result.Should().NotBeNull().And.HaveCount(0);
        }
    }
}
