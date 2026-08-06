using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace PDEWebAPIS.Model
{
    public class GrievanceUserMasterModel
    {
        public string? usertype { get; set; }
        public string? fullname { get; set; }
        public string? username { get; set; }

        //new cols for officers login
        public string? district_code { get; set; }
        public string? district_english_name { get; set; }
        public string? district_name { get; set; }

        public string? region_code { get; set; }
        public string? region_english_name { get; set; }
        public string? region_name { get; set; }
        //

        public string? division { get; set; }
        public string? mobileno { get; set; }
        public string? emailid { get; set; }
        public string? password { get; set; }
        //public string? confirmpassword { get; set; }
        public DateTime registerdatetime { get; set; }
        public string? webtoken { get; set; }
        public string? moiletoken { get; set; }
    }
}
