using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BDS_WEBAPI.Model;
using BDS_WEBAPI.IRespository;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace BDS_WEBAPI.Controllers
{
    [Route("api/admin")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IUserRespository userRespository;
        private IConfiguration _configuration;
        public AuthenticationController(IConfiguration configuration, IUserRespository _I)
        {
            _configuration = configuration;
            userRespository = _I;
        }
        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult login([FromBody] Users account)
        {
            if(account != null)
            {
                var User= userRespository.GetbyUsername(account.Username!=null? account.Username:"").Result;
                if (User != null)
                {
                    //lấy các thông tin cần có add vào trong token
                    var claims = new[] {
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                        new Claim("username", User.Username),
                        new Claim("Fullname",User.Fullname),
                        new Claim(ClaimTypes.Role, (User.Role==1?"admin":"user"))//thêm role vào đây
                    };
                    var key= new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Appsettings:SecretKey"]));
                    var signIn = new Microsoft.IdentityModel.Tokens.SigningCredentials(key, Microsoft.IdentityModel.Tokens.SecurityAlgorithms.HmacSha256);
                    //khởi tạo token
                    var token = new JwtSecurityToken(
                      null,
                      null,
                      claims,
                      expires: DateTime.UtcNow.AddYears(1),
                      signingCredentials: signIn);
                    return Ok( new ResponseAPI<string>(true,"success login", new JwtSecurityTokenHandler().WriteToken(token)));
                }
                return StatusCode(401, new ResponseAPI<string>(false,"username or password wrong", ""));
            }
            return BadRequest();
        }
        [HttpGet("test")]
       public IActionResult GetProfile()
        {
            //khi gửi request kèm authentication token thì sử dụng ClaimTypes.Role để lấy giá trị Role của token đó
            var userName = HttpContext.User.FindFirst(ClaimTypes.Role)?.Value;
            var username = HttpContext.User.FindFirst("username");
            HttpContext.Response.Redirect("/home/index");
            var full = HttpContext.User.FindFirst("Fullname");
            return Redirect("https://www.facebook.com/");
        }
        
    }
}
