using System.Text.Json.Serialization;

namespace PDEWebAPIS.Model
{
    public class LgdApiResponse
    {
        public int? Status { get; set; }
        public object? Data { get; set; }
        public string? message { get; set; }
        //public object[] ServiceRequestInputs { get; set; }
    }

   

    public class EPICDistrict
    {
        public string? district_code { get; set; }
        public string? district_name { get; set; }
    }
    public class OfficeByDist
    {
        public string? office_code { get; set; }
        public string? office_name { get; set; }
        public string? office_english_name { get; set; }
    }

    public class VillageByOffice
    {
        public string? village_lgd_code { get; set; }
        public string? village_code { get; set; }
        public string? village_name { get; set; }
        public string? village_english_name { get; set; }
        public string? zone_code { get; set; }
        public string? amount { get; set; }
    }

    public class EPCIApplicationType
    {
        public string? application_code { get; set; }
        public string? application_type { get; set; }
    }
    public class EPCIMutationType
    {
        public string? mutation_code { get; set; }
        public string? mutation_type { get; set; }
    }

    public class EPCIDocListMutationtype
    {
        public string? document_code { get; set; }
        public string? document_name { get; set; }
    }
    public class EPCIDocument
    {
        public int document_code { get; set; }
        public string? document_name { get; set; }
        public bool? document_is_mandatory { get; set; }
    }


    public class EPCIapplicationTypeList
    {
        public string? applicant_category_code { get; set; }
        public string? applicant_category_type { get; set; }
    }
    public class RequestCTSDetails
    {
        public string? village_code { get; set; }
        public string? cts_no { get; set; }
    }
    public class EPCICTSNODetails
    {
        
       public string? village_code { get; set; }
       public string? cts_number { get; set; }
       public string? cts_puid { get; set; }
       public string? cts_numbe_area { get; set; }
       public string? cts_number_status { get; set; }
       public string? sub_property_number { get; set; }
       public string? tenure_of_cts_number { get; set; }
       
    }

    public class RequestOwnerNameInfo
    {
        public string? village_code { get; set; }
        public string? cts_no { get; set; }
        public string? subprop_no { get; set; }
    }
    public class EPCIOwnerNameInfo
    {
        public string? village_code { get; set; }
        public string? cts_number { get; set; }
        public string? mutation_srno { get; set; }
        public string? entry_date { get; set; }
        public string? entry_bracketed { get; set; }
        public string? owner_number { get; set; }
        public string? owner_name { get; set; }
        public string? first_name { get; set; }
        public string? middle_name { get; set; }
        public string? last_name { get; set; }
        public string? owner_bracketed { get; set; }

    }
    public class RequestOwnerDetails
    {
        public string? village_code { get; set; }
        public string? cts_no {get;set;}
        public string? mut_sr_no { get; set; }
        public string? owner_no { get; set; }
    }
    public class EPCIOwnerDetails
    {
       public string? village_code { get; set; }
        public string? cts_number { get; set; }
        public string? mutation_srno { get; set; }
        public string? entry_date { get; set; }
        public string? entry_bracketed { get; set; }
        public string? owner_number { get; set; }
        public string? owner_name { get; set; }
        public string? first_name { get; set; }
        public string? middle_name { get; set; }
        public string? last_name { get; set; }
        public string? nick_name { get; set; }
        public string? owner_bracketed { get; set; }
        public string? area_bracketed { get; set; }
        public string? email_id { get; set; }
        public string? owner_cell_number { get; set; }
        public string? pincode { get; set; }
        public string? owner_type { get; set; }
        public string? apk_code { get; set; }
        public string? apk_name { get; set; }
        public string? flat_or_house_number { get; set; }
        public string? building_number { get; set; }
        public string? road { get; set; }
        public string? city_or_village { get; set; }
        public string? taluka_name { get; set; }
        public string?  district_name { get; set; }
        public string? state_name { get; set; }
        public string?  gender_code { get; set; }
        public string? date_of_birth { get; set; }
        public string? owner_area { get; set; }
        public string? owner_area_bracketed { get; set; }
    }

