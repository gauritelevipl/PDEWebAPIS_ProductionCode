using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PDEWebAPIS.Repository
{
    [Table("grievanceStatusMaster")]
    public class GrievanceStatusMaster
    {
        [Required,Key]
        public int statusId { get; set; }   

        [Required]
        public string? status { get; set; }
        [Required]
        public int statusCode { get; set; }
    }
}
