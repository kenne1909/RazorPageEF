using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Razorpage.models;

namespace App.Admin.Role
{
    //[Authorize(Roles ="vaitro1,vaitro2,vaitro3")] // user có 1 trong 3 cái vai trò sẽ đc truy cập
    [Authorize(Roles ="Admin")]

    // [Authorize(Roles ="vaitro1")]
    // [Authorize(Roles ="vaitro1")]
    // [Authorize(Roles ="vaitro1")] user phải thỏa mãn cả 3 vai trò ms có quyền truy cập

    public class IndexModel : RolePageModel          //PageModel
    {
        public IndexModel(RoleManager<IdentityRole> roleManager, MyBlogContext myBlogContext) : base(roleManager, myBlogContext)
        {
        }

        // private readonly RoleManager<IdentityRole> _roleManager;
        // public IndexModel(RoleManager<IdentityRole> roleManager)
        // {
        //     _roleManager=roleManager;
        // }
        public List<IdentityRole>? roles{set;get;}
        public async Task OnGet()
        {
            roles = await _roleManager.Roles.OrderBy(r => r.Name).ToListAsync();
        }
        public void OnPost()
        {
            RedirectToPage();
        }
    }
}
