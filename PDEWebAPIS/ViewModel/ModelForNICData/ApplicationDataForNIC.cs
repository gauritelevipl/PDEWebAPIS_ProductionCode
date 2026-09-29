using PDEWebAPIS.InputDataModel;
using PDEWebAPIS.Repository;

namespace PDEWebAPIS.ViewModel.ModelForNICData
{
    public class ApplicationDataForNIC
    {
        public List<FetchUserDataForNIC>? usermaster { set; get; }
        public List<FetchApplicationDTLsForNIC>? applicationdtl { get; set; }
        public List<FetchApplicantsDataForNIC>? applicantmaster { get; set; }
        public List<FetchMutationCTSNoDataForNIC>? mutation_cts_no_dtl { get; set; }
        public List<FetchDastInformationDataForNIC>? dast_information { get; set; } = null;
        public List<FetchCourtClaimInformationDataForNIC>? court_claim_information { get; set; } = null;
        public List<FetchPOADataForNIC> ? power_of_attorney_information {  get; set; } = null;
        public List<FetchMayatDTLForNIC>? mayatdtl { get; set; }
        //public List<FetchPOAForGiverDataForNIC>? power_puof_attorney_information_giver { get; set; }
        //public List<FetchPOAForTakerDataForNIC>? power_puof_attorney_information_taker { get; set; }
        public List<FetcMutationGiverTakerDTL>? mutationgivertakerdtls { get; set; }
        public List<FetchBhadepattaInfoDTLForNIC>? bhadepattaInfoDtl { get; set; }
        public List<FetchErrorCorrectionDataForNIC>? errorcorrectiondtls { get; set; }
        public List<FetchNavatBadalDataForNIC>? name_change_dtl { get; set; }
        public List<FetchHibanamaWitnessDataForNIC>? witness_info { get; set; }
        //public dynamic? mutationgivertakerdtls { get; set; }
        //public List<FetchUploadedDocumentsForNIC>? uploaded_documents_dtl { get; set; }
        public List<FetchUploadedDocumentDataForNIC>? uploaded_documents_dtl { get; set; }
    }

    public class FetchUserDataForNIC
    {
        public int userid { set; get; }
        public int usertype_code { set; get; }
        public string? usertype { set; get; }
        public string? mobileno { set; get; }
        public string? mobilenoverified { set; get; }
        public string? emailid { set; get; }
        public string? emailidverified { set; get; }
        public string? securitypin { set; get; }
        public string? prefixcode_eng { set; get; }
        public string? prefix_in_eng { set; get; }
        public string? fname_in_eng { set; get; }
        public string? mname_in_eng { set; get; }
        public string? lname_in_eng { set; get; }
        public string? prefixcode_marathi { set; get; }
        public string? prefix_in_marathi { set; get; }
        public string? fname_in_marathi { set; get; }
        public string? mname_in_marathi { set; get; }
        public string? lname_in_marathi { set; get; }
        public string? company_name_in_marathi { set; get; }
        public string? company_name_in_eng { set; get; }
        public string? username { set; get; }
        public string? address_type { set; get; }
        public string? address { set; get; }
        public string? state { set; get; }
        public string? district { set; get; }
        public string? taluka { set; get; }
        public string? city { set; get; }
        public string? flatno_plotno { set; get; }
        public string? societyname { set; get; }
        public string? mainstreet { set; get; }
        public string? landmark { set; get; }
        public string? locality { set; get; }
        public string? pincode { set; get; }
        public string? postofficename { set; get; }
        public string? address_proof_document_name { set; get; }
        public string? address_proof_document_path { set; get; }
        public bool owner_of_property_in_maharashtra { set; get; }
        public int propertytypemasterpropertytypeid { get; set; }
        public string? property_district_code { set; get; }
        public string? property_district_name { set; get; }
        public string? property_taluka_code { set; get; }
        public string? property_taluka_name { set; get; }
        public string? property_village_code { set; get; }
        public string? property_village_name { set; get; }
        public string? khateno { set; get; }
        public string? city_servey_no { set; get; }
        public string? ulpin { set; get; }
        public string? profile_pic_file_name { set; get; }
        public string? profile_pic_file_path { set; get; }
        public string? signed_file_path { set; get; }
        public string? signed_file_name { set; get; }
        public string? createddatetime { set; get; }
        public string? web_token { set; get; } 
        public string? mobile_token { set; get; } 
    }

