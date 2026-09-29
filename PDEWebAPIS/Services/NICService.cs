using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using PDEWebAPIS.CommonMethods;
using PDEWebAPIS.ContractRepo;
using PDEWebAPIS.Data;
using PDEWebAPIS.Helpers;
using PDEWebAPIS.InputDataModel;
using PDEWebAPIS.Model;
using PDEWebAPIS.Repository;
using PDEWebAPIS.ViewModel;
using PDEWebAPIS.ViewModel.ModelForNICData;
using System;
using System.Net.Http.Headers;
using System.Text;
using System.Transactions;
using static System.Net.Mime.MediaTypeNames;
namespace PDEWebAPIS.Services
{
    public class NICService
    {
        private AppDBContext _context;
        private AppDBContextR _contextR;
        private readonly ApplicationServices applicationServices;
        private readonly UserServices userServices;
        private readonly MutationServices mutationServices;
        private CommonFunctions commonFunctions = new CommonFunctions();
        private static readonly HttpClient client = new HttpClient();
        private readonly ILogger<TestFileUpload> _loggerTest;
        private readonly ILogger<UserServices> _loggeruserService;
        private readonly IMapper _mapper;
        public NICService(AppDBContext context, AppDBContextR contextR, ILogger<UserServices> loggerservice, ICommonRepository commonRepository, IMapper mapper)
        {
            _loggeruserService = loggerservice;
            _context = context;
            _contextR = contextR;
            //_loggerTest = loggerTest;
            applicationServices = new ApplicationServices(_context, contextR,  _loggeruserService);
            userServices = new UserServices(context, contextR, loggerservice);
            mutationServices = new MutationServices(context, contextR,  loggerservice, commonRepository, mapper);
        }

        public ApplicationDTL FetchApplicationData(string applicationid)
        {
            try
            {
                ApplicationDTL applicationDTL = new ApplicationDTL();
                applicationDTL = _contextR.applicationDTL.Include(user => user.userMaster).Include(app => app.applicationTypeMaster).Where(data => data.applicationid!.Equals(applicationid)).FirstOrDefault()!;
                return applicationDTL;
            }
            catch (Exception ex)
            {
                throw new HandleException(ex.Message.ToString());
            }
        }
        public FetchUserDataForNIC FetchUserData(int UserID)
        {
            try
            {
                FetchUserDataForNIC fetchUserData = new FetchUserDataForNIC();
                UserMaster userData = new UserMaster();
                MethodForFileUpload methodForFile = new MethodForFileUpload();
                userData = _contextR.userMasters.Include(i => i.PropertyTypeMaster).Where(data => data.userid.Equals(UserID)).FirstOrDefault()!;

                fetchUserData.userid = UserID;
                fetchUserData.profile_pic_file_name = userData.profile_pic_file_name;
                fetchUserData.profile_pic_file_path = string.IsNullOrEmpty(userData.profile_pic_file_path) ? "NA" : userData.profile_pic_file_path;

                fetchUserData.address_proof_document_name = string.IsNullOrEmpty(userData.address_proof_document_name) ? "NA" : userData.address_proof_document_name;
                fetchUserData.address_proof_document_path = string.IsNullOrEmpty(userData.address_proof_document_path) ? "NA" : userData.address_proof_document_path;

                fetchUserData.signed_file_name = string.IsNullOrEmpty(userData.signed_file_name) ? "NA" : userData.signed_file_name;
                fetchUserData.signed_file_path = string.IsNullOrEmpty(userData.signed_file_path) ? "NA" : userData.signed_file_path;

                fetchUserData.usertype_code = userData.usertype_code;
                fetchUserData.usertype = userData.usertype;
                fetchUserData.mobileno = userData.mobileno;
                fetchUserData.mobilenoverified = userData.mobilenoverified;
                fetchUserData.emailid = userData.emailid;
                fetchUserData.emailidverified = userData.emailidverified;
                fetchUserData.securitypin = userData.securitypin;
                fetchUserData.prefixcode_eng = commonFunctions.ReplaceNA(userData.prefixcode_eng!);
                fetchUserData.prefix_in_eng = commonFunctions.ReplaceNA(userData.prefix_in_eng!);
                fetchUserData.fname_in_eng = commonFunctions.ReplaceNA(userData.fname_in_eng!);
                fetchUserData.mname_in_eng = commonFunctions.ReplaceNA(userData.mname_in_eng!);
                fetchUserData.lname_in_eng = commonFunctions.ReplaceNA(userData.lname_in_eng!);
                fetchUserData.prefixcode_marathi = commonFunctions.ReplaceNA(userData.prefixcode_marathi!);
                fetchUserData.prefix_in_marathi = commonFunctions.ReplaceNA(userData.prefix_in_marathi!);
                fetchUserData.fname_in_marathi = commonFunctions.ReplaceNA(userData.fname_in_marathi!);
                fetchUserData.mname_in_marathi = commonFunctions.ReplaceNA(userData.mname_in_marathi!);
                fetchUserData.lname_in_marathi = commonFunctions.ReplaceNA(userData.lname_in_marathi!);
                fetchUserData.company_name_in_marathi = commonFunctions.ReplaceNA(userData.company_name_in_marathi);
                fetchUserData.company_name_in_eng = commonFunctions.ReplaceNA(userData.company_name_in_eng);
                fetchUserData.username = commonFunctions.ReplaceNA(userData.username);
                fetchUserData.address_type = userData.address_type;
                fetchUserData.address = commonFunctions.ReplaceNA(userData.address);
                fetchUserData.state = userData.state;
                fetchUserData.district = userData.district;
                fetchUserData.taluka = userData.taluka;
                fetchUserData.city = userData.city;
                fetchUserData.flatno_plotno = userData.flatno_plotno;
                fetchUserData.societyname = userData.societyname;
                fetchUserData.mainstreet = userData.mainstreet;
                fetchUserData.landmark = userData.landmark;
                fetchUserData.locality = userData.locality;
                fetchUserData.pincode = string.IsNullOrEmpty(userData.pincode) ? "NA" : userData.pincode;
                fetchUserData.postofficename = userData.postofficename;
                fetchUserData.address_proof_document_name = commonFunctions.ReplaceNA(userData.address_proof_document_name)!;
                fetchUserData.address_proof_document_path = commonFunctions.ReplaceNA(userData.address_proof_document_path);
                fetchUserData.owner_of_property_in_maharashtra = userData.owner_of_property_in_maharashtra;
                fetchUserData.propertytypemasterpropertytypeid = userData.PropertyTypeMaster.propertytypeid;
                fetchUserData.property_district_code = commonFunctions.ReplaceNA(userData.property_district_code!);
                fetchUserData.property_district_name = commonFunctions.ReplaceNA(userData.property_district_name!);
                fetchUserData.property_taluka_code = commonFunctions.ReplaceNA(userData.property_taluka_code!);
                fetchUserData.property_taluka_name = commonFunctions.ReplaceNA(userData.property_taluka_name!);
                fetchUserData.property_village_code = commonFunctions.ReplaceNA(userData.property_village_code!);
                fetchUserData.property_village_name = commonFunctions.ReplaceNA(userData.property_village_name!);
                fetchUserData.khateno = commonFunctions.ReplaceNA(userData.khateno!);
                fetchUserData.city_servey_no = commonFunctions.ReplaceNA(userData.city_servey_no!);
                fetchUserData.ulpin = commonFunctions.ReplaceNA(userData.ulpin!);
                fetchUserData.profile_pic_file_name = commonFunctions.ReplaceNA(userData.profile_pic_file_name!);
                fetchUserData.profile_pic_file_path = commonFunctions.ReplaceNA(userData.profile_pic_file_path!);
                fetchUserData.signed_file_name = commonFunctions.ReplaceNA(userData.signed_file_name!);
                fetchUserData.signed_file_path = commonFunctions.ReplaceNA(fetchUserData.signed_file_path!);
                TimeZoneInfo INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
                DateTime indianTime = TimeZoneInfo.ConvertTimeFromUtc(Convert.ToDateTime(userData.createddatetime.ToString()), INDIAN_ZONE);
                fetchUserData.createddatetime = indianTime.ToString("yyyy-MM-dd");
                fetchUserData.web_token = "";
                fetchUserData.mobile_token = "";
                return fetchUserData;
            }
            catch (Exception ex)
            {
                throw new HandleException(ex.Message.ToString());
            }

        }

        public FetchApplicationDTLsForNIC GetApplicationDtls(ApplicationDTL application1)
        {
            FetchApplicationDTLsForNIC application = new FetchApplicationDTLsForNIC();
            application.applicationid = application1.applicationid;
            application.usermasteruserid = application1.userMaster!.userid;
            application.applicationtypemasterapplicationtypeid = (application1.applicationTypeMaster!.applicationtypeid == 1) ? "Y" : "N";
            application.mutation_type_code = application1.mutation_type_code;
            application.mutation_type_name = application1.mutation_type_name;
            application.district_code = application1.district_code;
            application.district_name_in_marathi = commonFunctions.ReplaceNA(application1.district_name_in_marathi!);
            application.district_name_in_english = commonFunctions.ReplaceNA(application1.district_name_in_english!);
            application.office_code = application1.office_code;
            application.office_name = application1.office_name;
            application.do_you_have_power_of_attorney = application1.do_you_have_power_of_attorney;
            application.is_the_claim_pending_before_the_court = application1.Is_the_claim_pending_before_the_court;
            application.does_the_original_charter_info_apply = application1.does_the_original_charter_info_apply;
            application.applicantids = application1.applicantIDs;
            application.mutation_cts_nos = application1.mutation_cts_nos;
            application.dastids = application1.dastIDs;
            application.courtclaimids = application1.courtClaimIDs;
            application.powerofattorneyids = application1.powerOfAttorneyIDs;
            application.uploadeddocids = application1.uploadedDocIDs;
            application.mutationgiverids = application1.mutationgiverIDs;
            application.mutationtakerids = application1.mutationtakerIDs;
            application.mayatids = application1.mayatIDs;
            application.varasids = application1.varasIDS;
            application.inwardno = commonFunctions.ReplaceNA(application1.inwardno!);
            application.status = application1.status;
            application.self_declaration_doc_name = string.IsNullOrEmpty(application1.self_declaration_doc_name) ? commonFunctions.ReplaceNA("NA") : application1.self_declaration_doc_name;
            application.self_declaration_doc_name = commonFunctions.ReplaceNA(application.self_declaration_doc_name);
            application.self_declaration_doc_path = string.IsNullOrEmpty(application1.self_declaration_doc_path) ? commonFunctions.ReplaceNA("NA") : application1.self_declaration_doc_path;
            application.self_declaration_doc_path = commonFunctions.ReplaceNA(application.self_declaration_doc_path);
            application.createddatetime = application1.createddatetime.ToString("yyyy-MM-dd");
            application.isdeleted = application1.isDeleted;
            application.deleteddate = application1.deleteddate.ToString("yyyy-MM-dd");
            application.is_sentnic = application1.is_sentnic;
            application.sentnic_attempts = application1.sentnic_attempts;
            return application;
        }

        public FetchApplicantsDataForNIC FetchApplicantData(int applicantid)
        {
            try
            {
                ApplicantMaster applicant = new ApplicantMaster();
                applicant = _contextR.applicantMasters.Include(i => i.PropertyTypeMaster).Include(u => u.userMaster).Where(data => data.applicantid.Equals(applicantid) && data.isDeleted == false).FirstOrDefault()!;
                FetchApplicantsDataForNIC fetchData = new FetchApplicantsDataForNIC();
                if (applicant != null)
                {
                    fetchData.applicantid = applicantid;
                    fetchData.usermasteruserid = applicant.userMaster!.userid;
                    fetchData.applicationdtlapplicationid = applicant.applicationDTL!.applicationid;
                    fetchData.propertytypemasterpropertytypeid = applicant.PropertyTypeMaster!.propertytypeid;
                    fetchData.usertype_code = applicant.usertype_code;
                    fetchData.usertype = applicant.usertype;
                    fetchData.mobileno = applicant.mobileno;
                    fetchData.mobilenoverified = applicant.mobilenoverified;
                    fetchData.emailid = applicant.emailid;
                    fetchData.emailidverified = applicant.emailidverified;
                    fetchData.securitypin = applicant.securitypin;
                    fetchData.prefix_in_eng = commonFunctions.ReplaceNA(applicant.prefix_in_eng!);
                    fetchData.fname_in_eng = commonFunctions.ReplaceNA(applicant.fname_in_eng!);
                    fetchData.mname_in_eng = commonFunctions.ReplaceNA(applicant.mname_in_eng!);
                    fetchData.lname_in_eng = commonFunctions.ReplaceNA(applicant.lname_in_eng!);
                    fetchData.prefix_in_marathi = commonFunctions.ReplaceNA(applicant.prefix_in_marathi!);
                    fetchData.fname_in_marathi = commonFunctions.ReplaceNA(applicant.fname_in_marathi!);
                    fetchData.mname_in_marathi = commonFunctions.ReplaceNA(applicant.mname_in_marathi!);
                    fetchData.lname_in_marathi = commonFunctions.ReplaceNA(applicant.lname_in_marathi!);
                    fetchData.company_name_in_marathi = commonFunctions.ReplaceNA(applicant.company_name_in_marathi!);
                    fetchData.company_name_in_eng = commonFunctions.ReplaceNA(applicant.company_name_in_eng!);
                    fetchData.username = commonFunctions.ReplaceNA(applicant.username!);
                    fetchData.address_type = applicant.address_type;
                    fetchData.address = commonFunctions.ReplaceNA(applicant.address!);
                    fetchData.state = applicant.state;
                    fetchData.district = applicant.district;
                    fetchData.taluka = applicant.taluka;
                    fetchData.city = applicant.city;
                    fetchData.flatno_plotno = applicant.flatno_plotno;
                    fetchData.societyname = applicant.societyname;
                    fetchData.mainstreet = applicant.mainstreet;
                    fetchData.landmark = applicant.landmark;
                    fetchData.locality = applicant.locality;
                    fetchData.pincode = string.IsNullOrEmpty(applicant.pincode) ? "NA" : applicant.pincode;
                    fetchData.postofficename = applicant.postofficename;
                    fetchData.address_proof_document_name = string.IsNullOrEmpty(applicant.address_proof_document_name) ? "NA" : applicant.address_proof_document_name;
                    fetchData.address_proof_document_path = string.IsNullOrEmpty(applicant.address_proof_document_path) ? "NA" : applicant.address_proof_document_path;
                    fetchData.owner_of_property_in_maharashtra = applicant.owner_of_property_in_maharashtra;
                    fetchData.khateno = commonFunctions.ReplaceNA(applicant.khateno!);
                    fetchData.city_servey_no = commonFunctions.ReplaceNA(applicant.city_servey_no!);
                    fetchData.ulpin = commonFunctions.ReplaceNA(applicant.ulpin!);
                    fetchData.property_district_code = commonFunctions.ReplaceNA(applicant.property_district_code!);
                    fetchData.property_district_name = commonFunctions.ReplaceNA(applicant.property_district_name!);
                    fetchData.property_taluka_code = commonFunctions.ReplaceNA(applicant.property_taluka_code!);
                    fetchData.property_taluka_name = commonFunctions.ReplaceNA(applicant.property_taluka_name!);
                    fetchData.property_village_code = commonFunctions.ReplaceNA(applicant.property_village_code!);
                    fetchData.property_village_name = commonFunctions.ReplaceNA(applicant.property_village_name!);
                    fetchData.profile_pic_file_name = string.IsNullOrEmpty(applicant.profile_pic_file_name) ? "NA" : applicant.profile_pic_file_name;
                    fetchData.profile_pic_file_path = string.IsNullOrEmpty(applicant.profile_pic_file_path) ? "NA" : applicant.profile_pic_file_path;
                    fetchData.signed_file_name = string.IsNullOrEmpty(applicant.signed_file_name) ? "NA" : applicant.signed_file_name;
                    fetchData.signed_file_path = string.IsNullOrEmpty(applicant.signed_file_path) ? "NA" : applicant.signed_file_path;
                    fetchData.profile_pic_file_name = commonFunctions.ReplaceNA(fetchData.profile_pic_file_name!);
                    fetchData.profile_pic_file_path = commonFunctions.ReplaceNA(fetchData.profile_pic_file_path!);
                    fetchData.signed_file_name = commonFunctions.ReplaceNA(fetchData.signed_file_name!);
                    fetchData.signed_file_path = commonFunctions.ReplaceNA(fetchData.signed_file_path!);

                    TimeZoneInfo INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
                    DateTime indianTime = TimeZoneInfo.ConvertTimeFromUtc(Convert.ToDateTime(applicant.createddatetime.ToString()), INDIAN_ZONE);
                    fetchData.createddatetime = indianTime.ToString("yyyy-MM-dd");
                    fetchData.isdeleted = applicant.isDeleted;
                    fetchData.deleteddate = applicant.deleteddate.ToString("yyyy-MM-dd");
                }
                else
                {
                    fetchData = null!;
                }
                return fetchData;
            }
            catch (Exception ex)
            {
                throw new HandleException(ex.Message.ToString());
            }

        }

        public FetchMutationCTSNoDataForNIC FetchMutationCTSData(int mutationCTSNOId)
        {
            try
            {
                MutationCTSNoDTL mutationCTSNoDTL = new MutationCTSNoDTL();
                mutationCTSNoDTL = _contextR.mutationCTSNoDTLs.Include(app => app.applicationDTL).Where(data => data.mutation_cts_no_id.Equals(mutationCTSNOId) && data.isDeleted == false).FirstOrDefault()!;
                FetchMutationCTSNoDataForNIC fetchData = new FetchMutationCTSNoDataForNIC();
                if (mutationCTSNoDTL != null)
                {
                    fetchData.mutation_cts_no_id = mutationCTSNoDTL.mutation_cts_no_id;
                    fetchData.usermasteruserid = mutationCTSNoDTL.userMaster!.userid;
                    fetchData.applicationdtlapplicationid = mutationCTSNoDTL.applicationDTL!.applicationid;
                    fetchData.what_is_mentioned_in_the_doc = mutationCTSNoDTL.what_is_mentioned_in_the_doc;
                    fetchData.village_or_peth_code = mutationCTSNoDTL.village_or_peth_code;
                    fetchData.village_or_peth_name = mutationCTSNoDTL.village_or_peth_name;
                    fetchData.village_english_name = mutationCTSNoDTL.village_english_name;
                    fetchData.village_lgd_code = mutationCTSNoDTL.village_lgd_code;
                    fetchData.zone_code = mutationCTSNoDTL.zone_code;
                    fetchData.amount = mutationCTSNoDTL.amount;
                    fetchData.mutation_modification_type = mutationCTSNoDTL.mutation_modification_type;
                    fetchData.city_servey_no_mentioned_in_application = mutationCTSNoDTL.city_servey_no_mentioned_in_application;
                    fetchData.servey_no = commonFunctions.ReplaceNA(mutationCTSNoDTL.servey_no!);
                    fetchData.selected_city_servey_no = mutationCTSNoDTL.selected_city_servey_no;
                    fetchData.lr_property_uid = mutationCTSNoDTL.lr_property_uid;
                    fetchData.application_income_type = mutationCTSNoDTL.application_income_type;
                    fetchData.city_servey_area_in_sq_m = mutationCTSNoDTL.city_servey_area_in_sq_m;
                    fetchData.building_name = commonFunctions.ReplaceNA(mutationCTSNoDTL.building_name!);
                    fetchData.floor_type = mutationCTSNoDTL.floor_type;
                    fetchData.floor_desc = commonFunctions.ReplaceNA(mutationCTSNoDTL.floor_desc!);
                    fetchData.floor_order_by = mutationCTSNoDTL.floor_order_by;
                    fetchData.floor_no = commonFunctions.ReplaceNA(mutationCTSNoDTL.floor_no!);
                    fetchData.unit_code_156 = mutationCTSNoDTL.unit_code_156;
                    fetchData.unit_name_156 = commonFunctions.ReplaceNA(mutationCTSNoDTL.unit_name_156!);
                    fetchData.unit_no = commonFunctions.ReplaceNA(mutationCTSNoDTL.unit_no!);
                    fetchData.buildup_area_in_sq_m = commonFunctions.ReplaceNA(mutationCTSNoDTL.buildup_area_in_sq_m!);
                    fetchData.carpet_area_in_sq_m = commonFunctions.ReplaceNA(mutationCTSNoDTL.carpet_area_in_sq_m!);
                    fetchData.terrace_area_in_sq_m = commonFunctions.ReplaceNA(mutationCTSNoDTL.terrace_area_in_sq_m!);
                    fetchData.parking_no = commonFunctions.ReplaceNA(mutationCTSNoDTL.parking_no!);
                    fetchData.parking_area_in_sq_m = commonFunctions.ReplaceNA(mutationCTSNoDTL.parking_area_in_sq_m!);
                    fetchData.shares_in_percent = commonFunctions.ReplaceNA(mutationCTSNoDTL.shares_in_percent!);
                    fetchData.nic_flat_details = commonFunctions.ReplaceNA(mutationCTSNoDTL.nic_flat_details!);
                    fetchData.flat_bulit_up_area = commonFunctions.ReplaceNA(mutationCTSNoDTL.flat_bulit_up_area!);
                    fetchData.sub_property_id = mutationCTSNoDTL.sub_property_id;
                    TimeZoneInfo INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
                    DateTime indianTime = TimeZoneInfo.ConvertTimeFromUtc(Convert.ToDateTime(mutationCTSNoDTL.createddatetime.ToString()), INDIAN_ZONE);
                    fetchData.createddatetime = indianTime.ToString("yyyy-MM-dd");
                    fetchData.isdeleted = mutationCTSNoDTL.isDeleted;
                    fetchData.deleteddate = mutationCTSNoDTL.deleteddate.ToString("yyyy-MM-dd");
                }
                else
                {
                    fetchData = null!;
                }
                return fetchData;
            }
            catch (Exception ex)
            {
                throw new HandleException(ex.Message.ToString());
            }
        }

        public FetchDastInformationDataForNIC FetchDastInformationData(int dastNo)
        {
            try
            {
                DastInformation dastInformation = new DastInformation();
                dastInformation = _contextR.dastInformation.Include(i => i.userMaster).Include(app => app.applicationDTL).Where(data => data.dast_id.Equals(dastNo) && data.isDeleted == false).FirstOrDefault()!;

                FetchDastInformationDataForNIC fetchData = new FetchDastInformationDataForNIC();
                if (dastInformation != null)
                {
                    CommonFunctions commonFunctions = new CommonFunctions();
                    fetchData.dast_id = dastInformation.dast_id;
                    fetchData.usermasteruserid = dastInformation.userMaster!.userid;
                    fetchData.applicationdtlapplicationid = dastInformation.applicationDTL!.applicationid;
                    fetchData.dasttype = dastInformation.dastType;
                    fetchData.division_code = dastInformation.division_code.ToString();
                    fetchData.division_name = dastInformation.division_name;
                    fetchData.districtcode = dastInformation.districtCode;
                    fetchData.districtname = dastInformation.districtName;
                    fetchData.office_of_the_second_registrar_code = dastInformation.office_of_the_second_registrar_code;
                    fetchData.office_of_the_second_registrar_name = dastInformation.office_of_the_second_registrar_name;
                    fetchData.registered_dast_no = dastInformation.registered_dast_no;
                    fetchData.registered_dast_date = commonFunctions.ConvertStringToDate(dastInformation.registered_dast_date!);
                    fetchData.registered_dast_year = dastInformation.registered_dast_year;
                    fetchData.dastnabhu = dastInformation.dastNabhu;
                    fetchData.remarks = dastInformation.remarks;
                    fetchData.isdastverified = dastInformation.isDastVerified;
                    fetchData.verifieddastdata = dastInformation.verifiedDastData;
                    TimeZoneInfo INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
                    DateTime indianTime = TimeZoneInfo.ConvertTimeFromUtc(Convert.ToDateTime(dastInformation.createddatetime.ToString()), INDIAN_ZONE);
                    fetchData.createddatetime = indianTime.ToString("yyyy-MM-dd");
                    fetchData.isdeleted = dastInformation.isDeleted;
                    fetchData.deleteddate = dastInformation.deleteddate.ToString("yyyy-MM-dd");
                }
                else
                {
                    fetchData = null!;
                }
                return fetchData;
            }
            catch (Exception ex)
            {
                throw new HandleException(ex.Message.ToString());
            }
        }

        public FetchCourtClaimInformationDataForNIC FetchCourtClaimInformation(int courtClaimID)
        {
            try
            {
                CourtClaimInformation courtClaimInformation = new CourtClaimInformation();
                courtClaimInformation = _contextR.courtClaimInformation.Include(i => i.userMaster).Include(app => app.applicationDTL).Where(data => data.court_claim_id.Equals(courtClaimID) && data.isDeleted == false).FirstOrDefault()!;
                FetchCourtClaimInformationDataForNIC fetchData = new FetchCourtClaimInformationDataForNIC();
                if (courtClaimInformation != null)
                {
                    fetchData.court_claim_id = courtClaimInformation.court_claim_id;
                    fetchData.usermasteruserid = courtClaimInformation.userMaster!.userid;
                    fetchData.applicationdtlapplicationid = courtClaimInformation.applicationDTL!.applicationid;
                    fetchData.court_case_code = courtClaimInformation.court_case_code;
                    fetchData.court_case_name = courtClaimInformation.court_case_name;
                    fetchData.court_case_type_code = courtClaimInformation.court_case_type_code;
                    fetchData.court_case_type_name = courtClaimInformation.court_case_type_name;
                    fetchData.lr_property_uid = courtClaimInformation.lr_property_uid;
                    fetchData.city_servey_no = courtClaimInformation.city_servey_no;
                    fetchData.order_details = courtClaimInformation.order_details;
                    fetchData.stay_order = courtClaimInformation.stay_order;
                    fetchData.sub_property_no = courtClaimInformation.sub_property_no;
                    TimeZoneInfo INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
                    DateTime indianTime = TimeZoneInfo.ConvertTimeFromUtc(Convert.ToDateTime(courtClaimInformation.createddatetime.ToString()), INDIAN_ZONE);
                    fetchData.createddatetime = indianTime.ToString("yyyy-MM-dd");
                    fetchData.isdeleted = courtClaimInformation.isDeleted;
                    fetchData.deleteddate = courtClaimInformation.deleteddate.ToString("yyyy-MM-dd");
                }
                else
                {
                    fetchData = null!;
                }
                return fetchData;
            }
            catch (Exception ex)
            {
                throw new HandleException(ex.Message.ToString());
            }
        }

