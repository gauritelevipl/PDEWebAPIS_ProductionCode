namespace PDEWebAPIS.InputDataModel
{
    public class GenericDataForGiver
    {
        public string? applicationid { get; set; }
        public int userid { get; set; }
        public string? subPropNo { get; set; }
        public string? village_code { get; set; }
        public string? cts_number { get; set; }
        public string? mutation_srno { get; set; }
        public string? entry_date { get; set; }
        public string? entry_bracketed { get; set; }
        public string? owner_number { get; set; }
        public string? owner_name { get; set; }
        public string? owner_bracketed { get; set; }
        public string? nabhu { get; set; }
        public string? suffixEng { get; set; }
        public string? suffixCodeEng { get; set; }
        public string? firstNameEng { get; set; }
        public string? middleNameEng { get; set; }
        public string? lastNameEng { get; set; }
        public string? suffixcode { get; set; }
        public string? suffix { get; set; }
        public string? first_name { get; set; }
        public string? middle_name { get; set; }
        public string? last_name { get; set; }
        public string? lrPropertyUID { get; set; }
        public string? milkat { get; set; }
        public string? namud { get; set; }
        public string? actualArea { get; set; }

        //// Below fields are for Additional data
        public string? mutation_dtl_id { get; set; }
        public string? ctsNo { get; set; }
        public string? mutationSroNo { get; set; }
        public string? ownerNo { get; set; }
        public UserDTLForGenericGiver? userDetails { get; set; }
        public areaForMutationDTLGenericGiver? areaForMutation { get; set; }
        public AddressDTLForGenericGiver? address { get; set; }
    }

    public class UserDTLForGenericGiver
    {
        public string? firstName { set; get; }
        public string? middleName { get; set; } = null;
        public string? lastName { set; get; }
        public string? firstNameEng { set; get; }
        public string? middleNameEng { set; get; }
        public string? lastNameEng { set; get; }
        public string? aliceName { set; get; }
        public string? dob { set; get; }
        public string? motherName { get; set; }
        public string? motherNameEng { get; set; }
        public string? userName { get; set; }
        public string? suffix { set; get; } = null;
        public string? suffixEng { set; get; }
        public string? suffixcode { set; get; }
        public string? suffixCodeEng { set; get; }
        public string? subPropNo { set; get; }
        public string? nabhu { get; set; }
        public string? lrPropertyUID { set; get; }
        public string? milkat { get; set; }
        public string? namud { get; set; }

    }

    public class areaForMutationDTLGenericGiver
    {
        public string? isFullAreaGiven { set; get; }
        public string? actualArea { get; set; }
        public string? mutationArea { get; set; }
        public string? availableArea { set; get; }
    }

    public class AddressDTLForGenericGiver
    {
        public string? addressType { set; get; }
        public IndiaAddressForGenericGiver? indiaAddress { get; set; }
        public AddressForForeignForGenericGiver? foreignAddress { set; get; }
    }

    public class IndiaAddressForGenericGiver
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

    public class AddressForForeignForGenericGiver
    {
        public string? address { set; get; }
        public string? mobile { set; get; }
        public string? email { set; get; }
        public string? emailOTP { set; get; }
        public string? signatureName { set; get; }
        public string? signatureSrc { set; get; }
    }
}
