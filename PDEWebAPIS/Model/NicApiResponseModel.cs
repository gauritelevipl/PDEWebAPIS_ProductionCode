namespace PDEWebAPIS.Model
{
    public class NicApiResponseModel
    {
        public string? statuscode { get; set; }
        public string? applicationid { get; set; }
        public string? inwardno { get; set; }
        public string? response { get; set; }
        public int apicallcount { get; set; }
        public bool inwardno_generated { get; set; }
    }
}