        //public FetchPOAForGiverDataForNIC FetchPowerOfAttorneyInfoForGiver(int powerOfAttorneyID)
        //{
        //    try
        //    {
        //        PowerOfAttorneyInformation powerOfAttorneyInformation = new PowerOfAttorneyInformation();
        //        powerOfAttorneyInformation = _context.powerOfAttorneyInformation.Include(i => i.userMaster).Include(app => app.applicationDTL).Where(data => data.power_of_attorney_id.Equals(powerOfAttorneyID) && data.is_taker == false && data.isDeleted == false).FirstOrDefault()!;
        //        FetchPOAForGiverDataForNIC fetchData = new FetchPOAForGiverDataForNIC();
        //        if (powerOfAttorneyInformation != null)
        //        {
        //            fetchData.power_of_attorney_id = powerOfAttorneyInformation.power_of_attorney_id;
        //            fetchData.userid = powerOfAttorneyInformation.userMaster!.userid;
        //            fetchData.applicationid = powerOfAttorneyInformation.applicationDTL!.applicationid;
        //            fetchData.power_of_attorney_code = powerOfAttorneyInformation.power_of_attorney_code;
        //            fetchData.is_taker = powerOfAttorneyInformation.is_taker;
        //            fetchData.usertype_code = powerOfAttorneyInformation.usertype_code;
        //            fetchData.usertype = commonFunctions.ReplaceNA(powerOfAttorneyInformation.usertype!);
        //            fetchData.mutation_id = powerOfAttorneyInformation.mutation_id;
        //            fetchData.mobileno = powerOfAttorneyInformation.mobileno;
        //            fetchData.mobilenoverified = powerOfAttorneyInformation.mobilenoverified;
        //            fetchData.emailid = commonFunctions.ReplaceNA(powerOfAttorneyInformation.emailid!);
        //            fetchData.emailidverified = commonFunctions.ReplaceNA(powerOfAttorneyInformation.emailidverified!);
        //            fetchData.prefixcode_marathi = commonFunctions.ReplaceNA(powerOfAttorneyInformation.prefixcode_marathi!);
        //            fetchData.prefix_in_marathi = commonFunctions.ReplaceNA(powerOfAttorneyInformation.prefix_in_marathi!);
        //            fetchData.fname_in_marathi = commonFunctions.ReplaceNA(powerOfAttorneyInformation.fname_in_marathi!);
        //            fetchData.mname_in_marathi = commonFunctions.ReplaceNA(powerOfAttorneyInformation.mname_in_marathi!);
        //            fetchData.lname_in_marathi = commonFunctions.ReplaceNA(powerOfAttorneyInformation.lname_in_marathi!);
        //            fetchData.prefixcode_eng = commonFunctions.ReplaceNA(powerOfAttorneyInformation.prefixcode_eng!);
        //            fetchData.prefix_in_eng = commonFunctions.ReplaceNA(powerOfAttorneyInformation.prefix_in_eng!);
        //            fetchData.fname_in_eng = commonFunctions.ReplaceNA(powerOfAttorneyInformation.fname_in_eng!);
        //            fetchData.mname_in_eng = commonFunctions.ReplaceNA(powerOfAttorneyInformation.mname_in_eng!);
        //            fetchData.lname_in_eng = commonFunctions.ReplaceNA(powerOfAttorneyInformation.lname_in_eng!);
        //            fetchData.company_name_in_marathi = commonFunctions.ReplaceNA(powerOfAttorneyInformation.company_name_in_marathi!);
        //            fetchData.company_name_in_eng = commonFunctions.ReplaceNA(powerOfAttorneyInformation.company_name_in_eng!);
        //            fetchData.username = powerOfAttorneyInformation.username;
        //            fetchData.alias_name = commonFunctions.ReplaceNA(powerOfAttorneyInformation.alias_name!);
        //            fetchData.gender_code = commonFunctions.ReplaceNA(powerOfAttorneyInformation.gender_code!);
        //            fetchData.gender_description = commonFunctions.ReplaceNA(powerOfAttorneyInformation.gender_description!);
        //            fetchData.dob = commonFunctions.ReplaceNA(powerOfAttorneyInformation.dob!);
        //            fetchData.mother_name_in_marathi = commonFunctions.ReplaceNA(powerOfAttorneyInformation.mother_name_in_marathi!);
        //            fetchData.mother_name_in_eng = commonFunctions.ReplaceNA(powerOfAttorneyInformation.mother_name_in_eng!);
        //            fetchData.address_type = powerOfAttorneyInformation.address_type;
        //            fetchData.address = commonFunctions.ReplaceNA(powerOfAttorneyInformation.address!);
        //            fetchData.state = powerOfAttorneyInformation.state;
        //            fetchData.district = powerOfAttorneyInformation.district;
        //            fetchData.taluka = powerOfAttorneyInformation.taluka;
        //            fetchData.city = powerOfAttorneyInformation.city;
        //            fetchData.flatno_plotno = powerOfAttorneyInformation.flatno_plotno;
        //            fetchData.societyname = powerOfAttorneyInformation.societyname;
        //            fetchData.mainstreet = powerOfAttorneyInformation.mainstreet;
        //            fetchData.landmark = powerOfAttorneyInformation.landmark;
        //            fetchData.locality = powerOfAttorneyInformation.locality;
        //            fetchData.pincode = powerOfAttorneyInformation.pincode;
        //            fetchData.postofficename = powerOfAttorneyInformation.postofficename;
        //            fetchData.address_proof_document_name = string.IsNullOrEmpty(powerOfAttorneyInformation.address_proof_document_name) ? "NA" : powerOfAttorneyInformation.address_proof_document_name;
        //            fetchData.address_proof_document_path = string.IsNullOrEmpty(powerOfAttorneyInformation.address_proof_document_path) ? "NA" : powerOfAttorneyInformation.address_proof_document_path;
        //            fetchData.city_servey_no = powerOfAttorneyInformation.city_servey_no;
        //            fetchData.lr_property_id = powerOfAttorneyInformation.lr_property_id;
        //            fetchData.sub_property_no = powerOfAttorneyInformation.sub_property_no;
        //            fetchData.owner_of_property_in_maharashtra = powerOfAttorneyInformation.owner_of_property_in_maharashtra;
        //            //fetchData.propertytypeid = string.IsNullOrEmpty(powerOfAttorneyInformation.propertyType)?"0": powerOfAttorneyInformation.propertyType.propertytypeid;
        //            fetchData.property_district_code = commonFunctions.ReplaceNA(powerOfAttorneyInformation.property_district_code!);
        //            fetchData.property_district_name_in_marathi = commonFunctions.ReplaceNA(powerOfAttorneyInformation.property_district_name_in_marathi!);
        //            fetchData.property_district_name_in_english = commonFunctions.ReplaceNA(powerOfAttorneyInformation.property_district_name_in_english!);
        //            fetchData.property_taluka_code = commonFunctions.ReplaceNA(powerOfAttorneyInformation.property_taluka_code!);
        //            fetchData.property_taluka_name = commonFunctions.ReplaceNA(powerOfAttorneyInformation.property_taluka_name!);
        //            fetchData.property_city_code = commonFunctions.ReplaceNA(powerOfAttorneyInformation.property_city_code!);
        //            fetchData.property_city_name = commonFunctions.ReplaceNA(powerOfAttorneyInformation.property_city_name!);
        //            fetchData.khateno = commonFunctions.ReplaceNA(powerOfAttorneyInformation.khateno!);
        //            fetchData.ulpin = commonFunctions.ReplaceNA(powerOfAttorneyInformation.ulpin!);
        //            fetchData.khata_type_code = commonFunctions.ReplaceNA(powerOfAttorneyInformation.khata_type_code!);
        //            fetchData.khata_type_name = commonFunctions.ReplaceNA(powerOfAttorneyInformation.khata_type_name!);
        //            fetchData.owner_status_code = commonFunctions.ReplaceNA(powerOfAttorneyInformation.owner_status_code!);
        //            fetchData.owner_status_description = commonFunctions.ReplaceNA(powerOfAttorneyInformation.owner_status_description!);
        //            fetchData.attornytype_code = powerOfAttorneyInformation.attornytype_code;
        //            fetchData.attornytype_desc = commonFunctions.ReplaceNA(powerOfAttorneyInformation.attornytype_desc!);
        //            fetchData.land_buy_area = commonFunctions.ReplaceNA(powerOfAttorneyInformation.landBuyArea!);
        //            fetchData.is_poa_is_partof_dast = commonFunctions.ReplaceNA(powerOfAttorneyInformation.isPOAisPartofDast!);
        //            fetchData.is_decleration_involved_in_poa = commonFunctions.ReplaceNA(powerOfAttorneyInformation.isDeclerationInvolvedInPOA!);
        //            fetchData.is_poa_permanant = commonFunctions.ReplaceNA(powerOfAttorneyInformation.isPOAPermanant!);
        //            fetchData.is_transfer_rights = commonFunctions.ReplaceNA(powerOfAttorneyInformation.isTransferRights!);
        //            fetchData.dast_no = commonFunctions.ReplaceNA(powerOfAttorneyInformation.dast_no!);
        //            fetchData.dast_no_date = commonFunctions.ReplaceNA(powerOfAttorneyInformation.dast_no_date!);
        //            fetchData.dast_no_year = commonFunctions.ReplaceNA(powerOfAttorneyInformation.dast_no_year!);
        //            fetchData.is_dast_verified = powerOfAttorneyInformation.isDastVerified;
        //            fetchData.verifieddast_data = commonFunctions.ReplaceNA(powerOfAttorneyInformation.verifieddastData!);
        //            fetchData.digcode = powerOfAttorneyInformation.digcode;
        //            fetchData.digname = commonFunctions.ReplaceNA(powerOfAttorneyInformation.digname!);
        //            fetchData.poa_district_code = powerOfAttorneyInformation.poa_district_code;
        //            fetchData.poa_district_name = commonFunctions.ReplaceNA(powerOfAttorneyInformation.poa_district_name!);
        //            fetchData.sro_office_code = powerOfAttorneyInformation.sro_office_code;
        //            fetchData.sro_office_name = commonFunctions.ReplaceNA(powerOfAttorneyInformation.sro_office_name!);
        //            TimeZoneInfo INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
        //            DateTime indianTime = TimeZoneInfo.ConvertTimeFromUtc(Convert.ToDateTime(powerOfAttorneyInformation.createddatetime.ToString()), INDIAN_ZONE);
        //            fetchData.createddatetime = indianTime.ToString("yyyy-MM-dd");
        //            fetchData.isdeleted = powerOfAttorneyInformation.isDeleted;
        //            fetchData.deleteddate = powerOfAttorneyInformation.deleteddate.ToString("yyyy-MM-dd");
        //            fetchData.signed_file_name = string.IsNullOrEmpty(powerOfAttorneyInformation.signed_file_name) ? "NA" : powerOfAttorneyInformation.signed_file_name;
        //            fetchData.signed_file_path = string.IsNullOrEmpty(powerOfAttorneyInformation.signed_file_path) ? "NA" : powerOfAttorneyInformation.signed_file_path;
        //            fetchData.profile_pic_file_name = string.IsNullOrEmpty(powerOfAttorneyInformation.profile_pic_file_name) ? "NA" : powerOfAttorneyInformation.profile_pic_file_name;
        //            fetchData.profile_pic_file_path = string.IsNullOrEmpty(powerOfAttorneyInformation.profile_pic_file_path) ? "NA" : powerOfAttorneyInformation.profile_pic_file_path;
        //            fetchData.cts_number = powerOfAttorneyInformation.cts_number;
        //            fetchData.mutation_srno = powerOfAttorneyInformation.mutation_srno;
        //            fetchData.owner_number = powerOfAttorneyInformation.owner_number;
        //            fetchData.village_code = powerOfAttorneyInformation.village_code;
        //            fetchData.village_name = powerOfAttorneyInformation.village_name;

        //            fetchData.signed_file_name = commonFunctions.ReplaceNA(fetchData.signed_file_name!);
        //            fetchData.signed_file_path = commonFunctions.ReplaceNA(fetchData.signed_file_path!);
        //            fetchData.profile_pic_file_name = commonFunctions.ReplaceNA(fetchData.profile_pic_file_name!);
        //            fetchData.profile_pic_file_path = commonFunctions.ReplaceNA(fetchData.profile_pic_file_path!);
        //        }
        //        else
        //        {
        //            fetchData = null!;
        //        }
        //        return fetchData;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new HandleException(ex.Message.ToString());
        //    }
        //}

        //public FetchPOAForTakerDataForNIC FetchPowerOfAttorneyInfoForTaker(int powerOfAttorneyID)
        //{
        //    try
        //    {
        //        PowerOfAttorneyInformation powerOfAttorneyInformation = new PowerOfAttorneyInformation();
        //        powerOfAttorneyInformation = _context.powerOfAttorneyInformation.Include(i => i.userMaster).Include(app => app.applicationDTL).Where(data => data.power_of_attorney_id.Equals(powerOfAttorneyID) && data.is_taker == true && data.isDeleted == false).FirstOrDefault()!;
        //        FetchPOAForTakerDataForNIC fetchData = new FetchPOAForTakerDataForNIC();
        //        if (powerOfAttorneyInformation != null)
        //        {
        //            fetchData.power_of_attorney_id = powerOfAttorneyInformation.power_of_attorney_id;
        //            fetchData.userid = powerOfAttorneyInformation.userMaster!.userid;
        //            fetchData.applicationid = powerOfAttorneyInformation.applicationDTL!.applicationid;
        //            fetchData.power_of_attorney_code = powerOfAttorneyInformation.power_of_attorney_code;
        //            fetchData.is_taker = powerOfAttorneyInformation.is_taker;
        //            fetchData.usertype_code = powerOfAttorneyInformation.usertype_code;
        //            fetchData.usertype = powerOfAttorneyInformation.usertype;
        //            fetchData.mutation_id = powerOfAttorneyInformation.mutation_id;
        //            fetchData.mobileno = powerOfAttorneyInformation.mobileno;
        //            fetchData.mobilenoverified = powerOfAttorneyInformation.mobilenoverified;
        //            fetchData.emailid = commonFunctions.ReplaceNA(powerOfAttorneyInformation.emailid!);
        //            fetchData.emailidverified = commonFunctions.ReplaceNA(powerOfAttorneyInformation.emailidverified!);
        //            fetchData.prefixcode_marathi = commonFunctions.ReplaceNA(powerOfAttorneyInformation.prefixcode_marathi!);
        //            fetchData.prefix_in_marathi = commonFunctions.ReplaceNA(powerOfAttorneyInformation.prefix_in_marathi!);
        //            fetchData.fname_in_marathi = commonFunctions.ReplaceNA(powerOfAttorneyInformation.fname_in_marathi!);
        //            fetchData.mname_in_marathi = commonFunctions.ReplaceNA(powerOfAttorneyInformation.mname_in_marathi!);
        //            fetchData.lname_in_marathi = commonFunctions.ReplaceNA(powerOfAttorneyInformation.lname_in_marathi!);
        //            fetchData.prefixcode_eng = commonFunctions.ReplaceNA(powerOfAttorneyInformation.prefixcode_eng!);
        //            fetchData.prefix_in_eng = commonFunctions.ReplaceNA(powerOfAttorneyInformation.prefix_in_eng!);
        //            fetchData.fname_in_eng = commonFunctions.ReplaceNA(powerOfAttorneyInformation.fname_in_eng!);
        //            fetchData.mname_in_eng = commonFunctions.ReplaceNA(powerOfAttorneyInformation.mname_in_eng!);
        //            fetchData.lname_in_eng = commonFunctions.ReplaceNA(powerOfAttorneyInformation.lname_in_eng!);
        //            fetchData.company_name_in_marathi = commonFunctions.ReplaceNA(powerOfAttorneyInformation.company_name_in_marathi!);
        //            fetchData.company_name_in_eng = commonFunctions.ReplaceNA(powerOfAttorneyInformation.company_name_in_eng!);
        //            fetchData.username = commonFunctions.ReplaceNA(powerOfAttorneyInformation.username!);
        //            fetchData.alias_name = commonFunctions.ReplaceNA(powerOfAttorneyInformation.alias_name!);
        //            fetchData.gender_code = powerOfAttorneyInformation.gender_code;
        //            fetchData.gender_description = powerOfAttorneyInformation.gender_description;
        //            fetchData.dob = powerOfAttorneyInformation.dob;
        //            fetchData.mother_name_in_marathi = commonFunctions.ReplaceNA(powerOfAttorneyInformation.mother_name_in_marathi!);
        //            fetchData.mother_name_in_eng = commonFunctions.ReplaceNA(powerOfAttorneyInformation.mother_name_in_eng!);
        //            fetchData.address_type = powerOfAttorneyInformation.address_type;
        //            fetchData.address = commonFunctions.ReplaceNA(powerOfAttorneyInformation.address!);
        //            fetchData.state = powerOfAttorneyInformation.state;
        //            fetchData.district = powerOfAttorneyInformation.district;
        //            fetchData.taluka = powerOfAttorneyInformation.taluka;
        //            fetchData.city = powerOfAttorneyInformation.city;
        //            fetchData.flatno_plotno = powerOfAttorneyInformation.flatno_plotno;
        //            fetchData.societyname = powerOfAttorneyInformation.societyname;
        //            fetchData.mainstreet = powerOfAttorneyInformation.mainstreet;
        //            fetchData.landmark = powerOfAttorneyInformation.landmark;
        //            fetchData.locality = powerOfAttorneyInformation.locality;
        //            fetchData.pincode = powerOfAttorneyInformation.pincode;
        //            fetchData.postofficename = powerOfAttorneyInformation.postofficename;
        //            fetchData.address_proof_document_name = string.IsNullOrEmpty(powerOfAttorneyInformation.address_proof_document_name) ? "NA" : powerOfAttorneyInformation.address_proof_document_name;
        //            fetchData.address_proof_document_path = string.IsNullOrEmpty(powerOfAttorneyInformation.address_proof_document_path) ? "NA" : powerOfAttorneyInformation.address_proof_document_path;
        //            fetchData.city_servey_no = commonFunctions.ReplaceNA(powerOfAttorneyInformation.city_servey_no!);
        //            fetchData.lr_property_id = commonFunctions.ReplaceNA(powerOfAttorneyInformation.lr_property_id!);
        //            fetchData.sub_property_no = powerOfAttorneyInformation.sub_property_no;
        //            fetchData.owner_of_property_in_maharashtra = powerOfAttorneyInformation.owner_of_property_in_maharashtra;
        //            fetchData.propertytypeid = powerOfAttorneyInformation.propertyType.propertytypeid;
        //            fetchData.property_district_code = commonFunctions.ReplaceNA(powerOfAttorneyInformation.property_district_code!);
        //            fetchData.property_district_name_in_marathi = commonFunctions.ReplaceNA(powerOfAttorneyInformation.property_district_name_in_marathi!);
        //            fetchData.property_district_name_in_english = commonFunctions.ReplaceNA(powerOfAttorneyInformation.property_district_name_in_english!);
        //            fetchData.property_taluka_code = commonFunctions.ReplaceNA(powerOfAttorneyInformation.property_taluka_code!);
        //            fetchData.property_taluka_name = commonFunctions.ReplaceNA(powerOfAttorneyInformation.property_taluka_name!);
        //            fetchData.property_city_code = commonFunctions.ReplaceNA(powerOfAttorneyInformation.property_city_code!);
        //            fetchData.property_city_name = commonFunctions.ReplaceNA(powerOfAttorneyInformation.property_city_name!);
        //            fetchData.khateno = commonFunctions.ReplaceNA(powerOfAttorneyInformation.khateno!);
        //            fetchData.ulpin = commonFunctions.ReplaceNA(powerOfAttorneyInformation.ulpin!);
        //            fetchData.khata_type_code = powerOfAttorneyInformation.khata_type_code;
        //            fetchData.khata_type_name = powerOfAttorneyInformation.khata_type_name;
        //            fetchData.owner_status_code = powerOfAttorneyInformation.owner_status_code;
        //            fetchData.owner_status_description = powerOfAttorneyInformation.owner_status_description;
        //            fetchData.attornytype_code = powerOfAttorneyInformation.attornytype_code;
        //            fetchData.attornytype_desc = commonFunctions.ReplaceNA(powerOfAttorneyInformation.attornytype_desc!);
        //            fetchData.land_buy_area = commonFunctions.ReplaceNA(powerOfAttorneyInformation.landBuyArea!);
        //            fetchData.is_poa_is_partof_dast = powerOfAttorneyInformation.isPOAisPartofDast;
        //            fetchData.is_decleration_involved_in_poa = powerOfAttorneyInformation.isDeclerationInvolvedInPOA;
        //            fetchData.is_poa_permanant = powerOfAttorneyInformation.isPOAPermanant;
        //            fetchData.is_transfer_rights = powerOfAttorneyInformation.isTransferRights;
        //            fetchData.dast_no = powerOfAttorneyInformation.dast_no;
        //            fetchData.dast_no_date = powerOfAttorneyInformation.dast_no_date;
        //            fetchData.dast_no_year = powerOfAttorneyInformation.dast_no_year;
        //            fetchData.is_dast_verified = powerOfAttorneyInformation.isDastVerified;
        //            fetchData.verifieddast_data = powerOfAttorneyInformation.verifieddastData;
        //            fetchData.digcode = powerOfAttorneyInformation.digcode;
        //            fetchData.digname = powerOfAttorneyInformation.digname;
        //            fetchData.poa_district_code = powerOfAttorneyInformation.poa_district_code;
        //            fetchData.poa_district_name = powerOfAttorneyInformation.poa_district_name;
        //            fetchData.sro_office_code = powerOfAttorneyInformation.sro_office_code;
        //            fetchData.sro_office_name = powerOfAttorneyInformation.sro_office_name;
        //            TimeZoneInfo INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
        //            DateTime indianTime = TimeZoneInfo.ConvertTimeFromUtc(Convert.ToDateTime(powerOfAttorneyInformation.createddatetime.ToString()), INDIAN_ZONE);
        //            fetchData.createddatetime = indianTime.ToString("yyyy-MM-dd");
        //            fetchData.isdeleted = powerOfAttorneyInformation.isDeleted;
        //            fetchData.deleteddate = powerOfAttorneyInformation.deleteddate.ToString("yyyy-MM-dd");
        //            fetchData.signed_file_name = string.IsNullOrEmpty(powerOfAttorneyInformation.signed_file_name) ? "NA" : powerOfAttorneyInformation.signed_file_name;
        //            fetchData.signed_file_path = string.IsNullOrEmpty(powerOfAttorneyInformation.signed_file_path) ? "NA" : powerOfAttorneyInformation.signed_file_path;
        //            fetchData.profile_pic_file_name = string.IsNullOrEmpty(powerOfAttorneyInformation.profile_pic_file_name) ? "NA" : powerOfAttorneyInformation.profile_pic_file_name;
        //            fetchData.profile_pic_file_path = string.IsNullOrEmpty(powerOfAttorneyInformation.profile_pic_file_path) ? "NA" : powerOfAttorneyInformation.profile_pic_file_path;
        //            fetchData.cts_number = commonFunctions.ReplaceNA(powerOfAttorneyInformation.cts_number!);
        //            fetchData.mutation_srno = commonFunctions.ReplaceNA(powerOfAttorneyInformation.mutation_srno!);
        //            fetchData.owner_number = commonFunctions.ReplaceNA(powerOfAttorneyInformation.owner_number!);
        //            fetchData.village_code = commonFunctions.ReplaceNA(powerOfAttorneyInformation.village_code!);
        //            fetchData.village_name = commonFunctions.ReplaceNA(powerOfAttorneyInformation.village_name!);

        //            fetchData.signed_file_name = commonFunctions.ReplaceNA(fetchData.signed_file_name!);
        //            fetchData.signed_file_path = commonFunctions.ReplaceNA(fetchData.signed_file_path!);
        //            fetchData.profile_pic_file_name = commonFunctions.ReplaceNA(fetchData.profile_pic_file_name!);
        //            fetchData.profile_pic_file_path = commonFunctions.ReplaceNA(fetchData.profile_pic_file_path!);
        //        }
        //        else
        //        {
        //            fetchData = null!;
        //        }
        //        return fetchData;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new HandleException(ex.Message.ToString());
        //    }
        //}

        //public FetchKharediNondDataForGiver FetchKhrediNondInformationDataForGiver(int mutationdtlid)
        //{
        //    try
        //    {
        //        MutationGiverTakerDTL KharedinondInformation = new MutationGiverTakerDTL();
        //        KharedinondInformation = _context.mutationDTL.Include(i => i.userMaster).Include(app => app.applicationDTL).Where(data => data.mutation_givertaker_id.Equals(mutationdtlid) && data.isDeleted == false).FirstOrDefault()!;

        //        FetchKharediNondDataForGiver fetchData = new FetchKharediNondDataForGiver();
        //        fetchData.mutation_dtl_id = KharedinondInformation.mutation_givertaker_id;
        //        fetchData.userid = KharedinondInformation.userMaster!.userid;
        //        fetchData.applicationid = KharedinondInformation.applicationDTL!.applicationid;
        //        fetchData.fullNameInMarathi = commonFunctions.ReplaceNA(KharedinondInformation.fname_in_marathi!.Trim()) + " " + commonFunctions.ReplaceNA(KharedinondInformation.mname_in_marathi!.Trim()) + " " + commonFunctions.ReplaceNA(KharedinondInformation.lname_in_marathi!.Trim());
        //        fetchData.fullNameInEng = commonFunctions.ReplaceNA(KharedinondInformation.fname_in_eng!.Trim()) + " " + commonFunctions.ReplaceNA(KharedinondInformation.mname_in_eng!.Trim()) + " " + commonFunctions.ReplaceNA(KharedinondInformation.lname_in_eng!.Trim());
        //        fetchData.mobileNo = KharedinondInformation.mobileno;

        //        InputDataModel.UserDTLForKharediNond userDetails = new InputDataModel.UserDTLForKharediNond();
        //        userDetails.suffixcode = KharedinondInformation.prefixcode_marathi;
        //        userDetails.suffixCodeEng = KharedinondInformation.prefixcode_eng;
        //        userDetails.suffix = KharedinondInformation.prefix_in_marathi;
        //        userDetails.firstName = KharedinondInformation.fname_in_marathi;
        //        userDetails.middleName = KharedinondInformation.mname_in_marathi;
        //        userDetails.lastName = KharedinondInformation.lname_in_marathi;
        //        userDetails.suffixEng = KharedinondInformation.prefix_in_eng;
        //        userDetails.firstNameEng = KharedinondInformation.fname_in_eng;
        //        userDetails.middleNameEng = KharedinondInformation.mname_in_eng;
        //        userDetails.lastNameEng = KharedinondInformation.lname_in_eng;
        //        userDetails.aliceName = KharedinondInformation.alias_name;
        //        userDetails.holderType = KharedinondInformation.holder_type;
        //        userDetails.dob = KharedinondInformation.dob;
        //        userDetails.motherName = KharedinondInformation.mother_name_in_marathi;
        //        userDetails.motherNameEng = KharedinondInformation.mother_name_in_eng;
        //        userDetails.nabhu = KharedinondInformation.city_servey_no;
        //        userDetails.userName = KharedinondInformation.userName;
        //        userDetails.lrPropertyUID = KharedinondInformation.lr_property_id;
        //        userDetails.milkat = KharedinondInformation.milkat;
        //        userDetails.namud = KharedinondInformation.namud;
        //        userDetails.subPropNo = KharedinondInformation.sub_property_no;
        //        //Aapak aapak1 = new Aapak();
        //        //aapak1.apk_code = Convert.ToString(KharedinondInformation.apk_code);
        //        //aapak1.apk_description = KharedinondInformation.apk_description;
        //        //userDetails!.aapak = aapak1;
        //        //InputDataModel.Holdertype holdertype = new InputDataModel.Holdertype();
        //        //holdertype.account_type_code = KharedinondInformation.account_type_code;
        //        //holdertype.account_type_description = KharedinondInformation.account_type_description;
        //        //userDetails.holderType = holdertype;
        //        fetchData.userDetails = userDetails;