    public class FetchApplicationDTLsForNIC
    {
        public string? applicationid { get; set; }
        public int usermasteruserid { get; set; }
        public string? applicationtypemasterapplicationtypeid { get; set; }
        public string? mutation_type_code { set; get; }
        public string? mutation_type_name { set; get; }
        public string? district_code { set; get; }
        public string? district_name_in_marathi { set; get; }
        public string? district_name_in_english { set; get; }
        public string? office_code { set; get; }
        public string? office_name { set; get; }
        public bool do_you_have_power_of_attorney { set; get; }
        public bool is_the_claim_pending_before_the_court { set; get; }
        public bool does_the_original_charter_info_apply { set; get; }
        public string? applicantids { set; get; }
        public string? mutation_cts_nos { set; get; }
        public string? dastids { set; get; }
        public string? courtclaimids { set; get; }
        public string? powerofattorneyids { set; get; }
        public string? uploadeddocids { get; set; }
        public string? mutationgiverids { set; get; }
        public string? mutationtakerids { set; get; }
        public string? mayatids { set; get; }
        public string? varasids { set; get; }
        public string? inwardno { set; get; }
        public int status { set; get; }
        public string? self_declaration_doc_name { set; get; }
        public string? self_declaration_doc_path { set; get; }
        public string? createddatetime { get; set; }
        public bool isdeleted { set; get; }
        public string? deleteddate { set; get; }
        public bool? is_sentnic { set; get; }
        public int? sentnic_attempts { set; get; }
    }

    public class FetchApplicantsDataForNIC
    {
        public int applicantid { get; set; }
        public int usermasteruserid { get; set; }
        public string? applicationdtlapplicationid { set; get; }
        public int propertytypemasterpropertytypeid { get; set; }
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
        public string khateno { set; get; } = string.Empty;
        public string city_servey_no { set; get; } = string.Empty;
        public string ulpin { set; get; } = string.Empty;
        public string property_district_code { set; get; } = string.Empty;
        public string property_district_name { set; get; } = string.Empty;
        public string property_taluka_code { set; get; } = string.Empty;
        public string property_taluka_name { set; get; } = string.Empty;
        public string property_village_code { set; get; } = string.Empty;
        public string property_village_name { set; get; } = string.Empty;
        public string profile_pic_file_name { set; get; } = string.Empty;
        public string profile_pic_file_path { set; get; } = string.Empty;
        public string signed_file_path { set; get; } = string.Empty;
        public string signed_file_name { set; get; } = string.Empty;
        public string? createddatetime { set; get; }
        public bool isdeleted { set; get; }
        public string? deleteddate { set; get; }
    }

    public class FetchMutationCTSNoDataForNIC
    {
        public int mutation_cts_no_id { get; set; }
        public int usermasteruserid { get; set; }
        public string? applicationdtlapplicationid { get; set; }
        public string? what_is_mentioned_in_the_doc { get; set; }
        public string? village_or_peth_code { get; set; }
        public string? village_or_peth_name { get; set; }
        public string? village_lgd_code { get; set; }
        public string? village_english_name { get; set; }
        public string? zone_code { get; set; }
        public string? amount { set; get; }
        public string? mutation_modification_type { set; get; }
        public string? city_servey_no_mentioned_in_application { get; set; }
        public string? servey_no { get; set; }
        public string? selected_city_servey_no { get; set; }
        public string? lr_property_uid { get; set; }
        public string? application_income_type { get; set; }
        public string? city_servey_area_in_sq_m { get; set; }
        public string? building_name { get; set; }
        public int? floor_type { get; set; }
        public string? floor_desc { get; set; }
        public int? floor_order_by { get; set; }
        public string? floor_no { get; set; }
        public int? unit_code_156 { get; set; }
        public string? unit_name_156 { get; set; }
        public string? unit_no { get; set; }
        public string? buildup_area_in_sq_m { get; set; }
        public string? carpet_area_in_sq_m { get; set; }
        public string? terrace_area_in_sq_m { get; set; }
        public string? parking_no { get; set; }
        public string? parking_area_in_sq_m { get; set; }
        public string? shares_in_percent { get; set; }
        public string? nic_flat_details { get; set; }
        public string? flat_bulit_up_area { get; set; }
        public string? sub_property_id { get; set; }
        public string? createddatetime { get; set; }
        public bool isdeleted { get; set; }
        public string? deleteddate { get; set; }
    }

