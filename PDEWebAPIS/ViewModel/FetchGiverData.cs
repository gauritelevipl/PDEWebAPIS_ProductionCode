namespace PDEWebAPIS.ViewModel
{
    public class FetchGiverData
    {
        public List<string>? giver_names_in_marathi { set; get; }
        public List<string>? giver_names_in_english { set; get; }
    }
    public class FetchGiverDataForPOA
    {
        public string? giver_names_in_marathi { set; get; }
        public string? giver_names_in_english { set; get; }
        public string? code {  set; get; }
    }
}
