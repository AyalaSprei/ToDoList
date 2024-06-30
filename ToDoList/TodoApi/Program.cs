using Microsoft.EntityFrameworkCore;
using TodoApi;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddDbContext<ToDoDbContext>(options =>
    options.UseMySql("name=ToDoDB", Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.35-mysql")));

builder.Services.AddScoped<ToDoDbContext>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("myAppCors", policy =>
    {
        policy.WithOrigins("http://localhost:3000")
                .AllowAnyHeader()
                .AllowAnyMethod();
    });
});
var app = builder.Build();
app.UseCors("myAppCors");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/items", async (ToDoDbContext dbContext) =>
{
    var items = await dbContext.Items.ToListAsync();
    return items;
});

app.MapPost("/items", async (Item newItem, ToDoDbContext dbContext) =>
{
    dbContext.Items.Add(newItem);
    await dbContext.SaveChangesAsync();
    return newItem;
});

app.MapPut("/items/{id}", async (HttpContext context, ToDoDbContext dbContext) =>
{
    var id = int.Parse(context.Request.RouteValues["id"].ToString());

    var existingItem = await dbContext.Items.FirstOrDefaultAsync(item => item.Id == id);

    if (existingItem != null)
    {
        var updatedItem = await context.Request.ReadFromJsonAsync<Item>();

        if (updatedItem != null)
        {
            existingItem.Name = updatedItem.Name ?? existingItem.Name;
            existingItem.IsComplete = updatedItem.IsComplete ?? existingItem.IsComplete;

            await dbContext.SaveChangesAsync();

            return Results.Ok(existingItem);
        }
        else
        {
            return Results.BadRequest("Invalid or missing JSON data in the request body");
        }
    }
    else
    {
        return Results.NotFound($"Item with ID {id} not found");
    }
});

app.MapDelete("/items/{id}", async (int id, ToDoDbContext dbContext) =>
{
    var itemToRemove = await dbContext.Items.FirstOrDefaultAsync(item => item.Id == id);
    if (itemToRemove != null)
    {
        dbContext.Items.Remove(itemToRemove);
        await dbContext.SaveChangesAsync();
        return Results.Ok();
    }
    else
    {
        return Results.NotFound($"Item with ID {id} not found");
    }
});
app.Run();
