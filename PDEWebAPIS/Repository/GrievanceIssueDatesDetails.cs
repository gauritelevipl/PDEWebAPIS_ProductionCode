using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PDEWebAPIS.Repository
{
    [Table("grievanceIssueDatesDetails")]
    public class GrievanceIssueDatesDetails
    {
        [Key]
        public int Id { get; set; }
        //public GrievanceTbl? grievancetbl { get; set; }
        public string? tickitid { get; set; }
        public string? applicationId { get; set; }
        public string? PerformByUserId { get; set; }
        public string? IssueAssignToUserId { get; set; }
        public string? IssueReassignToUserId { get; set; }
        public string? IssueSeenByUserId { get; set; }
        public string? IssueResolvedByUserid { get; set; }
        //public DateTime IssueSeenDateTime { get; set; }
        //public DateTime IssueAssignDateTime { get; set; }
        //public DateTime IssueResolvedDateTime { get; set; }
        //public DateTime IssueReassignedDateTime { get; set; }

        //public string? IssueSeenbyUserId { get; set; }//n
        //public string? IssueResolvedbyUserId { get; set; }//n
        //public string? IssueReassignedtoUserId { get; set; }//n
        public DateTime StatusDatetime { get; set; }
        public DateTime Datetime { get; set; }
        public string? status { get; set; }
    }
}
