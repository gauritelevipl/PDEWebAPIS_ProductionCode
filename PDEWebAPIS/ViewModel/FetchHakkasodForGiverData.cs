using PDEWebAPIS.InputDataModel;

namespace PDEWebAPIS.ViewModel
{
    public class FetchHakkasodForGiverData
    {
        public int mutation_givertaker_id { get; set; }
        public int? userid { get; set; }
        public string? applicationid { get; set; }
        public string? ActualctsNo { get; set; }
        public string? mutationSroNo { set; get; }
        public string? ownerNo { set; get; }
        public string? fullNameInMarathi { get; set; }
        public string? fullNameInEng { get; set; }
        public UserDetailsForHakkasodGiver? userDetails { get; set; }
        public AreaForMutationForHakkasodGiver? areaForMutation { get; set; }
        public AddressDTLForHakkasodGiver? address { get; set; }
    }
}
