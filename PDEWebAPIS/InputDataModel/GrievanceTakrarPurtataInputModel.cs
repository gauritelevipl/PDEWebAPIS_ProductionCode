namespace PDEWebAPIS.InputDataModel
{
    public class GrievanceTakrarPurtataInputModel
    {
        public string? applicaitonId { get; set; }
        public string? tickitId { get; set; }
        public string? actualUserIssue { get; set; }
        public string? userCompliance { get; set; }
        public string? adminComplience { get; set; }
        public string? grievanceStatus { get; set; }
        public string? takrarPurtataStatusInYOrN { get; set; }
        public string? isTakrarPurtataDone { get; set; }

        //assign dtls
        public int AssignIssueToUserId { get; set; }
        public string? priority { get; set; }
    }
}