        //        InputDataModel.areaForMutationDTLKarediNond areamutation = new areaForMutationDTLKarediNond();
        //        areamutation.isFullAreaGiven = KharedinondInformation.isFullAreaGiven;
        //        areamutation.actualArea = KharedinondInformation.actual_area;
        //        areamutation.mutationArea = KharedinondInformation.mutation_area;
        //        fetchData.areaForMutation = areamutation;

        //        InputDataModel.AddressDTLForKharediNond addressData = new InputDataModel.AddressDTLForKharediNond();
        //        addressData.addressType = KharedinondInformation.address_type;
        //        if (KharedinondInformation.address_type == "INDIA")
        //        {
        //            InputDataModel.IndiaAddressForKharediNond addressForIndia = new InputDataModel.IndiaAddressForKharediNond();
        //            addressForIndia.state = KharedinondInformation.state;
        //            addressForIndia.district = KharedinondInformation.district;
        //            addressForIndia.city = KharedinondInformation.city;
        //            addressForIndia.taluka = KharedinondInformation.taluka;
        //            addressForIndia.plotNo = KharedinondInformation.flatno_plotno;
        //            addressForIndia.building = KharedinondInformation.societyname;
        //            addressForIndia.mainRoad = KharedinondInformation.mainstreet;
        //            addressForIndia.impSymbol = KharedinondInformation.landmark;
        //            addressForIndia.area = KharedinondInformation.locality;
        //            addressForIndia.pincode = KharedinondInformation.pincode;
        //            addressForIndia.postOfficeName = KharedinondInformation.post_office_name;
        //            addressForIndia.addressProofName = KharedinondInformation.address_proof_document_name;
        //            addressForIndia.mobile = KharedinondInformation.mobileno;
        //            addressForIndia.mobileOTP = KharedinondInformation.mobilenoverified;
        //            addressForIndia.signatureName = KharedinondInformation.signed_file_name!;
        //            /*if (addressForIndia.addressProofName != "NA")
        //                addressForIndia.addressProofSrc = KharedinondInformation.address_proof_src == null ? "" : Convert.ToBase64String(KharedinondInformation.address_proof_src);
        //            else addressForIndia.addressProofSrc = "";
        //            if (addressForIndia.signatureName != "NA")
        //                addressForIndia.signatureSrc = KharedinondInformation.signature_src == null ? "" : Convert.ToBase64String(KharedinondInformation.signature_src);
        //            else addressForIndia.signatureSrc = "";*/

        //            if (KharedinondInformation.address_proof_document_path != "NA")
        //            {
        //                KharedinondInformation.address_proof_document_name = string.IsNullOrEmpty(KharedinondInformation.address_proof_document_name) ? "NA" : KharedinondInformation.address_proof_document_name;
        //                KharedinondInformation.address_proof_document_path = string.IsNullOrEmpty(KharedinondInformation.address_proof_document_path) ? "NA" : KharedinondInformation.address_proof_document_path;
        //                addressForIndia.addressProofName = KharedinondInformation.address_proof_document_name;
        //                addressForIndia.addressProofSrc = KharedinondInformation.address_proof_document_path;
        //            }
        //            else
        //            {
        //                addressForIndia.addressProofName = KharedinondInformation.address_proof_document_name;
        //                addressForIndia.addressProofSrc = KharedinondInformation.address_proof_document_path;
        //            }
        //            //if (KharedinondInformation.signed_file_path != "NA")
        //            //{
        //            //    string SignatureExt = Path.GetExtension(KharedinondInformation.signed_file_path)!;
        //            //    string Signature = methodForFile.ConvertImageToBase64(KharedinondInformation.signed_file_path!);
        //            //    KharedinondInformation.signed_file_path = string.IsNullOrEmpty(Signature) ? "NA" : "data:image/" + SignatureExt.Replace(".", "") + ";base64," + Signature;
        //            //    addressForIndia.signatureSrc = KharedinondInformation.signed_file_path;
        //            //}
        //            addressForIndia.signatureName = string.IsNullOrEmpty(KharedinondInformation.signed_file_name) ? "NA" : KharedinondInformation.signed_file_name!;
        //            addressForIndia.signatureSrc = string.IsNullOrEmpty(KharedinondInformation.signed_file_path) ? "NA" : KharedinondInformation.signed_file_path!;
        //            addressData.indiaAddress = addressForIndia;
        //        }
        //        else if (KharedinondInformation.address_type == "FOREIGN")
        //        {
        //            InputDataModel.AddressForForeign addressForForeign = new InputDataModel.AddressForForeign();
        //            addressForForeign.address = KharedinondInformation.address;
        //            addressForForeign.mobile = KharedinondInformation.mobileno;
        //            addressForForeign.email = KharedinondInformation.emailid;
        //            addressForForeign.emailOTP = KharedinondInformation.emailidverified;
        //            //string SignatureExt = Path.GetExtension(KharedinondInformation.signed_file_path)!;
        //            //string Signature = methodForFile.ConvertImageToBase64(KharedinondInformation.signed_file_path!);
        //            //KharedinondInformation.signed_file_path = string.IsNullOrEmpty(Signature) ? "NA" : "data:image/" + SignatureExt.Replace(".", "") + ";base64," + Signature;
        //            addressForForeign.signatureName = KharedinondInformation.signed_file_name;
        //            addressForForeign.signatureSrc = KharedinondInformation.signed_file_path;
        //            addressData.foreignAddress = addressForForeign;
        //        }
        //        fetchData.address = addressData;
        //        return fetchData;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new HandleException(ex.Message.ToString());
        //    }
        //}
        //public FetchKharediNondDataForTaker FetchKhrediNondInformationDataForTaker(int mutationdtlid)
        //{
        //    try
        //    {
        //        MutationGiverTakerDTL KharedinondInformation = new MutationGiverTakerDTL();
        //        KharedinondInformation = _context.mutationDTL.Include(i => i.userMaster).Include(app => app.applicationDTL).Include(prop => prop.prop_type).Where(data => data.mutation_givertaker_id.Equals(mutationdtlid) && data.isDeleted == false).FirstOrDefault()!;

        //        FetchKharediNondDataForTaker fetchData = new FetchKharediNondDataForTaker();
        //        fetchData.mutation_dtl_id = KharedinondInformation.mutation_givertaker_id;
        //        fetchData.userid = KharedinondInformation.userMaster!.userid;
        //        fetchData.applicationid = KharedinondInformation.applicationDTL!.applicationid;
        //        fetchData.usertype_code = KharedinondInformation.user_type_code;
        //        fetchData.userType = KharedinondInformation.user_type;

        //        fetchData.mobileNo = KharedinondInformation.mobileno;

        //        //Photo Details
        //        InputDataModel.photoDetails photo = new photoDetails();
        //        photo.passportName = KharedinondInformation.profile_pic_file_name;
        //        photo.passportSrc = KharedinondInformation.profile_pic_file_path;
        //        //string PhotoProofExt = Path.GetExtension(KharedinondInformation.profile_pic_file_path)!;
        //        //string PhotoProof = methodForFile.ConvertImageToBase64(KharedinondInformation.profile_pic_file_path!);
        //        //KharedinondInformation.profile_pic_file_path = string.IsNullOrEmpty(PhotoProof) ? "NA" : "data:image/" + PhotoProofExt.Replace(".", "") + ";base64," + PhotoProof;
        //        //photo.passportSrc = KharedinondInformation.profile_pic_file_path;
        //        fetchData.photo = photo;

        //        //isMHDetails
        //        InputDataModel.isMHProperty isMHproperty = new InputDataModel.isMHProperty();
        //        isMHproperty.hasProperty = KharedinondInformation.has_property;
        //        // PropertyTypeMaster proptype = new PropertyTypeMaster();

        //        isMHproperty.propType = KharedinondInformation.prop_type!.propertytype;


        //        InputDataModel.TakeruserDetails takeruserDetails = new TakeruserDetails();
        //        takeruserDetails.suffixcode = KharedinondInformation.prefixcode_marathi;
        //        takeruserDetails!.suffix = KharedinondInformation.prefix_in_marathi;
        //        takeruserDetails.firstName = KharedinondInformation.fname_in_marathi;
        //        takeruserDetails.middleName = KharedinondInformation.mname_in_marathi;
        //        takeruserDetails.lastName = KharedinondInformation.lname_in_marathi;
        //        takeruserDetails.suffixCodeEng = KharedinondInformation.prefixcode_marathi;
        //        takeruserDetails.suffixEng = KharedinondInformation.prefix_in_eng;
        //        takeruserDetails.firstNameEng = KharedinondInformation.fname_in_eng;
        //        takeruserDetails.middleNameEng = KharedinondInformation.mname_in_eng;
        //        takeruserDetails.lastNameEng = KharedinondInformation.lname_in_eng;
        //        takeruserDetails.companyName = KharedinondInformation.company_name_in_marathi!;
        //        takeruserDetails.companyNameEng = KharedinondInformation.company_name_in_eng!;
        //        takeruserDetails.khataNo = KharedinondInformation.khatano;
        //        takeruserDetails.naBhu = KharedinondInformation.city_servey_no;
        //        takeruserDetails.ulpin = KharedinondInformation.ulpin;
        //        takeruserDetails.userName = KharedinondInformation.userName;


        //        District district = new District();
        //        district.district_code = KharedinondInformation.district_code;
        //        district.district_name = KharedinondInformation.district_name_in_marathi;
        //        district.district_english_name = KharedinondInformation.district_name_in_eng;
        //        takeruserDetails.district = district;

        //        Taluka taluka = new Taluka();
        //        taluka.office_code = KharedinondInformation.ofc_code;
        //        taluka.office_name = KharedinondInformation.ofc_name;
        //        takeruserDetails.taluka = taluka;

        //        VillageforKharediNond village = new VillageforKharediNond();
        //        village.village_code = KharedinondInformation.village_code;
        //        village.village_name = KharedinondInformation.village_name;
        //        takeruserDetails.village = village;

        //        isMHproperty.userDetails = takeruserDetails;
        //        fetchData.isMHProperty = isMHproperty;

        //        //Dharak Details
        //        InputDataModel.dharakDetails dharak = new InputDataModel.dharakDetails();
        //        if (KharedinondInformation.user_type_code == 1)
        //        {
        //            //fetchData.fullNameInMarathi = KharedinondInformation.fname_in_marathi!.Trim() + " " + KharedinondInformation.mname_in_marathi!.Trim() + " " + KharedinondInformation.lname_in_marathi!.Trim();
        //            //fetchData.fullNameInEng = KharedinondInformation.fname_in_eng!.Trim() + " " + KharedinondInformation.mname_in_eng!.Trim() + " " + KharedinondInformation.lname_in_eng!.Trim();
        //            fetchData.fullNameInMarathi = commonFunctions.ReplaceNA(KharedinondInformation.fname_in_marathi!.Trim()) + " " + commonFunctions.ReplaceNA(KharedinondInformation.mname_in_marathi!.Trim()) + " " + commonFunctions.ReplaceNA(KharedinondInformation.lname_in_marathi!.Trim());
        //            fetchData.fullNameInEng = commonFunctions.ReplaceNA(KharedinondInformation.fname_in_eng!.Trim()) + " " + commonFunctions.ReplaceNA(KharedinondInformation.mname_in_eng!.Trim()) + " " + commonFunctions.ReplaceNA(KharedinondInformation.lname_in_eng!.Trim());
        //            InputDataModel.userdharakDetails userdharak = new userdharakDetails();
        //            userdharak!.aliceName = KharedinondInformation.alias_name;
        //            Gender gender = new Gender();
        //            gender.gender_code = KharedinondInformation.gender_code;
        //            gender.gender_description = KharedinondInformation.gender_description;
        //            userdharak.gender = gender;
        //            aapakDropdown aapakdropdown = new aapakDropdown();
        //            aapakdropdown.apk_code = KharedinondInformation.apk_code;
        //            aapakdropdown.apk_description = KharedinondInformation.apk_description;
        //            userdharak.aapakDropdown = aapakdropdown;
        //            userdharak.aapak = KharedinondInformation.aapak;
        //            aapakRelation aapakrelation = new aapakRelation();
        //            aapakrelation.relation_code = KharedinondInformation.relation_code.ToString();
        //            aapakrelation.relation_name = KharedinondInformation.relation_name;
        //            userdharak.aapakRelation = aapakrelation;

        //            //userdharak.khataType = KharedinondInformation.khata_type;
        //            //InputDataModel.KhataType khatatype = new KhataType();
        //            //khatatype.khataCode = KharedinondInformation.khata_type_code;
        //            //khatatype.khataLabel = KharedinondInformation.khata_type_name;
        //            //userdharak.khataType = khatatype;

        //            //userdharak.holderType = KharedinondInformation.holder_type;
        //            userdharak.dob = KharedinondInformation.dob;
        //            userdharak.motherName = KharedinondInformation.mother_name_in_marathi;
        //            userdharak.motherNameEng = KharedinondInformation.mother_name_in_eng;
        //            InputDataModel.Holdertype holdertype = new InputDataModel.Holdertype();
        //            holdertype.owner_status_code = KharedinondInformation.owner_status_code;
        //            holdertype.owner_status_description = KharedinondInformation.owner_status_description;
        //            userdharak.holderType = holdertype;
        //            userdharak.landBuyArea = KharedinondInformation.land_buy_area;
        //            dharak.userdharak = userdharak;
        //        }
        //        else
        //        {
        //            fetchData.fullNameInMarathi = KharedinondInformation.company_name_in_marathi;
        //            fetchData.fullNameInEng = KharedinondInformation.company_name_in_eng;
        //            InputDataModel.companydharakDetails companydharak = new companydharakDetails();
        //            // companydharak.holderType = KharedinondInformation.holder_type;
        //            //companydharak.khataType = KharedinondInformation.khata_type;
        //            // companydharak.aapakDropdown = KharedinondInformation.aapak_dropdown;
        //            //companydharak.aapak = KharedinondInformation.aapak;
        //            companydharak.landBuyArea = KharedinondInformation.land_buy_area;

        //            KhataType khatatype = new KhataType();
        //            khatatype.khataCode = KharedinondInformation.khata_type_code;
        //            khatatype.khataLabel = KharedinondInformation.khata_type_name;
        //            //companydharak.khataType = khatatype;

        //            InputDataModel.Holdertype holdertype = new InputDataModel.Holdertype();
        //            holdertype.owner_status_code = KharedinondInformation.owner_status_code;
        //            holdertype.owner_status_description = KharedinondInformation.owner_status_description;
        //            companydharak.holderType = holdertype;
        //            InputDataModel.aapakDropdown aapak = new InputDataModel.aapakDropdown();
        //            aapak.apk_code = KharedinondInformation.apk_code;
        //            aapak.apk_description = KharedinondInformation.apk_description;
        //            //companydharak.aapakDropdown = aapak;
        //            dharak.companydharak = companydharak;
        //        }

        //        fetchData.dharak = dharak;

        //        //Address Details
        //        InputDataModel.AddressDTLForKharediNond addressData = new InputDataModel.AddressDTLForKharediNond();
        //        addressData.addressType = KharedinondInformation.address_type;
        //        if (KharedinondInformation.address_type == "INDIA")
        //        {
        //            InputDataModel.IndiaAddressForKharediNond addressForIndia = new InputDataModel.IndiaAddressForKharediNond();
        //            addressForIndia.state = KharedinondInformation.state;
        //            addressForIndia.district = KharedinondInformation.district;
        //            addressForIndia.city = KharedinondInformation.city;
        //            addressForIndia.taluka = KharedinondInformation.taluka;
        //            addressForIndia.plotNo = KharedinondInformation.flatno_plotno;
        //            addressForIndia.building = KharedinondInformation.societyname;
        //            addressForIndia.mainRoad = KharedinondInformation.mainstreet;
        //            addressForIndia.impSymbol = KharedinondInformation.landmark;
        //            addressForIndia.area = KharedinondInformation.locality;
        //            addressForIndia.pincode = KharedinondInformation.pincode;
        //            addressForIndia.postOfficeName = KharedinondInformation.post_office_name;
        //            addressForIndia.addressProofName = KharedinondInformation.address_proof_document_name;
        //            addressForIndia.mobile = KharedinondInformation.mobileno;
        //            addressForIndia.mobileOTP = KharedinondInformation.mobilenoverified;
        //            addressForIndia.signatureName = KharedinondInformation.signed_file_name!;

        //            if (KharedinondInformation.address_proof_document_path != "NA")
        //            {
        //                addressForIndia.addressProofName = string.IsNullOrEmpty(KharedinondInformation.address_proof_document_name) ? "NA" : KharedinondInformation.address_proof_document_name;
        //                addressForIndia.addressProofSrc = string.IsNullOrEmpty(KharedinondInformation.address_proof_document_path) ? "NA" : KharedinondInformation.address_proof_document_path;
        //            }
        //            else
        //            {
        //                addressForIndia.addressProofName = string.IsNullOrEmpty(KharedinondInformation.address_proof_document_name) ? "NA" : KharedinondInformation.address_proof_document_name;
        //                addressForIndia.addressProofSrc = string.IsNullOrEmpty(KharedinondInformation.address_proof_document_path) ? "NA" : KharedinondInformation.address_proof_document_path;
        //            }
        //            //if (KharedinondInformation.signed_file_path != "NA")
        //            //{
        //            //    string SignatureExt = Path.GetExtension(KharedinondInformation.signed_file_path)!;
        //            //    string Signature = methodForFile.ConvertImageToBase64(KharedinondInformation.signed_file_path!);
        //            //    KharedinondInformation.signed_file_path = string.IsNullOrEmpty(Signature) ? "NA" : "data:image/" + SignatureExt.Replace(".", "") + ";base64," + Signature;
        //            //    addressForIndia.signatureSrc = KharedinondInformation.signed_file_path;
        //            //}
        //            addressForIndia.signatureName = KharedinondInformation.signed_file_name!;
        //            addressForIndia.signatureSrc = KharedinondInformation.signed_file_path!;

        //            addressForIndia.signatureName = commonFunctions.ReplaceNA(addressForIndia.signatureName);
        //            addressForIndia.signatureSrc = commonFunctions.ReplaceNA(addressForIndia.signatureSrc);
        //            addressForIndia.addressProofName = commonFunctions.ReplaceNA(addressForIndia.addressProofName);
        //            addressForIndia.addressProofSrc = commonFunctions.ReplaceNA(addressForIndia.addressProofSrc);
        //            addressData.indiaAddress = addressForIndia;
        //        }
        //        else if (KharedinondInformation.address_type == "FOREIGN")
        //        {
        //            InputDataModel.AddressForForeign addressForForeign = new InputDataModel.AddressForForeign();
        //            addressForForeign.address = KharedinondInformation.address;
        //            addressForForeign.mobile = KharedinondInformation.mobileno;
        //            addressForForeign.email = KharedinondInformation.emailid;
        //            addressForForeign.emailOTP = KharedinondInformation.emailidverified;
        //            addressForForeign.signatureName = commonFunctions.ReplaceNA(KharedinondInformation.signed_file_name!);
        //            addressForForeign.signatureSrc = commonFunctions.ReplaceNA(KharedinondInformation.signed_file_path!);
        //            //string SignatureExt = Path.GetExtension(KharedinondInformation.signed_file_path)!;
        //            //string Signature = methodForFile.ConvertImageToBase64(KharedinondInformation.signed_file_path!);
        //            //KharedinondInformation.signed_file_path = string.IsNullOrEmpty(Signature) ? "NA" : "data:image/" + SignatureExt.Replace(".", "") + ";base64," + Signature;
        //            //addressForForeign.signatureSrc = KharedinondInformation.signed_file_path;
        //            addressData.foreignAddress = addressForForeign;
        //        }
        //        fetchData.address = addressData;
        //        return fetchData;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new HandleException(ex.Message.ToString());
        //    }
        //}


        //public FetchUploadedDocumentsForNIC uploadDocuments(string nabhu, string applicationid)
        //{
        //    FetchUploadedDocumentsForNIC document = new FetchUploadedDocumentsForNIC();
        //    var docname = _context.uploadedDocumentsDTLs.Where(a => a.city_servey_no == nabhu).Select(a => a.document_name).ToList();
        //    document.applicationID = applicationid;
        //    document.nabhuno = nabhu;
        //    document.docName = String.Join(",", docname);


        //    TimeZoneInfo INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
        //    DateTime indianTime = TimeZoneInfo.ConvertTimeFromUtc(Convert.ToDateTime(docname.createddatetime.ToString()), INDIAN_ZONE);
        //    fetchData.createddatetime = indianTime.ToString("yyyy-MM-dd");
        //    fetchData.isdeleted = dastInformation.isDeleted;
        //    fetchData.deleteddate = dastInformation.deleteddate.ToString("yyyy-MM-dd");

        //    return document;
        //}

        public FetchUploadedDocumentDataForNIC FetchUploadedDocumentDTLs(int documentID)
        {
            try
            {
                UploadedDocumentsDTL uploadedDocumentsDTL = new UploadedDocumentsDTL();
                uploadedDocumentsDTL = _contextR.uploadedDocumentsDTLs.Include(i => i.userMaster).Include(app => app.applicationDTL).Where(data => data.uploaded_doc_id.Equals(documentID) && data.truti_patra_flag == "NA").FirstOrDefault()!;
                FetchUploadedDocumentDataForNIC fetchData = new FetchUploadedDocumentDataForNIC();
                if (uploadedDocumentsDTL != null)
                {
                    fetchData.uploaded_doc_id = uploadedDocumentsDTL.uploaded_doc_id;
                    fetchData.usermasteruserid = uploadedDocumentsDTL.userMaster!.userid;
                    fetchData.applicationdtlapplicationid = uploadedDocumentsDTL.applicationDTL!.applicationid;
                    fetchData.document_type_code = uploadedDocumentsDTL.document_type_code;
                    fetchData.document_type = uploadedDocumentsDTL.document_type;
                    fetchData.city_servey_no = uploadedDocumentsDTL.city_servey_no;
                    fetchData.document_name = uploadedDocumentsDTL.document_name;
                    fetchData.document_path = uploadedDocumentsDTL.document_path;
                    TimeZoneInfo INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
                    DateTime indianTime = TimeZoneInfo.ConvertTimeFromUtc(Convert.ToDateTime(uploadedDocumentsDTL.createddatetime.ToString()), INDIAN_ZONE);
                    fetchData.createddatetime = indianTime.ToString("yyyy-MM-dd");
                    fetchData.isdeleted = uploadedDocumentsDTL.isDeleted;
                    fetchData.deleteddate = uploadedDocumentsDTL.deleteddate.ToString("yyyy-MM-dd");
                }
                else
                {
                    fetchData = null!;
                }
                return fetchData;
            }
            catch (Exception ex)
            {
                throw new HandleException(ex.Message.ToString());
            }
        }

