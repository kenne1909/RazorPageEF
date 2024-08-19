using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Razorpage.models;

namespace App.Admin.User
{
    public class EditUserRoleClaimModel : PageModel
    {
        private readonly MyBlogContext _context;
        private readonly UserManager<AppUser> _userManager;
        public EditUserRoleClaimModel(MyBlogContext myBlogContext,
        UserManager<AppUser> userManager)
        {
            _context=myBlogContext;
            _userManager=userManager;
        }
        [TempData]
        public string? StatusMessage{set;get;}
        public NotFoundObjectResult  OnGet()
        {
            return NotFound("Không được truy cập");
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
        public AppUser? user{set;get;}

        public async Task<IActionResult> OnGetAddClaimAsync(string userid)
        {
            user = await _userManager.FindByIdAsync(userid);
            if(user ==null)
            {
                return NotFound("Không tìm thấy user");
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAddClaimAsync(string userid)
        {
            user = await _userManager.FindByIdAsync(userid);
            if(user ==null)
            {
                return NotFound("Không tìm thấy user");
            }
            if(!ModelState.IsValid)
            {
                return Page();
            }
            if (Input == null)
            {
                ModelState.AddModelError(string.Empty, "Dữ liệu đầu vào không hợp lệ.");
                return Page();
            }

            var claims= _context.UserClaims.Where(c =>c.UserId ==user.Id);

            if(claims.Any(c=>c.ClaimType==Input.ClaimType && c.ClaimValue==Input.ClaimValue))
            {
                ModelState.AddModelError(string.Empty,"Đặc tính này đã có");
                return Page();
            }
            await _userManager.AddClaimAsync(user   ,new Claim(Input.ClaimType ?? string.Empty,Input.ClaimValue ?? string.Empty));
            StatusMessage ="Đã thêm đặc tính cho user";

            return RedirectToPage("./AddRole",new{Id=user.Id});
        }
        public IdentityUserClaim<string>? userClaim{set;get;}

        public async Task<IActionResult> OnGetEditClaimAsync(int? claimid)
        {
            if(claimid ==null)
            {
                return NotFound("Không tìm thấy user");
            }

            userClaim =  _context.UserClaims.Where(c=> c.Id == claimid).FirstOrDefault();
            if (userClaim == null)
            {
                ModelState.AddModelError(string.Empty, "Thông tin user claim không hợp lệ.");
                return Page();
            }
            user = await _userManager.FindByIdAsync(userClaim.UserId);
            if(user ==null)
            {
                return NotFound("Không tìm thấy user");
            }
            Input =new InputModel{
                ClaimType=userClaim.ClaimType,
                ClaimValue=userClaim.ClaimValue
            };
            return Page();
        }

        public async Task<IActionResult> OnPostEditClaimAsync(int? claimid)
        {
            if(claimid ==null)
            {
                return NotFound("Không tìm thấy user");
            }

            userClaim =  _context.UserClaims.Where(c=> c.Id == claimid).FirstOrDefault();
            if (userClaim == null)
            {
                ModelState.AddModelError(string.Empty, "Thông tin user claim không hợp lệ.");
                return Page();
            }
            user = await _userManager.FindByIdAsync(userClaim.UserId);
            if(user ==null)
            {
                return NotFound("Không tìm thấy user");
            }
            if (Input == null)
            {
                ModelState.AddModelError(string.Empty, "Dữ liệu đầu vào không hợp lệ.");
                return Page();
            }

            if(!ModelState.IsValid) return Page();

            if(_context.UserClaims.Any(c => c.UserId ==user.Id && c.ClaimType== Input.ClaimType && c.ClaimValue ==Input.ClaimValue && c.Id !=userClaim.Id))
            {
                ModelState.AddModelError(string.Empty,"Claim này đã có");
                return Page();
            }

            userClaim.ClaimType=Input?.ClaimType;
            userClaim.ClaimValue=Input?.ClaimValue;

            await _context.SaveChangesAsync();

            StatusMessage="Bạn vừa cập nhật Claim";
            return RedirectToPage("./AddRole",new{Id=user.Id});
        }

        public async Task<IActionResult> OnPostDeleteAsync(int? claimid)
        {
            if(claimid ==null)
            {
                return NotFound("Không tìm thấy user");
            }

            userClaim =  _context.UserClaims.Where(c=> c.Id == claimid).FirstOrDefault();
            if (userClaim == null)
            {
                ModelState.AddModelError(string.Empty, "Thông tin user claim không hợp lệ.");
                return Page();
            }
            user = await _userManager.FindByIdAsync(userClaim.UserId);
            if(user ==null)
            {
                return NotFound("Không tìm thấy user");
            }
            await _userManager.RemoveClaimAsync(user,new Claim(userClaim.ClaimType ?? string.Empty,userClaim.ClaimValue ?? string.Empty));

            StatusMessage="Bạn đã xóa Claim";
            return RedirectToPage("./AddRole",new{Id=user.Id});
        }


    

    }
}
