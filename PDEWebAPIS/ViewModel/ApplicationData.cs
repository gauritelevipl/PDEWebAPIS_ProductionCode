using PDEWebAPIS.InputDataModel;
using PDEWebAPIS.Repository;

namespace PDEWebAPIS.ViewModel
{
    public class ApplicationData
    {
        public FetchUserData? registereduser { set; get; }
        public FetchApplicationDTLs? applicationDtl { get; set; }
        public List<FetchApplicantsData>? applicants { get; set; }
        public List<FetchMutationCTSNoData>? mutationCTSNoData { get; set; }
        public List<FetchDastInformationData>? dastInformation { get; set; } = null;
        public List<FetchCourtClaimInformationData>? courtClaimInformation { get; set; } = null;
        public List<POADTL>? poa { set; get; }
        //public List<FetchPOAForGiverData>? poAForGiver { get; set; }
        //public List<FetchPOAForTakerData>? poAForTaker { get; set; }

        /*  public List<FetchMayatDetailsData>? mayatData { get; set; }
        public List<FetchMrutuDakhalaDetailsData>? mrutuDakhalaDetailsData { get; set; }
        public List<FetchVarasNondDetailsData>? varasNondDetailsData { get; set; }*/
        public dynamic? Mutation { get; set; }
        /*public dynamic? MutationGiver { get; set; } 
        public dynamic? MutationTaker { get; set; }*/
        public List<FetchUploadedDocumentData>? uploadedDocument { get; set; }
        public List<FetchUploadedDocuments>? uploaded { get; set; }

        //Bhadepatta
        public FetchBhadepattaInfoData? fetchBhadepattaInfoData { get; set; }
    }

    public class FetchApplicationDTLs
    {
        public string? applicationid { get; set; }
        //public int userid { get; set; }
        public string? applicantIDs { set; get; }
        public string? applicationdate { get; set; }
        public string? applicationType { get; set; }
        public string? applicationTypeEng { get; set; }
        public string? MutationTypeCode { set; get; }
        public string? MutationTypeName { set; get; }
        public string? DistrictCode { set; get; }
        public string? DistrictName { set; get; }
        public string? DistrictNameEng { set; get; }
        public string? OfficeCode { set; get; }
        public string? OfficeName { set; get; }

        public bool do_you_have_power_of_attorney { set; get; }

        public bool Is_the_claim_pending_before_the_court { set; get; }

        public bool does_the_original_charter_info_apply { set; get; }

    }


    public class FetchMutationDataForGiver
    {
        public int? mutation_dtl_id { get; set; }
        public int? userid { get; set; }
        public string? applicationid { get; set; }
        //public UserDTLForKharediNond? userDetails { get; set; }
        public AddressDTLForKharediNond? address { get; set; }
        public string? fullNameInMarathi { set; get; }
        public string? fullNameInEng { set; get; }
        public string? mobileNo { set; get; }
    }

    public class FetchMutationDataForTaker
    {
        public int? mutation_dtl_id { get; set; }
        public int? userid { get; set; }
        public string? applicationid { get; set; }
        public string? userType { get; set; }
        public string? fullNameInMarathi { set; get; }
        public string? fullNameInEng { set; get; }
        public string? mobileNo { set; get; }
        public photoDetails? photo { get; set; }
        public isMHProperty? isMHProperty { get; set; }
        public dharakDetails? dharak { get; set; } = null;
        public AddressDTLForKharediNond? address { get; set; }
    }

    public class FetchUploadedDocumentData
    {
        public string? applicationID { set; get; }
        public string? documentID { set; get; }
        public string? documentTypeCode { set; get; }
        public string? documentType { set; get; }
        public string? city { set; get; }
        public string? nabhuno { set; get; }
        public string? docName { set; get; }
    }

    public class FetchUploadedDocuments
    {
        public string? applicationID { set; get; }
        public string? nabhuno { set; get; }
        public string? docName { get; set; }
    }
    public class DocumentDTLList
    {
        public string? nabhuno { set; get; }
        public string? documentName { set; get; }
    }
    public class MutationList
    {
        public string? type { get; set; }
        public dynamic? value { set; get; }
    }

    public class POADTL
    {
        public List<FetchPOAForGiverData>? poAForGiver { set; get; }
        public List<FetchPOAForTakerData>? poAForTaker { set; get; }

    }
}
