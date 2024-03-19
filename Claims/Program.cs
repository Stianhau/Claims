using System.Reflection;
using System.Text.Json.Serialization;
using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers().AddJsonOptions(x =>
    {
        x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    }
);

builder.Services.AddDbContext<AuditContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddSingleton<CosmosClient>(serviceProvider =>
{
    string? account = builder.Configuration.GetSection("CosmosDb:Account").Value;
    string? key = builder.Configuration.GetSection("CosmosDb:Key").Value;
    return new CosmosClient(account, key);
});

builder.Services.AddScoped<IAuditService, AuditService>();

builder.Services.AddSingleton<IClaimRepository>(sp =>
{
    CosmosClient client = sp.GetRequiredService<CosmosClient>();
    Container container = InitialiseCosmosDbContainerAsync(client, "ClaimDb", "Claim").GetAwaiter().GetResult();
    return new ClaimRepository(container);
});

builder.Services.AddSingleton<ICoverRepository>(sp =>
{
    CosmosClient client = sp.GetRequiredService<CosmosClient>();
    Container container = InitialiseCosmosDbContainerAsync(client, "ClaimDb", "Cover").GetAwaiter().GetResult();
    return new CoverRepository(container);
});

builder.Services.AddScoped<IClaimsService, ClaimsService>();
builder.Services.AddScoped<ICoverService, CoverService>();


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

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

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AuditContext>();
    context.Database.Migrate();
}

app.Run();

static async Task<Container> InitialiseCosmosDbContainerAsync(CosmosClient client, string databaseName, string containerName)
{
    DatabaseResponse database = await client.CreateDatabaseIfNotExistsAsync(databaseName);
    await database.Database.CreateContainerIfNotExistsAsync(containerName, "/id");
    return client.GetContainer(databaseName, containerName);
}

public partial class Program { }