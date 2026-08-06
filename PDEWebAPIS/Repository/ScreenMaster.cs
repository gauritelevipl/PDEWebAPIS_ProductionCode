using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PDEWebAPIS.Repository
{
    [Table("screenmaster")]
    public class ScreenMaster
    {
        [Key, Required]
        public int screenid { get; set; }
        public string? screenname { set; get; }
    }
}
