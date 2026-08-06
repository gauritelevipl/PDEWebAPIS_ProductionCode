using System.ComponentModel.DataAnnotations;

namespace PDEWebAPIS.Model
{
    public class Mailid_And_Mobileno_VerificationModel
    {
        public string verificationtype { get; set; } = string.Empty;
        public string description { get; set; } = string.Empty;
        public Int64 otp { get; set; }
        public string guid { get; set; } = string.Empty;
    }
}
