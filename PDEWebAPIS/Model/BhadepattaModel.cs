using PDEWebAPIS.Repository;

namespace PDEWebAPIS.Model
{
    public class BhadepattaModel
    {
        public string? applicationid { get; set; }
        public string? village_code { get; set; }
        public string? ctsNo { get; set; }
        public string? mutationSroNo { get; set; }
        public string? ownerNo { get; set; }
        public int mutation_cts_no_id { get; set; }
        public UserMaster? userMaster { get; set; }
        public ApplicationDTL? applicationDTL { get; set; }

        //user details
        public string? firstName { get; set; }
        public string? middleName { get; set; }
        public string? lastName { get; set; }
        public string? firstNameEng { get; set; }
        public string? middleNameEng { get; set; }
        public string? lastNameEng { get; set; }
        public string? aliceName { get; set; }
        public string? dob { get; set; }
        public string? motherName { get; set; }
        public string? motherNameEng { get; set; }
        public string? userName { get; set; }
        public string? suffix { get; set; }
        public string? suffixEng { get; set; }
        public string? suffixcode { get; set; }
        public string? suffixCodeEng { get; set; }
        public string? nabhu { get; set; }
        public string? lrPropertyUID { get; set; }
        public string? milkat { get; set; }
        public string? namud { get; set; }
        public string? holderType { get; set; }
        public string? subPropNo { get; set; }
        public string? mobileno { get; set; }
        public string? mobilenoverified { get; set; }
        public string? emailid { get; set; }
        public string? emailidverified { get; set; }
        public string? gender_code { get; set; }
        public string? gender_description { get; set; }
        public string? usertype { get; set; }
        public int usertype_code { get; set; }
        public string? passport_name { get; set; }
        public string? passport_src { get; set; }
        public string? hasProperty { get; set; }
        public string? khataCode { get; set; }
        public string? khataLabel { get; set; }
        public string? Khatano { get; set; }
        public string? companyName { get; set; }
        public string? companyNameEng { get; set; }
        public int? apk_code { get; set; }
        public string? apk_description { get; set; }
        public string? aapak { get; set; }
        public string? aapak_name { get; set; }
        public string? landBuyArea { get; set; }
        public int? account_type_code { get; set; }
        public string? account_type_description { get; set; }
        public string? khatano { get; set; }
        public string? ulpin { get; set; }
        public string? district_code { get; set; }
        public string? district_name_in_marathi { get; set; }
        public string? district_name_in_eng { get; set; }
        public string? office_code { get; set; }
        public string? office_name { get; set; }
        public string? village_name { get; set; }
        public int relation_code { set; get; }
        public string? relation_name { set; get; }

        //AreaForMutation
        public string? isFullAreaGiven { get; set; }
        public string? actualArea { get; set; }
        public string? mutationArea { get; set; }
        public string? availableArea { get; set; }

        //address
        public string? addressType { get; set; }
        public string? plotNo { set; get; }
        public string? building { get; set; }
        public string? mainRoad { get; set; }
        public string? impSymbol { get; set; }
        public string? area { get; set; }
        public string? mobile { get; set; }
        public string? mobileOTP { get; set; }
        public string? pincode { get; set; }
        public string? postOfficeName { get; set; }
        public string? city { get; set; }
        public string? taluka { get; set; }
        public string? district { get; set; }
        public string? state { get; set; }
        public string? addressProofName { get; set; }
        public string? addressProofSrc { get; set; }
        public string? signatureName { get; set; }
        public string? signatureSrc { get; set; }
        public string? address { set; get; }
        public string? email { set; get; }
        public string? emailOTP { set; get; }
        public string? owner_village_code { get; set; }

        public PropertyTypeMaster? propType { get; set; }

        public string? owner_status_code { get; set; }
        public string? owner_status_description { get; set; }
        public string? city_servey_no { get; set; }
        // 0 -> Giver , 1 -> Taker
        public int isTaker { get; set; }
    }
}
