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
        public List<RoleModel>? roles{set;get;}

        public class RoleModel : IdentityRole
        {
            public string[]? Claims{get; set;}
        }
        public async Task OnGet()
        {
            // _roleManager.GetClaimsAsync()
            var r = await _roleManager.Roles.OrderBy(r => r.Name).ToListAsync();
            roles = new List<RoleModel>();
            foreach (var _r in r)
            {
                var claims =await _roleManager.GetClaimsAsync(_r);
                var claimsString= claims.Select(c=>c.Type + "="+c.Value);
                var rm = new RoleModel()
                {
                    Name =_r.Name,
                    Id = _r.Id,
                    Claims =claimsString.ToArray()
                };  
                roles.Add(rm);
            }
        }
        public void OnPost()
        {
            RedirectToPage();
        }
    }
}
