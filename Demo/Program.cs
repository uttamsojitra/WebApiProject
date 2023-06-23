using OfficeOpenXml;
using System.Text;
using Demo.Business.Exception;
using Demo.Business.Interface;
using Demo.Business.Interface.Interface_Service;
using Demo.Business.Repository;
using Demo.Business.Service;
using Demo.Entities.Data;
using Demo.Repository.Interface;
using Demo.Repository.Repository;   
using Demo.Repository.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Demo.Entities.Model.ViewModel;

ExcelPackage.LicenseContext = LicenseContext.NonCommercial;// Set the license context to NonCommercial


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//builder.Services.AddControllers();

builder.Services.AddControllers(options =>
{
    options.Filters.Add<GlobalExceptionFilter>();
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//Authenticate button 
builder.Services.AddSwaggerGen(
    c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "My API",
            Version = "v1"
        });
        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Description = "Please insert JWT with Bearer into field",
            Name = "Authorization",
            Type = SecuritySchemeType.ApiKey
        });
        c.AddSecurityRequirement(new OpenApiSecurityRequirement {
   {
     new OpenApiSecurityScheme
     {
       Reference = new OpenApiReference
       {
         Type = ReferenceType.SecurityScheme,
         Id = "Bearer"
       }
      },
      new string[] { }
    }
  });
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options => {
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AddUpdatePolicy", policy =>
       policy.RequireRole("admin","user")
             .RequireAssertion(context =>
                       context.User.HasClaim(claim =>
                         (claim.Type == "permission" &&
                          claim.Value == "CanCreate" || claim.Value == "CanUpdate"
                         )
                       )
                    ));

    options.AddPolicy("DeletePolicy", policy => 
        policy.RequireRole("admin")
              .RequireAssertion(context =>
                       context.User.HasClaim(claim =>
                         (claim.Type == "permission" &&
                          (claim.Value == "CanDelete")
                         )
                       )
                    ));
});



builder.Services.AddDbContext<UserDbcontext>((options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))));

builder.Services.AddDbContext<CiPlatformContext>((options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("CiPlatformConnection"))));

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();

builder.Services.AddScoped<IEmailSender, EmailSender>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IAuthHelperService, AuthHelperService>();


// Configure SMTP settings
var smtpSettings = builder.Configuration.GetSection("SmtpSettings").Get<SmtpSettings>();
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));
builder.Services.AddSingleton(smtpSettings);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())    
{
   app.UseSwagger();
   app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Demo v1"));
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();

app.Run();
