using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PDEWebAPIS.Repository
{
    [Table("mayatdtl")]
    public class MayatDTL
    {
        [Key, Required]
        public int mayat_id { get; set; }
        public UserMaster? userMaster { get; set; }
        public ApplicationDTL? applicationDTL { get; set; }
        public int mutation_cts_no_id { get; set; }
        [Required, MaxLength(15)]
        public string? mobileno { get; set; }
        [DefaultValue("FALSE")]
        public string? mobilenoverified { get; set; }
        public string? emailid { set; get; }
        [DefaultValue("FALSE")]
        public string? emailidverified { set; get; }
        public string? prefixcode_marathi { set; get; }
        public string? prefix_in_marathi { set; get; }
        public string? fname_in_marathi { set; get; }
        public string? mname_in_marathi { set; get; }
        public string? lname_in_marathi { set; get; }
        public string? prefixcode_eng { get; set; }
        public string? prefix_in_eng { set; get; }
        public string? fname_in_eng { set; get; }
        public string? mname_in_eng { set; get; }
        public string? lname_in_eng { set; get; }
        public string? alias_name { get; set; }
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
        [Required]
        public string? post_office_name { get; set; }
        public string? city_servey_no { get; set; }
        public string? lr_property_id { get; set; }
        public string? milkat { get; set; }
        public string? namud { get; set; }
        public string? sub_property_no { get; set; }
        public string? mutation_srno { set; get; } = "NA";
        public string? owner_number { set; get; } = "NA";
        public string? cts_number { set; get; } = "NA";
        public string? actual_area { get; set; }
        public string? mrutyu_date { get; set; }
       // public string? death_certificate_ofc_name { get; set; }
        public string? certificate_authority_code { get; set; }
        public string? certificate_authority_name { get; set; }
        public string? mrutyucert_no { get;set; }
        public string? mrutyu_certificate_date { get; set; }
        public string? mrutyu_certificate__name { get; set; }
        public string? mrutyu_certificate_path { get; set; }
      //  public string? mrutyu_certificate_taker_name { set; get; }
        public string? is_name_same { get; set; }
        public string? reason { get; set; }
        public string? namecorrect_docname { get; set; }
        public string? namecorrect_docpath { get; set; }
        public string? address_proof_document_name { get; set; }
        public string? address_proof_document_path { get; set; }
        public string? signed_file_name { get; set; }
        public string? signed_file_path { get; set; }
        public DateTime createddatetime { get; set; }
        public bool isDeleted { set; get; }
        public DateOnly deleteddate { set; get; }
        public string? owner_village_code { get; set; }

        // Below fields are for Probet
        public bool isprobet { get; set; }
        public string? probet_file_name { get; set; }
        public string? probet_file_path { get; set; }
    }

}
