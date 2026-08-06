namespace PDEWebAPIS.InputDataModel
{
    public class AkumaiDataForTaker
    {
        public List<PropertyDataForAkumaiTaker>? giver { get; set; }
        public int usertype_code { set; get; }
        public string? usertype { get; set; }
        public string? applicationid { get; set; }
        public int userid { get; set; }
        public photoDetailsForAkumaiTaker? photo { get; set; }
        public isMHPropertyAkumaiTaker? IsMHProperty { get; set; }
        public DharakAkumaiTaker? dharak { get; set; }
        public AddressDTLAkumaiTaker? address { get; set; }
    }

    public class PropertyDataForAkumaiTaker
    {
        public int? mutation_dtl_id { get; set; }
        public string? nabhu { get; set; }
        public string? subPropNo { get; set; }
    }

    public class isMHPropertyAkumaiTaker
    {
        public string? hasProperty { get; set; }
        public string? propType { get; set; }
        public UserDTLForAkumaiTaker? userDetails { get; set; }

    }
    public class HolderTypeAkumaiTaker
    {
        public string? account_type_code { get; set; }
        public string? account_type_description { get; set; }
    }
    public class DharakAkumaiTaker
    {
        public string? aliceName { get; set; }
        public Gender? gender { get; set; }
        public HolderTypeAkumaiTaker? holderType { get; set; } = null;
        public aapakDropdownForAkumaiTaker? aapakDropdown { get; set; }
        public deadRelationForAkumaiTaker? deadRelation { get; set; }
        public string? aapak { get; set; }
        public string? dob { get; set; }
        public string? motherName { get; set; }
        public string? motherNameEng { get; set; }
        public string? landBuyArea { get; set; }
        public aapakRelationForAkumaiTaker? aapakRelation { get; set; }
    }
    public class aapakDropdownForAkumaiTaker
    {
        public int? apk_code { get; set; }
        public string? apk_description { get; set; }
    }
    public class deadRelationForAkumaiTaker
    {
        public int relation_code { get; set; }
        public string? relation_name { get; set; }
    }
    public class UserDTLForAkumaiTaker
    {
        public string? suffixcode { set; get; }
        public string? suffixCodeEng { set; get; }
        public string? suffix { set; get; } = null;
        public string? suffixEng { set; get; }
        public string? firstName { set; get; }
        public string? middleName { get; set; } = null;
        public string? lastName { set; get; }
        public string? firstNameEng { set; get; }
        public string? middleNameEng { set; get; }
        public string? lastNameEng { set; get; }
        public string? khataNo { set; get; }
        public string? naBhu { set; get; }
        public string? ulpin { set; get; }
        public string? userName { get; set; }
        public District? district { set; get; }
        public Taluka? taluka { set; get; }
        public VillageforKharediNond? village { set; get; }

        public string companyName { set; get; } = string.Empty;
        public string companyNameEng { set; get; } = string.Empty;
    }
    public class photoDetailsForAkumaiTaker
    {
        public string? passportName { get; set; }
        public string? passportSrc { get; set; }
    }
    public class AddressDTLAkumaiTaker
    {
        public string? addressType { set; get; }
        public IndiaAddressAkumaiTaker? indiaAddress { get; set; }
        public AddressForForeign? foreignAddress { set; get; }
    }
    public class IndiaAddressAkumaiTaker
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

    public class aapakRelationForAkumaiTaker
    {
        public string? relation_code { get; set; }
        public string? relation_name { get; set; }
    }
    public class EditDTLForAkumaiTaker
    {
        public int? MutationId { get; set; }
        public int usertype_code { set; get; }
        public string? usertype { get; set; }
        public string? applicationid { get; set; }
        public int userid { get; set; }
        public photoDetailsForAkumaiTaker? photo { get; set; }
        // public signatureDetails? signature { get; set; }
        public isMHPropertyAkumaiTaker? IsMHProperty { get; set; }
        public DharakAkumaiTaker? dharak { get; set; }
        public AddressDTLAkumaiTaker? address { get; set; }
    }
}
