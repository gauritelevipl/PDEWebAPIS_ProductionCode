using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
namespace PDEWebAPIS.Repository
{
    [Table("application_data_submitted_history")]
    public class ApplicationDataSubmittedHistory
    {
        [Key, Required]
        public int application_data_submitted_history_id { get; set; }
        public string? applicationid { get; set; }
        public string? application_submitted_form_name { get; set; }
        public string? mutation_type_code { set; get; }
        public string? mutation_type_name { set; get; }
        public string? application_submitted_type { get; set; }
        public DateTime createddatetime { set; get; }
    }
}
