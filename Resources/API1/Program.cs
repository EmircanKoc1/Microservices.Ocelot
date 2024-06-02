using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });


builder.Services.AddAuthorization(config =>
{
    config.AddPolicy("Authenticated", policy =>
    {
        policy.RequireAuthenticatedUser();

    });
    config.AddPolicy("AdminPolicy", policy =>
    {
        policy.RequireClaim("Role","Admin");
    });
});



var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () => "api1")
    .RequireAuthorization("Authenticated");

app.MapGet("/admin", () => "admin")
    .RequireAuthorization("AdminPolicy");




app.Run();