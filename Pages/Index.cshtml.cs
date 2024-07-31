using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Razorpage.models;

namespace CS048_RazorPage8_EF.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly MyBlogContext myBlogContext;

    public IndexModel(ILogger<IndexModel> logger,MyBlogContext _myContext)
    {
        _logger = logger;
        myBlogContext =_myContext;
    }

    public void OnGet()
    {
        var posts = (from a in myBlogContext.articles
                    orderby a.Create descending
                    select a).ToList();

        ViewData["posts"]=posts;
    }
}
