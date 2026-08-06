namespace PDEWebAPIS.InputDataModel
{
    public class NameChangeData
    {
        public string? applicationid { get; set; }
        public string? village_code { get; set; }
        public int userid { get; set; }
        public UserDetailsForNameChange? userDetails { get; set; }
        public NameChangeDetails? nameChange { get; set; }
        public List<SelectedUserDTLsForNameChange>? selectedUserDetails { get; set; }
        public UpdatedUserDetailsForNameChange? updatedUserDetails { get; set; }
        public AddressDTLForNameChange? address { get; set; }
    }

    public class UserDetailsForNameChange
    {
        public string? subPropNo { get; set; }
        public string? nabhu { get; set; }
        public string? lrPropertyUID { get; set; }
        public string? milkat { get; set; }
        public string? namud { get; set; }
    }
    public class NameChangeDetails
    {
        public ReasonDataForNameChange? reason { get; set; }
        public string? no { get; set; }
        public string? date { get; set; }
    }
    public class ReasonDataForNameChange
    {
        public int name_change_by_code { get; set; }
        public string? name_change_by_description { get; set; }
    }

    public class SelectedUserDTLsForNameChange
    {
        public string? village_code { get; set; }
        public string? cts_number { get; set; }
        public string? mutation_srno { get; set; }
        public string? entry_date { get; set; }
        public string? entry_bracketed { get; set; }
        public string? owner_number { get; set; }
        public string? owner_name { get; set; }
        public string? first_name { get; set; }
        public string? middle_name { get; set; }
        public string? last_name { get; set; }
        public string? nick_name { get; set; }
        public string? owner_bracketed { get; set; }
        public string? area_bracketed { get; set; }
        public string? email_id { get; set; }
        public string? owner_cell_number { get; set; }
        public string? pincode { get; set; }
        public string? owner_type { get; set; }
        public string? apk_code { get; set; }
        public string? apk_name { get; set; }
        public string? flat_or_house_number { get; set; }
        public string? building_number { get; set; }
        public string? road { get; set; }
        public string? city_or_village { get; set; }
        public string? taluka_name { get; set; }
        public string? district_name { get; set; }
        public string? state_name { get; set; }
        public string? gender_code { get; set; }
        public string? date_of_birth { get; set; }
        public string? owner_area { get; set; }
        public string? owner_area_bracketed { get; set; }
    }

    public class UpdatedUserDetailsForNameChange
    {
        public int userType { get; set; }
        public string? userTypeLabel { get; set; }
        public NameDetailsForNameChange? details { get; set; }
    }

    public class NameDetailsForNameChange
    {
        public string? suffix { get; set; }
        public string? suffixEng { get; set; }
        public string? suffixcode { get; set; }
        public string? suffixCodeEng { get; set; }
        public string? firstName { get; set; }
        public string? middleName { get; set; }
        public string? lastName { get; set; }
        public string? firstNameEng { get; set; }
        public string? middleNameEng { get; set; }
        public string? lastNameEng { get; set; }
        public string? companyName { set; get; }
        public string? companyNameEng { set; get; }
    }
    public class AddressDTLForNameChange
    {
        public string? addressType { set; get; }
        public IndiaAddressForNameChange? indiaAddress { get; set; }
        public AddressForForeignForNameChange? foreignAddress { set; get; }
    }

    public class IndiaAddressForNameChange
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

    public class AddressForForeignForNameChange
    {
        public string? address { set; get; }
        public string? mobile { set; get; }
        public string? email { set; get; }
        public string? emailOTP { set; get; }
        public string? signatureName { set; get; }
        public string? signatureSrc { set; get; }
    }
}
