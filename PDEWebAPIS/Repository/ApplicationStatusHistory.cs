using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PDEWebAPIS.Repository
{
    [Table("application_status_history")]
    public class ApplicationStatusHistory
    {
        [Key, Required]
        public int application_status_history_id { get; set; }
        public string? applicationid { get; set; }
        public string? application_status { get; set; }
        public string? mutation_type_code { set; get; }
        public string? mutation_type_name { set; get; }
        public DateTime createddatetime { set; get; }
    }
}
