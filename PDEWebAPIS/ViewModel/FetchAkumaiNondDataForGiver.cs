using PDEWebAPIS.InputDataModel;

namespace PDEWebAPIS.ViewModel
{
    public class FetchAkumaiNondDataForGiver
    {
        public int? mutation_dtl_id { get; set; }
        public int? userid { get; set; }
        public string? applicationid { get; set; }
        public string? ActualctsNo { get; set; }
        public string? mutationSroNo { get; set; }
        public string? ownerNo { get; set; }
        public UserDTLForAkumaiNond? userDetails { get; set; }
        public areaForMutationDTLAkumaiNond? areaForMutation { get; set; }
        public AddressDTLForAkumaiNond? address { get; set; }
        public string? fullNameInMarathi { set; get; }
        public string? fullNameInEng { set; get; }
        public string? mobileNo { set; get; }
    }
}
