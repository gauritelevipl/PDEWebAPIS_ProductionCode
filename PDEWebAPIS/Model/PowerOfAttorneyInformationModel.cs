using PDEWebAPIS.Repository;
using System.ComponentModel;

namespace PDEWebAPIS.Model
{
    public class PowerOfAttorneyInformationModel
    {
        public int power_of_attorney_id { get; set; }
        public int power_of_attorney_code { get; set; }
        public string? mutation_id {  get; set; }
        // 0 -> Giver , 1 -> Taker
        public bool is_taker { get; set; }
        public UserMaster? userMaster { get; set; }
        public ApplicationDTL? applicationDTL { get; set; }
        public string? mobileno { set; get; } 
        public string? mobilenoverified { set; get; } 
        public string? emailid { set; get; } 
        public string? emailidverified { set; get; }
        //New Fields For Taker
        public string? usertype { set; get; }
        public int usertype_code { set; get; }
        //End
        public string? prefixcode_eng { get; set; }
        public string? prefix_in_eng { set; get; } 
        public string? fname_in_eng { set; get; } 
        public string? mname_in_eng { set; get; } 
        public string? lname_in_eng { set; get; } 
        public string? prefixcode_marathi { get; set; }
        public string? prefix_in_marathi { set; get; } 
        public string? fname_in_marathi { set; get; } 
        public string? mname_in_marathi { set; get; } 
        public string? lname_in_marathi { set; get; }
        //New Fields For Taker
        public string? company_name_in_marathi { set; get; } 
        public string? company_name_in_eng { set; get; } 
        //End
        public string? username { set; get; }
        //New Fields for Taker
        public string? alias_name { set; get; }
        public string? gender_code { set; get; }
        public string? gender_description {  set; get; }
        /*public string? khata_type_code { set; get; }
        public string? khata_type_name { set; get; }
        public string? holder_type_code { set; get; }
        public string? holder_type_name { set; get; }*/
        public string? dob { set; get; }
        public string? mother_name_in_marathi { set; get; }
        public string? mother_name_in_eng { set; get; }
        public string? attornytype_desc { set; get; }
        public int attornytype_code { set; get; }
        /*public int? apk_code { get; set; }
        public string? apk_description { get; set; } = string.Empty;
        public string? aapak { set; get; } = string.Empty;*/
        public string? landBuyArea { set; get; } = string.Empty;
        //End
        public string? address_type { set; get; } 
        public string? address { set; get; } 
        public string? state { set; get; } 
        public string? district { set; get; } 
        public string? taluka { set; get; } 
        public string? city { set; get; } 
        public string? flatno_plotno { set; get; } 
        public string? societyname { set; get; } 
        public string? mainstreet { set; get; } 
        public string? landmark { set; get; } 
        public string? locality { set; get; } 
        public string? pincode { set; get; } 
        public string? postofficename { set; get; } 
        public string? address_proof_document_name { set; get; } 
        public string? address_proof_document_path { set; get; }
        //New Fields For Taker
        public bool owner_of_property_in_maharashtra { set; get; }
        public PropertyTypeMaster? PropertyTypeMaster { get; set; }
        public string? property_district_code { set; get; }
        public string? property_district_name_in_marathi { set; get; }
        public string? property_district_name_in_english { set; get; }
        public string? property_taluka_code { set; get; }
        public string? property_taluka_name { set; get; }
        public string? property_city_code { set; get; }
        public string? property_city_name { set; get; }
        public string? khateno { set; get; } 
        public string? ulpin { set; get; } 
        //End
        public string? village_code { set; get; }
        public string? village_name { set; get; }
        public string? city_servey_no { set; get; }
        public string? lr_property_id { set; get; }
        public string? sub_property_id {  get; set; }
        public string? mutation_srno { set; get; }
        public string? owner_number { set; get; }
        public string? cts_number { set; get; }

        //New Fields For Taker
        public string? profile_pic_file_name { set; get; }
        public string? profile_pic_file_path { set; get; }
        public string isPOAisPartofDast { set; get; } = "NA";
        public string isDeclerationInvolvedInPOA { set; get; } = "NA";
        public string isPOAPermanant { set; get; } = "NA";
        public string isTransferRights { set; get; } = "NA";
        public string dastNo { set; get; } = "NA";
        public string dastNoDate { set; get; } = string.Empty;
        public string dastNoYear { set; get; } = "NA";
        public bool isDastVerified { set; get; } = false;
        public string verifiedDastData { get; set; } = "NA";
        public int? digcode { set; get; } = 0;
        public string digname { set; get; } = "NA";
        public string poa_district_code { set; get; } = string.Empty;
        public string poa_district_name { set; get; } = string.Empty;
     //   public string poa_district_name_in_eng { set; get; } = string.Empty;
        public int sro_office_code { set; get; } 
        public string sro_office_name { set; get; } = string.Empty;
        //End
        public string? signed_file_path { set; get; } 
        public string? signed_file_name { set; get; } 
        public string? poa_giver_ids { set; get; }

      

    }
}
