using PDEWebAPIS.InputDataModel;

namespace PDEWebAPIS.ViewModel
{
    public class FetchHibanamaWitnessInfoData
    {
        public int witness_info_id { get; set; }
        public string? applicationid { get; set; }
        public string? permissionNo { get; set; }
        public string? permissionDate { get; set; }
        public int userid { get; set; }
        public HibanamaWitnessDetails? witnessDetails { get; set; }
        public AddressForHibanamaWitness? address { get; set; }
        public string? fullNameInMarathi { get; set; }
        public string? fullNameInEng { get; set; }
    }
}
