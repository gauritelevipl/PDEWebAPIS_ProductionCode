using PDEWebAPIS.InputDataModel.GahankhatNond;

namespace PDEWebAPIS.ViewModel
{
    public class FetchGahankhatDataForGiver
    {
        public int mutation_givertaker_id { get; set; }
        public int? userid { get; set; }
        public string? applicationid { get; set; }
        public string? fullNameInMarathi { set; get; }
        public string? fullNameInEng { set; get; }
        public UserDetailsForGahankhatGiver? userDetails { set; get; }
        public AddressDTLForGahankhatGiver? address { set; get; }
    }
}
