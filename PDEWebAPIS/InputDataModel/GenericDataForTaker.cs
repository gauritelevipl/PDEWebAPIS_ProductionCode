namespace PDEWebAPIS.InputDataModel
{
    public class GenericDataForTaker
    {
        public int usertype_code { set; get; }
        public string? usertype { get; set; }
        public string? applicationid { get; set; }
        public List<PropertyDataForGenericNondTaker>? giver { get; set; }
        public int userid { get; set; }
        public photoDetailsForGenericTaker? photo { get; set; }
        public isMHPropertyForGenericTaker? isMHProperty { get; set; }
        public dharakDetailsyForGenericTaker? dharak { get; set; }
        public AddressDTLForGenericGiver? address { get; set; }
    }


    public class PropertyDataForGenericNondTaker
    {
        public int? mutation_dtl_id { get; set; }
        public string? nabhu { get; set; }
        public string? subPropNo { get; set; }
    }

    public class photoDetailsForGenericTaker
    {
        public string? passportName { get; set; }
        public string? passportSrc { get; set; }

    }

    public class isMHPropertyForGenericTaker
    {
        public string? hasProperty { get; set; }
        public string? propType { get; set; }
        public TakeruserDetailsForGenericTaker? userDetails { get; set; }
    }
    public class TakeruserDetailsForGenericTaker
    {
        public string? suffixcode { set; get; }
        public string? suffixCodeEng { set; get; }
        public string? suffix { get; set; }
        public string? suffixEng { get; set; }
        public string? firstName { get; set; }
        public string? middleName { get; set; }
        public string? lastName { get; set; }
        public string? firstNameEng { get; set; } = null;
        public string? middleNameEng { get; set; }
        public string? lastNameEng { get; set; }
        public string companyName { set; get; } = string.Empty;
        public string companyNameEng { set; get; } = string.Empty;
        public string? khataNo { get; set; }
        public string? naBhu { get; set; }
        public string? userName { get; set; }
        public string? ulpin { get; set; }
        public DistrictForGenericeTaker? district { get; set; }
        public TalukaForGenericeTaker? taluka { get; set; }
        public VillageForGenericeTaker? village { get; set; }
    }
    public class DistrictForGenericeTaker
    {
        public string? district_code { get; set; }
        public string? district_name { get; set; }
        public string? district_english_name { get; set; }
    }
    public class TalukaForGenericeTaker
    {
        public string? office_code { get; set; }
        public string? office_name { get; set; }
    }
    public class VillageForGenericeTaker
    {
        public string? village_code { get; set; }
        public string? village_name { get; set; }
    }
    public class HoldertypeForGenericeTaker
    {
        public string? owner_status_code { get; set; }
        public string? owner_status_description { get; set; }
    }
    public class KhataTypeForGenericeTaker
    {
        public string? khataCode { get; set; }
        public string? khataLabel { get; set; }
    }
    public class aapakDropdownForGenericeTaker
    {
        public int? apk_code { get; set; }
        public string? apk_description { get; set; }
    }
    public class dharakDetailsyForGenericTaker
    {
        public userdharakDetailsForGenericeTaker? userdharak { get; set; }
        public companydharakDetailsForGenericeTaker? companydharak { get; set; }
    }

    public class userdharakDetailsForGenericeTaker
    {
        public string? aliceName { get; set; }
        public aapakDropdownForGenericeTaker? aapakDropdown { get; set; }
        public string? aapak { get; set; }
        public aapakRelationForGenericeTaker? aapakRelation { get; set; }
        public GenderForGenericeTaker? gender { get; set; }
        //public KhataType? khataType { get; set; }
        public HoldertypeForGenericeTaker? holderType { get; set; }
        public string? dob { get; set; }
        public string? motherName { get; set; }
        public string? motherNameEng { get; set; }
        public string? landBuyArea { get; set; }
    }

    public class aapakRelationForGenericeTaker
    {
        public string? relation_code { get; set; }
        public string? relation_name { get; set; }
    }
    public class GenderForGenericeTaker
    {
        public string? gender_code { get; set; }
        public string? gender_description { get; set; }
    }



    public class companydharakDetailsForGenericeTaker
    {
        public HoldertypeForGenericeTaker? holderType { get; set; }
        // public KhataType? khataType { get; set; }
        //public aapakDropdown? aapakDropdown { get; set; }
        //public string? aapak { get; set; }
        public string? landBuyArea { get; set; }
    }

    public class FetchGenericNondDataForTaker
    {
        public int? mutation_dtl_id { get; set; }
        public int? userid { get; set; }
        public string? applicationid { get; set; }
        public int usertype_code { set; get; }
        public string? userType { get; set; }
        public string? fullNameInMarathi { set; get; }
        public string? fullNameInEng { set; get; }
        public string? mobileNo { set; get; }
        public photoDetailsForGenericTaker? photo { get; set; }
        public isMHPropertyForGenericTaker? isMHProperty { get; set; }
        public dharakDetailsyForGenericTaker? dharak { get; set; } = null;
        public AddressDTLForGenericGiver? address { get; set; }
    }
}
