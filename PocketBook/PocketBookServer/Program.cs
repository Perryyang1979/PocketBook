global using Grpc.Core;
global using GrpcShared;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PocketBookServer.Data;
using PocketBookServer.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(opt=>opt.UseSqlite("dataSource = PocketBook.db"));

builder.Services.AddGrpc();
var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    // Resolve the DbContext
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    // Ensure the database is created
    dbContext.Database.EnsureCreated();
    dbContext.SaveChanges();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.MapGrpcService<AccountBookService>();

app.Run();
