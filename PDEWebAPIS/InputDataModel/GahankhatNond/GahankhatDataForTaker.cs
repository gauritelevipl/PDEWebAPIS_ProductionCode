namespace PDEWebAPIS.InputDataModel.GahankhatNond
{
    public class GahankhatDataForTaker
    {
        public int userid {  get; set; }
        public string? applicationid {  get; set; }
        public string? mutationSroNo { set; get; }
        public string? village_code { set; get; }
        
        public string? ctsNo { set; get; }
        public string? ownerNo { set; get; }
        public UserDetailsForGahankhatTaker? userDetails { get; set; }
        public AreaForMutationForGahankhatTaker? areaForMutation { get; set; }
       public AddressDTLForGahankhatTaker? address {  get; set; }
    }

    public class UserDetailsForGahankhatTaker
    {
        public string? firstName { get; set; }
        public string? middleName { get; set; }
        public string? lastName { get; set; }
        public string? firstNameEng { get; set; }
        public string? middleNameEng { get; set; }
        public string? lastNameEng { get; set; }
        public string? aliceName { get; set; }
        public string? holderType { get; set; }
        public string? dob {  get; set; }
        public string? motherName {  get; set; }
        public string? motherNameEng {  get; set; }
        public string? userName {  get; set; }
        public string? suffixcode { set; get; }
        public string? suffixCodeEng { set; get; }
        public string? suffix { get; set; }
        public string? suffixEng { get; set; }
        public string? nabhu { get; set; }
        public string? lrPropertyUID { get; set; }
        public string? milkat {  get; set; }
        public string? namud { get; set; }
        public string? subPropNo { get; set; }

    }

    public class AreaForMutationForGahankhatTaker
    {
        public string? isFullAreaGiven { get; set; }
        public string? actualArea { get; set; }
        public string? mutationArea { get; set; }
        public string? availableArea { get; set; }
    }

    public class AddressDTLForGahankhatTaker
    {
        public string? addressType { set; get; }
        public IndiaAddressForGahankhatTaker? indiaAddress { get; set; }
        public AddressForForeignForGahankhatTaker? foreignAddress { set; get; }
    }

    public class IndiaAddressForGahankhatTaker
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

    public class AddressForForeignForGahankhatTaker
    {
        public string? address { set; get; }
        public string? mobile { set; get; }
        public string? email { set; get; }
        public string? emailOTP { set; get; }
        public string? signatureName { set; get; }
        public string? signatureSrc { set; get; }
    }

    public class EditGahankhatDataForTaker
    {
        public int? MutationId { get; set; }
        public int userid { get; set; }
        public string? applicationid { get; set; }
        public UserDetailsForGahankhatTaker? userDetails { get; set; }
        public AreaForMutationForGahankhatTaker? areaForMutation { get; set; }
        public AddressDTLForGahankhatTaker? address { get; set; }
    }
}
