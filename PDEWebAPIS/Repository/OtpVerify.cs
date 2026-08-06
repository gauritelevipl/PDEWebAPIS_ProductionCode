using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PDEWebAPIS.Repository
{
    [Table("otp_verify")]
    public class OtpVerify
    {
        [Required]
        public string mobileno { get; set; } = string.Empty;
        [Required, MaxLength(6)]
        public Int64 otp { get; set; }
        public DateTime createddatetime { set; get; }
    }
}
