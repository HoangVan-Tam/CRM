using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.EntityFrameworkCore;
using SMSDOME_Standard_Contest_BlazorServer.Data;
using Entities.Models;
using SMSDOME_Standard_Contest_BlazorServer.Mapper;
using System.Reflection;
using Services.Common;
using Microsoft.Data.SqlClient;
using DAL.Interface;
using DAL.Implement;
using Services.Interface;
using Services.Implement;
using Entities.Helper;

var builder = WebApplication.CreateBuilder(args);
var SMSDOMEAllowSpecificOrigins = "_SMSDOMEAllowSpecificOrigins";

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<WeatherForecastService>();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddDbContext<StandardContest2023Context>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("BaseDB"));
});
//builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped(typeof(ISQLRepository), typeof(SQLRepository));
builder.Services.AddScoped<SqlConnection>();
builder.Services.AddScoped(typeof(IUnitOfWork), typeof(UnitOfWork));
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
builder.Host.ConfigureContainer<ContainerBuilder>(builder =>
{
    //builder.RegisterAssemblyTypes(System.Reflection.Assembly.Load("DAL"))
    //                  .Where(t => t.Name.EndsWith("Repository"))
    //                  .AsImplementedInterfaces()
    //                  .InstancePerLifetimeScope();
    builder.RegisterAssemblyTypes(System.Reflection.Assembly.Load("Services"))
                     .Where(t => t.Name.EndsWith("Service"))
                     .AsImplementedInterfaces()
                     .InstancePerLifetimeScope();
});

builder.Services.Configure<AppConfig>(builder.Configuration.GetSection("AppConfig"));
//CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: SMSDOMEAllowSpecificOrigins,
        builder =>
        {
            builder.WithOrigins("https://localhost:7087/");
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseCors(SMSDOMEAllowSpecificOrigins);
app.UseRouting();

app.MapBlazorHub();
app.MapControllers();
app.MapFallbackToPage("/_Host");

app.Run();
