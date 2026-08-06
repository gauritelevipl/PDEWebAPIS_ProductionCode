namespace PDEWebAPIS.Model
{
    public class BhadepattaInfoModel
    {
        public int userid { get; set; }
        //public string? village_code { get; set; }
        //public string? ctsNo { get; set; }
        //public string? mutationSroNo { get; set; }
        //public string? ownerNo { get; set; }
        public string? applicationid { get; set; }
        public string? bhadepattaTenureYear { get; set; }
        public string? bhadepattaTenureMonth { get; set; }
        public string? bhadepattaFromDate { get; set; }
        public string? bhadepattaToDate { get; set; }
        public string? bhadepattaAmount { get; set; }
        public DateTime createdDateTime { get; set; }
        public bool? isDeleted { get; set; }
        public DateTime deletedDateTime { get; set; }
        public string? leaseperiod { get; set; }
    }
}
