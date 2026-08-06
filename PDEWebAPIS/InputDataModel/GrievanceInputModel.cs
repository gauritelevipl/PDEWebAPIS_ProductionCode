namespace PDEWebAPIS.InputDataModel
{
    public class GrievanceInputModel
    {
        public int userId { get; set; }
        public string? applicationId { get; set; }
        public string? mutationName { get; set; }
        public string? issueCategory { get; set; }
        public string? issueDesc { get; set; }
        public string? ImageName { get; set; }
        public string? imagesrc { get; set; }
            
        public string? district_code { get; set; }
        public string? district_name_in_marathi { get; set; }
        public string? taluka_code { get; set; }
        public string? taluka_name { get; set; }
        public string? secondaryMoNo { get; set; }
    }
}
