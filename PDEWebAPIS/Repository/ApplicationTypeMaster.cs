using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PDEWebAPIS.Repository
{
    [Table("applicationtypemaster")]
    public class ApplicationTypeMaster
    {
        [Key, Required]
        public int applicationtypeid { get; set; }
        public string? application_type_name_in_eng { set; get; }
        public string? application_type_name_in_marathi { set; get; }
    }
}
