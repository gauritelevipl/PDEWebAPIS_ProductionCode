using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PDEWebAPIS.Repository
{
    [Table("nic_api_response")]
    public class NICAPIResponse
    {
        [Key, Required]
        public int response_id { get; set; }
        public string? applicationid {  get; set; }
        public string? inwardno { get; set; }
        public string? statuscode { get; set; }
        public string? response { get; set; }
        public int apicallcount {  get; set; }
        [Required]
        public DateTime createddatetime { set; get; }
    }
}
