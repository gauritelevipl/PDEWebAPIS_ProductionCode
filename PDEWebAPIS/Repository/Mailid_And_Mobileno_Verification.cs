using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PDEWebAPIS.Repository
{
    [Table("mailid_and_mobileno_verification")]
    public class Mailid_And_Mobileno_Verification
    {
        [Required]
        [RegularExpression(@"MOBILENO|EMAILID")]
        public string verificationtype { get; set; } = string.Empty;
        [Required]
        public string description { get; set; } = string.Empty;
        [Required,MaxLength(6)]
        public Int64 otp { get; set; }
    }
}
