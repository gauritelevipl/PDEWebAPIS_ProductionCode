using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace PDEWebAPIS.Repository
{
    [Table("usermaster")]
    public class UserMaster
    {
        [Key, Required]
        public int userid { get; set; }
        public int usertype_code { set; get; }
        [Required(ErrorMessage = "User Type Is Required!")]
        public string usertype { set; get; } = string.Empty;
        [Required, MaxLength(15)]
        public string mobileno { set; get; } = string.Empty;
        [DefaultValue("NO")]
        public string mobilenoverified { set; get; } = string.Empty;
        [Required]
        public string emailid { set; get; } = string.Empty;
        [DefaultValue("NO")]
        public string emailidverified { set; get; } = string.Empty;
        [Required, MaxLength(6)]
        public string securitypin { set; get; } = string.Empty;
        [Required]
        public string prefixcode_eng { get; set; } = string.Empty;
        public string prefix_in_eng { set; get; } = string.Empty;
        [Required]
        public string fname_in_eng { set; get; } = string.Empty;
        [Required]
        public string mname_in_eng { set; get; } = string.Empty;
        [Required]
        public string lname_in_eng { set; get; } = string.Empty;
        public string prefixcode_marathi {  set; get; } = string.Empty;
        [Required]
        public string prefix_in_marathi { set; get; } = string.Empty;
        [Required]
        public string fname_in_marathi { set; get; } = string.Empty;
        [Required]
        public string mname_in_marathi { set; get; } = string.Empty;
        [Required]
        public string lname_in_marathi { set; get; } = string.Empty;
        [Required]
        public string company_name_in_marathi { set; get; } = string.Empty;
        [Required]
        public string company_name_in_eng { set; get; } = string.Empty;
        [Required]
        public string username { set; get; } = string.Empty;
        [Required]
        public string address_type { set; get; } = string.Empty;
        public string address { set; get; } = string.Empty;
        public string state { set; get; } = string.Empty;
        public string district { set; get; } = string.Empty;
        public string taluka { set; get; } = string.Empty;
        public string city { set; get; } = string.Empty;
        public string flatno_plotno { set; get; } = string.Empty;
        public string societyname { set; get; } = string.Empty;
        public string mainstreet { set; get; } = string.Empty;
        public string landmark { set; get; } = string.Empty;
        public string locality { set; get; } = string.Empty;
        public string pincode { set; get; } = string.Empty;
        [Required]
        public string postofficename { set; get; } = string.Empty;
        public string address_proof_document_name { set; get; } = string.Empty;
        public string address_proof_document_path { set; get; } = string.Empty;
        [Required]
        public bool owner_of_property_in_maharashtra { set; get; }
        //[Required]
        //public PropertyTypeMaster? PropertyTypeMaster { get; set; }
        public PropertyTypeMaster PropertyTypeMaster { get; set; }

        //public Int32 propertytypeid { set; get; }
        //public string property_flat_no { set; get; } = string.Empty;

        //
        public string? property_district_code { set; get; }
        public string? property_district_name { set; get; }
        public string? property_taluka_code { set; get; }
        public string? property_taluka_name { set; get; }
        public string? property_village_code { set; get; }
        public string? property_village_name { set; get; }
        public string? khateno { set; get; } 
        public string? city_servey_no { set; get; }
        public string? ulpin { set; get; }
        [Required]
        public string profile_pic_file_name { set; get; } = string.Empty;
        [Required]
        public string profile_pic_file_path { set; get; } = string.Empty;
        [Required]
        public string signed_file_path { set; get; } = string.Empty;
        [Required]
        public string signed_file_name { set; get; } = string.Empty;
        [Required]
        public DateTime createddatetime { set; get; }
        [Required]
        public string web_token { set; get; } = string.Empty;
        [Required]
        public string mobile_token { set; get; } = string.Empty;
    }
}