    public class nameTitleList
    {
        public string? name_title_code { get; set; }
        public string? name_title { get; set; }
        public string? name_title_english { get; set; }
    }

    public class  EPCIFlatDetails
    {
        public string? village_code { get; set; }
        public string?  cts_number { get; set; }
        public string?  cts_puid { get; set; }
        public string?  cts_number_area { get; set; }
        public string?   cts_number_status { get; set; }
        public string?   sub_property_number { get; set; }
        public string?   mutation_srno { get; set; }
        public string?   entry_date { get; set; }
        public string?  apartment_serial_number { get; set; }
        public string?  floor_type_code { get; set; }
        public string?  floor_type_description { get; set; }
        public string?  floor_number { get; set; }
        public string? unit_type_code { get; set; }
        public string?  unit_type_name { get; set; }
        public string?  building_number { get; set; }
        public string? building_name { get; set; }
        public string? flat_number { get; set; }
        public string? build_up_area_of_flat { get; set; }
        public string? carpet_area_of_flat { get; set; }
        public string? sub_propperty_no { get; set; }

    }
    public class EPCICTSDetails
    {
        public string? village_code { get; set; }
        public string? cts_number { get; set; }
        public string? cts_puid { get; set; }
        public string? cts_numbe_area { get; set; }
        public string? cts_number_status { get; set; }
        public string? sub_property_number { get; set; }
    }

    public class EPCIULPINDetails
    {
        public string? ulpin { get; set; }
        public string? region_code { get; set; }
        public string? region_name { get; set; }
        public string? dist_code { get; set; }
        public string? dist_name { get; set; }
        public string? office_code { get; set; }
        public string? office_name { get; set; }
        public string? lgd_village_code { get; set; }
        public string? village_code { get; set; }
        public string? village_name { get; set; }
        public string? cts_no { get; set; }
    }

    public class EPCIPropertyDetails
    {
        public string? district_name { get; set; }
        public string? office_name { get; set; }
        public string? village_name { get; set; }
        public string? cts_no { get; set; }
        public string? cts_number { get; set; }
        public string? sub_cts_number { get; set; }
        public string? tenure { get; set; }
        public string? total_area { get; set; }
        public string? holder { get; set; }
        public string? other_rights { get; set; }
    }

    public class EPCIBojaInstituteList
    {
        public string? institute_code { get; set; }
        public string? institute_description { get; set; }
    }

    public class EPCIOwnerAccountTypeList
    {
        public string? account_type_code { get; set; }
        public string? account_type_description { get; set; }
    }

    public class EPCIAPKMasterList
    {
        public string? apk_code { get; set; }
        public string? apk_description { get; set; }
    }

    public class EPCIGenderList
    {
        public string? gender_code { get; set; }
        public string? gender_description { get; set; }
    }

    public class EPCIHolderRelationList
    {
        public string? relation_code { get; set; }
        public string? relation_name { get; set; }
    }

    public class EPCIOwnerStatusOrCategory
    {
        public string? owner_status_code { get; set; }
        public string? owner_status_description { get; set; }
    }

    public class EPCIDeathCertificateList
    {
        public string? certificate_authority_code { set; get; }
        public string? certificate_authority_name { get; set; }
    }

    public class EPCICaseTypeList
    {
        public int case_type { get; set; }
        public string? case_type_description { get; set; }
    }

    public class EPCIPOATypeList
    {
        public string? poa_type_code { get; set; }
        public string? poa_type_description { set; get; }
    }

    public class EPCISROOfficeList
    {
        public int sro_office_code { get; set; }
        public string? sro_office_name { get; set; }
    }
    public class EPCIFloorTypeList
    {
        public int floor_type { get; set; }
        public string? floor_desc { get; set; }
        public string? floor_order_by { get;set; }
    }
    public class EPCIUnitTypeList
    {
        public int unit_code_156 { get; set; }
        public string? unit_name_156 { get; set; }
    }

