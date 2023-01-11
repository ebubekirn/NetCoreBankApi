using Microsoft.EntityFrameworkCore;
using SocialNetworkBE.DAL;
using SocialNetworkBE.Models;
using SocialNetworkBE.Services;
using SocialNetworkBE.Services.Implementations;
using SocialNetworkBE.Services.Interfaces;
using SocialNetworkBE.Utils;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<YouBankingDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("YouBankingDbConnection")));
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<Response>();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));
//builder.Services.AddSwaggerGen(x =>
//{
//    x.SwaggerDoc("v2", new Microsoft.OpenApi.Models.OpenApiInfo
//    {
//        Title = "My Youtube Banking API doc",
//        Version = "v2",
//        Description = "We were crazy enough to build a Bank API",
//        Contact = new Microsoft.OpenApi.Models.OpenApiContact
//        {
//            Name = "Ebubekir N.",
//            Email = "en@info.com",
//            Url = new Uri("https://github.com")
//        }
//    });
//});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//void Configure(IApplicationBuilder app, IWebHostEnvironment env)
//{
//    app.UseEndpoints(endpoints =>
//    {
//        endpoints.MapControllers();
//    });
//}

app.UseHttpsRedirection();

app.UseAuthorization();

//app.UseSwagger();
//app.UseSwaggerUI(x =>
//{
//    var prefix = string.IsNullOrEmpty(x.RoutePrefix) ? "." : "..";
//    x.SwaggerEndpoint($"{prefix}/swagger/v2/swagger.json", "My Youtube Banking API doc");

    
//});


app.MapControllers();

app.Run();
