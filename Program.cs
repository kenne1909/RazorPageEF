using System.Configuration;
using System.Security.Claims;
using App.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Razorpage.models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddOptions();
var mailSetting  = builder.Configuration.GetSection("MailSettings");
builder.Services.Configure<MailSettings>(mailSetting);
builder.Services.AddSingleton<IEmailSender, SendMailService>();


builder.Services.AddDbContext<MyBlogContext>(options =>{
    // string? connectString=builder.Configuration.GetConnectionString("MyBlogContext");
    // options.UseSqlServer(connectString);

    options.UseSqlServer(builder.Configuration.GetConnectionString("MyBlogContext"));
});
// đăng ký Identity
builder.Services.AddIdentity<AppUser, IdentityRole>()
                .AddEntityFrameworkStores<MyBlogContext>()
                .AddDefaultTokenProviders();

// builder.Services.AddDefaultIdentity<AppUser>()
//                 .AddEntityFrameworkStores<MyBlogContext>()
//                 .AddDefaultTokenProviders();

// Truy cập IdentityOptions
builder.Services.Configure<IdentityOptions> (options => {
    // Thiết lập về Password
    options.Password.RequireDigit = false; // Không bắt phải có số
    options.Password.RequireLowercase = false; // Không bắt phải có chữ thường
    options.Password.RequireNonAlphanumeric = false; // Không bắt ký tự đặc biệt
    options.Password.RequireUppercase = false; // Không bắt buộc chữ in
    options.Password.RequiredLength = 3; // Số ký tự tối thiểu của password
    options.Password.RequiredUniqueChars = 1; // Số ký tự riêng biệt

    // Cấu hình Lockout - khóa user
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes (5); // Khóa 5 phút
    options.Lockout.MaxFailedAccessAttempts = 3; // Thất bại 3 lầ thì khóa
    options.Lockout.AllowedForNewUsers = true;

    // Cấu hình về User.
    options.User.AllowedUserNameCharacters = // các ký tự đặt tên user
        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = true;  // Email là duy nhất

    // Cấu hình đăng nhập.
    options.SignIn.RequireConfirmedEmail = true;            // Cấu hình xác thực địa chỉ email (email phải tồn tại)
    options.SignIn.RequireConfirmedPhoneNumber = false;     // Xác thực số điện thoại
    options.SignIn.RequireConfirmedAccount = true;         // Người dùng phải xác nhận tài khoản
});

builder.Services.ConfigureApplicationCookie(options =>{
    options.LoginPath = "/login/";
    options.LogoutPath= "/logout/";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied"; //đường dẫn tới trang khi user bị cấm truy cập
});

builder.Services.AddAuthentication()
                .AddGoogle(options =>{
                    IConfigurationSection? gconfi = builder.Configuration.GetSection("Authentication:Google");

                    string clientId = gconfi["ClientId"] ?? throw new InvalidOperationException("Google ClientId chưa được định cấu hình.");
                    string clientSecret = gconfi["ClientSecret"] ?? throw new InvalidOperationException("Google ClientSecret chưa được định cấu hình.");
                    
                    options.ClientId =clientId;
                    options.ClientSecret = clientSecret;
                    // https://localhost:7134/signin-google nếu k thiết lập CallbackPath 
                    options.CallbackPath = "/dang-nhap-tu-google";
                })
                .AddFacebook(options => {
                    IConfigurationSection? gconfi = builder.Configuration.GetSection("Authentication:Facebook");
                    string appId = gconfi["AppId"] ?? throw new InvalidOperationException("Facebook AppId chưa được định cấu hình.");
                    string appSecret = gconfi["AppSecret"] ?? throw new InvalidOperationException("Facebook AppSecret chưa được định cấu hình.");

                    options.AppId =appId;
                    options.AppSecret = appSecret;
                    // https://localhost:7134/signin-google nếu k thiết lập CallbackPath 
                    options.CallbackPath = "/dang-nhap-tu-facebook";
                })
                // .AddFacebook()
                // .AddTwitter()
                // .AddMicrosoftAccount()
                ;

builder.Services.AddSingleton<IdentityErrorDescriber,AppIdentityErrorDescriber>();

