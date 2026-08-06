using PDEWebAPIS.InputDataModel;

namespace PDEWebAPIS.ViewModel
{
    public class FetchVataniPatraNondDataForGiver
    {
        public int? mutation_dtl_id { get; set; }
        public int? userid { get; set; }
        public string? applicationid { get; set; }
        public string? ActualctsNo { get; set; }
        public string? mutationSroNo { get; set; }
        public string? ownerNo { get; set; }
        public UserDTLForVataniPatra? userDetails { get; set; }
        public areaForMutationDTLForVataniPatra? areaForMutation { get; set; }
        public AddressDTLForVataniPatra? address { get; set; }
        public string? fullNameInMarathi { set; get; }
        public string? fullNameInEng { set; get; }
        public string? mobileNo { set; get; }
    }
}
