namespace PDEWebAPIS.InputDataModel
{
    public class VataniPatraDataForTaker
    {
        public int usertype_code { set; get; }
        public string? usertype { get; set; }
        public string? applicationid { get; set; }
        public int userid { get; set; }
        public isMHPropertyForVataniPatraNondTaker? isMHProperty { get; set; }
        public DharakVataniPatraNondTaker? dharak { get; set; }
        public AddressDTLVataniPatraNondTaker? address { get; set; }
        public List<PropertyDataForVataniPatraNondTaker>? giver { get; set; }
    }
    public class PropertyDataForVataniPatraNondTaker
    {
        public int? mutation_dtl_id { get; set; }
        public string? nabhu { get; set; }
        public string? subPropNo { get; set; }
    }
    public class isMHPropertyForVataniPatraNondTaker
    {
        public string? hasProperty { get; set; }
        public string? propType { get; set; }
        public UserDTLForVataniPatraNondTaker? userDetails { get; set; }
    }
    public class UserDTLForVataniPatraNondTaker
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

    public class HolderTypeVataniPatraNondTaker
    {
        public string? owner_status_code { get; set; }
        public string? owner_status_description { get; set; }
    }
    public class UserDharakVataniPatraNondTaker
    {
        public string? aliceName { get; set; }
        public aapakDropdownForVataniPatraNondTaker? aapakDropdown { get; set; }
        public string? aapak { get; set; }
        public aapakRelationForVataniPatraNondTaker? aapakRelation { get; set; }
        public Gender? gender { get; set; }
        public HolderTypeVataniPatraNondTaker? holderType { get; set; } = null;
        public string? dob { get; set; }
        public string? motherName { get; set; }
        public string? motherNameEng { get; set; }
        public string? landBuyArea { get; set; }
    }
    public class CompanyDharakVataniPatraTaker
    {
        public HolderTypeVataniPatraNondTaker? holderType { get; set; }
        public string? landBuyArea { get; set; }
    }
    public class DharakVataniPatraNondTaker
    {
        public UserDharakVataniPatraNondTaker? userdharak { set; get; }
        public CompanyDharakVataniPatraTaker? companydharak { get; set; }
    }
    public class aapakDropdownForVataniPatraNondTaker
    {
        public int? apk_code { get; set; }
        public string? apk_description { get; set; }
    }


    public class AddressDTLVataniPatraNondTaker
    {
        public string? addressType { set; get; }
        public IndiaAddressVataniPatraNondTaker? indiaAddress { get; set; }
        public AddressForForeign? foreignAddress { set; get; }
    }
    public class IndiaAddressVataniPatraNondTaker
    {
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
    }
    public class aapakRelationForVataniPatraNondTaker
    {
        public string? relation_code { get; set; }
        public string? relation_name { get; set; }
    }
    public class FetchVataniPatraNondDataForTaker
    {
        public int? mutation_dtl_id { get; set; }
        public int? userid { get; set; }
        public string? applicationid { get; set; }
        public int usertype_code { set; get; }
        public string? userType { get; set; }
        public string? fullNameInMarathi { set; get; }
        public string? fullNameInEng { set; get; }
        public string? mobileNo { set; get; }
        public isMHPropertyForVataniPatraNondTaker? isMHProperty { get; set; }
        public DharakVataniPatraNondTaker? dharak { get; set; } = null;
        public AddressDTLVataniPatraNondTaker? address { get; set; }
    }
}
