using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PDEWebAPIS.Repository
{
    [Table("labelmaster")]
    public class LabelMaster
    {
        [Key, Required]
        public int labelid { get; set; }
        public ScreenMaster? screenMaster { get; set; }
        public MutationTypeMaster? mutationTypeMaster { get; set; }
        //public string? mutationtype { set; get; }
        //public string? mutationname { set; get; }
        public string? englishname { set; get; }
        public string? marathiname { set; get; }
        public string? createdby { set; get; }
        [Required]
        public DateTime createddatetime { set; get; }
        [Required]
        public DateTime updateddatetime { set; get; }
    }
}
