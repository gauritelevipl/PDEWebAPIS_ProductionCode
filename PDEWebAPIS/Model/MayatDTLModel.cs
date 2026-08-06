using PDEWebAPIS.Repository;

namespace PDEWebAPIS.Model
{
    public class MayatDTLModel
    {
        
        public int mayat_id { get; set; }
        public UserMaster? userMaster { get; set; }
        public ApplicationDTL? applicationDTL { get; set; }
        public string? owner_village_code { get; set; }

        public int mutation_cts_no_id { get; set; }
        public string? ctsNo { get; set; }
        public string? mutationSroNo { get; set; }
        public string? ownerNo { get; set; }
        public DateTime createddatetime { get; set; }

        public string? mobileno { get; set; } = "NA";

        public string? mobilenoverified { get; set; }
        public string? emailid { set; get; }
    
        public string? emailidverified { set; get; }
        public string? city_servey_no { get; set; }
        public string? lr_property_id { get; set; }
        public string? milkat { get; set; }
        public string? namud { get; set; }
        public string? sub_property_no { get; set; }
        public string? prefixcode_marathi { get; set; }
        public string? prefix_in_marathi { set; get; }
        public string? fname_in_marathi { set; get; }
        public string? mname_in_marathi { set; get; }
        public string? lname_in_marathi { set; get; }
        public string? aliceName { get; set; }
        public string? holderType { get; set; }
        public string? dob { get; set; }
        public string? motherName_in_marathi { set; get; }
        public string? motherName_in_eng { set; get; }
        public string? userName { get; set; }
        public string? prefixcode_eng { get; set; }
        public string? prefix_in_eng { set; get; }

        public string? fname_in_eng { set; get; }

        public string? mname_in_eng { set; get; }

        public string? lname_in_eng { set; get; }

        public string? alias_name { get; set; }
        public string? actual_area { get; set; }
        public string? address_type { get; set; }

        public string? address { get; set; }

        public string? flatno_plotno { get; set; }
        public string? plotno { get; set; } = "NA";
        public string? building { get; set; } = "NA";
        public string? mainroad { get; set; } = "NA";
        public string? impSymbol { get; set; } = "NA";
        public string? area { get; set; } = "NA";
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
        public string? address_proof_name { get; set; } = "NA";
        public string? address_proof_src { get; set; } = "NA";
        public string? signature_name { get; set; }
        public string? signature_src { get; set; }
        public string? mrutu_date { get; set; }
        public string? death_certificate_ofc_name { get; set; }
        public string? certificate_authority_code { get; set; }
        public string? certificate_authority_name { get; set; }
        public string? behalf_off_name { get; set; }
        public string? mrutucert_no { get; set; }
        public string? mrutu_certificate_date { get; set; }
        public string? mrutu_certificate_name { get; set; }
        public string? mrutu_certificate_path { get;set; }
        public string? is_name_same { get; set; }
        public string? reason { get; set; }
        public string? namecorrect_docname { get; set; }
        public string? namecorrect_docpath { get;set; }
        public string? isFullAreaGiven { get; set; }
        public string? actualArea { get; set; }
        public string? mutationArea { get; set; }
        public string? availableArea { get; set; }

        // Below fields are for Probet
        public bool isProbet { get; set; }
        public string? probet_file_name { get; set; }
        public string? probet_file_path { get; set; }
    }
}
