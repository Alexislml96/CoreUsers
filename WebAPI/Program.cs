using CORE.Users.Configuration;
using CORE.Users.Interfaces;
using CORE.Users.Models;
using CORE.Users.Services;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "WebAPI", Version = "v1" });
});

builder.Services.AddTransient((ServiceProvider) => BridgeDBConnection<UserModel>.Create(builder.Configuration.GetConnectionString("LocalServer"), Alexis.CORE.Connection.Models.DbEnum.Sql));
builder.Services.AddTransient((ServiceProvider) => BridgeDBConnection<LoginModel>.Create(builder.Configuration.GetConnectionString("CloudServer"), Alexis.CORE.Connection.Models.DbEnum.Sql));



builder.Services.AddScoped<IUser, UserService>();
builder.Services.AddScoped<ILogin, LoginService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebAPI v1"));
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
