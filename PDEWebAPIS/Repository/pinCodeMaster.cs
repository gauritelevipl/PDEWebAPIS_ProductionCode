using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PDEWebAPIS.Repository
{
    [Table("pinCodeMaster")]
    public class pinCodeMaster
    {
        [Key]
        public int pid { get; set; }
        [Required]
        public string? Name { get; set; }
        [Required]
        public string? Description { get; set; }
        [Required]
        public string? BranchType { get; set; }
        [Required]
        public string? DeliveryStatus { get; set; }
        [Required]
        public string? Circle { get; set; }
        [Required]
        public string? District { get; set; }
        [Required]
        public string? Division { get; set; }
        [Required]
        public string? Region { get; set; }
        [Required]
        public string? Block { get; set; }
        [Required]
        public string? State { get; set; }
        [Required]
        public string? Country { get; set; }
        [Required]
        public string? Pincode { get; set; }
        [Required]
        public string? createdDateTime { get; set; }
    }
}
