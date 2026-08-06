using PDEWebAPIS.InputDataModel.GahankhatNond;

namespace PDEWebAPIS.ViewModel
{
    public class FetchGahankhatForTakerData
    {
        public int mutation_givertaker_id { get; set; }
        public int? userid { get; set; }
        public string? applicationid { get; set; }
        public string? ActualctsNo { get; set; }
        public string? mutationSroNo { set; get; }
        public string? ownerNo { set; get; }
        public string? fullNameInMarathi { get; set; }
        public string? fullNameInEng { get; set; }
        public UserDetailsForGahankhatTaker? userDetails { get; set; }
        public AreaForMutationForGahankhatTaker? areaForMutation { get; set; }
        public AddressDTLForGahankhatTaker? address { get; set; }
    }
}
