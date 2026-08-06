using PDEWebAPIS.Repository;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace PDEWebAPIS.Model
{
    public class GahankhatModel
    {
        public int mutation_givertaker_id { get; set; }
        public UserMaster? userMaster { get; set; }
        public ApplicationDTL? applicationDTL { get; set; }
        public string? mutationSroNo { set; get; }
        public string? owner_village_code { set; get; }
        
        public string? ctsNo { set; get; }
        public string? ownerNo { set; get; }
        public int mutation_cts_no_id { get; set; }
        public PropertyTypeMaster? prop_type { get; set; }
        // 0 -> Giver , 1 -> Taker
        public int isTaker { get; set; }
        [Required, MaxLength(15)]
        public string? mobileno { get; set; }
        [DefaultValue("FALSE")]
        public string? mobilenoverified { get; set; }
        public string? emailid { set; get; }
        [DefaultValue("FALSE")]
        public string? emailidverified { set; get; }
        //user Details
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
        public string? holder_type { get; set; }
        public string? dob { get; set; }
        public string? mother_name_in_marathi { get; set; }
        public string? mother_name_in_eng { get; set; }
        public string? userName { get; set; }
        public string? city_servey_no { get; set; } = null;
        public string? lr_property_id { get; set; }
        public string? milkat { get; set; }
        public string? namud { get; set; }
        public string? sub_property_no { get; set; }
        public string? isFullAreaGiven { get; set; }
        public string? actual_area { get; set; }
        public string? mutation_area { get; set; }
        public string? available_area { get; set; }
        public string? address_type { get; set; }
        public string? address { get; set; }
        public string? flatno_plotno { get; set; }
        public string? societyname { get; set; }
        public string? mainstreet { get; set; }
        public string? landmark { get; set; }
        public string? locality { get; set; }
        public string? pincode { get; set; }
        public string? post_office_name { get; set; }
        public string? city { get; set; }
        public string? taluka { get; set; }
        public string? district { get; set; }
        public string? state { get; set; }
        public string? address_proof_document_name { get; set; }
        public string? address_proof_document_path { get; set; }
        public string? signed_file_name { get; set; }
        public string? signed_file_path { get; set; }

        // Below fields for Taker
        public int institute_code {  get; set; }
        public string? institute_description {  get; set; }
        public string? bank_name_in_marathi { set; get; }
        public string? bank_name_in_english {  set; get; }
        public string? ifsc {  set; get; }
        public string? bojaArea { get; set; }
        public string? bojaValue { get; set; }
        public string? bojaDate { get; set; }
        public string? bojaPeriod { get; set; }
    }
}
