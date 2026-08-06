using PDEWebAPIS.InputDataModel;
using PDEWebAPIS.InputDataModel.MrutyuPatra;

namespace PDEWebAPIS.ViewModel
{
    public class FetchMrutyuPatraForGiverData
    {
        public int mutation_givertaker_id { get; set; }
        public int? mutation_dtl_id { get; set; }
        public int? userid { get; set; }
        public string? applicationid { get; set; }
        public string? village_code { get; set; }
        public string? ctsNo { get; set; }
        public string? mutationSroNo { get; set; }
        public string? ownerNo { get; set; }
        public string? fullNameInMarathi { get; set; }
        public string? fullNameInEng { get; set; }
        public UserDataForMrutyuPatraGiver? userDetails { set; get; }
        public AreaForMutationForMrutyuPatraGiver? areaForMutation { get; set; }
        public AddressDataForMrutyuPatraGiver? address { set; get; }
        public InputDataModel.docUpload? docUpload { set; get; }
    }
}
