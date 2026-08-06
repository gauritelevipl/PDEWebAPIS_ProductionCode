namespace PDEWebAPIS.Model
{
    public class GrievanceModel
    {

        //public int Gid { get; set; }
        public int userId { get; set; }
        public string? applicationId { get; set; }

        public string? mutationName { get; set; }

        public int mutationCode { get; set; }
        public string? secondaryMoNo { get; set; }
        public string? mobileno { get; set; }
        public string? district_code { get; set; }
        public string? district_name_in_marathi { get; set; }
        public string? taluka_code { get; set; }
        public string? taluka_name { get; set; }

        public string? issueCategory { get; set; }

        public string? grivanceStatus { get; set; }

        public DateTime issueReportDate { get; set; }

        public string? issueDesc { get; set; }

        public string? docPath { get; set; }

        public string? ImageName { get; set; }

        public string? tickitId { get; set; }

        public string? AssignIssueTo { get; set; }
        public int ReAssignIssueTo { get; set; }
        public string? IssueClosedBy { get; set; }
        public int AssignIssueToUserId { get; set; }

        public string? priority { get; set; }

        public string? IsDeleted { get; set; }

        public DateTime IssueSeenDateTime { get; set; }
        public DateTime IssueAssignDateTime { get; set; }
        public DateTime IssueResolvedDateTime { get; set; }
        public DateTime IssueReassignedDateTime { get; set; }

        public string? takrarPurtataStatusInYOrN { get; set; }
    }
}
