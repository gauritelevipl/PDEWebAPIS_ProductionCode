namespace PDEWebAPIS.Model
{
    
    public class LGDDistrict
    {
        public string? districtcode { get; set; }
        public string? districtnameenglish { get; set; }
        public string? districtlocalname { get; set; }
        public string? lbtype { get; set; }
    }

    public class LGDTaluka
    {
        public string? subdistrictcode { get; set; }
        public string? subdistrictnameenglish { get; set; }
        public string? subdistrictlocalname { get; set; }
        public string? lbtype { get; set; }
    }
    public class RequestVillage
    {
        public string? distcode { get; set; }
        public string? talukacode {get; set;}
    }
    public class LGDVillage
    {
        public string? districtcode { get; set; }
        public string? subdistrictcode { get; set; }
        public string? villagecode { get; set; }
        public string? villagenameenglish { get; set; }
        public string? villagelocalname { get; set; }
        public string? lbtype { get; set; }
    }
    public class LGDServeyNoList
    {
        public string? village_code { get; set; }
        public string? survey_no_list { get; set; }
    }
}
