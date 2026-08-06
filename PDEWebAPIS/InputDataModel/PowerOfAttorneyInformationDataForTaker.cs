using static PDEWebAPIS.InputDataModel.IUserData;
using static PDEWebAPIS.InputDataModel.INameWithCodeFormatData;
namespace PDEWebAPIS.InputDataModel
{
    public class PowerOfAttorneyInformationDataForTaker : IUserData, INameWithCodeFormatData
    {
        public List<GiverTakerInfoData>? selectedOptions { get; set; }
        public int? userid { get; set; }
        public string? applicationid { get; set; }
        public string? usertype { set; get; }
        public int usertype_code { set; get; }
        public string? mutation_id {  set; get; }
        public PhotoData? photo { set; get; }
        public IsMHPropertyDataForPOATaker? isMHProperty { set; get; }
        public Dharak? dharak { set; get; }
        public PersonAddressData? address { set; get; }
        public AttroneyTypeForPOATaker? attornytype { set; get; }
        public Division? division { set; get; } 
        public DastDistrict? district { set; get; }
        public Dastregsiter? registrar { set; get; }
       // public RegistrarForPOATaker? registrar { set; get; }
        public string? dastNo { set; get; }
        public string? dastNoDate { set; get; }
        public string? dastNoYear { set; get; }
        public bool isDastVarified { set; get; }
        public string? verifiedDastData { set; get; }   
        public string? isPOAisPartofDast { set; get; }
        public string? isDeclerationInvolvedInPOA { set; get; }
        public string? isPOAPermanant { set; get; }
        public string? isTransferRights { set; get; }
    }

    public class IsMHPropertyDataForPOATaker
    {
        public string? hasProperty { set; get; }
        public string? propType { set; get; }
        public UserDetailsDataForPOATaker? userDetails { set; get; }
    }

    public class UserDetailsDataForPOATaker
    {
        public string? khataNo { set; get; }
        public string? naBhu { set; get; }
        public string? ulpin { set; get; }
        public string? userName { set; get; }
        public DistrictForPOATaker? district { get; set; }
        public TalukaForPOATaker? taluka { get; set; }
        public VillageForPOATaker? village { get; set; }
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
        public string? companyName { set; get; }
        public string? companyNameEng { set; get; }
    }

    public class DistrictForPOATaker
    {
        public string? district_code { get; set; }
        public string? district_name { get; set; }
        public string? district_english_name { get; set; }
    }
    public class TalukaForPOATaker
    {
        public string? office_code { get; set; }
        public string? office_name { get; set; }
    }
    public class VillageForPOATaker
    {
        public string? village_code { get; set; }
        public string? village_name { get; set; }
    }

    public class Dharak
    {
        public UserDharak? userdharak { get; set; }
        public CompanyDharak? companydharak { get; set; }
    }

    public class UserDharak
    {
        public string? aliceName { set; get; }
        public Gender? gender { set; get; }
        //public KhataTypeData? khataType { get; set; }
        //public HoldertypeData? holderType { get; set; }
        public string? dob { set; get; }
        public string? motherName { set; get; }
        public string? motherNameEng { set; get; }
    }

    public class CompanyDharak
    {
        public KhataTypeData? khataType { get; set; }
        public HoldertypeData? holderType { get; set; }
        public aapakDropdownData? aapakDropdown { get; set; }
        public string? aapak { set; get; }
        public string? landBuyArea { set; get; }
    }

    public class AttroneyTypeForPOATaker
    {
        public int poa_type_code { set; get; }
        public string? poa_type_description { set; get; }
    }

    public class EditPowerOfAttorneyInformationDataForTaker : IUserData, INameWithCodeFormatData
    {
        public int? userid { get; set; }
        public string? applicationid { get; set; }
        public int power_of_attorney_id {  set; get; }
        public int usertype_code { set; get; }
        public string? usertype { set; get; }
        public PhotoData? photo { set; get; }
        public IsMHPropertyDataForPOATaker? isMHProperty { set; get; }
        public Dharak? dharak { set; get; }
        public PersonAddressData? address { set; get; }
        public AttroneyTypeForPOATaker? attornytype { set; get; }
        public Division? division { set; get; }
        public DastDistrict? district { set; get; }
        public Dastregsiter? registrar { set; get; }
        public string? dastNo { set; get; }
        public string? dastNoDate { set; get; }
        public string? dastNoYear { set; get; }
        public bool? isDastVerified { set; get; }
        public string? verifiedDastData { set; get; }
        public string? isPOAisPartofDast { set; get; }
        public string? isDeclerationInvolvedInPOA { set; get; }
        public string? isPOAPermanant { set; get; }
        public string? isTransferRights { set; get; }
    }

    public class DeletePowerOfAttorneyInformationDataForTaker
    {
        public string? applicationid { get; set; }
        public int power_of_attorney_id { set; get; }
    }

    public class GiverTakerInfoData
    {
        public int code { set; get; }
        public string? name { set; get; }
    }
}