        public ApplicationDataForNIC GetApplicationDataForNIC(int userid, string applicationid)
        {
            ApplicationDataForNIC application = new ApplicationDataForNIC();
            // Fetch Registered User Data
            List<FetchUserDataForNIC> userDataList = new List<FetchUserDataForNIC>();

            FetchUserDataForNIC userData = new FetchUserDataForNIC();
            userData = FetchUserData(userid);
            userDataList.Add(userData);
            application.usermaster = userDataList;
            //Fetch All Application Details
            List<FetchApplicationDTLsForNIC> applicationDTLList = new List<FetchApplicationDTLsForNIC>();
            ApplicationDTL applicationDTL = FetchApplicationData(applicationid);

            if (applicationDTL == null)
            {
                throw new HandleException("Application Not Found");
            }
            //Get Application Details
            FetchApplicationDTLsForNIC fetchApplicationDTLsForNIC = new FetchApplicationDTLsForNIC();
            fetchApplicationDTLsForNIC = GetApplicationDtls(applicationDTL);
            applicationDTLList.Add(fetchApplicationDTLsForNIC);

            application.applicationdtl = applicationDTLList;

            //Get All Applicant Details
            List<FetchApplicantsDataForNIC> applicantsList = new List<FetchApplicantsDataForNIC>();
            if (applicationDTL != null && !string.IsNullOrEmpty(applicationDTL.applicantIDs))
            {
                string[] applicantIDS = applicationDTL.applicantIDs.Split(",");

                if (applicantIDS.Length > 0)
                {
                    for (int i = 0; i < applicantIDS.Length; i++)
                    {
                        FetchApplicantsDataForNIC applicant = new FetchApplicantsDataForNIC();
                        applicant = FetchApplicantData(Convert.ToInt32(applicantIDS[i]));
                        if (applicant != null)
                        {
                            applicantsList.Add(applicant);
                        }
                    }
                }
            }
            application.applicantmaster = applicantsList;

            //Get All Mutation CTS Data
            List<FetchMutationCTSNoDataForNIC> mutationCTSNoList = new List<FetchMutationCTSNoDataForNIC>();
            if (applicationDTL != null && !string.IsNullOrEmpty(applicationDTL.mutation_cts_nos))
            {
                string[] mutationCTSNoIDs = applicationDTL.mutation_cts_nos.Split(",");

                if (mutationCTSNoIDs.Length > 0)
                {
                    for (int i = 0; i < mutationCTSNoIDs.Length; i++)
                    {
                        FetchMutationCTSNoDataForNIC mutationCTSNoData = new FetchMutationCTSNoDataForNIC();
                        mutationCTSNoData = FetchMutationCTSData(Convert.ToInt32(mutationCTSNoIDs[i]));
                        if (mutationCTSNoData != null)
                        {
                            mutationCTSNoList.Add(mutationCTSNoData);
                        }
                    }

                }
            }
            else
            {
                FetchMutationCTSNoDataForNIC fetchData = new FetchMutationCTSNoDataForNIC();
                fetchData.mutation_cts_no_id = 0;
                fetchData.usermasteruserid = 0;
                fetchData.applicationdtlapplicationid = "";
                fetchData.what_is_mentioned_in_the_doc = "";
                fetchData.village_or_peth_code = "";
                fetchData.village_or_peth_name = "";
                fetchData.village_english_name = "";
                fetchData.village_lgd_code = "";
                fetchData.zone_code = "";
                fetchData.amount = "";
                fetchData.mutation_modification_type = "";
                fetchData.city_servey_no_mentioned_in_application = "";
                fetchData.servey_no = "";
                fetchData.selected_city_servey_no = "";
                fetchData.lr_property_uid = "";
                fetchData.application_income_type = "";
                fetchData.city_servey_area_in_sq_m = "";
                fetchData.building_name = "";
                fetchData.floor_type = 0;
                fetchData.floor_desc = "";
                fetchData.floor_order_by = 0;
                fetchData.floor_no = "";
                fetchData.unit_code_156 = 0;
                fetchData.unit_name_156 = "";
                fetchData.unit_no = "";
                fetchData.buildup_area_in_sq_m = "";
                fetchData.carpet_area_in_sq_m = "";
                fetchData.terrace_area_in_sq_m = "";
                fetchData.parking_no = "";
                fetchData.parking_area_in_sq_m = "";
                fetchData.shares_in_percent = "";
                fetchData.nic_flat_details = "";
                fetchData.flat_bulit_up_area = "";
                fetchData.sub_property_id = "";
                fetchData.createddatetime = "";
                fetchData.isdeleted = false;
                fetchData.deleteddate = "";
                mutationCTSNoList.Add(fetchData);
            }
            application.mutation_cts_no_dtl = mutationCTSNoList;

            //Get All Dast information
            List<FetchDastInformationDataForNIC> fetchDastInformationList = new List<FetchDastInformationDataForNIC>();
            if (applicationDTL != null && !string.IsNullOrEmpty(applicationDTL.dastIDs))
            {
                string[] dastIDs = applicationDTL.dastIDs.Split(",");
                if (dastIDs.Length > 0)
                {
                    for (int i = 0; i < dastIDs.Length; i++)
                    {
                        FetchDastInformationDataForNIC fetchDastInformationData = new FetchDastInformationDataForNIC();
                        fetchDastInformationData = FetchDastInformationData(Convert.ToInt32(dastIDs[i]));
                        if (fetchDastInformationData != null)
                        {
                            fetchDastInformationList.Add(fetchDastInformationData);
                        }
                    }
                }
            }
            else
            {
                FetchDastInformationDataForNIC fetchData = new FetchDastInformationDataForNIC();
                fetchData.dast_id = 0;
                fetchData.usermasteruserid = 0;
                fetchData.applicationdtlapplicationid = "";
                fetchData.dasttype = "";
                fetchData.division_code = "";
                fetchData.division_name = "";
                fetchData.districtcode = "";
                fetchData.districtname = "";
                fetchData.office_of_the_second_registrar_code = "";
                fetchData.office_of_the_second_registrar_name = "";
                fetchData.registered_dast_no = "";
                fetchData.registered_dast_date = "";
                fetchData.registered_dast_year = "";
                fetchData.dastnabhu = "";
                fetchData.remarks = "";
                fetchData.isdastverified = false;
                fetchData.verifieddastdata = "";
                fetchData.createddatetime = "";
                fetchData.isdeleted = false;
                fetchData.deleteddate = "";
                fetchDastInformationList.Add(fetchData);
            }
            application.dast_information = fetchDastInformationList;

            //Get All courtClaim
            List<FetchCourtClaimInformationDataForNIC> fetchCourtClaimInformationList = new List<FetchCourtClaimInformationDataForNIC>();
            if (applicationDTL!.Is_the_claim_pending_before_the_court == true)
            {
                if (applicationDTL != null && !string.IsNullOrEmpty(applicationDTL.courtClaimIDs))
                {
                    string[] courtClaimIDS = applicationDTL.courtClaimIDs.Split(",");
                    if (courtClaimIDS.Length > 0)
                    {
                        for (int i = 0; i < courtClaimIDS.Length; i++)
                        {
                            FetchCourtClaimInformationDataForNIC fetchCourtClaimInformationData = new FetchCourtClaimInformationDataForNIC();
                            fetchCourtClaimInformationData = FetchCourtClaimInformation(Convert.ToInt32(courtClaimIDS[i]));
                            if (fetchCourtClaimInformationData != null)
                            {
                                fetchCourtClaimInformationList.Add(fetchCourtClaimInformationData);
                            }
                        }
                    }
                }
            }
            else
            {
                FetchCourtClaimInformationDataForNIC fetchCourtClaimInformationData = new FetchCourtClaimInformationDataForNIC();
                fetchCourtClaimInformationData.court_claim_id = 0;
                fetchCourtClaimInformationData.usermasteruserid = 0;
                fetchCourtClaimInformationData.applicationdtlapplicationid = "";
                fetchCourtClaimInformationData.court_case_code = "";
                fetchCourtClaimInformationData.court_case_name = "";
                fetchCourtClaimInformationData.court_case_type_code = "";
                fetchCourtClaimInformationData.court_case_type_name = "";
                fetchCourtClaimInformationData.lr_property_uid = "";
                fetchCourtClaimInformationData.city_servey_no = "";
                fetchCourtClaimInformationData.order_details = "";
                fetchCourtClaimInformationData.stay_order = "";
                fetchCourtClaimInformationData.sub_property_no = "";
                fetchCourtClaimInformationData.createddatetime = "";
                fetchCourtClaimInformationData.isdeleted = false;
                fetchCourtClaimInformationData.deleteddate = "";
                fetchCourtClaimInformationList.Add(fetchCourtClaimInformationData);
            }
            application.court_claim_information = fetchCourtClaimInformationList;

            //Get Power of Attorney Giver & Taker
            List<FetchPOADataForNIC> fetchPOADTL = new List<FetchPOADataForNIC>();
            //List<FetchPOAForGiverDataForNIC> fetchPowerOfAttorneyGiverInformationList = new List<FetchPOAForGiverDataForNIC>();
            //List<FetchPOAForTakerDataForNIC> fetchPowerOfAttorneyInformationList = new List<FetchPOAForTakerDataForNIC>();
            if (applicationDTL!.do_you_have_power_of_attorney == true)
            {
                if (applicationDTL != null && !string.IsNullOrEmpty(applicationDTL.powerOfAttorneyIDs))
                {
                    string[] poaids = applicationDTL.powerOfAttorneyIDs.Split(",");
                    if (poaids.Length > 0)
                    {
                        for (int i = 0; i < poaids.Length; i++)
                        {
                            FetchPOADataForNIC fetcPOAData = new FetchPOADataForNIC();
                            fetcPOAData = FetchPOADTLForNIC(Convert.ToInt32(poaids[i]));
                            if (fetcPOAData != null)
                            {
                                fetchPOADTL.Add(fetcPOAData);
                            }
                        }
                    }
                }

                ////Giver
                //if (applicationDTL != null && !string.IsNullOrEmpty(applicationDTL.powerOfAttorneyIDs))
                //{
                //    string[] powerOfAttorneyIDS = applicationDTL.powerOfAttorneyIDs.Split(",");

                //    if (powerOfAttorneyIDS.Length > 0)
                //    {
                //        for (int i = 0; i < powerOfAttorneyIDS.Length; i++)
                //        {
                //            FetchPOAForGiverDataForNIC fetchPowerOfAttorneyInformationData = new FetchPOAForGiverDataForNIC();
                //            fetchPowerOfAttorneyInformationData = FetchPowerOfAttorneyInfoForGiver(Convert.ToInt32(powerOfAttorneyIDS[i]));
                //            if (fetchPowerOfAttorneyInformationData != null)
                //            {
                //                fetchPowerOfAttorneyGiverInformationList.Add(fetchPowerOfAttorneyInformationData);
                //            }
                //        }
                //    }
                //}
                //Taker
                //if (applicationDTL != null && !string.IsNullOrEmpty(applicationDTL.powerOfAttorneyIDs))
                //{
                //    string[] powerOfAttorneyIDS = applicationDTL.powerOfAttorneyIDs.Split(",");

                //    if (powerOfAttorneyIDS.Length > 0)
                //    {
                //        FetchGiverData fetchGiverData = new FetchGiverData();
                //        fetchGiverData = applicationServices.FetchPOAGiverData(applicationid);
                //        for (int i = 0; i < powerOfAttorneyIDS.Length; i++)
                //        {
                //            FetchPOAForTakerDataForNIC fetchPowerOfAttorneyInformationData = new FetchPOAForTakerDataForNIC();
                //            fetchPowerOfAttorneyInformationData = FetchPowerOfAttorneyInfoForTaker(Convert.ToInt32(powerOfAttorneyIDS[i]));
                //            if (fetchPowerOfAttorneyInformationData != null)
                //            {
                //                //if (fetchGiverData != null)
                //                //{
                //                //    for (int j = 0; j < fetchGiverData.giver_names_in_marathi!.Count; j++)
                //                //    {
                //                //        if (j == 0)
                //                //        {
                //                //            fetchPowerOfAttorneyInformationData.giver_name_in_marathi = fetchGiverData.giver_names_in_marathi[j];
                //                //            fetchPowerOfAttorneyInformationData.giver_name_in_english = fetchGiverData.giver_names_in_english![j];
                //                //        }
                //                //        else
                //                //        {
                //                //            fetchPowerOfAttorneyInformationData.giver_name_in_marathi = fetchPowerOfAttorneyInformationData.giver_name_in_marathi + ", " + fetchGiverData.giver_names_in_marathi[j];
                //                //            fetchPowerOfAttorneyInformationData.giver_name_in_english = fetchPowerOfAttorneyInformationData.giver_name_in_english + ", " + fetchGiverData.giver_names_in_english![j];
                //                //        }
                //                //    }
                //                //}
                //                fetchPowerOfAttorneyInformationList.Add(fetchPowerOfAttorneyInformationData);
                //            }
                //        }
                //    }
                //}
            }
            else
            {
                FetchPOADataForNIC fetcPOAData = new FetchPOADataForNIC();
                fetcPOAData.power_of_attorney_id = 0;
                fetcPOAData.usermasteruserid = 0;
                fetcPOAData.applicationdtlapplicationid = "";
                fetcPOAData.power_of_attorney_code = 0;
                fetcPOAData.is_taker = false;
                fetcPOAData.usertype_code = 0;
                fetcPOAData.usertype = "";
                fetcPOAData.mutation_id = 0;
                fetcPOAData.mobileno = "";
                fetcPOAData.mobilenoverified = "";
                fetcPOAData.emailid = "";
                fetcPOAData.emailidverified = "";
                fetcPOAData.prefixcode_marathi = "";
                fetcPOAData.prefix_in_marathi = "";
                fetcPOAData.fname_in_marathi = "";
                fetcPOAData.mname_in_marathi = "";
                fetcPOAData.lname_in_marathi = "";
                fetcPOAData.prefixcode_eng = "";
                fetcPOAData.prefix_in_eng = "";
                fetcPOAData.fname_in_eng = "";
                fetcPOAData.mname_in_eng = "";
                fetcPOAData.lname_in_eng = "";
                fetcPOAData.company_name_in_marathi = "";
                fetcPOAData.company_name_in_eng = "";
                fetcPOAData.username = "";
                fetcPOAData.alias_name = "";
                fetcPOAData.gender_code = "";
                fetcPOAData.gender_description = "";
                fetcPOAData.dob = "";
                fetcPOAData.mother_name_in_marathi = "";
                fetcPOAData.mother_name_in_eng = "";
                fetcPOAData.address_type = "";
                fetcPOAData.address = "";
                fetcPOAData.state = "";
                fetcPOAData.district = "";
                fetcPOAData.taluka = "";
                fetcPOAData.city = "";
                fetcPOAData.flatno_plotno = "";
                fetcPOAData.societyname = "";
                fetcPOAData.mainstreet = "";
                fetcPOAData.landmark = "";
                fetcPOAData.locality = "";
                fetcPOAData.pincode = "";
                fetcPOAData.postofficename = "";
                fetcPOAData.address_proof_document_name = "";
                fetcPOAData.address_proof_document_path = "";
                fetcPOAData.city_servey_no = "";
                fetcPOAData.lr_property_id = "";
                fetcPOAData.sub_property_no = "";
                fetcPOAData.owner_of_property_in_maharashtra = false;
                fetcPOAData.mutation_srno = "";
                fetcPOAData.owner_number = "";
                fetcPOAData.cts_number = "";
                fetcPOAData.village_code = "";
                fetcPOAData.village_name = "";
                fetcPOAData.propertytypeid = 0;
                fetcPOAData.property_district_code = "";
                fetcPOAData.property_district_name_in_marathi = "";
                fetcPOAData.property_district_name_in_english = "";
                fetcPOAData.property_taluka_code = "";
                fetcPOAData.property_taluka_name = "";
                fetcPOAData.property_city_code = "";
                fetcPOAData.property_city_name = "";
                fetcPOAData.khateno = "";
                fetcPOAData.ulpin = "";
                fetcPOAData.khata_type_code = "";
                fetcPOAData.khata_type_name = "";
                fetcPOAData.owner_status_code = "";
                fetcPOAData.owner_status_description = "";
                fetcPOAData.attornytype_code = 0;
                fetcPOAData.attornytype_desc = "";
                fetcPOAData.landbuyarea = "";
                fetcPOAData.ispoaispartofdast = "";
                fetcPOAData.isdeclerationinvolvedinpoa = "";
                fetcPOAData.ispoapermanant = "";
                fetcPOAData.istransferrights = "";
                fetcPOAData.dast_no = "";
                fetcPOAData.dast_no_date = "";
                fetcPOAData.dast_no_year = "";
                fetcPOAData.isdastverified = false;
                fetcPOAData.verifieddastdata = "";
                fetcPOAData.digcode = 0;
                fetcPOAData.digname = "";
                fetcPOAData.poa_district_code = "";
                fetcPOAData.poa_district_name = "";
                fetcPOAData.sro_office_code = 0;
                fetcPOAData.sro_office_name = "";
                fetcPOAData.deleteddate = "";
                fetcPOAData.createddatetime = "";
                fetcPOAData.isdeleted = false;
                fetcPOAData.signed_file_path = "";
                fetcPOAData.signed_file_name = "";
                fetcPOAData.profile_pic_file_name = "";
                fetcPOAData.profile_pic_file_path = "";
                fetcPOAData.poa_giver_ids = "";
                fetchPOADTL.Add(fetcPOAData);
            }
            application.power_of_attorney_information = fetchPOADTL;

            //Get Mayat Data ,mrutyu dakhala and varas nond
            List<FetchMayatDTLForNIC> fetchMrutyuDetails = new List<FetchMayatDTLForNIC>();
            List<FetchVarasNondDetailsData> fetchvarasNondList = new List<FetchVarasNondDetailsData>();
            //if (applicationDTL!.mutation_type_code == "01")
            //{
            if (!string.IsNullOrEmpty(applicationDTL!.mayatIDs))
            {
                string[] kharediNondIDs = applicationDTL.mayatIDs!.Split(",");
                if (kharediNondIDs.Length > 0)
                {
                    for (int i = 0; i < kharediNondIDs.Length; i++)
                    {
                        FetchMayatDTLForNIC fetchmayatData = new FetchMayatDTLForNIC();
                        fetchmayatData = FetchMayatDetails(Convert.ToInt32(kharediNondIDs[i]));
                        if (fetchmayatData != null)
                        {
                            fetchMrutyuDetails.Add(fetchmayatData);
                        }
                    }
                }
            }
            else
            {
                FetchMayatDTLForNIC fetchData = new FetchMayatDTLForNIC();
                fetchData.mayat_id = 0;
                fetchData.usermasteruserid = 0;
                fetchData.applicationDTLapplicationid = "";
                fetchData.mutation_cts_no_id = 0;
                fetchData.mobileno = "";
                fetchData.mobilenoverified = "";
                fetchData.emailid = "";
                fetchData.emailidverified = "";
                fetchData.prefixcode_marathi = "";
                fetchData.prefix_in_marathi = "";
                fetchData.fname_in_marathi = "";
                fetchData.mname_in_marathi = "";
                fetchData.lname_in_marathi = "";
                fetchData.prefixcode_eng = "";
                fetchData.prefix_in_eng = "";
                fetchData.fname_in_eng = "";
                fetchData.mname_in_eng = "";
                fetchData.lname_in_eng = "";
                fetchData.alias_name = "";
                fetchData.address_type = "";
                fetchData.address = "";
                fetchData.state = "";
                fetchData.district = ""; ;
                fetchData.taluka = "";
                fetchData.city = "";
                fetchData.flatno_plotno = "";
                fetchData.societyname = "";
                fetchData.mainstreet = "";
                fetchData.landmark = "";
                fetchData.locality = "";
                fetchData.pincode = "";
                fetchData.post_office_name = "";
                fetchData.city_servey_no = "";
                fetchData.lr_property_id = "";
                fetchData.milkat = "";
                fetchData.namud = "";
                fetchData.sub_property_no = "";
                fetchData.mutation_srno = "";
                fetchData.owner_number = "";
                fetchData.cts_number = "";
                fetchData.actual_area = "";
                fetchData.mrutyu_date = "";
                fetchData.certificate_authority_code = "";
                fetchData.certificate_authority_name = "";
                fetchData.mrutyucert_no = "";
                fetchData.mrutyu_certificate_date = "";
                fetchData.mrutyu_certificate__name = "";
                fetchData.mrutyu_certificate_path = "";
                fetchData.is_name_same = "";
                fetchData.reason = "";
                fetchData.namecorrect_docname = "";
                fetchData.namecorrect_docpath = "";
                fetchData.address_proof_document_name = "";
                fetchData.address_proof_document_path = "";
                fetchData.signed_file_name = "";
                fetchData.signed_file_path = "";
                fetchData.isdeleted = false;
                fetchData.deleteddate = "";
                fetchData.createddatetime = "";
                //Below are new fields -> Added on 31 Dec 25
                fetchData.probet_file_name = "";
                fetchData.probet_file_path = "";
                fetchData.isprobet = false;
                fetchMrutyuDetails.Add(fetchData);
            }
            //if (!string.IsNullOrEmpty(applicationDTL.varasIDS))
            //{
            //    string[] kharediNondIDs = applicationDTL.varasIDS.Split(",");
            //    if (kharediNondIDs.Length > 0)
            //    {
            //        for (int i = 0; i < kharediNondIDs.Length; i++)
            //        {
            //            FetchVarasNondDetailsData fetchdata = new FetchVarasNondDetailsData();
            //            fetchdata = FetchVarasData(Convert.ToInt32(kharediNondIDs[i]));
            //            if (fetchdata != null)
            //            {
            //                fetchvarasNondList.Add(fetchdata);
            //            }
            //        }
            //    }
            //}
            //}
            application.mayatdtl = fetchMrutyuDetails;
            //application.varasNondDetailsData = fetchvarasNondList;
            //string mutationgivertype = "";
            //string mutationtakertype = "";
            //string varasnond = "";
            //if (applicationDTL.mutation_type_code == "01")
            //{
            //    mutationgivertype = "मयताची माहिती";
            //    mutationtakertype = "मृत्यू दाखला माहिती";
            //    varasnond = "वारस नोंद माहिती";
            //}
            //else if (applicationDTL.mutation_type_code == "03")
            //{
            //    mutationgivertype = "खरेदीनोंद देणार";
            //    mutationtakertype = "खरेदीनोंद घेणार";
            //}
            ////Bakshish Patra
            //else if (applicationDTL.mutation_type_code == "04")
            //{
            //    mutationgivertype = "बक्षीसपत्र देणार";
            //    mutationtakertype = "बक्षीसपत्र घेणार";
            //}
            ////Mrutyu Nond
            //else if (applicationDTL.mutation_type_code == "05")
            //{
            //    mutationgivertype = "मृत्यूपत्र/इच्छापत्र देणार";
            //    mutationtakertype = "मृत्यूपत्र/इच्छापत्र घेणार";
            //}
            ////Gahankhat Nond
            //else if (applicationDTL.mutation_type_code == "06")
            //{
            //    mutationgivertype = "गाहाणखत/तारण/बोजा देणार";
            //    mutationtakertype = "गाहाणखत/तारण/बोजा घेणार";

            //}
            ////HakkaSod Nond
            //else if (applicationDTL.mutation_type_code == "09")
            //{
            //    mutationgivertype = "हक्कसोड पत्र/रिलीज डिड देणार";
            //    mutationtakertype = "हक्कसोड पत्र/रिलीज डिड घेणार";
            //}


            List<FetcMutationGiverTakerDTL> fetcMutationGiverTakerDTLs = new List<FetcMutationGiverTakerDTL>();
            if (!string.IsNullOrEmpty(applicationDTL.mutationgiverIDs))
            {
                string[] mutationGiverTakerIDS = applicationDTL.mutationgiverIDs.Split(",");
                if (mutationGiverTakerIDS.Length > 0)
                {
                    for (int i = 0; i < mutationGiverTakerIDS.Length; i++)
                    {
                        FetcMutationGiverTakerDTL fetchdata = new FetcMutationGiverTakerDTL();
                        fetchdata = FetchMutationDTL(Convert.ToInt32(mutationGiverTakerIDS[i]), applicationid);
                        if (fetchdata != null)
                        {
                            fetcMutationGiverTakerDTLs.Add(fetchdata);
                        }
                    }
                }
            }
            if (!string.IsNullOrEmpty(applicationDTL.mutationtakerIDs))
            {
                string[] mutationGiverTakerIDS = applicationDTL.mutationtakerIDs.Split(",");
                if (mutationGiverTakerIDS.Length > 0)
                {
                    for (int i = 0; i < mutationGiverTakerIDS.Length; i++)
                    {
                        FetcMutationGiverTakerDTL fetchdata = new FetcMutationGiverTakerDTL();
                        fetchdata = FetchMutationDTL(Convert.ToInt32(mutationGiverTakerIDS[i]), applicationid);
                        if (fetchdata != null)
                        {
                            fetcMutationGiverTakerDTLs.Add(fetchdata);
                        }
                    }
                }
            }
            else if (!string.IsNullOrEmpty(applicationDTL.varasIDS))
            {
                string[] varasids = applicationDTL.varasIDS.Split(",");
                if (varasids.Length > 0)
                {
                    for (int i = 0; i < varasids.Length; i++)
                    {
                        FetcMutationGiverTakerDTL fetchdata = new FetcMutationGiverTakerDTL();
                        fetchdata = FetchMutationDTL(Convert.ToInt32(varasids[i]), applicationid);
                        if (fetchdata != null)
                        {
                            fetcMutationGiverTakerDTLs.Add(fetchdata);
                        }
                    }
                }
            }
            else
            {
                FetcMutationGiverTakerDTL fetchData = new FetcMutationGiverTakerDTL();
                fetchData.mutation_givertaker_id = 0;
                fetchData.usermasteruserid = 0;
                fetchData.applicationdtlapplicationid = "";
                fetchData.user_type_code = 0;
                fetchData.user_type = "";
                fetchData.prop_typepropertytypeid = "";
                fetchData.istaker = 0;
                fetchData.mobileno = "";
                fetchData.mobilenoverified = "";
                fetchData.emailid = "";
                fetchData.emailidverified = "";
                fetchData.prefixcode_marathi = "";
                fetchData.prefix_in_marathi = "";
                fetchData.fname_in_marathi = "";
                fetchData.mname_in_marathi = "";
                fetchData.lname_in_marathi = "";
                fetchData.prefixcode_eng = "";
                fetchData.prefix_in_eng = "";
                fetchData.fname_in_eng = "";
                fetchData.mname_in_eng = "";
                fetchData.lname_in_eng = "";
                fetchData.alias_name = "";
                fetchData.company_name_in_marathi = "";
                fetchData.company_name_in_eng = "";
                fetchData.gender_code = "";
                fetchData.gender_description = "";
                fetchData.holder_type = "";
                fetchData.dob = "";
                fetchData.mother_name_in_marathi = "";
                fetchData.mother_name_in_eng = "";
                fetchData.username = "";
                fetchData.city_servey_no = "";
                fetchData.lr_property_id = "";
                fetchData.sub_property_no = "";
                fetchData.sellerid = "";
                fetchData.buyerid = "";
                fetchData.mutation_srno = "";
                fetchData.owner_number = "";
                fetchData.cts_number = "";
                fetchData.milkat = "";
                fetchData.namud = "";
                fetchData.isfullareagiven = "";
                fetchData.actual_area = "";
                fetchData.available_area = "";
                fetchData.mutation_area = "";
                fetchData.address_type = "";
                fetchData.address = "";
                fetchData.state = "";
                fetchData.district = "";
                fetchData.taluka = "";
                fetchData.city = "";
                fetchData.flatno_plotno = "";
                fetchData.societyname = "";
                fetchData.mainstreet = "";
                fetchData.landmark = "";
                fetchData.locality = "";
                fetchData.pincode = "";
                fetchData.post_office_name = "";
                fetchData.address_proof_document_name = "";
                fetchData.address_proof_document_path = "";
                fetchData.has_property = "";
                fetchData.aapak = "";
                fetchData.land_buy_area = "";
                fetchData.account_type_code = 0;
                fetchData.account_type_description = "";
                fetchData.apk_code = 0;
                fetchData.apk_description = "";
                fetchData.khata_type_code = "";
                fetchData.khata_type_name = "";
                fetchData.owner_status_code = "";
                fetchData.owner_status_description = "";
                fetchData.khatano = "";
                fetchData.ulpin = "";
                fetchData.district_code = "";
                fetchData.district_name_in_marathi = "";
                fetchData.district_name_in_eng = "";
                fetchData.village_code = "";
                fetchData.village_name = "";
                fetchData.ofc_code = "";
                fetchData.ofc_name = "";
                fetchData.relation_code = 0;
                fetchData.relation_name = "";
                fetchData.varas_relation_code = 0;
                fetchData.varas_relation_name = "";
                fetchData.institute_code = 0;
                fetchData.institute_description = "";
                fetchData.bank_name_in_marathi = "";
                fetchData.bank_name_in_english = "";
                fetchData.ifsc = "";
                fetchData.boja_value = "";
                fetchData.boja_date = "";
                fetchData.boja_period = "";
                fetchData.benefit_amt = "";
                fetchData.isdeleted = false;
                fetchData.deleteddate = "";
                fetchData.createddatetime = "";
                fetchData.signed_file_name = "";
                fetchData.signed_file_path = "";
                fetchData.profile_pic_file_name = "";
                fetchData.profile_pic_file_path = "";
                fetchData.mutation_cts_no_id = 0;
                fetcMutationGiverTakerDTLs.Add(fetchData);
            }
            //Fetch Bhadepatta Info Data
            List<FetchBhadepattaInfoDTLForNIC> fetchBhadepattaInfoList = new List<FetchBhadepattaInfoDTLForNIC>();
            if (applicationDTL.mutation_type_code == "10")
            {
                FetchBhadepattaInfoDTLForNIC fetchBhadepattaInfo = new FetchBhadepattaInfoDTLForNIC();
                fetchBhadepattaInfo = FetchBhadepattaInfoDTL(applicationid);
                fetchBhadepattaInfoList.Add(fetchBhadepattaInfo);
            }
            else
            {
                FetchBhadepattaInfoDTLForNIC fetchBhadepattaInfo = new FetchBhadepattaInfoDTLForNIC();
                fetchBhadepattaInfo.Info_id = 0;
                fetchBhadepattaInfo.userid = 0;
                fetchBhadepattaInfo.applicationid = "";
                fetchBhadepattaInfo.bhadepattaTenureYear = "";
                fetchBhadepattaInfo.bhadepattaTenureMonth = "";
                fetchBhadepattaInfo.bhadepattaAmount = "";
                fetchBhadepattaInfo.leaseperiod = true;
                fetchBhadepattaInfo.bhadepattaToDate = "";
                fetchBhadepattaInfo.bhadepattaFromDate = "";
                fetchBhadepattaInfo.createdDateTime = "";
                fetchBhadepattaInfo.deletedDateTime = "";
                fetchBhadepattaInfo.isDeleted = false;
                fetchBhadepattaInfoList.Add(fetchBhadepattaInfo);
            }
            //Fetch Hibanama Info Data
            List<FetchHibanamaWitnessDataForNIC> fetchHibanamaWitnessInfoDataList = new List<FetchHibanamaWitnessDataForNIC>();
            if (!string.IsNullOrEmpty(applicationDTL.witnessids))
            {
                string[] witnessIds = applicationDTL.witnessids.Split(",");
                if (witnessIds.Length > 0)
                {
                    for (int i = 0; i < witnessIds.Length; i++)
                    {
                        FetchHibanamaWitnessDataForNIC fetchData = new FetchHibanamaWitnessDataForNIC();
                        fetchData = FetchHibanamaWitnessData(Convert.ToInt32(witnessIds[i]));
                        if (fetchData != null)
                        {
                            fetchHibanamaWitnessInfoDataList.Add(fetchData);
                        }
                    }
                }
            }
            else
            {
                FetchHibanamaWitnessDataForNIC fetchData = new FetchHibanamaWitnessDataForNIC();
                fetchData.witness_info_id = 0;
                fetchData.usermasteruserid = 0;
                fetchData.applicationdtlapplicationid = "";
                fetchData.permission_no = "";
                fetchData.permission_date = "";
                fetchData.prefixcode_marathi = "";
                fetchData.prefix_in_marathi = "";
                fetchData.fname_in_marathi = "";
                fetchData.mname_in_marathi = "";
                fetchData.lname_in_marathi = "";
                fetchData.prefixcode_eng = "";
                fetchData.prefix_in_eng = "";
                fetchData.fname_in_eng = "";
                fetchData.mname_in_eng = "";
                fetchData.lname_in_eng = "";
                fetchData.alias_name = "";
                fetchData.address_type = "";
                fetchData.address = "";
                fetchData.state = "";
                fetchData.district = "";
                fetchData.taluka = "";
                fetchData.city = "";
                fetchData.flatno_plotno = "";
                fetchData.societyname = "";
                fetchData.mainstreet = "";
                fetchData.landmark = "";
                fetchData.locality = "";
                fetchData.pincode = "";
                fetchData.post_office_name = "";
                fetchData.address_proof_document_name = "";
                fetchData.address_proof_document_path = "";
                fetchData.mobileno = "";
                fetchData.mobilenoverified = "";
                fetchData.emailid = "";
                fetchData.emailidverified = "";
                fetchData.createddatetime = "";
                fetchData.isdeleted = false;
                fetchData.deleteddate = "";
                fetchHibanamaWitnessInfoDataList.Add(fetchData);
            }
            //Fetch Error Correction data
            List<FetchErrorCorrectionDataForNIC> fetchErrorCorrectionDataList = new List<FetchErrorCorrectionDataForNIC>();
            if (!string.IsNullOrEmpty(applicationDTL.errorcorrectionids))
            {
                string[] errorcorrectionIDS = applicationDTL.errorcorrectionids.Split(",");
                if (errorcorrectionIDS.Length > 0)
                {
                    for (int i = 0; i < errorcorrectionIDS.Length; i++)
                    {
                        FetchErrorCorrectionDataForNIC fetchErrorCorrectionData = new FetchErrorCorrectionDataForNIC();
                        fetchErrorCorrectionData = FetchErrorCorrectionData(Convert.ToInt32(errorcorrectionIDS[i]));
                        if (fetchErrorCorrectionData != null)
                        {
                            fetchErrorCorrectionDataList.Add(fetchErrorCorrectionData);
                        }
                    }
                }
            }
            else
            {
                FetchErrorCorrectionDataForNIC fetchData = new FetchErrorCorrectionDataForNIC();
                fetchData.error_correction_id = 0;
                fetchData.usermasteruserid = 0;
                fetchData.applicationdtlapplicationid = "";
                fetchData.village_code = "";
                fetchData.sub_property_no = "";
                fetchData.city_servey_no = "";
                fetchData.lr_property_id = "";
                fetchData.milkat = "";
                fetchData.namud = "";
                //fetchData.var_village_code = "";
                //fetchData.var_cts_number =
                //fetchData.var_cts_puid =
                //fetchData.var_mutation_srno =
                //fetchData.var_entry_date =
                //fetchData.var_mutation_number =
                //fetchData.var_mutation_date =
                //fetchData.var_sro_office_name_marathi =
                //fetchData.var_sro_office_name_english =
                //fetchData.var_document_number =
                //fetchData.var_document_year =
                //fetchData.var_document_date =
                //fetchData.var_entry_details =
                //fetchData.var_owner_details =
                fetchData.reason = "";
                fetchData.address_type = "";
                fetchData.emailid = "";
                fetchData.mobileno = "";
                fetchData.mobilenoverified = "";
                fetchData.address = "";
                fetchData.state = "";
                fetchData.district = "";
                fetchData.taluka = "";
                fetchData.city = "";
                fetchData.flatno_plotno = "";
                fetchData.societyname = "";
                fetchData.mainstreet = "";
                fetchData.landmark = "";
                fetchData.locality = "";
                fetchData.pincode = "";
                fetchData.post_office_name = "";
                fetchData.address_proof_document_name = "";
                fetchData.address_proof_document_path = "";
                fetchData.createddatetime = "";
                fetchData.isdeleted = false;
                fetchData.deleteddate = "";
                fetchErrorCorrectionDataList.Add(fetchData);
            }

            // Fetch Navatbadal Data
            List<FetchNavatBadalDataForNIC> fetchNavatBadalDataList = new List<FetchNavatBadalDataForNIC>();
            if (!string.IsNullOrEmpty(applicationDTL.namechangeids))
            {
                string[] namechangeIDS = applicationDTL.namechangeids.Split(",");
                if (namechangeIDS.Length > 0)
                {
                    for (int i = 0; i < namechangeIDS.Length; i++)
                    {
                        FetchNavatBadalDataForNIC fetchNavatBadalData = new FetchNavatBadalDataForNIC();
                        fetchNavatBadalData = FetchNavatBadalData(Convert.ToInt32(namechangeIDS[i]));
                        if (fetchNavatBadalData != null)
                        {
                            fetchNavatBadalDataList.Add(fetchNavatBadalData);
                        }
                    }
                }
            }
            else
            {
                FetchNavatBadalDataForNIC fetchData = new FetchNavatBadalDataForNIC();
                fetchData.name_change_id = 0;
                fetchData.usermasteruserid = 0;
                fetchData.applicationdtlapplicationid = "";
                fetchData.village_code = "";
                fetchData.subpropno = "";
                fetchData.nabhu = "";
                fetchData.lrpropertyuid = "";
                fetchData.milkat = "";
                fetchData.namud = "";
                fetchData.name_change_by_code = 0;
                fetchData.name_change_by_description = "";
                fetchData.name_change_no = "";
                fetchData.name_change_date = "";
                // selectedMutation -> data from EPCIS
                fetchData.selected_village_code = "";
                fetchData.selected_cts_number = "";
                fetchData.selected_mutation_srno = "";
                fetchData.selected_entry_date = "";
                fetchData.selected_entry_bracketed = "";
                fetchData.selected_owner_number = "";
                fetchData.selected_owner_name = "";
                fetchData.selected_first_name = "";
                fetchData.selected_middle_name = "";
                fetchData.selected_last_name = "";
                fetchData.selected_nick_name = "";
                fetchData.selected_owner_bracketed = "";
                fetchData.selected_area_bracketed = "";
                fetchData.selected_email_id = "";
                fetchData.selected_owner_cell_number = "";
                fetchData.selected_pincode = "";
                fetchData.selected_owner_type = "";
                fetchData.selected_apk_code = "";
                fetchData.selected_apk_name = "";
                fetchData.selected_flat_or_house_number = "";
                fetchData.selected_building_number = "";
                fetchData.selected_road = "";
                fetchData.selected_city_or_village = "";
                fetchData.selected_taluka_name = "";
                fetchData.selected_district_name = "";
                fetchData.selected_state_name = "";
                fetchData.selected_gender_code = "";
                fetchData.selected_date_of_birth = "";
                fetchData.selected_owner_area = "";
                fetchData.selected_owner_area_bracketed = "";
                // Updated Details
                fetchData.updated_usertype = 0;
                fetchData.updated_usertypelabel = "";
                fetchData.updated_prefixcode_marathi = "";
                fetchData.updated_prefix_in_marathi = "";
                fetchData.updated_fname_in_marathi = "";
                fetchData.updated_mname_in_marathi = "";
                fetchData.updated_lname_in_marathi = "";
                fetchData.updated_prefixcode_eng = "";
                fetchData.updated_prefix_in_eng = "";
                fetchData.updated_fname_in_eng = "";
                fetchData.updated_mname_in_eng = "";
                fetchData.updated_lname_in_eng = "";
                fetchData.company_name_in_marathi = "";
                fetchData.company_name_in_eng = "";
                // Address Fields
                fetchData.address_type = "";
                fetchData.emailid = "";
                fetchData.mobileno = "";
                fetchData.mobilenoverified = "";
                fetchData.address = "";
                fetchData.state = "";
                fetchData.district = "";
                fetchData.taluka = "";
                fetchData.city = "";
                fetchData.flatno_plotno = "";
                fetchData.societyname = "";
                fetchData.mainstreet = "";
                fetchData.landmark = "";
                fetchData.locality = "";
                fetchData.pincode = "";
                fetchData.postofficename = "";
                fetchData.address_proof_document_name = "";
                fetchData.address_proof_document_path = "";
                fetchData.createddatetime = "";
                fetchData.isdeleted = false;
                fetchData.deleteddate = "";
                fetchNavatBadalDataList.Add(fetchData);
            }
            //            //Mutation Giver 
            //            List<dynamic> giver = mutationgiverData(applicationDTL);
            ////Mutation Taker
            //List<dynamic> taker = mutationtakerData(applicationDTL);

            //List<MutationList> mutation = new List<MutationList>();
            //if (applicationDTL.mutation_type_code == "01")
            //{
            //    mutation.Add(new MutationList { type = mutationgivertype, value = fetchmayatinfoList });
            //    mutation.Add(new MutationList { type = mutationtakertype, value = fetchMrutyuDetails });
            //    mutation.Add(new MutationList { type = varasnond, value = fetchvarasNondList });
            //}
            //else
            //{
            //mutation.Add(new MutationList { type = mutationgivertype, value = giver });
            //mutation.Add(new MutationList { type = mutationtakertype, value = taker });
            //}

            application.mutationgivertakerdtls = fetcMutationGiverTakerDTLs;
            application.bhadepattaInfoDtl = fetchBhadepattaInfoList;
            application.errorcorrectiondtls = fetchErrorCorrectionDataList;
            application.name_change_dtl = fetchNavatBadalDataList;
            application.witness_info = fetchHibanamaWitnessInfoDataList;
            //Document Data

            var nabhuNoList = _contextR.mutationCTSNoDTLs.Include(app => app.applicationDTL).Where(app => app.applicationDTL!.applicationid == applicationid).Select(a => a.selected_city_servey_no).ToList();
            //string nabh  = String.Join(", ", nabhuNoList);
            //List<FetchUploadedDocumentsForNIC> document = new List<FetchUploadedDocumentsForNIC>();
            //foreach (string? nabhu in nabhuNoList)
            //{
            //    var data = uploadDocuments(nabhu!, applicationid);
            //    if (data != null)
            //    {
            //        document.Add(data);
            //    }
            //}
            //application.uploaded_documents_dtl = document;

            List<FetchUploadedDocumentDataForNIC> fetchUploadedDocumentsList = new List<FetchUploadedDocumentDataForNIC>();
            if (applicationDTL != null && !string.IsNullOrEmpty(applicationDTL.uploadedDocIDs))
            {
                string[] uploadedDocIDs = applicationDTL.uploadedDocIDs.Split(",");

                if (uploadedDocIDs.Length > 0)
                {
                    for (int i = 0; i < uploadedDocIDs.Length; i++)
                    {
                        FetchUploadedDocumentDataForNIC fetchUploadedDocumentsData = new FetchUploadedDocumentDataForNIC();
                        fetchUploadedDocumentsData = FetchUploadedDocumentDTLs(Convert.ToInt32(uploadedDocIDs[i]));
                        if (fetchUploadedDocumentsData != null)
                        {
                            fetchUploadedDocumentsList.Add(fetchUploadedDocumentsData);
                        }
                    }
                }
            }
            else
            {
                FetchUploadedDocumentDataForNIC fetchData = new FetchUploadedDocumentDataForNIC();
                fetchData.uploaded_doc_id = 0;
                fetchData.usermasteruserid = 0;
                fetchData.applicationdtlapplicationid = "";
                fetchData.document_type_code = "";
                fetchData.document_type = "";
                fetchData.city_servey_no = "";
                fetchData.document_name = "";
                fetchData.document_path = "";
                fetchData.createddatetime = "";
                fetchData.isdeleted = false;
                fetchData.deleteddate = "";
                fetchUploadedDocumentsList.Add(fetchData);
            }
            application.uploaded_documents_dtl = fetchUploadedDocumentsList;
            return application;
        }

