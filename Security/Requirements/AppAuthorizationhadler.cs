using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Razorpage.models;

namespace App.Security.Requirements
{
    public class AppAuthorizationhadler : IAuthorizationHandler
    {
        private readonly ILogger<AppAuthorizationhadler> _logger;
        private readonly UserManager<AppUser> _userManager;

        public AppAuthorizationhadler(ILogger<AppAuthorizationhadler> logger,UserManager<AppUser> userManager)
        {
            _logger=logger;
            _userManager=userManager;
        }

        public Task HandleAsync(AuthorizationHandlerContext context)
        {
            //context.PendingRequirements//những requi chưa kiểm tra
            //context.User
            //context.Resource//mặc định là HTTPCONTEXT;truyền tới 1bài post, 1 sản phẩm,... 
            var requirements=context.PendingRequirements.ToList();
            _logger.LogInformation("context.Resource ~ " +context.Resource?.GetType().Name);
            foreach (var requirement in requirements)
            {
                if(requirement is GenZRequirement)
                {
                    if (IsGenZ(context.User,(GenZRequirement)requirement))
                    {
                        context.Succeed(requirement);
                    }
                    //code xử lý kiểm tra User đảm bảo requirement l;à đối tượng GenZRequirement
                    //context.Succeed(requirement);
                }
                // if(requirement is OtherRequirement)
                // {
                //     //code xử lý kiểm tra User đảm bảo requirement l;à đối tượng GenZRequirement
                //     //context.Succeed(requirement);
                // }
                if(requirement is ArticleUpdateRequirement)
                {
                    bool  canUpdate = CanUpdateArticle(context.User,context.Resource,(ArticleUpdateRequirement)requirement);
                    if(canUpdate)
                        context.Succeed(requirement);
                }
            }

            return Task.CompletedTask;
        }

        private bool CanUpdateArticle(ClaimsPrincipal user, object? resource, ArticleUpdateRequirement requirement)
        {
            if (user.IsInRole("Admin"))
            {
                _logger.LogInformation("Admin cap nhat ...");
                return true;
            }
            var article= resource as Article;
            var dateCreate =  article?.Create;
            var dateCanUpdate = new DateTime(2023,6,30);
            if(dateCreate < dateCanUpdate)
            {
                _logger.LogInformation("Qua ngay cap nhat");
                return false;
            }
            return true;
        }

        private bool IsGenZ(ClaimsPrincipal user, GenZRequirement requirement)
        {
            var appUserTask =  _userManager.GetUserAsync(user);
            Task.WaitAll(appUserTask);
            var appUser= appUserTask.Result;

            if(appUser?.BirthDate == null) 
            {
                _logger.LogInformation($"{appUser?.UserName} khong co ngay sinh");
                return false;
            }
            

            int year = appUser.BirthDate.Value.Year;

            var success= (year >= requirement.FromYear && year <=requirement.ToYear);

            if(success)
            {
                _logger.LogInformation($"{appUser?.UserName} thỏa man GenZRequirement");
            }
            else
            {
                _logger.LogInformation($"{appUser?.UserName} khong thoa man GenZRequirement");
            }

            return success;
        }
    }
}