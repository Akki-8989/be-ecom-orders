var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseCors();

var orders = new List<Order>
{
    new(1, "user-1", new[] { "Laptop", "Headphones" }, 1149.98m, "Delivered", DateTime.Parse("2026-02-01")),
    new(2, "user-2", new[] { "Running Shoes" }, 89.99m, "Shipped", DateTime.Parse("2026-02-05")),
    new(3, "user-1", new[] { "Backpack", "Water Bottle" }, 69.98m, "Processing", DateTime.Parse("2026-02-10"))
};

app.MapGet("/api/orders", () => Results.Ok(orders))
   .WithName("GetOrders")
   .WithOpenApi();

app.MapGet("/api/orders/{id}", (int id) =>
{
    var order = orders.FirstOrDefault(o => o.Id == id);
    return order is not null ? Results.Ok(order) : Results.NotFound();
})
.WithName("GetOrderById")
.WithOpenApi();

app.MapGet("/api/orders/user/{userId}", (string userId) =>
{
    var userOrders = orders.Where(o => o.UserId == userId).ToList();
    return Results.Ok(userOrders);
})
.WithName("GetOrdersByUser")
.WithOpenApi();

app.MapGet("/api/orders/health", () => Results.Ok(new { service = "be-ecom-orders", status = "healthy" }));

app.Run();

record Order(int Id, string UserId, string[] Items, decimal Total, string Status, DateTime OrderDate);
