using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PDEWebAPIS.Repository
{
    [Table("dast_information")]
    public class DastInformation
    {
        [Key, Required]
        public int dast_id { get; set; }
        public UserMaster? userMaster { get; set; }
        public ApplicationDTL? applicationDTL { get; set; }
        public string? dastType { get; set; }
        public int? division_code { get; set; }
        public string? division_name { get; set; }
        public string? districtCode { set; get; }
        public string? districtName { set; get; }
        //public string? districtNameInEnglish { set; get; }
        public string? office_of_the_second_registrar_code { set; get; }
        public string? office_of_the_second_registrar_name { set; get; }
        public string? registered_dast_no { set; get; }
        public string? registered_dast_date { set; get; }
        public string? registered_dast_year { set; get; }
        public string? dastNabhu {  get; set; }
        public string? remarks { set; get; }
        public bool? isDastVerified { get; set; }
        public string? verifiedDastData { set; get; }
        public DateTime createddatetime { set; get; }
        public bool isDeleted { set; get; }
        public DateOnly deleteddate { set; get; }
    }
}
