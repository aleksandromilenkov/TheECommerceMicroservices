using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using ECommerce.SharedLibrary.Logs;
using ECommerce.SharedLibrary.Responses;
using Microsoft.EntityFrameworkCore;
using ProductApi.Application.Entites;
using ProductApi.Application.Interfaces;
using ProductApi.Infrastructure.Data;

namespace ProductApi.Infrastructure.Repositories
{
    public class ProductRepository(ProductDbContext context) : IProduct
    {
        public async Task<Response> CreateAsync(Product entity)
        {
            try
            {
                // check if product already exists
                var getProduct = await GetByAsync((p => p.ProductName!.Equals(entity.ProductName)));
                if (getProduct is not null && !string.IsNullOrEmpty(getProduct.ProductName)) {
                    return new Response(false, $"{entity.ProductName} already added.");
                }
                var currentEntity = context.Products.Add(entity).Entity;
                await context.SaveChangesAsync();
                if (currentEntity is not null && currentEntity.Id > 0) {
                    return new Response(true, $"{entity.ProductName} added to the database successfully.");
                }else {
                    return new Response(false, $"Error occurred when adding {entity.ProductName}.");
                }
            }
            catch (Exception ex) { 
                //Log the original exception
                LogException.LogExceptions(ex);

                //display user-friendly message to the client
                return new Response(false, "Error occurred adding new product");
            }
        }

        public async Task<Response> DeleteAsync(Product entity)
        {
            try
            {
                var product = await FindByIdAsync(entity.Id);
                if(product is null) {
                    return new Response(false, $"{entity.ProductName} not found.");

                }
                context.Products.Remove(entity);
                await context.SaveChangesAsync();
                return new Response(true, $"{entity.ProductName} is deleted successfully.");

            } catch(Exception ex) {
                //Log the original exception
                LogException.LogExceptions(ex);

                //display user-friendly message to the client
                return new Response(false, "Error occurred adding new product");
            }
        }

        public async Task<Product?> FindByIdAsync(int id)
        {
            try
            {
                var product = await context.Products.FindAsync(id);
                return product is null ? null : product;
            }
            catch (Exception ex)
            {
                //Log the original exception
                LogException.LogExceptions(ex);

                //display user-friendly message to the client
                throw new Exception("Error occurred retreiving product");
            }
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            try
            {
                var products = await context.Products.ToListAsync();
                return products;
            }
            catch (Exception ex)
            {
                //Log the original exception
                LogException.LogExceptions(ex);

                //display user-friendly message to the client
                throw new Exception("Error occurred retreiving products");
            }
        }

        public async Task<Product?> GetByAsync(Expression<Func<Product, bool>> predicate)
        {
            try
            {
                var product = await context.Products.Where(predicate).FirstOrDefaultAsync();
                return product is null ? null : product;
            }
            catch (Exception ex)
            {
                //Log the original exception
                LogException.LogExceptions(ex);

                //display user-friendly message to the client
                throw new Exception("Error occurred retreiving product");
            }
        }

        public async Task<Response> UpdateAsync(Product entity)
        {
            try
            {
                var product = await FindByIdAsync(entity.Id);
                if (product is null) { 
                    return new Response(false, $"Product with name {entity.ProductName} does not exist");
                }
                context.Entry(product).State = EntityState.Detached;
                context.Products.Update(entity);
                await context.SaveChangesAsync();
                return new Response(true, $"{entity.ProductName} is successfully updated.");
            }
            catch (Exception ex)
            {
                //Log the original exception
                LogException.LogExceptions(ex);

                //display user-friendly message to the client
                return new Response(false, "Error occurred updating existing product");
            }
        }
    }
}
