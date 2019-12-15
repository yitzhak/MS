using System;
using System.Threading.Tasks;
using ClothingStore.WebApi.Attributes;
using ClothingStore.WebApi.Enum;
using ClothingStore.WebApi.Models;
using ClothingStore.WebApi.Services;
using Microsoft.AspNetCore.Mvc;
//using Microsoft.Extensions.Logging;

namespace ClothingStore.WebApi.Controllers
{
    [Authorize(AccessLevel.Salesman, AccessLevel.Administrator)]
    public class CheckoutController : ControllerBase
    {
        private readonly IBaseServise _baseServise;
        private readonly ICheckoutService _checkoutService;
        private readonly IProductPiecesAvailableService _productPiecesAvailableService;

        public CheckoutController(IBaseServise baseServise,
            IProductPiecesAvailableService productPiecesAvailableService,
            ICheckoutService checkoutService)
        {
            _baseServise = baseServise;
            _productPiecesAvailableService = productPiecesAvailableService;
            _checkoutService = checkoutService;
        }


        [HttpPost]
        [Route("Api/Checkout/AddressAndPayment")]
        public async Task<IActionResult> AddressAndPayment([FromBody] Order order)
        {
            try
            {
                int orderId = await _checkoutService.AddressAndPayment(order);
                return Ok(orderId);

            }
            catch (Exception ex)
            {
                _baseServise.Logger.Error("User {userName} Throw exception in checkout {Exception}.", order.Username, ex);
                throw;
            }
        }
    }
}