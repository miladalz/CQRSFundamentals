using MediatR;
using Microsoft.EntityFrameworkCore;
using Ordering.Domain.AggregatesModel.BuyerAggregate;
using Ordering.Domain.AggregatesModel.OrderAggregate;
using Ordering.Infrastructure;
using Ordering.Infrastructure.Repositories;
using System.Reflection;
using FluentValidation.AspNetCore;
using Ordering.API.Infrastructure;
using Ordering.API.Application.Models;
using Ordering.API.Application.Behaviors;

var builder = WebApplication.CreateBuilder(args);

//Add services to the container.
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly()));

//ef core db context
builder.Services.AddDbContext<OrderingContext>(options =>
{
    options.UseSqlServer(builder.Configuration["ConnectionString"]);
});

//Repository injection
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IBuyerRepository, BuyerRepository>();


builder.Services.AddTransient<OrderingContextSeed>();

//connectionString for dapper
var connectionString = new ConnectionString(builder.Configuration["ConnectionString"]);
builder.Services.AddSingleton(connectionString);

//Behaviors
builder.Services.AddScoped(typeof(IPipelineBehavior<,>), typeof(TransactionBehavior<,>));

builder.Services.AddControllers()
    .AddFluentValidation(cfg => cfg.RegisterValidatorsFromAssembly(typeof(Program).Assembly));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//Data Seed
using (var scope = app.Services.CreateScope())
{
    scope.ServiceProvider.GetService<OrderingContextSeed>().SeedAsync().Wait();
}

app.UseAuthorization();
app.MapControllers();
app.Run();