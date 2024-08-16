using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Razorpage.models;

namespace App.Admin.User
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;
        public IndexModel(UserManager<AppUser> userManager)
        {
            _userManager=userManager;
        }

        [TempData]
        public string? StatusMessage{set;get;}

        public int totalUsers{set;get;}

        public class UserAndRole : AppUser
        {
            public string? RoleName{set;get;}
        }

        public List<UserAndRole>? users{set;get;}

        public async Task OnGet()
        {
            var qr =     _userManager.Users.OrderBy(u => u.UserName).Select(u =>new UserAndRole() {
                Id=u.Id,
                UserName = u.UserName
            });
            totalUsers= await qr.CountAsync();
            users = await qr.ToListAsync();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                user.RoleName = string.Join(",",roles);
            }
        }
        public void OnPost()
        {
            RedirectToPage();
        }
    }
}
