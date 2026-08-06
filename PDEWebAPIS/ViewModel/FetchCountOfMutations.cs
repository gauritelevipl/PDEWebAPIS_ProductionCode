namespace PDEWebAPIS.ViewModel
{
    public class FetchCountOfMutations
    {
        public string? MutationName { get; set; }
        public List<StatusDetail>? Statuses { get; set; }
        public int CountOfMutation { get; internal set; }
    }
    public class StatusDetail
    {
        public int ApplicationStatusCode { get; set; }
        public string? ApplicationStatus { get; set; }
        public int CountOfMutation { get; set; }
    }
}
