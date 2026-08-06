namespace PDEWebAPIS.InputDataModel
{
    public class PowerOfAttorneyInformationDataForGiver 
    {
        public int userid { get; set; }
        public string? applicationid { get; set; }
        public string? mutation_id {  get; set; }
        public string? ctsNo { get; set; }
        public string? mutationSroNo { get; set; }
        public string? ownerNo { get; set; }
        public string? village_code { get; set; }
        public string? village_name { set; get; }
        public UserDataForPOAGiver? userDetails { set; get; }
        public AddressData? address { set; get; }
    }
    public class UserDataForPOAGiver
    {
        public string? suffixcode { set; get; }
        public string? suffixCodeEng { set; get; }
        public string? suffix { set; get; }
        public string? firstName { set; get; }
        public string? middleName { set; get; }
        public string? lastName { set; get; }
        public string? suffixEng { set; get; }
        public string? firstNameEng { set; get; }
        public string? middleNameEng { set; get; }
        public string? lastNameEng { set; get; }
        /*public string? aliceName { set; get; }
        public string? holderType { set; get; }
        public string? dob { set; get; }
        public string? motherName { set; get; }
        public string? motherNameEng { set; get; }*/
        public string? userName { set; get; }
        public string? nabhu { set; get; }
        public string? lrPropertyUID { set; get; }
        public string? subPropNo { get; set; }
        //public string? milkat { set; get; }
        //  public string? namud { set; get; }
        //User Type fields
        public int usertype_code { set; get; }
        public string? usertype {  set; get; }
        public string? company_name_in_marathi {  set; get; }
        public string? company_name_in_eng {  set; get; }
        //end
    }
    public class AddressData
    {
        public string? addressType { set; get; }
        public AddressForForeign? foreignAddress { set; get; }
        public AddressForIndia? indiaAddress { set; get; }
    }
    public class AddressForForeign
    {
        public string? address { set; get; }
        public string? mobile { set; get; }
        public string? email { set; get; }
        public string? emailOTP { set; get; }
        public string? signatureName { set; get; }
        public string? signatureSrc { set; get; }
    }

    public class AddressForIndia
    {
        public string? state { set; get; }
        public string? district { set; get; }
        public string? city { set; get; }
        public string? taluka { set; get; }
        public string? plotNo { set; get; }
        public string? building { set; get; }
        public string? mainRoad { set; get; }
        public string? impSymbol { set; get; }
        public string? area { set; get; }
        public string? pincode { set; get; }
        public string? postOfficeName { set; get; }
        public string? addressProofName { set; get; }
        public string? addressProofSrc { set; get; }
        public string? mobile { set; get; }
        public string? mobileOTP { set; get; }
        public string? signatureName { set; get; }
        public string? signatureSrc { set; get; }
    }

    public class EditPowerOfAttorneyInformationDataForGiver
    {
        public int? userid { get; set; }
        public string? applicationid { get; set; }
        public int power_of_attorney_id { get; set; }
        public UserDataForPOAGiver? userDetails { set; get; }
        public AddressData? address { set; get; }
    }

    public class DeletePowerOfAttorneyInformationDataForGiver
    {
        public string? applicationid { get; set; }
        public int power_of_attorney_id { get; set; }
    }
}
