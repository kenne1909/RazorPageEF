// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Razorpage.models;

namespace App.Admin.User
{
    public class AddRoleModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly MyBlogContext _context;

        public AddRoleModel(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            MyBlogContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _context=context;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        [DisplayName("Các role gán cho user")]
        public string[] RoleNames{set;get;}

        public AppUser user{set;get;}
        public SelectList allRoles{set;get;}
        public List<IdentityRoleClaim<string>> claimsInRole{set;get;}
        public List<IdentityUserClaim<string>> claimsInUser{set;get;}
        
        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound($"không có User.");
            }
            user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound($"không thấy user với id = '{id}'.");
            }

            RoleNames = (await _userManager.GetRolesAsync(user)).ToArray<string>();

            List<string> rolaname= await _roleManager.Roles.Select(r =>r.Name).ToListAsync();
            allRoles = new SelectList(rolaname);

            await GetClaims(id);


            return Page();
        }
        async Task GetClaims(string id)
        {
            var listRole =  from r in _context.Roles
                            join ur in _context.UserRoles
                            on r.Id equals ur.RoleId
                            where ur.UserId == id
                            select r; // trả về role của user
            var _claimsInRole = from c in _context.RoleClaims
                                join r in listRole on c.RoleId equals r.Id
                                select c;
            claimsInRole = await _claimsInRole.ToListAsync();

            claimsInUser = await(from c in _context.UserClaims
                                where c.UserId == id
                                select c).ToListAsync();
        }

        public async Task<IActionResult> OnPostAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound($"không có User.");
            }

            user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound($"không thấy user với id = '{id}'.");
            }

            //Rolename
            await GetClaims(id);

            var OldRoleNames= (await _userManager.GetRolesAsync(user)).ToArray();
            var deleteRole =OldRoleNames.Where(r=>!RoleNames.Contains(r));
            var addRole =RoleNames.Where(r =>!OldRoleNames.Contains(r));

            List<string> rolaname= await _roleManager.Roles.Select(r =>r.Name).ToListAsync();
            allRoles = new SelectList(rolaname);

            var resultDelete = await _userManager.RemoveFromRolesAsync(user,deleteRole);
            if(!resultDelete.Succeeded)
            {
                resultDelete.Errors.ToList().ForEach(error => {
                    ModelState.AddModelError(String.Empty,error.Description);
                });
                return Page();
            }

            var resultAdd = await _userManager.AddToRolesAsync(user,addRole);
            if(!resultAdd.Succeeded)
            {
                resultAdd.Errors.ToList().ForEach(error => {
                    ModelState.AddModelError(String.Empty,error.Description);
                });
                return Page();
            }

    
            StatusMessage = $"Vừa cập nhật role cho user {user.UserName}.";

            return RedirectToPage("./Index");
        }
    }
}
