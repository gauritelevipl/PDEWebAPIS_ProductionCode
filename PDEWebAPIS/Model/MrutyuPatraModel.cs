using PDEWebAPIS.Repository;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace PDEWebAPIS.Model
{
    public class MrutyuPatraModel
    {
        public int mutation_givertaker_id { get; set; }
        public UserMaster? userMaster { get; set; }
        public ApplicationDTL? applicationDTL { get; set; }
        public int mutation_cts_no_id { get; set; }
        // 0 -> Giver , 1 -> Taker
        public bool isTaker { get; set; }
        [Required, MaxLength(10)]
        public string? mobileno { get; set; }
        public string? mobilenoverified { get; set; }
        public string? emailid { set; get; }
        public string? emailidverified { set; get; }
        public int usertype_code { set; get; }
        public string? usertype { get; set; }
        //user Details
        public string? prefix_in_marathi { set; get; }
        public string? fname_in_marathi { set; get; }
        public string? mname_in_marathi { set; get; }
        public string? lname_in_marathi { set; get; }
        public string? prefix_in_eng { set; get; }
        public string? fname_in_eng { set; get; }
        public string? mname_in_eng { set; get; }
        public string? lname_in_eng { set; get; }
        public string? alias_name { get; set; }
        public string? holderTypeCode { get; set; }
        public string? holderTypeName { get; set; }
        public string? holderType { set;get; }
        public string? dob { get; set; }
        public string? motherName_in_marathi { get; set; }
        public string? motherName_in_eng { get; set; }
        public string? username { get; set; }
        public string? city_servey_no { get; set; } = null;
        public string? lr_property_id { get; set; }
        public string? milkat { get; set; }
        public string? namud { get; set; }
        public string? isFullAreaGiven { get; set; }
        public string? actualArea { get; set; }
        public string? mutationArea { get; set; }
        public string? availableArea { get; set; }
        public string? address_type { get; set; }
        public string? address { get; set; }
        public string? flatno_plotno { get; set; }
        public string? building { get; set; }
        public string? mainroad { get; set; }
        public string? impSymbol { get; set; }
        public string? area { get; set; }
        public string? pincode { get; set; }
        [Required]
        public string? post_office_name { get; set; }
        public string? city { get; set; }
        public string? taluka { get; set; }
        public string? district { get; set; }
        public string? state { get; set; }
        public string? address_proof_name { get; set; }
        public string? address_proof_path { get; set; }
        public string? signature_name { get; set; }
        public string? signature_src_path { get; set; }

        // 04 Oct 24
        public string owner_of_property_in_maharashtra { set; get; }
        public PropertyTypeMaster? propType { get; set; }
        public string? companyName { set; get; } = "NA";
        public string? companyNameEng { set; get; } = "NA";
        public string? khatano { get; set; } = "NA";
        public string? ulpin { get; set; } = "NA";
        public string? district_code { get; set; } = "NA";
        public string? district_name_in_marathi { get; set; } = "NA";
        public string? district_name_in_eng { get; set; } = "NA";
        public string? office_code { get; set; } = "NA";
        public string? office_name { get; set; } = "NA";
        public string? village_code { get; set; } = "NA";
        public string? village_name { get; set; } = "NA";
        public string? aliceName { get; set; } = "NA";
        public string? owner_status_code { get; set; }
        public string? owner_status_description { get; set; }
        //public string? gender { get; set; } = "NA";
        public string? gender_code { get; set; } = "NA";
        public string? gender_description { get; set; } = "NA";
        public string? khataCode { get; set; } = "NA";
        public string? khataLabel { get; set; } = "NA";
        public string? benefitArea { set; get; }
        public int? apk_code { get; set; }
        public string? apk_description { get; set; }
        public string? aapak { get; set; } = "NA";
        public string? passport_name { set; get; }
        public string? passport_src { set; get; }

    }
}
