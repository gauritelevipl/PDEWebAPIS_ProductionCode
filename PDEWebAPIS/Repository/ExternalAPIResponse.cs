using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PDEWebAPIS.Repository
{
    [Table("external_api_response")]
    public class ExternalAPIResponse
    {
        [Key, Required]
        public int ex_response_id { get; set; }
        public string? applicationid { get; set; }
        public string? vendorname { get; set; }
        public string? api { get; set; }
        public string? statuscode { get; set; }
        public string? response { get; set; }
        [Required]
        public DateTime createddatetime { set; get; }
    }
}
