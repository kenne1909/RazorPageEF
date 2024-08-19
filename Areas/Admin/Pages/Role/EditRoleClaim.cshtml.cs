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
    public class EditRoleClaimModel : RolePageModel         //PageModel
    {
        public EditRoleClaimModel(RoleManager<IdentityRole> roleManager, MyBlogContext myBlogContext) : base(roleManager, myBlogContext)
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
        public IdentityRoleClaim<string>? claim{set;get;}

        public async Task<IActionResult> OnGet(int? claimid)
        {
            if (Input == null)
            {
                Input = new InputModel();
            }
            if(claimid == null) return NotFound("Không tìm thấy role");
            claim= _context.RoleClaims.Where(c=> c.Id==claimid).FirstOrDefault();
            if(claim == null) return NotFound("Không tìm thấy role");

            role = await _roleManager.FindByIdAsync(claim.RoleId);
            if(role == null) return NotFound("không tìm thấy role");

            Input = new InputModel()
            {
                ClaimType =  claim.ClaimType,
                ClaimValue = claim.ClaimValue
            };  
            return Page();
        }
        public async Task<IActionResult> OnPostAsync(int? claimid)
        {
            if (Input == null)
            {
                ModelState.AddModelError(string.Empty, "Dữ liệu đầu vào không hợp lệ.");
                return Page();
            }
            if(claimid == null) return NotFound("Không tìm thấy role");
            claim= _context.RoleClaims.Where(c=> c.Id==claimid).FirstOrDefault();
            if(claim == null) return NotFound("Không tìm thấy role");

            role = await _roleManager.FindByIdAsync(claim.RoleId);
            if(role == null) return NotFound("không tìm thấy role");

            if (!ModelState.IsValid)//nếu dữ liệu k phù hợp vs validation
            {
                return Page();
            }
            if (_context.RoleClaims.Any(c =>c.RoleId==role.Id && c.ClaimType == Input.ClaimType && c.ClaimValue==Input.ClaimValue && c.Id != claim.Id))
            {
                ModelState.AddModelError(string.Empty,"Claim này đã có trong role");
                return Page();
            }

            claim.ClaimType=Input?.ClaimType;
            claim.ClaimValue=Input?.ClaimValue;

            await _context.SaveChangesAsync();

            StatusMessage ="Vừa cập nhật đặt tính(Claim)";

            return RedirectToPage("./Edit", new {roleid= role.Id}); // Trả về trang hiện tại
        }

        public async Task<IActionResult> OnPostDeleteAsync(int? claimid)
        {
            if(claimid == null) return NotFound("Không tìm thấy role");
            claim= _context.RoleClaims.Where(c=> c.Id==claimid).FirstOrDefault();
            if(claim == null) return NotFound("Không tìm thấy role");

            role = await _roleManager.FindByIdAsync(claim.RoleId);
            if(role == null) return NotFound("không tìm thấy role");


            await _roleManager.RemoveClaimAsync(role, new Claim(claim.ClaimType ?? string.Empty, claim.ClaimValue ?? string.Empty));

            StatusMessage ="Vừa xóa đặt tính(Claim)";

            return RedirectToPage("./Edit", new {roleid= role.Id}); // Trả về trang hiện tại
        } 
    }
}
