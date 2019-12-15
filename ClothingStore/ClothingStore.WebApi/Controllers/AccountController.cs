using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClothingStore.WebApi.Attributes;
using ClothingStore.WebApi.DTO;
using ClothingStore.WebApi.Enum;
using ClothingStore.WebApi.Helpers;
using ClothingStore.WebApi.Models;
using ClothingStore.WebApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ClothingStore.WebApi.Controllers
{

    public class AccountController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ITokenHandler _tokenHandler;

        public AccountController(IUserService userService, ITokenHandler tokenHandler)
        {
            _userService = userService;
            _tokenHandler = tokenHandler;
        }

        // POST: api/Account/Login
        [HttpPost]
        [Route("api/Account/Login")]
        public async Task<string> Login([FromBody] LoginDTO dto)
        {
            User user = await _userService.Authenticate(dto.Username, dto.Password);
            string token = _tokenHandler.CreateUserToken(user);
            return token;
        }

        // POST: api/Account/Register
        [Authorize(AccessLevel.Administrator)]
        [HttpPost]
        [Route("api/Account/Register")]
        public async Task<string> Register([FromBody] User user)
        {
            int userId = await _userService.Creat(user);
            string token = _tokenHandler.CreateUserToken(user);
            return token;
        }

    }
}
