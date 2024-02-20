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

userEndPoint.MapPost("/add", async (User obj, AppDbContext db) =>
{
    if (obj is null) return Results.BadRequest(new ResponseMessage(false, "Request json ไม่ถูกต้อง"));

    var user = await db.Users.FirstOrDefaultAsync(x => x.Username == obj.Username);
    if (user is not null)
    {
        return Results.BadRequest(new ResponseMessage(false, $"ชื่อผู้ใช้งาน {user.Username} ถูกใช้ไปแล้ว"));
    }

    obj.UserId = 0;
    db.Users.Add(obj);
    if (db.SaveChanges() == 0) return Results.Ok(new ResponseMessage(false, $"เกิดข้อผิดพลาดเพิ่มผู้ใช้งาน {obj.Username} ไม่สำเร็จ"));

    return Results.Ok(new ResponseMessage(true, $"เพิ่มผู้ใช้งาน {obj.Username} เรียบร้อยแล้ว"));
});

userEndPoint.MapPut("/update", async (User obj, AppDbContext db) =>
{
    if (obj is null) return Results.BadRequest(new ResponseMessage(false, "Request json ไม่ถูกต้อง"));

    var user = await db.Users.FirstOrDefaultAsync(x => x.UserId == obj.UserId);
    if (user is null)
        return Results.NotFound(new ResponseMessage(false, $"ไม่พบผู้ใช้งาน {obj.Username} ในระบบ"));

    user.Username = obj.Username;
    user.Password = obj.Password;
    user.Email = obj.Email;

    db.Users.Update(user);
    if (db.SaveChanges() == 0)
        return Results.BadRequest(new ResponseMessage(false, $"แก้ไขผู้ใช้งาน {obj.Username} ไม่สำเร็จ"));

    return Results.Ok(new ResponseMessage(true, $"แก้ไขผู้ใช้งาน {obj.Username} เรียบร้อยแล้ว"));
});

userEndPoint.MapDelete("/delete", async (int id, AppDbContext db) =>
{
    var user = await db.Users.FirstOrDefaultAsync(x => x.UserId == id);
    if (user is null)
        return Results.NotFound(new ResponseMessage(false, "ไม่พบผู้ใช้งานที่ต้องการลบ"));

    db.Users.Remove(user);
    if (db.SaveChanges() == 0)
        return Results.NotFound(new ResponseMessage(false, $"เกิดข้อผิดพลาดไม่ได้สามารถลบผู้ใช้งาน {user.Username} ได้"));

    return Results.NotFound(new ResponseMessage(true, $"ลบผู้ใช้งาน {user.Username} เรียบร้อยแล้ว"));
});

userEndPoint.MapGet("/getAll", async (AppDbContext db) => await db.Users.ToListAsync());

userEndPoint.MapGet("/getById", async (int id, AppDbContext db) => await db.Users.FirstOrDefaultAsync(x => x.UserId == id));

#endregion User


#region Category
var categoryEndPoint = app.MapGroup("/category").WithTags("Category");

categoryEndPoint.MapPost("/add", async (Category obj, AppDbContext db) =>
{
    if (obj is null) return Results.BadRequest(new ResponseMessage(false, "Request json ไม่ถูกต้อง"));

    var cat = await db.Categories.FirstOrDefaultAsync(x => x.CategoryName == obj.CategoryName && x.UserId == obj.UserId);
    if (cat is not null) return Results.BadRequest(new ResponseMessage(false, $"ไม่สามารถเพิ่มหมวด {obj.CategoryName} ซ้ำกับที่มีอยู่แล้วได้"));

    obj.CategoryId = 0;
    await db.Categories.AddAsync(obj);
    if (db.SaveChanges() == 0)
        return Results.BadRequest(new ResponseMessage(false, $"เกิดข้อผิดพลาดเพิ่มหมวด {obj.CategoryName} ไม่สำเร็จ"));

    return Results.BadRequest(new ResponseMessage(true, $"เพิ่มหมวด {obj.CategoryName} เรียบร้อยแล้ว"));
});


#endregion Category


app.Run();

internal record ResponseMessage(bool success, string message);