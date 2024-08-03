using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Razorpage.models
{
    public class AppUser :IdentityUser
    {
        [Column(TypeName ="nvarchar")]
        [StringLength(400)]
        public string? HomeAdress{set;get;}
    }
}