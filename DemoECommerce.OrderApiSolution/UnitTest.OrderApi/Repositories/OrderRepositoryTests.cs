using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ECommerce.SharedLibrary.Responses;
using Microsoft.EntityFrameworkCore;
using OrderApi.Domain.Entities;
using OrderApi.Infrastructure.Data;
using OrderApi.Infrastructure.Repositories;
using Xunit;
using FluentAssertions;

namespace OrderApi.Tests.Infrastructure.Repositories
{
    public class OrderRepositoryTests
    {
        private OrderDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<OrderDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new OrderDbContext(options);
        }

        [Fact]
        public async Task CreateAsync_ShouldCreateOrder()
        {
            // Arrange
            var context = GetDbContext();
            var repository = new OrderRepository(context);
            var order = new Order { Id = 1, UserId = 1, ProductId = 1, PurchaseQuantity = 10, OrderedDate = DateTime.UtcNow };

            // Act
            var response = await repository.CreateAsync(order);

            // Assert
            response.Flag.Should().BeTrue();
            response.Message.Should().Be("Order successfully created");
            (await context.Orders.FindAsync(1)).Should().NotBeNull();
        }

        [Fact]
        public async Task DeleteAsync_ShouldDeleteOrder()
        {
            // Arrange
            var context = GetDbContext();
            var repository = new OrderRepository(context);
            var order = new Order { Id = 1, UserId = 1, ProductId = 1, PurchaseQuantity = 10, OrderedDate = DateTime.UtcNow };
            await context.Orders.AddAsync(order);
            await context.SaveChangesAsync();

            // Act
            var response = await repository.DeleteAsync(order);

            // Assert
            response.Flag.Should().BeTrue();
            response.Message.Should().Be("Order successfully removed");
            (await context.Orders.FindAsync(1)).Should().BeNull();
        }

        [Fact]
        public async Task FindByIdAsync_ShouldReturnOrder()
        {
            // Arrange
            var context = GetDbContext();
            var repository = new OrderRepository(context);
            var order = new Order { Id = 1, UserId = 1, ProductId = 1, PurchaseQuantity = 10, OrderedDate = DateTime.UtcNow };
            await context.Orders.AddAsync(order);
            await context.SaveChangesAsync();

            // Act
            var result = await repository.FindByIdAsync(1);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(1);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllOrders()
        {
            // Arrange
            var context = GetDbContext();
            var repository = new OrderRepository(context);
            var orders = new List<Order>
            {
                new() { Id = 1, UserId = 1, ProductId = 1, PurchaseQuantity = 10, OrderedDate = DateTime.UtcNow },
                new() { Id = 2, UserId = 2, ProductId = 2, PurchaseQuantity = 20, OrderedDate = DateTime.UtcNow }
            };
            await context.Orders.AddRangeAsync(orders);
            await context.SaveChangesAsync();

            // Act
            var result = await repository.GetAllAsync();

            // Assert
            result.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetOrdersAsync_ShouldReturnFilteredOrders()
        {
            // Arrange
            var context = GetDbContext();
            var repository = new OrderRepository(context);
            var orders = new List<Order>
            {
                new() { Id = 1, UserId = 1, ProductId = 1, PurchaseQuantity = 10, OrderedDate = DateTime.UtcNow },
                new() { Id = 2, UserId = 2, ProductId = 2, PurchaseQuantity = 20, OrderedDate = DateTime.UtcNow }
            };
            await context.Orders.AddRangeAsync(orders);
            await context.SaveChangesAsync();

            // Act
            var result = await repository.GetOrdersAsync(o => o.UserId == 1);

            // Assert
            result.Should().HaveCount(1);
            result.First().UserId.Should().Be(1);
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateOrder()
        {
            // Arrange
            var context = GetDbContext();
            var repository = new OrderRepository(context);
            var order = new Order { Id = 1, UserId = 1, ProductId = 1, PurchaseQuantity = 10, OrderedDate = DateTime.UtcNow };
            await context.Orders.AddAsync(order);
            await context.SaveChangesAsync();

            order.PurchaseQuantity = 50;

            // Act
            var response = await repository.UpdateAsync(order);

            // Assert
            response.Flag.Should().BeTrue();
            response.Message.Should().Be("Order successfully updated");
            (await context.Orders.FindAsync(1))!.PurchaseQuantity.Should().Be(50);
        }
    }
}
