using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Razorpage.models;

namespace App.Admin.Role
{
    [Authorize(Roles ="Admin")]
    public class AddRoleClaimModel : RolePageModel         //PageModel
    {
        public AddRoleClaimModel(RoleManager<IdentityRole> roleManager, MyBlogContext myBlogContext) : base(roleManager, myBlogContext)
        {
        }

        public class InputModel
        {
            [DisplayName("Kiểu(Tên) claim")]
            [Required(ErrorMessage ="Phai nhap {0}")]
            [StringLength(256, MinimumLength = 3, ErrorMessage ="{0} phải dài từ {2} đến {1} kí tự")]
            public string? ClaimType{set;get;}

            [DisplayName("Giá trị claim")]
            [Required(ErrorMessage ="Phai nhap {0}")]
            [StringLength(256, MinimumLength = 3, ErrorMessage ="{0} phải dài từ {2} đến {1} kí tự")]
            public string? ClaimValue{set;get;}
        }

        [BindProperty]
        public InputModel? Input{set;get;}
        public IdentityRole? role{set;get;}

        public async Task<IActionResult> OnGet(string roleid)
        {
            if (Input == null)
            {
                Input = new InputModel();
            }
            role = await _roleManager.FindByIdAsync(roleid);
            if(role == null) return NotFound("không tìm thấy role");
            return Page();
        }
        public async Task<IActionResult> OnPostAsync(string roleid)
        {
            role = await _roleManager.FindByIdAsync(roleid);
            if(role == null) return NotFound("không tìm thấy role");
            if (!ModelState.IsValid)//nếu dữ liệu k phù hợp vs validation
            {
                return Page();
            }
            if((await _roleManager.GetClaimsAsync(role)).Any(c => c.Type == Input?.ClaimType && c.Value==Input?.ClaimValue))
            {
                ModelState.AddModelError(string.Empty,"Claim này đã có trong role");
                return Page();
            }

            var newClaim = new Claim(Input?.ClaimType ?? string.Empty, Input?.ClaimValue ?? string.Empty);

            var result = await _roleManager.AddClaimAsync(role,newClaim);

            if(!result.Succeeded)
            {
                result.Errors.ToList().ForEach(e => {
                    ModelState.AddModelError(string.Empty,e.Description);
                });
                return Page();
            }

            StatusMessage ="Vừa thêm đặt tính(Claim) mới";

            return RedirectToPage("./Edit", new {roleid= role.Id}); // Trả về trang hiện tại
        }
    }
}
