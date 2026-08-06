using PDEWebAPIS.InputDataModel;

namespace PDEWebAPIS.ViewModel
{
    public class FetchErrorCorrectionData
    {
        public int error_correction_id { get; set; }
        public string? applicationid { get; set; }
        public int userid { get; set; }
        public string? village_code { get; set; }
        public UserDetailsForErrorCorrection? userDetails { get; set; }
        public AddressDTLForErrorCorrection? address { get; set; }
    }
}