        //public List<dynamic> mutationgiverData(ApplicationDTL applicationDTL)
        //{
        //    List<dynamic> mutationgiver = new List<dynamic>();
        //    //Kharedi Nond Giver
        //    if (applicationDTL.mutation_type_code == "03")
        //    {
        //        mutationgiver = new List<dynamic>();
        //        if (!string.IsNullOrEmpty(applicationDTL.mutationgiverIDs))
        //        {
        //            string[] kharediNondIDs = applicationDTL.mutationgiverIDs.Split(",");

        //            if (kharediNondIDs.Length > 0)
        //            {
        //                for (int i = 0; i < kharediNondIDs.Length; i++)
        //                {
        //                    FetchKharediNondDataForGiver fetchKharediNondGiverInformationData = new FetchKharediNondDataForGiver();
        //                    fetchKharediNondGiverInformationData = FetchKhrediNondInformationDataForGiver(Convert.ToInt32(kharediNondIDs[i]));
        //                    if (fetchKharediNondGiverInformationData != null)
        //                    {
        //                        mutationgiver.Add(fetchKharediNondGiverInformationData);
        //                    }
        //                }

        //            }
        //        }
        //    }
        //    ////Bakshish Patra
        //    //else if (applicationDTL.mutation_type_code == "04")
        //    //{
        //    //    mutationgiver = new List<dynamic>();
        //    //    if (!string.IsNullOrEmpty(applicationDTL.mutationgiverIDs))
        //    //    {
        //    //        string[] mutationgiverIDs = applicationDTL.mutationgiverIDs.Split(",");
        //    //        if (mutationgiverIDs.Length > 0)
        //    //        {
        //    //            for (int i = 0; i < mutationgiverIDs.Length; i++)
        //    //            {
        //    //                FetchBakshishPatraDataForGiver fetchKharediNondGiverInformationData = new FetchBakshishPatraDataForGiver();
        //    //                fetchKharediNondGiverInformationData = FetchBakshishPatraInformationDataForGiver(Convert.ToInt32(mutationgiverIDs[i]));
        //    //                if (fetchKharediNondGiverInformationData != null)
        //    //                {
        //    //                    mutationgiver.Add(fetchKharediNondGiverInformationData);
        //    //                }
        //    //            }

        //    //        }
        //    //    }
        //    //}
        //    ////Mrutyu Nond
        //    //else if (applicationDTL.mutation_type_code == "05")
        //    //{
        //    //    if (applicationDTL != null && !string.IsNullOrEmpty(applicationDTL.mutationgiverIDs))
        //    //    {
        //    //        string[] mutationgiverIDs = applicationDTL.mutationgiverIDs.Split(",");

        //    //        if (mutationgiverIDs.Length > 0)
        //    //        {
        //    //            for (int i = 0; i < mutationgiverIDs.Length; i++)
        //    //            {
        //    //                FetchMrutyuPatraForGiverData fetchData = new FetchMrutyuPatraForGiverData();
        //    //                fetchData = FetchMrutyuPatraInfoForGiver(Convert.ToInt32(mutationgiverIDs[i]));
        //    //                if (fetchData != null)
        //    //                {
        //    //                    mutationgiver.Add(fetchData);
        //    //                }
        //    //            }

        //    //        }
        //    //    }
        //    //}
        //    ////Gahankhat Nond
        //    //else if (applicationDTL.mutation_type_code == "06")
        //    //{
        //    //    if (!string.IsNullOrEmpty(applicationDTL.mutationgiverIDs))
        //    //    {
        //    //        string[] mutationgiverIDs = applicationDTL!.mutationgiverIDs!.Split(",");
        //    //        for (int i = 0; i < mutationgiverIDs.Length; i++)
        //    //        {
        //    //            FetchGahankhatForGiverData fetchData = new FetchGahankhatForGiverData();
        //    //            fetchData = FetchGahankhatInfoForGiver(Convert.ToInt32(mutationgiverIDs[i]));
        //    //            if (fetchData != null)
        //    //            {
        //    //                mutationgiver.Add(fetchData);
        //    //            }
        //    //        }
        //    //    }

        //    //}
        //    ////HakkaSod Nond
        //    //else if (applicationDTL.mutation_type_code == "09")
        //    //{
        //    //    if (applicationDTL != null && !string.IsNullOrEmpty(applicationDTL.mutationgiverIDs))
        //    //    {
        //    //        string[] mutationgiverIDs = applicationDTL.mutationgiverIDs.Split(",");

        //    //        if (mutationgiverIDs.Length > 0)
        //    //        {
        //    //            for (int i = 0; i < mutationgiverIDs.Length; i++)
        //    //            {
        //    //                FetchHakkasodForGiverData fetchData = new FetchHakkasodForGiverData();
        //    //                fetchData = FetchHakkasodInfoForGiver(Convert.ToInt32(mutationgiverIDs[i]));
        //    //                if (fetchData != null)
        //    //                {
        //    //                    mutationgiver.Add(fetchData);
        //    //                }
        //    //            }
        //    //        }
        //    //    }
        //    //}
        //    return mutationgiver;
        //}

        //public List<dynamic> mutationtakerData(ApplicationDTL applicationDTL)
        //{
        //    List<dynamic> mutationtaker = new List<dynamic>();
        //    //Kharedi Nond Giver
        //    if (applicationDTL.mutation_type_code == "03")
        //    {
        //        mutationtaker = new List<dynamic>();
        //        if (!string.IsNullOrEmpty(applicationDTL.mutationtakerIDs))
        //        {
        //            string[] kharediNondIDs = applicationDTL.mutationtakerIDs!.Split(",");

        //            if (kharediNondIDs.Length > 0)
        //            {
        //                for (int i = 0; i < kharediNondIDs.Length; i++)
        //                {
        //                    FetchKharediNondDataForTaker fetchKharediNondGiverInformationData = new FetchKharediNondDataForTaker();
        //                    fetchKharediNondGiverInformationData = FetchKhrediNondInformationDataForTaker(Convert.ToInt32(kharediNondIDs[i]));
        //                    if (fetchKharediNondGiverInformationData != null)
        //                    {
        //                        mutationtaker.Add(fetchKharediNondGiverInformationData);
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    ////Bakshish Patra
        //    //else if (applicationDTL.mutation_type_code == "04")
        //    //{
        //    //    mutationtaker = new List<dynamic>();
        //    //    if (!string.IsNullOrEmpty(applicationDTL.mutationtakerIDs))
        //    //    {
        //    //        string[] kharediNondIDs = applicationDTL.mutationtakerIDs!.Split(",");

        //    //        if (kharediNondIDs.Length > 0)
        //    //        {
        //    //            for (int i = 0; i < kharediNondIDs.Length; i++)
        //    //            {
        //    //                FetchBakshishPatraDataForTaker fetchKharediNondGiverInformationData = new FetchBakshishPatraDataForTaker();
        //    //                fetchKharediNondGiverInformationData = FetchBakshishPatraInformationDataForTaker(Convert.ToInt32(kharediNondIDs[i]));
        //    //                if (fetchKharediNondGiverInformationData != null)
        //    //                {
        //    //                    mutationtaker.Add(fetchKharediNondGiverInformationData);
        //    //                }
        //    //            }

        //    //        }
        //    //    }
        //    //}
        //    ////Mrutyu Nond
        //    //else if (applicationDTL.mutation_type_code == "05")
        //    //{
        //    //    if (applicationDTL != null && !string.IsNullOrEmpty(applicationDTL.mutationtakerIDs))
        //    //    {
        //    //        string[] mutationtakerIDs = applicationDTL.mutationtakerIDs.Split(",");

        //    //        if (mutationtakerIDs.Length > 0)
        //    //        {
        //    //            for (int i = 0; i < mutationtakerIDs.Length; i++)
        //    //            {
        //    //                FetchMrutyuPatraForTakerData fetchData = new FetchMrutyuPatraForTakerData();
        //    //                fetchData = FetchMrutyuPatraForTakerData(Convert.ToInt32(mutationtakerIDs[i]));
        //    //                if (fetchData != null)
        //    //                {
        //    //                    mutationtaker.Add(fetchData);
        //    //                }
        //    //            }

        //    //        }
        //    //    }
        //    //}
        //    ////Gahankhat Nond
        //    //else if (applicationDTL.mutation_type_code == "06")
        //    //{
        //    //    if (applicationDTL != null && !string.IsNullOrEmpty(applicationDTL.mutationtakerIDs))
        //    //    {
        //    //        string[] mutationtakerIDs = applicationDTL.mutationtakerIDs.Split(",");

        //    //        if (mutationtakerIDs.Length > 0)
        //    //        {
        //    //            for (int i = 0; i < mutationtakerIDs.Length; i++)
        //    //            {
        //    //                FetchGahankhatDataForTaker fetchData = new FetchGahankhatDataForTaker();
        //    //                fetchData = FetchGahankhatForTakerData(Convert.ToInt32(mutationtakerIDs[i]));
        //    //                if (fetchData != null)
        //    //                {
        //    //                    mutationtaker.Add(fetchData);
        //    //                }
        //    //            }
        //    //        }
        //    //    }
        //    //}
        //    ////HakkaSod Nond
        //    //else if (applicationDTL.mutation_type_code == "09")
        //    //{
        //    //    if (!string.IsNullOrEmpty(applicationDTL.mutationtakerIDs))
        //    //    {
        //    //        string[] kharediNondIDs = applicationDTL.mutationtakerIDs!.Split(",");
        //    //        if (kharediNondIDs.Length > 0)
        //    //        {
        //    //            for (int i = 0; i < kharediNondIDs.Length; i++)
        //    //            {
        //    //                FetchHakkaSodTaker fetchKharediNondGiverInformationData = new FetchHakkaSodTaker();
        //    //                fetchKharediNondGiverInformationData = FetchHakkaSodInfoForTaker(Convert.ToInt32(kharediNondIDs[i]));
        //    //                if (fetchKharediNondGiverInformationData != null)
        //    //                {
        //    //                    mutationtaker.Add(fetchKharediNondGiverInformationData);
        //    //                }
        //    //            }
        //    //        }
        //    //    }
        //    //}
        //    return mutationtaker;
        //}

