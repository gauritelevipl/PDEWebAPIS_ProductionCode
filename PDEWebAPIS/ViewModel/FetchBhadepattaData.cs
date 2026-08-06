using PDEWebAPIS.InputDataModel;

namespace PDEWebAPIS.ViewModel
{
    public class FetchBhadepattaData
    {
        public int? mutation_givertaker_id { get; set; }
        public int? mutation_dtl_id { get; set; }
        public int? userid { get; set; }
        public string? applicationid { get; set; }
        public string? cts_number { get; set; }
        public string? mutation_srno { get; set; }
        public string? owner_number { get; set; }
        public string? usertype { get; set; }
        public int? usertype_code { get; set; }

        public UserDetails? userDetails { get; set; }
        public AreaForMutation? areaForMutation { get; set; }
        public Address? address { get; set; }
        public dharakDetails? dharak { get; set; }//
        public string? fullNameInMarathi { set; get; }
        public string? fullNameInEng { set; get; }
        public string companyName { set; get; } = string.Empty;
        public string companyNameEng { set; get; } = string.Empty;
        public string? mobileNo { set; get; }
    }
}
