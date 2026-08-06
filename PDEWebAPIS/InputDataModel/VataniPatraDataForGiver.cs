namespace PDEWebAPIS.InputDataModel
{
    public class VataniPatraDataForGiver
    {
        public string? applicationid { get; set; }
        public int userid { get; set; }
        //new
        public string? village_code { get; set; }
        public string? ctsNo { get; set; }
        public string? mutationSroNo { get; set; }
        public string? ownerNo { get; set; }
        public UserDTLForVataniPatra? userDetails { get; set; }
        public areaForMutationDTLForVataniPatra? areaForMutation { get; set; }
        public AddressDTLForVataniPatra? address { get; set; }
    }

    public class UserDTLForVataniPatra
    {
        public string? suffix { set; get; }
        public string? suffixcode { set; get; }
        public string? firstName { set; get; }
        public string? middleName { get; set; } = null;
        public string? lastName { set; get; }
        public string? suffixEng { set; get; }
        public string? suffixCodeEng { set; get; }
        public string? firstNameEng { set; get; }
        public string? middleNameEng { set; get; }
        public string? lastNameEng { set; get; }
        public string? aliceName { set; get; }
        public string? dob { set; get; }
        public string? motherName { get; set; }
        public string? motherNameEng { get; set; }
        public string? userName { get; set; }
        public string? nabhu { get; set; }
        public string? lrPropertyUID { set; get; }
        public string? milkat { get; set; }
        public string? namud { get; set; }
        public string? subPropNo { set; get; }
    }
    public class areaForMutationDTLForVataniPatra
    {
        public string? isFullAreaGiven { set; get; }
        public string? actualArea { get; set; }
        public string? mutationArea { get; set; }
        public string? availableArea { set; get; }
    }

    public class AddressDTLForVataniPatra
    {
        public string? addressType { set; get; }
        public IndiaAddressForVataniPatra? indiaAddress { get; set; }
        public AddressForeignForVataniPatra? foreignAddress { set; get; }
    }

    public class IndiaAddressForVataniPatra
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

    public class AddressForeignForVataniPatra
    {
        public string? address { set; get; }
        public string? mobile { set; get; }
        public string? email { set; get; }
        public string? emailOTP { set; get; }
        public string? signatureName { set; get; }
        public string? signatureSrc { set; get; }
    }
}
