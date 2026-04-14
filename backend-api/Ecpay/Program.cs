
using ConnectStringModel;
using Ecpay;
using Ecpay.Models;
using Ecpay.Services;
using Microsoft.Extensions.Options;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();


//單一值
var conStr = builder.Configuration.GetValue<string>("ConnectionStrings:Ecpay");
builder.Services.AddSingleton(conStr);

//拿到資料轉成物件
var ecpaySetting = builder.Configuration.GetSection("EcpaySetting").Get<EcpaySettingModel>();
builder.Services.AddSingleton(ecpaySetting);


//生命週期 AddSingleton(全程就這一個)、AddScoped(每一次請求建立一個物件)、AddTransient(每次用都new)
builder.Services.AddScoped<OrderService>();
builder.Services.AddScoped<CreditCardService>();


builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy
            .WithOrigins("http://localhost:5230", "http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

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
app.UseCors("Frontend");

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
