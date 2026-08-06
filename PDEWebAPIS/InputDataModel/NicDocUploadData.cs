using System.ComponentModel.DataAnnotations;
namespace PDEWebAPIS.InputDataModel
{
    public class NicDocUploadData
    {
        public int userid { get; set; }
        [Required(ErrorMessage = "ApplicationID Is Required.")]
        public string? applicationid { get; set; }
        [Required(ErrorMessage = "Inward Number Is Required.")]
        public string? inwardno { get; set; }
        [Required(ErrorMessage = "Document type Code Is Required.")]
        public string? document_type_code { set; get; }
        [Required(ErrorMessage = "Document Type Is Required.")]
        public string? document_type { set; get; }
        //[Required(ErrorMessage = "City Servey Number Is Required.")]
        public string? city_servey_no { get; set; }
        [Required(ErrorMessage = "Document Name Is Required.")]
        public string? document_name { set; get; }
        [Required(ErrorMessage = "Document Path Is Required.")]
        public string? document_path { set; get; }
        [Required(ErrorMessage = "Nic Date Is Required.")]
        public DateOnly nicdate { set; get; }
    }
}
