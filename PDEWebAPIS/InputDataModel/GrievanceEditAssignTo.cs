namespace PDEWebAPIS.InputDataModel
{
    public class GrievanceEditAssignTo
    {
        public string? tickitId { get; set; }

        public string? grievanceStatus { get; set; }
        public int AssignIssueToUserId { get; set; }
        public string? priority { get; set; }
    }
}
