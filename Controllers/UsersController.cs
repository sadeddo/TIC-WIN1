using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Web.Http.Cors;
using Etape1.Data;
using Etape1.Models;
using System.Text;  
using System.Security.Cryptography;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
namespace Etape1.Controllers; 



[DisableCors]
[Route("/users")]
[ApiController]
public class UsersController : ControllerBase
{
    
    private readonly DataDbContext dbContext;
    private readonly JwtSettings jwtSettings;
    public UsersController(DataDbContext dbContext,IOptions<JwtSettings> options)
    {
        this.dbContext =dbContext;
        this.jwtSettings = options.Value;
    }

    [Route("")]
    [HttpGet]
    public  async Task<IActionResult> GetUsers()
    {
        var listUsers = await dbContext.Users.ToListAsync();
        IEnumerable<Users> users =  listUsers.OrderBy(a => a.Role);
        return Ok( users );
    }

    

    [Route("{id:guid}")]
    [HttpGet]
    public async Task<IActionResult> GetUser([FromRoute] Guid id)
    {
        var users = await dbContext.Users.ToListAsync();
        var user = users.Where(u => u.Id == id);
        if(user == null)
        {
            return NotFound();
        }
        return Ok(user);

    }

    [System.Web.Http.Authorize]
    [Route("create")]
    [HttpPost]
    public async Task<IActionResult> AddUser(UserRequest UserRequest)
    {
        var user = new Users()
        {
            Id = Guid.NewGuid(),
            
            Email = UserRequest.Email,
            Password = ComputeSha256Hash(UserRequest.Password),
            Role = UserRequest.Role

        };
        await dbContext.Users.AddAsync(user);
        await dbContext.SaveChangesAsync();

        return Ok(user);
    }

    [Route("update/{id:guid}")]
    [Authorize]
    [HttpPut]
    public async Task<IActionResult> UpdateUser([FromRoute] Guid id,UserRequest UserRequest)
    {
        var user = await dbContext.Users.FindAsync(id);
        if(user != null)
        {
            user.Email = UserRequest.Email;
            user.Password = ComputeSha256Hash(UserRequest.Password);
            user.Role = UserRequest.Role;

            await dbContext.SaveChangesAsync();
            return Ok(user);
        }
        return NotFound();
    }

    
    
    [Route("delete/{id:guid}")]
    [Authorize]
    [HttpDelete]
    public async Task<IActionResult> DeleteUser([FromRoute] Guid id)
    {
        var user = await dbContext.Users.FindAsync(id);

        if(user != null)
        {
            dbContext.Remove(user);
            await dbContext.SaveChangesAsync();
            return Ok(user);
        }
        return NotFound();
    }
    
    //for hash password with SHA256
    static string ComputeSha256Hash(string Password)  
        {  
            // Create a SHA256   
            using (SHA256 sha256Hash = SHA256.Create())  
            {  
                // ComputeHash - returns byte array  
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(Password));  
  
                // Convert byte array to a string   
                StringBuilder builder = new StringBuilder();  
                for (int i = 0; i < bytes.Length; i++)  
                {  
                    builder.Append(bytes[i].ToString("x2"));  
                }  
                return builder.ToString();  
            }  
        }  
    
    //Authentificate 
    
    [Route("authentificate")]
    [HttpPost]
    public async Task<IActionResult> Auth([FromBody] AuthUsers model)
    {
        var user = await dbContext.Users.FirstOrDefaultAsync(item=>item.Email==model.Email && item.Password==ComputeSha256Hash(model.Password));
        if(user ==null)
        {
            return Unauthorized();
        } 
        var tokenhandler = new JwtSecurityTokenHandler();
        var tokenkey = Encoding.UTF8.GetBytes(this.jwtSettings.securityKey);
        var tokendesc = new SecurityTokenDescriptor{
            Subject = new ClaimsIdentity(
                new Claim[] { new Claim(ClaimTypes.Name, user.Email), new Claim(ClaimTypes.Role, user.Role), new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())}
            ),
            Expires = DateTime.Now.AddDays(1),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenkey),SecurityAlgorithms.HmacSha256)
        };
        var token = tokenhandler.CreateToken(tokendesc);
        string finaltoken = tokenhandler.WriteToken(token);
        return Ok(finaltoken);
    }
    
}