        public FetcMutationGiverTakerDTL FetchMutationDTL(int mutationdtlid, string applicationid)
        {
            try
            {
                ApplicationDTL applicationDTL = new ApplicationDTL();
                applicationDTL = _contextR.applicationDTL.Where(data => data.applicationid!.Equals(applicationid)).FirstOrDefault()!;

                MutationGiverTakerDTL mutationGiverTakerDTL = new MutationGiverTakerDTL();
                mutationGiverTakerDTL = _contextR.mutationDTL.Include(i => i.userMaster).Include(app => app.applicationDTL).Include(prop => prop.prop_type).Where(data => data.mutation_givertaker_id.Equals(mutationdtlid) && data.isDeleted == false).FirstOrDefault()!;
                FetcMutationGiverTakerDTL fetchData = new FetcMutationGiverTakerDTL();
                if (mutationGiverTakerDTL != null)
                {
                    fetchData.mutation_givertaker_id = mutationGiverTakerDTL.mutation_givertaker_id;
                    fetchData.usermasteruserid = mutationGiverTakerDTL.userMaster!.userid;
                    fetchData.applicationdtlapplicationid = mutationGiverTakerDTL.applicationDTL!.applicationid;
                    fetchData.user_type_code = mutationGiverTakerDTL.user_type_code;
                    fetchData.user_type = commonFunctions.ReplaceNA(mutationGiverTakerDTL.user_type!);
                    fetchData.prop_typepropertytypeid = commonFunctions.ReplaceNA(mutationGiverTakerDTL.prop_type!.propertytype!);
                    if (applicationDTL.mutation_type_code == "06" || applicationDTL.mutation_type_code == "07")
                    {
                        fetchData.istaker = (mutationGiverTakerDTL.isTaker == 0) ? 1 : 0;
                    }
                    else
                    {
                        fetchData.istaker = mutationGiverTakerDTL.isTaker;
                    }
                    fetchData.mobileno = mutationGiverTakerDTL.mobileno;
                    fetchData.mobilenoverified = mutationGiverTakerDTL.mobilenoverified;
                    fetchData.emailid = commonFunctions.ReplaceNA(mutationGiverTakerDTL.emailid!);
                    fetchData.emailidverified = commonFunctions.ReplaceNA(mutationGiverTakerDTL.emailidverified!);
                    fetchData.prefixcode_marathi = mutationGiverTakerDTL.prefixcode_marathi;
                    fetchData.prefix_in_marathi = commonFunctions.ReplaceNA(mutationGiverTakerDTL.prefix_in_marathi!);
                    fetchData.fname_in_marathi = commonFunctions.ReplaceNA(mutationGiverTakerDTL.fname_in_marathi!);
                    fetchData.mname_in_marathi = commonFunctions.ReplaceNA(mutationGiverTakerDTL.mname_in_marathi!);
                    fetchData.lname_in_marathi = commonFunctions.ReplaceNA(mutationGiverTakerDTL.lname_in_marathi!);
                    fetchData.prefixcode_eng = commonFunctions.ReplaceNA(mutationGiverTakerDTL.prefixcode_eng!);
                    fetchData.prefix_in_eng = commonFunctions.ReplaceNA(mutationGiverTakerDTL.prefix_in_eng!);
                    fetchData.fname_in_eng = commonFunctions.ReplaceNA(mutationGiverTakerDTL.fname_in_eng!);
                    fetchData.mname_in_eng = commonFunctions.ReplaceNA(mutationGiverTakerDTL.mname_in_eng!);
                    fetchData.lname_in_eng = commonFunctions.ReplaceNA(mutationGiverTakerDTL.lname_in_eng!);
                    fetchData.alias_name = commonFunctions.ReplaceNA(mutationGiverTakerDTL.alias_name!);
                    fetchData.company_name_in_marathi = commonFunctions.ReplaceNA(mutationGiverTakerDTL.company_name_in_marathi!);
                    fetchData.company_name_in_eng = commonFunctions.ReplaceNA(mutationGiverTakerDTL.company_name_in_eng!);
                    fetchData.gender_code = commonFunctions.ReplaceNA(mutationGiverTakerDTL.gender_code!);
                    fetchData.gender_description = commonFunctions.ReplaceNA(mutationGiverTakerDTL.gender_description!);
                    fetchData.holder_type = commonFunctions.ReplaceNA(mutationGiverTakerDTL.holder_type!);
                    fetchData.dob = commonFunctions.ReplaceNA(mutationGiverTakerDTL.dob!);
                    fetchData.mother_name_in_marathi = commonFunctions.ReplaceNA(mutationGiverTakerDTL.mother_name_in_marathi!);
                    fetchData.mother_name_in_eng = commonFunctions.ReplaceNA(mutationGiverTakerDTL.mother_name_in_eng!);
                    fetchData.username = commonFunctions.ReplaceNA(mutationGiverTakerDTL.userName!);
                    fetchData.city_servey_no = commonFunctions.ReplaceNA(mutationGiverTakerDTL.city_servey_no!);
                    fetchData.lr_property_id = commonFunctions.ReplaceNA(mutationGiverTakerDTL.lr_property_id!);
                    fetchData.sub_property_no = commonFunctions.ReplaceNA(mutationGiverTakerDTL.sub_property_no!);
                    fetchData.sellerid = commonFunctions.ReplaceNA(mutationGiverTakerDTL.sellerid!);
                    fetchData.buyerid = commonFunctions.ReplaceNA(mutationGiverTakerDTL.buyerid!);
                    fetchData.mutation_srno = commonFunctions.ReplaceNA(mutationGiverTakerDTL.mutation_srno!);
                    fetchData.owner_number = commonFunctions.ReplaceNA(mutationGiverTakerDTL.owner_number!);
                    fetchData.cts_number = commonFunctions.ReplaceNA(mutationGiverTakerDTL.cts_number!);
                    fetchData.milkat = commonFunctions.ReplaceNA(mutationGiverTakerDTL.milkat!);
                    fetchData.namud = commonFunctions.ReplaceNA(mutationGiverTakerDTL.namud!);
                    fetchData.isfullareagiven = commonFunctions.ReplaceNA(mutationGiverTakerDTL.isFullAreaGiven!);
                    fetchData.actual_area = commonFunctions.ReplaceNA(mutationGiverTakerDTL.actual_area!);
                    fetchData.available_area = commonFunctions.ReplaceNA(mutationGiverTakerDTL.available_area!);
                    fetchData.mutation_area = commonFunctions.ReplaceNA(mutationGiverTakerDTL.mutation_area!);
                    fetchData.address_type = mutationGiverTakerDTL.address_type;
                    fetchData.address = commonFunctions.ReplaceNA(mutationGiverTakerDTL.address!);
                    fetchData.state = commonFunctions.ReplaceNA(mutationGiverTakerDTL.state!);
                    fetchData.district = commonFunctions.ReplaceNA(mutationGiverTakerDTL.district!);
                    fetchData.taluka = commonFunctions.ReplaceNA(mutationGiverTakerDTL.taluka!);
                    fetchData.city = commonFunctions.ReplaceNA(mutationGiverTakerDTL.city!);
                    fetchData.flatno_plotno = commonFunctions.ReplaceNA(mutationGiverTakerDTL.flatno_plotno!);
                    fetchData.societyname = commonFunctions.ReplaceNA(mutationGiverTakerDTL.societyname!);
                    fetchData.mainstreet = commonFunctions.ReplaceNA(mutationGiverTakerDTL.mainstreet!);
                    fetchData.landmark = commonFunctions.ReplaceNA(mutationGiverTakerDTL.landmark!);
                    fetchData.locality = commonFunctions.ReplaceNA(mutationGiverTakerDTL.locality!);
                    fetchData.pincode = string.IsNullOrEmpty(mutationGiverTakerDTL.pincode) ? "NA" : mutationGiverTakerDTL.pincode;
                    fetchData.post_office_name = commonFunctions.ReplaceNA(mutationGiverTakerDTL.post_office_name!);
                    fetchData.address_proof_document_name = commonFunctions.ReplaceNA(mutationGiverTakerDTL.address_proof_document_name!);
                    fetchData.address_proof_document_path = commonFunctions.ReplaceNA(mutationGiverTakerDTL.address_proof_document_path!);
                    fetchData.has_property = commonFunctions.ReplaceNA(mutationGiverTakerDTL.has_property!);
                    fetchData.aapak = commonFunctions.ReplaceNA(mutationGiverTakerDTL.aapak!);
                    fetchData.land_buy_area = commonFunctions.ReplaceNA(mutationGiverTakerDTL.land_buy_area!);
                    fetchData.account_type_code = mutationGiverTakerDTL.account_type_code;
                    fetchData.account_type_description = commonFunctions.ReplaceNA(mutationGiverTakerDTL.account_type_description!);
                    fetchData.apk_code = mutationGiverTakerDTL.apk_code;
                    fetchData.apk_description = commonFunctions.ReplaceNA(mutationGiverTakerDTL.apk_description!);
                    fetchData.khata_type_code = commonFunctions.ReplaceNA(mutationGiverTakerDTL.khata_type_code!);
                    fetchData.khata_type_name = commonFunctions.ReplaceNA(mutationGiverTakerDTL.khata_type_name!);
                    fetchData.owner_status_code = commonFunctions.ReplaceNA(mutationGiverTakerDTL.owner_status_code!);
                    fetchData.owner_status_description = commonFunctions.ReplaceNA(mutationGiverTakerDTL.owner_status_description!);
                    fetchData.khatano = commonFunctions.ReplaceNA(mutationGiverTakerDTL.khatano!);
                    fetchData.ulpin = commonFunctions.ReplaceNA(mutationGiverTakerDTL.ulpin!);
                    fetchData.district_code = commonFunctions.ReplaceNA(mutationGiverTakerDTL.district_code!);
                    fetchData.district_name_in_marathi = commonFunctions.ReplaceNA(mutationGiverTakerDTL.district_name_in_marathi!);
                    fetchData.district_name_in_eng = commonFunctions.ReplaceNA(mutationGiverTakerDTL.district_name_in_eng!);
                    fetchData.village_code = commonFunctions.ReplaceNA(mutationGiverTakerDTL.village_code!);
                    fetchData.village_name = commonFunctions.ReplaceNA(mutationGiverTakerDTL.village_name!);
                    fetchData.ofc_code = commonFunctions.ReplaceNA(mutationGiverTakerDTL.ofc_code!);
                    fetchData.ofc_name = commonFunctions.ReplaceNA(mutationGiverTakerDTL.ofc_name!);
                    fetchData.relation_code = mutationGiverTakerDTL.relation_code;
                    fetchData.relation_name = commonFunctions.ReplaceNA(mutationGiverTakerDTL.relation_name!);
                    fetchData.varas_relation_code = mutationGiverTakerDTL.varas_relation_code;
                    fetchData.varas_relation_name = commonFunctions.ReplaceNA(mutationGiverTakerDTL.varas_relation_name!);
                    fetchData.institute_code = mutationGiverTakerDTL.institute_code;
                    fetchData.institute_description = commonFunctions.ReplaceNA(mutationGiverTakerDTL.institute_description!);
                    fetchData.bank_name_in_marathi = commonFunctions.ReplaceNA(mutationGiverTakerDTL.bank_name_in_marathi!);
                    fetchData.bank_name_in_english = commonFunctions.ReplaceNA(mutationGiverTakerDTL.bank_name_in_english!);
                    fetchData.ifsc = commonFunctions.ReplaceNA(mutationGiverTakerDTL.ifsc!);
                    fetchData.boja_value = commonFunctions.ReplaceNA(mutationGiverTakerDTL.boja_value!);
                    fetchData.boja_date = (mutationGiverTakerDTL.boja_date == "NA") ? commonFunctions.ReplaceNA(mutationGiverTakerDTL.boja_date!) : commonFunctions.ConvertStringToDate(mutationGiverTakerDTL.boja_date);
                    fetchData.boja_period = commonFunctions.ReplaceNA(mutationGiverTakerDTL.boja_period!);
                    fetchData.benefit_amt = commonFunctions.ReplaceNA(mutationGiverTakerDTL.benefit_amt!);
                    fetchData.isdeleted = mutationGiverTakerDTL.isDeleted;
                    fetchData.deleteddate = mutationGiverTakerDTL.deleteddate.ToString("yyyy-MM-dd");
                    fetchData.createddatetime = mutationGiverTakerDTL.createddatetime.ToString("yyyy-MM-dd");
                    fetchData.signed_file_name = commonFunctions.ReplaceNA(mutationGiverTakerDTL.signed_file_name!);
                    fetchData.signed_file_path = commonFunctions.ReplaceNA(mutationGiverTakerDTL.signed_file_path!);
                    fetchData.profile_pic_file_name = commonFunctions.ReplaceNA(mutationGiverTakerDTL.profile_pic_file_name!);
                    fetchData.profile_pic_file_path = commonFunctions.ReplaceNA(mutationGiverTakerDTL.profile_pic_file_path!);
                    fetchData.mutation_cts_no_id = mutationGiverTakerDTL.mutation_cts_no_id;
                    // Below are new Fields -> Added on 31 Dec 25
                    fetchData.owner_village_code = mutationGiverTakerDTL.owner_village_code;
                    fetchData.entry_bracketed = mutationGiverTakerDTL.entry_bracketed;
                    fetchData.entry_date = mutationGiverTakerDTL.entry_date;
                    fetchData.owner_bracketed = mutationGiverTakerDTL.owner_bracketed;
                    fetchData.owner_name = mutationGiverTakerDTL.owner_name;
                }
                else
                {
                    fetchData = null!;
                }
                return fetchData!;
            }
            catch (Exception ex)
            {
                throw new HandleException(ex.Message.ToString());
            }
        }

        public FetchPOADataForNIC FetchPOADTLForNIC(int poaid)
        {
            try
            {
                PowerOfAttorneyInformation powerOfAttorneyInformation = new PowerOfAttorneyInformation();
                powerOfAttorneyInformation = _contextR.powerOfAttorneyInformation.Include(i => i.userMaster).Include(prop => prop.propertyType).Include(app => app.applicationDTL).Where(data => data.power_of_attorney_id.Equals(poaid) && data.isDeleted == false).FirstOrDefault()!;
                FetchPOADataForNIC fetchdata = new FetchPOADataForNIC();
                if (powerOfAttorneyInformation != null)
                {
                    fetchdata.power_of_attorney_id = powerOfAttorneyInformation.power_of_attorney_id;
                    fetchdata.usermasteruserid = powerOfAttorneyInformation.userMaster!.userid;
                    fetchdata.applicationdtlapplicationid = powerOfAttorneyInformation.applicationDTL!.applicationid;
                    fetchdata.power_of_attorney_code = powerOfAttorneyInformation.power_of_attorney_code;
                    fetchdata.is_taker = powerOfAttorneyInformation.is_taker;
                    fetchdata.usertype_code = powerOfAttorneyInformation.usertype_code;
                    fetchdata.usertype = commonFunctions.ReplaceNA(powerOfAttorneyInformation.usertype);
                    fetchdata.mutation_id = powerOfAttorneyInformation.mutation_id;
                    fetchdata.mobileno = commonFunctions.ReplaceNA(powerOfAttorneyInformation.mobileno!);
                    fetchdata.mobilenoverified = commonFunctions.ReplaceNA(powerOfAttorneyInformation.mobilenoverified!);
                    fetchdata.emailid = commonFunctions.ReplaceNA(powerOfAttorneyInformation.emailid!);
                    fetchdata.emailidverified = commonFunctions.ReplaceNA(powerOfAttorneyInformation.emailidverified!);
                    fetchdata.prefixcode_marathi = commonFunctions.ReplaceNA(powerOfAttorneyInformation.prefixcode_marathi!);
                    fetchdata.prefix_in_marathi = commonFunctions.ReplaceNA(powerOfAttorneyInformation.prefix_in_marathi!);
                    fetchdata.fname_in_marathi = commonFunctions.ReplaceNA(powerOfAttorneyInformation.fname_in_marathi!);
                    fetchdata.mname_in_marathi = commonFunctions.ReplaceNA(powerOfAttorneyInformation.mname_in_marathi!);
                    fetchdata.lname_in_marathi = commonFunctions.ReplaceNA(powerOfAttorneyInformation.lname_in_marathi!);
                    fetchdata.prefixcode_eng = commonFunctions.ReplaceNA(powerOfAttorneyInformation.prefixcode_eng);
                    fetchdata.prefix_in_eng = commonFunctions.ReplaceNA(powerOfAttorneyInformation.prefixcode_eng!);
                    fetchdata.fname_in_eng = commonFunctions.ReplaceNA(powerOfAttorneyInformation.fname_in_eng!);
                    fetchdata.mname_in_eng = commonFunctions.ReplaceNA(powerOfAttorneyInformation.mname_in_eng!);
                    fetchdata.lname_in_eng = powerOfAttorneyInformation.lname_in_eng;
                    fetchdata.company_name_in_marathi = commonFunctions.ReplaceNA(powerOfAttorneyInformation.company_name_in_marathi!);
                    fetchdata.company_name_in_eng = commonFunctions.ReplaceNA(powerOfAttorneyInformation.company_name_in_eng!);
                    fetchdata.username = commonFunctions.ReplaceNA(powerOfAttorneyInformation.username!);
                    fetchdata.alias_name = commonFunctions.ReplaceNA(powerOfAttorneyInformation.alias_name!);
                    fetchdata.gender_code = commonFunctions.ReplaceNA(powerOfAttorneyInformation.gender_code!);
                    fetchdata.gender_description = commonFunctions.ReplaceNA(powerOfAttorneyInformation.gender_description!);
                    fetchdata.dob = commonFunctions.ReplaceNA(powerOfAttorneyInformation.dob!);
                    fetchdata.mother_name_in_marathi = commonFunctions.ReplaceNA(powerOfAttorneyInformation.mother_name_in_marathi!);
                    fetchdata.mother_name_in_eng = commonFunctions.ReplaceNA(powerOfAttorneyInformation.mother_name_in_eng!);
                    fetchdata.address_type = powerOfAttorneyInformation.address_type;
                    fetchdata.address = commonFunctions.ReplaceNA(powerOfAttorneyInformation.address!);
                    fetchdata.state = commonFunctions.ReplaceNA(powerOfAttorneyInformation.state!);
                    fetchdata.district = commonFunctions.ReplaceNA(powerOfAttorneyInformation.district!);
                    fetchdata.taluka = commonFunctions.ReplaceNA(powerOfAttorneyInformation.taluka!);
                    fetchdata.city = commonFunctions.ReplaceNA(powerOfAttorneyInformation.city!);
                    fetchdata.flatno_plotno = commonFunctions.ReplaceNA(powerOfAttorneyInformation.flatno_plotno!);
                    fetchdata.societyname = commonFunctions.ReplaceNA(powerOfAttorneyInformation.societyname);
                    fetchdata.mainstreet = commonFunctions.ReplaceNA(powerOfAttorneyInformation.mainstreet!);
                    fetchdata.landmark = commonFunctions.ReplaceNA(powerOfAttorneyInformation.landmark!);
                    fetchdata.locality = commonFunctions.ReplaceNA(powerOfAttorneyInformation.locality!);
                    fetchdata.pincode = string.IsNullOrEmpty(powerOfAttorneyInformation.pincode) ? "NA" : powerOfAttorneyInformation.pincode;
                    fetchdata.postofficename = commonFunctions.ReplaceNA(powerOfAttorneyInformation.postofficename!);
                    fetchdata.address_proof_document_name = commonFunctions.ReplaceNA(powerOfAttorneyInformation.address_proof_document_name);
                    fetchdata.address_proof_document_path = commonFunctions.ReplaceNA(powerOfAttorneyInformation.address_proof_document_path);
                    fetchdata.city_servey_no = commonFunctions.ReplaceNA(powerOfAttorneyInformation.city_servey_no!);
                    fetchdata.lr_property_id = commonFunctions.ReplaceNA(powerOfAttorneyInformation.lr_property_id!);
                    fetchdata.sub_property_no = commonFunctions.ReplaceNA(powerOfAttorneyInformation.sub_property_no!);
                    fetchdata.owner_of_property_in_maharashtra = powerOfAttorneyInformation.owner_of_property_in_maharashtra;
                    fetchdata.mutation_srno = commonFunctions.ReplaceNA(powerOfAttorneyInformation.mutation_srno!);
                    fetchdata.owner_number = commonFunctions.ReplaceNA(powerOfAttorneyInformation.owner_number!);
                    fetchdata.cts_number = commonFunctions.ReplaceNA(powerOfAttorneyInformation.cts_number!);
                    fetchdata.village_code = commonFunctions.ReplaceNA(powerOfAttorneyInformation.village_code!);
                    fetchdata.village_name = commonFunctions.ReplaceNA(powerOfAttorneyInformation.village_name!);
                    if (powerOfAttorneyInformation.propertyType != null && !string.IsNullOrEmpty(powerOfAttorneyInformation.propertyType!.propertytype!))
                    {
                        fetchdata.propertytypeid = powerOfAttorneyInformation.propertyType!.propertytypeid;
                    }
                    else
                    {
                        fetchdata.propertytypeid = 0;
                    }
                    fetchdata.property_district_code = commonFunctions.ReplaceNA(powerOfAttorneyInformation.property_district_code!);
                    fetchdata.property_district_name_in_marathi = commonFunctions.ReplaceNA(powerOfAttorneyInformation.property_district_name_in_marathi!);
                    fetchdata.property_district_name_in_english = commonFunctions.ReplaceNA(powerOfAttorneyInformation.property_district_name_in_english!);
                    fetchdata.property_taluka_code = commonFunctions.ReplaceNA(powerOfAttorneyInformation.property_taluka_code!);
                    fetchdata.property_taluka_name = commonFunctions.ReplaceNA(powerOfAttorneyInformation.property_taluka_name!);
                    fetchdata.property_city_code = commonFunctions.ReplaceNA(powerOfAttorneyInformation.property_city_code!);
                    fetchdata.property_city_name = commonFunctions.ReplaceNA(powerOfAttorneyInformation.property_city_name!);
                    fetchdata.khateno = commonFunctions.ReplaceNA(powerOfAttorneyInformation.khateno!);
                    fetchdata.ulpin = commonFunctions.ReplaceNA(powerOfAttorneyInformation.ulpin!);
                    fetchdata.khata_type_code = commonFunctions.ReplaceNA(powerOfAttorneyInformation.khata_type_code!);
                    fetchdata.khata_type_name = commonFunctions.ReplaceNA(powerOfAttorneyInformation.khata_type_name!);
                    fetchdata.owner_status_code = commonFunctions.ReplaceNA(powerOfAttorneyInformation.owner_status_code!);
                    fetchdata.owner_status_description = commonFunctions.ReplaceNA(powerOfAttorneyInformation.owner_status_description!);
                    fetchdata.attornytype_code = powerOfAttorneyInformation.attornytype_code;
                    fetchdata.attornytype_desc = commonFunctions.ReplaceNA(powerOfAttorneyInformation.attornytype_desc!);
                    fetchdata.landbuyarea = commonFunctions.ReplaceNA(powerOfAttorneyInformation.landBuyArea!);
                    fetchdata.ispoaispartofdast = commonFunctions.ReplaceNA(powerOfAttorneyInformation.isPOAisPartofDast!);
                    fetchdata.isdeclerationinvolvedinpoa = commonFunctions.ReplaceNA(powerOfAttorneyInformation.isDeclerationInvolvedInPOA!);
                    fetchdata.ispoapermanant = commonFunctions.ReplaceNA(powerOfAttorneyInformation.isPOAPermanant!);
                    fetchdata.istransferrights = commonFunctions.ReplaceNA(powerOfAttorneyInformation.isTransferRights!);
                    fetchdata.dast_no = commonFunctions.ReplaceNA(powerOfAttorneyInformation.dast_no!);
                    fetchdata.dast_no_date = (powerOfAttorneyInformation.dast_no_date == "NA") ? commonFunctions.ReplaceNA(powerOfAttorneyInformation.dast_no_date) : commonFunctions.ConvertStringToDate(powerOfAttorneyInformation.dast_no_date);
                    fetchdata.dast_no_year = commonFunctions.ReplaceNA(powerOfAttorneyInformation.dast_no_year!);
                    fetchdata.isdastverified = powerOfAttorneyInformation.isDastVerified;
                    fetchdata.verifieddastdata = commonFunctions.ReplaceNA(powerOfAttorneyInformation.verifieddastData!);
                    fetchdata.digcode = powerOfAttorneyInformation.digcode;
                    fetchdata.digname = commonFunctions.ReplaceNA(powerOfAttorneyInformation.digname!);
                    fetchdata.poa_district_code = commonFunctions.ReplaceNA(powerOfAttorneyInformation.poa_district_code!);
                    fetchdata.poa_district_name = commonFunctions.ReplaceNA(powerOfAttorneyInformation.poa_district_name!);
                    fetchdata.sro_office_code = powerOfAttorneyInformation.sro_office_code;
                    fetchdata.sro_office_name = commonFunctions.ReplaceNA(powerOfAttorneyInformation.sro_office_name!);
                    fetchdata.deleteddate = powerOfAttorneyInformation.deleteddate.ToString("yyyy-MM-dd");
                    fetchdata.createddatetime = powerOfAttorneyInformation.createddatetime.ToString("yyyy-MM-dd");
                    fetchdata.isdeleted = powerOfAttorneyInformation.isDeleted;
                    fetchdata.signed_file_path = commonFunctions.ReplaceNA(powerOfAttorneyInformation.signed_file_path);
                    fetchdata.signed_file_name = commonFunctions.ReplaceNA(powerOfAttorneyInformation.signed_file_name);
                    fetchdata.profile_pic_file_name = commonFunctions.ReplaceNA(powerOfAttorneyInformation.profile_pic_file_name);
                    fetchdata.profile_pic_file_path = commonFunctions.ReplaceNA(powerOfAttorneyInformation.profile_pic_file_path);
                    fetchdata.poa_giver_ids = powerOfAttorneyInformation.poa_giver_ids;
                }
                else
                {
                    fetchdata = null!;
                }
                return fetchdata!;

            }
            catch (Exception ex)
            {
                throw new HandleException(ex.Message.ToString());
            }
        }

        public FetchMayatDTLForNIC FetchMayatDetails(int mayatid)
        {
            try
            {
                MethodForFileUpload methodForFile = new MethodForFileUpload();
                MayatDTL mayatInfo = new MayatDTL();
                mayatInfo = _contextR.mayatDTL.Include(i => i.userMaster).Include(app => app.applicationDTL).Where(data => data.mayat_id.Equals(mayatid) && data.isDeleted == false).FirstOrDefault()!;
                FetchMayatDTLForNIC fetchData = new FetchMayatDTLForNIC();
                if (mayatInfo != null)
                {
                    fetchData.mayat_id = mayatInfo.mayat_id;
                    fetchData.usermasteruserid = mayatInfo.userMaster!.userid;
                    fetchData.applicationDTLapplicationid = mayatInfo.applicationDTL!.applicationid;
                    fetchData.mutation_cts_no_id = mayatInfo.mutation_cts_no_id;
                    fetchData.mobileno = mayatInfo.mobileno;
                    fetchData.mobilenoverified = mayatInfo.mobilenoverified;
                    fetchData.emailid = mayatInfo.emailid;
                    fetchData.emailidverified = mayatInfo.emailidverified;
                    fetchData.prefixcode_marathi = mayatInfo.prefixcode_marathi;
                    fetchData.prefix_in_marathi = mayatInfo.prefix_in_marathi;
                    fetchData.fname_in_marathi = mayatInfo.fname_in_marathi;
                    fetchData.mname_in_marathi = mayatInfo.mname_in_marathi;
                    fetchData.lname_in_marathi = mayatInfo.lname_in_marathi;
                    fetchData.prefixcode_eng = mayatInfo.prefixcode_eng;
                    fetchData.prefix_in_eng = mayatInfo.prefix_in_eng;
                    fetchData.fname_in_eng = mayatInfo.fname_in_eng;
                    fetchData.mname_in_eng = mayatInfo.mname_in_eng;
                    fetchData.lname_in_eng = mayatInfo.lname_in_eng;
                    fetchData.alias_name = mayatInfo.alias_name;
                    fetchData.address_type = mayatInfo.address_type;
                    fetchData.address = mayatInfo.address;
                    fetchData.state = mayatInfo.state;
                    fetchData.district = mayatInfo.district;
                    fetchData.taluka = mayatInfo.taluka;
                    fetchData.city = mayatInfo.city;
                    fetchData.flatno_plotno = mayatInfo.flatno_plotno;
                    fetchData.societyname = mayatInfo.societyname;
                    fetchData.mainstreet = mayatInfo.mainstreet;
                    fetchData.landmark = mayatInfo.landmark;
                    fetchData.locality = mayatInfo.locality;
                    fetchData.pincode = string.IsNullOrEmpty(mayatInfo.pincode) ? "NA" : mayatInfo.pincode;
                    fetchData.post_office_name = mayatInfo.post_office_name;
                    fetchData.city_servey_no = mayatInfo.city_servey_no;
                    fetchData.lr_property_id = mayatInfo.lr_property_id;
                    fetchData.milkat = mayatInfo.milkat;
                    fetchData.namud = mayatInfo.namud;
                    fetchData.sub_property_no = mayatInfo.sub_property_no;
                    fetchData.mutation_srno = mayatInfo.mutation_srno;
                    fetchData.owner_number = mayatInfo.owner_number;
                    fetchData.cts_number = mayatInfo.cts_number;
                    fetchData.actual_area = mayatInfo.actual_area;
                    fetchData.mrutyu_date = mayatInfo.mrutyu_date;
                    fetchData.certificate_authority_code = mayatInfo.certificate_authority_code;
                    fetchData.certificate_authority_name = mayatInfo.certificate_authority_name;
                    fetchData.mrutyucert_no = mayatInfo.mrutyucert_no;
                    fetchData.mrutyu_certificate_date = (mayatInfo.mrutyu_certificate_date == "NA") ? commonFunctions.ReplaceNA(mayatInfo.mrutyu_certificate_date!) : commonFunctions.ConvertStringToDate(mayatInfo.mrutyu_certificate_date);
                    fetchData.mrutyu_certificate__name = mayatInfo.mrutyu_certificate__name;
                    fetchData.mrutyu_certificate_path = mayatInfo.mrutyu_certificate_path;
                    fetchData.is_name_same = mayatInfo.is_name_same;
                    fetchData.reason = mayatInfo.reason;
                    fetchData.namecorrect_docname = mayatInfo.namecorrect_docname;
                    fetchData.namecorrect_docpath = mayatInfo.namecorrect_docpath;
                    fetchData.address_proof_document_name = mayatInfo.address_proof_document_name;
                    fetchData.address_proof_document_path = mayatInfo.address_proof_document_path;
                    fetchData.signed_file_name = mayatInfo.signed_file_name;
                    fetchData.signed_file_path = mayatInfo.signed_file_path;
                    fetchData.isdeleted = mayatInfo.isDeleted;
                    fetchData.deleteddate = mayatInfo.deleteddate.ToString("yyyy-MM-dd");
                    fetchData.createddatetime = mayatInfo.createddatetime.ToString("yyyy-MM-dd");
                    //Below are Probet fields
                    fetchData.probet_file_name = mayatInfo.probet_file_name;
                    fetchData.probet_file_path = mayatInfo.probet_file_path;
                    fetchData.isprobet = mayatInfo.isprobet;
                }
                else
                {
                    fetchData = null!;
                }
                return fetchData;
            }
            catch (Exception ex)
            {
                throw new HandleException(ex.Message.ToString());
            }
        }

