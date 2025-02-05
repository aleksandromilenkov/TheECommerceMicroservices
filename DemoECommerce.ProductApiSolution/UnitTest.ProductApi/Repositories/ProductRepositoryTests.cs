using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using FakeItEasy;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using ProductApi.Application.DTOs;
using ProductApi.Application.Entites;
using ProductApi.Application.Interfaces;
using ProductApi.Infrastructure.Data;
using ProductApi.Infrastructure.Repositories;
using ProductApi.Presentation.Controllers;

namespace UnitTest.ProductApi.Repositories
{
    public class ProductRepositoryTests
    {
        private readonly ProductDbContext _context;
        private readonly ProductRepository _repository;
        private readonly IMapper _mapper;
        public ProductRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<ProductDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            _context = new ProductDbContext(options);
            _repository = new ProductRepository(_context);
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Product, ProductDTO>();
                cfg.CreateMap<ProductDTO, Product>();
                cfg.CreateMap<ProductCreateDTO, Product>();
                cfg.CreateMap<Product, ProductCreateDTO>();
                cfg.CreateMap<ProductCreateDTO, Product>();
            });
            _mapper = config.CreateMapper();
        }

        [Fact]
        public async Task CreateAsync_WhenProductAlreadyExist_ReturnErrorResponse()
        {
            // Arrange
            var existingProduct = new Product() { Id = 1, ProductName = "Test", Price = 10, Quantity = 5 };
            _context.Add(existingProduct);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.CreateAsync(existingProduct);

            // Assert
            result.Should().NotBeNull();
            result.Flag.Should().BeFalse();
            result.Message.Should().Be("Test already added.");
        }

        [Fact]
        public async Task CreateAsync_SuccessfullyCreateProduct_ReturnOkResponse()
        {
            // Arrange
            var existingProduct = new Product() { Id = 2, ProductName = "Asd", Price = 10, Quantity = 5 };

            // Act
            var result = await _repository.CreateAsync(existingProduct);

            // Assert
            result.Should().NotBeNull();
            result.Flag.Should().BeTrue();
            result.Message.Should().Be("Asd added to the database successfully.");
        }


        [Fact]
        public async Task DeleteAsync_SuccessfullyDeleteProduct_ReturnOkResponse()
        {
            // Arrange
            var existingProduct = new Product() { Id = 1, ProductName = "Asd", Price = 10, Quantity = 5 };
            _context.Add(existingProduct);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.DeleteAsync(existingProduct);

            // Assert
            result.Should().NotBeNull();
            result.Flag.Should().BeTrue();
            result.Message.Should().Be("Asd is deleted successfully.");
        }

        [Fact]
        public async Task DeleteAsync_ProductNotFound_ReturnNotFound()
        {
            // Arrange
            var existingProduct = new Product() { Id = 1, ProductName = "Asdz", Price = 10, Quantity = 5 };

            // Act
            var result = await _repository.DeleteAsync(existingProduct);

            // Assert
            result.Should().NotBeNull();
            result.Flag.Should().BeFalse();
            result.Message.Should().Be("Error occurred deleting new product");
        }


        [Fact]
        public async Task FindByIdAsync_WhenProductExists_ReturnsProduct()
        {
            // Arrange
            var product = new Product { Id = 5, ProductName = "Test1", Price = 100, Quantity = 10 };
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.FindByIdAsync(5);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(5);
            result.ProductName.Should().Be("Test1");
        }

        [Fact]
        public async Task FindByIdAsync_WhenProductDoesNotExist_ReturnsNull()
        {
            // Act
            var result = await _repository.FindByIdAsync(99);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetAllAsync_WhenProductsExist_ReturnsAllProducts()
        {
            // Arrange
            var products = new List<Product>
            {
                new Product { Id = 1, ProductName = "Product1", Price = 100, Quantity = 5 },
                new Product { Id = 2, ProductName = "Product2", Price = 200, Quantity = 10 }
            };
            _context.Products.AddRange(products);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetAllAsync();

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetAllAsync_WhenNoProductsExist_ReturnsEmptyList()
        {
            // Act
            var result = await _repository.GetAllAsync();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetByAsync_WhenProductMatchesPredicate_ReturnsProduct()
        {
            // Arrange
            var product = new Product { Id = 1, ProductName = "Test", Price = 100, Quantity = 8 };
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetByAsync(p => p.ProductName == "Test");

            // Assert
            result.Should().NotBeNull();
            result.ProductName.Should().Be("Test");
        }

        [Fact]
        public async Task GetByAsync_WhenNoProductMatchesPredicate_ReturnsNull()
        {
            // Act
            var result = await _repository.GetByAsync(p => p.ProductName == "Nopr");

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task UpdateAsync_WhenProductExists_ReturnsSuccessResponse()
        {
            // Arrange
            var existingProduct = new Product { Id = 1, ProductName = "UpdatePr", Price = 100, Quantity = 5 };
            _context.Products.Add(existingProduct);
            await _context.SaveChangesAsync();

            var updatedProduct = new Product { Id = 1, ProductName = "UpdatePr", Price = 120, Quantity = 6 };

            // Act
            var result = await _repository.UpdateAsync(updatedProduct);

            // Assert
            result.Should().NotBeNull();
            result.Flag.Should().BeTrue();
            result.Message.Should().Be("UpdatePr is successfully updated.");
        }

        [Fact]
        public async Task UpdateAsync_WhenProductDoesNotExist_ReturnsErrorResponse()
        {
            // Arrange
            var nonExistingProduct = new Product { Id = 111, ProductName = "Noq", Price = 100, Quantity = 6 };

            // Act
            var result = await _repository.UpdateAsync(nonExistingProduct);

            // Assert
            result.Should().NotBeNull();
            result.Flag.Should().BeFalse();
            result.Message.Should().Be("Product with name Noq does not exist");
        }

    }
}
