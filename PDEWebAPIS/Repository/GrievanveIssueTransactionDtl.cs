using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PDEWebAPIS.Repository
{
    [Table("grievanceIssueTransactionDtl")]
    public class GrievanveIssueTransactionDtl
    {
        [Key]
        [Required]
        public int transactionId { get; set; }
        [Required]
        public string? applicationId { get; set; }
        [Required]
        public string? tickitId { get; set; }
        [Required]
        public string? issueDescByUser { get; set; }
        [Required]
        public string? reply { get; set; }
        [Required]
        public string? replyUserid { get; set; }
        [Required]
        public DateTime datetime { get; set; }
    }
}
