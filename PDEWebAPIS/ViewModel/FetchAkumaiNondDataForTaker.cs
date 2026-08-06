using PDEWebAPIS.InputDataModel;

namespace PDEWebAPIS.ViewModel
{
    public class FetchAkumaiNondDataForTaker
    {
        public int? mutation_dtl_id { get; set; }
        public int? userid { get; set; }
        public int usertype_code { set; get; }
        public string? usertype { get; set; }
        public string? applicationid { get; set; }
        public string? fullNameInMarathi { set; get; }
        public string? fullNameInEng { set; get; }
        public string? mobileNo { set; get; }
        public photoDetailsForAkumaiTaker? photo { get; set; }
        public isMHPropertyAkumaiTaker? IsMHProperty { get; set; }
        public DharakAkumaiTaker? dharak { get; set; }
        public AddressDTLAkumaiTaker? address { get; set; }
    }
}
