using PDEWebAPIS.Repository;

namespace PDEWebAPIS.Model
{
    public class KharediNondModel
    {
        public int mutation_givertaker_id { get; set; }
        // 0 -> Giver , 1 -> Taker
        public bool isTaker { get; set; } = false;
        public UserMaster? userMaster { get; set; }
        public int mutation_cts_no_id { get; set; }
        public string? ctsNo { get; set; }
        public string? mutationSroNo { get; set; }
        public string? ownerNo { get; set; }
        public ApplicationDTL? applicationDTL { get; set; }
        public string? mobileno { set; get; } = "NA";
        public string? mobilenoverified { set; get; } = "NA";
        public string? emailid { set; get; } = "NA";
        public string? emailidverified { set; get; } = "NA";

        //user Details
        public string? prefixcode_marathi { get; set; }
        public string? prefix_in_marathi { set; get; } = "NA";
        public string? fname_in_marathi { set; get; } = "NA";
        public string? mname_in_marathi { set; get; } = "NA";
        public string? lname_in_marathi { set; get; } = "NA";
        public string? prefixcode_eng { get; set; }
        public string? prefix_in_eng { set; get; } = "NA";
        public string? fname_in_eng { set; get; } = "NA";
        public string? mname_in_eng { set; get; } = "NA";
        public string? lname_in_eng { set; get; } = "NA";
        public string? aliceName { get; set; } = "NA";
        public string? holderType { get; set; } = "NA";
        public string? owner_status_code { get; set; }
        public string? owner_status_description { get; set; }
        public string? dob { get; set; } = "NA";
        public string? motherName_in_marathi { get; set; } = "NA";
        public string? motherName_in_eng { get; set; } = "NA";
        public string? userName { get; set; } = "NA";
        public string? city_servey_no { get; set; } = "NA";
        public string? lr_property_id { get; set; } = "NA";
        public string? milkat { get; set; } = "NA";
        public string? namud { get; set; } = "NA";
        public string? sub_property_no { get; set; }
        //Taker
        public string? khatano { get; set; } = "NA";
        public string? ulpin { get; set; } = "NA";
        public string? district_code { get; set; } = "NA";
        public string? district_name_in_marathi { get; set; } = "NA";
        public string? district_name_in_eng { get; set; } = "NA";
        public string? office_code { get; set; } = "NA";
        public string? office_name { get; set; } = "NA";
        public string? village_code { get; set; } = "NA";
        public string? village_name { get; set; } = "NA";


        //user details End

        //area for Mutation
        public string? isFullAreaGiven { get; set; } = "NA";
        public string? actualArea { get; set; } = "NA";
        public string? mutationArea { get; set; } = "NA";

        //For Bakshish Patra

        public string? availableArea { get; set; } = "NA";

        //End area for Mutation

        //address
        public string? address_type { get; set; } = "NA";

        //India Address
        public string? plotno { get; set; } = "NA";
        public string? building { get; set; } = "NA";
        public string? mainroad { get; set; } = "NA";
        public string? impSymbol { get; set; } = "NA";
        public string? area { get; set; } = "NA";
        public string? pincode { get; set; } = "NA";
        public string? post_office_name { get; set; } = "NA";
        public string? city { get; set; } = "NA";
        public string? taluka { get; set; } = "NA";
        public string? district { get; set; } = "NA";
        public string? state { get; set; } = "NA";
        public string? address_proof_name { get; set; } = "NA";
        public string? address_proof_src { get; set; } = "NA";
        //End

        //Foreign Address
        public string? address { get; set; } = "NA";
        public string? signature_name { get; set; } = "NA";
        public string? signature_src { get; set; } = "NA";

        //End

        //Taker
        public int usertype_code { get; set; } 
        public string? usertype { get; set; } = "NA";
        public string? passport_name { get; set; } = "NA";
        public string? passport_src { get; set; } = "NA";
        public string? hasProperty { get; set; } = "NA";
        public PropertyTypeMaster? propType { get; set; }
        public string? gender_code { get; set; } = "NA";
        public string? gender_description { get; set; } = "NA";

        //public string? khataType { get; set; } = "NA";
        public string? khataCode { get; set; } = "NA";
        public string? khataLabel { get; set; } = "NA";
        public string? companyName { get; set; } = "NA";
        public string? companyNameEng { get; set; } = "NA";
        //public string? aapakDropdown { get; set; } = "NA";
        public int? apk_code { get; set; }
        public string? apk_description { get; set; }
        public string? aapak { get; set; } = "NA";
        public string? aapak_name { get; set; } = "NA";
        public string? landBuyArea { get; set; } = "NA";

        //Bakshish Patra Ghenar
        public string? giftArea { get; set; } = "NA";
        public MayatDTL? mayatDTL { set; get; }

        public int relation_code { set; get; }
        public string? relation_name { set; get; }
        public int dead_relation_code { set; get; }
        public string? dead_relation_name { set; get; }

        //Hakka Sod Taker
        public string? benefit_amt { get; set; }

        public string? owner_village_code { get; set; }
    }
}
