using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Razorpage.models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddDbContext<MyBlogContext>(options =>{
    // string? connectString=builder.Configuration.GetConnectionString("MyBlogContext");
    // options.UseSqlServer(connectString);

    options.UseSqlServer(builder.Configuration.GetConnectionString("MyBlogContext"));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();


/*
CREATE,READ,UPDATE,DELETE (CRUD)

dotnet aspnet-codegenerator razorpage -m Razorpage.models.Article -dc Razorpage.models.MyBlogContext ourDir Pages/Blog -udl --referenceScriptLibraries

dotnet tool install -g dotnet-aspnet-codegenerator dùng lệnh này trc nếu lệnh trên chạy k đc

dotnet add package Microsoft.VisualStudio.Web.CodeGeneration.Design
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.VisualStudio.Web.CodeGenerators.Mvc

*/
