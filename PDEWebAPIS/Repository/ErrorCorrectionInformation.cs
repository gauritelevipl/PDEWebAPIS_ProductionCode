using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PDEWebAPIS.Repository
{
    [Table("errorcorrectiondtls")]
    public class ErrorCorrectionInformation
    {
        [Key, Required]
        public int error_correction_id { get; set; }
        public UserMaster? userMaster { get; set; }
        public ApplicationDTL? applicationDTL { get; set; }
        public string? village_code { get; set; }
        public string? sub_property_no { get; set; }
        public string? city_servey_no { get; set; }
        public string? lr_property_id { get; set; }
        public string? milkat { get; set; }
        public string? namud { get; set; }

        // selectedMutation -> data from EPCIS
        //public string? var_village_code { get; set; }
        //public string? var_cts_number { get; set; }
        //public string? var_cts_puid { get; set; }
        //public string? var_mutation_srno { get; set; }
        //public string? var_entry_date { get; set; }
        //public string? var_mutation_number { get; set; }
        //public string? var_mutation_date { get; set; }
        //public string? var_sro_office_name_marathi { get; set; }
        //public string? var_sro_office_name_english { get; set; }
        //public string? var_document_number { get; set; }
        //public string? var_document_year { get; set; }
        //public string? var_document_date { get; set; }
        //public string? var_entry_details { get; set; }
        //public string? var_owner_details { get; set; }
        // End EPCIS data fields
        public string? reason { get; set; }
        // Address Data
        public string? address_type { get; set; }
        public string? emailid { get; set; }
        public string? mobileno { get; set; }
        [DefaultValue("FALSE")]
        public string? mobilenoverified { get; set; }
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
        public DateTime createddatetime { set; get; }
        public bool isDeleted { set; get; }
        public DateOnly deleteddate { set; get; }
    }
}
