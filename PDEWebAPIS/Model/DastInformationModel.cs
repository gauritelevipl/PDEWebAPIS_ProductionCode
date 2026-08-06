using PDEWebAPIS.Repository;

namespace PDEWebAPIS.Model
{
    public class DastInformationModel
    {
        public int dast_id { get; set; }
        public UserMaster? userMaster { get; set; }
        public ApplicationDTL? applicationDTL { get; set; }
        public string? dastType { get; set; }
        public int?  digcode { get; set; }
        public string? dig_name{ get; set; }
        public string? districtCode { set; get; }
        //public string? districtNameInMarathi { set; get; }
        public string? districtName { set; get; }
        public string? office_of_the_second_registrar_code { set; get; }
        public string? office_of_the_second_registrar_name { set; get; }
        public string? registered_dast_no { set; get; }
        public string? registered_dast_date { set; get; }
        public string? registered_dast_year { set; get; }
        public string? dastNabhu { get; set; }
        public bool? isDastVerified { get; set; }
        public string? verifiedDastData { set; get; }
        public string? remarks { set; get; }
    }
}