    public class FetchDastInformationDataForNIC
    {
        public int dast_id { get; set; }
        public int? usermasteruserid { get; set; }
        public string? applicationdtlapplicationid { get; set; }
        public string? dasttype { get; set; }
        public string? division_code { get; set; }
        public string? division_name { get; set; }
        public string? districtcode { set; get; }
        public string? districtname { set; get; }
        public string? office_of_the_second_registrar_code { get; set; }
        public string? office_of_the_second_registrar_name { get; set; }
        public string? registered_dast_no { get; set; }
        public string? registered_dast_date { get; set; }
        public string? registered_dast_year { get; set; }
        public string? dastnabhu { get; set; }
        public string? remarks { get; set; }
        public bool? isdastverified { get; set; }
        public string? verifieddastdata { get; set; }
        public string? createddatetime { get; set; }
        public bool isdeleted { get; set; }
        public string? deleteddate { get; set; }
    }

    public class FetchCourtClaimInformationDataForNIC
    {
        public int court_claim_id { get; set; }
        public int? usermasteruserid { get; set; }
        public string? applicationdtlapplicationid { get; set; }
        public string? court_case_code { get; set; }
        public string? court_case_name { get; set; }
        public string? court_case_type_code { get; set; }
        public string? court_case_type_name { get; set; }
        public string? lr_property_uid { get; set; }
        public string? city_servey_no { get; set; }
        public string? order_details { get; set; }
        public string? stay_order { get; set; }
        public string? sub_property_no { get; set; }
        public string? createddatetime { get; set; }
        public bool isdeleted { get; set; }
        public string? deleteddate { get; set; }
    }

