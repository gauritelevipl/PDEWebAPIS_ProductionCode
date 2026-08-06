namespace PDEWebAPIS.ViewModel
{
    public class DashboardMetricsData
    {
        public List<FetchEPCISgetDashboardMetrics>? ePCISgetDashboardMetricsResponse { get; set; }
    }
    public class FetchEPCISgetDashboardMetrics
    {
        public string? state_name { get; set; }
        public string? divisioncode { get; set; }
        public string? divisionname { get; set; }
        public string? districtcode { get; set; }
        public string? districtname { get; set; }
        public string? officecode { get; set; }
        public string? officename { get; set; }
        public string? total_inward { get; set; }
        public string? pending_at_ms_inward { get; set; }
        public string? pending_at_ctso_inward { get; set; }
        public string? disposed_inward { get; set; }
        public string? totalCreatedApplicationCount { get; set; }
    }
}
