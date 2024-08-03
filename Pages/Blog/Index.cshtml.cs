using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Razorpage.models;

namespace CS048_RazorPage8_EF.Pages_Blog
{
    public class IndexModel : PageModel
    {
        private readonly Razorpage.models.MyBlogContext _context;

        public IndexModel(Razorpage.models.MyBlogContext context)
        {
            _context = context;
        }

        public IList<Article> Article { get;set; } = default!;//giúp tránh cảnh báo nullability mà không cần khởi tạo thuộc tính ngay lập tức.

        public async Task OnGetAsync(string SearchString) 
        {
            // Article = await _context.articles.ToListAsync();

            var qr =from a in _context.articles
                    orderby a.Create descending
                    select a;
            
            if(!string.IsNullOrEmpty(SearchString))
            {
                Article = qr.Where(a => a.Title!=null &&  a.Title.Contains(SearchString) ).ToList();
                // vì sao k dùng await
            }
            else
            {
                Article =  await qr.ToListAsync();
            }
            //Article =  await qr.ToListAsync();  
        }
    }
}
