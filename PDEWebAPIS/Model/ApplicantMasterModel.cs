using PDEWebAPIS.Repository;

namespace PDEWebAPIS.Model
{
    public class ApplicantMasterModel
    {
        public int applicantid { get; set; }
        public ApplicationDTL? applicationDTL { get; set; }
        public UserMaster? userMaster { get; set; }
        public int usertype_code { set; get; } 
        public string usertype { set; get; } = string.Empty;
        public string mobileno { set; get; } = string.Empty;
        public string mobilenoverified { set; get; } = string.Empty;
        public string emailid { set; get; } = string.Empty;
        public string emailidverified { set; get; } = string.Empty;
        public string securitypin { set; get; } = string.Empty;
        public string prefix_in_eng { set; get; } = string.Empty;
        public string fname_in_eng { set; get; } = string.Empty;
        public string mname_in_eng { set; get; } = string.Empty;
        public string lname_in_eng { set; get; } = string.Empty;
        public string prefix_in_marathi { set; get; } = string.Empty;
        public string fname_in_marathi { set; get; } = string.Empty;
        public string mname_in_marathi { set; get; } = string.Empty;
        public string lname_in_marathi { set; get; } = string.Empty;
        public string company_name_in_marathi { set; get; } = string.Empty;
        public string company_name_in_eng { set; get; } = string.Empty;
        public string username { set; get; } = string.Empty;
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
        public string postofficename { set; get; } = string.Empty;
        public string address_proof_document_name { set; get; } = string.Empty;
        public string address_proof_document_path { set; get; } = string.Empty;
        public bool owner_of_property_in_maharashtra { set; get; } 
        public PropertyTypeMaster? PropertyTypeMaster { get; set; }
        public string property_district_code { set; get; } = string.Empty;
        public string property_district_name { set; get; } = string.Empty;
        public string property_taluka_code { set; get; } = string.Empty;
        public string property_taluka_name { set; get; } = string.Empty;
        public string property_village_code { set; get; } = string.Empty;
        public string property_village_name { set; get; } = string.Empty;
        public string khateno { set; get; } = string.Empty;
        public string city_servey_no { set; get; } = string.Empty;
        public string ulpin { set; get; } = string.Empty;
        public string profile_pic_file_name { set; get; } = string.Empty;
        public string profile_pic_file_path { set; get; } = string.Empty;
        public string signed_file_path { set; get; } = string.Empty;
        public string signed_file_name { set; get; } = string.Empty;
        public DateTime createddatetime { set; get; }
    }
}
