using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PDEWebAPIS.Repository
{
    [Table("applicationdtl")]
    public class ApplicationDTL
    {
        [Key, Required]
        public string? applicationid { get; set; }
        public UserMaster? userMaster { get; set; }
        public ApplicationTypeMaster? applicationTypeMaster { get; set; }
        public string? mutation_type_code { set; get; }
        public string? mutation_type_name { set; get; }
        public string? district_code { set; get; }
        public string? district_name_in_marathi { set; get; }
        public string? district_name_in_english { set; get; }
        public string? office_code { set; get; }
        public string? office_name { set; get; }
        [Required]
        public bool do_you_have_power_of_attorney { set; get; }
        [Required]
        public bool Is_the_claim_pending_before_the_court { set; get; }
        [Required]
        public bool does_the_original_charter_info_apply  { set; get; }
        public string? applicantIDs { set; get; }
        public string? mutation_cts_nos { set; get; }
        public string? dastIDs { set; get; }
        public string? courtClaimIDs { set; get; }
        public string? powerOfAttorneyIDs { set; get; }
        public string? uploadedDocIDs { set; get; }
        public string? mutationgiverIDs { get; set; }
        public string? mutationtakerIDs { get; set; }
        public string? mayatIDs { set; get; }
        public string? varasIDS {  set; get; }
        public int status { set; get; }
        // Self Declaration dco
        public string? self_declaration_doc_name { set; get; }
        public string? self_declaration_doc_path { set; get; }
        public string? inwardno { set; get; }
        public bool? is_sentnic { set; get; } = false;
        public int? sentnic_attempts { set; get; } = 0;

        [Required]
        public DateTime createddatetime { set; get; }
        public bool isDeleted { set; get; }
        public DateOnly deleteddate { set; get; }

        // Below code added on 06 Aug 2026
        // Below new column is added for Error Correc
        //ALTER TABLE public.applicationdtl ADD COLUMN errorCorrectionIDs text
        public string? errorcorrectionids { get; set; }
        public string? namechangeids { get; set; }
        public string? witnessids { get; set; }
    }
}
