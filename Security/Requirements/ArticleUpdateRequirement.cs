using Microsoft.AspNetCore.Authorization;

namespace App.Security.Requirements
{
    public class ArticleUpdateRequirement : IAuthorizationRequirement
    {
        // có thể đưa vào ngày tháng cập nhât dateCanUpdate
    }
}