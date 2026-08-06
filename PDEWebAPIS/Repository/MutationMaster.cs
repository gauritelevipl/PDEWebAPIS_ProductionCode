using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PDEWebAPIS.Repository
{
    [Table("mutationmaster")]
    public class MutationMaster
    {
        [Key, Required]
        public string? mutation_code { get; set; }
        public string? mutation_name { get; set; }
        public ApplicationTypeMaster? applicationType { get; set; }
        public DateTime createddatetime { set; get; }
    }
}
