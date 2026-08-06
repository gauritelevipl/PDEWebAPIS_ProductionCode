using PDEWebAPIS.InputDataModel;
using static PDEWebAPIS.InputDataModel.INameWithCodeFormatData;

namespace PDEWebAPIS.ViewModel
{
    public class FetchPOAForTakerData : INameWithCodeFormatData
    {
        //public List<FetchGiverData>? poaGiverData { get; set; }
        public string? giver_name_in_marathi {  get; set; }
        public string? giver_name_in_english {  get; set; }
        public int? power_of_attorney_id { get; set; }
        public int? userid { get; set; }
        public string? applicationid { get; set; }
        public UserDetailsDataForPOATaker? userDetails { set; get; }
        public AddressDataForPOATaker? address { set; get; }
        public string? fullNameInMarathi { set; get; }
        public string? fullNameInEng { set; get; }
        public string? mobileNo { set; get; }
        public string? usertype { set; get; }
        public int usertype_code { set; get; }
        // 0 -> Giver , 1 -> Taker
        public bool is_taker { get; set; }
        public DharakForPOATaker? dharak { set; get; }
        public AttroneyTypeForPOATaker? attornytype { set; get; }
        public Division? division { set; get; }
        public DastDistrict? district { set; get; }
        public Dastregsiter? registrar { set; get; }
        public string? dastNo { set; get; }
        public string? dastNoDate { set; get; }
        public string? dastNoYear { set; get; }
        public bool? isDastVarified { get; set; }
        public string? varifiedDastData { set; get; }
        public string? isPOAisPartofDast { set; get; }
        public string? isDeclerationInvolvedInPOA { set; get; }
        public string? isPOAPermanant { set; get; }
        public string? isTransferRights { set; get; }
        public string? passportName { set; get; }
        public string? passportSrc { set; get; }
        public string? poa_giver_ids { set;get; }
    }
    public class UserDetailsDataForPOATaker
    {
        public string? suffixcode {  set; get; }
        public string? suffix { set; get; }
        public string? firstName { set; get; }
        public string? middleName { set; get; }
        public string? lastName { set; get; }
        public string? suffixCodeEng {  set; get; }
        public string? suffixEng { set; get; }
        public string? firstNameEng { set; get; }
        public string? middleNameEng { set; get; }
        public string? lastNameEng { set; get; }
        public string? nabhu { set; get; }
        public string? userName { set; get; }
        public string? lrPropertyUID { set; get; }
        public DistrictForPOATaker? district { get; set; }
        public TalukaForPOATaker? taluka { get; set; }
        public VillageForPOATaker? village { get; set; }

    }
    public class AddressDataForPOATaker
    {
        public string? addressType { set; get; }
        public AddressForForeignForPOATaker? foreignAddress { set; get; }
        public AddressForIndiaForPOATaker? indiaAddress { set; get; }
    }
    public class AddressForForeignForPOATaker
    {
        public string? address { set; get; }
        public string? mobile { set; get; }
        public string? email { set; get; }
        public string? emailOTP { set; get; }
        public string? signatureName { set; get; }
        public string? signatureSrc { set; get; }
    }

    public class AddressForIndiaForPOATaker
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

    public class DharakForPOATaker
    {
        public UserDharakForPOATaker? userdharak { get; set; }
        public CompanyDharakForPOATaker? companydharak { get; set; }
    }

    public class UserDharakForPOATaker
    {
        public string? aliceName { set; get; }
        public Gender? gender { set; get; }
     /*   public KhataTypeData? khataType { set; get; }
        public HoldertypeData? holderType { set; get; }*/
        public string? dob { set; get; }
        public string? motherName { set; get; }
        public string? motherNameEng { set; get; }
    }

    public class CompanyDharakForPOATaker
    {
    /*    public KhataTypeData? khataType { set; get; }
        public HoldertypeData? holderType { set; get; }
        public aapakDropdownData? aapakDropdown { set; get; }
        public string? aapak { set; get; }*/
        public string? landBuyArea { set; get; }
    }

    //public class FetchFinalPOATakerData
    //{
    //    public List<FetchPOAForTakerData>? poaTakersData { get; set; }
    
    //}
}