    //GW
    public class EPCIRegion
    {
        public int region_code { get; set; }
        public string? region_name { get; set; }
        public string? region_english_name { get; set; }
    }
    public class EPCIDistrictByRegionList
    {
        public int district_code { get; set; }
        public string? district_name { get; set; }
        public string? district_english_name { get; set; }
    }

    public class RequestvalidateMultipleMutationApplications
    {
        public string? district_code { get; set; }
        public string? office_code { get; set; }
        public string? village_code { get; set; }
        public string? cts_no { get; set; }
        public string? subprop_no { get; set; }
        public string? mutation_srno { get; set; }
        public string? owner_no { get; set; }
    }

    public class ResponsevalidateMultipleMutationApplications
    {
        [JsonPropertyName("Inward Number")]
        public string? InwardNumber { get; set; }
        [JsonPropertyName("Inward Date")]
        public string? InwardDate { get; set; }
        [JsonPropertyName("Mutation Number")]
        public string? MutationNumber { get; set; }
        [JsonPropertyName("Mutation Date")]
        public string? MutationDate { get; set; }
        [JsonPropertyName("Village Code")]
        public string? VillageCode { get; set; }
        [JsonPropertyName("CTS Number")]
        public string? CTSNumber { get; set; }
    }

    public class EPCISgetDocListMutationtype
    {
        public string? mut_type { get; set; }
        public string? mut_category { get; set; }
    }

    public class EPCISReasonForOwnerNameChangeList
    {
        public int? name_change_by_code { get; set; }
        public string? name_change_by_description { get; set; }
    }

    public class EPCISentryDetailsOfRegisteredMutationRequestData
    {
        public string? district_code { get; set; }
        public string? office_code { get; set; }
        public string? village_code { get; set; }
        public string? cts_no { get; set; }
    }
    public class EPCISentryDetailsOfRegisteredMutationResponseData
    {
        public string? var_village_code { get; set; }
        public string? var_cts_number { get; set; }
        public string? var_cts_puid { get; set; }
        public string? var_mutation_srno { get; set; }
        public string? var_entry_date { get; set; }
        public string? var_mutation_number { get; set; }
        public string? var_mutation_date { get; set; }
        public string? var_sro_office_name_marathi { get; set; }
        public string? var_sro_office_name_english { get; set; }
        public string? var_document_number { get; set; }
        public string? var_document_year { get; set; }
        public string? var_document_date { get; set; }
        public string? var_entry_details { get; set; }
        public string? var_owner_details { get; set; }
    }

    public class EPCISOrderGivenByAuthorityNamesResponseData
    {
        public int? order_given_by_code { get; set; }
        public string? order_given_by_description { get; set; }
        public int? sqequence_by { get; set; }
    }

    public class EPCISGetTenureList
    {
        public string? tenure_code { get; set; }
        public string? tenure_description { get; set; }
    }
    public class EPCISgetTenureRequestData
    {
        public string? village_code { get; set; }
        public string? cts_no { get; set; }
    }

    public class EPCISCorrectionData
    {
        public int? sr_no_180 { get; set; }
        public string? name_180 { get; set; }
    }
    public class EPCISgetDashboardMetricsRequestData
    {
        public string? type { get; set; }
        public string? code { get; set; }
    }
    public class EPCISgetDashboardMetricsResponse
    {
        public string? state_name { get; set; }
        public string? divisioncode { get; set; }
        public string? divisionname { get; set; }
        public string? districtcode { get; set; }
        public string? districtname { get; set; }
        public string? officecode { get; set; }
        public string? officename { get; set; }
        public string? total_inward { get; set; }
        public string? pending_at_ms_inward { get; set; }
        public string? pending_at_ctso_inward { get; set; }
        public string? disposed_inward { get; set; }
    }
}

