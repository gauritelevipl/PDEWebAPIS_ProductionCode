using PDEWebAPIS.InputDataModel;
using PDEWebAPIS.InputDataModel.MrutyuPatra;
using static PDEWebAPIS.InputDataModel.IUserData;

namespace PDEWebAPIS.ViewModel
{
    public class FetchMrutyuPatraForTakerData: IUserData, INameWithCodeFormatData
    {
        public int? mutation_dtl_id { get; set; }
        public int? userid { get; set; }
        public string? applicationid { get; set; }
        public int usertype_code { set; get; }
        public string? userType { get; set; }
        public string? fullNameInMarathi { set; get; }
        public string? fullNameInEng { set; get; }
        public string? mobileNo { set; get; }
        public PhotoData? photo { get; set; }
        public isMHPropertyForMrutyuPatraTaker? isMHProperty { get; set; }
        public DharakDTLForMrutyuPatraTaker? dharak { get; set; }
        public PersonAddressData? address { get; set; }
    }
}
