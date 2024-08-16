using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Razorpage.models;

namespace App.Admin.Role
{
    [Authorize(Roles ="Admin")]
    public class DeleteModel : RolePageModel         //PageModel
    {
        public DeleteModel(RoleManager<IdentityRole> roleManager, MyBlogContext myBlogContext) : base(roleManager, myBlogContext)
        {
        }


        public IdentityRole? role{set;get;}

        public async Task<IActionResult> OnGet(string roleid)
        {
            if(roleid == null)
            {
                return NotFound("Không tìm thấy role");
            }
            else
            {
                role = await _roleManager.FindByIdAsync(roleid) ?? null;
                if(role==null)
                {
                    return NotFound("Không tìm thấy role");
                }
                return Page();
            }
            
        }
        public async Task<IActionResult> OnPostAsync(string roleid)
        {
            if(roleid == null)
            {
                return NotFound("Không tìm thấy role");
            }
            else
            {
                role = await _roleManager.FindByIdAsync(roleid) ?? null;
                if(role == null)
                {
                    return NotFound("Không tìm thấy role");
                }
                else
                {
                    var result = await _roleManager.DeleteAsync(role);

                    if(result.Succeeded)
                    {
                        StatusMessage =$"Ban vừa xóa role: {role.Name}";
                        return RedirectToPage("./Index");
                    }
                    else
                    {
                        result.Errors.ToList().ForEach(error => {
                            ModelState.AddModelError(string.Empty,error.Description);
                        });
                    }

                }
            }
            return Page(); // Trả về trang hiện tại
        }
    }
}
