using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Notemywork.Api.Data;
using Notemywork.Api.Entities;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("Develop"));
});

var securityScheme = new OpenApiSecurityScheme()
{
    Name = "Authorization",
    Type = SecuritySchemeType.ApiKey,
    Scheme = "Bearer",
    BearerFormat = "JWT",
    In = ParameterLocation.Header,
    Description = "JWT Authentication for Minimal API"
};

var securityRequirements = new OpenApiSecurityRequirement()
{
    {
        new OpenApiSecurityScheme
        {
            Reference = new OpenApiReference
            {
                Type = ReferenceType.SecurityScheme,
                Id = "Bearer"
            }
        },
        new string[] {}
    }
};

var contactInfo = new OpenApiContact()
{
    Name = "Suttiporn Srisawad",
    Email = "suttiporn.s2540@gmail.com",
    Url = new Uri("https://github.com/yee-suttiporn")
};

var license = new OpenApiLicense()
{
    Name = "Free License",
    Url = new Uri("https://github.com/yee-suttiporn")
};

var info = new OpenApiInfo()
{
    Version = "v1",
    Title = "Note My Work API",
    //Description = "My Note API",
    TermsOfService = new Uri("https://github.com/yee-suttiporn"),
    Contact = contactInfo,
    License = license
};


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", info);
    options.AddSecurityDefinition("Bearer", securityScheme);
    options.AddSecurityRequirement(securityRequirements);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

#region User
var userEndPoint = app.MapGroup("/users").WithTags("Users");

userEndPoint.MapPost("/add", async (User user, AppDbContext db) =>
{
    if (user is null) return Results.BadRequest();

    var isExist = await db.Users.FirstOrDefaultAsync(x => x.Username == user.Username);
    if (isExist is not null)
    {
        return Results.BadRequest(new ResponseMessage(false, $"ชื่อผู้ใช้งาน {isExist.Username} ถูกใช้ไปแล้ว"));
    }

    user.UserId = 0;
    db.Users.Add(user);
    if (db.SaveChanges() == 0) return Results.Ok(new ResponseMessage(false, "เกิดข้อผิดพลาดไม่ได้เพิ่มผู้ใช้ใหม่ได้"));

    return Results.Ok(new ResponseMessage(true, "เพิ่มผู้ใช้งานใหม่เรียบร้อยแล้ว"));
});

userEndPoint.MapPost("/update", async (User user, AppDbContext db) =>
{
    if (user is null) return Results.BadRequest();

    var isExist = await db.Users.FirstOrDefaultAsync(x => x.UserId == user.UserId);
    if (isExist is null)
        return Results.NotFound(new ResponseMessage(false, "ไม่พบผู้ใช้งาน"));

    isExist.Username = user.Username;
    isExist.Password = user.Password;
    isExist.Email = user.Email;

    db.Users.Update(isExist);
    if (db.SaveChanges() == 0)
        return Results.BadRequest(new ResponseMessage(false, "แก้ไขผู้ใช้งานไม่สำเร็จ"));

    return Results.Ok(new ResponseMessage(true, "แก้ไขผู้ใช้งานเรียบร้อยแล้ว"));
});

#endregion User


app.Run();

internal record ResponseMessage(bool success, string message);

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
