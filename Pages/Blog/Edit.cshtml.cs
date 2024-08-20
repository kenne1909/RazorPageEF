using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Razorpage.models;

namespace CS048_RazorPage8_EF.Pages_Blog
{
    public class EditModel : PageModel
    {
        private readonly Razorpage.models.MyBlogContext _context;
        private readonly IAuthorizationService _authorizationService;

        public EditModel(Razorpage.models.MyBlogContext context,IAuthorizationService authorizationService)
        {
            _context = context;
            _authorizationService=authorizationService;
        }

        [BindProperty]
        public Article Article { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)// từ index truyền đến
        {
            if (id == null)
            {
                return Content("Không tìm thấy bài viết");
            }

            var article =  await _context.articles.FirstOrDefaultAsync(m => m.Id == id);
            if (article == null)
            {
                return Content("Không tìm thấy bài viết");
            }
            Article = article;
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(Article).State = EntityState.Modified;

            try
            {
                //kiem tra quyen cap nhat
                var canUpdate = await _authorizationService.AuthorizeAsync(this.User,Article,"CanUpdateArticle");
                if(canUpdate.Succeeded)
                {
                    await _context.SaveChangesAsync();
                }
                else
                {
                    return Content("Khong duoc quyen cap nhat");
                }
                
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ArticleExists(Article.Id))// từ form truyền đến
                {
                    return Content("Không tìm thấy bài viết");
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool ArticleExists(int id)
        {
            return _context.articles.Any(e => e.Id == id);
        }
    }
}
