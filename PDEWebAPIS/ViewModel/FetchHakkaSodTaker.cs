using PDEWebAPIS.InputDataModel;

namespace PDEWebAPIS.ViewModel
{
    public class FetchHakkaSodTaker
    {
        public int? mutation_dtl_id { get; set; }
        public int? userid { get; set; }
        public string? applicationid { get; set; }
        public string? mutationSroNo { set; get; }
        public string? ownerNo { set; get; }
        public string? ctsNo { set; get; }
        public string? fullNameInMarathi { set; get; }
        public string? fullNameInEng { set; get; }
        public string? mobileNo { set; get; }
        public userDetailsHakkaSod? userDetails { get; set; }
        public AddressDTLForKharediNond? address { get; set; }
    }
}
