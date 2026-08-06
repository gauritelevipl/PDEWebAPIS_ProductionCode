namespace PDEWebAPIS.ViewModel
{
    public class FetchCourtClaimInformationData
    {
        public int court_claim_id { get; set; }
        public int? userid { get; set; }
        public string? applicationid { get; set; }
        public CaseDawaData? caseDawa { set; get; }
        public CaseType? caseType { set; get; }
        public string? lrPropertyUID { set; get; }
        public string? nabhu { set; get; }
        public string? orderDetails { set; get; }
        public string? stayOrder { set; get; }
        public string? subPropNo { get; set; }
    }
    public class CaseDawaData
    {
        public string? caseNoCode { set; get; }
        public string? caseNolabel { set; get; }
    }
    public class CaseType
    {
        public string? caseTypeCode { set; get; }
        public string? caseTypeLabel { set; get; }
    }
}
