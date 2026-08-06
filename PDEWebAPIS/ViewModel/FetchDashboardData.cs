namespace PDEWebAPIS.ViewModel
{
    public class FetchDashboardData
    {
        public string? applicationID { set; get; }
        public Status? status { set; get; }
        public string? docName { set; get; }
        public string? docSrc { set; get; }
        public string? application_date { set; get; }
        public string? mutation_type_code { set; get; }
        public string? mutation_type {  set; get; }
        public string? district_name_in_marathi {  set; get; }
        public string? district_name_in_english { set; get; }
        public string? nabhunos {  set; get; }
        public string? taluka { set; get; }
        public string? village {  set; get; }   
        public string? uploaded_docName {  set; get; }
        public string? isCourtDawa {  set; get; }
        public string? isDastApplicable { set; get; }
        public string? isMainPatra { set; get; }
        public bool applicationsubmitted { set; get; }
        public string? inwardno { set;get; }
        public string? slashinwardno { set; get; }
        public List<NICDocdata>? nicDocData { set; get; }
        public string? inwardno_status { set; get; }
        public string? application_type { set; get; }
    }

    public class Status
    {
        public string? code { set; get; } 
        public string? label { set; get; } 
    }
    public class NICDocdata
    {
        public string? docName { set; get; }
        public string? docSrc { set; get; }
    }
}
