using Enterprise_Web.Middleware;
using Enterprise_Web.Pagination.Service;
using Enterprise_Web.Repository;
using Enterprise_Web.Repository.IRepository;
using Enterprise_Web.Services;
using EnterpriseWeb.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Serialization;
using System.Net;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Add services to the container.
builder.Services.AddControllersWithViews()
    .AddNewtonsoftJson(options =>
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft
                .Json.ReferenceLoopHandling.Ignore);
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "AllowOrigin",
        builder =>
        {
            builder.WithOrigins("http://localhost:3000", "https://septuple-project.vercel.app")
                                .AllowAnyHeader()
                                .AllowAnyMethod();
        });
});

builder.Services.AddControllers();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(o =>
{
    o.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey
        (Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = false,
        ValidateIssuerSigningKey = true
    };
});

builder.Services.AddAuthorization();

builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<IUriService>(o =>
{
    var accessor = o.GetRequiredService<IHttpContextAccessor>();
    var request = accessor.HttpContext.Request;
    var uri = string.Concat(request.Scheme, "://", request.Host.ToUriComponent());
    return new UriService(uri);
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "My API",
        Version = "v1"
    });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 1safsfsdfdfd\"",
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
        {
            new OpenApiSecurityScheme {
                Reference = new OpenApiReference {
                    Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});
builder.Services.AddDbContext<ApplicationDbContext>(x => x.UseSqlServer(connectionString));
builder.Services.AddTransient<IUserRepository, UserRepository>();
builder.Services.AddTransient<IDepartmentRepository, DepartmentRepository>();
builder.Services.AddTransient<IAcademicYearRepository, AcademicYearRepository>();
builder.Services.AddTransient<ICategoryRepository, CategoryRepository>();
builder.Services.AddTransient<IIdeaRepository, IdeaRepository>();
builder.Services.AddTransient<ICommentRepository, CommentRepository>();
builder.Services.AddTransient<INotificationRepository, NotificationRepository>();
builder.Services.AddTransient<IEmailSender, EmailSender>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ErrorHandlerMiddleware>();

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseCors("AllowOrigin");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
