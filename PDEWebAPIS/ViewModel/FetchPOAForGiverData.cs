using PDEWebAPIS.InputDataModel;
using static PDEWebAPIS.InputDataModel.INameWithCodeFormatData;

namespace PDEWebAPIS.ViewModel
{
    public class FetchPOAForGiverData
    {
        public int? power_of_attorney_id { get; set; }
        public int? userid { get; set; }
        public string? applicationid { get; set; }
        public string? ctsNo { get; set; }
        public string? mutationSroNo { get; set; }
        public string? ownerNo { get; set; }
        public string? village_code { get; set; }
        public string? village_name { set; get; }
        public UserDetailsDataForPOAGiver? userDetails { set; get; }
        public AddressDataForPOAGiver? address { set; get; }
        public string? fullNameInMarathi { set; get; }
        public string? fullNameInEng {  set; get; }
        public string? mobileNo { set; get; }
        public string? usertype { set; get; }
        public int user_type_code {  set; get; }
        // 0 -> Giver , 1 -> Taker
        public bool is_taker { get; set; }
        public AttroneyTypeForPOATaker? attornytype { set; get; }
        public Division? division { set; get; }
        public DastDistrict? district { set; get; }
        public Dastregsiter? registrar { set; get; }
        public string? dastNo { set; get; }
        public string? dastNoDate { set; get; }
        public string? dastNoYear { set; get; }
        public string? isPOAisPartofDast { set; get; }
        public string? isDeclerationInvolvedInPOA { set; get; }
        public string? isPOAPermanant { set; get; }
        public string? isTransferRights { set; get; }
        public string? passportName { set; get; }
        public string? passportSrc {  set; get; }
    }

    public class UserDetailsDataForPOAGiver
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
        public string? nabhu { set; get; }
        public string? userName { set; get; }
        public string? lrPropertyUID { set; get; }
        public string? subPropNo { get; set; }
        public string? company_name_in_eng { set; get; }
        public string? company_name_in_marathi {  set; get; }
    }

    public class AddressDataForPOAGiver
    {
        public string? addressType { set; get; }
        public AddressForForeignForPOAGiver? foreignAddress { set; get; }
        public AddressForIndiaForPOAGiver? indiaAddress { set; get; }
    }

    public class AddressForForeignForPOAGiver
    {
        public string? address { set; get; }
        public string? mobile { set; get; }
        public string? email { set; get; }
        public string? emailOTP { set; get; }
        public string? signatureName { set; get; }
        public string? signatureSrc { set; get; }
    }

    public class AddressForIndiaForPOAGiver
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
}
