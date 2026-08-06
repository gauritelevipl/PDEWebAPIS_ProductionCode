using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace PDEWebAPIS.Repository
{
    [Table("mutationgivertakerDtls")]
    public class MutationGiverTakerDTL
    {
        [Key, Required]
        public int mutation_givertaker_id { get; set; }
        public UserMaster? userMaster { get; set; }
        public ApplicationDTL? applicationDTL { get; set; }
        public int mutation_cts_no_id { get; set; } = 0;
        public int user_type_code { get; set; }
        public string? user_type { get; set; }
        public PropertyTypeMaster? prop_type { get; set; }
        // 0 -> Giver , 1 -> Taker, 2 -> Varas
        public int isTaker { get; set; }
        [Required, MaxLength(15)]
        public string? mobileno { get; set; }
        [DefaultValue("FALSE")]
        public string? mobilenoverified { get; set; }
        public string? emailid { set; get; }
        [DefaultValue("FALSE")]
        public string? emailidverified { set; get; }
        //user Details

        public string prefixcode_marathi { set; get; } = string.Empty;
        public string? prefix_in_marathi { set; get; }
        public string? fname_in_marathi { set; get; }
        public string? mname_in_marathi { set; get; }
        public string? lname_in_marathi { set; get; }
        public string? prefixcode_eng { set; get; }
        public string? prefix_in_eng { set; get; }
        public string? fname_in_eng { set; get; }
        public string? mname_in_eng { set; get; }
        public string? lname_in_eng { set; get; }
        public string? alias_name { get; set; }
        public string? company_name_in_marathi { get; set; }
        public string? company_name_in_eng { get; set; }
        public string? gender_code { get; set; }
        public string? gender_description { get; set; }
        public string? holder_type { get; set; }
        public string? dob { get; set; }
        public string? mother_name_in_marathi { get; set; }
        public string? mother_name_in_eng { get; set; }
        public string? userName { get; set; }
        public string? city_servey_no { get; set; } = null;
        public string? lr_property_id { get; set; }
        public string? sub_property_no { get; set; }
        // NIC Columns
        public string? sellerid { set; get; }
        public string? buyerid { set; get; }
        public string? mutation_srno { set; get; }
        public string? owner_number { set; get; }
        public string? cts_number { set; get; }
        public string? milkat { get; set; }
        public string? namud { get; set; }
        public string? isFullAreaGiven { get; set; }
        public string? actual_area { get; set; }
        public string? available_area { get; set; }
        public string? mutation_area { get; set; }

        //Bakshish Patra
        public string? address_type { get; set; }
        public string? address { get; set; }
        public string? state { get; set; }
        public string? district { get; set; }
        public string? taluka { get; set; }
        public string? city { get; set; }
        public string? flatno_plotno { get; set; }
        public string? societyname { get; set; }
        public string? mainstreet { get; set; }
        public string? landmark { get; set; }
        public string? locality { get; set; }
        public string? pincode { get; set; }
        [Required]
        public string? post_office_name { get; set; }
        public string? address_proof_document_name { get; set; }
        public string? address_proof_document_path { get; set; }
        public string? has_property { get; set; }
        //public string? khata_type { get; set; }   //Commented By Gauri becaues we have to remove th this column
  

        //public string? aapak_dropdown { get; set; }   //Commented By Gauri becaues we have to remove th this column
        public string? aapak { get; set; }
        public string? land_buy_area { get; set; }
        //Bakshish Patra Taker
        //11-11-2024 As per discussion only add 3 column for area
      //  public string? gift_area { get; set; }


        // 27 Sept 24
        public int? account_type_code { get; set; }
        public string? account_type_description { get; set; }
        public int? apk_code { get; set; }
        public string? apk_description { get; set; }

        // 01 Oct 24
        public string? khata_type_code { get; set; }
        public string? khata_type_name{ get; set; }
        public string? owner_status_code { get; set; }
        public string? owner_status_description { get; set; }

        // 30 Sept 24
        public string? khatano { get; set; }
        public string? ulpin { get; set; }
        public string? district_code { get; set; }
        public string? district_name_in_marathi { get; set; }
        public string? district_name_in_eng { get; set; }
        public string? village_code { get; set; }
        public string? village_name { get; set; }
        public string? ofc_code { get; set; }
        public string? ofc_name { get; set; }

        // 04 Oct 24 For MrutyuPatra
        // public string? actualArea { set;get; }
       // public string? benefitArea { set; get; }

        public int relation_code { get; set; }
        public string? relation_name { get; set; }

        public int varas_relation_code { get; set; }
        public string? varas_relation_name { get; set; }

        // 11 Oct 24
        // Gahankhat Nond Denar
        public int institute_code { set; get; }
        public string? institute_description { set; get; }
        public string? bank_name_in_marathi { set; get; }
        public string? bank_name_in_english { set; get; }
        public string? ifsc {  set; get; }
        //public string? boja_area { set; get; }
        public string? boja_value { set; get; }
        public string? boja_date { set;get; }
        public string? boja_period { set; get; }
        // Hakkasod taker
        public string? benefit_amt {  set; get; }
        public bool isDeleted { set; get; }
        public DateOnly deleteddate { set; get; }
        public DateTime createddatetime { get; set; }
        public string? signed_file_name { get; set; }
        public string? signed_file_path { get; set; }
        //Taker
        public string? profile_pic_file_name { get; set; }
        public string? profile_pic_file_path { get; set; }

        public string? owner_village_code { get; set; }
        // New fields for Generic 
        public string? entry_date { set; get; }
        public string? entry_bracketed { set; get; }
        public string? owner_name { get; set; }
        public string? owner_bracketed { get; set; }
    }
}
