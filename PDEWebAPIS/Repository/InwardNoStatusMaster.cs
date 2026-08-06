using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PDEWebAPIS.Repository
{
    [Table("inward_no_status_master")]
    public class InwardNoStatusMaster
    {
        [Key, Required]
        public int inward_no_status_id { get; set; }
        public string? applicationid { get; set; }
        public string? mutation_type_code { set; get; }
        public string? mutation_type_name { set; get; }
        public string? inwardno { get; set; }
        public int srno { get; set; }
        public string? status { get; set; }
        public DateOnly status_date { get; set; }
        [Required]
        public DateTime createddatetime { set; get; }
    }
}