        public FetchBhadepattaInfoDTLForNIC FetchBhadepattaInfoDTL(string applicationid)
        {
            try
            {
                BhadepattaInfoDtl bhadepattaInfoDtl = new BhadepattaInfoDtl();
                bhadepattaInfoDtl = _contextR.bhadepattaInfoDtl.Where(data => data.applicationid!.Equals(applicationid) && data.isDeleted == false).FirstOrDefault()!;
                FetchBhadepattaInfoDTLForNIC fetchData = new FetchBhadepattaInfoDTLForNIC();
                if (bhadepattaInfoDtl != null)
                {
                    fetchData.Info_id = bhadepattaInfoDtl.Info_id;
                    fetchData.userid = bhadepattaInfoDtl.userid;
                    fetchData.applicationid = bhadepattaInfoDtl.applicationid;
                    fetchData.bhadepattaTenureYear = bhadepattaInfoDtl.bhadepattaTenureYear;
                    fetchData.bhadepattaTenureMonth = bhadepattaInfoDtl.bhadepattaTenureMonth;
                    fetchData.bhadepattaAmount = bhadepattaInfoDtl.bhadepattaAmount;
                    fetchData.leaseperiod = bhadepattaInfoDtl.leaseperiod;
                    fetchData.bhadepattaToDate = bhadepattaInfoDtl.bhadepattaToDate;
                    fetchData.bhadepattaFromDate = bhadepattaInfoDtl.bhadepattaFromDate;
                    fetchData.createdDateTime = bhadepattaInfoDtl.createdDateTime.ToString("yyyy-MM-dd");
                    fetchData.deletedDateTime = bhadepattaInfoDtl.deletedDateTime.ToString("yyyy-MM-dd");
                    fetchData.isDeleted = bhadepattaInfoDtl.isDeleted;
                }
                else
                {
                    fetchData = null;
                }
                return fetchData!;
            }
            catch (Exception ex)
            {
                throw new HandleException(ex.Message.ToString());
            }
        }

        public FetchErrorCorrectionDataForNIC FetchErrorCorrectionData(int errorcorrectionid)
        {
            try
            {
                MethodForFileUpload methodForFile = new MethodForFileUpload();
                ErrorCorrectionInformation errorCorrectionData = new ErrorCorrectionInformation();
                errorCorrectionData = _contextR.errorCorrectionInformation.Include(i => i.userMaster).Include(app => app.applicationDTL).Where(data => data.error_correction_id == errorcorrectionid && data.isDeleted == false).FirstOrDefault()!;

                FetchErrorCorrectionDataForNIC fetchData = new FetchErrorCorrectionDataForNIC();
                fetchData.error_correction_id = errorCorrectionData.error_correction_id;
                fetchData.applicationdtlapplicationid = errorCorrectionData.applicationDTL!.applicationid;
                fetchData.usermasteruserid = errorCorrectionData.userMaster!.userid;
                fetchData.village_code = errorCorrectionData.village_code;
                fetchData.sub_property_no = errorCorrectionData.sub_property_no;
                fetchData.city_servey_no = errorCorrectionData.city_servey_no;
                fetchData.lr_property_id = errorCorrectionData.lr_property_id;
                fetchData.milkat = errorCorrectionData.milkat;
                fetchData.namud = errorCorrectionData.namud;
                //fetchData.var_village_code = errorCorrectionData.
                //fetchData.var_cts_number =
                //fetchData.var_cts_puid =
                //fetchData.var_mutation_srno =
                //fetchData.var_entry_date =
                //fetchData.var_mutation_number =
                //fetchData.var_mutation_date =
                //fetchData.var_sro_office_name_marathi =
                //fetchData.var_sro_office_name_english =
                //fetchData.var_document_number =
                //fetchData.var_document_year =
                //fetchData.var_document_date =
                //fetchData.var_entry_details =
                //fetchData.var_owner_details =
                fetchData.reason = errorCorrectionData.reason;
                fetchData.address_type = errorCorrectionData.address_type;
                fetchData.state = commonFunctions.ReplaceNA(errorCorrectionData.state!);
                fetchData.district = commonFunctions.ReplaceNA(errorCorrectionData.district!);
                fetchData.city = commonFunctions.ReplaceNA(errorCorrectionData.city!);
                fetchData.taluka = commonFunctions.ReplaceNA(errorCorrectionData.taluka!);
                fetchData.flatno_plotno = commonFunctions.ReplaceNA(errorCorrectionData.flatno_plotno!);
                fetchData.societyname = commonFunctions.ReplaceNA(errorCorrectionData.societyname!);
                fetchData.mainstreet = commonFunctions.ReplaceNA(errorCorrectionData.mainstreet!);
                fetchData.landmark = commonFunctions.ReplaceNA(errorCorrectionData.landmark!);
                fetchData.locality = commonFunctions.ReplaceNA(errorCorrectionData.locality!);
                fetchData.pincode = commonFunctions.ReplaceNA(errorCorrectionData.pincode!);
                fetchData.post_office_name = commonFunctions.ReplaceNA(errorCorrectionData.post_office_name!);
                fetchData.mobileno = commonFunctions.ReplaceNA(errorCorrectionData.mobileno!);
                fetchData.mobilenoverified = commonFunctions.ReplaceNA(errorCorrectionData.mobilenoverified!);
                fetchData.address_proof_document_name = commonFunctions.ReplaceNA(errorCorrectionData.address_proof_document_name!);
                fetchData.address_proof_document_path = commonFunctions.ReplaceNA(errorCorrectionData.address_proof_document_path!);
                fetchData.address = commonFunctions.ReplaceNA(errorCorrectionData.address!);
                fetchData.emailid = commonFunctions.ReplaceNA(errorCorrectionData.emailid!);
                TimeZoneInfo INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
                DateTime indianTime = TimeZoneInfo.ConvertTimeFromUtc(Convert.ToDateTime(errorCorrectionData.createddatetime.ToString()), INDIAN_ZONE);
                fetchData.createddatetime = indianTime.ToString("yyyy-MM-dd");
                fetchData.isdeleted = errorCorrectionData.isDeleted;
                fetchData.deleteddate = errorCorrectionData.deleteddate.ToString("yyyy-MM-dd");
                return fetchData;
            }
            catch (Exception ex)
            {
                throw new HandleException(ex.Message.ToString());
            }
        }

        public FetchNavatBadalDataForNIC FetchNavatBadalData(int name_change_id)
        {
            try
            {
                MethodForFileUpload methodForFile = new MethodForFileUpload();
                NameChangeDTL nameChangeDTL = new NameChangeDTL();
                nameChangeDTL = _contextR.nameChangeDTLs.Include(i => i.userMaster).Include(app => app.applicationDTL).Where(data => data.name_change_id == name_change_id && data.isDeleted == false).FirstOrDefault()!;

                FetchNavatBadalDataForNIC fetchData = new FetchNavatBadalDataForNIC();
                fetchData.name_change_id = nameChangeDTL.name_change_id;
                fetchData.usermasteruserid = nameChangeDTL.userMaster!.userid;
                fetchData.applicationdtlapplicationid = nameChangeDTL.applicationDTL!.applicationid;
                fetchData.village_code = nameChangeDTL.village_code;
                fetchData.subpropno = nameChangeDTL.subPropNo;
                fetchData.nabhu = nameChangeDTL.nabhu;
                fetchData.lrpropertyuid = nameChangeDTL.lrPropertyUID;
                fetchData.milkat = nameChangeDTL.milkat;
                fetchData.namud = nameChangeDTL.namud;
                fetchData.name_change_by_code = nameChangeDTL.name_change_by_code;
                fetchData.name_change_by_description = commonFunctions.ReplaceNA(nameChangeDTL.name_change_by_description!);
                fetchData.name_change_no = commonFunctions.ReplaceNA(nameChangeDTL.name_change_no!);
                fetchData.name_change_date = commonFunctions.ReplaceNA(nameChangeDTL.name_change_date!);
                // selectedMutation -> data from EPCIS
                fetchData.selected_village_code = commonFunctions.ReplaceNA(nameChangeDTL.selected_village_code!);
                fetchData.selected_cts_number = commonFunctions.ReplaceNA(nameChangeDTL.selected_cts_number!);
                fetchData.selected_mutation_srno = commonFunctions.ReplaceNA(nameChangeDTL.selected_mutation_srno!);
                fetchData.selected_entry_date = commonFunctions.ReplaceNA(nameChangeDTL.selected_entry_date!);
                fetchData.selected_entry_bracketed = commonFunctions.ReplaceNA(nameChangeDTL.selected_entry_bracketed!);
                fetchData.selected_owner_number = commonFunctions.ReplaceNA(nameChangeDTL.selected_owner_number!);
                fetchData.selected_owner_name = commonFunctions.ReplaceNA(nameChangeDTL.selected_owner_name!);
                fetchData.selected_first_name = commonFunctions.ReplaceNA(nameChangeDTL.selected_first_name!);
                fetchData.selected_middle_name = commonFunctions.ReplaceNA(nameChangeDTL.selected_middle_name!);
                fetchData.selected_last_name = commonFunctions.ReplaceNA(nameChangeDTL.selected_last_name!);
                fetchData.selected_nick_name = commonFunctions.ReplaceNA(nameChangeDTL.selected_nick_name!);
                fetchData.selected_owner_bracketed = commonFunctions.ReplaceNA(nameChangeDTL.selected_owner_bracketed!);
                fetchData.selected_area_bracketed = nameChangeDTL.selected_area_bracketed;
                fetchData.selected_email_id = nameChangeDTL.selected_email_id;
                fetchData.selected_owner_cell_number = nameChangeDTL.selected_owner_cell_number;
                fetchData.selected_pincode = nameChangeDTL.selected_pincode;
                fetchData.selected_owner_type = nameChangeDTL.selected_owner_type;
                fetchData.selected_apk_code = nameChangeDTL.selected_apk_code;
                fetchData.selected_apk_name = nameChangeDTL.selected_apk_name;
                fetchData.selected_flat_or_house_number = commonFunctions.ReplaceNA(nameChangeDTL.selected_flat_or_house_number!);
                fetchData.selected_building_number = commonFunctions.ReplaceNA(nameChangeDTL.selected_building_number!);
                fetchData.selected_road = commonFunctions.ReplaceNA(nameChangeDTL.selected_road!);
                fetchData.selected_city_or_village = commonFunctions.ReplaceNA(nameChangeDTL.selected_city_or_village!);
                fetchData.selected_taluka_name = commonFunctions.ReplaceNA(nameChangeDTL.selected_taluka_name!);
                fetchData.selected_district_name = commonFunctions.ReplaceNA(nameChangeDTL.selected_district_name!);
                fetchData.selected_state_name = commonFunctions.ReplaceNA(nameChangeDTL.selected_state_name!);
                fetchData.selected_gender_code = commonFunctions.ReplaceNA(nameChangeDTL.selected_gender_code!);
                fetchData.selected_date_of_birth = commonFunctions.ReplaceNA(nameChangeDTL.selected_date_of_birth!);
                fetchData.selected_owner_area = commonFunctions.ReplaceNA(nameChangeDTL.selected_owner_area!);
                fetchData.selected_owner_area_bracketed = commonFunctions.ReplaceNA(nameChangeDTL.selected_owner_area_bracketed!);
                // Updated Details
                fetchData.updated_usertype = nameChangeDTL.updated_userType;
                fetchData.updated_usertypelabel = commonFunctions.ReplaceNA(nameChangeDTL.updated_userTypeLabel!);
                fetchData.updated_prefixcode_marathi = commonFunctions.ReplaceNA(nameChangeDTL.updated_prefixcode_marathi!);
                fetchData.updated_prefix_in_marathi = commonFunctions.ReplaceNA(nameChangeDTL.updated_prefix_in_marathi!);
                fetchData.updated_fname_in_marathi = commonFunctions.ReplaceNA(nameChangeDTL.updated_fname_in_marathi!);
                fetchData.updated_mname_in_marathi = commonFunctions.ReplaceNA(nameChangeDTL.updated_mname_in_marathi!);
                fetchData.updated_lname_in_marathi = commonFunctions.ReplaceNA(nameChangeDTL.updated_lname_in_marathi!);
                fetchData.updated_prefixcode_eng = commonFunctions.ReplaceNA(nameChangeDTL.updated_prefixcode_eng!);
                fetchData.updated_prefix_in_eng = commonFunctions.ReplaceNA(nameChangeDTL.updated_prefix_in_eng!);
                fetchData.updated_fname_in_eng = commonFunctions.ReplaceNA(nameChangeDTL.updated_fname_in_eng!);
                fetchData.updated_mname_in_eng = commonFunctions.ReplaceNA(nameChangeDTL.updated_mname_in_eng!);
                fetchData.updated_lname_in_eng = commonFunctions.ReplaceNA(nameChangeDTL.updated_lname_in_eng!);
                fetchData.company_name_in_marathi = commonFunctions.ReplaceNA(nameChangeDTL.company_name_in_marathi!);
                fetchData.company_name_in_eng = commonFunctions.ReplaceNA(nameChangeDTL.company_name_in_eng!);
                // Address Fields
                fetchData.address_type = nameChangeDTL.address_type;
                fetchData.emailid = commonFunctions.ReplaceNA(nameChangeDTL.emailid!);
                fetchData.mobileno = commonFunctions.ReplaceNA(nameChangeDTL.mobileno!);
                fetchData.mobilenoverified = commonFunctions.ReplaceNA(nameChangeDTL.mobilenoverified!);
                fetchData.address = commonFunctions.ReplaceNA(nameChangeDTL.address!);
                fetchData.state = commonFunctions.ReplaceNA(nameChangeDTL.state!);
                fetchData.district = commonFunctions.ReplaceNA(nameChangeDTL.district!);
                fetchData.taluka = commonFunctions.ReplaceNA(nameChangeDTL.taluka!);
                fetchData.city = commonFunctions.ReplaceNA(nameChangeDTL.city!);
                fetchData.flatno_plotno = commonFunctions.ReplaceNA(nameChangeDTL.flatno_plotno!);
                fetchData.societyname = commonFunctions.ReplaceNA(nameChangeDTL.societyname!);
                fetchData.mainstreet = commonFunctions.ReplaceNA(nameChangeDTL.mainstreet!);
                fetchData.landmark = commonFunctions.ReplaceNA(nameChangeDTL.landmark!);
                fetchData.locality = commonFunctions.ReplaceNA(nameChangeDTL.locality!);
                fetchData.pincode = commonFunctions.ReplaceNA(nameChangeDTL.pincode!);
                fetchData.postofficename = commonFunctions.ReplaceNA(nameChangeDTL.postofficename!);
                fetchData.address_proof_document_name = commonFunctions.ReplaceNA(nameChangeDTL.address_proof_document_name!);
                fetchData.address_proof_document_path = commonFunctions.ReplaceNA(nameChangeDTL.address_proof_document_path!);

                TimeZoneInfo INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
                DateTime indianTime = TimeZoneInfo.ConvertTimeFromUtc(Convert.ToDateTime(nameChangeDTL.createddatetime.ToString()), INDIAN_ZONE);
                fetchData.createddatetime = indianTime.ToString("yyyy-MM-dd");
                fetchData.isdeleted = nameChangeDTL.isDeleted;
                fetchData.deleteddate = nameChangeDTL.deleteddate.ToString("yyyy-MM-dd");
                return fetchData;
            }
            catch (Exception ex)
            {
                throw new HandleException(ex.Message.ToString());
            }
        }

        public FetchHibanamaWitnessDataForNIC FetchHibanamaWitnessData(int witnessid)
        {
            try
            {
                MethodForFileUpload methodForFile = new MethodForFileUpload();
                WitnessDTL witnessData = new WitnessDTL();
                witnessData = _contextR.witnessDTLs.Include(i => i.userMaster).Include(app => app.applicationDTL).Where(data => data.witness_info_id.Equals(witnessid) && data.isDeleted == false).FirstOrDefault()!;

                FetchHibanamaWitnessDataForNIC fetchData = new FetchHibanamaWitnessDataForNIC();
                fetchData.witness_info_id = witnessData.witness_info_id;
                fetchData.applicationdtlapplicationid = witnessData.applicationDTL!.applicationid;
                fetchData.usermasteruserid = witnessData.userMaster!.userid;
                fetchData.permission_no = commonFunctions.ReplaceNA(witnessData.permission_no!);
                fetchData.permission_date = commonFunctions.ReplaceNA(witnessData.permission_date!);
                fetchData.prefixcode_marathi = witnessData.prefixcode_marathi;
                fetchData.prefix_in_marathi = witnessData.prefix_in_marathi;
                fetchData.fname_in_marathi = witnessData.fname_in_marathi;
                fetchData.mname_in_marathi = witnessData.mname_in_marathi;
                fetchData.lname_in_marathi = witnessData.lname_in_marathi;
                fetchData.prefixcode_eng = witnessData.prefixcode_eng;
                fetchData.prefix_in_eng = witnessData.prefix_in_eng;
                fetchData.fname_in_eng = witnessData.fname_in_eng;
                fetchData.mname_in_eng = witnessData.mname_in_eng;
                fetchData.lname_in_eng = witnessData.lname_in_eng;
                fetchData.alias_name = witnessData.alias_name;
                fetchData.address_type = witnessData.address_type;
                fetchData.address = commonFunctions.ReplaceNA(witnessData.address!);
                fetchData.state = commonFunctions.ReplaceNA(witnessData.state!);
                fetchData.district = commonFunctions.ReplaceNA(witnessData.district!);
                fetchData.city = commonFunctions.ReplaceNA(witnessData.city!);
                fetchData.taluka = commonFunctions.ReplaceNA(witnessData.taluka!);
                fetchData.flatno_plotno = commonFunctions.ReplaceNA(witnessData.flatno_plotno!);
                fetchData.societyname = commonFunctions.ReplaceNA(witnessData.societyname!);
                fetchData.mainstreet = commonFunctions.ReplaceNA(witnessData.mainstreet!);
                fetchData.landmark = commonFunctions.ReplaceNA(witnessData.landmark!);
                fetchData.locality = commonFunctions.ReplaceNA(witnessData.locality!);
                fetchData.pincode = commonFunctions.ReplaceNA(witnessData.pincode!);
                fetchData.post_office_name = commonFunctions.ReplaceNA(witnessData.post_office_name!);
                fetchData.mobileno = commonFunctions.ReplaceNA(witnessData.mobileno!);
                fetchData.mobilenoverified = commonFunctions.ReplaceNA(witnessData.mobilenoverified!);
                fetchData.address_proof_document_name = commonFunctions.ReplaceNA(witnessData.address_proof_document_name!);
                fetchData.address_proof_document_path = commonFunctions.ReplaceNA(witnessData.address_proof_document_path!);
                fetchData.emailid = commonFunctions.ReplaceNA(witnessData.emailid!);
                fetchData.emailidverified = witnessData.emailidverified;
                TimeZoneInfo INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
                DateTime indianTime = TimeZoneInfo.ConvertTimeFromUtc(Convert.ToDateTime(witnessData.createddatetime.ToString()), INDIAN_ZONE);
                fetchData.createddatetime = indianTime.ToString("yyyy-MM-dd");
                fetchData.isdeleted = witnessData.isDeleted;
                fetchData.deleteddate = witnessData.deleteddate.ToString("yyyy-MM-dd");
                return fetchData;
            }
            catch (Exception ex)
            {
                throw new HandleException(ex.Message.ToString());
            }
        }
        public string UpdateInwardnoData(UpdateInwardNoData updateInwardNoData)
        {
            using (var dbContextTransaction = _context.Database.BeginTransaction())
            {
                try
                {
                    ApplicationDTL enitityData = new ApplicationDTL();
                    enitityData = _context.applicationDTL.Include(user => user.userMaster).Include(app => app.applicationTypeMaster).Where(data => data.applicationid!.Equals(updateInwardNoData.applicationid) && data.isDeleted == false && data.inwardno == "NA").FirstOrDefault()!;
                    if (enitityData != null)
                    {
                        //Update Inward No
                        enitityData.inwardno = updateInwardNoData.inwardno;
                        enitityData.status = 10;
                        _context.applicationDTL.Attach(enitityData);
                        _context.SaveChanges();
                        dbContextTransaction.Commit();
                        dbContextTransaction.Dispose();
                        return "Success";
                    }
                    else
                    {
                        enitityData = _context.applicationDTL.Include(user => user.userMaster).Include(app => app.applicationTypeMaster).Where(data => data.applicationid!.Equals(updateInwardNoData.applicationid) && data.isDeleted == false && data.inwardno != "NA").FirstOrDefault()!;
                        if (enitityData != null)
                        {
                            return "Inward No Is Already Updated " + enitityData.inwardno;
                        }
                        else
                        {
                            return "Data Not Found";
                        }
                    }
                }
                catch (Exception ex)
                {
                    dbContextTransaction.Rollback();
                    dbContextTransaction.Dispose();
                    throw new HandleException(ex.Message.ToString());
                }
            }
        }
        //public async Task<string> SendRequestAsync(HttpMethod method, ILogger _logger, object? body = null)
        //{
        //    string url = "http://115.124.105.137/uatepcisapi/Pcis/insertPDEApplicationDetails";
        //    int maxRetries = 3;
        //    int delayMilliseconds = 1000;
        //    int attempt = 0;
        //    while (attempt < maxRetries)
        //    {
        //        attempt++;
        //        try
        //        {
        //            // Create a new HttpRequestMessage
        //            var request = new HttpRequestMessage(method, url);
        //            // Add body if provided
        //            if (body != null)
        //            {
        //                string jsonBody = JsonConvert.SerializeObject(body);
        //                request.Content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
        //            }
        //            // Send the request
        //            HttpResponseMessage response = await client.SendAsync(request);
        //            _logger.LogInformation($"Attempt {attempt}: Send Request URL NIC - {url}");

        //            if (response.IsSuccessStatusCode)
        //            {
        //                string responseBody = await response.Content.ReadAsStringAsync();
        //                if (!string.IsNullOrEmpty(responseBody))
        //                {
        //                    _logger.Log(LogLevel.Warning, $"Attempt {attempt}: Get Response Async NIC - {responseBody}");
        //                    var jsonObject = responseBody;
        //                    return jsonObject.ToString()!;
        //                }
        //            }
        //            else if ((int)response.StatusCode == 500)
        //            {
        //                string responseBody = await response.Content.ReadAsStringAsync();
        //                var jsonObject = "500 Status Code - " + responseBody;
        //                _logger.Log(LogLevel.Warning, $"Attempt {attempt}: Response Status 500 - {responseBody}");
        //                return jsonObject.ToString()!;
        //            }
        //            else
        //            {
        //                _logger.LogError($"Attempt {attempt}: Error - {response.StatusCode}");
        //                string responseContent = await response.Content.ReadAsStringAsync();
        //                var jsonObject = responseContent;
        //                return jsonObject.ToString()!;
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            _logger.LogError($"Attempt {attempt}: Exception occurred - {ex.Message}");
        //        }
        //        if (attempt < maxRetries)
        //        {
        //            _logger.LogWarning($"Retrying... Attempt {attempt + 1} in {delayMilliseconds}ms");
        //            await Task.Delay(delayMilliseconds); // Wait before retrying
        //        }
        //    }
        //    _logger.LogError("All retry attempts failed.");
        //    return "Data Not Found" + "|" + "400";
        //}

        //public async Task<string> callNICAPI(ApplicationDataForNIC applicationData, ILogger _logger)
        //{
        //    string response = await SendRequestAsync(HttpMethod.Post, _logger, applicationData);
        //    //if (response.Split("|")[1] == "200")
        //    //{
        //    //    if (response.Split("|")[0] != null && response.Split("|")[0].ToList().Count > 0)
        //    //    {
        //    //        var responseData = JsonConvert.DeserializeObject<List<OfficeByDist>>(response);
        //    //        return JsonConvert.SerializeObject(responseData);
        //    //    }
        //    //    else
        //    //    {
        //    //        return "Data List is Empty" + "|" + response.Split("|")[1];
        //    //    }
        //    //}
        //    //else 
        //    return response.ToString();
        //}
        //public async Task<string> SendRequestAsync(HttpMethod method, ILogger _logger, object? body = null, int attempt)
        //{
        //    string url = "http://115.124.105.137/uatepcisapi/Pcis/insertPDEApplicationDetails";
        //    //int maxRetries = 3;
        //    //int delayMilliseconds = 1000;
        //    //int attempt = 0;
        //    //while (attempt < maxRetries)
        //    //{
        //    //    attempt++;
        //    try
        //    {
        //        // Create a new HttpRequestMessage
        //        var request = new HttpRequestMessage(method, url);
        //        // Add body if provided
        //        if (body != null)
        //        {
        //            string jsonBody = JsonConvert.SerializeObject(body);
        //            request.Content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
        //        }
        //        // Send the request
        //        HttpResponseMessage response = await client.SendAsync(request);
        //        _logger.LogInformation($"Attempt {attempt}: Send Request URL NIC - {url}");

        //        if (response.IsSuccessStatusCode)
        //        {
        //            string responseBody = await response.Content.ReadAsStringAsync();
        //            if (!string.IsNullOrEmpty(responseBody))
        //            {
        //                _logger.Log(LogLevel.Warning, $"Attempt {attempt}: Get Response Async NIC - {responseBody}");
        //                var jsonObject = responseBody;
        //                return jsonObject.ToString()!;
        //            }
        //        }
        //        else if ((int)response.StatusCode == 500)
        //        {
        //            string responseBody = await response.Content.ReadAsStringAsync();
        //            var jsonObject = "500 Status Code - " + responseBody;
        //            _logger.Log(LogLevel.Warning, $"Attempt {attempt}: Response Status 500 From NIC - {responseBody}");
        //            return jsonObject.ToString()!;
        //            //return "Data Not Found" + "|" + "400";
        //        }
        //        else
        //        {
        //            _logger.LogError($"Attempt {attempt}: Error From NIC - {response.StatusCode}");
        //            string responseContent = await response.Content.ReadAsStringAsync();
        //            var jsonObject = responseContent;
        //            return jsonObject.ToString()!;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError($"Attempt {attempt}: Exception occurred From NIC - {ex.Message}");
        //    }
        //    //if (attempt < maxRetries)
        //    //{
        //    //    _logger.LogWarning($"Retrying... Attempt {attempt + 1} in {delayMilliseconds}ms");
        //    //    await Task.Delay(delayMilliseconds); // Wait before retrying
        //    //}
        //    //}
        //    //_logger.LogError("All retry attempts failed.");
        //    //return "Data Not Found" + "|" + "400";
        //}

        //public async Task<string> callNICAPI(ApplicationDataForNIC applicationData, ILogger _logger)
        //{
        //    string response = string.Empty;
        //    for (int i = 0; i < 3; i++)
        //    {
        //        response = await SendRequestAsync(HttpMethod.Post, _logger, applicationData, i);
        //    }
        //    return response.ToString();
        //}

