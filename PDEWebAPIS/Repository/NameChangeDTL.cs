using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PDEWebAPIS.Repository
{
    [Table("name_change_dtl")]
    public class NameChangeDTL
    {
        [Key, Required]
        public int name_change_id { get; set; }
        public UserMaster? userMaster { get; set; }
        public ApplicationDTL? applicationDTL { get; set; }
        public string? village_code { get; set; }
        public string? subPropNo { get; set; }
        public string? nabhu { get; set; }
        public string? lrPropertyUID { get; set; }
        public string? milkat { get; set; }
        public string? namud { get; set; }
        public int name_change_by_code { get; set; }
        public string? name_change_by_description { get; set; }
        public string? name_change_no { get; set; }
        public string? name_change_date { get; set; }
        // selectedMutation -> data from EPCIS
        public string? selected_village_code { get; set; }
        public string? selected_cts_number { get; set; }
        public string? selected_mutation_srno { get; set; }
        public string? selected_entry_date { get; set; }
        public string? selected_entry_bracketed { get; set; }
        public string? selected_owner_number { get; set; }
        public string? selected_owner_name { get; set; }
        public string? selected_first_name { get; set; }
        public string? selected_middle_name { get; set; }
        public string? selected_last_name { get; set; }
        public string? selected_nick_name { get; set; }
        public string? selected_owner_bracketed { get; set; }
        public string? selected_area_bracketed { get; set; }
        public string? selected_email_id { get; set; }
        public string? selected_owner_cell_number { get; set; }
        public string? selected_pincode { get; set; }
        public string? selected_owner_type { get; set; }
        public string? selected_apk_code { get; set; }
        public string? selected_apk_name { get; set; }
        public string? selected_flat_or_house_number { get; set; }
        public string? selected_building_number { get; set; }
        public string? selected_road { get; set; }
        public string? selected_city_or_village { get; set; }
        public string? selected_taluka_name { get; set; }
        public string? selected_district_name { get; set; }
        public string? selected_state_name { get; set; }
        public string? selected_gender_code { get; set; }
        public string? selected_date_of_birth { get; set; }
        public string? selected_owner_area { get; set; }
        public string? selected_owner_area_bracketed { get; set; }
        // Updated Details
        public int updated_userType { get; set; } = 0;
        public string? updated_userTypeLabel { get; set; }
        public string? updated_prefixcode_marathi { get; set; }
        public string? updated_prefix_in_marathi { get; set; }
        public string? updated_fname_in_marathi { get; set; }
        public string? updated_mname_in_marathi { get; set; }
        public string? updated_lname_in_marathi { get; set; }
        public string? updated_prefixcode_eng { get; set; }
        public string? updated_prefix_in_eng { get; set; }
        public string? updated_fname_in_eng { get; set; }
        public string? updated_mname_in_eng { get; set; }
        public string? updated_lname_in_eng { get; set; }
        public string company_name_in_marathi { set; get; } = string.Empty;
        public string company_name_in_eng { set; get; } = string.Empty;
        // Address Fields
        public string address_type { set; get; } = string.Empty;
        public string? emailid { get; set; }
        public string? mobileno { get; set; }
        [DefaultValue("FALSE")]
        public string? mobilenoverified { get; set; }
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
        public string postofficename { set; get; } = string.Empty;
        public string address_proof_document_name { set; get; } = string.Empty;
        public string address_proof_document_path { set; get; } = string.Empty;
        public DateTime createddatetime { set; get; }
        public bool isDeleted { set; get; }
        public DateOnly deleteddate { set; get; }
    }
}
