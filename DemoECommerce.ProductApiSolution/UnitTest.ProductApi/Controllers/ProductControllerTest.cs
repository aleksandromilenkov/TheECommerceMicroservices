using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using ECommerce.SharedLibrary.Responses;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProductApi.Application.DTOs;
using ProductApi.Application.Entites;
using ProductApi.Application.Interfaces;
using ProductApi.Infrastructure.Repositories;
using ProductApi.Presentation.Controllers;

namespace UnitTest.ProductApi.Controllers
{
    public class ProductControllerTest
    {
        private readonly IProduct productInterface;
        private readonly IMapper mapper;
        private readonly ProductsController productsController;
        public ProductControllerTest()
        {
            // Set up dependencies
            productInterface = A.Fake<IProduct>();
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Product, ProductDTO>();
                cfg.CreateMap<ProductDTO, Product>();
                cfg.CreateMap<ProductCreateDTO, Product>();
                cfg.CreateMap<Product, ProductCreateDTO>();
                cfg.CreateMap<ProductCreateDTO, Product>();
            });
            mapper = config.CreateMapper();

            // Set up System Under Test - SUT
            productsController = new ProductsController(productInterface, mapper);
        }

        // GET ALL PRODUCTS
        [Fact]
        public async Task GetProduct_ReturnOkResponseWithProducts()
        {
            // Arrange
            var products = new List<Product>()
            { 
              new() { Id = 1, ProductName = "Product 1", Quantity = 10, Price = 100.53m },
              new() { Id = 2,  ProductName = "Product 2", Quantity = 5, Price = 50.99m }
            };

            // Set up fake response for GetAllAsync method
            A.CallTo(()=> productInterface.GetAllAsync()).Returns(products);

            // Act
            var results = await productsController.GetProducts();

            // Assert
            var okResult = results.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(StatusCodes.Status200OK);

            var returnedProducts = okResult.Value as IEnumerable<ProductDTO>;
            returnedProducts.Should().NotBeNull();
            returnedProducts.Count().Should().Be(2);
            returnedProducts.First().Id.Should().Be(1);
            returnedProducts.Last().Id.Should().Be(2);
        }

        [Fact]
        public async Task GetProducts_ReturnsOkResponse_WithEmptyList()
        {
            // Arrange
            var products = new List<Product>(); // Empty list

            // Set up fake response for GetAllAsync method
            A.CallTo(() => productInterface.GetAllAsync()).Returns(products);

            // Act
            var result = await productsController.GetProducts();

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(StatusCodes.Status200OK);
            okResult.Value.Should().BeEquivalentTo(products); // Should return an empty list
        }

        [Fact]
        public async Task CreateProductWhenModelStateIsInvalid_ReturnBadRequest()
        {
            // Arrange
            var product = new ProductCreateDTO ("ProductN1",  2,  4 );

            // Simulate model validation failure
            productsController.ModelState.AddModelError("ProductName", "Required");

            // Act
            var result = await productsController.CreateProduct(product);

            // Assert
            var badRequest = result.Result as BadRequestObjectResult;
            badRequest.Should().NotBeNull();
            badRequest.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        }

        [Fact]
        public async Task CreateProductWhenCreateIsSuccessfull_ReturnOkResponse()
        {
            var productCreateDTO = new ProductCreateDTO("ProductN1", 2, 4);

            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
            new Claim(ClaimTypes.Role, "Admin")
             }, "mock"));

            productsController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

           
            var response = new Response { Flag = true, Message = "Product created successfully." };
            A.CallTo(() => productInterface.CreateAsync(A<Product>._)).Returns(response);

            // Act
            var result = await productsController.CreateProduct(productCreateDTO);

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(StatusCodes.Status200OK);

            var returnedResponse = okResult.Value as Response;
            returnedResponse.Should().NotBeNull();
            returnedResponse.Flag.Should().BeTrue();
            returnedResponse.Message.Should().Be("Product created successfully.");
        }

        [Fact]
        public async Task CreateProduct_WhenCreateFails_ReturnsBadRequestWithResponse()
        {
            // Arrange
            var productCreateDto = new ProductCreateDTO("ProductN1", 2, 4);
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
           {
            new Claim(ClaimTypes.Role, "Admin")
             }, "mock"));

            productsController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            var response = new Response { Flag = false, Message = "Failed to create product" };
            A.CallTo(() => productInterface.CreateAsync(A<Product>._)).Returns(response);

            // Act
            var result = await productsController.CreateProduct(productCreateDto);

            // Assert
            var badRequestResult = result.Result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult.StatusCode.Should().Be(StatusCodes.Status400BadRequest);

            var returnedResponse = badRequestResult.Value as Response;
            returnedResponse.Should().NotBeNull();
            returnedResponse.Flag.Should().BeFalse();
            returnedResponse.Message.Should().Be("Failed to create product");
        }

        [Fact]
        public async Task CreateProduct_WhenUserIsNotAdmin_ReturnsBadRequest()
        {
            // Arrange
            var productCreateDto = new ProductCreateDTO("ProductN1", 2, 4);

            var user = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Role, "User")}, "mock"));

            productsController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            // Act
            var result = await productsController.CreateProduct(productCreateDto);

            // Assert
            var badRequestResult = result.Result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        }

        [Fact]
        public async Task UpdateProduct_WhenUpdateIsSuccessful_ReturnsOk()
        {
            // Arrange
            var productToUpdate = new ProductDTO(2, "asd", 2, 4);

            var response = new Response { Flag = true, Message = "Product updated successfully" };
            A.CallTo(() => productInterface.UpdateAsync(A<Product>._)).Returns(response);

            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
        new Claim(ClaimTypes.Role, "Admin") // Admin user
    }, "mock"));

            productsController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            // Act
            var result = await productsController.UpdateProduct(productToUpdate);

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(StatusCodes.Status200OK);

            var returnedResponse = okResult.Value as Response;
            returnedResponse.Should().NotBeNull();
            returnedResponse.Flag.Should().BeTrue();
            returnedResponse.Message.Should().Be("Product updated successfully");
        }

        [Fact]
        public async Task DeleteProduct_WhenDeleteIsSuccessfull_ReturnOkResponse()
        {
            var productToDelete = new ProductDTO(2, "asd", 2, 4);
            var response = new Response { Flag = true, Message = "Product deleted successfully" };
            A.CallTo(() => productInterface.DeleteAsync(A<Product>._)).Returns(response);
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Role, "Admin") // Admin user
            }, "mock"));

            productsController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
            // Act
            var result = await productsController.DeleteProduct(productToDelete.Id);

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(StatusCodes.Status200OK);

            var returnedResponse = okResult.Value as Response;
            returnedResponse.Should().NotBeNull();
            returnedResponse.Flag.Should().BeTrue();
            returnedResponse.Message.Should().Be("Product deleted successfully");
        }

        [Fact]
        public async Task DeleteProduct_WhenDeleteIsUnuccessfullNegativeId_ReturnBadRequest()
        {
            var productToDelete = new ProductDTO(-2, "asd", 2, 4);
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Role, "Admin") // Admin user
            }, "mock"));

            productsController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
            // Act
            var result = await productsController.DeleteProduct(productToDelete.Id);

            // Assert
            var BadRequestResult = result.Result as BadRequestObjectResult;
            BadRequestResult.Should().NotBeNull();
            BadRequestResult.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        }

        [Fact]
        public async Task DeleteProduct_WhenDeleteIsUnuccessfullProduct_ReturnNotFound()
        {
            var productToDelete = new ProductDTO(9, "Mike", 2, 4);
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Role, "Admin") // Admin user
            }, "mock"));

            productsController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
            A.CallTo(() => productInterface.FindByIdAsync(productToDelete.Id)).Returns(Task.FromResult<Product>(null));

            // Act
            var result = await productsController.DeleteProduct(productToDelete.Id);

            // Assert
            var notFoundResult = result.Result as NotFoundObjectResult;
            notFoundResult.Should().NotBeNull();
            notFoundResult.StatusCode.Should().Be(StatusCodes.Status404NotFound);
            notFoundResult.Value.Should().Be("Product not found.");
        }

        [Fact]
        public async Task DeleteProduct_WhenDeleteIsUnsuccessfull_ReturnBadRequest()
        {
            var productToDelete = new ProductDTO(2, "asd", 2, 4);
            var response = new Response { Flag = false, Message = "Error occurred adding new product" };
            A.CallTo(() => productInterface.DeleteAsync(A<Product>._)).Returns(response);
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Role, "Admin") // Admin user
            }, "mock"));

            productsController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
            // Act
            var result = await productsController.DeleteProduct(productToDelete.Id);

            // Assert
            var badRequestResult = result.Result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult.StatusCode.Should().Be(StatusCodes.Status400BadRequest);

            var returnedResponse = badRequestResult.Value as Response;
            returnedResponse.Should().NotBeNull();
            returnedResponse.Flag.Should().BeFalse();
            returnedResponse.Message.Should().Be("Error occurred adding new product");
        }
    }
}
