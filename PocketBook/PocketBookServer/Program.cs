global using Grpc.Core;
global using GrpcShared;
using Microsoft.EntityFrameworkCore;
using PocketBookServer.Data;
using PocketBookServer.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(opt=>opt.UseSqlite("dataSource = PocketBook.db"));

builder.Services.AddGrpc();
builder.Services.AddSingleton<UsersManager>();
var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.MapGrpcService<UserService>();
app.MapGrpcService<AccountBookService>();

app.Run();
