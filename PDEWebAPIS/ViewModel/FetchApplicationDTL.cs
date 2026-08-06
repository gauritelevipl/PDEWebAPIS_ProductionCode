namespace PDEWebAPIS.ViewModel
{
    public class FetchApplicationDTL
    {
        public string? applicationId { get; set; }
        public string? district_code { set; get; }
        public string? district_name_in_marathi { set; get; }
        public string? district_name_in_english { set; get; }
        public string? taluka_code { set; get; }
        public string? taluka_name { set; get; }
        public string? application_type_code { set; get; }
        public string? application_type_in_marathi { set; get; }
        public string? application_type_in_english { set; get; }
        public string? mutation_type_code { set; get; }
        public string? mutation_type { set; get; }
        public string? village_code { set; get; }
        public string? village_name { set; get; }
        public List<NabhuDTLFetchApplicationDTL>? nabhDTL { set; get; }
    }

    public class NabhuDTLFetchApplicationDTL
    {
        public string? naBhu { set; get; }
        public string? sub_property_no { get; set; }
        public string? actual_cts_no { set; get; }
        public string? milkat { set; get; }
        public string? lrPropertyUID { set; get; }
        public string? cityServeyAreaInSqm { set; get; }
        public string? namud { set; get; }
    }
}
