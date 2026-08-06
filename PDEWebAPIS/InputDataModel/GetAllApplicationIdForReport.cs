namespace PDEWebAPIS.InputDataModel
{
    public class GetAllApplicationIdForReport
    {
        public int pageno { get; set; }
        public int pagesize { get; set; }
        public DateTime? fromDate { get; set; }
        public DateTime? toDate { get; set; }
        //
        public string? region_code { get; set; }
        public string? district_code { get; set; }
        public string? office_code { get; set; }

        public int? statusId { get; set; }
    }
    public class FetchDataForVerticalChart
    {
        public string? category { get; set; }
        public int[]? data { get; set; }
    }
}
