using AutoMapper;
using ECommerce.SharedLibrary.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProductApi.Application.DTOs;
using ProductApi.Application.Entites;
using ProductApi.Application.Interfaces;

namespace ProductApi.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController(IProduct productRepository, IMapper mapper) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetProducts()
        {
            var products = await productRepository.GetAllAsync();
            var mappedProducts = mapper.Map<List<ProductDTO>>(products);
            return mappedProducts;
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ProductDTO>> GetProductById([FromRoute] int id)
        {
            var product = await productRepository.FindByIdAsync(id);
            if (product == null) {
                return NotFound("Product not found.");
            }
            var mappedProduct = mapper.Map<ProductDTO>(product);
            return mappedProduct;
        }

        [HttpPost]
        public async Task<ActionResult<Response>> CreateProduct([FromBody] ProductCreateDTO productToCreate)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Bad Request");
            }
            var product = mapper.Map<Product>(productToCreate);
            var resp = await productRepository.CreateAsync(product);
            return resp.Flag is true ? Ok(resp) : BadRequest(resp);
        }

        [HttpPut]
        public async Task<ActionResult<Response>> UpdateProduct([FromBody] ProductDTO productToCreate)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Bad Request");
            }
            var product = mapper.Map<Product>(productToCreate);
            var resp = await productRepository.UpdateAsync(product);
            return resp.Flag is true ? Ok(resp) : BadRequest(resp);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<Response>> DeleteProduct([FromRoute] int id)
        {
            if (id < 0)
            {
                return BadRequest("Bad Request");
            }
            var product = await productRepository.FindByIdAsync(id);
            if (product == null) {
                return NotFound("Product not found.");
            }
            var resp = await productRepository.DeleteAsync(product);
            return resp.Flag is true ? Ok(resp) : BadRequest(resp);
        }
    }
}
