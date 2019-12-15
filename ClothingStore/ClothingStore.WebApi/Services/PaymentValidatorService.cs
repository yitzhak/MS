using ClothingStore.WebApi.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClothingStore.WebApi.Services
{
    public interface IPaymentValidatorService
    {
        Task<bool> ValidatCoupon(decimal amount, string paymentIdentity);
    }

    public class PaymentValidatorService : IPaymentValidatorService
    {
        private readonly StoreContext db;

        public PaymentValidatorService(StoreContext context)
        {
            db = context;
        }

        public async Task<bool> ValidatCoupon(decimal amount, string paymentIdentity)
        {
            bool isExist =await db.Coupons.AnyAsync(p => !p.IsUsed && p.Id.ToString() == paymentIdentity
            && p.Amount == amount);
            return isExist;
        }

    }

}