builder.Services.AddAuthorization(options => {
    // options.AddPolicy("Tenpolicy", policyBuilder =>{
    //     //Dieu kien cua policy
    // });
    options.AddPolicy("AllowEditRole", policyBuilder =>{
        //Dieu kien cua policy
        policyBuilder.RequireAuthenticatedUser();//userphari dang nhapj
        // policyBuilder.RequireRole("Admin");
        // policyBuilder.RequireRole("Editor");

        policyBuilder.RequireClaim("canedit","user","post");
        
        //Claims-based authorization
        // policyBuilder.RequireClaim("ten Claim","giatri1","giatri2");
        // policyBuilder.RequireClaim("ten Claim",new string[]
        // {
        //     "giatri1",
        //     "giatri2"
        // });

        // IdentityRoleClaim<string> claim1;
        // IdentityUserClaim<string> claim2;
        // Claim claim;
    });
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

app.UseAuthentication();
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

/*
Thư viện Identity: 
    -Authentication: xác định danh tính -> Liên quan đến login, logout, ...
    -Authortization: xác thực quyền truy cập
        Role-based authorization -> hổ trợ xác thực quyền theo vai trò
        Role(vai trò): admin,editor,manager,member,...
        tạo trang:  /Areas/Admin/Pages/Role
            Index: quản lí danh sách hiển thị các role
            create,edit,delete
        dotnet new page --name Index --output Areas/Admin/Pages/Role --namespace App.Admin.Role



    -Quản lý user: Sign up, User, Role, ...


    dotnet add package Microsoft.AspNetCore.Identity
    dotnet add package Microsoft.AspNetCore.Identity.EntityFrameworkCore
    dotnet add package Microsoft.AspNetCore.Identity.UI


Nếu muốn xài đăng nhập thì add những package sau:
    dotnet add package Microsoft.AspNetCore.Authentication.Facebook
    dotnet add package Microsoft.AspNetCore.Authentication.Google
    dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
    dotnet add package Microsoft.AspNetCore.Authentication.MicrosoftAccount
    dotnet add package Microsoft.AspNetCore.Authentication.oAuth
    dotnet add package Microsoft.AspNetCore.Authentication.OpenIDConnect
    dotnet add package Microsoft.AspNetCore.Authentication.Twitter



/Identity/Account/Login
/Identity/Account/Manage

phát sinh code cho identity
dotnet aspnet-codegenerator identity -dc Razorpage.models.MyBlogContext
*/


/*
Model:
-   Có thể mở rộng các lớp như IdentityUser, IdentityRole, IdentityUserRole<TKey> , ...

-   Mở rộng lớp IdentityDbContext để sử dụng lớp tùy chỉnh EX: AppRole,AppUser,AppUserRole,...
Cấu hình DbContext để sử dụng các lớp tùy chỉnh và chỉ định cấu trúc bảng trong OnModelCreating.
public class tencontext : IdentityDbContext<AppUser,AppRole, Tkey, IdentityUserClaim<Tkey>, AppUserRole ,
IdentityUserLogin<Tkey> IdentityRoleClaim<Tkey>, IdentityUserToken<Tkey>>

-    Đăng ký Identity
EX: builder.Services.AddIdentity<Tên mở rộng của IdentityUser, Tên mở rộng của IdentityRole()>()
                    .AddEntityFrameworkStores<tencontext>()
                    .AddDefaultTokenProviders();


-   Muốn xác thực quyền truy cập:   [Authorize] -Controller,Action,PageModel(k thiết lập halder) -> user phải đăng nhập
-   Policy-based authorization(q truy cập theo chính sách)
-   Claims-based authorization(q truy cập theo đặc tính, tính chất của user)
    -> Claims: đặt tính, tính chất của 1 đối tượng(User)

    Bằng lái B2(Role) ->đc lái xe 4 chổ
    -ngày sinh  -> claim
    -nơi sinh   -> claim

    Mua rươu(trên 18t): đưa bằng lái để xác định(kiểm tra tính chất của ng mua rượu)
    - Kiểm tra ngày sinh: Claims-based authorization



CallbackPath: google sẽ  gửi mã token đến địa chỉ này
https://localhost:7134/dang-nhap-tu-google
*/
