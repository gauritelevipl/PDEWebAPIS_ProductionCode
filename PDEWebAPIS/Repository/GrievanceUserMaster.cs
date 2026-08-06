using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PDEWebAPIS.Repository
{
    [Table("GrievanceUserMaster")]
    public class GrievanceUserMaster
    {
        [Key, Required]
        public int guserid { get; set; }

        public string? usertype { get; set; } = string.Empty;

        public string? fullname { get; set; } = string.Empty;

        public string? username { get; set; } = string.Empty;

        //new cols for officers login
        public string? district_code { get; set; } = string.Empty;
        public string? district_english_name { get; set; } = string.Empty;
        public string? district_name { get; set; } = string.Empty;


        public string? region_code { get; set; } = string.Empty;
        public string? region_english_name { get; set; } = string.Empty;
        public string? region_name { get; set; } = string.Empty;
        //
        public string? division { get; set; } = string.Empty;
        [Required, MaxLength(10)]
        public string? mobileno { get; set; } = string.Empty;

        public string? emailid { get; set; } = string.Empty;
        public string? password { get; set; } = string.Empty;
        //[Required]
        //public string? confirmpassword { get; set; } = string.Empty;

        public DateTime registerdatetime { get; set; }

        [DefaultValue("NA")]
        public string? webtoken { get; set; }

        [DefaultValue("NA")]
        public string? moiletoken { get; set; }
    }
}
