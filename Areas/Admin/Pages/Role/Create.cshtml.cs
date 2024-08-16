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
    public class CreateModel : RolePageModel         //PageModel
    {
        public CreateModel(RoleManager<IdentityRole> roleManager, MyBlogContext myBlogContext) : base(roleManager, myBlogContext)
        {
        }

        public class InputModel
        {
            [DisplayName("Tên của role")]
            [Required(ErrorMessage ="Phai nhap {0}")]
            [StringLength(256, MinimumLength = 3, ErrorMessage ="{0} phải dài từ {2} đến {1} kí tự")]
            public string? Name{set;get;}
        }

        [BindProperty]
        public InputModel? Input{set;get;}

        public void OnGet()
        {
            if (Input == null)
            {
                Input = new InputModel();
            }
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)//nếu dữ liệu k phù hợp vs validation
            {
                return Page();
            }

            if (string.IsNullOrEmpty(Input?.Name)) // Kiểm tra xem Input.Name có null hoặc rỗng không
            {
                ModelState.AddModelError(string.Empty, "Tên role không tồn tại");
                return Page();
            }

            var newRole = new IdentityRole(Input.Name); // Tạo role mới từ tên trong Input
            var result  = await _roleManager.CreateAsync(newRole); // Tạo role mới trong hệ thống
            if(result.Succeeded)
            {
                StatusMessage =$"Ban vừa tạo role mới : {Input.Name}";
                return RedirectToPage("./Index");
            }
            else
            {
                result.Errors.ToList().ForEach(error => {
                    ModelState.AddModelError(string.Empty,error.Description);
                });
            }

            return Page(); // Trả về trang hiện tại
        }
    }
}
