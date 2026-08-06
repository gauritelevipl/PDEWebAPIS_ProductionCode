using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PDEWebAPIS.Repository
{
    [Table("power_of_attorney_information")]
    public class PowerOfAttorneyInformation
    {
        [Key, Required]
        public int power_of_attorney_id { get; set; }
        public UserMaster? userMaster { get; set; }
        public ApplicationDTL? applicationDTL { get; set; }
        //New Fields For Taker
        public int power_of_attorney_code { get; set; }
        // 0 -> Giver , 1 -> Taker
        public bool is_taker { get; set; }
        public int usertype_code { set; get; } = 0;
        public string usertype { set; get; } = string.Empty;
        public int mutation_id { set; get; } = 0;
        [Required, MaxLength(15)]
        public string mobileno { set; get; } = string.Empty;
        [DefaultValue("NO")]
        public string mobilenoverified { set; get; } = string.Empty;
        public string emailid { set; get; } = string.Empty;
        [DefaultValue("NO")]
        public string emailidverified { set; get; } = string.Empty;
        public string prefixcode_marathi { get; set; } = string.Empty;
        public string prefix_in_marathi { set; get; } = string.Empty;
        public string fname_in_marathi { set; get; } = string.Empty;
        public string mname_in_marathi { set; get; } = string.Empty;
        public string lname_in_marathi { set; get; } = string.Empty;
        public string prefixcode_eng {  get; set; } = string.Empty;
        public string prefix_in_eng { set; get; } = string.Empty;
        public string fname_in_eng { set; get; } = string.Empty;
        public string mname_in_eng { set; get; } = string.Empty;
        public string lname_in_eng { set; get; } = string.Empty;
        public string company_name_in_marathi { set; get; } = string.Empty;
        public string company_name_in_eng { set; get; } = string.Empty;
        public string username { set; get; } = string.Empty;
        public string alias_name { set; get; } = string.Empty;
        public string gender_code { set; get; } = string.Empty;
        public string gender_description { set; get; } = string.Empty ;
        public string dob { set; get; } = string.Empty;
        public string mother_name_in_marathi { set; get; } = string.Empty;
        public string mother_name_in_eng { set; get; } = string.Empty;
        public string address_type { set; get; } = string.Empty;
        public string address { set; get; } = string.Empty;
        public string state { set; get; } = string.Empty;
        public string district { set; get; } = string.Empty;
        public string taluka { set; get; } = string.Empty;
        public string city { set; get; } = string.Empty;
        public string flatno_plotno { set; get; } = string.Empty;
        public string societyname { set; get; } = string.Empty;
        public string mainstreet { set; get; } = string.Empty;
        public string landmark { set; get; } = string.Empty;
        public string locality { set; get; } = string.Empty;
        public string pincode { set; get; } = string.Empty;
        [Required]
        public string postofficename { set; get; } = string.Empty;
        public string address_proof_document_name { set; get; } = string.Empty;
        public string address_proof_document_path { set; get; } = string.Empty;
        public string city_servey_no { set; get; } = string.Empty;
        public string lr_property_id { set; get; } = string.Empty;
        public string sub_property_no {  set; get; } = "999999";
        public string? mutation_srno { set; get; } = "NA";
        public string? owner_number { set; get; } = "NA";
        public string? cts_number { set; get; } = "NA";
        public string? village_code {  set; get; } = "0";
        public string? village_name { get; set; } = "NA";
        public bool owner_of_property_in_maharashtra { set; get; }
        public PropertyTypeMaster? propertyType { get; set; }
        public string? property_district_code { set; get; }
        public string? property_district_name_in_marathi { set; get; }
        public string? property_district_name_in_english { set; get; }
        public string? property_taluka_code { set; get; }
        public string? property_taluka_name { set; get; }
        public string? property_city_code { set; get; }
        public string? property_city_name { set; get; }
        public string? khateno { set; get; }
        public string? ulpin { set; get; }
        public string khata_type_code { set; get; } = string.Empty;
        public string khata_type_name { set; get; } = string.Empty;
        public string owner_status_code { set; get; } = string.Empty;
        public string owner_status_description { set; get; } = string.Empty;
        public int attornytype_code { set; get; }
        public string? attornytype_desc { set; get; }
        //public int? apk_code { get; set; }
        //public string? apk_description { get; set; } = string.Empty;
        //public string? aapak { set; get; } = string.Empty;
        public string? landBuyArea { set; get; } = string.Empty;
        public string isPOAisPartofDast { set; get; }  = string.Empty;
        public string isDeclerationInvolvedInPOA {  set; get; }  = string.Empty;
        public string isPOAPermanant { set; get; }  = string.Empty;
        public string isTransferRights { set; get; }  = string.Empty;
        public string dast_no { set; get; }  = string.Empty;
        public string dast_no_date { set; get; }  = string.Empty;
        public string dast_no_year { set; get; }  = string.Empty;
        public bool isDastVerified { set; get; } = false;
        public string verifieddastData {  set; get; } = string.Empty;
        public int? digcode {  set; get; } = 0;    
        public string digname { set; get; } = string.Empty;
        public string poa_district_code { set; get; } = string.Empty;
        public string poa_district_name { set; get; } = string.Empty;
      //  public string poa_district_name_in_eng { set; get; } = string.Empty;
        public int sro_office_code { set; get; }
        public string sro_office_name { set; get; } = string.Empty;

        //End
        // Columns for NIC
        public DateTime createddatetime { set; get; }
        public bool isDeleted { set; get; }
        public DateOnly deleteddate { set; get; }
        public string signed_file_path { set; get; } = string.Empty;
        public string signed_file_name { set; get; } = string.Empty;
        public string profile_pic_file_name { set; get; } = string.Empty;
        public string profile_pic_file_path { set; get; } = string.Empty;
        public string poa_giver_ids { set; get; } = string.Empty;
    }
}
