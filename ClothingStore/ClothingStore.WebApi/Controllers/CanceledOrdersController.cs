using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ClothingStore.WebApi.Models;
using ClothingStore.WebApi.Services;
using ClothingStore.WebApi.Attributes;
using ClothingStore.WebApi.Enum;
using ClothingStore.WebApi.DTO;

namespace ClothingStore.WebApi.Controllers
{
    [Authorize(AccessLevel.Salesman, AccessLevel.Administrator)]
    [Route("api/[controller]")]
    [ApiController]
    public class CanceledOrdersController : ControllerBase
    {
        private readonly ICanceledOrdersService _canceledOrdersService;

        public CanceledOrdersController(ICanceledOrdersService canceledOrdersService)
        {
            _canceledOrdersService = canceledOrdersService;
        }

        // POST: api/CanceledOrders
        [HttpPost]
        public async Task<ActionResult<CustomerPaybackDTO>> PostCanceledOrder(CanceledOrder canceledOrder)
        {
            CustomerPaybackDTO customerPayback = await _canceledOrdersService.PostCanceledOrder(canceledOrder);
            return customerPayback;
        }
    }
}
