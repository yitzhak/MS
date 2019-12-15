using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClothingStore.WebApi.Attributes;
using ClothingStore.WebApi.DTO;
using ClothingStore.WebApi.Enum;
using ClothingStore.WebApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ClothingStore.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatisticsController : ControllerBase
    {
        private readonly IStatisticsService _statisticsService;

        public StatisticsController(IStatisticsService statisticsService)
        {
            _statisticsService = statisticsService;
        }

        // GET: api/Statistics/5
        [Authorize(AccessLevel.Administrator)]
        [HttpGet("{year}/{month}", Name = "Get")]
        public async Task<List<OrderStatisticsDTO>> Get( int year, int month)
        {
            List<OrderStatisticsDTO> orderStatistics = await _statisticsService.GetOrderStatisticsForMonth(month,year);
            return orderStatistics;
        }
    }
}
