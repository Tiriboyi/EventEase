using Microsoft.EntityFrameworkCore;
using EventEase.Data;
using Azure.Storage.Blobs;
using EventEase.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddMemoryCache();

// Configure DbContext
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<EventEaseContext>(options =>
    options.UseSqlServer(connectionString, providerOptions => providerOptions.EnableRetryOnFailure())
           .EnableSensitiveDataLogging()
           .EnableDetailedErrors());

// Register EventSearchService
builder.Services.AddScoped<EventSearchService>();

// Configure Azure Blob Storage
var blobConnectionString = builder.Configuration.GetSection("AzureBlobStorage:ConnectionString").Value;
var blobContainerName = builder.Configuration.GetSection("AzureBlobStorage:ContainerName").Value;
builder.Services.AddSingleton(x => new BlobServiceClient(blobConnectionString));
builder.Services.AddSingleton(x => new BlobContainerClient(blobConnectionString, blobContainerName));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();