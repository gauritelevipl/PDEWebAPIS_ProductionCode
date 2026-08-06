using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PDEWebAPIS.Repository
{
    [Table("document_type_master")]
    public class DocumentTypeMaster
    {
        [Key, Required]
        public int document_type_id { get; set; }
        public string? document_type_name { get; set; }
        public string? mutation_type_code { get; set; }
        public string? mutation_description { get; set; }
        public DateTime createddatetime { get; set; }
    }
}
