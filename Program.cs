using Etape1.Data;
using Etape1.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
 
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "MyPolicy",
        policy =>
        {
            policy.AllowAnyOrigin()
                    .WithMethods("PUT", "DELETE", "GET","POST");
        });
});
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Description = "Standard Authorization header using the Bearer scheme (\"bearer {token}\")",
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });

    options.OperationFilter<SecurityRequirementsOperationFilter>();
});

var _authkey=builder.Configuration.GetValue<string>("JwtSettings:securityKey");

builder.Services.AddAuthentication(item=>
    {
    item.DefaultAuthenticateScheme=JwtBearerDefaults.AuthenticationScheme;
    item.DefaultChallengeScheme=JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(item=>
    {
        item.RequireHttpsMetadata=true;
        item.SaveToken=true;
        item.TokenValidationParameters=new TokenValidationParameters(){
            ValidateIssuerSigningKey=true,
            IssuerSigningKey= new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authkey)),
            ValidateIssuer = false,
            ValidateAudience = false,
            ClockSkew=TimeSpan.Zero,
        };
});
//for connection
string connectionString = builder.Configuration.GetConnectionString("WebApiDatabase");
builder.Services.AddDbContext<DataDbContext>(options => 
    options.UseMySql(connectionString,ServerVersion.AutoDetect(connectionString))
);

var _jwtsettings=builder.Configuration.GetSection("JwtSettings");
builder.Services.Configure<JwtSettings>(_jwtsettings);
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

