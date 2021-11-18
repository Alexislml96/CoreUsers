using System.Text;
using CORE.Users.Configuration;
using CORE.Users.Interfaces;
using CORE.Users.Models;
using CORE.Users.Services;
using CORE.Users.Tools;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "WebAPI", Version = "v1" });
});

if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Production")
{
    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                 .AddJwtBearer(options =>
                 {
                     options.TokenValidationParameters = new TokenValidationParameters
                     {
                         ValidateIssuer = true,
                         ValidateAudience = true,
                         ValidateLifetime = true,
                         ValidateIssuerSigningKey = true,
                         ValidIssuer = Environment.GetEnvironmentVariable("ISSUER_TOKEN"),
                         ValidAudience = Environment.GetEnvironmentVariable("AUDIENCE_TOKEN"),
                         IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("SECRET_KEY"))),
                         ClockSkew = TimeSpan.Zero,
                     };
                 });
}

else
{
    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                  .AddJwtBearer(options =>
                  {
                      options.TokenValidationParameters = new TokenValidationParameters
                      {
                          ValidateIssuer = true,
                          ValidateAudience = true,
                          ValidateLifetime = true,
                          ValidateIssuerSigningKey = true,
                          ValidIssuer = builder.Configuration["JWT:ISSUER_TOKEN"],
                          ValidAudience = builder.Configuration["JWT:AUDIENCE_TOKEN"],
                          IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:SECRET_KEY"])),
                          ClockSkew = TimeSpan.Zero,
                      };
                  });
}

builder.Services.AddTransient((ServiceProvider) => BridgeDBConnection<UserModel>.Create(builder.Configuration.GetConnectionString("LocalServer"), Alexis.CORE.Connection.Models.DbEnum.Sql));
builder.Services.AddTransient((ServiceProvider) => BridgeDBConnection<LoginModel>.Create(builder.Configuration.GetConnectionString("CloudServer"), Alexis.CORE.Connection.Models.DbEnum.Sql));



builder.Services.AddScoped<IUser, UserService>();
builder.Services.AddScoped<ILogin, LoginService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebAPI v1");
        //c.RoutePrefix = String.Empty;
    });
    
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
