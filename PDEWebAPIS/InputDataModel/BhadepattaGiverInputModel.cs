namespace PDEWebAPIS.InputDataModel
{
    public class BhadepattaGiverInputModel
    {
        public string? applicationid { get; set; }
        public string? village_code { get; set; }
        public string? ctsNo { get; set; }
        public string? mutationSroNo { get; set; }
        public string? ownerNo { get; set; }
        public int userid { get; set; }
        public UserDetails? userDetails { get; set; }
        public AreaForMutation? areaOfMutation { get; set; }
        public Address? address { get; set; }

    }

    public class UserDetails
    {
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
    }
    public class AreaForMutation
    {
        public string? isFullAreaGiven { get; set; }
        public string? actualArea { get; set; }
        public string? mutationArea { get; set; }
        public string? availableArea { get; set; }
    }
    public class Address
    {
        public string? addressType { get; set; }
        public IndianAddress? indiaAddress { get; set; }
        public ForeignAddress? foreignAddress { get; set; }
    }
    public class IndianAddress
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
    public class ForeignAddress
    {
        public string? address { set; get; }
        public string? mobile { set; get; }
        public string? email { set; get; }
        public string? emailOTP { set; get; }
        public string? signatureName { set; get; }
        public string? signatureSrc { set; get; }
    }
}
