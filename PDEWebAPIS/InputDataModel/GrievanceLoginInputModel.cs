namespace PDEWebAPIS.InputDataModel
{
    public class GrievanceLoginInputModel
    {
        public string? username { get; set; }
        public string? password { get; set; }
        public int loginType { get; set; }
        public string? district_code { get; set; }
        public string? region_code { get; set; }

        //public string? usertype { get; set; }
    }
}
