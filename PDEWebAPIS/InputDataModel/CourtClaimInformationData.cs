namespace PDEWebAPIS.InputDataModel
{
    public class CourtClaimInformationData
    {
        public int? userid { get; set; }
        public string? applicationid { get; set; }
        public CaseDawaData? caseDawa { set; get; }
        public CaseType? caseType { set; get; }
        public string? lrPropertyUID { set; get; }
        public string? nabhu { set; get; }
        public string? orderDetails { set; get; }
        public string? stayOrder { set; get; }
        public string? subPropNo { set; get; }
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

    public class EditCourtClaimInformationData
    {
        public int? userid { get; set; }
        public string? applicationid { get; set; }
        public int court_claim_id {  set; get; }
        public CaseDawaData? caseDawa { set; get; }
        public CaseType? caseType { set; get; }
        public string? lrPropertyUID { set; get; }
        public string? nabhu { set; get; }
        public string? orderDetails { set; get; }
        public string? stayOrder { set; get; }
        public string? subPropNo { set; get; }
    }

    public class DeleteCourtClaimInformationData
    {
        public string? applicationid { get; set; }
        public int court_claim_id { set; get; }
    }
}