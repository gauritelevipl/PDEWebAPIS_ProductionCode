using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PDEWebAPIS.Repository
{
    [Table("buyerdtl")]
    public class BuyerDTL
    {
        [Key, Required]
        public int buyerid { get; set; }
        public UserMaster? userMaster { get; set; }
        [Required, MaxLength(15)]
        public string mobileno { set; get; } = string.Empty;
        [DefaultValue("FALSE")]
        public string mobilenoverified { set; get; } = string.Empty;
        public string? prefix_in_marathi { set; get; }
        public string? fname_in_marathi {  get; set; }
        public string? mname_in_marathi { get; set; }
        public string? lname_in_marathi { get; set; }
        public string? prefix_in_eng { set; get; }
        public string? fname_in_eng { get; set; }
        public string? mname_in_eng { get; set; }
        public string? lname_in_eng { get; set; }
        public string? alias_name { get; set; }
        public string? holder_type { get; set; }
        public string? dob { get; set; }
        public string? mother_name_in_marathi { get; set; }
        public string? mother_name_in_eng { get; set; }
        public string? address_type { set;get; }
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
        [Required]
        public DateTime createddatetime { set; get; }
    }
}
