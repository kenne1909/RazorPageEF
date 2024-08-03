using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Razorpage.models
{
    //[Table("posts")]
    public class Article
    {
        [Key]
        public int Id{set;get;}

        [StringLength(255, MinimumLength =5 ,ErrorMessage = "{0} phải dài từ {2} tới {1} kí tự")]
        [Required (ErrorMessage ="{0} phải nhập")]
        [Column(TypeName ="nvarchar")]
        [DisplayName("Tiêu đề")]
        public string? Title{set;get;}

        [DataType(DataType.Date)]
        [Required (ErrorMessage ="{0} không được trống")]
        [DisplayName("Ngày tạo")]
        public DateTime Create{set;get;}

        [Column(TypeName ="ntext")]
        [DisplayName("Nội dung")]
        public string? Content{set;get;}
    }
}