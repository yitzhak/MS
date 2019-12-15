using ClothingStore.WebApi.Models;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ClothingStore.WebApi.Helpers
{
    public interface ITokenHandler
    {
        string CreateUserToken(User user);
    }

    public class TokenHandler : ITokenHandler
    {
        public string CreateUserToken(User user)
        {
            ////Get user details for the user who is trying to login - JRozario
            //var user = UserList.SingleOrDefault(x => x.USERID == UserID);

            ////Authenticate User, Check if its a registered user in DB  - JRozario
            //if (user == null)
            //    return null;

            //If its registered user, check user password stored in DB - JRozario
            //For demo, password is not hashed. Its just a string comparison - JRozario
            //In reality, password would be hashed and stored in DB. Before comparing, hash the password - JRozario
            //Authentication successful, Issue Token with user credentials - JRozario
            //Provide the security key which was given in the JWToken configuration in Startup.cs - JRozario
            var key = Encoding.ASCII.GetBytes("YourKey-2374-OFFKDI940NG7:56753253-tyuw-5769-0921-kfirox29zoxv");
            //Generate Token for user - JRozario
            var JWToken = new JwtSecurityToken(
                issuer: "http://localhost:45092/",
                audience: "http://localhost:45092/",
                claims: GetUserClaims(user),
                notBefore: new DateTimeOffset(DateTime.Now).DateTime,
                expires: new DateTimeOffset(DateTime.Now.AddDays(30)).DateTime,
                //Using HS256 Algorithm to encrypt Token - JRozario
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            );
            var token = new JwtSecurityTokenHandler().WriteToken(JWToken);
            return token;
        }

        private IEnumerable<Claim> GetUserClaims(User user)
        {
            List<Claim> claims = new List<Claim>();
            Claim _claim;
            _claim = new Claim(ClaimTypes.Name, user.FirstName + " " + user.LastName);
            claims.Add(_claim);
            _claim = new Claim("USERID", user.Id.ToString());
            claims.Add(_claim);
            _claim = new Claim(user.AccessLevel.ToString(), user.AccessLevel.ToString());
            claims.Add(_claim);

            return claims.AsEnumerable<Claim>();
        }


    }
}
