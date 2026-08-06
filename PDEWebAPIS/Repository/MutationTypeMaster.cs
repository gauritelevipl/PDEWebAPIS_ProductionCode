using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PDEWebAPIS.Repository
{
    [Table("mutationtypemaster")]
    public class MutationTypeMaster
    {
        [Key, Required]
        public int mutationid { set; get; }
        public string? mutationtype { set; get; }
    }
}
