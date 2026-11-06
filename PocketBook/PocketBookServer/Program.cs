global using Grpc.Core;
global using GrpcShared;
using Microsoft.EntityFrameworkCore;
using PocketBookClient.Helper;
using PocketBookServer.Data;
using PocketBookServer.Services;

var builder = WebApplication.CreateBuilder(args);

ApplySecretsFromConfiguration(builder.Configuration);

builder.Services.AddDbContext<AppDbContext>(opt=>opt.UseSqlite("dataSource = PocketBook.db"));

builder.Services.AddGrpc();

builder.Services.AddCors(o => o.AddPolicy("AllowAll", builder =>
{
    builder.AllowAnyOrigin()
           .AllowAnyMethod()
           .AllowAnyHeader()
           .WithExposedHeaders("Grpc-Status", "Grpc-Message", "Grpc-Encoding", "Grpc-Accept-Encoding");
}));
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

app.UseRouting();

app.UseGrpcWeb();
app.UseCors();

app.MapGrpcService<AccountBookService>().EnableGrpcWeb().RequireCors("AllowAll");

app.Run();

static void ApplySecretsFromConfiguration(IConfiguration configuration)
{
    string[] keys =
    {
        SecretConfiguration.SmtpHostKey,
        SecretConfiguration.SmtpPortKey,
        SecretConfiguration.SmtpUserKey,
        SecretConfiguration.SmtpPasswordKey,
        SecretConfiguration.SmtpFromNameKey,
        SecretConfiguration.RsaPublicKeyKey,
        SecretConfiguration.RsaPrivateKeyKey,
        SecretConfiguration.GoogleClientIdKey,
        SecretConfiguration.GoogleClientSecretKey,
        SecretConfiguration.GoogleSpreadsheetIdKey,
        SecretConfiguration.GoogleCredentialsPathKey,
        SecretConfiguration.GoogleCredentialsJsonKey
    };

    foreach (var key in keys)
    {
        SecretConfiguration.ApplyConfigurationValue(key, configuration[key]);
    }
}
