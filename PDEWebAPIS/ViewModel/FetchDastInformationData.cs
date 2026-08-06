namespace PDEWebAPIS.ViewModel
{
    public class FetchDastInformationData
    {
        public int dast_id { get; set; }
        public int? userid { get; set; }
        public string? applicationid { get; set; }
        public string? dastNabhu { get; set; }
        public string? dastNo { set; get; }
        public string? dastNoDate { set; get; }
        public string? dastNoYear { set; get; }
        public string? dastType { get; set; }
        public string? divisionCode { get; set; }
        public string? divisionName { get; set; }
        public string? districtCode { set; get; }
        public string? districtName { set; get; }
       // public string? districtNameInEnglish { set; get; }
        public string? registrarCode { set; get; }
        public string? registrarName { set; get; }
        public string? remarks { set; get; }
        public bool? isDastVerified { get; set; }
        public string? verifiedDastData { get; set; }
    }
}
