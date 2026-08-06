using System.ComponentModel.DataAnnotations;

namespace PDEWebAPIS.InputDataModel
{
    public class VerifyMobileOTP
    {
        [Required(ErrorMessage = "Mobile No Is Required.")]
        public string? mobileno { get; set; }
        [Required(ErrorMessage = "OTP Is Required.")]
        public Int64 otp { get; set; }
    }
}
