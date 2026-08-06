using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PDEWebAPIS.Repository
{
    [Table("grievanceReasonMaster")]
    public class GrievanceReasonMaster
    {
        [Key, Required]
        public int rid { get; set; }

        [Required]
        public string? reasonName { get; set; }
        [Required]
        public int reasonCode { get; set; }
    }
}