    public class FetchPOAForGiverDataForNIC
    {
        public int? power_of_attorney_id { get; set; }
        public int? userid { get; set; }
        public string? applicationid { get; set; }
        public int power_of_attorney_code { get; set; }
        public bool is_taker { get; set; }
        public int usertype_code { get; set; }
        public string? usertype { get; set; }
        public int mutation_id { get; set; }
        public string? mobileno { get; set; }
        public string? mobilenoverified { get; set; }
        public string? emailid { get; set; }
        public string? emailidverified { get; set; }
        public string? prefixcode_marathi { get; set; }
        public string? prefix_in_marathi { get; set; }
        public string? fname_in_marathi { get; set; }
        public string? mname_in_marathi { get; set; }
        public string? lname_in_marathi { get; set; }
        public string? prefixcode_eng { get; set; }
        public string? prefix_in_eng { get; set; }
        public string? fname_in_eng { get; set; }
        public string? mname_in_eng { get; set; }
        public string? lname_in_eng { get; set; }
        public string? company_name_in_marathi { set; get; }
        public string? company_name_in_eng { get; set; }
        public string? username { get; set; }
        public string? alias_name { get; set; }
        public string? gender_code { get; set; }
        public string? gender_description { get; set; }
        public string? dob { get; set; }
        public string? mother_name_in_marathi { get; set; }
        public string? mother_name_in_eng { set; get; }
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
        public string? postofficename { get; set; }
        public string? address_proof_document_name { get; set; }
        public string? address_proof_document_path { get; set; }
        public string? city_servey_no { get; set; }
        public string? lr_property_id { get; set; }
        public string? sub_property_no { get; set; }
        public bool owner_of_property_in_maharashtra { get; set; }
        public int propertytypeid { get; set; }
        public string? property_district_code { get; set; }
        public string? property_district_name_in_marathi { get; set; }
        public string? property_district_name_in_english { get; set; }
        public string? property_taluka_code { get; set; }
        public string? property_taluka_name { get; set; }
        public string? property_city_code { get; set; }
        public string? property_city_name { get; set; }
        public string? khateno { get; set; }
        public string? ulpin { get; set; }
        public string? khata_type_code { get; set; }
        public string? khata_type_name { get; set; }
        public string? owner_status_code { get; set; }
        public string? owner_status_description { get; set; }
        public int attornytype_code { set; get; }
        public string? attornytype_desc { set; get; }
        public string? land_buy_area { set; get; } = string.Empty;
        public string is_poa_is_partof_dast { set; get; } = string.Empty;
        public string is_decleration_involved_in_poa { set; get; } = string.Empty;
        public string is_poa_permanant { set; get; } = string.Empty;
        public string is_transfer_rights { set; get; } = string.Empty;
        public string dast_no { set; get; } = string.Empty;
        public string dast_no_date { set; get; } = string.Empty;
        public string dast_no_year { set; get; } = string.Empty;
        public bool is_dast_verified { set; get; } = false;
        public string verifieddast_data { set; get; } = string.Empty;
        public int? digcode { set; get; } = 0;
        public string digname { set; get; } = string.Empty;
        public string poa_district_code { set; get; } = string.Empty;
        public string poa_district_name { set; get; } = string.Empty;
        public int sro_office_code { set; get; }
        public string sro_office_name { set; get; } = string.Empty;
        public string? createddatetime { set; get; }
        public bool isdeleted { set; get; }
        public string? deleteddate { set; get; }
        public string signed_file_path { set; get; } = string.Empty;
        public string signed_file_name { set; get; } = string.Empty;
        public string profile_pic_file_name { set; get; } = string.Empty;
        public string profile_pic_file_path { set; get; } = string.Empty;
        public string? cts_number { set; get; }
        public string? mutation_srno { set; get; }
        public string? owner_number { set; get; }
        public string? village_code { set; get; }
        public string? village_name { set; get; }
    }
    public class FetchPOAForTakerDataForNIC
    {
        public int? power_of_attorney_id { get; set; }
        public int? userid { get; set; }
        public string? applicationid { get; set; }
        public int power_of_attorney_code { get; set; }
        public bool is_taker { get; set; }
        public int usertype_code { get; set; }
        public string? usertype { get; set; }
        public int mutation_id { get; set; }
        public string? mobileno { get; set; }
        public string? mobilenoverified { get; set; }
        public string? emailid { get; set; }
        public string? emailidverified { get; set; }
        public string? prefixcode_marathi { get; set; }
        public string? prefix_in_marathi { get; set; }
        public string? fname_in_marathi { get; set; }
        public string? mname_in_marathi { get; set; }
        public string? lname_in_marathi { get; set; }
        public string? prefixcode_eng { get; set; }
        public string? prefix_in_eng { get; set; }
        public string? fname_in_eng { get; set; }
        public string? mname_in_eng { get; set; }
        public string? lname_in_eng { get; set; }
        public string? company_name_in_marathi { set; get; }
        public string? company_name_in_eng { get; set; }
        public string? username { get; set; }
        public string? alias_name { get; set; }
        public string? gender_code { get; set; }
        public string? gender_description { get; set; }
        public string? dob { get; set; }
        public string? mother_name_in_marathi { get; set; }
        public string? mother_name_in_eng { set; get; }
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
        public string? postofficename { get; set; }
        public string? address_proof_document_name { get; set; }
        public string? address_proof_document_path { get; set; }
        public string? city_servey_no { get; set; }
        public string? lr_property_id { get; set; }
        public string? sub_property_no { get; set; }
        public bool owner_of_property_in_maharashtra { get; set; }
        public int propertytypeid { get; set; }
        public string? property_district_code { get; set; }
        public string? property_district_name_in_marathi { get; set; }
        public string? property_district_name_in_english { get; set; }
        public string? property_taluka_code { get; set; }
        public string? property_taluka_name { get; set; }
        public string? property_city_code { get; set; }
        public string? property_city_name { get; set; }
        public string? khateno { get; set; }
        public string? ulpin { get; set; }
        public string? khata_type_code { get; set; }
        public string? khata_type_name { get; set; }
        public string? owner_status_code { get; set; }
        public string? owner_status_description { get; set; }
        public int attornytype_code { set; get; }
        public string? attornytype_desc { set; get; }
        public string? land_buy_area { set; get; } = string.Empty;
        public string is_poa_is_partof_dast { set; get; } = string.Empty;
        public string is_decleration_involved_in_poa { set; get; } = string.Empty;
        public string is_poa_permanant { set; get; } = string.Empty;
        public string is_transfer_rights { set; get; } = string.Empty;
        public string dast_no { set; get; } = string.Empty;
        public string dast_no_date { set; get; } = string.Empty;
        public string dast_no_year { set; get; } = string.Empty;
        public bool is_dast_verified { set; get; } = false;
        public string verifieddast_data { set; get; } = string.Empty;
        public int? digcode { set; get; } = 0;
        public string digname { set; get; } = string.Empty;
        public string poa_district_code { set; get; } = string.Empty;
        public string poa_district_name { set; get; } = string.Empty;
        public int sro_office_code { set; get; }
        public string sro_office_name { set; get; } = string.Empty;
        public string? createddatetime { set; get; }
        public bool isdeleted { set; get; }
        public string? deleteddate { set; get; }
        public string signed_file_path { set; get; } = string.Empty;
        public string signed_file_name { set; get; } = string.Empty;
        public string profile_pic_file_name { set; get; } = string.Empty;
        public string profile_pic_file_path { set; get; } = string.Empty;
        public string? cts_number { set; get; }
        public string? mutation_srno { set; get; }
        public string? owner_number { set; get; }
        public string? village_code { set; get; }
        public string? village_name { set; get; }
    }

