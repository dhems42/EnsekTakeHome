using System.Data.SQLite;
using System.Data;
using Microsoft.Extensions.DependencyInjection.Extensions;
using EnsekTakeHome.Repositories.Accounts;

var builder = WebApplication.CreateBuilder(args);
// Add services 
builder.Services.AddSingleton<IDbConnection>(provider =>
{
    // Create an in-memory SQLite connection
    var connection = new SQLiteConnection("Data Source=:memory:");
    connection.Open();

    // Create the table
    using (var cmd = new SQLiteCommand("CREATE TABLE MeterReadings (Id INTEGER PRIMARY KEY, AccountId INTEGER, MeterReadingDateTime DATE, MeterReadValue INTEGER);", connection))
    {
        cmd.ExecuteNonQuery();
    }

    return connection;
});

builder.Services.AddTransient<IAccountsRepository, AccountsRepository>();

builder.Services.AddControllers();
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

app.UseAuthorization();

app.MapControllers();

app.Run();
