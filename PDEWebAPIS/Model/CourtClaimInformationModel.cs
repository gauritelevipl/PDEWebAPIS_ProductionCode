using PDEWebAPIS.Repository;

namespace PDEWebAPIS.Model
{
    public class CourtClaimInformationModel
    {
        public int court_claim_id { get; set; }
        public UserMaster? userMaster { get; set; }
        public ApplicationDTL? applicationDTL { get; set; }
        public string? court_case_code { set; get; }
        public string? court_case_name { set; get; }
        public string? court_case_type_code { set; get; }
        public string? court_case_type_name { set; get; }
        public string? lr_property_uid { set; get; }
        public string? city_servey_no { set; get; }
        public string? order_details { set; get; }
        public string? stay_order { set; get; }
        public string? sub_property_no { set; get; }
    }
}