    public class FetchUploadedDocumentsForNIC
    {
        public int uploaded_doc_id { set; get; }
        public int usermasteruserid { set; get; }
        public string? applicationid { set; get; }
        public string? document_type_code { set; get; }
        public string? document_type { get; set; }
        public string? city_servey_no { set; get; }
        public string? document_name { set; get; }
        public string? document_path { set; get; }
        public string? createddatetime { set; get; }
        public bool isDeleted { set; get; }
        public string? deleteddate { set; get; }
    }

    public class FetchUploadedDocumentDataForNIC
    {
        public int uploaded_doc_id { set; get; }
        public int usermasteruserid { set; get; }
        public string? applicationdtlapplicationid { set; get; }
        public string? document_type_code { set; get; }
        public string? document_type { get; set; }
        public string? city_servey_no { set; get; }
        public string? document_name { set; get; }
        public string? document_path { set; get; }
        public string? createddatetime { set; get; }
        public bool isdeleted { set; get; }
        public string? deleteddate { set; get; }
        //public string? applicationID { set; get; }
        //public string? documentID { set; get; }
        //public string? documentTypeCode { set; get; }
        //public string? documentType { set; get; }
        //public string? city { set; get; }
        //public string? nabhuno { set; get; }
        //public string? docName { set; get; }
    }

