using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PDEWebAPIS.Repository
{
    [Table("witness_info")]
    public class WitnessDTL
    {
        [Key, Required]
        public int witness_info_id { get; set; }
        public UserMaster? userMaster { get; set; }
        public ApplicationDTL? applicationDTL { get; set; }
        public string? permission_no { get; set; }
        public string? permission_date { get; set; }
        public string? prefixcode_marathi { set; get; }
        public string? prefix_in_marathi { set; get; }
        public string? fname_in_marathi { set; get; }
        public string? mname_in_marathi { set; get; }
        public string? lname_in_marathi { set; get; }
        public string? prefixcode_eng { set; get; }
        public string? prefix_in_eng { set; get; }
        public string? fname_in_eng { set; get; }
        public string? mname_in_eng { set; get; }
        public string? lname_in_eng { set; get; }
        public string? alias_name { get; set; }

        // Address Data
        public string? address_type { get; set; }
        public string? address { get; set; }
        public string? state { get; set; }
        public string? district { get; set; }
        public string? taluka { get; set; }
        public string? city { get; set; }
        public string? flatno_plotno { get; set; }
        public string? societyname { get; set; }
        public string? mainstreet { get; set; }
        public string? landmark { get; set; }
        public string? locality { get; set; }
        public string? pincode { get; set; }
        public string? post_office_name { get; set; }
        public string? address_proof_document_name { get; set; }
        public string? address_proof_document_path { get; set; }
        public string? mobileno { get; set; }
        [DefaultValue("FALSE")]
        public string? mobilenoverified { get; set; }
        public string? emailid { set; get; }
        [DefaultValue("FALSE")]
        public string? emailidverified { set; get; }
        public DateTime createddatetime { get; set; }
        public bool isDeleted { set; get; }
        public DateOnly deleteddate { set; get; }
    }
}
