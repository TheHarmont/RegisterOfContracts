using Microsoft.EntityFrameworkCore;
using RegisterOfContracts.Domain.Abstract;
using RegisterOfContracts.Domain.DAL;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();



var connectionString = builder.Configuration.GetConnectionString("RegisterOfContracts");
builder.Services.AddDbContext<DataBaseContext>(option => { option.UseSqlServer(connectionString); });
builder.Services.AddScoped<IContractRepository, EFContractRepository>();

var app = builder.Build();
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseCookiePolicy();

app.UseAuthorization();

app.UseStaticFiles();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Contract}/{action=Index}/{id?}");

app.Run();
