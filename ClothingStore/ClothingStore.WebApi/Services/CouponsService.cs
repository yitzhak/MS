using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ClothingStore.WebApi.Models;
using ClothingStore.WebApi.Helpers;

namespace ClothingStore.WebApi.Services
{
    public interface ICouponsService
    {
        bool CouponExists(int id);
        Task<Coupon> DeleteCoupon(int id);
        Task<Coupon> GetCoupon(int id);
        Task<List<Coupon>> GetCoupons();
        Task<Coupon> CreateCoupon(decimal amount, int orderId);
        Task PutCoupon(int id, Coupon coupon);
    }

    public class CouponsService : ICouponsService
    {
        private readonly StoreContext _context;

        public CouponsService(StoreContext context)
        {
            _context = context;
        }

        public async Task<List<Coupon>> GetCoupons()
        {
            return await _context.Coupons.ToListAsync();
        }

        public async Task<Coupon> GetCoupon(int id)
        {
            var coupon = await _context.Coupons.FindAsync(id);
            return coupon;
        }

        public async Task PutCoupon(int id, Coupon coupon)
        {
            _context.Entry(coupon).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CouponExists(id))
                {
                    throw new BadRequestException(Enum.InternalCode.NotFound, "Coupon not found");
                }
                else
                {
                    throw;
                }
            }
        }

        public async Task<Coupon> CreateCoupon(decimal amount,int orderId)
        {
            Coupon coupon = new Coupon(amount, orderId);
            _context.Coupons.Add(coupon);
            await _context.SaveChangesAsync();

            return coupon;
        }

        public async Task<Coupon> DeleteCoupon(int id)
        {
            var coupon = await _context.Coupons.FindAsync(id);
            if (coupon == null)
            {
                throw new BadRequestException(Enum.InternalCode.NotFound, "Coupon not found");
            }

            _context.Coupons.Remove(coupon);
            await _context.SaveChangesAsync();

            return coupon;
        }

        public bool CouponExists(int id)
        {
            return _context.Coupons.Any(e => e.Id == id);
        }
    }
}
