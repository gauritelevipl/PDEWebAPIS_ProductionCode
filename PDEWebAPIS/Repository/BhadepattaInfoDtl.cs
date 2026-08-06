using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PDEWebAPIS.Repository
{
    [Table("bhadepattaInfoDtl")]
    public class BhadepattaInfoDtl
    {
        [Key, Required]
        public int Info_id { get; set; }
        public int userid { get; set; }
        //public string? village_code { get; set; }
        //public string? ctsNo { get; set; }
        //public string? mutationSroNo { get; set; }
        //public string? ownerNo { get; set; }
        public string? applicationid { get; set; }
        public string? bhadepattaTenureYear { get; set; }
        public string? bhadepattaTenureMonth { get; set; }
        public string? bhadepattaFromDate { get; set; }
        public string? bhadepattaToDate { get; set; }
        public string? bhadepattaAmount { get; set; }
        public DateTime createdDateTime { get; set; } = DateTime.UtcNow;
        public DateTime deletedDateTime { get; set; } = new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        public bool? isDeleted { get; set; }
        public bool? leaseperiod { get; set; }
    }
}
