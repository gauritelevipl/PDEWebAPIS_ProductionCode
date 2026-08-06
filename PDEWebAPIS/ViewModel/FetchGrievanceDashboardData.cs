using System;

namespace PDEWebAPIS.ViewModel
{
    public class FetchGrievanceDashboardData
    {
        public string? applicationId { get; set; }
        public string? mutationName { get; set; }
        public string? issueCategory { get; set; }
        public string? grivanceStatus { get; set; }
        public int grivanceStatusCode { get; set; }
        public string? SeenStatus { get; set; }
        public string? seenby { get; set; }
        public int seenStatusCode { get; set; }
        public DateTime issueReportDate { get; set; }
        public string? tickitId { get; set; }
        public string? AssignIssueTo { get; set; }
        public string? priority { get; set; }
        public string? district_code { get; set; }
        public string? district_name_in_marathi { get; set; }
        public string? taluka_code { get; set; }
        public string? taluka_name { get; set; }
        public string? docpath { get; set; }
        public string? docname { get; set; }
        public string? issueDescription { get; set; }
        public string? mobileno { get; set; }
        public string? secondaryMoNo { get; set; }

        //public int

        public List<GrievanceReplyData>? grievanceReplyData { get; set; }
    }
    public class GrievanceReplyData
    {
        public string? reply { get; set; }
        public string? replyFrom { get; set; }
        //public DateTime dateTime { get; set; }
        public DateTime time { get; set; }
    }
}