        //public string SaveNICApiResponse(NicApiResponseModel nicApiResponseModel)
        //{
        //    using (var scope = new TransactionScope())
        //    {
        //        try
        //        {
        //            // Assign Values to Model
        //            DastInformationModel dastInformationModel = new DastInformationModel();
        //            UserMaster userMaster = _context.userMasters.FirstOrDefault(s => s.userid == nicApiResponseModel.userid)!;
        //            dastInformationModel.userMaster = userMaster;

        //            ApplicationDTL applicationDTL = _context.applicationDTL.FirstOrDefault(s => s.applicationid == dastInformationData.applicationid)!;
        //            dastInformationModel.applicationDTL = applicationDTL;

        //            dastInformationModel.dastType = dastInformationData.dastType!.Trim();
        //            dastInformationModel.digcode = dastInformationData.division!.digcode;
        //            dastInformationModel.dig_name = dastInformationData.division.dig;
        //            dastInformationModel.districtCode = dastInformationData.district!.jdrcode.ToString();
        //            dastInformationModel.districtName = dastInformationData.district.jdr!.Trim();
        //            // dastInformationModel.districtNameInEnglish = dastInformationData.district.district_english_name!.Trim();
        //            dastInformationModel.office_of_the_second_registrar_code = dastInformationData.registrar!.srocode.ToString();
        //            dastInformationModel.office_of_the_second_registrar_name = dastInformationData.registrar.sro!.Trim();
        //            dastInformationModel.registered_dast_no = dastInformationData.dastNo;
        //            dastInformationModel.registered_dast_date = dastInformationData.dastNoDate;
        //            dastInformationModel.registered_dast_year = dastInformationData.dastNoYear;
        //            dastInformationModel.dastNabhu = dastInformationData.dastNabhu;
        //            dastInformationModel.remarks = dastInformationData.remarks!.Trim();
        //            dastInformationModel.isDastVerified = dastInformationData.isDastVarified;
        //            dastInformationModel.verifiedDastData = dastInformationData.verifiedDastData;


        //            //Assign Data to Table fields to insert new records
        //            DastInformation dbTable = new DastInformation();
        //            dbTable.userMaster = dastInformationModel.userMaster;
        //            dbTable.applicationDTL = dastInformationModel.applicationDTL;
        //            dbTable.dastType = dastInformationModel.dastType;
        //            dbTable.division_code = dastInformationModel.digcode;
        //            dbTable.division_name = dastInformationModel.dig_name;
        //            dbTable.districtCode = dastInformationModel.districtCode;
        //            dbTable.districtName = dastInformationModel.districtName;
        //            // dbTable.districtNameInEnglish = dastInformationModel.districtNameInEnglish;
        //            dbTable.office_of_the_second_registrar_code = dastInformationModel.office_of_the_second_registrar_code;
        //            dbTable.office_of_the_second_registrar_name = dastInformationModel.office_of_the_second_registrar_name;
        //            dbTable.registered_dast_no = dastInformationModel.registered_dast_no;
        //            dbTable.registered_dast_date = dastInformationModel.registered_dast_date;
        //            dbTable.registered_dast_year = dastInformationModel.registered_dast_year;
        //            dbTable.dastNabhu = dastInformationModel.dastNabhu;
        //            dbTable.remarks = dastInformationModel.remarks;
        //            dbTable.isDastVerified = dastInformationModel.isDastVerified;
        //            dbTable.verifiedDastData = dastInformationModel.verifiedDastData;
        //            _context.dastInformation.Add(dbTable);
        //            _context.SaveChanges();

        //            //Get Saved Row ID
        //            int dastID = dbTable.dast_id;
        //            var applicationDTLdata = _context.applicationDTL.Where(data => data.applicationid!.Equals(dastInformationData.applicationid)).FirstOrDefault();
        //            if (applicationDTLdata != null)
        //            {
        //                if (!string.IsNullOrEmpty(applicationDTLdata.dastIDs) && !applicationDTLdata.dastIDs.Contains(dastID.ToString()))
        //                {
        //                    applicationDTLdata.dastIDs = applicationDTLdata.dastIDs + "," + dastID.ToString();
        //                }
        //                else
        //                {
        //                    applicationDTLdata.dastIDs = dastID.ToString();
        //                }
        //                applicationDTLdata.status = 4;
        //                _context.Entry(applicationDTLdata).CurrentValues.SetValues(applicationDTLdata);
        //                _context.SaveChanges();
        //            }
        //            scope.Complete();
        //            return "Success";
        //        }
        //        catch (Exception ex)
        //        {
        //            throw new HandleException(ex.Message.ToString());
        //        }
        //    }
        //}

        public async Task<NICApiRes> CallNICInwardNoAPI(HttpMethod method, int attempt, ILogger _logger, object? body = null)
        {
            NICApiRes nICAPI = new NICApiRes();
            //Live URL
            string url = "http://115.124.105.137/PRPDEePCISAPI/Pcis/insertPDEApplicationDetails";
            //UAT URL
            //string url = "http://115.124.105.137/uatepcisapi/Pcis/insertPDEApplicationDetails";
            string jsonBodydata = string.Empty;
            jsonBodydata = JsonConvert.SerializeObject(body);
            ApplicationDataForNIC fetchapplication = new ApplicationDataForNIC();
            fetchapplication = JsonConvert.DeserializeObject<ApplicationDataForNIC>(jsonBodydata!)!;
            try
            {
                // Create a new HttpRequestMessage
                var request = new HttpRequestMessage(method, url);
                string jsonBody = string.Empty;
                // Add body if provided
                if (body != null)
                {
                    jsonBody = JsonConvert.SerializeObject(body);
                    request.Content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
                }
                // Send the request
                HttpResponseMessage response = await client.SendAsync(request);
                _logger.LogInformation($"Attempt {attempt}: Send Request URL NIC - {url} For Application ID {fetchapplication!.applicationdtl![0].applicationid}");
                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    if (!string.IsNullOrEmpty(responseBody))
                    {
                        _logger.Log(LogLevel.Warning, $"Attempt {attempt}: Get Response Async NIC For Application ID -> {fetchapplication!.applicationdtl![0].applicationid} Response-> {responseBody}");
                        var jsonObject = responseBody;
                        nICAPI.statusCode = "200";
                        nICAPI.statusMsg = "Success";
                        nICAPI.inwardno = responseBody.ToString();
                    }
                }
                else if ((int)response.StatusCode == 500)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    var jsonObject = "500 Status Code - " + responseBody;
                    _logger.Log(LogLevel.Warning, $"Attempt {attempt}: Response Status 500 For Application ID -> {fetchapplication!.applicationdtl![0].applicationid}  -> {responseBody}");
                    nICAPI.statusCode = "500";
                    nICAPI.statusMsg = responseBody.ToString();
                }
                else
                {
                    _logger.LogError($"Attempt {attempt}: Error For Application ID -> {fetchapplication!.applicationdtl![0].applicationid} -> {response.StatusCode}");
                    string responseContent = await response.Content.ReadAsStringAsync();
                    var jsonObject = responseContent;
                    nICAPI.statusCode = response.StatusCode.ToString();
                    nICAPI.statusMsg = responseContent;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Attempt {attempt}: Exception occurred For Application ID -> {fetchapplication!.applicationdtl![0].applicationid} -> {ex.Message}");
                nICAPI.statusMsg = ex.Message;
                nICAPI.statusCode = "Exception";
            }
            return nICAPI;
        }

        public string insertDocUploadData(NicDocUploadData nicDocUploadData)
        {
            using (var scope = new TransactionScope())
            {
                try
                {
                    // Assign Values to Model
                    UploadedDocumentsDTLModel uploadedDocumentsDTLModel = new UploadedDocumentsDTLModel();
                    UserMaster userMaster = _context.userMasters.FirstOrDefault(s => s.userid == nicDocUploadData.userid)!;
                    uploadedDocumentsDTLModel.userMaster = userMaster;

                    ApplicationDTL applicationDTL = _context.applicationDTL.FirstOrDefault(s => s.applicationid == nicDocUploadData.applicationid)!;
                    uploadedDocumentsDTLModel.applicationDTL = applicationDTL;
                    uploadedDocumentsDTLModel.documentTypeCode = nicDocUploadData.document_type_code;
                    uploadedDocumentsDTLModel.documentType = nicDocUploadData.document_type;
                    uploadedDocumentsDTLModel.document_name = nicDocUploadData.document_name;
                    uploadedDocumentsDTLModel.document_path = nicDocUploadData.document_path;
                    uploadedDocumentsDTLModel.city_servey_no = nicDocUploadData.city_servey_no;

                    //Assign Data to Table fields to insert new records
                    UploadedDocumentsDTL dbTable = new UploadedDocumentsDTL();
                    dbTable.userMaster = uploadedDocumentsDTLModel.userMaster;
                    dbTable.applicationDTL = uploadedDocumentsDTLModel.applicationDTL;
                    dbTable.document_type_code = uploadedDocumentsDTLModel.documentTypeCode;
                    dbTable.document_type = uploadedDocumentsDTLModel.documentType;
                    dbTable.city_servey_no = uploadedDocumentsDTLModel.city_servey_no;
                    dbTable.document_name = uploadedDocumentsDTLModel.document_name;
                    dbTable.document_path = uploadedDocumentsDTLModel.document_path;
                    dbTable.nicdate = nicDocUploadData.nicdate;
                    _context.uploadedDocumentsDTLs.Add(dbTable);
                    _context.SaveChanges();
                    //Get Saved Row ID
                    int uploadedDocID = dbTable.uploaded_doc_id;
                    var applicationDTLdata = _context.applicationDTL.Where(data => data.applicationid!.Equals(nicDocUploadData.applicationid)).FirstOrDefault();
                    if (applicationDTLdata != null)
                    {
                        if (!string.IsNullOrEmpty(applicationDTLdata.uploadedDocIDs) && !applicationDTLdata.uploadedDocIDs.Contains(uploadedDocID.ToString()))
                        {
                            applicationDTLdata.uploadedDocIDs = applicationDTLdata.uploadedDocIDs + "," + uploadedDocID.ToString();
                        }
                        else
                        {
                            applicationDTLdata.uploadedDocIDs = uploadedDocID.ToString();
                        }
                        if (uploadedDocumentsDTLModel.documentTypeCode == "902")
                        {
                            applicationDTLdata.status = 11;// Truti Patra (Document code = 902)
                        }
                        if (uploadedDocumentsDTLModel.documentTypeCode == "903")
                        {
                            applicationDTLdata.status = 12;//Rejection - document code 903
                        }
                        if (uploadedDocumentsDTLModel.documentTypeCode == "909")
                        {
                            applicationDTLdata.status = 13;//Nikali Patra- document code 909
                        }
                        if (uploadedDocumentsDTLModel.documentTypeCode == "905")
                        {
                            applicationDTLdata.status = 14;//Notice 9 (can multiple times)- document code 905
                        }
                        _context.Entry(applicationDTLdata).CurrentValues.SetValues(applicationDTLdata);
                        _context.SaveChanges();
                    }
                    scope.Complete();
                    return "Success";
                }
                catch (Exception ex)
                {
                    throw new HandleException(ex.Message.ToString());
                }
            }
        }

        public string SaveNICApiResponse(NicApiResponseModel nicApiResponseModel)
        {
            using (var scope = new TransactionScope())
            {
                try
                {
                    NICAPIResponse dbTable = new NICAPIResponse();
                    dbTable.applicationid = nicApiResponseModel.applicationid;
                    dbTable.statuscode = nicApiResponseModel.statuscode;
                    dbTable.inwardno = nicApiResponseModel.inwardno;
                    dbTable.response = nicApiResponseModel.response;
                    dbTable.apicallcount = 1;
                    _context.nICAPIResponses.Add(dbTable);
                    _context.SaveChanges();

                    if (!nicApiResponseModel.inwardno_generated)
                    {
                        ApplicationDTL enitityData = new ApplicationDTL();
                        enitityData = _context.applicationDTL.Include(user => user.userMaster).Include(app => app.applicationTypeMaster).Where(data => data.applicationid!.Equals(nicApiResponseModel.applicationid) && data.isDeleted == false).FirstOrDefault()!;
                        if (enitityData != null)
                        {

                            enitityData.status = 15;
                            _context.applicationDTL.Attach(enitityData);
                            _context.SaveChanges();
                        }
                    }
                    scope.Complete();
                    return "Success";
                    //NICAPIResponse fetchData = new NICAPIResponse();
                    //fetchData = _context.nICAPIResponses.Where(data => data.applicationid!.Equals(nicApiResponseModel.applicationid)).FirstOrDefault()!;
                    //if (fetchData != null)
                    //{
                    //    var maxApiCallCount = _context.nICAPIResponses.Where(data => data.applicationid!.Equals(nicApiResponseModel.applicationid)).Max(x => x.apicallcount);
                    //    NICAPIResponse dbTable = new NICAPIResponse();
                    //    dbTable.statuscode = nicApiResponseModel.statuscode;
                    //    dbTable.applicationid = nicApiResponseModel.applicationid;
                    //    dbTable.inwardno = nicApiResponseModel.inwardno;
                    //    dbTable.response = nicApiResponseModel.response;
                    //    dbTable.apicallcount = maxApiCallCount + 1;
                    //    _context.nICAPIResponses.Add(dbTable);
                    //    _context.SaveChanges();
                    //    scope.Complete();
                    //    return "Success";
                    //}
                    //else
                    //{
                    //    NICAPIResponse dbTable = new NICAPIResponse();
                    //    dbTable.applicationid = nicApiResponseModel.applicationid;
                    //    dbTable.statuscode = nicApiResponseModel.statuscode;
                    //    dbTable.inwardno = nicApiResponseModel.inwardno;
                    //    dbTable.response = nicApiResponseModel.response;
                    //    dbTable.apicallcount = 1;
                    //    _context.nICAPIResponses.Add(dbTable);
                    //    _context.SaveChanges();
                    //    scope.Complete();
                    //    return "Success";
                    //}
                }
                catch (Exception ex)
                {
                    throw new HandleException(ex.Message.ToString());
                }
            }
        }

        public ApplicationDTL FetchApplicationDataForInwardNo(string applicationid, string inwardno)
        {
            try
            {
                ApplicationDTL applicationDTL = new ApplicationDTL();
                MethodForFileUpload methodForFile = new MethodForFileUpload();
                applicationDTL = _contextR.applicationDTL.Include(user => user.userMaster).Include(app => app.applicationTypeMaster).Where(data => data.applicationid!.Equals(applicationid) && data.inwardno == inwardno).FirstOrDefault()!;
                return applicationDTL;
            }
            catch (Exception ex)
            {
                throw new HandleException(ex.Message.ToString());
            }
        }


        //for scheduler
        public NICApiRes CallNICInwardNoAPIForJob(HttpMethod method, int attempt, ILogger _logger, object? body = null)
        {
            NICApiRes nICAPI = new NICApiRes();
            //Live URL
            string url = "http://115.124.105.137/PRPDEePCISAPI/Pcis/insertPDEApplicationDetails";
            //UAT URL
            //string url = "http://115.124.105.137/uatepcisapi/Pcis/insertPDEApplicationDetails";
            string jsonBodydata = string.Empty;
            jsonBodydata = JsonConvert.SerializeObject(body);
            ApplicationDataForNIC fetchapplication = new ApplicationDataForNIC();
            fetchapplication = JsonConvert.DeserializeObject<ApplicationDataForNIC>(jsonBodydata!)!;
            try
            {
                string jsonBody = JsonConvert.SerializeObject(body);
                var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                using (var client = new HttpClient())
                {
                    HttpResponseMessage response = client.PostAsync(url, content).Result; // Blocking call
                    _logger.LogInformation($"Attempt {attempt}: Send Request URL NIC - {url} For Application ID {fetchapplication!.applicationdtl![0].applicationid}");
                    if (response.IsSuccessStatusCode)
                    {
                        string responseBody = response.Content.ReadAsStringAsync().Result;
                        if (!string.IsNullOrEmpty(responseBody))
                        {
                            _logger.Log(LogLevel.Warning, $"Attempt {attempt}: Get Response Async NIC For Application ID -> {fetchapplication!.applicationdtl![0].applicationid} Response-> {responseBody}");
                            var jsonObject = responseBody;
                            nICAPI.statusCode = "200";
                            nICAPI.statusMsg = "Success";
                            nICAPI.inwardno = responseBody.ToString();
                        }
                    }
                    else if ((int)response.StatusCode == 500)
                    {
                        string responseBody = response.Content.ReadAsStringAsync().Result;
                        var jsonObject = "500 Status Code - " + responseBody;
                        _logger.Log(LogLevel.Warning, $"Attempt {attempt}: Response Status 500 For Application ID -> {fetchapplication!.applicationdtl![0].applicationid}  -> {responseBody}");
                        nICAPI.statusCode = "500";
                        nICAPI.statusMsg = responseBody.ToString();
                    }
                    else
                    {
                        _logger.LogError($"Attempt {attempt}: Error For Application ID -> {fetchapplication!.applicationdtl![0].applicationid} -> {response.StatusCode}");
                        string responseContent = response.Content.ReadAsStringAsync().Result;
                        var jsonObject = responseContent;
                        nICAPI.statusCode = response.StatusCode.ToString();
                        nICAPI.statusMsg = responseContent;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Attempt {attempt}: Exception occurred For Application ID -> {fetchapplication!.applicationdtl![0].applicationid} -> {ex.Message}");
                nICAPI.statusMsg = ex.Message;
                nICAPI.statusCode = "Exception";
            }
            return nICAPI;
        }

        public NICTrutiPatraAPIResponse CallNICTrutiPatraDTLAPI(HttpMethod method, int attempt, ILogger _logger, object? body = null)
        {
            NICTrutiPatraAPIResponse nICAPI = new NICTrutiPatraAPIResponse();
            //UAT URL
            //string url = "http://115.124.105.137/uatepcisapi/Pcis/trutiPatraDetails";
            //Live URL
            string url = "http://115.124.105.137/PRPDEePCISAPI/Pcis/trutiPatraDetails";
            string jsonBodydata = string.Empty;
            jsonBodydata = JsonConvert.SerializeObject(body);
            NICTrutiPatraData nICTrutiPatraData = new NICTrutiPatraData();
            nICTrutiPatraData = JsonConvert.DeserializeObject<NICTrutiPatraData>(jsonBodydata!)!;
            try
            {
                string jsonBody = JsonConvert.SerializeObject(body);
                var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
                using (var client = new HttpClient())
                {
                    HttpResponseMessage response = client.PostAsync(url, content).Result; // Blocking call
                    _logger.LogInformation($"Attempt {attempt}: Send Request URL NIC - {url} For Application ID {nICTrutiPatraData!.uploaded_documents_dtl![0].applicationdtlapplicationid}");
                    nICAPI.apiname = url;
                    if (response.IsSuccessStatusCode)
                    {
                        string responseBody = response.Content.ReadAsStringAsync().Result;
                        if (!string.IsNullOrEmpty(responseBody))
                        {
                            _logger.Log(LogLevel.Warning, $"Attempt {attempt}: Get Response Async NIC For Application ID -> {nICTrutiPatraData!.uploaded_documents_dtl![0].applicationdtlapplicationid} Response-> {responseBody}");
                            var jsonObject = responseBody;
                            nICAPI.statusCode = "200";
                            nICAPI.statusMsg = "Success";
                            nICAPI.response = responseBody.ToString();
                        }
                    }
                    else if ((int)response.StatusCode == 500)
                    {
                        string responseBody = response.Content.ReadAsStringAsync().Result;
                        var jsonObject = "500 Status Code - " + responseBody;
                        _logger.Log(LogLevel.Warning, $"Attempt {attempt}: Response Status 500 For Application ID -> {nICTrutiPatraData!.uploaded_documents_dtl![0].applicationdtlapplicationid}  -> {responseBody}");
                        nICAPI.statusCode = "500";
                        nICAPI.statusMsg = responseBody.ToString();
                    }
                    else
                    {
                        _logger.LogError($"Attempt {attempt}: Error For Application ID -> {nICTrutiPatraData!.uploaded_documents_dtl![0].applicationdtlapplicationid} -> {response.StatusCode}");
                        string responseContent = response.Content.ReadAsStringAsync().Result;
                        var jsonObject = responseContent;
                        nICAPI.statusCode = response.StatusCode.ToString();
                        nICAPI.statusMsg = responseContent;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Attempt {attempt}: Exception occurred For Application ID -> {nICTrutiPatraData!.uploaded_documents_dtl![0].applicationdtlapplicationid} -> {ex.Message}");
                nICAPI.statusMsg = ex.Message;
                nICAPI.statusCode = "Exception";
            }
            return nICAPI;
        }

        public List<string> FetchTrutiUploadedDocumentIDs(string applicationid)
        {
            List<string> documentIDList = new List<string>();
            List<InputDataModel.docUpload> docDataList = new List<InputDataModel.docUpload>();
            string FolderPath = @"\\10.10.248.2\pde_propertycard\MUTATIONDOCS\" + applicationid + @"\TRUTIDOCUMENTS";
            try
            {
                if (Directory.Exists(FolderPath))
                {
                    string[] fetchfilePath = Directory.GetFiles(FolderPath);

                    foreach (string file in fetchfilePath)
                    {
                        InputDataModel.docUpload docData = new InputDataModel.docUpload();
                        docData.docName = Path.GetFileName(file);
                        docData.docSrc = file;
                        docDataList.Add(docData);
                    }
                    foreach (InputDataModel.docUpload docData in docDataList)
                    {
                        UploadedDocumentsDTL uploadedDocumentsDTL = new UploadedDocumentsDTL();
                        uploadedDocumentsDTL = _contextR.uploadedDocumentsDTLs.Include(i => i.userMaster).Include(app => app.applicationDTL).Where(data => data.applicationDTL.applicationid.Equals(applicationid) && data.isDeleted == false
                        && data.document_name == docData.docName && data.document_path == docData.docSrc
                        && data.truti_patra_flag == "TRUE").FirstOrDefault()!;
                        if (uploadedDocumentsDTL != null)
                        {
                            documentIDList.Add(uploadedDocumentsDTL.uploaded_doc_id.ToString());
                        }
                    }
                }
                else
                {
                    documentIDList = null!;
                }
                return documentIDList;
            }
            catch (Exception ex)
            {
                throw new HandleException(ex.Message.ToString());
            }
        }

        public string UpdateApplicationStatusForTruti(string applicationid)
        {
            using (var dbContextTransaction = _context.Database.BeginTransaction())
            {
                try
                {
                    ApplicationDTL enitityData = new ApplicationDTL();
                    enitityData = _context.applicationDTL.Where(data => data.applicationid!.Equals(applicationid) && data.isDeleted == false).FirstOrDefault()!;
                    if (enitityData != null)
                    {
                        //Update Inward No
                        enitityData.status = 10;
                        _context.applicationDTL.Attach(enitityData);
                        _context.SaveChanges();
                        dbContextTransaction.Commit();
                        dbContextTransaction.Dispose();
                        return "Success";
                    }
                    else
                    {
                        return "Data Not Found";
                    }
                }
                catch (Exception ex)
                {
                    dbContextTransaction.Rollback();
                    dbContextTransaction.Dispose();
                    throw new HandleException(ex.Message.ToString());
                }
            }
        }

        public FetchUploadedDocumentDataForNIC FetchUploadedDocumentDTLsForTrutiPatra(int documentID)
        {
            try
            {
                UploadedDocumentsDTL uploadedDocumentsDTL = new UploadedDocumentsDTL();
                uploadedDocumentsDTL = _contextR.uploadedDocumentsDTLs.Include(i => i.userMaster).Include(app => app.applicationDTL).Where(data => data.uploaded_doc_id.Equals(documentID) && data.isDeleted == false).FirstOrDefault()!;
                FetchUploadedDocumentDataForNIC fetchData = new FetchUploadedDocumentDataForNIC();
                if (uploadedDocumentsDTL != null)
                {
                    fetchData.uploaded_doc_id = uploadedDocumentsDTL.uploaded_doc_id;
                    fetchData.usermasteruserid = uploadedDocumentsDTL.userMaster!.userid;
                    fetchData.applicationdtlapplicationid = uploadedDocumentsDTL.applicationDTL!.applicationid;
                    fetchData.document_type_code = uploadedDocumentsDTL.document_type_code;
                    fetchData.document_type = uploadedDocumentsDTL.document_type;
                    fetchData.city_servey_no = uploadedDocumentsDTL.city_servey_no;
                    fetchData.document_name = uploadedDocumentsDTL.document_name;
                    fetchData.document_path = uploadedDocumentsDTL.document_path;
                    TimeZoneInfo INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
                    DateTime indianTime = TimeZoneInfo.ConvertTimeFromUtc(Convert.ToDateTime(uploadedDocumentsDTL.createddatetime.ToString()), INDIAN_ZONE);
                    fetchData.createddatetime = indianTime.ToString("yyyy-MM-dd");
                    fetchData.isdeleted = uploadedDocumentsDTL.isDeleted;
                    fetchData.deleteddate = uploadedDocumentsDTL.deleteddate.ToString("yyyy-MM-dd");
                }
                else
                {
                    fetchData = null!;
                }
                return fetchData;
            }
            catch (Exception ex)
            {
                throw new HandleException(ex.Message.ToString());
            }
        }

        public string SaveNICApiResponse(SaveExternalAPIResponse saveExternalAPIResponse)
        {
            using (var dbContextTransaction = _context.Database.BeginTransaction())
            {
                try
                {
                    ExternalAPIResponse dbTable = new ExternalAPIResponse();
                    dbTable.applicationid = saveExternalAPIResponse.applicationid;
                    dbTable.vendorname = saveExternalAPIResponse.vendorname;
                    dbTable.api = saveExternalAPIResponse.api;
                    dbTable.statuscode = saveExternalAPIResponse.statuscode;
                    dbTable.response = saveExternalAPIResponse.response;
                    _context.externalAPIResponses.Add(dbTable);
                    _context.SaveChanges();

                    dbContextTransaction.Commit();
                    dbContextTransaction.Dispose();
                    return "Success";
                }
                catch (Exception ex)
                {
                    dbContextTransaction.Rollback();
                    dbContextTransaction.Dispose();
                    throw new HandleException(ex.Message.ToString());
                }
            }
        }

        public void SaveApplicationStatusHistory(string applicationid, string application_status)
        {
            using (var dbContextTransaction = _context.Database.BeginTransaction())
            {
                try
                {
                    ApplicationDTL applicationDTL = _context.applicationDTL.FirstOrDefault(s => s.applicationid == applicationid && s.isDeleted == false)!;
                    if (applicationDTL != null)
                    {
                        ApplicationStatusHistory applicationStatusHistory = new ApplicationStatusHistory();
                        applicationStatusHistory.applicationid = applicationid;
                        applicationStatusHistory.application_status = application_status;
                        applicationStatusHistory.mutation_type_code = applicationDTL.mutation_type_code;
                        applicationStatusHistory.mutation_type_name = applicationDTL.mutation_type_name;
                        _context.applicationStatusHistories.Add(applicationStatusHistory);
                        _context.SaveChanges();
                    }
                    dbContextTransaction.Commit();
                    dbContextTransaction.Dispose();
                }
                catch (Exception ex)
                {
                    dbContextTransaction.Rollback();
                    dbContextTransaction.Dispose();
                    throw new HandleException(ex.Message.ToString());
                }
            }
        }

    }
}
