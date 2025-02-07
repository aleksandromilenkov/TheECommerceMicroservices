using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using ECommerce.SharedLibrary.Logs;
using ECommerce.SharedLibrary.Responses;
using Microsoft.EntityFrameworkCore;
using OrderApi.Application.Interfaces;
using OrderApi.Domain.Entities;
using OrderApi.Infrastructure.Data;

namespace OrderApi.Infrastructure.Repositories
{
    public class OrderRepository(OrderDbContext context) : IOrder
    {
        public async Task<Response> CreateAsync(Order entity)
        {
            try
            {
                await context.AddAsync(entity);
                await context.SaveChangesAsync();
                return new Response()
                {
                    Flag = true,
                    Message = "Order successfully created"
                };
            }
            catch (Exception ex)
            {
                // Log Original Exception
                LogException.LogExceptions(ex);

                //display user-friendly message to the client
                throw new Exception("Error occurred creating order");
            }
        }

        public async Task<Response> DeleteAsync(Order entity)
        {
            try
            {
                var order = await context.Orders.AsNoTracking().FirstOrDefaultAsync(o=>o.Id == entity.Id);
                if(order == null)
                {
                    return new Response()
                    {
                        Flag = false,
                        Message = "Could not remove order because order does not exists."
                    };
                }
                context.Remove(entity);
                await context.SaveChangesAsync();
                return new Response()
                {
                    Flag = true,
                    Message = "Order successfully removed"
                };
            }
            catch (Exception ex) {
                // Log Original Exception
                LogException.LogExceptions(ex);

                //display user-friendly message to the client
                throw new Exception("Error occurred deliting order");
            }
        }

        public async Task<Order?> FindByIdAsync(int id)
        {
            try
            {
                return await context.Orders.FindAsync(id);
            }
            catch (Exception ex)
            {
                // Log Original Exception
                LogException.LogExceptions(ex);

                //display user-friendly message to the client
                throw new Exception("Error occurred retreiving order");
            }
        }

        public async Task<IEnumerable<Order>> GetAllAsync()
        {
            try
            {
                return await context.Orders.ToListAsync();
            }
            catch (Exception ex)
            {
                // Log Original Exception
                LogException.LogExceptions(ex);

                //display user-friendly message to the client
                throw new Exception("Error occurred retreiving orders");
            }
        }

        public async Task<Order?> GetByAsync(Expression<Func<Order, bool>> predicate)
        {
            try
            {
                var order = await context.Orders.FirstOrDefaultAsync(predicate);
                return order == null ? null! : order;
            }
            catch (Exception ex)
            {
                // Log Original Exception
                LogException.LogExceptions(ex);

                //display user-friendly message to the client
                throw new Exception("Error occurred retreiving order");
            }
        }

        public async Task<IEnumerable<Order>> GetOrdersAsync(Expression<Func<Order, bool>> predicate)
        {
            try
            {

            return await context.Orders.Where(predicate).ToListAsync();
            }
            catch (Exception ex)
            {
                // Log Original Exception
                LogException.LogExceptions(ex);

                //display user-friendly message to the client
                throw new Exception("Error occurred retreiving orders");
            }
        }

        public async Task<Response> UpdateAsync(Order entity)
        {
            try
            {
                var order = await context.Orders.AsNoTracking().FirstOrDefaultAsync(o=> o.Id == entity.Id);
                if(order == null)
                {
                    return new Response()
                    {
                        Flag = true,
                        Message = "Could not update the Order because Order does not exist."
                    };
                }
                context.Update(entity);
                await context.SaveChangesAsync();
                return new Response()
                {
                    Flag = true,
                    Message = "Order successfully updated"
                };
            }
            catch (Exception ex)
            {
                // Log Original Exception
                LogException.LogExceptions(ex);

                //display user-friendly message to the client
                throw new Exception("Error occurred updating order");
            }
        }
    }
}