    public class FetcMutationGiverTakerDTL
    {
        public int mutation_givertaker_id { set; get; }
        public int usermasteruserid { set; get; }
        public string? applicationdtlapplicationid { set; get; }
        public int user_type_code { set; get; }
        public string? user_type { set; get; }
        public string? prop_typepropertytypeid { set; get; }
        public int istaker { set; get; }
        public string? mobileno { set; get; }
        public string? mobilenoverified { set; get; }
        public string? emailid { set; get; }
        public string? emailidverified { set; get; }
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
        public string? alias_name { set; get; }
        public string? company_name_in_marathi { set; get; }
        public string? company_name_in_eng { set; get; }
        public string? gender_code { set; get; }
        public string? gender_description { set; get; }
        public string? holder_type { set; get; }
        public string? dob { set; get; }
        public string? mother_name_in_marathi { set; get; }
        public string? mother_name_in_eng { set; get; }
        public string? username { set; get; }
        public string? city_servey_no { set; get; }
        public string? lr_property_id { set; get; }
        public string? sub_property_no { get; set; }
        // NIC Columns
        public string? sellerid { set; get; }
        public string? buyerid { set; get; }
        public string? mutation_srno { set; get; }
        public string? owner_number { set; get; }
        public string? cts_number { set; get; }
        public string? milkat { get; set; }
        public string? namud { get; set; }
        public string? isfullareagiven { get; set; }
        public string? actual_area { get; set; }
        public string? available_area { get; set; }
        public string? mutation_area { get; set; }

        //Bakshish Patra
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
        public string? has_property { get; set; }
        //public string? khata_type { get; set; }   //Commented By Gauri becaues we have to remove th this column


        //public string? aapak_dropdown { get; set; }   //Commented By Gauri becaues we have to remove th this column
        public string? aapak { get; set; }
        public string? land_buy_area { get; set; }
        //Bakshish Patra Taker
        //11-11-2024 As per discussion only add 3 column for area
        //  public string? gift_area { get; set; }


        // 27 Sept 24
        public int? account_type_code { get; set; }
        public string? account_type_description { get; set; }
        public int? apk_code { get; set; }
        public string? apk_description { get; set; }

        // 01 Oct 24
        public string? khata_type_code { get; set; }
        public string? khata_type_name { get; set; }
        public string? owner_status_code { get; set; }
        public string? owner_status_description { get; set; }

        // 30 Sept 24
        public string? khatano { get; set; }
        public string? ulpin { get; set; }
        public string? district_code { get; set; }
        public string? district_name_in_marathi { get; set; }
        public string? district_name_in_eng { get; set; }
        public string? village_code { get; set; }
        public string? village_name { get; set; }
        public string? ofc_code { get; set; }
        public string? ofc_name { get; set; }

        // 04 Oct 24 For MrutyuPatra
        // public string? actualArea { set;get; }
        // public string? benefitArea { set; get; }

        public int relation_code { get; set; }
        public string? relation_name { get; set; }
        public int varas_relation_code { get; set; }
        public string? varas_relation_name { get; set; }

        // Gahankhat Nond Denar
        public int institute_code { set; get; }
        public string? institute_description { set; get; }
        public string? bank_name_in_marathi { set; get; }
        public string? bank_name_in_english { set; get; }
        public string? ifsc { set; get; }
        public string? boja_value { set; get; }
        public string? boja_date { set; get; }
        public string? boja_period { set; get; }
        // Hakkasod taker
        public string? benefit_amt { set; get; }
        public bool isdeleted { set; get; }
        public string? deleteddate { set; get; }
        public string? createddatetime { get; set; }
        public string? signed_file_name { get; set; }
        public string? signed_file_path { get; set; }
        //Taker
        public string? profile_pic_file_name { get; set; }
        public string? profile_pic_file_path { get; set; }
        public int mutation_cts_no_id { get; set; }
        //Below are new fields  Added on 16 Feb 2026
        public string? owner_village_code { get; set; }
        public string? entry_bracketed { get; set; }
        public string? entry_date { get; set; }
        public string? owner_bracketed { set; get; }
        public string? owner_name { get; set; }
    }

