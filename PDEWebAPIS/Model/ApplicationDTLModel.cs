using PDEWebAPIS.Repository;

namespace PDEWebAPIS.Model
{
    public class ApplicationDTLModel
    {
        public string? applicationid { get; set; }
        public UserMaster? userMaster { get; set; }
        public ApplicationTypeMaster? applicationTypeMaster { get; set; }
        public string? applicantIDs { set; get; }
        public string? mutation_type_code { set; get; }
        public string? mutation_type_name { set; get; }
        public string? district_code { set; get; }
        public string? district_name_in_marathi { set; get; }
        public string? district_name_in_english { set; get; }
        public string? office_code { set; get; }
        public string? office_name { set; get; }
        public bool do_you_have_power_of_attorney { set; get; }
        public bool Is_the_claim_pending_before_the_court { set; get; }
        public bool does_the_original_charter_info_apply { set; get; }
        public string? mutation_cts_nos { set; get; }
        public string? dastIDs { set; get; }
        public string? courtClaimIDs { set; get; }
        public string? powerOfAttorneyIDs { set; get; }
        public string? uploadedDocIDs { set; get; }
        public string? mutationgiverIDs { get; set; }
        public string? mutationtakerIDs { get; set; }
        public string? self_declaration_doc_name { set; get; }
        public string? self_declaration_doc_path { set; get; }
    }
}
