using PDEWebAPIS.CommonMethods;

namespace PDEWebAPIS.ViewModel
{
    public class FetchApplicationDataBySearch
    {
        public string? applicationId { get; set; }
        public string? mutationName { get; set; }
        public string? mutationTypeCode { get; set; }
        public string? district_code { get; set; }
        public string? district_name_in_marathi { get; set; }
        public string? taluka_code { get; set; }
        public string? taluka_name { get; set; }
        public string? mobileno { get; set; }
        public string? inwardNo { get; set; }
        public string? applicationStatus { get; set; }
        public string? UserName { get; set; }
        public DateTime? ApplicationCreatedDate { get; set; }
        public string? inwardNoError { get; set; }
    }

}
