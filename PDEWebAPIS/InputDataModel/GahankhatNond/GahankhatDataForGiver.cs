namespace PDEWebAPIS.InputDataModel.GahankhatNond
{
    public class GahankhatDataForGiver
    {
        public int userid {  get; set; }
        public string? applicationid { set; get; }
        public List<PropertyDataForGahankhatGiver>? giver { get; set; }
        public UserDetailsForGahankhatGiver? userDetails { set; get; }
        public AddressDTLForGahankhatGiver? address { set; get; }
    }
    public class PropertyDataForGahankhatGiver
    {
        public int? mutation_dtl_id { get; set; }
        public string? nabhu { get; set; }
        public string? subPropNo { get; set; }
    }

    public class UserDetailsForGahankhatGiver
    {
        public BankDataForGahankhatGiver? bankDropdown {  set; get; }
        public string? bankNameMar {  set; get; }   
        public string? bankNameEng {  set; get; }
        public string? ifsc {  set; get; }
        public string? bojaArea {  set; get; }
        public string? bojaValue {  set; get; }
        public string? bojaDate {  set; get; }
        public string? bojaPeriod {  set; get; }
    }

    public class BankDataForGahankhatGiver
    {
        public int institute_code { set; get; }
        public string? institute_description { set; get; }
    }
    public class AddressDTLForGahankhatGiver
    {
        public string? addressType { set; get; }
        public IndiaAddressForGahankhatGiver? indiaAddress { get; set; }
        public AddressForForeignForGahankhatGiver? foreignAddress { set; get; }
    }

    public class IndiaAddressForGahankhatGiver
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

    public class AddressForForeignForGahankhatGiver
    {
        public string? address { set; get; }
        public string? mobile { set; get; }
        public string? email { set; get; }
        public string? emailOTP { set; get; }
        public string? signatureName { set; get; }
        public string? signatureSrc { set; get; }
    }

    public class EditGahankhatDataForGiver
    {
        public int? MutationId { get; set; }
        public int userid { get; set; }
        public string? applicationid { set; get; }
        public UserDetailsForGahankhatGiver? userDetails { set; get; }
        public AddressDTLForGahankhatGiver? address { set; get; }
    }
}
