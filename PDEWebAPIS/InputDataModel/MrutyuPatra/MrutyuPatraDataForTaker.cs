using static PDEWebAPIS.InputDataModel.INameWithCodeFormatData;
using static PDEWebAPIS.InputDataModel.IUserData;

namespace PDEWebAPIS.InputDataModel.MrutyuPatra
{
    public class MrutyuPatraDataForTaker : IUserData, INameWithCodeFormatData
    {
        public int usertype_code {  get; set; }
        public string? usertype { get; set; }
        public string? applicationid { get; set; }
        public int userid { get; set; }
        public PhotoData? photo { get; set; }
        public isMHPropertyForMrutyuPatraTaker? isMHProperty { get; set; }
        public DharakDTLForMrutyuPatraTaker? dharak { get; set; }
        public PersonAddressData? address { get; set; }

    }
    public class isMHPropertyForMrutyuPatraTaker
    {
        public string? hasProperty { get; set; }
        public string? propType { get; set; }
        public MrutyuPatraTakeruserDetails? userDetails { get; set; }
    }
    public class MrutyuPatraTakeruserDetails
    {
        public string? suffix { get; set; }
        public string? suffixEng { get; set; }
        public string? firstName { get; set; }
        public string? middleName { get; set; }
        public string? lastName { get; set; }
        public string? firstNameEng { get; set; } = null;
        public string? middleNameEng { get; set; }
        public string? lastNameEng { get; set; }
        public string companyName { set; get; } = string.Empty;
        public string companyNameEng { set; get; } = string.Empty;
        public string? khataNo { get; set; }
        public string? naBhu { get; set; }
        public string? userName { get; set; }
        public string? ulpin { get; set; }
        public DistrictData? district { get; set; }
        public TalukaData? taluka { get; set; }
        public VillageData? village { get; set; }
    }

    public class DharakDTLForMrutyuPatraTaker
    {
        public UserdharakDetailsForMrutyuPatraTaker? userdharak { get; set; }
        public CompanydharakDetailsForMrutyuPatraTaker? companydharak { get; set; }
    }

    public class UserdharakDetailsForMrutyuPatraTaker
    {
        public string? aliceName { get; set; }
        public aapakDropdownData? aapakDropdown { get; set; }
        public aapakRelation? aapakRelation { get; set; }
        public Gender? gender { get; set; }
        public HoldertypeData? holderType { get; set; }
        public string? dob { get; set; }
        public string? motherName { get; set; }
        public string? motherNameEng { get; set; }
        public KhataTypeData? khataType { get; set; }
        //
        public string? landBuyArea { get; set; }
        //
        public string? aapak { set; get; }

        //public string? actualArea { get; set; }
        //public string? benefitArea { get; set; }
    }

    public class CompanydharakDetailsForMrutyuPatraTaker
    {
        public HoldertypeData? holderType { get; set; }
        public KhataTypeData? khataType { get; set; }
        public aapakDropdownData? aapakDropdown { get; set; }
        public string? aapak { get; set; }
        public string? benefitArea { get; set; }
    }

    public class EditMrutyuPatraDataForTaker : IUserData, INameWithCodeFormatData
    {
        public int? MutationId { get; set; }
        public int usertype_code { get; set; }
        public string? usertype { get; set; }
        public string? applicationid { get; set; }
        public int userid { get; set; }
        public PhotoData? photo { get; set; }
        public isMHPropertyForMrutyuPatraTaker? isMHProperty { get; set; }
        public DharakDTLForMrutyuPatraTaker? dharak { get; set; }
        public PersonAddressData? address { get; set; }

    }
}