    public class FetchPOADataForNIC
    {
        public int power_of_attorney_id { get; set; }
        public int usermasteruserid { get; set; }
        public string? applicationdtlapplicationid { get; set; }
        //New Fields For Taker
        public int power_of_attorney_code { get; set; }
        // 0 -> Giver , 1 -> Taker
        public bool is_taker { get; set; }
        public int usertype_code { set; get; } = 0;
        public string usertype { set; get; } = string.Empty;
        public int mutation_id { set; get; } = 0;
        public string mobileno { set; get; } = string.Empty;
        public string mobilenoverified { set; get; } = string.Empty;
        public string emailid { set; get; } = string.Empty;
        public string emailidverified { set; get; } = string.Empty;
        public string prefixcode_marathi { get; set; } = string.Empty;
        public string prefix_in_marathi { set; get; } = string.Empty;
        public string fname_in_marathi { set; get; } = string.Empty;
        public string mname_in_marathi { set; get; } = string.Empty;
        public string lname_in_marathi { set; get; } = string.Empty;
        public string prefixcode_eng { get; set; } = string.Empty;
        public string prefix_in_eng { set; get; } = string.Empty;
        public string fname_in_eng { set; get; } = string.Empty;
        public string mname_in_eng { set; get; } = string.Empty;
        public string lname_in_eng { set; get; } = string.Empty;
        public string company_name_in_marathi { set; get; } = string.Empty;
        public string company_name_in_eng { set; get; } = string.Empty;
        public string username { set; get; } = string.Empty;
        public string alias_name { set; get; } = string.Empty;
        public string gender_code { set; get; } = string.Empty;
        public string gender_description { set; get; } = string.Empty;
        public string dob { set; get; } = string.Empty;
        public string mother_name_in_marathi { set; get; } = string.Empty;
        public string mother_name_in_eng { set; get; } = string.Empty;
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
        public string city_servey_no { set; get; } = string.Empty;
        public string lr_property_id { set; get; } = string.Empty;
        public string sub_property_no { set; get; } = string.Empty;
        public bool owner_of_property_in_maharashtra { set; get; }
        public string? mutation_srno { set; get; }
        public string? owner_number { set; get; } 
        public string? cts_number { set; get; } 
        public string? village_code { set; get; } 
        public string? village_name { get; set; } 
        public int propertytypeid { get; set; }
        public string? property_district_code { set; get; }
        public string? property_district_name_in_marathi { set; get; }
        public string? property_district_name_in_english { set; get; }
        public string? property_taluka_code { set; get; }
        public string? property_taluka_name { set; get; }
        public string? property_city_code { set; get; }
        public string? property_city_name { set; get; }
        public string? khateno { set; get; }
        public string? ulpin { set; get; }
        public string khata_type_code { set; get; } = string.Empty;
        public string khata_type_name { set; get; } = string.Empty;
        public string owner_status_code { set; get; } = string.Empty;
        public string owner_status_description { set; get; } = string.Empty;
        public int attornytype_code { set; get; }
        public string? attornytype_desc { set; get; }
        public string? landbuyarea { set; get; } = string.Empty;
        public string ispoaispartofdast { set; get; } = string.Empty;
        public string isdeclerationinvolvedinpoa { set; get; } = string.Empty;
        public string ispoapermanant { set; get; } = string.Empty;
        public string istransferrights { set; get; } = string.Empty;
        public string dast_no { set; get; } = string.Empty;
        public string dast_no_date { set; get; } = string.Empty;
        public string dast_no_year { set; get; } = string.Empty;
        public bool isdastverified { set; get; } = false;
        public string verifieddastdata { set; get; } = string.Empty;
        public int? digcode { set; get; } = 0;
        public string digname { set; get; } = string.Empty;
        public string poa_district_code { set; get; } = string.Empty;
        public string poa_district_name { set; get; } = string.Empty;
        public int sro_office_code { set; get; }
        public string sro_office_name { set; get; } = string.Empty;

