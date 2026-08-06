namespace PDEWebAPIS.InputDataModel
{
    public class AkumaiDataForGiver
    {
        public string? applicationid { get; set; }
        public int userid { get; set; }
        //new
        public string? village_code { get; set; }
        public string? ctsNo { get; set; }
        public string? mutationSroNo { get; set; }
        public string? ownerNo { get; set; }
        public UserDTLForAkumaiNond? userDetails { get; set; }
        public areaForMutationDTLAkumaiNond? areaForMutation { get; set; }
        public AddressDTLForAkumaiNond? address { get; set; }
    }
    public class UserDTLForAkumaiNond
    {
        public string? firstName { set; get; }
        public string? middleName { get; set; } = null;
        public string? lastName { set; get; }
        public string? firstNameEng { set; get; }
        public string? middleNameEng { set; get; }
        public string? lastNameEng { set; get; }
        public string? aliceName { set; get; }
        public string? holderType { set; get; }
        public string? dob { set; get; }
        // public Aapak? aapak { set; get; }
        public string? motherName { get; set; }
        public string? motherNameEng { get; set; }
        public string? userName { get; set; }
        public string? suffix { set; get; } = null;
        public string? suffixEng { set; get; }
        public string? suffixcode { set; get; }
        public string? suffixCodeEng { set; get; }
        public string? nabhu { get; set; }
        public string? lrPropertyUID { set; get; }
        public string? milkat { get; set; }
        public string? namud { get; set; }
        public string? subPropNo { set; get; }
        public AapakDropDownForAkumaiNond? aapakDropdown { set; get; }

    }
    public class areaForMutationDTLAkumaiNond
    {
        public string? isFullAreaGiven { set; get; }
        public string? actualArea { get; set; }
        public string? mutationArea { get; set; }
        public string? availableArea { set; get; }
    }

    public class AddressDTLForAkumaiNond
    {
        public string? addressType { set; get; }
        public IndiaAddressForAkumaiNond? indiaAddress { get; set; }
        public AddressForForeign? foreignAddress { set; get; }
    }

    public class IndiaAddressForAkumaiNond
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
        public string addressProofSrc { get; set; }
        public string signatureName { get; set; }
        public string signatureSrc { get; set; }
    }

    public class AapakDropDownForAkumaiNond
    {
        public string? apk_code { set; get; }
        public string? apk_description { set; get; }
    }
}
