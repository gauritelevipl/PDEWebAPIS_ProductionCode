namespace PDEWebAPIS.Model
{
    public class grievanceIssueTransactionDtlModel
    {
        public int Id { get; set; }
        public int transactionId { get; set; }
        public string? applicationId { get; set; }
        public string? tickitId { get; set; }
        public string? issueDescByUser { get; set; }
        public string? reply { get; set; }
        public string? replyUserid { get; set; }
        public DateTime datetime { get; set; }
    }
}