        //End
        // Columns for NIC
        public string? createddatetime { set; get; }
        public bool isdeleted { set; get; }
        public string? deleteddate { set; get; }
        public string signed_file_path { set; get; } = string.Empty;
        public string signed_file_name { set; get; } = string.Empty;
        public string profile_pic_file_name { set; get; } = string.Empty;
        public string profile_pic_file_path { set; get; } = string.Empty;
        public string poa_giver_ids { set; get; } = string.Empty;
    }

    public class FetchMayatDTLForNIC
    {
        public int mayat_id { get; set; }
        public int usermasteruserid { get; set; }
        public string? applicationDTLapplicationid { get; set; }
        public int mutation_cts_no_id { get; set; }
        public string? mobileno { get; set; }
        public string? mobilenoverified { get; set; }
        public string? emailid { set; get; }
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
        public string? certificate_authority_code { get; set; }
        public string? certificate_authority_name { get; set; }
        public string? mrutyucert_no { get; set; }
        public string? mrutyu_certificate_date { get; set; }
        public string? mrutyu_certificate__name { get; set; }
        public string? mrutyu_certificate_path { get; set; }
        public string? is_name_same { get; set; }
        public string? reason { get; set; }
        public string? namecorrect_docname { get; set; }
        public string? namecorrect_docpath { get; set; }
        public string? address_proof_document_name { get; set; }
        public string? address_proof_document_path { get; set; }
        public string? signed_file_name { get; set; }
        public string? signed_file_path { get; set; }
        public string? createddatetime { get; set; }
        public bool isdeleted { set; get; }
        public string? deleteddate { set; get; }
        //Below are Probet fields -> Added on 31 Dec 25
        public string? probet_file_name { set; get; }
        public string? probet_file_path { set; get; }
        public bool isprobet { set; get; }
    }
    public class FetchBhadepattaInfoDTLForNIC
    {
        public int Info_id { get; set; }
        public int userid { get; set; }
        public string? applicationid { get; set; }
        public string? bhadepattaTenureYear { get; set; }
        public string? bhadepattaTenureMonth { get; set; }
        public string? bhadepattaFromDate { get; set; }
        public string? bhadepattaToDate { get; set; }
        public string? bhadepattaAmount { get; set; }
        public string? createdDateTime { get; set; }
        public string? deletedDateTime { get; set; }
        public bool? isDeleted { get; set; }
        public bool? leaseperiod { get; set; }
    }

    public class FetchErrorCorrectionDataForNIC
    {
        public int error_correction_id { get; set; }
        public int usermasteruserid { get; set; }
        public string? applicationdtlapplicationid { get; set; }
        public string? village_code { get; set; }
        public string? sub_property_no { get; set; }
        public string? city_servey_no { get; set; }
        public string? lr_property_id { get; set; }
        public string? milkat { get; set; }
        public string? namud { get; set; }
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
        public string? reason { get; set; }
        public string? address_type { get; set; }
        public string? emailid { get; set; }
        public string? mobileno { get; set; }
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
        public string? createddatetime { get; set; }
        public bool isdeleted { set; get; }
        public string? deleteddate { set; get; }
    }
    public class FetchNavatBadalDataForNIC
    {
        public int name_change_id { get; set; }
        public int usermasteruserid { get; set; }
        public string? applicationdtlapplicationid { get; set; }
        public string? village_code { get; set; }
        public string? subpropno { get; set; }
        public string? nabhu { get; set; }
        public string? lrpropertyuid { get; set; }
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
        public int updated_usertype { get; set; } = 0;
        public string? updated_usertypelabel { get; set; }
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
        public string? createddatetime { get; set; }
        public bool isdeleted { set; get; }
        public string? deleteddate { set; get; }
    }
    public class FetchHibanamaWitnessDataForNIC
    {
        public int witness_info_id { get; set; }
        public int usermasteruserid { get; set; }
        public string? applicationdtlapplicationid { get; set; }
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
        public string? mobilenoverified { get; set; }
        public string? emailid { set; get; }
        public string? emailidverified { set; get; }
        public string? createddatetime { get; set; }
        public bool isdeleted { set; get; }
        public string? deleteddate { set; get; }
    }
}
