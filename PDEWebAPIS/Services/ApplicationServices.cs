using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Microsoft.IdentityModel.Tokens;
using Npgsql;
using NuGet.Packaging.Signing;
using PDEWebAPIS.CommonMethods;
using PDEWebAPIS.Data;
using PDEWebAPIS.Helpers;
using PDEWebAPIS.InputDataModel;
using PDEWebAPIS.InputDataModel.MrutyuPatra;
using PDEWebAPIS.Model;
using PDEWebAPIS.Repository;
using PDEWebAPIS.ViewModel;
using System;
using System.Data;
using System.Globalization;
using System.Reflection.Metadata;
using static PDEWebAPIS.InputDataModel.INameWithCodeFormatData;
using static PDEWebAPIS.InputDataModel.IUserData;
using static System.Net.Mime.MediaTypeNames;
using System.Transactions;
using Microsoft.AspNetCore.Http;
using Renci.SshNet;
using System.IO;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace PDEWebAPIS.Services
{
    public class ApplicationServices
    {
        TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
        private AppDBContext _context;
        private AppDBContextR _contextR;
        private readonly UserServices userServices;
        private CommonFunctions commonFunctions = new CommonFunctions();
        //private readonly ILogger<TestFileUpload> _logger;
        private readonly ILogger<UserServices> _logger;

        public ApplicationServices(AppDBContext context, AppDBContextR contextR, ILogger<UserServices> loggeruserService)
        {
            _context = context;
            _contextR = contextR;
            //_loggeruserService = loggeruserService;
            userServices = new UserServices(context, contextR, loggeruserService);
            _logger = loggeruserService;
        }
        public List<FetchMutationTypes> FetchMutationType(string applicationType)
        {
            List<FetchMutationTypes> fetchDataList = new List<FetchMutationTypes>();
            var mutationMasterList = _contextR.mutationMasters.Include(i => i.applicationType).Where(data => data.applicationType!.application_type_name_in_marathi!.Equals(applicationType)).ToList();
            if (mutationMasterList.Count > 0)
            {
                mutationMasterList.ForEach(row => fetchDataList.Add(new FetchMutationTypes()
                {
                    mutationTypeCode = (row.mutation_code!).ToString(),
                    mutationTypeName = row.mutation_name
                }));
            }
            return fetchDataList;
        }

        public FetchDocumentTypeData FetchDocumentTypeByMutation(string mutationCode)
        {
            FetchDocumentTypeData fetchData = new FetchDocumentTypeData();
            List<DocumentTypeData> documentTypeDataList = new List<DocumentTypeData>();
            MutationMaster mutationMaster = new MutationMaster();
            mutationMaster = _contextR.mutationMasters.Where(data => data.mutation_code!.Equals(mutationCode)).FirstOrDefault()!;

            DocumentTypeMaster documentType = new DocumentTypeMaster();
            var documentTypeList = _contextR.documentTypeMasters.Where(data => data.mutation_type_code!.Equals(mutationCode)).ToList();
            if (documentTypeList.Count > 0)
            {
                fetchData.mutationType = mutationMaster.mutation_name;
                documentTypeList.ForEach(row => documentTypeDataList.Add(new DocumentTypeData()
                {
                    documentTypeCode = (row.document_type_id).ToString(),
                    documentType = row.document_type_name + " (Format - PDF ,Size - Upto 2 MB)"
                }));
                fetchData.mutationTypeHDR = documentTypeList[0].mutation_description + " (Format - PDF ,Size - Upto 2 MB)";
            }
            fetchData.documentTypeDataList = documentTypeDataList;
            return fetchData;
        }

        //Application Registration APIS
        public string SaveApplicationData(CreateApplicationData applicationData)
        {
            using (var scope = new TransactionScope(TransactionScopeOption.Suppress))
            {
                ApplicationDTL dbTable = new ApplicationDTL();
                try
                {
                    string applicantStatus = "Success";
                    // Assign values to model
                    ApplicationDTLModel applicationDTLModel = new ApplicationDTLModel();
                    applicationDTLModel.mutation_type_code = applicationData.mutationType!.mutationTypeCode;
                    applicationDTLModel.mutation_type_name = applicationData.mutationType.mutationTypeName;
                    applicationDTLModel.district_code = applicationData.district!.district_code;
                    applicationDTLModel.district_name_in_marathi = applicationData.district.district_name;
                    applicationDTLModel.district_name_in_english = applicationData.district.district_english_name;
                    applicationDTLModel.office_code = applicationData.office!.office_code;
                    applicationDTLModel.office_name = applicationData.office.office_name;
                    if (!string.IsNullOrEmpty(applicationData.isMainPatra))
                    {
                        applicationDTLModel.do_you_have_power_of_attorney = (applicationData.isMainPatra.Trim().ToUpper() == "YES") ? true : false;
                    }
                    else
                    {
                        return "Do You Have Power Of Attorney (मेन पत्र ) Value Should Not Be Empty";
                    }
                    if (!string.IsNullOrEmpty(applicationData.isCourtDawa))
                    {
                        applicationDTLModel.Is_the_claim_pending_before_the_court = (applicationData.isCourtDawa.Trim().ToUpper() == "YES") ? true : false;
                    }
                    else
                    {
                        return "Is The Claim Pending Before The Court (कोर्ट दावा ) Value Should Not Be Empty";
                    }
                    if (!string.IsNullOrEmpty(applicationData.isDastApplicable))
                    {
                        applicationDTLModel.does_the_original_charter_info_apply = (applicationData.isDastApplicable.Trim().ToUpper() == "YES") ? true : false;
                    }
                    else
                    {
                        return "Does The Original Charter Info Apply (दस्त applicable) Value Should Not Be Empty";
                    }

                    UserMaster userData = _context.userMasters.FirstOrDefault(s => s.userid == Convert.ToInt32(applicationData.userId))!;
                    applicationDTLModel.userMaster = userData;
                    ApplicationTypeMaster applicationType = _context.applicationTypeMasters.FirstOrDefault(s => s.application_type_name_in_marathi!.Trim() == applicationData.applicationType!.Trim())!;
                    applicationDTLModel.applicationTypeMaster = applicationType;


                    //Assign Data to Insert into table
                    dbTable.applicationid = GenerateApplicationID(applicationDTLModel.district_code!, applicationDTLModel.office_code!, applicationDTLModel.mutation_type_code!);
                    dbTable.userMaster = applicationDTLModel.userMaster;
                    dbTable.applicationTypeMaster = applicationDTLModel.applicationTypeMaster;
                    dbTable.mutation_type_code = applicationDTLModel.mutation_type_code;
                    dbTable.mutation_type_name = applicationDTLModel.mutation_type_name;
                    dbTable.district_code = applicationDTLModel.district_code;
                    dbTable.district_name_in_marathi = applicationDTLModel.district_name_in_marathi;
                    dbTable.district_name_in_english = applicationDTLModel.district_name_in_english;
                    dbTable.office_code = applicationDTLModel.office_code;
                    dbTable.office_name = applicationDTLModel.office_name;
                    dbTable.do_you_have_power_of_attorney = applicationDTLModel.do_you_have_power_of_attorney;
                    dbTable.Is_the_claim_pending_before_the_court = applicationDTLModel.Is_the_claim_pending_before_the_court;
                    dbTable.does_the_original_charter_info_apply = applicationDTLModel.does_the_original_charter_info_apply;
                    dbTable.status = 1;
                    _context.applicationDTL.Add(dbTable);
                    _context.SaveChanges();

                    // Check Applicant data is exists or not 
                    ApplicantMaster applicant = new ApplicantMaster();
                    using (var transaction = _contextR.Database.BeginTransaction(System.Data.IsolationLevel.ReadCommitted)) // Changed here
                    {
                        applicant = _contextR.applicantMasters.Include(i => i.PropertyTypeMaster).Include(u => u.userMaster).Include(app => app.applicationDTL).Where(data => data.applicationDTL!.applicationid == dbTable.applicationid).FirstOrDefault()!;
                        transaction.Commit();
                    }
                    _logger.LogInformation("Applicant Data Found " + applicant);
                    if (applicant == null)
                    {
                        FetchUserData getRegisteredUserData = new FetchUserData();
                        getRegisteredUserData = userServices.FetchUserData(Convert.ToInt32(applicationData.userId));

                        CreateApplicantData createApplicantData = new CreateApplicantData();
                        createApplicantData.usertype = getRegisteredUserData.usertype;
                        createApplicantData.usertype_code = getRegisteredUserData.usertype_code;

                        PhotoData photo = new PhotoData();
                        photo.passportSrc = getRegisteredUserData.profile_pic_file_path;
                        photo.passportName = getRegisteredUserData.profile_pic_file_name;
                        createApplicantData.photo = photo;

                        IsMHPropertyData isMHProperty = new IsMHPropertyData();
                        isMHProperty.hasProperty = (getRegisteredUserData.owner_of_property_in_maharashtra) ? "YES" : "NO";
                        isMHProperty.propType = getRegisteredUserData.propertyType_code.ToString();

                        UserDetailsData userDetails = new UserDetailsData();
                        userDetails.khataNo = getRegisteredUserData.khateno;
                        userDetails.naBhu = getRegisteredUserData.city_servey_no;
                        userDetails.ulpin = getRegisteredUserData.ulpin;
                        userDetails.userName = getRegisteredUserData.username;

                        DistrictData district = new DistrictData();
                        district.district_code = getRegisteredUserData.property_district_code;
                        district.district_name = getRegisteredUserData.property_district_name;
                        userDetails.district = district;

                        TalukaData taluka = new TalukaData();
                        taluka.office_code = getRegisteredUserData.property_taluka_code;
                        taluka.office_name = getRegisteredUserData.property_taluka_name;
                        userDetails.taluka = taluka;

                        VillageData village = new VillageData();
                        village.village_code = getRegisteredUserData.property_village_code;
                        village.village_name = getRegisteredUserData.property_village_name;
                        userDetails.village = village;

                        userDetails.suffix = getRegisteredUserData.prefix_in_marathi;
                        userDetails.firstName = getRegisteredUserData.fname_in_marathi;
                        userDetails.middleName = getRegisteredUserData.mname_in_marathi;
                        userDetails.lastName = getRegisteredUserData.lname_in_marathi;
                        userDetails.suffixEng = getRegisteredUserData.prefix_in_eng;
                        userDetails.firstNameEng = getRegisteredUserData.fname_in_eng;
                        userDetails.middleNameEng = getRegisteredUserData.mname_in_eng;
                        userDetails.lastNameEng = getRegisteredUserData.lname_in_eng;
                        userDetails.companyName = getRegisteredUserData.company_name_in_marathi;
                        userDetails.companyNameEng = getRegisteredUserData.company_name_in_eng;
                        isMHProperty.userDetails = userDetails;

                        PersonAddressData address = new PersonAddressData();
                        address.addressType = getRegisteredUserData.address_type;

                        if (address.addressType == "INDIA")
                        {
                            IndiaAddressData indiaAddress = new IndiaAddressData();
                            indiaAddress.state = getRegisteredUserData.state;
                            indiaAddress.district = getRegisteredUserData.district;
                            indiaAddress.city = getRegisteredUserData.city;
                            indiaAddress.taluka = getRegisteredUserData.taluka;
                            indiaAddress.plotNo = getRegisteredUserData.flatno_plotno;
                            indiaAddress.building = getRegisteredUserData.societyname;
                            indiaAddress.mainRoad = getRegisteredUserData.mainstreet;
                            indiaAddress.impSymbol = getRegisteredUserData.landmark;
                            indiaAddress.area = getRegisteredUserData.locality;
                            indiaAddress.pincode = getRegisteredUserData.pincode;
                            indiaAddress.postOfficeName = getRegisteredUserData.postofficename;
                            indiaAddress.addressProofName = getRegisteredUserData.address_proof_document_name;
                            indiaAddress.addressProofSrc = getRegisteredUserData.address_proof_document_path;
                            indiaAddress.signatureName = getRegisteredUserData.signed_file_name;
                            indiaAddress.signatureSrc = getRegisteredUserData.signed_file_path;
                            indiaAddress.mobile = getRegisteredUserData.mobileno;
                            indiaAddress.mobileOTP = getRegisteredUserData.mobilenoverified;
                            indiaAddress.email = getRegisteredUserData.emailid!;
                            indiaAddress.emailOTP = getRegisteredUserData.emailidverified!;
                            indiaAddress.securityKey = getRegisteredUserData.securitypin!;
                            address.indiaAddress = indiaAddress;
                        }
                        if (address.addressType == "FOREIGN")
                        {
                            ForeignAddressData foreignAddress = new ForeignAddressData();
                            foreignAddress.address = getRegisteredUserData.address;
                            foreignAddress.mobile = getRegisteredUserData.mobileno;
                            foreignAddress.email = getRegisteredUserData.emailid;
                            foreignAddress.emailOTP = getRegisteredUserData.emailidverified;
                            foreignAddress.signatureName = getRegisteredUserData.signed_file_name;
                            foreignAddress.signatureSrc = getRegisteredUserData.signed_file_path;
                            address.foreignAddress = foreignAddress;
                        }
                        createApplicantData.address = address;
                        createApplicantData.isMHProperty = isMHProperty;
                        createApplicantData.applicationId = dbTable.applicationid;
                        createApplicantData.userId = Convert.ToInt32(applicationData.userId);
                        applicantStatus = SaveApplicantData(createApplicantData, "USER");
                    }
                    if (applicantStatus == "Success")
                    {
                        _context.SaveChanges();

                        ApplicationStatusHistory applicationStatusHistory = new ApplicationStatusHistory();
                        applicationStatusHistory.applicationid = dbTable.applicationid;
                        applicationStatusHistory.application_status = "Partially Submitted";
                        applicationStatusHistory.mutation_type_code = dbTable.mutation_type_code;
                        applicationStatusHistory.mutation_type_name = dbTable.mutation_type_name;
                        _context.applicationStatusHistories.Add(applicationStatusHistory);
                        _context.SaveChanges();

                        //_context.Dispose();
                        scope.Complete();
                        return "Success," + dbTable.applicationid;
                    }
                    else
                    {
                        _context.applicationDTL.Remove(dbTable);
                        _context.SaveChanges();
                        //_context.Dispose();
                        return applicantStatus;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogInformation("Application Data Error " + ex.Message.ToString());
                    _context.applicationDTL.Remove(dbTable);
                    _context.SaveChanges();
                    //_context.Dispose();
                    throw new HandleException(ex.Message.ToString());
                }
            }
        }

        public string GenerateApplicationID(string districtCode, string officeCode, string mutationTypeCode)
        {
            //var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", optional: false);
            //IConfiguration configuration = builder.Build();
            //string ConnectionString = configuration.GetValue<string>("ConnectionStrings:PDEDBR")!;
            try
            {
                string ApplicationID = string.Empty;
                using var command = _context.Database.GetDbConnection().CreateCommand();
                command.CommandText = "SELECT public.generate_application_id(@districtCode,@officeCode,@mutationTypeCode)";
                command.Parameters.Add(new NpgsqlParameter("@districtCode", districtCode));
                command.Parameters.Add(new NpgsqlParameter("@officeCode", officeCode));
                command.Parameters.Add(new NpgsqlParameter("@mutationTypeCode", mutationTypeCode));
                _context.Database.OpenConnection();
                ApplicationID = command.ExecuteScalar().ToString()!;
                _logger.LogInformation("GenerateApplicationID - " + ApplicationID);
                _context.Database.CloseConnection();
                //NpgsqlConnection connection = new NpgsqlConnection(ConnectionString);
                //connection.Open();
                //NpgsqlCommand npgsqlCommand = new NpgsqlCommand();
                //npgsqlCommand.Connection = connection;
                //npgsqlCommand.Parameters.Clear();
                //npgsqlCommand.CommandType = CommandType.Text;
                //npgsqlCommand.CommandText = "SELECT public.generate_application_id('" + districtCode + "' , '" + officeCode + "','" + mutationTypeCode + "')";

                //ApplicationID = npgsqlCommand.ExecuteScalar()!.ToString()!;
                //npgsqlCommand.Dispose();
                //connection.Close();
                return ApplicationID!;
            }
            catch (Exception ex)
            {
                _logger.LogError("GenerateApplicationID Error - " + ex.Message.ToString());
                throw new HandleException(ex.Message.ToString());
            }
        }

        public ApplicationDTL FetchApplicationData(string applicationid)
        {
            try
            {
                ApplicationDTL applicationDTL = new ApplicationDTL();
                applicationDTL = _contextR.applicationDTL.Include(app => app.applicationTypeMaster).Include(user => user.userMaster).Where(data => data.applicationid!.Equals(applicationid)).FirstOrDefault()!;
                return applicationDTL;
            }
            catch (Exception ex)
            {
                throw new HandleException(ex.Message.ToString());
            }
        }

        public ApplicationDTL FetchApplicationDataForPOA(string applicationid)
        {
            try
            {
                ApplicationDTL applicationDTL = new ApplicationDTL();
                applicationDTL = _context.applicationDTL.Include(app => app.applicationTypeMaster).Where(data => data.applicationid!.Equals(applicationid)).FirstOrDefault()!;
                return applicationDTL;
            }
            catch (Exception ex)
            {
                throw new HandleException(ex.Message.ToString());
            }
        }

        //public ApplicationDTL FetchApplicationData(string applicationid)
        //{
        //    try
        //    {
        //        ApplicationDTL applicationDTL = new ApplicationDTL();
        //        using (var transaction = _contextR.Database.BeginTransaction(System.Data.IsolationLevel.ReadCommitted)) // Changed here
        //        {
        //            applicationDTL = _contextR.applicationDTL.Include(app => app.applicationTypeMaster).Where(data => data.applicationid!.Equals(applicationid)).FirstOrDefault()!;
        //            transaction.Commit();
        //        }
        //        return applicationDTL;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new HandleException(ex.Message.ToString());
        //    }
        //}

        //Add Applicants in created Application
        public int FetchApplicantID(string description, string VerificationType)
        {
            try
            {
                int FetchApplicantID = 0;
                if (VerificationType.Trim().ToUpper() == "MOBILENO")
                {
                    var row = _contextR.applicantMasters.Where(data => data.mobileno.Equals(description)).FirstOrDefault();
                    if (row != null)
                    {
                        FetchApplicantID = row.applicantid;
                    }
                    else
                    {
                        FetchApplicantID = 0;
                    }
                }
                else if (VerificationType.Trim().ToUpper() == "EMAILID")
                {
                    var row = _contextR.applicantMasters.Where(data => data.emailid.Equals(description.Trim())).FirstOrDefault();
                    //var row1 = _context.userMasters.Where(data => data.mobileno.Equals(MobileNo)).First();
                    if (row != null)
                    {
                        FetchApplicantID = row.applicantid;
                    }
                    else
                    {
                        FetchApplicantID = 0;
                    }
                }
                return FetchApplicantID;
            }
            catch (Exception ex)
            {
                throw new HandleException(ex.Message.ToString());
            }
        }

        public string DeleteApplication(string applicationID)
        {
            using (var scope = new TransactionScope())
            {
                try
                {
                    var entity = _context.applicationDTL.FirstOrDefault(s => s.applicationid == applicationID && s.isDeleted == false)!;
                    if (entity != null && entity.status != 9)
                    {
                        entity.isDeleted = true;
                        entity.deleteddate = DateOnly.FromDateTime(DateTime.Now);
                        _context.applicationDTL.Attach(entity);
                        _context.SaveChanges();
                        scope.Complete();
                        return "Success";
                    }
                    else
                    {
                        return "Data Not Found";
                    }
                }
                catch (Exception ex)
                {
                    throw new HandleException(ex.Message.ToString());
                }
            }
        }

        public string SaveApplicantData(CreateApplicantData createApplicantData, string checkFLag)
        {
            MethodForFileUpload methodForFile = new MethodForFileUpload();
            ApplicantMaster dbTable = new ApplicantMaster();
            //string FolderPath = @"D:\WWW\MUTATIONDOCS\" + createApplicantData.applicationId + @"\APPLICANTS\";
            string FolderPath = @"\\10.10.248.2\pde_propertycard\MUTATIONDOCS\" + createApplicantData.applicationId + @"\APPLICANTS\";
            using (var scope = new TransactionScope(TransactionScopeOption.Suppress))
            {
                try
                {
                    if (checkFLag == "APPLICANT")
                    {
                        if (string.IsNullOrEmpty(createApplicantData.photo!.passportName) || string.IsNullOrEmpty(createApplicantData.photo.passportSrc))
                        {
                            return "Please Upload Passport Photo";
                        }
                        if (createApplicantData.address!.addressType!.Trim().ToUpper() == "INDIA")
                        {
                            if (string.IsNullOrEmpty(createApplicantData.address.indiaAddress!.signatureName) || string.IsNullOrEmpty(createApplicantData.address.indiaAddress.signatureSrc))
                            {
                                return "Please Upload Signature";
                            }
                            if (string.IsNullOrEmpty(createApplicantData.address.indiaAddress.plotNo))
                            {
                                return "Please Enter Flat / Plot No";
                            }
                            if (string.IsNullOrEmpty(createApplicantData.address.indiaAddress.impSymbol))
                            {
                                return "Please Enter Landmark";
                            }
                            if (string.IsNullOrEmpty(createApplicantData.address.indiaAddress.pincode))
                            {
                                return "Please Enter Pincode";
                            }
                        }
                        if (createApplicantData.address.addressType.Trim().ToUpper() == "FOREIGN" && (string.IsNullOrEmpty(createApplicantData.address.foreignAddress!.signatureName) || string.IsNullOrEmpty(createApplicantData.address.foreignAddress.signatureSrc)))
                        {
                            return "Please Upload Signature";
                        }
                    }

                    // Assign values to model
                    ApplicantMasterModel applicantMasterModel = new ApplicantMasterModel();
                    applicantMasterModel.usertype_code = createApplicantData.usertype_code;
                    applicantMasterModel.usertype = createApplicantData.usertype!.Trim().ToUpper();
                    applicantMasterModel.profile_pic_file_name = createApplicantData.photo!.passportName!;

                    applicantMasterModel.owner_of_property_in_maharashtra = (createApplicantData.isMHProperty!.hasProperty!.Trim().ToUpper() == "YES") ? true : false;
                    PropertyTypeMaster proptype = _context.propertyTypes.FirstOrDefault(s => s.propertytypeid == Convert.ToInt32(createApplicantData.isMHProperty.propType))!;
                    applicantMasterModel.PropertyTypeMaster = proptype;
                    if (applicantMasterModel.owner_of_property_in_maharashtra)
                    {
                        if (applicantMasterModel.PropertyTypeMaster.propertytypeid == 1 && string.IsNullOrEmpty(createApplicantData.isMHProperty.userDetails!.khataNo))
                        {
                            return "When Property Type Is 7/12 Then Khate No Should Not Be Empty";
                        }
                        if (applicantMasterModel.PropertyTypeMaster.propertytypeid == 2 && string.IsNullOrEmpty(createApplicantData.isMHProperty.userDetails!.naBhu))
                        {
                            return "When Property Type Is Property Card Then City Servey No Should Not Be Empty";
                        }
                        if (applicantMasterModel.PropertyTypeMaster.propertytypeid == 3 && string.IsNullOrEmpty(createApplicantData.isMHProperty.userDetails!.ulpin))
                        {
                            return "When Property Type Is ULPIN Then ULPIN Should Not Be Empty";
                        }
                        applicantMasterModel.khateno = string.IsNullOrEmpty(createApplicantData.isMHProperty.userDetails!.khataNo) ? "NA" : createApplicantData.isMHProperty.userDetails.khataNo;
                        applicantMasterModel.city_servey_no = string.IsNullOrEmpty(createApplicantData.isMHProperty.userDetails.naBhu) ? "NA" : createApplicantData.isMHProperty.userDetails.naBhu;
                        applicantMasterModel.ulpin = string.IsNullOrEmpty(createApplicantData.isMHProperty.userDetails.ulpin) ? "NA" : createApplicantData.isMHProperty.userDetails.ulpin;
                        if (applicantMasterModel.PropertyTypeMaster.propertytypeid == 1)
                        {
                            applicantMasterModel.city_servey_no = "NA";
                            applicantMasterModel.ulpin = "NA";
                        }
                        else if (applicantMasterModel.PropertyTypeMaster.propertytypeid == 2)
                        {
                            applicantMasterModel.khateno = "NA";
                            applicantMasterModel.ulpin = "NA";
                        }
                        else if (applicantMasterModel.PropertyTypeMaster.propertytypeid == 3)
                        {
                            applicantMasterModel.khateno = "NA";
                            applicantMasterModel.city_servey_no = "NA";
                        }
                        applicantMasterModel.property_district_code = createApplicantData.isMHProperty.userDetails.district!.district_code!;
                        applicantMasterModel.property_district_name = createApplicantData.isMHProperty.userDetails.district!.district_name!;

                        applicantMasterModel.property_taluka_code = createApplicantData.isMHProperty.userDetails.taluka!.office_code!;
                        applicantMasterModel.property_taluka_name = createApplicantData.isMHProperty.userDetails.taluka!.office_name!;

                        applicantMasterModel.property_village_code = createApplicantData.isMHProperty.userDetails.village!.village_code!;
                        applicantMasterModel.property_village_name = createApplicantData.isMHProperty.userDetails.village!.village_name!;

                    }
                    else
                    {
                        applicantMasterModel.khateno = "NA";
                        applicantMasterModel.city_servey_no = "NA";
                        applicantMasterModel.ulpin = "NA";
                        applicantMasterModel.property_district_code = "NA";
                        applicantMasterModel.property_district_name = "NA";
                        applicantMasterModel.property_taluka_code = "NA";
                        applicantMasterModel.property_taluka_name = "NA";
                        applicantMasterModel.property_village_code = "NA";
                        applicantMasterModel.property_village_name = "NA";
                        applicantMasterModel.city = "NA";
                    }
                    //if (string.IsNullOrEmpty(createApplicantData.isMHProperty.userDetails.userName))
                    //{
                    //    throw new HandleException("User Name Should Not Be Empty");
                    //}

                    applicantMasterModel.username = createApplicantData.isMHProperty.userDetails!.userName!;
                    if (createApplicantData.usertype_code == 1)
                    {
                        applicantMasterModel.prefix_in_marathi = createApplicantData.isMHProperty.userDetails.suffix!;
                        applicantMasterModel.fname_in_marathi = createApplicantData.isMHProperty.userDetails.firstName!;
                        applicantMasterModel.mname_in_marathi = createApplicantData.isMHProperty.userDetails.middleName!;
                        applicantMasterModel.lname_in_marathi = createApplicantData.isMHProperty.userDetails.lastName!;
                        applicantMasterModel.prefix_in_eng = createApplicantData.isMHProperty.userDetails.suffixEng!;
                        applicantMasterModel.fname_in_eng = createApplicantData.isMHProperty.userDetails.firstNameEng!;
                        applicantMasterModel.mname_in_eng = createApplicantData.isMHProperty.userDetails.middleNameEng!;
                        applicantMasterModel.lname_in_eng = createApplicantData.isMHProperty.userDetails.lastNameEng!;

                        applicantMasterModel.company_name_in_marathi = "NA";
                        applicantMasterModel.company_name_in_eng = "NA";
                    }
                    //if (createApplicantData.usertype.Trim().ToUpper() == "COMPANY")
                    else
                    {
                        applicantMasterModel.company_name_in_marathi = createApplicantData.isMHProperty.userDetails.companyName!;
                        applicantMasterModel.company_name_in_eng = createApplicantData.isMHProperty.userDetails.companyNameEng!;

                        applicantMasterModel.prefix_in_marathi = "NA";
                        applicantMasterModel.fname_in_marathi = "NA";
                        applicantMasterModel.mname_in_marathi = "NA";
                        applicantMasterModel.lname_in_marathi = "NA";
                        applicantMasterModel.prefix_in_eng = "NA";
                        applicantMasterModel.fname_in_eng = "NA";
                        applicantMasterModel.mname_in_eng = "NA";
                        applicantMasterModel.lname_in_eng = "NA";
                    }
                    applicantMasterModel.address_type = createApplicantData.address!.addressType!.Trim().ToUpper();
                    if (createApplicantData.address.addressType.Trim().ToUpper() == "INDIA")
                    {
                        applicantMasterModel.state = createApplicantData.address.indiaAddress!.state!;
                        applicantMasterModel.district = createApplicantData.address.indiaAddress.district!;
                        applicantMasterModel.city = createApplicantData.address.indiaAddress.state!;
                        applicantMasterModel.taluka = createApplicantData.address.indiaAddress.taluka!;
                        applicantMasterModel.flatno_plotno = createApplicantData.address.indiaAddress.plotNo!;
                        applicantMasterModel.societyname = createApplicantData.address.indiaAddress.building!;
                        applicantMasterModel.mainstreet = createApplicantData.address.indiaAddress.mainRoad!;
                        applicantMasterModel.landmark = createApplicantData.address.indiaAddress.impSymbol!;
                        applicantMasterModel.locality = createApplicantData.address.indiaAddress.area!;
                        applicantMasterModel.pincode = createApplicantData.address.indiaAddress.pincode!;
                        applicantMasterModel.postofficename = createApplicantData.address.indiaAddress.postOfficeName!;
                        applicantMasterModel.address_proof_document_name = createApplicantData.address.indiaAddress.addressProofName!;
                        applicantMasterModel.mobileno = createApplicantData.address.indiaAddress.mobile!;
                        applicantMasterModel.mobilenoverified = createApplicantData.address.indiaAddress.mobileOTP!.Trim().ToUpper();
                        applicantMasterModel.emailid = createApplicantData.address.indiaAddress.email;
                        applicantMasterModel.emailidverified = createApplicantData.address.indiaAddress.emailOTP.Trim().ToUpper();
                        applicantMasterModel.securitypin = createApplicantData.address.indiaAddress.securityKey;
                        applicantMasterModel.signed_file_name = createApplicantData.address.indiaAddress.signatureName!;
                        applicantMasterModel.address = "NA";
                    }
                    else if (createApplicantData.address.addressType.Trim().ToUpper() == "FOREIGN")
                    {
                        applicantMasterModel.address = createApplicantData.address.foreignAddress!.address!;
                        applicantMasterModel.mobileno = createApplicantData.address.foreignAddress.mobile!;
                        applicantMasterModel.mobilenoverified = "NO";
                        applicantMasterModel.address_proof_document_name = "NA";
                        applicantMasterModel.address_proof_document_path = "NA";
                        applicantMasterModel.emailid = createApplicantData.address.foreignAddress.email!;
                        applicantMasterModel.emailidverified = createApplicantData.address.foreignAddress.emailOTP!.Trim().ToUpper();
                        applicantMasterModel.signed_file_name = createApplicantData.address.foreignAddress.signatureName!;

                        applicantMasterModel.securitypin = "NA";
                        applicantMasterModel.state = "NA";
                        applicantMasterModel.district = "NA";
                        applicantMasterModel.city = "NA";
                        applicantMasterModel.taluka = "NA";
                        applicantMasterModel.flatno_plotno = "NA";
                        applicantMasterModel.societyname = "NA";
                        applicantMasterModel.mainstreet = "NA";
                        applicantMasterModel.landmark = "NA";
                        applicantMasterModel.locality = "NA";
                        applicantMasterModel.pincode = "NA";
                        applicantMasterModel.postofficename = "NA";
                    }

                    ApplicationDTL applicationDTL = _context.applicationDTL.FirstOrDefault(s => s.applicationid == createApplicantData.applicationId)!;
                    applicantMasterModel.applicationDTL = applicationDTL;

                    UserMaster userMaster = _context.userMasters.FirstOrDefault(s => s.userid == createApplicantData.userId)!;
                    applicantMasterModel.userMaster = userMaster;


                    //Assign Data to Table fields to insert new records
                    dbTable.usertype_code = applicantMasterModel.usertype_code;
                    dbTable.usertype = applicantMasterModel.usertype;
                    dbTable.mobileno = applicantMasterModel.mobileno;
                    dbTable.mobilenoverified = applicantMasterModel.mobilenoverified;
                    dbTable.emailid = applicantMasterModel.emailid;
                    dbTable.emailidverified = applicantMasterModel.emailidverified;
                    dbTable.securitypin = applicantMasterModel.securitypin;
                    dbTable.prefix_in_eng = string.IsNullOrEmpty(applicantMasterModel.prefix_in_eng.Trim()) ? "NA" : applicantMasterModel.prefix_in_eng.Trim();
                    dbTable.fname_in_eng = string.IsNullOrEmpty(applicantMasterModel.fname_in_eng.Trim()) ? "NA" : applicantMasterModel.fname_in_eng.Trim();
                    dbTable.mname_in_eng = string.IsNullOrEmpty(applicantMasterModel.mname_in_eng.Trim()) ? "NA" : applicantMasterModel.mname_in_eng.Trim();
                    dbTable.lname_in_eng = string.IsNullOrEmpty(applicantMasterModel.lname_in_eng.Trim()) ? "NA" : applicantMasterModel.lname_in_eng.Trim();
                    dbTable.prefix_in_marathi = string.IsNullOrEmpty(applicantMasterModel.prefix_in_marathi.Trim()) ? "NA" : applicantMasterModel.prefix_in_marathi.Trim();
                    dbTable.fname_in_marathi = string.IsNullOrEmpty(applicantMasterModel.fname_in_marathi.Trim()) ? "NA" : applicantMasterModel.fname_in_marathi.Trim();
                    dbTable.mname_in_marathi = string.IsNullOrEmpty(applicantMasterModel.mname_in_marathi.Trim()) ? "NA" : applicantMasterModel.mname_in_marathi.Trim();
                    dbTable.lname_in_marathi = string.IsNullOrEmpty(applicantMasterModel.lname_in_marathi.Trim()) ? "NA" : applicantMasterModel.lname_in_marathi.Trim();
                    dbTable.address_type = applicantMasterModel.address_type;
                    dbTable.address = string.IsNullOrEmpty(applicantMasterModel.address) ? "NA" : applicantMasterModel.address;
                    dbTable.state = applicantMasterModel.state;
                    dbTable.district = applicantMasterModel.district;
                    dbTable.taluka = applicantMasterModel.taluka;
                    dbTable.city = applicantMasterModel.city;
                    dbTable.flatno_plotno = applicantMasterModel.flatno_plotno;
                    dbTable.societyname = string.IsNullOrEmpty(applicantMasterModel.societyname) ? "NA" : applicantMasterModel.societyname;
                    dbTable.mainstreet = string.IsNullOrEmpty(applicantMasterModel.mainstreet) ? "NA" : applicantMasterModel.mainstreet;
                    dbTable.landmark = applicantMasterModel.landmark;
                    dbTable.locality = string.IsNullOrEmpty(applicantMasterModel.locality) ? "NA" : applicantMasterModel.locality;
                    dbTable.pincode = applicantMasterModel.pincode;
                    dbTable.postofficename = applicantMasterModel.postofficename;
                    dbTable.owner_of_property_in_maharashtra = applicantMasterModel.owner_of_property_in_maharashtra;
                    dbTable.PropertyTypeMaster = applicantMasterModel.PropertyTypeMaster;
                    dbTable.property_district_code = applicantMasterModel.property_district_code;
                    dbTable.property_district_name = applicantMasterModel.property_district_name;
                    dbTable.property_taluka_code = applicantMasterModel.property_taluka_code;
                    dbTable.property_taluka_name = applicantMasterModel.property_taluka_name;
                    dbTable.property_village_code = applicantMasterModel.property_village_code;
                    dbTable.property_village_name = applicantMasterModel.property_village_name;
                    dbTable.khateno = applicantMasterModel.khateno;
                    dbTable.city_servey_no = applicantMasterModel.city_servey_no;
                    dbTable.ulpin = applicantMasterModel.ulpin;
                    dbTable.username = string.IsNullOrEmpty(applicantMasterModel.username) ? "NA" : applicantMasterModel.username;
                    dbTable.company_name_in_eng = applicantMasterModel.company_name_in_eng;
                    dbTable.company_name_in_marathi = applicantMasterModel.company_name_in_marathi;
                    dbTable.userMaster = applicantMasterModel.userMaster;
                    dbTable.applicationDTL = applicantMasterModel.applicationDTL;
                    _context.applicantMasters.Add(dbTable);
                    _context.SaveChanges();

                    //Get Saved Row ID
                    int applicantID = dbTable.applicantid;

                    string VerificationType = string.Empty;
                    bool checkAddressFlag = true;
                    bool checkSignFlag = true;
                    string CurrentDateTime = DateTime.Now.ToString("yyyy-MMM-dd-HHmmss");

                    if (applicantMasterModel.address_type == "INDIA")
                    {
                        VerificationType = "MOBILENO";
                        //applicantID = FetchApplicantID(applicantMasterModel.mobileno, VerificationType);
                        if (!string.IsNullOrEmpty(createApplicantData.address.indiaAddress!.addressProofSrc) && createApplicantData.address.indiaAddress.addressProofSrc != "NA")
                        {
                            string[] AddressData = createApplicantData.address.indiaAddress.addressProofSrc.Split(",");
                            checkAddressFlag = methodForFile.SaveImageForApplicant(AddressData[1], createApplicantData.address.indiaAddress.addressProofName!, applicantID.ToString(), "AddressProof", FolderPath + @"\", CurrentDateTime);
                            string AddressProofExt = Path.GetExtension(createApplicantData.address.indiaAddress.addressProofName!);
                            applicantMasterModel.address_proof_document_name = "AddressProof" + applicantID + "_" + CurrentDateTime + AddressProofExt;
                            applicantMasterModel.address_proof_document_path = FolderPath + applicantID + @"\" + applicantMasterModel.address_proof_document_name;

                            var UpdateAddressFilePath = _context.applicantMasters.Where(w => w.applicantid == applicantID).FirstOrDefault();
                            if (UpdateAddressFilePath != null)
                            {
                                dbTable.address_proof_document_name = applicantMasterModel.address_proof_document_name;
                                dbTable.address_proof_document_path = applicantMasterModel.address_proof_document_path;
                                _context.Entry(dbTable).CurrentValues.SetValues(dbTable);
                                _context.SaveChanges();
                            }
                        }
                        else
                        {
                            dbTable.address_proof_document_name = "NA";
                            dbTable.address_proof_document_path = "NA";
                            _context.Entry(dbTable).CurrentValues.SetValues(dbTable);
                            _context.SaveChanges();
                        }

                        if (!string.IsNullOrEmpty(createApplicantData.address.indiaAddress.signatureSrc) && createApplicantData.address.indiaAddress.signatureSrc != "NA")
                        {
                            string[] signData = createApplicantData.address.indiaAddress.signatureSrc.Split(",");
                            checkSignFlag = methodForFile.SaveImageForApplicant(signData[1], createApplicantData.address.indiaAddress.signatureName!, applicantID.ToString(), "Signature", FolderPath + @"\", CurrentDateTime);
                            string SignatureExt = Path.GetExtension(createApplicantData.address.indiaAddress.signatureName!);
                            applicantMasterModel.signed_file_name = "Signature" + applicantID + "_" + CurrentDateTime + SignatureExt;
                            applicantMasterModel.signed_file_path = FolderPath + applicantID + @"\" + applicantMasterModel.signed_file_name;

                            var UpdateSignFilePath = _context.applicantMasters.Where(w => w.applicantid == applicantID).FirstOrDefault();
                            if (UpdateSignFilePath != null)
                            {
                                dbTable.signed_file_name = applicantMasterModel.signed_file_name;
                                dbTable.signed_file_path = applicantMasterModel.signed_file_path;
                                _context.Entry(dbTable).CurrentValues.SetValues(dbTable);
                                _context.SaveChanges();
                            }
                        }
                        else
                        {
                            dbTable.signed_file_name = "NA";
                            dbTable.signed_file_path = "NA";
                            _context.Entry(dbTable).CurrentValues.SetValues(dbTable);
                            _context.SaveChanges();
                        }
                    }
                    if (applicantMasterModel.address_type == "FOREIGN")
                    {
                        VerificationType = "EMAILID";
                        if (!string.IsNullOrEmpty(createApplicantData.address.foreignAddress!.signatureSrc) && createApplicantData.address.foreignAddress.signatureSrc != "NA")
                        {
                            string[] signData = createApplicantData.address.foreignAddress.signatureSrc.Split(",");
                            checkSignFlag = methodForFile.SaveImageForApplicant(signData[1], createApplicantData.address.foreignAddress.signatureName!, applicantID.ToString(), "Signature", FolderPath + @"\", CurrentDateTime);

                            string SignatureExt = Path.GetExtension(createApplicantData.address.foreignAddress.signatureName!);
                            applicantMasterModel.signed_file_name = "Signature" + applicantID + "_" + CurrentDateTime + SignatureExt;
                            applicantMasterModel.signed_file_path = FolderPath + applicantID + @"\" + applicantMasterModel.signed_file_name;
                            var UpdateSignFilePath = _context.applicantMasters.Where(w => w.applicantid == applicantID).FirstOrDefault();
                            if (UpdateSignFilePath != null)
                            {
                                dbTable.signed_file_name = applicantMasterModel.signed_file_name;
                                dbTable.signed_file_path = applicantMasterModel.signed_file_path;
                                _context.Entry(dbTable).CurrentValues.SetValues(dbTable);
                                _context.SaveChanges();
                            }
                        }
                        else
                        {
                            dbTable.signed_file_name = "NA";
                            dbTable.signed_file_path = "NA";
                            _context.Entry(dbTable).CurrentValues.SetValues(dbTable);
                            _context.SaveChanges();
                        }
                    }
                    bool checkImgFlag = false;
                    if (createApplicantData.photo.passportSrc == "NA")
                    {
                        checkImgFlag = true;
                        dbTable.profile_pic_file_name = "NA";
                        dbTable.profile_pic_file_path = "NA";
                        _context.Entry(dbTable).CurrentValues.SetValues(dbTable);
                        _context.SaveChanges();
                    }
                    else
                    {
                        string[] imgData = createApplicantData.photo.passportSrc!.Split(",");
                        checkImgFlag = methodForFile.SaveImageForApplicant(imgData[1], createApplicantData.photo.passportName!, applicantID.ToString(), "PassportPhoto", FolderPath + @"\", CurrentDateTime);
                        string ImgExt = Path.GetExtension(createApplicantData.photo.passportName!);
                        applicantMasterModel.profile_pic_file_name = "PassportPhoto" + applicantID + "_" + CurrentDateTime + ImgExt;
                        applicantMasterModel.profile_pic_file_path = FolderPath + applicantID + @"\" + applicantMasterModel.profile_pic_file_name;
                        var UpdateImgFilePath = _context.applicantMasters.Where(w => w.applicantid == applicantID).FirstOrDefault();
                        if (UpdateImgFilePath != null)
                        {
                            dbTable.profile_pic_file_name = applicantMasterModel.profile_pic_file_name;
                            dbTable.profile_pic_file_path = applicantMasterModel.profile_pic_file_path;
                            _context.Entry(dbTable).CurrentValues.SetValues(dbTable);
                            _context.SaveChanges();
                        }
                    }
                    if (checkAddressFlag && checkImgFlag && checkSignFlag)
                    {
                        //ApplicationDTL applicationDTLTB = new ApplicationDTL();
                        var applicationDTLdata = _context.applicationDTL.Where(data => data.applicationid!.Equals(createApplicantData.applicationId)).FirstOrDefault();
                        if (applicationDTLdata != null)
                        {
                            if (!string.IsNullOrEmpty(applicationDTLdata.applicantIDs) && !applicationDTLdata.applicantIDs.Contains(applicantID.ToString()))
                            {
                                applicationDTLdata.applicantIDs = applicationDTLdata.applicantIDs + "," + applicantID.ToString();
                            }
                            else
                            {
                                applicationDTLdata.applicantIDs = applicantID.ToString();
                            }
                            applicationDTLdata.status = 2;
                            _context.Entry(applicationDTLdata).CurrentValues.SetValues(applicationDTLdata);
                            _context.SaveChanges();
                            scope.Complete();
                        }
                        return "Success";
                    }
                    if (!checkImgFlag)
                    {
                        _context.applicantMasters.Remove(dbTable);
                        _context.SaveChanges();
                        methodForFile.DeleteFile(applicantMasterModel.address_proof_document_path);
                        methodForFile.DeleteFile(applicantMasterModel.signed_file_path);
                        return "Profile Picture Is Not Uploaded";
                    }
                    if (!checkAddressFlag)
                    {
                        _context.applicantMasters.Remove(dbTable);
                        _context.SaveChanges();
                        methodForFile.DeleteFile(applicantMasterModel.profile_pic_file_path);
                        methodForFile.DeleteFile(applicantMasterModel.signed_file_path);
                        return "Address Proof File Is Not Uploaded";
                    }
                    if (!checkSignFlag)
                    {
                        _context.applicantMasters.Remove(dbTable);
                        _context.SaveChanges();
                        methodForFile.DeleteFile(applicantMasterModel.profile_pic_file_path);
                        methodForFile.DeleteFile(applicantMasterModel.address_proof_document_path);
                        return "Signature File Is Not Uploaded";
                    }
                    else
                    {
                        _context.applicantMasters.Remove(dbTable);
                        _context.SaveChanges();
                        return "Some Files Are Not Uploaded";
                    }
                }
                catch (Exception ex)
                {
                    _context.applicantMasters.Remove(dbTable);
                    _context.SaveChanges();
                    throw new HandleException(ex.Message.ToString());
                }
            }
        }


        public FetchApplicantsData FetchApplicantData(int applicantid)
        {
            try
            {
                ApplicantMaster applicant = new ApplicantMaster();
                MethodForFileUpload methodForFile = new MethodForFileUpload();
                applicant = _contextR.applicantMasters.Include(i => i.PropertyTypeMaster).Include(app => app.applicationDTL).Include(u => u.userMaster).Where(data => data.applicantid.Equals(applicantid) && data.isDeleted == false).FirstOrDefault()!;
                FetchApplicantsData fetchData = new FetchApplicantsData();
                if (applicant != null)
                {
                    string ProfilePicExt = Path.GetExtension(applicant.profile_pic_file_path);
                    string ProfilePic = methodForFile.ConvertImageToBase64(applicant.profile_pic_file_path);
                    fetchData.profile_pic_file_path = string.IsNullOrEmpty(ProfilePic) ? "NA" : "data:image/" + ProfilePicExt.Replace(".", "") + ";base64," + ProfilePic;

                    string AddressProofExt = Path.GetExtension(applicant.address_proof_document_path);
                    string AddressProof = methodForFile.ConvertImageToBase64(applicant.address_proof_document_path);
                    fetchData.address_proof_document_path = string.IsNullOrEmpty(AddressProof) ? "NA" : "data:image/" + AddressProofExt.Replace(".", "") + ";base64," + AddressProof;

                    string SignatureExt = Path.GetExtension(applicant.signed_file_path);
                    string Signature = methodForFile.ConvertImageToBase64(applicant.signed_file_path);
                    fetchData.signed_file_path = string.IsNullOrEmpty(Signature) ? "NA" : "data:image/" + SignatureExt.Replace(".", "") + ";base64," + Signature;

                    fetchData.userid = applicant.userMaster!.userid;
                    fetchData.applicantid = applicant.applicantid;
                    fetchData.applicationid = applicant.applicationDTL!.applicationid;
                    fetchData.usertype_code = applicant.usertype_code;
                    fetchData.usertype = applicant.usertype;
                    fetchData.mobileno = applicant.mobileno;
                    fetchData.mobilenoverified = applicant.mobilenoverified;
                    fetchData.emailid = applicant.emailid;
                    fetchData.emailidverified = applicant.emailidverified;
                    fetchData.securitypin = applicant.securitypin;
                    fetchData.prefix_in_eng = applicant.prefix_in_eng;
                    fetchData.fname_in_eng = applicant.fname_in_eng;
                    fetchData.mname_in_eng = applicant.mname_in_eng;
                    fetchData.lname_in_eng = applicant.lname_in_eng;
                    fetchData.applicantNameInEnglish = applicant.fname_in_eng.Trim() + " " + applicant.mname_in_eng.Trim() + " " + applicant.lname_in_eng.Trim();
                    fetchData.prefix_in_marathi = applicant.prefix_in_marathi;
                    fetchData.fname_in_marathi = applicant.fname_in_marathi;
                    fetchData.mname_in_marathi = applicant.mname_in_marathi;
                    fetchData.lname_in_marathi = applicant.lname_in_marathi;
                    fetchData.applicantNameInMarathi = applicant.fname_in_marathi.Trim() + " " + applicant.mname_in_marathi.Trim() + " " + applicant.lname_in_marathi.Trim();
                    fetchData.company_name_in_eng = applicant.company_name_in_eng;
                    fetchData.company_name_in_marathi = applicant.company_name_in_marathi;
                    fetchData.username = applicant.username;
                    fetchData.address_type = applicant.address_type;
                    fetchData.address = applicant.address;
                    fetchData.state = applicant.state;
                    fetchData.district = applicant.district;
                    fetchData.taluka = applicant.taluka;
                    fetchData.city = applicant.city;
                    fetchData.flatno_plotno = applicant.flatno_plotno;
                    fetchData.societyname = applicant.societyname;
                    fetchData.mainstreet = applicant.mainstreet;
                    fetchData.landmark = applicant.landmark;
                    fetchData.locality = applicant.locality;
                    fetchData.pincode = applicant.pincode;
                    fetchData.postofficename = applicant.postofficename;
                    fetchData.address_proof_document_name = applicant.address_proof_document_name;
                    fetchData.owner_of_property_in_maharashtra = applicant.owner_of_property_in_maharashtra;
                    fetchData.propertyType_Code = applicant.PropertyTypeMaster!.propertytypeid.ToString();
                    fetchData.propertyType = applicant.PropertyTypeMaster.propertytype;
                    fetchData.property_district_code = applicant.property_district_code;
                    fetchData.property_district_name = applicant.property_district_name;
                    fetchData.property_taluka_code = applicant.property_taluka_code;
                    fetchData.property_taluka_name = applicant.property_taluka_name;
                    fetchData.property_village_code = applicant.property_village_code;
                    fetchData.property_village_name = applicant.property_village_name;
                    fetchData.khateno = applicant.khateno!;
                    fetchData.city_servey_no = applicant.city_servey_no!;
                    fetchData.ulpin = applicant.ulpin!;
                    fetchData.profile_pic_file_name = applicant.profile_pic_file_name;
                    fetchData.signed_file_name = applicant.signed_file_name;
                    TimeZoneInfo INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
                    DateTime indianTime = TimeZoneInfo.ConvertTimeFromUtc(Convert.ToDateTime(applicant.createddatetime.ToString()), INDIAN_ZONE);
                    fetchData.createddatetime = indianTime.ToString("dd/MM/yyyy");
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

        //public FetchApplicantsData FetchApplicantData(int applicantid)
        //{
        //    try
        //    {
        //        ApplicantMaster applicant = new ApplicantMaster();
        //        MethodForFileUpload methodForFile = new MethodForFileUpload();
        //        applicant = _contextR.applicantMasters.Include(i => i.PropertyTypeMaster).Include(app=>app.applicationDTL).Include(u => u.userMaster).Where(data => data.applicantid.Equals(applicantid) && data.isDeleted == false).FirstOrDefault()!;
        //        FetchApplicantsData fetchData = new FetchApplicantsData();
        //        if (applicant != null)
        //        {
        //            string ProfilePicExt = Path.GetExtension(applicant.profile_pic_file_path);
        //            string ProfilePic = methodForFile.ConvertImageToBase64(applicant.profile_pic_file_path);
        //            applicant.profile_pic_file_path = string.IsNullOrEmpty(ProfilePic) ? "NA" : "data:image/" + ProfilePicExt.Replace(".", "") + ";base64," + ProfilePic;

        //            string AddressProofExt = Path.GetExtension(applicant.address_proof_document_path);
        //            string AddressProof = methodForFile.ConvertImageToBase64(applicant.address_proof_document_path);
        //            applicant.address_proof_document_path = string.IsNullOrEmpty(AddressProof) ? "NA" : "data:image/" + AddressProofExt.Replace(".", "") + ";base64," + AddressProof;

        //            string SignatureExt = Path.GetExtension(applicant.signed_file_path);
        //            string Signature = methodForFile.ConvertImageToBase64(applicant.signed_file_path);
        //            applicant.signed_file_path = string.IsNullOrEmpty(Signature) ? "NA" : "data:image/" + SignatureExt.Replace(".", "") + ";base64," + Signature;

        //            fetchData.userid = applicant.userMaster!.userid;
        //            fetchData.applicantid = applicant.applicantid;
        //            fetchData.applicationid = applicant.applicationDTL!.applicationid;
        //            fetchData.usertype_code = applicant.usertype_code;
        //            fetchData.usertype = applicant.usertype;
        //            fetchData.mobileno = applicant.mobileno;
        //            fetchData.mobilenoverified = applicant.mobilenoverified;
        //            fetchData.emailid = applicant.emailid;
        //            fetchData.emailidverified = applicant.emailidverified;
        //            fetchData.securitypin = applicant.securitypin;
        //            fetchData.prefix_in_eng = applicant.prefix_in_eng;
        //            fetchData.fname_in_eng = applicant.fname_in_eng;
        //            fetchData.mname_in_eng = applicant.mname_in_eng;
        //            fetchData.lname_in_eng = applicant.lname_in_eng;
        //            fetchData.applicantNameInEnglish = applicant.fname_in_eng.Trim() + " " + applicant.mname_in_eng.Trim() + " " + applicant.lname_in_eng.Trim();
        //            fetchData.prefix_in_marathi = applicant.prefix_in_marathi;
        //            fetchData.fname_in_marathi = applicant.fname_in_marathi;
        //            fetchData.mname_in_marathi = applicant.mname_in_marathi;
        //            fetchData.lname_in_marathi = applicant.lname_in_marathi;
        //            fetchData.applicantNameInMarathi = applicant.fname_in_marathi.Trim() + " " + applicant.mname_in_marathi.Trim() + " " + applicant.lname_in_marathi.Trim();
        //            fetchData.company_name_in_eng = applicant.company_name_in_eng;
        //            fetchData.company_name_in_marathi = applicant.company_name_in_marathi;
        //            fetchData.username = applicant.username;
        //            fetchData.address_type = applicant.address_type;
        //            fetchData.address = applicant.address;
        //            fetchData.state = applicant.state;
        //            fetchData.district = applicant.district;
        //            fetchData.taluka = applicant.taluka;
        //            fetchData.city = applicant.city;
        //            fetchData.flatno_plotno = applicant.flatno_plotno;
        //            fetchData.societyname = applicant.societyname;
        //            fetchData.mainstreet = applicant.mainstreet;
        //            fetchData.landmark = applicant.landmark;
        //            fetchData.locality = applicant.locality;
        //            fetchData.pincode = applicant.pincode;
        //            fetchData.postofficename = applicant.postofficename;
        //            fetchData.address_proof_document_name = applicant.address_proof_document_name;
        //            fetchData.address_proof_document_path = applicant.address_proof_document_path;
        //            fetchData.owner_of_property_in_maharashtra = applicant.owner_of_property_in_maharashtra;
        //            fetchData.propertyType_Code = applicant.PropertyTypeMaster!.propertytypeid.ToString();
        //            fetchData.propertyType = applicant.PropertyTypeMaster.propertytype;
        //            fetchData.property_district_code = applicant.property_district_code;
        //            fetchData.property_district_name = applicant.property_district_name;
        //            fetchData.property_taluka_code = applicant.property_taluka_code;
        //            fetchData.property_taluka_name = applicant.property_taluka_name;
        //            fetchData.property_village_code = applicant.property_village_code;
        //            fetchData.property_village_name = applicant.property_village_name;
        //            fetchData.khateno = applicant.khateno!;
        //            fetchData.city_servey_no = applicant.city_servey_no!;
        //            fetchData.ulpin = applicant.ulpin!;
        //            fetchData.profile_pic_file_name = applicant.profile_pic_file_name;
        //            fetchData.profile_pic_file_path = applicant.profile_pic_file_path;
        //            fetchData.signed_file_name = applicant.signed_file_name;
        //            fetchData.signed_file_path = applicant.signed_file_path;
        //            TimeZoneInfo INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
        //            DateTime indianTime = TimeZoneInfo.ConvertTimeFromUtc(Convert.ToDateTime(applicant.createddatetime.ToString()), INDIAN_ZONE);
        //            fetchData.createddatetime = indianTime.ToString("dd/MM/yyyy");
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

        /*// Add Mutation CTS in created application
        public string SaveMutationCTSNoData(MutationCTSNoData mutationCTSNoData)
        {
            try
            {
                MutationCTSNoDTL fetchMutationCTSNoDTL = new MutationCTSNoDTL();
                fetchMutationCTSNoDTL = _context.mutationCTSNoDTLs.Include(app => app.applicationDTL).Where(data => data.applicationDTL!.applicationid!.Equals(mutationCTSNoData.applicationid)
                && data.village_or_peth_code != mutationCTSNoData.village!.village_code
                ).FirstOrDefault()!;
                if (fetchMutationCTSNoDTL != null)
                {
                    return "Could Not Add Another Village For The Same Application";
                }
                // Assign Values to Model
                MutationCTSNoDTLModel mutationCTSNoDTLModel = new MutationCTSNoDTLModel();
                UserMaster userMaster = _context.userMasters.FirstOrDefault(s => s.userid == mutationCTSNoData.userid)!;
                mutationCTSNoDTLModel.userMaster = userMaster;

                ApplicationDTL applicationDTL = _context.applicationDTL.FirstOrDefault(s => s.applicationid == mutationCTSNoData.applicationid)!;
                mutationCTSNoDTLModel.applicationDTL = applicationDTL;

                if (mutationCTSNoData.inDast == "surveynoorgatno")
                {
                    mutationCTSNoDTLModel.what_is_mentioned_in_the_doc = "SERVEYNO / GROUPNO";
                    mutationCTSNoDTLModel.servey_no = mutationCTSNoData.surveyNo;
                }
                if (mutationCTSNoData.inDast == "nabhu")
                {
                    mutationCTSNoDTLModel.what_is_mentioned_in_the_doc = "CITY SERVEY NO";
                    mutationCTSNoDTLModel.city_servey_no_mentioned_in_application = mutationCTSNoData.nabhuNo;
                }
                mutationCTSNoDTLModel.village_or_peth_name = textInfo.ToTitleCase(mutationCTSNoData.village!.village_name!.Trim());
                mutationCTSNoDTLModel.village_or_peth_code = mutationCTSNoData.village.village_code!.Trim();
                mutationCTSNoDTLModel.mutation_modification_type = mutationCTSNoData.milkat!.Trim().ToUpper();
                mutationCTSNoDTLModel.selected_city_servey_no = mutationCTSNoData.naBhu;
                mutationCTSNoDTLModel.lr_property_uid = mutationCTSNoData.lrPropertyUID;
                mutationCTSNoDTLModel.application_income_type = mutationCTSNoData.namud!.Trim();
                mutationCTSNoDTLModel.city_servey_area_in_sq_m = mutationCTSNoData.cityServeyAreaInSqm;
                //mutationCTSNoDTLModel.buildup_area_in_sq_m = mutationCTSNoData.fetchedBuildupArea;

                //if (mutationCTSNoData.milkat.Trim().ToUpper() == "FLAT")
                //{
                if (mutationCTSNoData.flatDetails != null && mutationCTSNoDTLModel.application_income_type.Trim().ToUpper() == "OTHER")
                {
                    mutationCTSNoDTLModel.building_name = string.IsNullOrEmpty(mutationCTSNoData.flatDetails.buildingName) ? "NA" : mutationCTSNoData.flatDetails.buildingName.Trim();
                    mutationCTSNoDTLModel.floor_type = string.IsNullOrEmpty(mutationCTSNoData.flatDetails.floorType) ? "NA" : mutationCTSNoData.flatDetails.floorType.Trim();
                    mutationCTSNoDTLModel.floor_no = string.IsNullOrEmpty(mutationCTSNoData.flatDetails.floorNo) ? "NA" : mutationCTSNoData.flatDetails.floorNo.Trim();
                    mutationCTSNoDTLModel.unit_type = string.IsNullOrEmpty(mutationCTSNoData.flatDetails.unitType) ? "NA" : mutationCTSNoData.flatDetails.unitType.Trim();
                    mutationCTSNoDTLModel.unit_no = string.IsNullOrEmpty(mutationCTSNoData.flatDetails.unitNo) ? "NA" : mutationCTSNoData.flatDetails.unitNo.Trim();
                    mutationCTSNoDTLModel.carpet_area_in_sq_m = string.IsNullOrEmpty(mutationCTSNoData.flatDetails.carpetArea) ? "NA" : mutationCTSNoData.flatDetails.carpetArea.Trim();
                    mutationCTSNoDTLModel.terrace_area_in_sq_m = string.IsNullOrEmpty(mutationCTSNoData.flatDetails.taraceArea) ? "NA" : mutationCTSNoData.flatDetails.taraceArea.Trim();
                    mutationCTSNoDTLModel.parking_no = string.IsNullOrEmpty(mutationCTSNoData.flatDetails.parkingNo) ? "NA" : mutationCTSNoData.flatDetails.parkingNo.Trim();
                    mutationCTSNoDTLModel.parking_area_in_sq_m = string.IsNullOrEmpty(mutationCTSNoData.flatDetails.parkingArea) ? "NA" : mutationCTSNoData.flatDetails.parkingArea.Trim();
                    mutationCTSNoDTLModel.shares_in_percent = string.IsNullOrEmpty(mutationCTSNoData.flatDetails.hissa) ? "NA" : mutationCTSNoData.flatDetails.hissa.Trim();
                    mutationCTSNoDTLModel.buildup_area_in_sq_m = string.IsNullOrEmpty(mutationCTSNoData.flatDetails.buildupArea) ? "NA" : mutationCTSNoData.flatDetails.buildupArea.Trim();
                }
                if (mutationCTSNoDTLModel.application_income_type.Trim().ToUpper() != "OTHER")
                {
                    mutationCTSNoDTLModel.nic_flat_details = mutationCTSNoData.nic_flat_details;
                    mutationCTSNoDTLModel.flat_bulit_up_area = mutationCTSNoData.flatBuiltUpArea;
                }
                //}
                //else
                //{
                //    mutationCTSNoDTLModel.building_name = "NA";
                //    mutationCTSNoDTLModel.floor_type = "NA";
                //    mutationCTSNoDTLModel.floor_no = "NA";
                //    mutationCTSNoDTLModel.unit_type = "NA";
                //    mutationCTSNoDTLModel.unit_no = "NA";
                //    mutationCTSNoDTLModel.buildup_area_in_sq_m = "NA";
                //    mutationCTSNoDTLModel.carpet_area_in_sq_m = "NA";
                //    mutationCTSNoDTLModel.terrace_area_in_sq_m = "NA";
                //    mutationCTSNoDTLModel.parking_no = "NA";
                //    mutationCTSNoDTLModel.parking_area_in_sq_m = "NA";
                //    mutationCTSNoDTLModel.shares_in_percent = "NA";
                //}

                //Assign Data to Table fields to insert new records
                MutationCTSNoDTL dbTable = new MutationCTSNoDTL();
                //Set Default Values

                dbTable.what_is_mentioned_in_the_doc = "NA";
                dbTable.village_or_peth_code = "NA";
                dbTable.village_or_peth_name = "NA";
                dbTable.mutation_modification_type = "NA";
                dbTable.city_servey_no_mentioned_in_application = "NA";
                dbTable.servey_no = "NA";
                dbTable.selected_city_servey_no = "NA";
                dbTable.lr_property_uid = "NA";
                dbTable.application_income_type = "NA";
                dbTable.city_servey_area_in_sq_m = "NA";
                dbTable.building_name = "NA";
                dbTable.floor_type = "NA";
                dbTable.floor_no = "NA";
                dbTable.unit_type = "NA";
                dbTable.unit_no = "NA";
                dbTable.buildup_area_in_sq_m = "NA";
                dbTable.carpet_area_in_sq_m = "NA";
                dbTable.terrace_area_in_sq_m = "NA";
                dbTable.parking_area_in_sq_m = "NA";
                dbTable.parking_no = "NA";
                dbTable.shares_in_percent = "NA";
                dbTable.nic_flat_details = "NA";
                dbTable.flat_built_up_area = "NA";

                // Assign Values
                dbTable.userMaster = mutationCTSNoDTLModel.userMaster;
                dbTable.applicationDTL = mutationCTSNoDTLModel.applicationDTL;
                dbTable.what_is_mentioned_in_the_doc = mutationCTSNoDTLModel.what_is_mentioned_in_the_doc;
                dbTable.village_or_peth_code = mutationCTSNoDTLModel.village_or_peth_code;
                dbTable.village_or_peth_name = mutationCTSNoDTLModel.village_or_peth_name;
                dbTable.mutation_modification_type = mutationCTSNoDTLModel.mutation_modification_type;
                dbTable.city_servey_no_mentioned_in_application = string.IsNullOrEmpty(mutationCTSNoDTLModel.city_servey_no_mentioned_in_application) ? "NA" : mutationCTSNoDTLModel.city_servey_no_mentioned_in_application;
                dbTable.servey_no = string.IsNullOrEmpty(mutationCTSNoDTLModel.servey_no) ? "NA" : mutationCTSNoDTLModel.servey_no;
                dbTable.selected_city_servey_no = mutationCTSNoDTLModel.selected_city_servey_no;
                dbTable.lr_property_uid = mutationCTSNoDTLModel.lr_property_uid;
                dbTable.application_income_type = mutationCTSNoDTLModel.application_income_type;
                dbTable.city_servey_area_in_sq_m = mutationCTSNoDTLModel.city_servey_area_in_sq_m;

                // Flat Details Entered By User
                if (mutationCTSNoDTLModel.application_income_type.Trim().ToUpper() == "OTHER")
                {
                    dbTable.building_name = string.IsNullOrEmpty(mutationCTSNoDTLModel.building_name!.Trim()) ? "NA" : mutationCTSNoDTLModel.building_name.Trim();
                    dbTable.floor_type = string.IsNullOrEmpty(mutationCTSNoDTLModel.floor_type!.Trim()) ? "NA" : mutationCTSNoDTLModel.floor_type;
                    dbTable.floor_no = string.IsNullOrEmpty(mutationCTSNoDTLModel.floor_no) ? "NA" : mutationCTSNoDTLModel.floor_no;
                    dbTable.unit_type = string.IsNullOrEmpty(mutationCTSNoDTLModel.unit_type) ? "NA" : mutationCTSNoDTLModel.unit_type;
                    dbTable.unit_no = string.IsNullOrEmpty(mutationCTSNoDTLModel.unit_no) ? "NA" : mutationCTSNoDTLModel.unit_no;
                    dbTable.buildup_area_in_sq_m = string.IsNullOrEmpty(mutationCTSNoDTLModel.buildup_area_in_sq_m) ? "NA" : mutationCTSNoDTLModel.buildup_area_in_sq_m;
                    dbTable.carpet_area_in_sq_m = string.IsNullOrEmpty(mutationCTSNoDTLModel.carpet_area_in_sq_m) ? "NA" : mutationCTSNoDTLModel.carpet_area_in_sq_m;
                    dbTable.terrace_area_in_sq_m = string.IsNullOrEmpty(mutationCTSNoDTLModel.terrace_area_in_sq_m) ? "NA" : mutationCTSNoDTLModel.terrace_area_in_sq_m;
                    dbTable.parking_area_in_sq_m = string.IsNullOrEmpty(mutationCTSNoDTLModel.parking_area_in_sq_m) ? "NA" : mutationCTSNoDTLModel.parking_area_in_sq_m;
                    dbTable.parking_no = string.IsNullOrEmpty(mutationCTSNoDTLModel.parking_no) ? "NA" : mutationCTSNoDTLModel.parking_no;
                    dbTable.shares_in_percent = string.IsNullOrEmpty(mutationCTSNoDTLModel.shares_in_percent) ? "NA" : mutationCTSNoDTLModel.shares_in_percent;
                }
                // End
                else
                {
                    dbTable.nic_flat_details = string.IsNullOrEmpty(mutationCTSNoDTLModel.nic_flat_details) ? "NA" : mutationCTSNoDTLModel.nic_flat_details;
                    dbTable.flat_built_up_area = string.IsNullOrEmpty(mutationCTSNoDTLModel.flat_bulit_up_area) ? "NA" : mutationCTSNoDTLModel.flat_bulit_up_area;
                }

                _context.mutationCTSNoDTLs.Add(dbTable);
                _context.SaveChanges();

                //Get Saved Row ID
                int mutation_cts_no_id = dbTable.mutation_cts_no_id;
                var applicationDTLdata = _context.applicationDTL.Where(data => data.applicationid!.Equals(mutationCTSNoData.applicationid)).FirstOrDefault();
                if (applicationDTLdata != null)
                {
                    if (!string.IsNullOrEmpty(applicationDTLdata.mutation_cts_nos) && !applicationDTLdata.mutation_cts_nos.Contains(mutation_cts_no_id.ToString()))
                    {
                        applicationDTLdata.mutation_cts_nos = applicationDTLdata.mutation_cts_nos + "," + mutation_cts_no_id.ToString();
                    }
                    else
                    {
                        applicationDTLdata.mutation_cts_nos = mutation_cts_no_id.ToString();
                    }
                    applicationDTLdata.status = 3;
                    _context.Entry(applicationDTLdata).CurrentValues.SetValues(applicationDTLdata);
                    _context.SaveChanges();
                }
                return "Success";
            }
            catch (Exception ex)
            {
                throw new HandleException(ex.Message.ToString());
            }
        }

        public FetchMutationCTSNoData FetchMutationCTSData(int mutationCTSNOId)
        {
            try
            {
                MutationCTSNoDTL mutationCTSNoDTL = new MutationCTSNoDTL();
                mutationCTSNoDTL = _context.mutationCTSNoDTLs.Include(app => app.applicationDTL).Where(data => data.mutation_cts_no_id.Equals(mutationCTSNOId)).FirstOrDefault()!;
                FetchMutationCTSNoData fetchData = new FetchMutationCTSNoData();
                if (mutationCTSNoDTL != null)
                {
                    fetchData.mutation_cts_no_id = mutationCTSNoDTL.mutation_cts_no_id;
                    fetchData.applicationid = mutationCTSNoDTL.applicationDTL!.applicationid;
                    fetchData.inDast = mutationCTSNoDTL.what_is_mentioned_in_the_doc;
                    fetchData.milkat = mutationCTSNoDTL.mutation_modification_type;
                    fetchData.nabhuNo = mutationCTSNoDTL.city_servey_no_mentioned_in_application;
                    fetchData.surveyNo = mutationCTSNoDTL.servey_no;
                    fetchData.naBhu = mutationCTSNoDTL.selected_city_servey_no;
                    fetchData.cityServeyAreaInSqm = mutationCTSNoDTL.city_servey_area_in_sq_m;
                    fetchData.lrPropertyUID = mutationCTSNoDTL.lr_property_uid;
                    fetchData.namud = mutationCTSNoDTL.application_income_type;
                    fetchData.floorNo = mutationCTSNoDTL.floor_no;
                    fetchData.floorType = mutationCTSNoDTL.floor_type;
                    fetchData.buildingName = mutationCTSNoDTL.building_name;
                    fetchData.buildupArea = mutationCTSNoDTL.buildup_area_in_sq_m;
                    fetchData.carpetArea = mutationCTSNoDTL.carpet_area_in_sq_m;
                    fetchData.hissa = mutationCTSNoDTL.shares_in_percent;
                    fetchData.parkingArea = mutationCTSNoDTL.parking_area_in_sq_m;
                    fetchData.parkingNo = mutationCTSNoDTL.parking_no;
                    fetchData.taraceArea = mutationCTSNoDTL.terrace_area_in_sq_m;
                    fetchData.unitNo = mutationCTSNoDTL.unit_no;
                    fetchData.unitType = mutationCTSNoDTL.unit_type;
                    fetchData.villageName = mutationCTSNoDTL.village_or_peth_name;
                    fetchData.villageCode = mutationCTSNoDTL.village_or_peth_code;
                    fetchData.flatBuiltUpArea = mutationCTSNoDTL.flat_built_up_area;
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
*/
        // Add Mutation CTS in created application
        public string SaveMutationCTSNoData(MutationCTSNoData mutationCTSNoData)
        {
            using (var scope = new TransactionScope())
            {
                try
                {
                    MutationCTSNoDTL fetchMutationCTSNoDTL = new MutationCTSNoDTL();
                    MethodForFileUpload methodForFileUpload = new MethodForFileUpload();

                    if (string.IsNullOrEmpty(mutationCTSNoData.nabhuNo) || string.IsNullOrEmpty(mutationCTSNoData.naBhu))
                    {
                        return "अर्जामधील नमूद न.भू.क्र. (अंकी भाग) / न.भू.क्र. क्रमांक feild is empty.";
                    }
                    if (string.IsNullOrEmpty(mutationCTSNoData.village!.village_name))
                    {
                        return "गाव / पेठ feild is empty";
                    }
                    if (methodForFileUpload.CheckNabhu(mutationCTSNoData.nabhuNo))
                    {
                        return "अर्जामधील नमूद न.भू.क्र. (अंकी भाग) field contains special characters.";
                    }

                    fetchMutationCTSNoDTL = _context.mutationCTSNoDTLs.Include(app => app.applicationDTL).Where(data => data.applicationDTL!.applicationid!.Equals(mutationCTSNoData.applicationid)
                    && data.village_or_peth_code != mutationCTSNoData.village!.village_code
                    && data.isDeleted == false
                    ).FirstOrDefault()!;
                    if (fetchMutationCTSNoDTL != null)
                    {
                        return "Could Not Add Another Village For The Same Application";
                    }

                    bool fetchMutationCTSNo = _context.mutationCTSNoDTLs.Include(app => app.applicationDTL).Any(data => data.applicationDTL!.applicationid!.Equals(mutationCTSNoData.applicationid)
                    && data.selected_city_servey_no == mutationCTSNoData.naBhu
                    && data.isDeleted == false)!;

                    if (fetchMutationCTSNo!)
                    {
                        return "Could Not Add Same CTS No For The Same Application";
                    }


                    // Assign Values to Model
                    MutationCTSNoDTLModel mutationCTSNoDTLModel = new MutationCTSNoDTLModel();
                    UserMaster userMaster = _context.userMasters.FirstOrDefault(s => s.userid == mutationCTSNoData.userid)!;
                    mutationCTSNoDTLModel.userMaster = userMaster;

                    ApplicationDTL applicationDTL = _context.applicationDTL.FirstOrDefault(s => s.applicationid == mutationCTSNoData.applicationid)!;
                    mutationCTSNoDTLModel.applicationDTL = applicationDTL;

                    if (mutationCTSNoData.inDast == "surveynoorgatno")
                    {
                        mutationCTSNoDTLModel.what_is_mentioned_in_the_doc = "SERVEYNO / GROUPNO";
                        mutationCTSNoDTLModel.servey_no = mutationCTSNoData.surveyNo;
                    }
                    if (mutationCTSNoData.inDast == "nabhu")
                    {
                        mutationCTSNoDTLModel.what_is_mentioned_in_the_doc = "CITY SERVEY NO";
                        mutationCTSNoDTLModel.city_servey_no_mentioned_in_application = mutationCTSNoData.nabhuNo;
                    }
                    mutationCTSNoDTLModel.village_or_peth_name = textInfo.ToTitleCase(mutationCTSNoData.village!.village_name!.Trim());
                    mutationCTSNoDTLModel.village_or_peth_code = mutationCTSNoData.village.village_code!.Trim();
                    mutationCTSNoDTLModel.village_lgd_code = mutationCTSNoData.village.village_lgd_code == null || mutationCTSNoData.village.village_lgd_code == "" ? "NA" : mutationCTSNoData.village.village_lgd_code;//string.IsNullOrEmpty(mutationCTSNoData.village.village_lgd_code!.Trim())?"NA":mutationCTSNoData.village.village_lgd_code;
                    mutationCTSNoDTLModel.village_english_name = mutationCTSNoData.village.village_english_name == null || mutationCTSNoData.village.village_english_name == "" ? "NA" : mutationCTSNoData.village.village_english_name;//string.IsNullOrEmpty(mutationCTSNoData.village.village_english_name!.Trim()) ? "NA" : mutationCTSNoData.village.village_english_name;
                    mutationCTSNoDTLModel.zone_code = mutationCTSNoData.village.zone_code == null || mutationCTSNoData.village.zone_code == "" ? "NA" : mutationCTSNoData.village.zone_code;//string.IsNullOrEmpty(mutationCTSNoData.village.zone_code!.Trim()) ? "NA" : mutationCTSNoData.village.zone_code;
                    mutationCTSNoDTLModel.amount = mutationCTSNoData.village.amount == null || mutationCTSNoData.village.amount == "" ? "NA" : mutationCTSNoData.village.amount;//string.IsNullOrEmpty(mutationCTSNoData.village.amount!.Trim()) ? "NA" : mutationCTSNoData.village.amount;
                    mutationCTSNoDTLModel.mutation_modification_type = mutationCTSNoData.milkat!.Trim().ToUpper();
                    mutationCTSNoDTLModel.selected_city_servey_no = mutationCTSNoData.naBhu;
                    mutationCTSNoDTLModel.lr_property_uid = mutationCTSNoData.lrPropertyUID;
                    mutationCTSNoDTLModel.application_income_type = mutationCTSNoData.namud!.Trim();
                    mutationCTSNoDTLModel.city_servey_area_in_sq_m = mutationCTSNoData.cityServeyAreaInSqm;
                    mutationCTSNoDTLModel.sub_property_no = mutationCTSNoData.subPropNo;
                    //mutationCTSNoDTLModel.buildup_area_in_sq_m = mutationCTSNoData.fetchedBuildupArea;

                    //if (mutationCTSNoData.milkat.Trim().ToUpper() == "FLAT")
                    //{
                    if (mutationCTSNoData.flatDetails != null && mutationCTSNoDTLModel.application_income_type.Trim().ToUpper() == "OTHER")
                    {

                        if (methodForFileUpload.CheckBuildingName(mutationCTSNoData.flatDetails!.buildingName!))
                        {
                            return "बिल्डिंगचे नाव field contains special characters. / Length is more that 300 characters.";
                        }
                        if (methodForFileUpload.checkFloorNo(mutationCTSNoData.flatDetails!.floorNo!))
                        {
                            return "मजला क्र. field contains special characters. / Please enter 4 digit मजला क्र.";
                        }
                        if (methodForFileUpload.checkUnitNo(mutationCTSNoData.flatDetails!.unitNo!))
                        {
                            return "युनिट क्र. field contains special characters. / Please enter 10 digit युनिट क्र.";
                        }
                        if (methodForFileUpload.CheckKArea(mutationCTSNoData.flatDetails!.buildupArea!))
                        {
                            return "बांधकाम क्षेत्र (चौ. मी.) field contains special characters. / Please enter 10 digit बांधकाम क्षेत्र (चौ. मी.)";
                        }
                        if (methodForFileUpload.CheckKArea(mutationCTSNoData.flatDetails!.carpetArea!))
                        {
                            return "कारपेट क्षेत्र (चौ. मी.) field contains special characters. / Please enter 10 digit कारपेट क्षेत्र (चौ. मी.)";
                        }
                        if (methodForFileUpload.CheckKArea(mutationCTSNoData.flatDetails!.taraceArea!))
                        {
                            return "टेरेस क्षेत्र (चौ. मी.) field contains special characters. / Please enter 10 digit टेरेस क्षेत्र (चौ. मी.)";
                        }
                        if (methodForFileUpload.CheckKArea(mutationCTSNoData.flatDetails!.parkingNo!))
                        {
                            return "पार्किंग क्र. field contains special characters. / Please enter 10 digit पार्किंग क्र.";
                        }
                        if (methodForFileUpload.CheckKArea(mutationCTSNoData.flatDetails!.parkingArea!))
                        {
                            return "पार्किंग क्षेत्र (चौ. मी.) field contains special characters. / Please enter 10 digit पार्किंग क्षेत्र (चौ. मी.)";
                        }
                        if (methodForFileUpload.CheckKArea(mutationCTSNoData.flatDetails!.hissa!))
                        {
                            return "हिस्सा (%) field contains special characters. / Please enter 10 digit हिस्सा (%)";
                        }

                        mutationCTSNoDTLModel.building_name = string.IsNullOrEmpty(mutationCTSNoData.flatDetails.buildingName) ? "NA" : mutationCTSNoData.flatDetails.buildingName.Trim();
                        mutationCTSNoDTLModel.floor_type = mutationCTSNoData.flatDetails.floorType!.floor_type;
                        mutationCTSNoDTLModel.floor_desc = string.IsNullOrEmpty(mutationCTSNoData.flatDetails.floorType!.floor_desc) ? "NA" : mutationCTSNoData.flatDetails.floorType.floor_desc.Trim();
                        mutationCTSNoDTLModel.floor_order_by = mutationCTSNoData.flatDetails.floorType.floor_order_by;
                        mutationCTSNoDTLModel.floor_no = string.IsNullOrEmpty(mutationCTSNoData.flatDetails.floorNo) ? "NA" : mutationCTSNoData.flatDetails.floorNo.Trim();
                        mutationCTSNoDTLModel.unit_code_156 = mutationCTSNoData.flatDetails.unitType!.unit_code_156;
                        mutationCTSNoDTLModel.unit_name_156 = string.IsNullOrEmpty(mutationCTSNoData.flatDetails.unitType!.unit_name_156) ? "NA" : mutationCTSNoData.flatDetails.unitType.unit_name_156.Trim();
                        mutationCTSNoDTLModel.unit_no = string.IsNullOrEmpty(mutationCTSNoData.flatDetails.unitNo) ? "NA" : mutationCTSNoData.flatDetails.unitNo.Trim();
                        mutationCTSNoDTLModel.carpet_area_in_sq_m = string.IsNullOrEmpty(mutationCTSNoData.flatDetails.carpetArea) ? "NA" : mutationCTSNoData.flatDetails.carpetArea.Trim();
                        mutationCTSNoDTLModel.terrace_area_in_sq_m = string.IsNullOrEmpty(mutationCTSNoData.flatDetails.taraceArea) ? "NA" : mutationCTSNoData.flatDetails.taraceArea.Trim();
                        mutationCTSNoDTLModel.parking_no = string.IsNullOrEmpty(mutationCTSNoData.flatDetails.parkingNo) ? "NA" : mutationCTSNoData.flatDetails.parkingNo.Trim();
                        mutationCTSNoDTLModel.parking_area_in_sq_m = string.IsNullOrEmpty(mutationCTSNoData.flatDetails.parkingArea) ? "NA" : mutationCTSNoData.flatDetails.parkingArea.Trim();
                        mutationCTSNoDTLModel.shares_in_percent = string.IsNullOrEmpty(mutationCTSNoData.flatDetails.hissa) ? "NA" : mutationCTSNoData.flatDetails.hissa.Trim();
                        mutationCTSNoDTLModel.buildup_area_in_sq_m = string.IsNullOrEmpty(mutationCTSNoData.flatDetails.buildupArea) ? "NA" : mutationCTSNoData.flatDetails.buildupArea.Trim();
                    }
                    if (mutationCTSNoDTLModel.application_income_type.Trim().ToUpper() != "OTHER")
                    {
                        mutationCTSNoDTLModel.nic_flat_details = mutationCTSNoData.nic_flat_details;
                        mutationCTSNoDTLModel.flat_bulit_up_area = mutationCTSNoData.flatBuiltUpArea;
                    }
                    if (mutationCTSNoDTLModel.mutation_modification_type.Trim().ToUpper() == "FLAT" || mutationCTSNoDTLModel.application_income_type.Trim().ToUpper() == "OTHER")
                    {
                        mutationCTSNoDTLModel.flat_bulit_up_area = mutationCTSNoData.flatBuiltUpArea;
                    }
                    //}
                    //else
                    //{
                    //    mutationCTSNoDTLModel.building_name = "NA";
                    //    mutationCTSNoDTLModel.floor_type = "NA";
                    //    mutationCTSNoDTLModel.floor_no = "NA";
                    //    mutationCTSNoDTLModel.unit_type = "NA";
                    //    mutationCTSNoDTLModel.unit_no = "NA";
                    //    mutationCTSNoDTLModel.buildup_area_in_sq_m = "NA";
                    //    mutationCTSNoDTLModel.carpet_area_in_sq_m = "NA";
                    //    mutationCTSNoDTLModel.terrace_area_in_sq_m = "NA";
                    //    mutationCTSNoDTLModel.parking_no = "NA";
                    //    mutationCTSNoDTLModel.parking_area_in_sq_m = "NA";
                    //    mutationCTSNoDTLModel.shares_in_percent = "NA";
                    //}

                    //Assign Data to Table fields to insert new records
                    MutationCTSNoDTL dbTable = new MutationCTSNoDTL();
                    //Set Default Values

                    dbTable.what_is_mentioned_in_the_doc = "NA";
                    dbTable.village_or_peth_code = "NA";
                    dbTable.village_or_peth_name = "NA";
                    dbTable.village_lgd_code = "NA";
                    dbTable.village_english_name = "NA";
                    dbTable.zone_code = "NA";
                    dbTable.amount = "NA";
                    dbTable.mutation_modification_type = "NA";
                    dbTable.city_servey_no_mentioned_in_application = "NA";
                    dbTable.servey_no = "NA";
                    dbTable.selected_city_servey_no = "NA";
                    dbTable.lr_property_uid = "NA";
                    dbTable.application_income_type = "NA";
                    dbTable.city_servey_area_in_sq_m = "NA";
                    dbTable.building_name = "NA";
                    dbTable.floor_type = 0;
                    dbTable.floor_desc = "NA";
                    dbTable.floor_order_by = 0;
                    dbTable.floor_no = "NA";
                    dbTable.unit_code_156 = 0;
                    dbTable.unit_name_156 = "NA";
                    dbTable.unit_no = "NA";
                    dbTable.buildup_area_in_sq_m = "NA";
                    dbTable.carpet_area_in_sq_m = "NA";
                    dbTable.terrace_area_in_sq_m = "NA";
                    dbTable.parking_area_in_sq_m = "NA";
                    dbTable.parking_no = "NA";
                    dbTable.shares_in_percent = "NA";
                    dbTable.nic_flat_details = "NA";
                    dbTable.flat_bulit_up_area = "NA";
                    dbTable.sub_property_id = "NA";

                    // Assign Values
                    dbTable.userMaster = mutationCTSNoDTLModel.userMaster;
                    dbTable.applicationDTL = mutationCTSNoDTLModel.applicationDTL;
                    dbTable.what_is_mentioned_in_the_doc = mutationCTSNoDTLModel.what_is_mentioned_in_the_doc;
                    dbTable.village_or_peth_code = mutationCTSNoDTLModel.village_or_peth_code;
                    dbTable.village_or_peth_name = mutationCTSNoDTLModel.village_or_peth_name;
                    dbTable.village_lgd_code = mutationCTSNoDTLModel.village_lgd_code;
                    dbTable.village_english_name = mutationCTSNoDTLModel.village_english_name;
                    dbTable.zone_code = mutationCTSNoDTLModel.zone_code;
                    dbTable.amount = mutationCTSNoDTLModel.amount;
                    dbTable.mutation_modification_type = mutationCTSNoDTLModel.mutation_modification_type;
                    dbTable.city_servey_no_mentioned_in_application = string.IsNullOrEmpty(mutationCTSNoDTLModel.city_servey_no_mentioned_in_application) ? "NA" : mutationCTSNoDTLModel.city_servey_no_mentioned_in_application;
                    dbTable.servey_no = string.IsNullOrEmpty(mutationCTSNoDTLModel.servey_no) ? "NA" : mutationCTSNoDTLModel.servey_no;
                    dbTable.selected_city_servey_no = mutationCTSNoDTLModel.selected_city_servey_no;
                    dbTable.lr_property_uid = mutationCTSNoDTLModel.lr_property_uid;
                    dbTable.application_income_type = mutationCTSNoDTLModel.application_income_type;
                    dbTable.city_servey_area_in_sq_m = mutationCTSNoDTLModel.city_servey_area_in_sq_m;
                    dbTable.sub_property_id = mutationCTSNoDTLModel.sub_property_no;

                    // Flat Details Entered By User
                    if (mutationCTSNoDTLModel.application_income_type.Trim().ToUpper() == "OTHER")
                    {
                        dbTable.building_name = string.IsNullOrEmpty(mutationCTSNoDTLModel.building_name!.Trim()) ? "NA" : mutationCTSNoDTLModel.building_name.Trim();
                        dbTable.floor_type = mutationCTSNoDTLModel.floor_type;
                        dbTable.floor_desc = string.IsNullOrEmpty(mutationCTSNoDTLModel.floor_desc!.Trim()) ? "NA" : mutationCTSNoDTLModel.floor_desc;
                        dbTable.floor_order_by = mutationCTSNoDTLModel.floor_order_by;
                        dbTable.floor_no = string.IsNullOrEmpty(mutationCTSNoDTLModel.floor_no) ? "NA" : mutationCTSNoDTLModel.floor_no;
                        dbTable.unit_code_156 = mutationCTSNoDTLModel.unit_code_156;
                        dbTable.unit_name_156 = string.IsNullOrEmpty(mutationCTSNoDTLModel.unit_name_156) ? "NA" : mutationCTSNoDTLModel.unit_name_156;
                        dbTable.unit_no = string.IsNullOrEmpty(mutationCTSNoDTLModel.unit_no) ? "NA" : mutationCTSNoDTLModel.unit_no;
                        dbTable.buildup_area_in_sq_m = string.IsNullOrEmpty(mutationCTSNoDTLModel.buildup_area_in_sq_m) ? "NA" : mutationCTSNoDTLModel.buildup_area_in_sq_m;
                        dbTable.carpet_area_in_sq_m = string.IsNullOrEmpty(mutationCTSNoDTLModel.carpet_area_in_sq_m) ? "NA" : mutationCTSNoDTLModel.carpet_area_in_sq_m;
                        dbTable.terrace_area_in_sq_m = string.IsNullOrEmpty(mutationCTSNoDTLModel.terrace_area_in_sq_m) ? "NA" : mutationCTSNoDTLModel.terrace_area_in_sq_m;
                        dbTable.parking_area_in_sq_m = string.IsNullOrEmpty(mutationCTSNoDTLModel.parking_area_in_sq_m) ? "NA" : mutationCTSNoDTLModel.parking_area_in_sq_m;
                        dbTable.parking_no = string.IsNullOrEmpty(mutationCTSNoDTLModel.parking_no) ? "NA" : mutationCTSNoDTLModel.parking_no;
                        dbTable.shares_in_percent = string.IsNullOrEmpty(mutationCTSNoDTLModel.shares_in_percent) ? "NA" : mutationCTSNoDTLModel.shares_in_percent;
                        dbTable.flat_bulit_up_area = string.IsNullOrEmpty(mutationCTSNoDTLModel.flat_bulit_up_area) ? "NA" : mutationCTSNoDTLModel.flat_bulit_up_area;
                    }
                    // End
                    else
                    {
                        dbTable.nic_flat_details = string.IsNullOrEmpty(mutationCTSNoDTLModel.nic_flat_details) ? "NA" : mutationCTSNoDTLModel.nic_flat_details;
                        dbTable.flat_bulit_up_area = string.IsNullOrEmpty(mutationCTSNoDTLModel.flat_bulit_up_area) ? "NA" : mutationCTSNoDTLModel.flat_bulit_up_area;
                    }

                    _context.mutationCTSNoDTLs.Add(dbTable);
                    _context.SaveChanges();

                    //Get Saved Row ID
                    int mutation_cts_no_id = dbTable.mutation_cts_no_id;
                    var applicationDTLdata = _context.applicationDTL.Where(data => data.applicationid!.Equals(mutationCTSNoData.applicationid)).FirstOrDefault();
                    if (applicationDTLdata != null)
                    {
                        if (!string.IsNullOrEmpty(applicationDTLdata.mutation_cts_nos) && !applicationDTLdata.mutation_cts_nos.Contains(mutation_cts_no_id.ToString()))
                        {
                            applicationDTLdata.mutation_cts_nos = applicationDTLdata.mutation_cts_nos + "," + mutation_cts_no_id.ToString();
                        }
                        else
                        {
                            applicationDTLdata.mutation_cts_nos = mutation_cts_no_id.ToString();
                        }
                        applicationDTLdata.status = 3;
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

        public FetchMutationCTSNoData FetchMutationCTSData(int mutationCTSNOId)
        {
            try
            {
                MutationCTSNoDTL mutationCTSNoDTL = new MutationCTSNoDTL();
                mutationCTSNoDTL = _contextR.mutationCTSNoDTLs.Include(app => app.applicationDTL).Where(data => data.mutation_cts_no_id.Equals(mutationCTSNOId) && data.isDeleted == false).FirstOrDefault()!;
                FetchMutationCTSNoData fetchData = new FetchMutationCTSNoData();
                if (mutationCTSNoDTL != null)
                {
                    fetchData.mutation_cts_no_id = mutationCTSNoDTL.mutation_cts_no_id;
                    fetchData.applicationid = mutationCTSNoDTL.applicationDTL!.applicationid;
                    fetchData.inDast = mutationCTSNoDTL.what_is_mentioned_in_the_doc;
                    fetchData.milkat = mutationCTSNoDTL.mutation_modification_type;
                    fetchData.nabhuNo = mutationCTSNoDTL.city_servey_no_mentioned_in_application;
                    fetchData.surveyNo = mutationCTSNoDTL.servey_no;
                    fetchData.naBhu = mutationCTSNoDTL.selected_city_servey_no;
                    fetchData.cityServeyAreaInSqm = mutationCTSNoDTL.city_servey_area_in_sq_m;
                    fetchData.lrPropertyUID = mutationCTSNoDTL.lr_property_uid;
                    fetchData.namud = mutationCTSNoDTL.application_income_type;
                    fetchData.floorNo = mutationCTSNoDTL.floor_no;
                    fetchData.floorType = mutationCTSNoDTL.floor_type;
                    fetchData.floorDesc = mutationCTSNoDTL.floor_desc;
                    fetchData.floor_order_by = mutationCTSNoDTL.floor_order_by;
                    fetchData.buildingName = mutationCTSNoDTL.building_name;
                    fetchData.buildupArea = mutationCTSNoDTL.buildup_area_in_sq_m;
                    fetchData.carpetArea = mutationCTSNoDTL.carpet_area_in_sq_m;
                    fetchData.hissa = mutationCTSNoDTL.shares_in_percent;
                    fetchData.parkingArea = mutationCTSNoDTL.parking_area_in_sq_m;
                    fetchData.parkingNo = mutationCTSNoDTL.parking_no;
                    fetchData.taraceArea = mutationCTSNoDTL.terrace_area_in_sq_m;
                    fetchData.unitNo = mutationCTSNoDTL.unit_no;
                    fetchData.unit_code_156 = mutationCTSNoDTL.unit_code_156;
                    fetchData.unit_name_156 = mutationCTSNoDTL.unit_name_156;
                    fetchData.villageName = mutationCTSNoDTL.village_or_peth_name;
                    fetchData.villageCode = mutationCTSNoDTL.village_or_peth_code;
                    fetchData.village_english_name = mutationCTSNoDTL.village_english_name;
                    fetchData.villageLGDCode = mutationCTSNoDTL.village_lgd_code;
                    fetchData.zone_code = mutationCTSNoDTL.zone_code;
                    fetchData.amount = mutationCTSNoDTL.amount;
                    fetchData.flatBuiltUpArea = mutationCTSNoDTL.flat_bulit_up_area;
                    fetchData.subPropNo = mutationCTSNoDTL.sub_property_id;
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

        // Add Dast Info in created application
        public string SaveDastInformation(DastInformationData dastInformationData)
        {
            using (var scope = new TransactionScope())
            {
                try
                {
                    // Assign Values to Model
                    DastInformationModel dastInformationModel = new DastInformationModel();
                    UserMaster userMaster = _context.userMasters.FirstOrDefault(s => s.userid == dastInformationData.userid)!;
                    dastInformationModel.userMaster = userMaster;
                    DastInformation dbTable = new DastInformation();

                    //check duplicate main dast
                    bool isTrue = _context.dastInformation.Include(s => s.applicationDTL).
                    Any(x => x.applicationDTL!.applicationid == dastInformationData.applicationid && x.isDeleted == false
                    && dastInformationData.dastType!.ToLower() == "maindast");

                    if (string.IsNullOrEmpty(dastInformationData.dastNo))
                    {
                        return "Dast No Can Not Be Empty";
                    }
                    if (!commonFunctions.IsEnglishNumber(dastInformationData.dastNo))
                    {
                        //return "Dast No Should Not Be Marathi Number";
                        return "नोंदणीकृत दस्त क्रमांक field contains special characters / Marathi numbers / English letters";
                    }
                    if (string.IsNullOrEmpty(dastInformationData.dastNoYear))
                    {
                        return "वर्ष Can Not Be Empty";
                    }
                    if (dastInformationData.dastNoYear.Length != 4)
                    {
                        return "वर्ष field Length Should Be 4 Digit";
                    }
                    if (!commonFunctions.IsEnglishNumber(dastInformationData.dastNoYear))
                    {
                        //return "Dast Year Should Not Be Marathi Number";
                        return "वर्ष  field contains special characters / Marathi numbers / English letters";
                    }
                    if (string.IsNullOrEmpty(dastInformationData.dastNoDate))
                    {
                        return "Dast Date Can Not Be Empty";
                    }
                    if (string.IsNullOrEmpty(dastInformationData.dastNabhu))
                    {
                        //return "Dast Nabhu No Should Not Be Empty";
                        return "दस्तामध्ये नमूद केलेले न.भू.क्र. field should not be empty";
                    }
                    //if (!commonFunctions.CheckDastNabhuNo(dastInformationData.dastNabhu))
                    //{
                    //    return "Invalid दस्तामध्ये नमूद केलेले न.भू.क्र.";
                    //}
                    if (dastInformationData.dastType!.ToLower() == "errorcorrect")
                    {
                        if (string.IsNullOrEmpty(dastInformationData.remarks))
                        {
                            return "Dast Remark Should Not Be Empty";
                        }
                        if (!commonFunctions.CheckDastRemark(dastInformationData.remarks))
                        {
                            return "Invalid Dast Remark... Dast Remark Will Accept Only English No, Dot(.), Comma(,), English And Marathi Words And Space.";
                        }
                    }

                    if (!isTrue)
                    {
                        ApplicationDTL applicationDTL = _context.applicationDTL.FirstOrDefault(s => s.applicationid == dastInformationData.applicationid)!;
                        dastInformationModel.applicationDTL = applicationDTL;

                        dastInformationModel.dastType = dastInformationData.dastType!.Trim();
                        dastInformationModel.digcode = dastInformationData.division!.digcode;
                        dastInformationModel.dig_name = dastInformationData.division.dig;
                        dastInformationModel.districtCode = dastInformationData.district!.jdrcode.ToString();
                        dastInformationModel.districtName = dastInformationData.district.jdr!.Trim();
                        // dastInformationModel.districtNameInEnglish = dastInformationData.district.district_english_name!.Trim();
                        dastInformationModel.office_of_the_second_registrar_code = dastInformationData.registrar!.srocode.ToString();
                        dastInformationModel.office_of_the_second_registrar_name = dastInformationData.registrar.sro!.Trim();
                        dastInformationModel.registered_dast_no = dastInformationData.dastNo;
                        dastInformationModel.registered_dast_date = dastInformationData.dastNoDate;
                        dastInformationModel.registered_dast_year = dastInformationData.dastNoYear;
                        dastInformationModel.dastNabhu = dastInformationData.dastNabhu;
                        dastInformationModel.remarks = dastInformationData.remarks!.Trim();
                        dastInformationModel.isDastVerified = dastInformationData.isDastVarified;
                        dastInformationModel.verifiedDastData = dastInformationData.verifiedDastData;


                        //Assign Data to Table fields to insert new records
                        //DastInformation dbTable = new DastInformation();
                        dbTable.userMaster = dastInformationModel.userMaster;
                        dbTable.applicationDTL = dastInformationModel.applicationDTL;
                        dbTable.dastType = dastInformationModel.dastType;
                        dbTable.division_code = dastInformationModel.digcode;
                        dbTable.division_name = dastInformationModel.dig_name;
                        dbTable.districtCode = dastInformationModel.districtCode;
                        dbTable.districtName = dastInformationModel.districtName;
                        // dbTable.districtNameInEnglish = dastInformationModel.districtNameInEnglish;
                        dbTable.office_of_the_second_registrar_code = dastInformationModel.office_of_the_second_registrar_code;
                        dbTable.office_of_the_second_registrar_name = dastInformationModel.office_of_the_second_registrar_name;
                        dbTable.registered_dast_no = dastInformationModel.registered_dast_no;
                        dbTable.registered_dast_date = dastInformationModel.registered_dast_date;
                        dbTable.registered_dast_year = dastInformationModel.registered_dast_year;
                        dbTable.dastNabhu = dastInformationModel.dastNabhu;
                        dbTable.remarks = dastInformationModel.remarks;
                        dbTable.isDastVerified = dastInformationModel.isDastVerified;
                        dbTable.verifiedDastData = dastInformationModel.verifiedDastData;
                        _context.dastInformation.Add(dbTable);
                        _context.SaveChanges();

                        //Get Saved Row ID
                        int dastID = dbTable.dast_id;
                        var applicationDTLdata = _context.applicationDTL.Where(data => data.applicationid!.Equals(dastInformationData.applicationid)).FirstOrDefault();
                        if (applicationDTLdata != null)
                        {
                            if (!string.IsNullOrEmpty(applicationDTLdata.dastIDs) && !applicationDTLdata.dastIDs.Contains(dastID.ToString()))
                            {
                                applicationDTLdata.dastIDs = applicationDTLdata.dastIDs + "," + dastID.ToString();
                            }
                            else
                            {
                                applicationDTLdata.dastIDs = dastID.ToString();
                            }
                            applicationDTLdata.status = 4;
                            _context.Entry(applicationDTLdata).CurrentValues.SetValues(applicationDTLdata);
                            _context.SaveChanges();
                        }
                        scope.Complete();
                        return "Success";
                    }
                    else
                    {
                        return "You can add only one main dast for one application id";
                    }


                }
                catch (Exception ex)
                {
                    throw new HandleException(ex.Message.ToString());
                }
            }
        }

        public FetchDastInformationData FetchDastInformationData(int dastNo)
        {
            try
            {
                DastInformation dastInformation = new DastInformation();
                dastInformation = _contextR.dastInformation.Include(i => i.userMaster).Include(app => app.applicationDTL).Where(data => data.dast_id.Equals(dastNo) && data.isDeleted == false).FirstOrDefault()!;

                FetchDastInformationData fetchData = new FetchDastInformationData();
                if (dastInformation != null)
                {
                    fetchData.dast_id = dastInformation.dast_id;
                    fetchData.userid = dastInformation.userMaster!.userid;
                    fetchData.applicationid = dastInformation.applicationDTL!.applicationid;
                    fetchData.dastNabhu = dastInformation.dastNabhu;
                    fetchData.dastNo = dastInformation.registered_dast_no;
                    fetchData.dastNoDate = dastInformation.registered_dast_date;
                    fetchData.dastNoYear = dastInformation.registered_dast_year;
                    fetchData.dastType = dastInformation.dastType;
                    fetchData.divisionCode = dastInformation.division_code.ToString();
                    fetchData.divisionName = dastInformation.division_name;
                    fetchData.districtCode = dastInformation.districtCode;
                    fetchData.districtName = dastInformation.districtName;
                    //fetchData.districtNameInEnglish = dastInformation.districtName;
                    fetchData.registrarCode = dastInformation.office_of_the_second_registrar_code;
                    fetchData.registrarName = dastInformation.office_of_the_second_registrar_name;
                    fetchData.remarks = dastInformation.remarks;
                    fetchData.isDastVerified = dastInformation.isDastVerified;
                    fetchData.verifiedDastData = dastInformation.verifiedDastData;
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

        // Add Court Claim in created application
        public string SaveCourtClaimInformation(CourtClaimInformationData courtClaimInformationData)
        {
            using (var scope = new TransactionScope())
            {
                try
                {
                    MethodForFileUpload methodforfileupload = new MethodForFileUpload();
                    if (string.IsNullOrEmpty(courtClaimInformationData.nabhu))
                    {
                        return "Please select न.भू.क्र. क्रमांक";
                    }
                    if (string.IsNullOrEmpty(courtClaimInformationData.caseDawa!.caseNolabel))
                    {
                        return "Please select केस क्रमांक/रे.मु.नं.(XXXX99YY)";
                    }
                    if (string.IsNullOrEmpty(courtClaimInformationData.caseType!.caseTypeLabel))
                    {
                        return "Please select केस प्रकार";
                    }
                    if (methodforfileupload.CheckCourtDavaTapshil(courtClaimInformationData.orderDetails!))
                    {
                        return "आदेशाचा तपशील field contains special characters.";
                    }
                    if (methodforfileupload.ContainsSpecialCharactersInCourtCase(courtClaimInformationData.orderDetails!))

                    {
                        return "आदेशाचा तपशील field contains special characters.";
                    }

                    // Assign Values to Model
                    CourtClaimInformationModel courtClaimInformationModel = new CourtClaimInformationModel();
                    UserMaster userMaster = _context.userMasters.FirstOrDefault(s => s.userid == courtClaimInformationData.userid)!;
                    courtClaimInformationModel.userMaster = userMaster;

                    ApplicationDTL applicationDTL = _context.applicationDTL.FirstOrDefault(s => s.applicationid == courtClaimInformationData.applicationid)!;
                    courtClaimInformationModel.applicationDTL = applicationDTL;

                    courtClaimInformationModel.court_case_code = courtClaimInformationData.caseDawa!.caseNoCode;
                    courtClaimInformationModel.court_case_name = courtClaimInformationData.caseDawa.caseNolabel;
                    courtClaimInformationModel.court_case_type_code = courtClaimInformationData.caseType!.caseTypeCode;
                    courtClaimInformationModel.court_case_type_name = courtClaimInformationData.caseType.caseTypeLabel;
                    courtClaimInformationModel.lr_property_uid = courtClaimInformationData.lrPropertyUID;
                    courtClaimInformationModel.city_servey_no = courtClaimInformationData.nabhu;
                    courtClaimInformationModel.order_details = courtClaimInformationData.orderDetails;
                    courtClaimInformationModel.stay_order = courtClaimInformationData.stayOrder;
                    courtClaimInformationModel.sub_property_no = courtClaimInformationData.subPropNo;

                    //Assign Data to Table fields to insert new records
                    CourtClaimInformation dbTable = new CourtClaimInformation();
                    dbTable.userMaster = courtClaimInformationModel.userMaster;
                    dbTable.applicationDTL = courtClaimInformationModel.applicationDTL;
                    dbTable.court_case_code = courtClaimInformationModel.court_case_code;
                    dbTable.court_case_name = courtClaimInformationModel.court_case_name;
                    dbTable.court_case_type_code = courtClaimInformationModel.court_case_type_code;
                    dbTable.court_case_type_name = courtClaimInformationModel.court_case_type_name;
                    dbTable.lr_property_uid = courtClaimInformationModel.lr_property_uid;
                    dbTable.city_servey_no = courtClaimInformationModel.city_servey_no;
                    dbTable.order_details = courtClaimInformationModel.order_details;
                    dbTable.stay_order = courtClaimInformationModel.stay_order!.Trim().ToUpper();
                    dbTable.sub_property_no = courtClaimInformationModel.sub_property_no;
                    _context.courtClaimInformation.Add(dbTable);
                    _context.SaveChanges();

                    //Get Saved Row ID
                    int courtClaimID = dbTable.court_claim_id;
                    var applicationDTLdata = _context.applicationDTL.Where(data => data.applicationid!.Equals(courtClaimInformationData.applicationid)).FirstOrDefault();
                    if (applicationDTLdata != null)
                    {
                        if (!string.IsNullOrEmpty(applicationDTLdata.courtClaimIDs) && !applicationDTLdata.courtClaimIDs.Contains(courtClaimID.ToString()))
                        {
                            applicationDTLdata.courtClaimIDs = applicationDTLdata.courtClaimIDs + "," + courtClaimID.ToString();
                        }
                        else
                        {
                            applicationDTLdata.courtClaimIDs = courtClaimID.ToString();
                        }
                        applicationDTLdata.status = 7;
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

        public FetchCourtClaimInformationData FetchCourtClaimInformation(int courtClaimID)
        {
            try
            {
                CourtClaimInformation courtClaimInformation = new CourtClaimInformation();
                courtClaimInformation = _contextR.courtClaimInformation.Include(i => i.userMaster).Include(app => app.applicationDTL).Where(data => data.court_claim_id.Equals(courtClaimID) && data.isDeleted == false).FirstOrDefault()!;

                FetchCourtClaimInformationData fetchData = new();
                if (courtClaimInformation != null)
                {

                    fetchData.court_claim_id = courtClaimInformation.court_claim_id;
                    fetchData.userid = courtClaimInformation.userMaster!.userid;
                    fetchData.applicationid = courtClaimInformation.applicationDTL!.applicationid;

                    ViewModel.CaseDawaData caseDawa = new();
                    caseDawa.caseNoCode = courtClaimInformation.court_case_code;
                    caseDawa.caseNolabel = courtClaimInformation.court_case_name;
                    fetchData.caseDawa = caseDawa;


#pragma warning disable IDE0017 // Simplify object initialization
                    ViewModel.CaseType caseType = new ViewModel.CaseType();
#pragma warning restore IDE0017 // Simplify object initialization
                    caseType.caseTypeCode = courtClaimInformation.court_case_type_code;
                    caseType.caseTypeLabel = courtClaimInformation.court_case_type_name;
                    fetchData.caseType = caseType;

                    fetchData.lrPropertyUID = courtClaimInformation.lr_property_uid;
                    fetchData.nabhu = courtClaimInformation.city_servey_no;
                    fetchData.orderDetails = courtClaimInformation.order_details;
                    fetchData.stayOrder = courtClaimInformation.stay_order;
                    fetchData.subPropNo = courtClaimInformation.sub_property_no;
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

        //Add Power of Attorney For Giver
        //public string SavePowerOfAttorneyGiver(PowerOfAttorneyInformationDataForGiver powerOfAttorneyInformationData)
        //{
        //    _logger.LogInformation("Save POA Giver - " + powerOfAttorneyInformationData);
        //    MethodForFileUpload methodForFile = new MethodForFileUpload();
        //    PowerOfAttorneyInformation dbTable = new PowerOfAttorneyInformation();
        //    string FolderPath = @"\\10.10.248.2\pde_propertycard\MUTATIONDOCS\" + powerOfAttorneyInformationData.applicationid + @"\POWEROFATTORNEY\GIVER\";
        //    //string FolderPath = @"\\10.10.248.2\pde_propertycard\MUTATIONDOCS\" + powerOfAttorneyInformationData.applicationid + @"\POWEROFATTORNEY\GIVER\";
        //    using (var scope = new TransactionScope(TransactionScopeOption.Suppress))
        //    {
        //        try
        //        {
        //            // Assign Values to Model
        //            PowerOfAttorneyInformationModel powerOfAttorneyInformationModel = new PowerOfAttorneyInformationModel();
        //            //Assign the values
        //            PropertyTypeMaster proptype = _contextR.propertyTypes.FirstOrDefault(s => s.propertytypeid == Convert.ToInt32("0"))!;
        //            powerOfAttorneyInformationModel.PropertyTypeMaster = proptype;
        //            powerOfAttorneyInformationModel.mobileno = "NA";
        //            powerOfAttorneyInformationModel.mobilenoverified = "NA";
        //            powerOfAttorneyInformationModel.emailid = "NA";
        //            powerOfAttorneyInformationModel.emailidverified = "NA";
        //            powerOfAttorneyInformationModel.usertype = "NA";
        //            powerOfAttorneyInformationModel.usertype_code = 0;
        //            powerOfAttorneyInformationModel.cts_number = "NA";
        //            powerOfAttorneyInformationModel.owner_number = "NA";
        //            powerOfAttorneyInformationModel.mutation_srno = "NA";
        //            powerOfAttorneyInformationModel.village_code = "NA";
        //            powerOfAttorneyInformationModel.village_name = "NA";
        //            powerOfAttorneyInformationModel.prefix_in_eng = "NA";
        //            powerOfAttorneyInformationModel.fname_in_eng = "NA";
        //            powerOfAttorneyInformationModel.mname_in_eng = "NA";
        //            powerOfAttorneyInformationModel.lname_in_eng = "NA";
        //            powerOfAttorneyInformationModel.prefix_in_marathi = "NA";
        //            powerOfAttorneyInformationModel.fname_in_marathi = "NA";
        //            powerOfAttorneyInformationModel.mname_in_marathi = "NA";
        //            powerOfAttorneyInformationModel.lname_in_marathi = "NA";
        //            //New Fields For Taker
        //            powerOfAttorneyInformationModel.company_name_in_marathi = "NA";
        //            powerOfAttorneyInformationModel.company_name_in_eng = "NA";
        //            //End
        //            powerOfAttorneyInformationModel.username = "NA";
        //            //New Fields for Taker
        //            powerOfAttorneyInformationModel.alias_name = "NA";
        //            powerOfAttorneyInformationModel.gender_code = "NA";
        //            powerOfAttorneyInformationModel.gender_description = "NA";
        //            powerOfAttorneyInformationModel.dob = "NA";
        //            powerOfAttorneyInformationModel.mother_name_in_marathi = "NA";
        //            powerOfAttorneyInformationModel.mother_name_in_eng = "NA";
        //            powerOfAttorneyInformationModel.attornytype_desc = "NA";
        //            powerOfAttorneyInformationModel.attornytype_code = 0;

        //            powerOfAttorneyInformationModel.landBuyArea = "NA";
        //            powerOfAttorneyInformationModel.address_type = "NA";
        //            powerOfAttorneyInformationModel.address = "NA";
        //            powerOfAttorneyInformationModel.state = "NA";
        //            powerOfAttorneyInformationModel.district = "NA";
        //            powerOfAttorneyInformationModel.taluka = "NA";
        //            powerOfAttorneyInformationModel.city = "NA";
        //            powerOfAttorneyInformationModel.flatno_plotno = "NA";
        //            powerOfAttorneyInformationModel.societyname = "NA";
        //            powerOfAttorneyInformationModel.mainstreet = "NA";
        //            powerOfAttorneyInformationModel.landmark = "NA";
        //            powerOfAttorneyInformationModel.locality = "NA";
        //            powerOfAttorneyInformationModel.pincode = "NA";
        //            powerOfAttorneyInformationModel.postofficename = "NA";
        //            powerOfAttorneyInformationModel.address_proof_document_name = "NA";
        //            powerOfAttorneyInformationModel.address_proof_document_path = "NA";

        //            powerOfAttorneyInformationModel.owner_of_property_in_maharashtra = false;
        //            // powerOfAttorneyInformationModel.PropertyTypeMaster = 0;
        //            powerOfAttorneyInformationModel.property_district_code = "NA";
        //            powerOfAttorneyInformationModel.property_district_name_in_marathi = "NA";
        //            powerOfAttorneyInformationModel.property_district_name_in_english = "NA";
        //            powerOfAttorneyInformationModel.property_taluka_code = "NA";
        //            powerOfAttorneyInformationModel.property_taluka_name = "NA";
        //            powerOfAttorneyInformationModel.property_city_code = "NA";
        //            powerOfAttorneyInformationModel.property_city_name = "NA";
        //            powerOfAttorneyInformationModel.khateno = "NA";
        //            powerOfAttorneyInformationModel.ulpin = "NA";
        //            powerOfAttorneyInformationModel.city_servey_no = "NA";
        //            powerOfAttorneyInformationModel.lr_property_id = "NA";
        //            powerOfAttorneyInformationModel.profile_pic_file_name = "NA";
        //            powerOfAttorneyInformationModel.profile_pic_file_path = "NA";
        //            powerOfAttorneyInformationModel.isPOAisPartofDast = "NA";
        //            powerOfAttorneyInformationModel.isDeclerationInvolvedInPOA = "NA";
        //            powerOfAttorneyInformationModel.isPOAPermanant = "NA";
        //            powerOfAttorneyInformationModel.isTransferRights = "NA";
        //            powerOfAttorneyInformationModel.dastNo = "NA";
        //            powerOfAttorneyInformationModel.dastNoDate = "NA";
        //            powerOfAttorneyInformationModel.dastNoYear = "NA";
        //            powerOfAttorneyInformationModel.isDastVerified = false;
        //            powerOfAttorneyInformationModel.verifiedDastData = "NA";
        //            powerOfAttorneyInformationModel.digcode = 0;
        //            powerOfAttorneyInformationModel.digname = "NA";
        //            powerOfAttorneyInformationModel.poa_district_code = "0";
        //            powerOfAttorneyInformationModel.poa_district_name = "NA";
        //            powerOfAttorneyInformationModel.sro_office_code = 0;
        //            powerOfAttorneyInformationModel.sro_office_name = "NA";
        //            powerOfAttorneyInformationModel.signed_file_path = "NA";
        //            powerOfAttorneyInformationModel.signed_file_name = "NA";
        //            powerOfAttorneyInformationModel.prefixcode_marathi = "0";
        //            powerOfAttorneyInformationModel.prefixcode_eng = "0";
        //            powerOfAttorneyInformationModel.sub_property_id = "999999";

        //            UserMaster userMaster;
        //            using (var transaction = _contextR.Database.BeginTransaction(System.Data.IsolationLevel.ReadCommitted)) // Changed here
        //            {
        //                userMaster = _contextR.userMasters.FirstOrDefault(s => s.userid == powerOfAttorneyInformationData.userid)!;
        //                powerOfAttorneyInformationModel.userMaster = userMaster;
        //                transaction.Commit();
        //            }
        //            ApplicationDTL applicationDTL;
        //            using (var transaction = _contextR.Database.BeginTransaction(System.Data.IsolationLevel.ReadCommitted)) // Changed here
        //            {
        //                applicationDTL = _contextR.applicationDTL.FirstOrDefault(s => s.applicationid == powerOfAttorneyInformationData.applicationid)!;
        //                powerOfAttorneyInformationModel.applicationDTL = applicationDTL;
        //                transaction.Commit();
        //            }

        //            powerOfAttorneyInformationModel.usertype_code = powerOfAttorneyInformationData.userDetails!.usertype_code;
        //            powerOfAttorneyInformationModel.usertype = powerOfAttorneyInformationData.userDetails.usertype;
        //            // Condition for User Type
        //            if (powerOfAttorneyInformationData.userDetails!.usertype_code == 1 || powerOfAttorneyInformationData.userDetails!.usertype_code == 0)
        //            {
        //                powerOfAttorneyInformationModel.prefixcode_eng = powerOfAttorneyInformationData.userDetails!.suffixCodeEng;
        //                powerOfAttorneyInformationModel.prefixcode_marathi = powerOfAttorneyInformationData.userDetails.suffixcode;
        //                powerOfAttorneyInformationModel.prefix_in_eng = powerOfAttorneyInformationData.userDetails!.suffixEng;
        //                powerOfAttorneyInformationModel.fname_in_eng = powerOfAttorneyInformationData.userDetails.firstNameEng;
        //                powerOfAttorneyInformationModel.mname_in_eng = powerOfAttorneyInformationData.userDetails.middleNameEng;
        //                powerOfAttorneyInformationModel.lname_in_eng = powerOfAttorneyInformationData.userDetails.lastNameEng;
        //                powerOfAttorneyInformationModel.prefix_in_marathi = powerOfAttorneyInformationData.userDetails.suffix;
        //                powerOfAttorneyInformationModel.fname_in_marathi = powerOfAttorneyInformationData.userDetails.firstName;
        //                powerOfAttorneyInformationModel.mname_in_marathi = powerOfAttorneyInformationData.userDetails.middleName;
        //                powerOfAttorneyInformationModel.lname_in_marathi = powerOfAttorneyInformationData.userDetails.lastName;
        //                powerOfAttorneyInformationModel.company_name_in_marathi = "NA";
        //                powerOfAttorneyInformationModel.company_name_in_eng = "NA";
        //            }
        //            else
        //            {
        //                powerOfAttorneyInformationModel.company_name_in_marathi = powerOfAttorneyInformationData.userDetails.company_name_in_marathi;
        //                powerOfAttorneyInformationModel.company_name_in_eng = powerOfAttorneyInformationData.userDetails.company_name_in_eng;
        //                powerOfAttorneyInformationModel.prefix_in_marathi = "NA";
        //                powerOfAttorneyInformationModel.fname_in_marathi = "NA";
        //                powerOfAttorneyInformationModel.mname_in_marathi = "NA";
        //                powerOfAttorneyInformationModel.lname_in_marathi = "NA";
        //                powerOfAttorneyInformationModel.prefix_in_eng = "NA";
        //                powerOfAttorneyInformationModel.fname_in_eng = "NA";
        //                powerOfAttorneyInformationModel.mname_in_eng = "NA";
        //                powerOfAttorneyInformationModel.lname_in_eng = "NA";
        //            }
        //            //End

        //            powerOfAttorneyInformationModel.address_type = powerOfAttorneyInformationData.address!.addressType!.Trim().ToUpper();
        //            if (powerOfAttorneyInformationModel.address_type == "INDIA")
        //            {
        //                powerOfAttorneyInformationModel.mobileno = powerOfAttorneyInformationData.address.indiaAddress!.mobile;
        //                powerOfAttorneyInformationModel.mobilenoverified = powerOfAttorneyInformationData.address.indiaAddress.mobileOTP;
        //                //powerOfAttorneyInformationModel.mobileno = "NA";
        //                //powerOfAttorneyInformationModel.mobilenoverified = "NA";
        //                powerOfAttorneyInformationModel.emailid = "NA";
        //                powerOfAttorneyInformationModel.emailidverified = "NA";

        //                powerOfAttorneyInformationModel.address = "NA";
        //                powerOfAttorneyInformationModel.state = powerOfAttorneyInformationData.address.indiaAddress!.state;
        //                powerOfAttorneyInformationModel.district = powerOfAttorneyInformationData.address.indiaAddress.district;
        //                powerOfAttorneyInformationModel.taluka = powerOfAttorneyInformationData.address.indiaAddress.taluka;
        //                powerOfAttorneyInformationModel.city = powerOfAttorneyInformationData.address.indiaAddress.city;
        //                powerOfAttorneyInformationModel.flatno_plotno = powerOfAttorneyInformationData.address.indiaAddress.plotNo;
        //                powerOfAttorneyInformationModel.societyname = powerOfAttorneyInformationData.address.indiaAddress.building;
        //                powerOfAttorneyInformationModel.mainstreet = powerOfAttorneyInformationData.address.indiaAddress.mainRoad;
        //                powerOfAttorneyInformationModel.landmark = powerOfAttorneyInformationData.address.indiaAddress.impSymbol;
        //                powerOfAttorneyInformationModel.locality = powerOfAttorneyInformationData.address.indiaAddress.area;
        //                powerOfAttorneyInformationModel.pincode = powerOfAttorneyInformationData.address.indiaAddress.pincode;
        //                powerOfAttorneyInformationModel.postofficename = powerOfAttorneyInformationData.address.indiaAddress.postOfficeName;
        //                //powerOfAttorneyInformationModel.address_proof_document_name = powerOfAttorneyInformationData.address.indiaAddress.addressProofName;
        //                //powerOfAttorneyInformationModel.signed_file_name = powerOfAttorneyInformationData.address.indiaAddress.signatureName;
        //            }
        //            else if (powerOfAttorneyInformationModel.address_type == "FOREIGN")
        //            {
        //                powerOfAttorneyInformationModel.address = powerOfAttorneyInformationData.address.foreignAddress!.address;
        //                powerOfAttorneyInformationModel.mobileno = powerOfAttorneyInformationData.address.foreignAddress.mobile;
        //                powerOfAttorneyInformationModel.emailid = powerOfAttorneyInformationData.address.foreignAddress.email;
        //                powerOfAttorneyInformationModel.emailidverified = powerOfAttorneyInformationData.address.foreignAddress.emailOTP;
        //                //powerOfAttorneyInformationModel.signed_file_name = powerOfAttorneyInformationData.address.foreignAddress.signatureName;

        //                powerOfAttorneyInformationModel.state = "NA";
        //                powerOfAttorneyInformationModel.district = "NA";
        //                powerOfAttorneyInformationModel.taluka = "NA";
        //                powerOfAttorneyInformationModel.city = "NA";
        //                powerOfAttorneyInformationModel.flatno_plotno = "NA";
        //                powerOfAttorneyInformationModel.societyname = "NA";
        //                powerOfAttorneyInformationModel.mainstreet = "NA";
        //                powerOfAttorneyInformationModel.landmark = "NA";
        //                powerOfAttorneyInformationModel.locality = "NA";
        //                powerOfAttorneyInformationModel.pincode = "NA";
        //                powerOfAttorneyInformationModel.postofficename = "NA";
        //                //powerOfAttorneyInformationModel.address_proof_document_name = "NA";
        //                //powerOfAttorneyInformationModel.address_proof_document_path = "NA";
        //                powerOfAttorneyInformationModel.signed_file_name = powerOfAttorneyInformationData.address.foreignAddress.signatureName;
        //            }

        //            powerOfAttorneyInformationModel.username = powerOfAttorneyInformationData.userDetails.userName;
        //            powerOfAttorneyInformationModel.city_servey_no = powerOfAttorneyInformationData.userDetails.nabhu;
        //            powerOfAttorneyInformationModel.lr_property_id = powerOfAttorneyInformationData.userDetails.lrPropertyUID;
        //            powerOfAttorneyInformationModel.mutation_id = powerOfAttorneyInformationData.mutation_id;
        //            powerOfAttorneyInformationModel.sub_property_id = powerOfAttorneyInformationData.userDetails.subPropNo;
        //            powerOfAttorneyInformationModel.cts_number = powerOfAttorneyInformationData.ctsNo;
        //            powerOfAttorneyInformationModel.owner_number = powerOfAttorneyInformationData.ownerNo;
        //            powerOfAttorneyInformationModel.mutation_srno = powerOfAttorneyInformationData.mutationSroNo;
        //            powerOfAttorneyInformationModel.village_code = powerOfAttorneyInformationData.village_code;
        //            powerOfAttorneyInformationModel.village_name = powerOfAttorneyInformationData.village_name;

        //            /*powerOfAttorneyInformationModel.attornytype_code = powerOfAttorneyInformationData.attornytype!.poa_type_code;
        //            powerOfAttorneyInformationModel.attornytype_desc = powerOfAttorneyInformationData.attornytype.poa_type_description;
        //            powerOfAttorneyInformationModel.dastNo = powerOfAttorneyInformationData.dastNo!;
        //            powerOfAttorneyInformationModel.dastNoYear = powerOfAttorneyInformationData.dastNoYear!;
        //            powerOfAttorneyInformationModel.dastNoDate = powerOfAttorneyInformationData.dastNoDate!;
        //            powerOfAttorneyInformationModel.isDastVerified = powerOfAttorneyInformationData.isDastVarified;
        //            powerOfAttorneyInformationModel.verifiedDastData = powerOfAttorneyInformationData.verifiedDastData!;
        //            powerOfAttorneyInformationModel.isPOAisPartofDast = powerOfAttorneyInformationData.isPOAisPartofDast!;
        //            powerOfAttorneyInformationModel.isDeclerationInvolvedInPOA = powerOfAttorneyInformationData.isDeclerationInvolvedInPOA!;
        //            powerOfAttorneyInformationModel.isPOAPermanant = powerOfAttorneyInformationData.isPOAPermanant!;
        //            powerOfAttorneyInformationModel.isTransferRights = powerOfAttorneyInformationData.isTransferRights!;
        //            powerOfAttorneyInformationModel.digcode = powerOfAttorneyInformationData.division!.digcode!;
        //            powerOfAttorneyInformationModel.digname = powerOfAttorneyInformationData.division.dig!;
        //            powerOfAttorneyInformationModel.poa_district_code = powerOfAttorneyInformationData.district!.jdrcode.ToString()!;
        //            powerOfAttorneyInformationModel.poa_district_name = powerOfAttorneyInformationData.district.jdr!;
        //            powerOfAttorneyInformationModel.sro_office_code = Convert.ToInt32(powerOfAttorneyInformationData.registrar!.srocode);
        //            powerOfAttorneyInformationModel.sro_office_name = powerOfAttorneyInformationData.registrar.sro!;
        //            powerOfAttorneyInformationModel.mutation_id = powerOfAttorneyInformationData.mutation_id;*/

        //            //Assign Default value
        //            dbTable.mobileno = "NA";
        //            dbTable.mobilenoverified = "NA";
        //            dbTable.emailid = "NA";
        //            dbTable.emailidverified = "NA";
        //            dbTable.usertype = "NA";
        //            dbTable.usertype_code = 0;
        //            dbTable.prefixcode_eng = "0";
        //            dbTable.prefixcode_marathi = "0";
        //            dbTable.sub_property_no = "999999";
        //            dbTable.prefix_in_eng = "NA";
        //            dbTable.fname_in_eng = "NA";
        //            dbTable.mname_in_eng = "NA";
        //            dbTable.lname_in_eng = "NA";
        //            dbTable.prefix_in_marathi = "NA";
        //            dbTable.fname_in_marathi = "NA";
        //            dbTable.mname_in_marathi = "NA";
        //            dbTable.lname_in_marathi = "NA";
        //            //New Fields For Taker
        //            dbTable.company_name_in_marathi = "NA";
        //            dbTable.company_name_in_eng = "NA";
        //            //End
        //            dbTable.username = "NA";
        //            //New Fields for Taker
        //            dbTable.alias_name = "NA";
        //            dbTable.gender_code = "NA";
        //            dbTable.gender_description = "NA";
        //            dbTable.dob = "NA";
        //            dbTable.mother_name_in_marathi = "NA";
        //            dbTable.mother_name_in_eng = "NA";
        //            dbTable.attornytype_desc = "NA";
        //            dbTable.attornytype_code = 0;

        //            dbTable.landBuyArea = "NA";
        //            dbTable.address_type = "NA";
        //            dbTable.address = "NA";
        //            dbTable.state = "NA";
        //            dbTable.district = "NA";
        //            dbTable.taluka = "NA";
        //            dbTable.city = "NA";
        //            dbTable.flatno_plotno = "NA";
        //            dbTable.societyname = "NA";
        //            dbTable.mainstreet = "NA";
        //            dbTable.landmark = "NA";
        //            dbTable.locality = "NA";
        //            dbTable.pincode = "NA";
        //            dbTable.postofficename = "NA";
        //            dbTable.address_proof_document_name = "NA";
        //            dbTable.address_proof_document_path = "NA";

        //            dbTable.owner_of_property_in_maharashtra = false;
        //            //dbTable.PropertyTypeMaster = 0;
        //            dbTable.property_district_code = "NA";
        //            dbTable.property_district_name_in_marathi = "NA";
        //            dbTable.property_district_name_in_english = "NA";
        //            dbTable.property_taluka_code = "NA";
        //            dbTable.property_taluka_name = "NA";
        //            dbTable.property_city_code = "NA";
        //            dbTable.property_city_name = "NA";
        //            dbTable.khateno = "NA";
        //            dbTable.ulpin = "NA";
        //            dbTable.city_servey_no = "NA";
        //            dbTable.lr_property_id = "NA";
        //            dbTable.profile_pic_file_name = "NA";
        //            dbTable.profile_pic_file_path = "NA";
        //            dbTable.isPOAisPartofDast = "NA";
        //            dbTable.isDeclerationInvolvedInPOA = "NA";
        //            dbTable.isPOAPermanant = "NA";
        //            dbTable.isTransferRights = "NA";
        //            dbTable.dast_no = "NA";
        //            dbTable.dast_no_date = "NA";
        //            dbTable.dast_no_year = "NA";
        //            dbTable.isDastVerified = false;
        //            dbTable.verifieddastData = "NA";
        //            dbTable.digcode = 0;
        //            dbTable.digname = "NA";
        //            dbTable.poa_district_code = "0";
        //            dbTable.poa_district_name = "NA";
        //            dbTable.sro_office_code = 0;
        //            dbTable.sro_office_name = "NA";
        //            dbTable.signed_file_path = "NA";
        //            dbTable.signed_file_name = "NA";

        //            //Assign Data to Table fields to insert new records
        //            dbTable.userMaster = powerOfAttorneyInformationModel.userMaster;
        //            dbTable.mutation_id = Convert.ToInt32(powerOfAttorneyInformationModel.mutation_id!);
        //            dbTable.applicationDTL = powerOfAttorneyInformationModel.applicationDTL;
        //            dbTable.mobileno = powerOfAttorneyInformationModel.mobileno!;
        //            dbTable.mobilenoverified = string.IsNullOrEmpty(powerOfAttorneyInformationModel.mobilenoverified) ? "NO" : powerOfAttorneyInformationModel.mobilenoverified;
        //            dbTable.emailid = powerOfAttorneyInformationModel.emailid!;
        //            dbTable.emailidverified = powerOfAttorneyInformationModel.emailidverified!;
        //            dbTable.usertype_code = powerOfAttorneyInformationModel.usertype_code;
        //            dbTable.usertype = string.IsNullOrEmpty(powerOfAttorneyInformationModel!.usertype) ? "NA" : powerOfAttorneyInformationModel!.usertype;
        //            dbTable.prefixcode_marathi = powerOfAttorneyInformationModel.prefixcode_marathi == "" || powerOfAttorneyInformationModel.prefixcode_marathi == null ? "0" : powerOfAttorneyInformationModel.prefixcode_marathi;
        //            dbTable.prefixcode_eng = powerOfAttorneyInformationModel.prefixcode_eng == "" || powerOfAttorneyInformationModel.prefixcode_eng == null ? "0" : powerOfAttorneyInformationModel.prefixcode_eng;
        //            dbTable.prefix_in_eng = string.IsNullOrEmpty(powerOfAttorneyInformationModel.prefix_in_eng) ? "NA" : powerOfAttorneyInformationModel.prefix_in_eng;
        //            dbTable.fname_in_eng = string.IsNullOrEmpty(powerOfAttorneyInformationModel.fname_in_eng) ? "NA" : textInfo.ToTitleCase(powerOfAttorneyInformationModel.fname_in_eng.Trim());
        //            dbTable.mname_in_eng = string.IsNullOrEmpty(powerOfAttorneyInformationModel.mname_in_eng) ? "NA" : textInfo.ToTitleCase(powerOfAttorneyInformationModel.mname_in_eng.Trim());
        //            dbTable.lname_in_eng = string.IsNullOrEmpty(powerOfAttorneyInformationModel.lname_in_eng) ? "NA" : textInfo.ToTitleCase(powerOfAttorneyInformationModel.lname_in_eng.Trim());
        //            dbTable.prefix_in_marathi = string.IsNullOrEmpty(powerOfAttorneyInformationModel.prefix_in_marathi) ? "NA" : powerOfAttorneyInformationModel.prefix_in_marathi.Trim();
        //            dbTable.fname_in_marathi = string.IsNullOrEmpty(powerOfAttorneyInformationModel.fname_in_marathi) ? "NA" : powerOfAttorneyInformationModel.fname_in_marathi.Trim();
        //            dbTable.mname_in_marathi = string.IsNullOrEmpty(powerOfAttorneyInformationModel.mname_in_marathi) ? "NA" : powerOfAttorneyInformationModel.mname_in_marathi.Trim();
        //            dbTable.lname_in_marathi = string.IsNullOrEmpty(powerOfAttorneyInformationModel.lname_in_marathi) ? "NA" : powerOfAttorneyInformationModel.lname_in_marathi.Trim();
        //            dbTable.company_name_in_eng = string.IsNullOrEmpty(powerOfAttorneyInformationModel.company_name_in_eng) ? "NA" : powerOfAttorneyInformationModel.company_name_in_eng;
        //            dbTable.company_name_in_marathi = string.IsNullOrEmpty(powerOfAttorneyInformationModel.company_name_in_marathi) ? "NA" : powerOfAttorneyInformationModel.company_name_in_marathi;
        //            dbTable.username = powerOfAttorneyInformationModel.username!;
        //            dbTable.city_servey_no = powerOfAttorneyInformationModel.city_servey_no!;
        //            dbTable.lr_property_id = powerOfAttorneyInformationModel.lr_property_id!;
        //            dbTable.sub_property_no = powerOfAttorneyInformationModel.sub_property_id!;
        //            dbTable.cts_number = powerOfAttorneyInformationModel.cts_number;
        //            dbTable.mutation_srno = powerOfAttorneyInformationModel.mutation_srno;
        //            dbTable.owner_number = powerOfAttorneyInformationModel.owner_number;
        //            dbTable.village_code = powerOfAttorneyInformationModel.village_code;
        //            dbTable.village_name = powerOfAttorneyInformationModel.village_name;
        //            dbTable.address_type = powerOfAttorneyInformationModel.address_type;
        //            dbTable.address = powerOfAttorneyInformationModel.address!;
        //            dbTable.state = powerOfAttorneyInformationModel.state!;
        //            dbTable.district = powerOfAttorneyInformationModel.district!;
        //            dbTable.taluka = powerOfAttorneyInformationModel.taluka!;
        //            dbTable.city = powerOfAttorneyInformationModel.city!;
        //            dbTable.flatno_plotno = powerOfAttorneyInformationModel.flatno_plotno!;
        //            dbTable.societyname = powerOfAttorneyInformationModel.societyname!;
        //            dbTable.mainstreet = powerOfAttorneyInformationModel.mainstreet!;
        //            dbTable.landmark = powerOfAttorneyInformationModel.landmark!;
        //            dbTable.locality = powerOfAttorneyInformationModel.locality!;
        //            dbTable.pincode = powerOfAttorneyInformationModel.pincode!;
        //            dbTable.postofficename = powerOfAttorneyInformationModel.postofficename!;
        //            //dbTable.address_proof_document_name = powerOfAttorneyInformationModel.address_proof_document_name;
        //            //dbTable.signed_file_name = powerOfAttorneyInformationModel.signed_file_name;
        //            dbTable.address_proof_document_name = "NA";
        //            dbTable.address_proof_document_path = "NA";
        //            dbTable.signed_file_name = "NA";
        //            dbTable.signed_file_path = "NA";
        //            dbTable.attornytype_code = 0;
        //            dbTable.attornytype_desc = "NA";
        //            dbTable.is_taker = false;
        //            dbTable.alias_name = "NA";
        //            dbTable.gender_code = "NA";
        //            dbTable.gender_description = "NA";
        //            dbTable.khata_type_code = "NA";
        //            dbTable.khata_type_name = "NA";

        //            dbTable.owner_status_code = "NA";
        //            dbTable.owner_status_description = "NA";

        //            dbTable.dob = "NA";
        //            dbTable.mother_name_in_marathi = "NA";
        //            dbTable.mother_name_in_eng = "NA";

        //            /* dbTable.apk_code = 0;
        //             dbTable.apk_description = "NA";

        //             dbTable.aapak = "NA";*/
        //            dbTable.landBuyArea = "NA";
        //            dbTable.profile_pic_file_name = "NA";
        //            dbTable.profile_pic_file_path = "NA";
        //            dbTable.power_of_attorney_code = GeneratePowerOfAttorneyCode(powerOfAttorneyInformationData.applicationid!, false);
        //            _logger.LogInformation("Generate POA Code - " + dbTable.power_of_attorney_code);
        //            _context.powerOfAttorneyInformation.Add(dbTable);
        //            _context.SaveChanges();

        //            //Get Saved Row ID
        //            int powerOfAttorneyID = dbTable.power_of_attorney_id;

        //            string CurrentDateTime = DateTime.Now.ToString("yyyy-MMM-dd-HHmmss");
        //            bool checkAddressFlag = true;
        //            //bool checkSignFlag = false;
        //            if (powerOfAttorneyInformationModel.address_type == "INDIA")
        //            {
        //                if (!string.IsNullOrEmpty(powerOfAttorneyInformationData.address.indiaAddress!.addressProofSrc) && !string.IsNullOrEmpty(powerOfAttorneyInformationData.address.indiaAddress.addressProofName) && powerOfAttorneyInformationData.address.indiaAddress!.addressProofSrc != "NA" && powerOfAttorneyInformationData.address.indiaAddress!.addressProofName != "NA")
        //                {
        //                    string[] AddressData = powerOfAttorneyInformationData.address.indiaAddress.addressProofSrc.Split(",");
        //                    checkAddressFlag = methodForFile.SaveImageForApplicant(AddressData[1], powerOfAttorneyInformationData.address.indiaAddress.addressProofName, powerOfAttorneyID.ToString(), "AddressProof", FolderPath + @"\", CurrentDateTime);
        //                    string AddressProofExt = Path.GetExtension(powerOfAttorneyInformationData.address.indiaAddress.addressProofName);
        //                    powerOfAttorneyInformationModel.address_proof_document_name = "AddressProof" + powerOfAttorneyID + "_" + CurrentDateTime + AddressProofExt;
        //                    powerOfAttorneyInformationModel.address_proof_document_path = FolderPath + powerOfAttorneyID + @"\" + powerOfAttorneyInformationModel.address_proof_document_name;

        //                    var UpdateAddressFilePath = _context.powerOfAttorneyInformation.Where(w => w.power_of_attorney_id == powerOfAttorneyID).FirstOrDefault();
        //                    if (UpdateAddressFilePath != null)
        //                    {
        //                        dbTable.address_proof_document_name = powerOfAttorneyInformationModel.address_proof_document_name;
        //                        dbTable.address_proof_document_path = powerOfAttorneyInformationModel.address_proof_document_path;
        //                        _context.Entry(dbTable).CurrentValues.SetValues(dbTable);
        //                        _context.SaveChanges();
        //                    }
        //                }

        //                //string[] signData = powerOfAttorneyInformationData.address.indiaAddress.signatureSrc.Split(",");
        //                //checkSignFlag = methodForFile.SaveImageForApplicant(signData[1], powerOfAttorneyInformationData.address.indiaAddress.signatureName, powerOfAttorneyID.ToString(), "Signature", FolderPath + @"\", CurrentDateTime);
        //                //string SignatureExt = Path.GetExtension(powerOfAttorneyInformationData.address.indiaAddress.signatureName);
        //                //powerOfAttorneyInformationModel.signed_file_name = "Signature" + powerOfAttorneyID + "_" + CurrentDateTime + SignatureExt;
        //                //powerOfAttorneyInformationModel.signed_file_path = FolderPath + @"\" + powerOfAttorneyID + @"\" + powerOfAttorneyInformationModel.signed_file_name;
        //                //var UpdateSignFilePath = _context.powerOfAttorneyInformation.Where(w => w.power_of_attorney_id == powerOfAttorneyID).FirstOrDefault();
        //                //if (UpdateSignFilePath != null)
        //                //{
        //                //    dbTable.signed_file_name = powerOfAttorneyInformationModel.signed_file_name;
        //                //    dbTable.signed_file_path = powerOfAttorneyInformationModel.signed_file_path;
        //                //    _context.Entry(dbTable).CurrentValues.SetValues(dbTable);
        //                //    _context.SaveChanges();
        //                //}
        //            }
        //            //if (powerOfAttorneyInformationModel.address_type == "FOREIGN")
        //            //{
        //            //    string[] signData = powerOfAttorneyInformationData.address.foreignAddress.signatureSrc.Split(",");
        //            //    checkSignFlag = methodForFile.SaveImageForApplicant(signData[1], powerOfAttorneyInformationData.address.foreignAddress.signatureName, powerOfAttorneyID.ToString(), "Signature", FolderPath + @"\", CurrentDateTime);

        //            //    string SignatureExt = Path.GetExtension(powerOfAttorneyInformationData.address.foreignAddress.signatureName);
        //            //    powerOfAttorneyInformationModel.signed_file_name = "Signature" + powerOfAttorneyID + "_" + CurrentDateTime + SignatureExt;
        //            //    powerOfAttorneyInformationModel.signed_file_path = FolderPath + @"\" + powerOfAttorneyID + @"\" + powerOfAttorneyInformationModel.signed_file_name;
        //            //    var UpdateSignFilePath = _context.powerOfAttorneyInformation.Where(w => w.power_of_attorney_id == powerOfAttorneyID).FirstOrDefault();
        //            //    if (UpdateSignFilePath != null)
        //            //    {
        //            //        dbTable.signed_file_name = powerOfAttorneyInformationModel.signed_file_name;
        //            //        dbTable.signed_file_path = powerOfAttorneyInformationModel.signed_file_path;
        //            //        _context.Entry(dbTable).CurrentValues.SetValues(dbTable);
        //            //        _context.SaveChanges();
        //            //    }
        //            //}
        //            if (checkAddressFlag)
        //            {
        //                scope.Complete();
        //                using (var scope1 = new TransactionScope(TransactionScopeOption.Suppress))
        //                {
        //                    var applicationDTLdata = _context.applicationDTL.AsNoTracking().Where(data => data.applicationid!.Equals(powerOfAttorneyInformationData.applicationid)).FirstOrDefault();
        //                    if (applicationDTLdata != null)
        //                    {
        //                        if (!string.IsNullOrEmpty(applicationDTLdata.powerOfAttorneyIDs) && !applicationDTLdata.powerOfAttorneyIDs.Contains(powerOfAttorneyID.ToString()))
        //                        {
        //                            applicationDTLdata.powerOfAttorneyIDs = applicationDTLdata.powerOfAttorneyIDs + "," + powerOfAttorneyID.ToString();
        //                        }
        //                        else
        //                        {
        //                            applicationDTLdata.powerOfAttorneyIDs = powerOfAttorneyID.ToString();
        //                        }
        //                        _context.Entry(applicationDTLdata).CurrentValues.SetValues(applicationDTLdata);
        //                        _context.SaveChanges();
        //                        scope1.Complete();
        //                    }
        //                }
        //                return "Success" + "," + powerOfAttorneyID;
        //            }
        //            else
        //            {
        //                _context.powerOfAttorneyInformation.Remove(dbTable);
        //                _context.SaveChanges();
        //                return "Address Proof File Is Not Uploaded";
        //            }
        //            //if (!checkAddressFlag)
        //            //{
        //            //    _context.powerOfAttorneyInformation.Remove(dbTable);
        //            //    _context.SaveChanges();
        //            //    return "Address Proof File Is Not Uploaded";
        //            //}
        //            //if (!checkSignFlag)
        //            //{
        //            //    _context.powerOfAttorneyInformation.Remove(dbTable);
        //            //    _context.SaveChanges();
        //            //    return "Signature File Is Not Uploaded";
        //            //}
        //            //else
        //            //{
        //            //    _context.powerOfAttorneyInformation.Remove(dbTable);
        //            //    _context.SaveChanges();
        //            //    return "Some Files Are Not Uploaded";
        //            //}
        //        }
        //        catch (Exception ex)
        //        {
        //            _context.powerOfAttorneyInformation.Remove(dbTable);
        //            _context.SaveChanges();
        //            throw new HandleException(ex.Message.ToString());
        //        }
        //    }
        //}

        public string SavePowerOfAttorneyGiver(PowerOfAttorneyInformationDataForGiver powerOfAttorneyInformationData)
        {
            MethodForFileUpload methodForFile = new MethodForFileUpload();
            PowerOfAttorneyInformation dbTable = new PowerOfAttorneyInformation();
            string FolderPath = @"\\10.10.248.2\pde_propertycard\MUTATIONDOCS\" + powerOfAttorneyInformationData.applicationid + @"\POWEROFATTORNEY\GIVER\";
            using (var scope = new TransactionScope())
            {
                try
                {
                    // Assign Values to Model
                    PowerOfAttorneyInformationModel powerOfAttorneyInformationModel = new PowerOfAttorneyInformationModel();
                    //Assign the values
                    PropertyTypeMaster proptype = _context.propertyTypes.FirstOrDefault(s => s.propertytypeid == Convert.ToInt32("0"))!;
                    powerOfAttorneyInformationModel.PropertyTypeMaster = proptype;
                    powerOfAttorneyInformationModel.mobileno = "NA";
                    powerOfAttorneyInformationModel.mobilenoverified = "NA";
                    powerOfAttorneyInformationModel.emailid = "NA";
                    powerOfAttorneyInformationModel.emailidverified = "NA";
                    powerOfAttorneyInformationModel.usertype = "NA";
                    powerOfAttorneyInformationModel.usertype_code = 0;
                    powerOfAttorneyInformationModel.cts_number = "NA";
                    powerOfAttorneyInformationModel.owner_number = "NA";
                    powerOfAttorneyInformationModel.mutation_srno = "NA";
                    powerOfAttorneyInformationModel.village_code = "NA";
                    powerOfAttorneyInformationModel.village_name = "NA";
                    powerOfAttorneyInformationModel.prefix_in_eng = "NA";
                    powerOfAttorneyInformationModel.fname_in_eng = "NA";
                    powerOfAttorneyInformationModel.mname_in_eng = "NA";
                    powerOfAttorneyInformationModel.lname_in_eng = "NA";
                    powerOfAttorneyInformationModel.prefix_in_marathi = "NA";
                    powerOfAttorneyInformationModel.fname_in_marathi = "NA";
                    powerOfAttorneyInformationModel.mname_in_marathi = "NA";
                    powerOfAttorneyInformationModel.lname_in_marathi = "NA";
                    //New Fields For Taker
                    powerOfAttorneyInformationModel.company_name_in_marathi = "NA";
                    powerOfAttorneyInformationModel.company_name_in_eng = "NA";
                    //End
                    powerOfAttorneyInformationModel.username = "NA";
                    //New Fields for Taker
                    powerOfAttorneyInformationModel.alias_name = "NA";
                    powerOfAttorneyInformationModel.gender_code = "NA";
                    powerOfAttorneyInformationModel.gender_description = "NA";
                    powerOfAttorneyInformationModel.dob = "NA";
                    powerOfAttorneyInformationModel.mother_name_in_marathi = "NA";
                    powerOfAttorneyInformationModel.mother_name_in_eng = "NA";
                    powerOfAttorneyInformationModel.attornytype_desc = "NA";
                    powerOfAttorneyInformationModel.attornytype_code = 0;

                    powerOfAttorneyInformationModel.landBuyArea = "NA";
                    powerOfAttorneyInformationModel.address_type = "NA";
                    powerOfAttorneyInformationModel.address = "NA";
                    powerOfAttorneyInformationModel.state = "NA";
                    powerOfAttorneyInformationModel.district = "NA";
                    powerOfAttorneyInformationModel.taluka = "NA";
                    powerOfAttorneyInformationModel.city = "NA";
                    powerOfAttorneyInformationModel.flatno_plotno = "NA";
                    powerOfAttorneyInformationModel.societyname = "NA";
                    powerOfAttorneyInformationModel.mainstreet = "NA";
                    powerOfAttorneyInformationModel.landmark = "NA";
                    powerOfAttorneyInformationModel.locality = "NA";
                    powerOfAttorneyInformationModel.pincode = "NA";
                    powerOfAttorneyInformationModel.postofficename = "NA";
                    powerOfAttorneyInformationModel.address_proof_document_name = "NA";
                    powerOfAttorneyInformationModel.address_proof_document_path = "NA";

                    powerOfAttorneyInformationModel.owner_of_property_in_maharashtra = false;
                    // powerOfAttorneyInformationModel.PropertyTypeMaster = 0;
                    powerOfAttorneyInformationModel.property_district_code = "NA";
                    powerOfAttorneyInformationModel.property_district_name_in_marathi = "NA";
                    powerOfAttorneyInformationModel.property_district_name_in_english = "NA";
                    powerOfAttorneyInformationModel.property_taluka_code = "NA";
                    powerOfAttorneyInformationModel.property_taluka_name = "NA";
                    powerOfAttorneyInformationModel.property_city_code = "NA";
                    powerOfAttorneyInformationModel.property_city_name = "NA";
                    powerOfAttorneyInformationModel.khateno = "NA";
                    powerOfAttorneyInformationModel.ulpin = "NA";
                    powerOfAttorneyInformationModel.city_servey_no = "NA";
                    powerOfAttorneyInformationModel.lr_property_id = "NA";
                    powerOfAttorneyInformationModel.profile_pic_file_name = "NA";
                    powerOfAttorneyInformationModel.profile_pic_file_path = "NA";
                    powerOfAttorneyInformationModel.isPOAisPartofDast = "NA";
                    powerOfAttorneyInformationModel.isDeclerationInvolvedInPOA = "NA";
                    powerOfAttorneyInformationModel.isPOAPermanant = "NA";
                    powerOfAttorneyInformationModel.isTransferRights = "NA";
                    powerOfAttorneyInformationModel.dastNo = "NA";
                    powerOfAttorneyInformationModel.dastNoDate = "NA";
                    powerOfAttorneyInformationModel.dastNoYear = "NA";
                    powerOfAttorneyInformationModel.isDastVerified = false;
                    powerOfAttorneyInformationModel.verifiedDastData = "NA";
                    powerOfAttorneyInformationModel.digcode = 0;
                    powerOfAttorneyInformationModel.digname = "NA";
                    powerOfAttorneyInformationModel.poa_district_code = "0";
                    powerOfAttorneyInformationModel.poa_district_name = "NA";
                    powerOfAttorneyInformationModel.sro_office_code = 0;
                    powerOfAttorneyInformationModel.sro_office_name = "NA";
                    powerOfAttorneyInformationModel.signed_file_path = "NA";
                    powerOfAttorneyInformationModel.signed_file_name = "NA";
                    powerOfAttorneyInformationModel.prefixcode_marathi = "0";
                    powerOfAttorneyInformationModel.prefixcode_eng = "0";
                    powerOfAttorneyInformationModel.sub_property_id = "999999";

                    UserMaster userMaster = _context.userMasters.FirstOrDefault(s => s.userid == powerOfAttorneyInformationData.userid)!;
                    powerOfAttorneyInformationModel.userMaster = userMaster;

                    ApplicationDTL applicationDTL = _context.applicationDTL.FirstOrDefault(s => s.applicationid == powerOfAttorneyInformationData.applicationid)!;
                    powerOfAttorneyInformationModel.applicationDTL = applicationDTL;

                    powerOfAttorneyInformationModel.usertype_code = powerOfAttorneyInformationData.userDetails.usertype_code;
                    powerOfAttorneyInformationModel.usertype = powerOfAttorneyInformationData.userDetails.usertype;
                    // Condition for User Type
                    if (powerOfAttorneyInformationData.userDetails!.usertype_code == 1 || powerOfAttorneyInformationData.userDetails!.usertype_code == 0)
                    {
                        powerOfAttorneyInformationModel.prefixcode_eng = powerOfAttorneyInformationData.userDetails!.suffixCodeEng;
                        powerOfAttorneyInformationModel.prefixcode_marathi = powerOfAttorneyInformationData.userDetails.suffixcode;
                        powerOfAttorneyInformationModel.prefix_in_eng = powerOfAttorneyInformationData.userDetails!.suffixEng;
                        powerOfAttorneyInformationModel.fname_in_eng = powerOfAttorneyInformationData.userDetails.firstNameEng;
                        powerOfAttorneyInformationModel.mname_in_eng = powerOfAttorneyInformationData.userDetails.middleNameEng;
                        powerOfAttorneyInformationModel.lname_in_eng = powerOfAttorneyInformationData.userDetails.lastNameEng;
                        powerOfAttorneyInformationModel.prefix_in_marathi = powerOfAttorneyInformationData.userDetails.suffix;
                        powerOfAttorneyInformationModel.fname_in_marathi = powerOfAttorneyInformationData.userDetails.firstName;
                        powerOfAttorneyInformationModel.mname_in_marathi = powerOfAttorneyInformationData.userDetails.middleName;
                        powerOfAttorneyInformationModel.lname_in_marathi = powerOfAttorneyInformationData.userDetails.lastName;
                        powerOfAttorneyInformationModel.company_name_in_marathi = "NA";
                        powerOfAttorneyInformationModel.company_name_in_eng = "NA";
                    }
                    else
                    {
                        powerOfAttorneyInformationModel.company_name_in_marathi = powerOfAttorneyInformationData.userDetails.company_name_in_marathi;
                        powerOfAttorneyInformationModel.company_name_in_eng = powerOfAttorneyInformationData.userDetails.company_name_in_eng;
                        powerOfAttorneyInformationModel.prefix_in_marathi = "NA";
                        powerOfAttorneyInformationModel.fname_in_marathi = "NA";
                        powerOfAttorneyInformationModel.mname_in_marathi = "NA";
                        powerOfAttorneyInformationModel.lname_in_marathi = "NA";
                        powerOfAttorneyInformationModel.prefix_in_eng = "NA";
                        powerOfAttorneyInformationModel.fname_in_eng = "NA";
                        powerOfAttorneyInformationModel.mname_in_eng = "NA";
                        powerOfAttorneyInformationModel.lname_in_eng = "NA";
                    }
                    //End

                    powerOfAttorneyInformationModel.address_type = powerOfAttorneyInformationData.address!.addressType!.Trim().ToUpper();
                    if (powerOfAttorneyInformationModel.address_type == "INDIA")
                    {
                        powerOfAttorneyInformationModel.mobileno = powerOfAttorneyInformationData.address.indiaAddress!.mobile;
                        powerOfAttorneyInformationModel.mobilenoverified = powerOfAttorneyInformationData.address.indiaAddress.mobileOTP;
                        //powerOfAttorneyInformationModel.mobileno = "NA";
                        //powerOfAttorneyInformationModel.mobilenoverified = "NA";
                        powerOfAttorneyInformationModel.emailid = "NA";
                        powerOfAttorneyInformationModel.emailidverified = "NA";

                        powerOfAttorneyInformationModel.address = "NA";
                        powerOfAttorneyInformationModel.state = powerOfAttorneyInformationData.address.indiaAddress!.state;
                        powerOfAttorneyInformationModel.district = powerOfAttorneyInformationData.address.indiaAddress.district;
                        powerOfAttorneyInformationModel.taluka = powerOfAttorneyInformationData.address.indiaAddress.taluka;
                        powerOfAttorneyInformationModel.city = powerOfAttorneyInformationData.address.indiaAddress.city;
                        powerOfAttorneyInformationModel.flatno_plotno = powerOfAttorneyInformationData.address.indiaAddress.plotNo;
                        powerOfAttorneyInformationModel.societyname = powerOfAttorneyInformationData.address.indiaAddress.building;
                        powerOfAttorneyInformationModel.mainstreet = powerOfAttorneyInformationData.address.indiaAddress.mainRoad;
                        powerOfAttorneyInformationModel.landmark = powerOfAttorneyInformationData.address.indiaAddress.impSymbol;
                        powerOfAttorneyInformationModel.locality = powerOfAttorneyInformationData.address.indiaAddress.area;
                        powerOfAttorneyInformationModel.pincode = powerOfAttorneyInformationData.address.indiaAddress.pincode;
                        powerOfAttorneyInformationModel.postofficename = powerOfAttorneyInformationData.address.indiaAddress.postOfficeName;
                        //powerOfAttorneyInformationModel.address_proof_document_name = powerOfAttorneyInformationData.address.indiaAddress.addressProofName;
                        //powerOfAttorneyInformationModel.signed_file_name = powerOfAttorneyInformationData.address.indiaAddress.signatureName;
                    }
                    else if (powerOfAttorneyInformationModel.address_type == "FOREIGN")
                    {
                        powerOfAttorneyInformationModel.address = powerOfAttorneyInformationData.address.foreignAddress!.address;
                        powerOfAttorneyInformationModel.mobileno = powerOfAttorneyInformationData.address.foreignAddress.mobile;
                        powerOfAttorneyInformationModel.emailid = powerOfAttorneyInformationData.address.foreignAddress.email;
                        powerOfAttorneyInformationModel.emailidverified = powerOfAttorneyInformationData.address.foreignAddress.emailOTP;
                        //powerOfAttorneyInformationModel.signed_file_name = powerOfAttorneyInformationData.address.foreignAddress.signatureName;

                        powerOfAttorneyInformationModel.state = "NA";
                        powerOfAttorneyInformationModel.district = "NA";
                        powerOfAttorneyInformationModel.taluka = "NA";
                        powerOfAttorneyInformationModel.city = "NA";
                        powerOfAttorneyInformationModel.flatno_plotno = "NA";
                        powerOfAttorneyInformationModel.societyname = "NA";
                        powerOfAttorneyInformationModel.mainstreet = "NA";
                        powerOfAttorneyInformationModel.landmark = "NA";
                        powerOfAttorneyInformationModel.locality = "NA";
                        powerOfAttorneyInformationModel.pincode = "NA";
                        powerOfAttorneyInformationModel.postofficename = "NA";
                        //powerOfAttorneyInformationModel.address_proof_document_name = "NA";
                        //powerOfAttorneyInformationModel.address_proof_document_path = "NA";
                        powerOfAttorneyInformationModel.signed_file_name = powerOfAttorneyInformationData.address.foreignAddress.signatureName;
                    }

                    powerOfAttorneyInformationModel.username = powerOfAttorneyInformationData.userDetails.userName;
                    powerOfAttorneyInformationModel.city_servey_no = powerOfAttorneyInformationData.userDetails.nabhu;
                    powerOfAttorneyInformationModel.lr_property_id = powerOfAttorneyInformationData.userDetails.lrPropertyUID;
                    powerOfAttorneyInformationModel.mutation_id = powerOfAttorneyInformationData.mutation_id;
                    powerOfAttorneyInformationModel.sub_property_id = powerOfAttorneyInformationData.userDetails.subPropNo;
                    powerOfAttorneyInformationModel.cts_number = powerOfAttorneyInformationData.ctsNo;
                    powerOfAttorneyInformationModel.owner_number = powerOfAttorneyInformationData.ownerNo;
                    powerOfAttorneyInformationModel.mutation_srno = powerOfAttorneyInformationData.mutationSroNo;
                    powerOfAttorneyInformationModel.village_code = powerOfAttorneyInformationData.village_code;
                    powerOfAttorneyInformationModel.village_name = powerOfAttorneyInformationData.village_name;

                    /*powerOfAttorneyInformationModel.attornytype_code = powerOfAttorneyInformationData.attornytype!.poa_type_code;
                    powerOfAttorneyInformationModel.attornytype_desc = powerOfAttorneyInformationData.attornytype.poa_type_description;
                    powerOfAttorneyInformationModel.dastNo = powerOfAttorneyInformationData.dastNo!;
                    powerOfAttorneyInformationModel.dastNoYear = powerOfAttorneyInformationData.dastNoYear!;
                    powerOfAttorneyInformationModel.dastNoDate = powerOfAttorneyInformationData.dastNoDate!;
                    powerOfAttorneyInformationModel.isDastVerified = powerOfAttorneyInformationData.isDastVarified;
                    powerOfAttorneyInformationModel.verifiedDastData = powerOfAttorneyInformationData.verifiedDastData!;
                    powerOfAttorneyInformationModel.isPOAisPartofDast = powerOfAttorneyInformationData.isPOAisPartofDast!;
                    powerOfAttorneyInformationModel.isDeclerationInvolvedInPOA = powerOfAttorneyInformationData.isDeclerationInvolvedInPOA!;
                    powerOfAttorneyInformationModel.isPOAPermanant = powerOfAttorneyInformationData.isPOAPermanant!;
                    powerOfAttorneyInformationModel.isTransferRights = powerOfAttorneyInformationData.isTransferRights!;
                    powerOfAttorneyInformationModel.digcode = powerOfAttorneyInformationData.division!.digcode!;
                    powerOfAttorneyInformationModel.digname = powerOfAttorneyInformationData.division.dig!;
                    powerOfAttorneyInformationModel.poa_district_code = powerOfAttorneyInformationData.district!.jdrcode.ToString()!;
                    powerOfAttorneyInformationModel.poa_district_name = powerOfAttorneyInformationData.district.jdr!;
                    powerOfAttorneyInformationModel.sro_office_code = Convert.ToInt32(powerOfAttorneyInformationData.registrar!.srocode);
                    powerOfAttorneyInformationModel.sro_office_name = powerOfAttorneyInformationData.registrar.sro!;
                    powerOfAttorneyInformationModel.mutation_id = powerOfAttorneyInformationData.mutation_id;*/

                    //Assign Default value
                    dbTable.mobileno = "NA";
                    dbTable.mobilenoverified = "NA";
                    dbTable.emailid = "NA";
                    dbTable.emailidverified = "NA";
                    dbTable.usertype = "NA";
                    dbTable.usertype_code = 0;
                    dbTable.prefixcode_eng = "0";
                    dbTable.prefixcode_marathi = "0";
                    dbTable.sub_property_no = "999999";
                    dbTable.prefix_in_eng = "NA";
                    dbTable.fname_in_eng = "NA";
                    dbTable.mname_in_eng = "NA";
                    dbTable.lname_in_eng = "NA";
                    dbTable.prefix_in_marathi = "NA";
                    dbTable.fname_in_marathi = "NA";
                    dbTable.mname_in_marathi = "NA";
                    dbTable.lname_in_marathi = "NA";
                    //New Fields For Taker
                    dbTable.company_name_in_marathi = "NA";
                    dbTable.company_name_in_eng = "NA";
                    //End
                    dbTable.username = "NA";
                    //New Fields for Taker
                    dbTable.alias_name = "NA";
                    dbTable.gender_code = "NA";
                    dbTable.gender_description = "NA";
                    dbTable.dob = "NA";
                    dbTable.mother_name_in_marathi = "NA";
                    dbTable.mother_name_in_eng = "NA";
                    dbTable.attornytype_desc = "NA";
                    dbTable.attornytype_code = 0;

                    dbTable.landBuyArea = "NA";
                    dbTable.address_type = "NA";
                    dbTable.address = "NA";
                    dbTable.state = "NA";
                    dbTable.district = "NA";
                    dbTable.taluka = "NA";
                    dbTable.city = "NA";
                    dbTable.flatno_plotno = "NA";
                    dbTable.societyname = "NA";
                    dbTable.mainstreet = "NA";
                    dbTable.landmark = "NA";
                    dbTable.locality = "NA";
                    dbTable.pincode = "NA";
                    dbTable.postofficename = "NA";
                    dbTable.address_proof_document_name = "NA";
                    dbTable.address_proof_document_path = "NA";

                    dbTable.owner_of_property_in_maharashtra = false;
                    //dbTable.PropertyTypeMaster = 0;
                    dbTable.property_district_code = "NA";
                    dbTable.property_district_name_in_marathi = "NA";
                    dbTable.property_district_name_in_english = "NA";
                    dbTable.property_taluka_code = "NA";
                    dbTable.property_taluka_name = "NA";
                    dbTable.property_city_code = "NA";
                    dbTable.property_city_name = "NA";
                    dbTable.khateno = "NA";
                    dbTable.ulpin = "NA";
                    dbTable.city_servey_no = "NA";
                    dbTable.lr_property_id = "NA";
                    dbTable.profile_pic_file_name = "NA";
                    dbTable.profile_pic_file_path = "NA";
                    dbTable.isPOAisPartofDast = "NA";
                    dbTable.isDeclerationInvolvedInPOA = "NA";
                    dbTable.isPOAPermanant = "NA";
                    dbTable.isTransferRights = "NA";
                    dbTable.dast_no = "NA";
                    dbTable.dast_no_date = "NA";
                    dbTable.dast_no_year = "NA";
                    dbTable.isDastVerified = false;
                    dbTable.verifieddastData = "NA";
                    dbTable.digcode = 0;
                    dbTable.digname = "NA";
                    dbTable.poa_district_code = "0";
                    dbTable.poa_district_name = "NA";
                    dbTable.sro_office_code = 0;
                    dbTable.sro_office_name = "NA";
                    dbTable.signed_file_path = "NA";
                    dbTable.signed_file_name = "NA";

                    //Assign Data to Table fields to insert new records
                    dbTable.userMaster = powerOfAttorneyInformationModel.userMaster;
                    dbTable.mutation_id = Convert.ToInt32(powerOfAttorneyInformationModel.mutation_id!);
                    dbTable.applicationDTL = powerOfAttorneyInformationModel.applicationDTL;
                    dbTable.mobileno = powerOfAttorneyInformationModel.mobileno!;
                    dbTable.mobilenoverified = string.IsNullOrEmpty(powerOfAttorneyInformationModel.mobilenoverified) ? "NO" : powerOfAttorneyInformationModel.mobilenoverified;
                    dbTable.emailid = powerOfAttorneyInformationModel.emailid!;
                    dbTable.emailidverified = powerOfAttorneyInformationModel.emailidverified!;
                    dbTable.usertype_code = powerOfAttorneyInformationModel.usertype_code;
                    dbTable.usertype = string.IsNullOrEmpty(powerOfAttorneyInformationModel!.usertype) ? "NA" : powerOfAttorneyInformationModel!.usertype;
                    dbTable.prefixcode_marathi = powerOfAttorneyInformationModel.prefixcode_marathi == "" || powerOfAttorneyInformationModel.prefixcode_marathi == null ? "0" : powerOfAttorneyInformationModel.prefixcode_marathi;
                    dbTable.prefixcode_eng = powerOfAttorneyInformationModel.prefixcode_eng == "" || powerOfAttorneyInformationModel.prefixcode_eng == null ? "0" : powerOfAttorneyInformationModel.prefixcode_eng;
                    dbTable.prefix_in_eng = string.IsNullOrEmpty(powerOfAttorneyInformationModel.prefix_in_eng) ? "NA" : powerOfAttorneyInformationModel.prefix_in_eng;
                    dbTable.fname_in_eng = string.IsNullOrEmpty(powerOfAttorneyInformationModel.fname_in_eng) ? "NA" : textInfo.ToTitleCase(powerOfAttorneyInformationModel.fname_in_eng.Trim());
                    dbTable.mname_in_eng = string.IsNullOrEmpty(powerOfAttorneyInformationModel.mname_in_eng) ? "NA" : textInfo.ToTitleCase(powerOfAttorneyInformationModel.mname_in_eng.Trim());
                    dbTable.lname_in_eng = string.IsNullOrEmpty(powerOfAttorneyInformationModel.lname_in_eng) ? "NA" : textInfo.ToTitleCase(powerOfAttorneyInformationModel.lname_in_eng.Trim());
                    dbTable.prefix_in_marathi = string.IsNullOrEmpty(powerOfAttorneyInformationModel.prefix_in_marathi) ? "NA" : powerOfAttorneyInformationModel.prefix_in_marathi.Trim();
                    dbTable.fname_in_marathi = string.IsNullOrEmpty(powerOfAttorneyInformationModel.fname_in_marathi) ? "NA" : powerOfAttorneyInformationModel.fname_in_marathi.Trim();
                    dbTable.mname_in_marathi = string.IsNullOrEmpty(powerOfAttorneyInformationModel.mname_in_marathi) ? "NA" : powerOfAttorneyInformationModel.mname_in_marathi.Trim();
                    dbTable.lname_in_marathi = string.IsNullOrEmpty(powerOfAttorneyInformationModel.lname_in_marathi) ? "NA" : powerOfAttorneyInformationModel.lname_in_marathi.Trim();
                    dbTable.company_name_in_eng = string.IsNullOrEmpty(powerOfAttorneyInformationModel.company_name_in_eng) ? "NA" : powerOfAttorneyInformationModel.company_name_in_eng;
                    dbTable.company_name_in_marathi = string.IsNullOrEmpty(powerOfAttorneyInformationModel.company_name_in_marathi) ? "NA" : powerOfAttorneyInformationModel.company_name_in_marathi;
                    dbTable.username = powerOfAttorneyInformationModel.username!;
                    dbTable.city_servey_no = powerOfAttorneyInformationModel.city_servey_no!;
                    dbTable.lr_property_id = powerOfAttorneyInformationModel.lr_property_id!;
                    dbTable.sub_property_no = powerOfAttorneyInformationModel.sub_property_id!;
                    dbTable.cts_number = powerOfAttorneyInformationModel.cts_number;
                    dbTable.mutation_srno = powerOfAttorneyInformationModel.mutation_srno;
                    dbTable.owner_number = powerOfAttorneyInformationModel.owner_number;
                    dbTable.village_code = powerOfAttorneyInformationModel.village_code;
                    dbTable.village_name = powerOfAttorneyInformationModel.village_name;
                    dbTable.address_type = powerOfAttorneyInformationModel.address_type;
                    dbTable.address = powerOfAttorneyInformationModel.address!;
                    dbTable.state = powerOfAttorneyInformationModel.state!;
                    dbTable.district = powerOfAttorneyInformationModel.district!;
                    dbTable.taluka = powerOfAttorneyInformationModel.taluka!;
                    dbTable.city = powerOfAttorneyInformationModel.city!;
                    dbTable.flatno_plotno = powerOfAttorneyInformationModel.flatno_plotno!;
                    dbTable.societyname = powerOfAttorneyInformationModel.societyname!;
                    dbTable.mainstreet = powerOfAttorneyInformationModel.mainstreet!;
                    dbTable.landmark = powerOfAttorneyInformationModel.landmark!;
                    dbTable.locality = powerOfAttorneyInformationModel.locality!;
                    dbTable.pincode = powerOfAttorneyInformationModel.pincode!;
                    dbTable.postofficename = powerOfAttorneyInformationModel.postofficename!;
                    //dbTable.address_proof_document_name = powerOfAttorneyInformationModel.address_proof_document_name;
                    //dbTable.signed_file_name = powerOfAttorneyInformationModel.signed_file_name;
                    dbTable.address_proof_document_name = "NA";
                    dbTable.address_proof_document_path = "NA";
                    dbTable.signed_file_name = "NA";
                    dbTable.signed_file_path = "NA";
                    dbTable.attornytype_code = 0;
                    dbTable.attornytype_desc = "NA";
                    dbTable.is_taker = false;
                    dbTable.alias_name = "NA";
                    dbTable.gender_code = "NA";
                    dbTable.gender_description = "NA";
                    dbTable.khata_type_code = "NA";
                    dbTable.khata_type_name = "NA";

                    dbTable.owner_status_code = "NA";
                    dbTable.owner_status_description = "NA";

                    dbTable.dob = "NA";
                    dbTable.mother_name_in_marathi = "NA";
                    dbTable.mother_name_in_eng = "NA";

                    /* dbTable.apk_code = 0;
                     dbTable.apk_description = "NA";

                     dbTable.aapak = "NA";*/
                    dbTable.landBuyArea = "NA";
                    dbTable.profile_pic_file_name = "NA";
                    dbTable.profile_pic_file_path = "NA";
                    dbTable.power_of_attorney_code = GeneratePowerOfAttorneyCode(powerOfAttorneyInformationData.applicationid!, false);
                    _context.powerOfAttorneyInformation.Add(dbTable);
                    _context.SaveChanges();

                    //Get Saved Row ID
                    int powerOfAttorneyID = dbTable.power_of_attorney_id;

                    string CurrentDateTime = DateTime.Now.ToString("yyyy-MMM-dd-HHmmss");
                    bool checkAddressFlag = true;
                    //bool checkSignFlag = false;
                    if (powerOfAttorneyInformationModel.address_type == "INDIA")
                    {
                        if (!string.IsNullOrEmpty(powerOfAttorneyInformationData.address.indiaAddress!.addressProofSrc) && !string.IsNullOrEmpty(powerOfAttorneyInformationData.address.indiaAddress.addressProofName) && powerOfAttorneyInformationData.address.indiaAddress!.addressProofSrc != "NA" && powerOfAttorneyInformationData.address.indiaAddress!.addressProofName != "NA")
                        {
                            string[] AddressData = powerOfAttorneyInformationData.address.indiaAddress.addressProofSrc.Split(",");
                            checkAddressFlag = methodForFile.SaveImageForApplicant(AddressData[1], powerOfAttorneyInformationData.address.indiaAddress.addressProofName, powerOfAttorneyID.ToString(), "AddressProof", FolderPath + @"\", CurrentDateTime);
                            string AddressProofExt = Path.GetExtension(powerOfAttorneyInformationData.address.indiaAddress.addressProofName);
                            powerOfAttorneyInformationModel.address_proof_document_name = "AddressProof" + powerOfAttorneyID + "_" + CurrentDateTime + AddressProofExt;
                            powerOfAttorneyInformationModel.address_proof_document_path = FolderPath + powerOfAttorneyID + @"\" + powerOfAttorneyInformationModel.address_proof_document_name;

                            var UpdateAddressFilePath = _context.powerOfAttorneyInformation.Where(w => w.power_of_attorney_id == powerOfAttorneyID).FirstOrDefault();
                            if (UpdateAddressFilePath != null)
                            {
                                dbTable.address_proof_document_name = powerOfAttorneyInformationModel.address_proof_document_name;
                                dbTable.address_proof_document_path = powerOfAttorneyInformationModel.address_proof_document_path;
                                _context.Entry(dbTable).CurrentValues.SetValues(dbTable);
                                _context.SaveChanges();
                            }
                        }

                        //string[] signData = powerOfAttorneyInformationData.address.indiaAddress.signatureSrc.Split(",");
                        //checkSignFlag = methodForFile.SaveImageForApplicant(signData[1], powerOfAttorneyInformationData.address.indiaAddress.signatureName, powerOfAttorneyID.ToString(), "Signature", FolderPath + @"\", CurrentDateTime);
                        //string SignatureExt = Path.GetExtension(powerOfAttorneyInformationData.address.indiaAddress.signatureName);
                        //powerOfAttorneyInformationModel.signed_file_name = "Signature" + powerOfAttorneyID + "_" + CurrentDateTime + SignatureExt;
                        //powerOfAttorneyInformationModel.signed_file_path = FolderPath + @"\" + powerOfAttorneyID + @"\" + powerOfAttorneyInformationModel.signed_file_name;
                        //var UpdateSignFilePath = _context.powerOfAttorneyInformation.Where(w => w.power_of_attorney_id == powerOfAttorneyID).FirstOrDefault();
                        //if (UpdateSignFilePath != null)
                        //{
                        //    dbTable.signed_file_name = powerOfAttorneyInformationModel.signed_file_name;
                        //    dbTable.signed_file_path = powerOfAttorneyInformationModel.signed_file_path;
                        //    _context.Entry(dbTable).CurrentValues.SetValues(dbTable);
                        //    _context.SaveChanges();
                        //}
                    }
                    //if (powerOfAttorneyInformationModel.address_type == "FOREIGN")
                    //{
                    //    string[] signData = powerOfAttorneyInformationData.address.foreignAddress.signatureSrc.Split(",");
                    //    checkSignFlag = methodForFile.SaveImageForApplicant(signData[1], powerOfAttorneyInformationData.address.foreignAddress.signatureName, powerOfAttorneyID.ToString(), "Signature", FolderPath + @"\", CurrentDateTime);

                    //    string SignatureExt = Path.GetExtension(powerOfAttorneyInformationData.address.foreignAddress.signatureName);
                    //    powerOfAttorneyInformationModel.signed_file_name = "Signature" + powerOfAttorneyID + "_" + CurrentDateTime + SignatureExt;
                    //    powerOfAttorneyInformationModel.signed_file_path = FolderPath + @"\" + powerOfAttorneyID + @"\" + powerOfAttorneyInformationModel.signed_file_name;
                    //    var UpdateSignFilePath = _context.powerOfAttorneyInformation.Where(w => w.power_of_attorney_id == powerOfAttorneyID).FirstOrDefault();
                    //    if (UpdateSignFilePath != null)
                    //    {
                    //        dbTable.signed_file_name = powerOfAttorneyInformationModel.signed_file_name;
                    //        dbTable.signed_file_path = powerOfAttorneyInformationModel.signed_file_path;
                    //        _context.Entry(dbTable).CurrentValues.SetValues(dbTable);
                    //        _context.SaveChanges();
                    //    }
                    //}
                    if (checkAddressFlag)
                    {
                        var applicationDTLdata = _context.applicationDTL.Where(data => data.applicationid!.Equals(powerOfAttorneyInformationData.applicationid)).FirstOrDefault();
                        if (applicationDTLdata != null)
                        {
                            if (!string.IsNullOrEmpty(applicationDTLdata.powerOfAttorneyIDs) && !applicationDTLdata.powerOfAttorneyIDs.Contains(powerOfAttorneyID.ToString()))
                            {
                                applicationDTLdata.powerOfAttorneyIDs = applicationDTLdata.powerOfAttorneyIDs + "," + powerOfAttorneyID.ToString();
                            }
                            else
                            {
                                applicationDTLdata.powerOfAttorneyIDs = powerOfAttorneyID.ToString();
                            }
                            _context.Entry(applicationDTLdata).CurrentValues.SetValues(applicationDTLdata);
                            _context.SaveChanges();
                        }
                        scope.Complete();
                        return "Success" + "," + powerOfAttorneyID;
                    }
                    else
                    {
                        _context.powerOfAttorneyInformation.Remove(dbTable);
                        _context.SaveChanges();
                        return "Address Proof File Is Not Uploaded";
                    }
                    //if (!checkAddressFlag)
                    //{
                    //    _context.powerOfAttorneyInformation.Remove(dbTable);
                    //    _context.SaveChanges();
                    //    return "Address Proof File Is Not Uploaded";
                    //}
                    //if (!checkSignFlag)
                    //{
                    //    _context.powerOfAttorneyInformation.Remove(dbTable);
                    //    _context.SaveChanges();
                    //    return "Signature File Is Not Uploaded";
                    //}
                    //else
                    //{
                    //    _context.powerOfAttorneyInformation.Remove(dbTable);
                    //    _context.SaveChanges();
                    //    return "Some Files Are Not Uploaded";
                    //}
                }
                catch (Exception ex)
                {
                    _context.powerOfAttorneyInformation.Remove(dbTable);
                    _context.SaveChanges();
                    throw new HandleException(ex.Message.ToString());
                }
            }
        }



        public FetchPOAForGiverData FetchPowerOfAttorneyInfoForGiver(int powerOfAttorneyID)
        {
            try
            {
                MethodForFileUpload methodForFile = new MethodForFileUpload();
                PowerOfAttorneyInformation powerOfAttorneyInformation = new PowerOfAttorneyInformation();
                powerOfAttorneyInformation = _contextR.powerOfAttorneyInformation.Include(i => i.userMaster).Include(app => app.applicationDTL).Where(data => data.power_of_attorney_id.Equals(powerOfAttorneyID) && data.is_taker == false && data.isDeleted == false).FirstOrDefault()!;
                FetchPOAForGiverData fetchData = new FetchPOAForGiverData();
                if (powerOfAttorneyInformation != null)
                {
                    fetchData.power_of_attorney_id = powerOfAttorneyInformation.power_of_attorney_id;
                    fetchData.userid = powerOfAttorneyInformation.userMaster!.userid;
                    fetchData.applicationid = powerOfAttorneyInformation.applicationDTL!.applicationid;
                    fetchData.ctsNo = powerOfAttorneyInformation.cts_number;
                    fetchData.ownerNo = powerOfAttorneyInformation.owner_number;
                    fetchData.mutationSroNo = powerOfAttorneyInformation.mutation_srno;
                    fetchData.village_code = powerOfAttorneyInformation.village_code;
                    fetchData.village_name = powerOfAttorneyInformation.village_name;

                    fetchData.mobileNo = powerOfAttorneyInformation.mobileno;
                    fetchData.is_taker = powerOfAttorneyInformation.is_taker;

                    ViewModel.UserDetailsDataForPOAGiver userDetails = new ViewModel.UserDetailsDataForPOAGiver();

                    userDetails.nabhu = powerOfAttorneyInformation.city_servey_no;
                    userDetails.userName = powerOfAttorneyInformation.username;
                    userDetails.lrPropertyUID = powerOfAttorneyInformation.lr_property_id;
                    userDetails.subPropNo = powerOfAttorneyInformation.sub_property_no;
                    fetchData.user_type_code = powerOfAttorneyInformation.usertype_code;
                    fetchData.usertype = powerOfAttorneyInformation.usertype;

                    if (powerOfAttorneyInformation.usertype_code == 1 || powerOfAttorneyInformation.usertype_code == 0)
                    {
                        fetchData.fullNameInMarathi = powerOfAttorneyInformation.fname_in_marathi.Trim() + " " + powerOfAttorneyInformation.mname_in_marathi.Trim() + " " + powerOfAttorneyInformation.lname_in_marathi.Trim();
                        fetchData.fullNameInEng = powerOfAttorneyInformation.fname_in_eng.Trim() + " " + powerOfAttorneyInformation.mname_in_eng.Trim() + " " + powerOfAttorneyInformation.lname_in_eng.Trim();
                        userDetails.suffixcode = powerOfAttorneyInformation.prefixcode_marathi;
                        userDetails.suffix = powerOfAttorneyInformation.prefix_in_marathi;
                        userDetails.firstName = powerOfAttorneyInformation.fname_in_marathi;
                        userDetails.middleName = powerOfAttorneyInformation.mname_in_marathi;
                        userDetails.lastName = powerOfAttorneyInformation.lname_in_marathi;
                        userDetails.suffixCodeEng = powerOfAttorneyInformation.prefixcode_eng;
                        userDetails.suffixEng = powerOfAttorneyInformation.prefix_in_eng;
                        userDetails.firstNameEng = powerOfAttorneyInformation.fname_in_eng;
                        userDetails.middleNameEng = powerOfAttorneyInformation.mname_in_eng;
                        userDetails.lastNameEng = powerOfAttorneyInformation.lname_in_eng;
                    }
                    else
                    {
                        userDetails.company_name_in_eng = powerOfAttorneyInformation.company_name_in_eng;
                        userDetails.company_name_in_marathi = powerOfAttorneyInformation.company_name_in_marathi;
                        fetchData.fullNameInMarathi = powerOfAttorneyInformation.company_name_in_marathi;
                        fetchData.fullNameInEng = powerOfAttorneyInformation.company_name_in_eng;
                    }
                    fetchData.userDetails = userDetails;
                    ViewModel.AddressDataForPOAGiver addressData = new ViewModel.AddressDataForPOAGiver();
                    addressData.addressType = powerOfAttorneyInformation.address_type;
                    if (powerOfAttorneyInformation.address_type == "INDIA")
                    {
                        ViewModel.AddressForIndiaForPOAGiver addressForIndia = new ViewModel.AddressForIndiaForPOAGiver();
                        addressForIndia.state = powerOfAttorneyInformation.state;
                        addressForIndia.district = powerOfAttorneyInformation.district;
                        addressForIndia.city = powerOfAttorneyInformation.city;
                        addressForIndia.taluka = powerOfAttorneyInformation.taluka;
                        addressForIndia.plotNo = powerOfAttorneyInformation.flatno_plotno;
                        addressForIndia.building = powerOfAttorneyInformation.societyname;
                        addressForIndia.mainRoad = powerOfAttorneyInformation.mainstreet;
                        addressForIndia.impSymbol = powerOfAttorneyInformation.landmark;
                        addressForIndia.area = powerOfAttorneyInformation.locality;
                        addressForIndia.pincode = powerOfAttorneyInformation.pincode;
                        addressForIndia.postOfficeName = powerOfAttorneyInformation.postofficename;
                        addressForIndia.addressProofName = powerOfAttorneyInformation.address_proof_document_name;
                        addressForIndia.mobile = powerOfAttorneyInformation.mobileno;
                        addressForIndia.mobileOTP = powerOfAttorneyInformation.mobilenoverified;
                        addressForIndia.signatureName = powerOfAttorneyInformation.signed_file_name;

                        if (powerOfAttorneyInformation.address_proof_document_path != "NA")
                        {
                            string AddressProofExt = Path.GetExtension(powerOfAttorneyInformation.address_proof_document_path);
                            string AddressProof = methodForFile.ConvertImageToBase64(powerOfAttorneyInformation.address_proof_document_path);
                            powerOfAttorneyInformation.address_proof_document_path = string.IsNullOrEmpty(AddressProof) ? "NA" : "data:image/" + AddressProofExt.Replace(".", "") + ";base64," + AddressProof;
                            addressForIndia.addressProofSrc = powerOfAttorneyInformation.address_proof_document_path;
                        }
                        else
                        {
                            addressForIndia.addressProofSrc = powerOfAttorneyInformation.address_proof_document_path;
                        }

                        //string SignatureExt = Path.GetExtension(powerOfAttorneyInformation.signed_file_path);
                        //string Signature = methodForFile.ConvertImageToBase64(powerOfAttorneyInformation.signed_file_path);
                        //powerOfAttorneyInformation.signed_file_path = string.IsNullOrEmpty(Signature) ? "NA" : "data:image/" + SignatureExt.Replace(".", "") + ";base64," + Signature;
                        addressForIndia.signatureSrc = powerOfAttorneyInformation.signed_file_path;
                        addressData.indiaAddress = addressForIndia;
                    }
                    else if (powerOfAttorneyInformation.address_type == "FOREIGN")
                    {
                        ViewModel.AddressForForeignForPOAGiver addressForForeign = new ViewModel.AddressForForeignForPOAGiver();
                        addressForForeign.address = powerOfAttorneyInformation.address;
                        addressForForeign.mobile = powerOfAttorneyInformation.mobileno;
                        addressForForeign.email = powerOfAttorneyInformation.emailid;
                        addressForForeign.emailOTP = powerOfAttorneyInformation.emailidverified;
                        addressForForeign.signatureName = powerOfAttorneyInformation.signed_file_name;

                        //string SignatureExt = Path.GetExtension(powerOfAttorneyInformation.signed_file_path);
                        //string Signature = methodForFile.ConvertImageToBase64(powerOfAttorneyInformation.signed_file_path);
                        //powerOfAttorneyInformation.signed_file_path = string.IsNullOrEmpty(Signature) ? "NA" : "data:image/" + SignatureExt.Replace(".", "") + ";base64," + Signature;
                        addressForForeign.signatureSrc = powerOfAttorneyInformation.signed_file_path;
                        addressData.foreignAddress = addressForForeign;
                    }
                    fetchData.address = addressData;

                    AttroneyTypeForPOATaker attroneyType = new AttroneyTypeForPOATaker();
                    attroneyType.poa_type_code = powerOfAttorneyInformation.attornytype_code;
                    attroneyType.poa_type_description = powerOfAttorneyInformation.attornytype_desc;
                    fetchData.attornytype = attroneyType;
                    Division devision = new Division();
                    devision.digcode = Convert.ToInt32(powerOfAttorneyInformation.digcode);
                    devision.dig = powerOfAttorneyInformation.digname;
                    fetchData.division = devision;

                    DastDistrict districtData = new DastDistrict();
                    districtData.jdrcode = Convert.ToInt32(powerOfAttorneyInformation.poa_district_code);
                    districtData.jdr = powerOfAttorneyInformation.poa_district_name;
                    //  districtData.district_english_name = powerOfAttorneyInformation.poa_district_name_in_eng;

                    fetchData.district = districtData;

                    Dastregsiter registrarData = new Dastregsiter();
                    registrarData.srocode = powerOfAttorneyInformation.sro_office_code;
                    registrarData.sro = powerOfAttorneyInformation.sro_office_name;
                    fetchData.registrar = registrarData;

                    fetchData.dastNo = powerOfAttorneyInformation.dast_no;
                    fetchData.dastNoDate = powerOfAttorneyInformation.dast_no_date;
                    fetchData.dastNoYear = powerOfAttorneyInformation.dast_no_year;
                    fetchData.isPOAisPartofDast = powerOfAttorneyInformation.isPOAisPartofDast;
                    fetchData.isDeclerationInvolvedInPOA = powerOfAttorneyInformation.isDeclerationInvolvedInPOA;
                    fetchData.isPOAPermanant = powerOfAttorneyInformation.isPOAPermanant;
                    fetchData.isTransferRights = powerOfAttorneyInformation.isTransferRights;
                    fetchData.passportName = powerOfAttorneyInformation.profile_pic_file_name;
                    fetchData.passportSrc = powerOfAttorneyInformation.profile_pic_file_path;
                    //if (!string.IsNullOrEmpty(powerOfAttorneyInformation.profile_pic_file_name) && powerOfAttorneyInformation.profile_pic_file_name != "NA")
                    //{
                    //    fetchData.passportName = powerOfAttorneyInformation.profile_pic_file_name;
                    //    string ProfilePicExt = Path.GetExtension(powerOfAttorneyInformation.profile_pic_file_path);
                    //    string ProfilePic = methodForFile.ConvertImageToBase64(powerOfAttorneyInformation.profile_pic_file_path);
                    //    powerOfAttorneyInformation.profile_pic_file_path = string.IsNullOrEmpty(ProfilePic) ? "NA" : "data:image/" + ProfilePicExt.Replace(".", "") + ";base64," + ProfilePic;
                    //    fetchData.passportSrc = powerOfAttorneyInformation.profile_pic_file_path;
                    //}
                    //else
                    //{
                    //    fetchData.passportName = powerOfAttorneyInformation.profile_pic_file_name;
                    //    fetchData.passportSrc = powerOfAttorneyInformation.profile_pic_file_path;
                    //}
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

        public FetchGiverData FetchPOAGiverData(string applicationID)
        {
            FetchGiverData fetchData = new FetchGiverData();
            var giverNamesData = _contextR.powerOfAttorneyInformation.Include(i => i.userMaster).Include(app => app.applicationDTL).Where(data => data.applicationDTL!.applicationid!.Equals(applicationID) && data.is_taker == false).ToList();
            if (giverNamesData.Count > 0)
            {
                List<string> giverNameInMarathiList = new List<string>();
                List<string> giverNameInEnglishList = new List<string>();
                for (int i = 0; i < giverNamesData.Count; i++)
                {
                    giverNameInMarathiList.Add(giverNamesData[i].fname_in_marathi.Trim() + " " + giverNamesData[i].mname_in_marathi.Trim() + " " + giverNamesData[i].lname_in_marathi.Trim());
                    giverNameInEnglishList.Add(giverNamesData[i].fname_in_eng.Trim() + " " + giverNamesData[i].mname_in_eng.Trim() + " " + giverNamesData[i].lname_in_eng.Trim());
                }
                fetchData.giver_names_in_marathi = giverNameInMarathiList;
                fetchData.giver_names_in_english = giverNameInEnglishList;
            }
            else
            {
                fetchData = null!;
            }
            return fetchData;
        }


        //Fetch Document Type By Application ID
        public FetchDocumentTypeData FetchDocumentType(string applicationID)
        {
            FetchDocumentTypeData fetchData = new FetchDocumentTypeData();
            List<DocumentTypeData> documentTypeDataList = new List<DocumentTypeData>();
            ApplicationDTL applicationDTL = new ApplicationDTL();
            applicationDTL = FetchApplicationData(applicationID);
            fetchData.mutationType = applicationDTL.mutation_type_name;

            DocumentTypeMaster documentType = new DocumentTypeMaster();
            var documentTypeList = _contextR.documentTypeMasters.Where(data => data.mutation_type_code!.Equals(applicationDTL.mutation_type_code)).ToList();
            if (documentTypeList.Count > 0)
            {
                documentTypeList.ForEach(row => documentTypeDataList.Add(new DocumentTypeData()
                {
                    documentTypeCode = (row.document_type_id).ToString(),
                    documentType = row.document_type_name
                }));
                fetchData.mutationTypeHDR = documentTypeList[0].mutation_description;
            }
            fetchData.documentTypeDataList = documentTypeDataList;
            return fetchData;
        }

        // Save Uploaded Docs Data
        public string SaveDocUploadedData(AddUploadedDocumentsDTLData addUploadedDocuments)
        {
            using (var scope = new TransactionScope())
            {
                try
                {
                    MethodForFileUpload methodForFile = new MethodForFileUpload();
                    string DocUploadStatus = string.Empty;
                    string FolderPath = @"\\10.10.248.2\pde_propertycard\MUTATIONDOCS\" + addUploadedDocuments.applicationid + @"\DOCUMENTS\";
                    //string FolderPath = @"D:\WWW\MUTATIONDOCS\" + addUploadedDocuments.applicationid + @"\DOCUMENTS\";
                    // Assign Values to Model
                    UploadedDocumentsDTLModel uploadedDocumentsDTLModel = new UploadedDocumentsDTLModel();
                    UserMaster userMaster = _context.userMasters.FirstOrDefault(s => s.userid == addUploadedDocuments.userid)!;
                    uploadedDocumentsDTLModel.userMaster = userMaster;

                    ApplicationDTL applicationDTL = _context.applicationDTL.FirstOrDefault(s => s.applicationid == addUploadedDocuments.applicationid)!;
                    uploadedDocumentsDTLModel.applicationDTL = applicationDTL;

                    //DocumentTypeMaster documentTypeMaster = _context.documentTypeMasters.FirstOrDefault(s => s.document_type_id == Convert.ToInt32(addUploadedDocuments.docType.document_code));
                    //uploadedDocumentsDTLModel.documentType = documentTypeMaster;
                    uploadedDocumentsDTLModel.documentTypeCode = addUploadedDocuments.docType!.document_code;
                    uploadedDocumentsDTLModel.documentType = addUploadedDocuments.docType.document_name;

                    //uploadedDocumentsDTLModel.city_servey_no = addUploadedDocuments.nabhu;
                    uploadedDocumentsDTLModel.document_name = addUploadedDocuments.docUpload!.docName;
                    uploadedDocumentsDTLModel.document_path = FolderPath + uploadedDocumentsDTLModel.document_name;

                    //Assign Data to Table fields to insert new records
                    UploadedDocumentsDTL dbTable = new UploadedDocumentsDTL();
                    dbTable.userMaster = uploadedDocumentsDTLModel.userMaster;
                    dbTable.applicationDTL = uploadedDocumentsDTLModel.applicationDTL;
                    dbTable.document_type_code = uploadedDocumentsDTLModel.documentTypeCode;
                    dbTable.document_type = uploadedDocumentsDTLModel.documentType;
                    dbTable.city_servey_no = uploadedDocumentsDTLModel.city_servey_no;
                    dbTable.document_name = uploadedDocumentsDTLModel.document_name;
                    dbTable.document_path = uploadedDocumentsDTLModel.document_path;
                    _context.uploadedDocumentsDTLs.Add(dbTable);

                    string imageName = System.IO.Path.GetFileNameWithoutExtension(addUploadedDocuments.docUpload.docName!);
                    if (methodForFile.ContainsSpecialCharacters(imageName))
                    {
                        return addUploadedDocuments.docUpload.docName! + " Image Name contains special Characters";
                    }
                    else
                    {
                        DocUploadStatus = methodForFile.ConvertBase64ToPdf(FolderPath, addUploadedDocuments.docUpload.docName!, addUploadedDocuments.docUpload.docSrc!);//methodForFile.UploadPDFFile(FolderPath, addUploadedDocuments.docUpload);
                        if (DocUploadStatus == "Success")
                        {
                            _context.SaveChanges();
                            //Get Saved Row ID
                            int uploadedDocID = dbTable.uploaded_doc_id;
                            var applicationDTLdata = _context.applicationDTL.Where(data => data.applicationid!.Equals(addUploadedDocuments.applicationid)).FirstOrDefault();
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
                                applicationDTLdata.status = 8;
                                _context.Entry(applicationDTLdata).CurrentValues.SetValues(applicationDTLdata);
                                _context.SaveChanges();
                            }
                            scope.Complete();
                            return "Success";
                        }
                        else
                        {
                            return DocUploadStatus;
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new HandleException(ex.Message.ToString());
                }
            }
        }

        public FetchUploadedDocumentsData FetchUploadedDocumentDTL(int documentID)
        {
            try
            {
                UploadedDocumentsDTL uploadedDocumentsDTL = new UploadedDocumentsDTL();
                uploadedDocumentsDTL = _contextR.uploadedDocumentsDTLs.Include(i => i.userMaster).Include(app => app.applicationDTL).Where(data => data.uploaded_doc_id.Equals(documentID) && data.isDeleted == false
                && data.truti_patra_flag == "NA").FirstOrDefault()!;
                FetchUploadedDocumentsData fetchData = new FetchUploadedDocumentsData();
                if (uploadedDocumentsDTL != null)
                {
                    fetchData.applicationID = uploadedDocumentsDTL.applicationDTL!.applicationid;
                    fetchData.documentID = uploadedDocumentsDTL.uploaded_doc_id.ToString();
                    fetchData.documentTypeCode = Convert.ToInt32(uploadedDocumentsDTL.document_type_code);
                    fetchData.documentType = uploadedDocumentsDTL.document_type;

                    MutationCTSNoDTL mutationCTSNoDTL = new MutationCTSNoDTL();
                    mutationCTSNoDTL = _contextR.mutationCTSNoDTLs.Where(data => data.applicationDTL!.applicationid!.Equals(uploadedDocumentsDTL.applicationDTL.applicationid)).FirstOrDefault()!;
                    //fetchData.city = mutationCTSNoDTL.village_or_peth_name;
                    fetchData.nabhuno = uploadedDocumentsDTL.city_servey_no == null || uploadedDocumentsDTL.city_servey_no == "" ? "NA" : uploadedDocumentsDTL.city_servey_no;
                    fetchData.docName = uploadedDocumentsDTL.document_name;

                    if (System.IO.File.Exists(uploadedDocumentsDTL.document_path))
                    {
                        //File
                        Byte[] fileBytes = File.ReadAllBytes(uploadedDocumentsDTL.document_path!);
                        string FileExt = Path.GetExtension(uploadedDocumentsDTL.document_path)!;
                        var content = Convert.ToBase64String(fileBytes);
                        fetchData.docSrc = string.IsNullOrEmpty(content) ? "NA" : "data:pdf/" + FileExt.Replace(".", "") + ";base64," + content;
                    }
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

        public string DeleteUploadedDocument(DeleteUploadedDocumentData documentData)
        {
            using (var scope = new TransactionScope())
            {
                try
                {
                    MethodForFileUpload methodForFile = new MethodForFileUpload();
                    UploadedDocumentsDTL dbTable = new UploadedDocumentsDTL();
                    var FetchdocumentData = _context.uploadedDocumentsDTLs.Include(i => i.applicationDTL).Where(w => w.uploaded_doc_id == Convert.ToInt32(documentData.documentID) && w.applicationDTL!.applicationid == documentData.applicationID && w.isDeleted == false).FirstOrDefault();
                    if (FetchdocumentData != null)
                    {
                        ApplicationDTL applicationDTLdata = new ApplicationDTL();
                        applicationDTLdata = _context.applicationDTL.Where(data => data.applicationid!.Equals(documentData.applicationID)).FirstOrDefault()!;
                        // dbTable.uploaded_doc_id = Convert.ToInt32(documentData.documentID);
                        //dbTable.applicationDTL = applicationDTLdata;
                        //documentData.applicationID;
                        _context.uploadedDocumentsDTLs.Remove(FetchdocumentData);
                        _context.Entry(dbTable).CurrentValues.SetValues(FetchdocumentData);

                        if (applicationDTLdata != null)
                        {
                            if (!string.IsNullOrEmpty(applicationDTLdata.uploadedDocIDs))
                            {
                                string[] documentIDS = applicationDTLdata.uploadedDocIDs.Split(",");
                                if (documentIDS.Length > 1)
                                {
                                    applicationDTLdata.uploadedDocIDs = "";
                                    for (int i = 0; i < documentIDS.Length; i++)
                                    {
                                        if (documentIDS[i] != documentData.documentID)
                                        {
                                            applicationDTLdata.uploadedDocIDs = !string.IsNullOrEmpty(applicationDTLdata.uploadedDocIDs) ? applicationDTLdata.uploadedDocIDs + "," + documentIDS[i] : documentIDS[i];
                                            //if (!string.IsNullOrEmpty(applicationDTLdata.uploadedDocIDs))
                                            //{
                                            //    applicationDTLdata.uploadedDocIDs = applicationDTLdata.uploadedDocIDs + "," + documentIDS[i];
                                            //}
                                            //else
                                            //{
                                            //    applicationDTLdata.uploadedDocIDs = documentIDS[i];
                                            //}
                                        }
                                    }
                                }
                                else
                                {
                                    applicationDTLdata.uploadedDocIDs = "";
                                }
                            }
                            else
                            {
                                applicationDTLdata.uploadedDocIDs = "";
                            }
                            _context.Entry(applicationDTLdata).CurrentValues.SetValues(applicationDTLdata);
                        }
                        _context.SaveChanges();
                        methodForFile.DeleteFile(FetchdocumentData.document_path!);
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

        //POA For Taker
        public string SavePowerOfAttorneyTaker(PowerOfAttorneyInformationDataForTaker takerData)
        {
            using (var scope = new TransactionScope())
            {
                MethodForFileUpload methodForFile = new MethodForFileUpload();
                PowerOfAttorneyInformation dbTable = new PowerOfAttorneyInformation();
                // Assign values to model
                PowerOfAttorneyInformationModel powerOfAttorneyInformationModel = new PowerOfAttorneyInformationModel();
                string FolderPath = @"\\10.10.248.2\pde_propertycard\MUTATIONDOCS\" + takerData.applicationid + @"\POWEROFATTORNEY\TAKER\";
                try
                {
                    if (takerData.selectedOptions != null)
                    {
                        foreach (var Item in takerData.selectedOptions)
                        {
                            if (string.IsNullOrEmpty(Item.name))
                            {
                                return "Please select मुखत्यारपत्र देणारे";
                            }
                        }
                    }
                    if (string.IsNullOrEmpty(takerData.isMHProperty!.userDetails!.firstName!) || string.IsNullOrEmpty(takerData.isMHProperty!.userDetails!.middleName!) || string.IsNullOrEmpty(takerData.isMHProperty!.userDetails!.lastName!))
                    {
                        return "Please Enter घेणाऱ्याचे नाव (मराठी मध्ये) ";
                    }
                    if (string.IsNullOrEmpty(takerData.isMHProperty!.userDetails!.firstNameEng!) || string.IsNullOrEmpty(takerData.isMHProperty!.userDetails!.middleNameEng!) || string.IsNullOrEmpty(takerData.isMHProperty!.userDetails!.lastNameEng!))
                    {
                        return "Please Enter घेणाऱ्याचे नाव (इंग्रजी मध्ये) ";
                    }
                    if (string.IsNullOrEmpty(takerData.dharak!.userdharak!.gender!.gender_description))
                    {
                        return "Please select लिंग निवडा ";
                    }
                    if (string.IsNullOrEmpty(takerData.dharak!.userdharak!.dob))
                    {
                        return "Please select जन्म दिनांक";
                    }
                    if (string.IsNullOrEmpty(takerData.division!.dig))
                    {
                        return "Please select विभाग ";
                    }
                    if (string.IsNullOrEmpty(takerData.district!.jdr))
                    {
                        return "Please select जिल्हा";
                    }
                    if (string.IsNullOrEmpty(takerData.registrar!.sro))
                    {
                        return "Please select दुय्यम निबंधक कार्यालय";
                    }
                    if (string.IsNullOrEmpty(takerData.dastNo))
                    {
                        return "Please enter रजीस्टर्ड दस्त क्रमांक";
                    }
                    if (string.IsNullOrEmpty(takerData.dastNoDate))
                    {
                        return "Please select दस्त दिनांक ";
                    }
                    if (string.IsNullOrEmpty(takerData.dastNoYear))
                    {
                        return "Please enter वर्ष";

                    }
                    if (methodForFile.ContainsSpecialCharactersInMarathiName(takerData.isMHProperty!.userDetails!.firstName!)
                        || methodForFile.ContainsSpecialCharactersInMarathiName(takerData.isMHProperty!.userDetails!.middleName!) ||
                        methodForFile.ContainsSpecialCharactersInMarathiName(takerData.isMHProperty!.userDetails!.lastName!))
                    {
                        return "घेणाऱ्याचे नाव (मराठी मध्ये) Field contains English Letters / special characters!";
                    }
                    if (methodForFile.ContainsSpecialCharactersInName(takerData.isMHProperty.userDetails!.firstNameEng!)
                        || methodForFile.ContainsSpecialCharactersInName(takerData.isMHProperty.userDetails!.middleNameEng!) ||
                        methodForFile.ContainsSpecialCharactersInName(takerData.isMHProperty.userDetails!.lastNameEng!))
                    {
                        return "घेणाऱ्याचे नाव (इंग्रजी मध्ये) Field contains special characters!";
                    }
                    if (!string.IsNullOrEmpty(takerData.dharak!.userdharak!.aliceName!) && methodForFile.ContainsSpecialCharactersInMarathiName(takerData.dharak!.userdharak!.aliceName!))
                    {
                        return "घेणाऱ्याचे उर्फ नाव (मराठी मध्ये) Field contains English Letter / special characters!";
                    }
                    if (methodForFile.CheckDastNo(takerData.dastNo))
                    {
                        return "रजीस्टर्ड दस्त क्रमांक Field contains English / Marathi Letters / special characters! / Length is greater than 10";
                    }
                    if (methodForFile.CheckYrLen(takerData.dastNoYear))
                    {
                        return "वर्ष Field contains English / Marathi Letters / special characters! / Please enter valid year.";
                    }


                    // Insert POA Giver Data
                    if (takerData.selectedOptions != null)
                    {
                        foreach (GiverTakerInfoData data in takerData.selectedOptions)
                        {
                            PowerOfAttorneyInformationDataForGiver powerOfAttorneyInformationData = new PowerOfAttorneyInformationDataForGiver();
                            powerOfAttorneyInformationData = FetchPOAGiverDataByMutationID(data.code);
                            if (powerOfAttorneyInformationData != null)
                            {
                                string Response = SavePowerOfAttorneyGiver(powerOfAttorneyInformationData);
                                string[] po_giver_id = Response!.Split(",");
                                if (po_giver_id[0] == "Success")
                                {
                                    if (!string.IsNullOrEmpty(powerOfAttorneyInformationModel.poa_giver_ids) && !powerOfAttorneyInformationModel.poa_giver_ids.Contains(data.code.ToString()))
                                    {
                                        powerOfAttorneyInformationModel.poa_giver_ids = powerOfAttorneyInformationModel.poa_giver_ids + "," + po_giver_id[1].ToString();
                                    }
                                    else
                                    {
                                        powerOfAttorneyInformationModel.poa_giver_ids = po_giver_id[1].ToString();
                                    }
                                }
                                else
                                {
                                    throw new HandleException(Response);
                                }
                            }
                        }
                    }
                    //End

                    //Assign Value
                    powerOfAttorneyInformationModel.mobileno = "NA";
                    powerOfAttorneyInformationModel.mobilenoverified = "NA";
                    powerOfAttorneyInformationModel.emailid = "NA";
                    powerOfAttorneyInformationModel.emailidverified = "NA";
                    powerOfAttorneyInformationModel.usertype = "NA";
                    powerOfAttorneyInformationModel.usertype_code = 0;
                    powerOfAttorneyInformationModel.cts_number = "NA";
                    powerOfAttorneyInformationModel.owner_number = "NA";
                    powerOfAttorneyInformationModel.mutation_srno = "NA";
                    powerOfAttorneyInformationModel.village_code = "NA";
                    powerOfAttorneyInformationModel.village_name = "NA";
                    powerOfAttorneyInformationModel.prefix_in_eng = "NA";
                    powerOfAttorneyInformationModel.fname_in_eng = "NA";
                    powerOfAttorneyInformationModel.mname_in_eng = "NA";
                    powerOfAttorneyInformationModel.lname_in_eng = "NA";
                    powerOfAttorneyInformationModel.prefix_in_marathi = "NA";
                    powerOfAttorneyInformationModel.fname_in_marathi = "NA";
                    powerOfAttorneyInformationModel.mname_in_marathi = "NA";
                    powerOfAttorneyInformationModel.lname_in_marathi = "NA";
                    //New Fields For Taker
                    powerOfAttorneyInformationModel.company_name_in_marathi = "NA";
                    powerOfAttorneyInformationModel.company_name_in_eng = "NA";
                    //End
                    powerOfAttorneyInformationModel.username = "NA";
                    //New Fields for Taker
                    powerOfAttorneyInformationModel.alias_name = "NA";
                    powerOfAttorneyInformationModel.gender_code = "NA";
                    powerOfAttorneyInformationModel.gender_description = "NA";
                    powerOfAttorneyInformationModel.dob = "NA";
                    powerOfAttorneyInformationModel.mother_name_in_marathi = "NA";
                    powerOfAttorneyInformationModel.mother_name_in_eng = "NA";
                    powerOfAttorneyInformationModel.attornytype_desc = "NA";
                    powerOfAttorneyInformationModel.attornytype_code = 0;

                    powerOfAttorneyInformationModel.landBuyArea = "NA";
                    powerOfAttorneyInformationModel.address_type = "NA";
                    powerOfAttorneyInformationModel.address = "NA";
                    powerOfAttorneyInformationModel.state = "NA";
                    powerOfAttorneyInformationModel.district = "NA";
                    powerOfAttorneyInformationModel.taluka = "NA";
                    powerOfAttorneyInformationModel.city = "NA";
                    powerOfAttorneyInformationModel.flatno_plotno = "NA";
                    powerOfAttorneyInformationModel.societyname = "NA";
                    powerOfAttorneyInformationModel.mainstreet = "NA";
                    powerOfAttorneyInformationModel.landmark = "NA";
                    powerOfAttorneyInformationModel.locality = "NA";
                    powerOfAttorneyInformationModel.pincode = "NA";
                    powerOfAttorneyInformationModel.postofficename = "NA";
                    powerOfAttorneyInformationModel.address_proof_document_name = "NA";
                    powerOfAttorneyInformationModel.address_proof_document_path = "NA";

                    powerOfAttorneyInformationModel.owner_of_property_in_maharashtra = false;
                    // powerOfAttorneyInformationModel.PropertyTypeMaster = 0;
                    powerOfAttorneyInformationModel.property_district_code = "NA";
                    powerOfAttorneyInformationModel.property_district_name_in_marathi = "NA";
                    powerOfAttorneyInformationModel.property_district_name_in_english = "NA";
                    powerOfAttorneyInformationModel.property_taluka_code = "NA";
                    powerOfAttorneyInformationModel.property_taluka_name = "NA";
                    powerOfAttorneyInformationModel.property_city_code = "NA";
                    powerOfAttorneyInformationModel.property_city_name = "NA";
                    powerOfAttorneyInformationModel.khateno = "NA";
                    powerOfAttorneyInformationModel.ulpin = "NA";
                    powerOfAttorneyInformationModel.city_servey_no = "NA";
                    powerOfAttorneyInformationModel.lr_property_id = "NA";
                    powerOfAttorneyInformationModel.profile_pic_file_name = "NA";
                    powerOfAttorneyInformationModel.profile_pic_file_path = "NA";
                    powerOfAttorneyInformationModel.isPOAisPartofDast = "NA";
                    powerOfAttorneyInformationModel.isDeclerationInvolvedInPOA = "NA";
                    powerOfAttorneyInformationModel.isPOAPermanant = "NA";
                    powerOfAttorneyInformationModel.isTransferRights = "NA";
                    powerOfAttorneyInformationModel.dastNo = "NA";
                    powerOfAttorneyInformationModel.dastNoDate = "NA";
                    powerOfAttorneyInformationModel.dastNoYear = "NA";
                    powerOfAttorneyInformationModel.isDastVerified = false;
                    powerOfAttorneyInformationModel.verifiedDastData = "NA";
                    powerOfAttorneyInformationModel.digcode = 0;
                    powerOfAttorneyInformationModel.digname = "NA";
                    powerOfAttorneyInformationModel.poa_district_code = "0";
                    powerOfAttorneyInformationModel.poa_district_name = "NA";
                    powerOfAttorneyInformationModel.sro_office_code = 0;
                    powerOfAttorneyInformationModel.sro_office_name = "NA";
                    powerOfAttorneyInformationModel.signed_file_path = "NA";
                    powerOfAttorneyInformationModel.signed_file_name = "NA";
                    powerOfAttorneyInformationModel.prefixcode_marathi = "0";
                    powerOfAttorneyInformationModel.prefixcode_eng = "0";
                    powerOfAttorneyInformationModel.sub_property_id = "999999";


                    powerOfAttorneyInformationModel.usertype_code = takerData.usertype_code;
                    powerOfAttorneyInformationModel.usertype = takerData.usertype!.Trim().ToUpper();
                    //powerOfAttorneyInformationModel.profile_pic_file_name = takerData.photo.passportName;

                    powerOfAttorneyInformationModel.owner_of_property_in_maharashtra = (takerData.isMHProperty!.hasProperty!.Trim().ToUpper() == "YES") ? true : false;
                    PropertyTypeMaster proptype = _context.propertyTypes.FirstOrDefault(s => s.propertytypeid == Convert.ToInt32(takerData.isMHProperty.propType))!;
                    powerOfAttorneyInformationModel.PropertyTypeMaster = proptype;
                    if (powerOfAttorneyInformationModel.owner_of_property_in_maharashtra)
                    {
                        if (powerOfAttorneyInformationModel.PropertyTypeMaster.propertytypeid == 1 && string.IsNullOrEmpty(takerData.isMHProperty.userDetails!.khataNo))
                        {
                            return "When Property Type Is 7/12 Then Khate No Should Not Be Empty";
                        }
                        if (powerOfAttorneyInformationModel.PropertyTypeMaster.propertytypeid == 2 && string.IsNullOrEmpty(takerData.isMHProperty.userDetails!.naBhu))
                        {
                            return "When Property Type Is Property Card Then City Servey No Should Not Be Empty";
                        }
                        if (powerOfAttorneyInformationModel.PropertyTypeMaster.propertytypeid == 3 && string.IsNullOrEmpty(takerData.isMHProperty.userDetails!.ulpin))
                        {
                            return "When Property Type Is ULPIN Then ULPIN Should Not Be Empty";
                        }
                        powerOfAttorneyInformationModel.khateno = string.IsNullOrEmpty(takerData.isMHProperty.userDetails!.khataNo) ? "NA" : takerData.isMHProperty.userDetails.khataNo;
                        powerOfAttorneyInformationModel.city_servey_no = string.IsNullOrEmpty(takerData.isMHProperty.userDetails.naBhu) ? "NA" : takerData.isMHProperty.userDetails.naBhu;
                        powerOfAttorneyInformationModel.ulpin = string.IsNullOrEmpty(takerData.isMHProperty.userDetails.ulpin) ? "NA" : takerData.isMHProperty.userDetails.ulpin;
                        if (powerOfAttorneyInformationModel.PropertyTypeMaster.propertytypeid == 1)
                        {
                            powerOfAttorneyInformationModel.city_servey_no = "NA";
                            powerOfAttorneyInformationModel.ulpin = "NA";
                        }
                        else if (powerOfAttorneyInformationModel.PropertyTypeMaster.propertytypeid == 2)
                        {
                            powerOfAttorneyInformationModel.khateno = "NA";
                            powerOfAttorneyInformationModel.ulpin = "NA";
                        }
                        else if (powerOfAttorneyInformationModel.PropertyTypeMaster.propertytypeid == 3)
                        {
                            powerOfAttorneyInformationModel.khateno = "NA";
                            powerOfAttorneyInformationModel.city_servey_no = "NA";
                        }
                        powerOfAttorneyInformationModel.property_district_code = takerData.isMHProperty.userDetails.district!.district_code;
                        powerOfAttorneyInformationModel.property_district_name_in_marathi = takerData.isMHProperty.userDetails.district.district_name;
                        powerOfAttorneyInformationModel.property_district_name_in_english = takerData.isMHProperty.userDetails.district.district_english_name;

                        powerOfAttorneyInformationModel.property_taluka_code = takerData.isMHProperty.userDetails.taluka!.office_code;
                        powerOfAttorneyInformationModel.property_taluka_name = takerData.isMHProperty.userDetails.taluka.office_name;

                        powerOfAttorneyInformationModel.property_city_code = takerData.isMHProperty.userDetails.village!.village_code;
                        powerOfAttorneyInformationModel.property_city_name = takerData.isMHProperty.userDetails.village.village_name;
                    }
                    else
                    {
                        powerOfAttorneyInformationModel.khateno = "NA";
                        //Gouri
                        powerOfAttorneyInformationModel.city_servey_no = "NA";
                        powerOfAttorneyInformationModel.ulpin = "NA";
                        powerOfAttorneyInformationModel.property_district_code = "NA";
                        powerOfAttorneyInformationModel.property_district_name_in_marathi = "NA";
                        powerOfAttorneyInformationModel.property_district_name_in_english = "NA";
                        powerOfAttorneyInformationModel.property_taluka_code = "NA";
                        powerOfAttorneyInformationModel.property_taluka_name = "NA";
                        powerOfAttorneyInformationModel.property_city_code = "NA";
                        powerOfAttorneyInformationModel.property_city_name = "NA";
                    }
                    powerOfAttorneyInformationModel.username = takerData.isMHProperty.userDetails!.userName;
                    powerOfAttorneyInformationModel.prefixcode_marathi = takerData.isMHProperty.userDetails!.suffixcode;
                    powerOfAttorneyInformationModel.prefixcode_eng = takerData.isMHProperty.userDetails!.suffixCodeEng;

                    if (takerData.usertype_code == 1)
                    {
                        powerOfAttorneyInformationModel.prefix_in_marathi = takerData.isMHProperty.userDetails.suffix;
                        powerOfAttorneyInformationModel.fname_in_marathi = takerData.isMHProperty.userDetails.firstName;
                        powerOfAttorneyInformationModel.mname_in_marathi = takerData.isMHProperty.userDetails.middleName;
                        powerOfAttorneyInformationModel.lname_in_marathi = takerData.isMHProperty.userDetails.lastName;
                        powerOfAttorneyInformationModel.prefix_in_eng = takerData.isMHProperty.userDetails.suffixEng;
                        powerOfAttorneyInformationModel.fname_in_eng = takerData.isMHProperty.userDetails.firstNameEng;
                        powerOfAttorneyInformationModel.mname_in_eng = takerData.isMHProperty.userDetails.middleNameEng;
                        powerOfAttorneyInformationModel.lname_in_eng = takerData.isMHProperty.userDetails.lastNameEng;

                        powerOfAttorneyInformationModel.alias_name = takerData.dharak!.userdharak!.aliceName;
                        powerOfAttorneyInformationModel.gender_code = takerData.dharak.userdharak.gender!.gender_code;
                        powerOfAttorneyInformationModel.gender_description = takerData.dharak.userdharak.gender.gender_description;

                        // powerOfAttorneyInformationModel.khata_type_code = takerData.dharak.userdharak.khataType!.khataCode;
                        // powerOfAttorneyInformationModel.khata_type_name = takerData.dharak.userdharak.khataType.khataLabel;

                        //powerOfAttorneyInformationModel.holder_type_code = takerData.dharak.userdharak.holderType!.owner_status_code;
                        //powerOfAttorneyInformationModel.holder_type_name = takerData.dharak.userdharak.holderType.owner_status_description;

                        powerOfAttorneyInformationModel.dob = takerData.dharak.userdharak.dob;
                        powerOfAttorneyInformationModel.mother_name_in_marathi = takerData.dharak.userdharak.motherName!.Trim();
                        powerOfAttorneyInformationModel.mother_name_in_eng = takerData.dharak.userdharak.motherNameEng!.Trim();


                        powerOfAttorneyInformationModel.company_name_in_marathi = "NA";
                        powerOfAttorneyInformationModel.company_name_in_eng = "NA";
                        /*powerOfAttorneyInformationModel.apk_code = 0;
                        powerOfAttorneyInformationModel.apk_description = "NA";
                        powerOfAttorneyInformationModel.aapak = "NA";*/
                        powerOfAttorneyInformationModel.landBuyArea = "NA";
                    }
                    //if (takerData.usertype.Trim().ToUpper() == "COMPANY")
                    else
                    {
                        powerOfAttorneyInformationModel.company_name_in_marathi = takerData.isMHProperty.userDetails.companyName!.Trim();
                        powerOfAttorneyInformationModel.company_name_in_eng = takerData.isMHProperty.userDetails.companyNameEng!.Trim();
                        // powerOfAttorneyInformationModel.holder_type_code = takerData.dharak!.companydharak!.holderType!.owner_status_code;
                        //powerOfAttorneyInformationModel.holder_type_name = takerData.dharak.companydharak.holderType.owner_status_description;

                        //powerOfAttorneyInformationModel.khata_type_code = takerData.dharak.companydharak.khataType!.khataCode;
                        //powerOfAttorneyInformationModel.khata_type_name = takerData.dharak.companydharak.khataType.khataLabel;

                        /*powerOfAttorneyInformationModel.apk_code = takerData.dharak.companydharak!.aapakDropdown!.apk_code;
                        powerOfAttorneyInformationModel.apk_description = takerData.dharak.companydharak.aapakDropdown.apk_description;

                        powerOfAttorneyInformationModel.aapak = takerData.dharak.companydharak.aapak!.Trim();*/
                        powerOfAttorneyInformationModel.landBuyArea = takerData.dharak!.companydharak!.landBuyArea!.Trim();


                        powerOfAttorneyInformationModel.prefix_in_marathi = "NA";
                        powerOfAttorneyInformationModel.fname_in_marathi = "NA";
                        powerOfAttorneyInformationModel.mname_in_marathi = "NA";
                        powerOfAttorneyInformationModel.lname_in_marathi = "NA";
                        powerOfAttorneyInformationModel.prefix_in_eng = "NA";
                        powerOfAttorneyInformationModel.fname_in_eng = "NA";
                        powerOfAttorneyInformationModel.mname_in_eng = "NA";
                        powerOfAttorneyInformationModel.lname_in_eng = "NA";

                        powerOfAttorneyInformationModel.alias_name = "NA";
                        powerOfAttorneyInformationModel.gender_code = "NA";
                        powerOfAttorneyInformationModel.gender_description = "NA";
                        powerOfAttorneyInformationModel.dob = "NA";
                        powerOfAttorneyInformationModel.mother_name_in_marathi = "NA";
                        powerOfAttorneyInformationModel.mother_name_in_eng = "NA";
                    }
                    powerOfAttorneyInformationModel.address_type = takerData.address!.addressType!.Trim().ToUpper();
                    if (takerData.address.addressType.Trim().ToUpper() == "INDIA")
                    {

                        if (string.IsNullOrEmpty(takerData.address!.indiaAddress!.plotNo))
                        {
                            return "Please enter सदनिका / घर /प्लॉट नं.";
                        }
                        if (string.IsNullOrEmpty(takerData.address!.indiaAddress!.impSymbol))
                        {
                            return "Please enter महत्त्वाची खूण";
                        }
                        if (string.IsNullOrEmpty(takerData.address!.indiaAddress!.pincode))
                        {
                            return "Please enter पिन कोड";
                        }
                        if (!string.IsNullOrEmpty(takerData.address!.indiaAddress!.pincode) && methodForFile.CheckPinCode(takerData.address!.indiaAddress!.pincode))
                        {
                            if (string.IsNullOrEmpty(takerData.address!.indiaAddress!.postOfficeName))
                            {
                                return "Please select Post Office Name / Enter correct Pin Code.";
                            }
                            return "पिन कोड field contains special characters";
                        }

                        if (methodForFile.CheckIndianAddress(takerData.address!.indiaAddress!.plotNo))
                        {
                            return "सदनिका / घर /प्लॉट नं. field contains special characters";
                        }
                        if (!string.IsNullOrEmpty(takerData.address!.indiaAddress!.building!) && methodForFile.CheckIndianAddress(takerData.address!.indiaAddress!.building!))
                        {
                            return "इमारत (बिल्डिंग)/सोसायटी क्रमांक किंवा नाव field contains special characters";
                        }
                        if (!string.IsNullOrEmpty(takerData.address!.indiaAddress!.mainRoad!) && methodForFile.CheckIndianAddress(takerData.address!.indiaAddress!.mainRoad!))
                        {
                            return "मुख्य रस्ता field contains special characters";
                        }
                        if (methodForFile.CheckIndianAddress(takerData.address!.indiaAddress!.impSymbol!))
                        {
                            return "महत्त्वाची खूण field contains special characters";
                        }
                        if (!string.IsNullOrEmpty(takerData.address!.indiaAddress!.area!) && methodForFile.CheckIndianAddress(takerData.address!.indiaAddress!.area!))
                        {
                            return "महत्त्वाची खूण field contains special characters";
                        }
                        if (!string.IsNullOrEmpty(takerData.address!.indiaAddress!.mobile!) && methodForFile.CheckMobNo(takerData.address!.indiaAddress!.mobile!))
                        {
                            return "मोबाईल field contains special characters";
                        }

                        powerOfAttorneyInformationModel.state = takerData.address.indiaAddress!.state;
                        powerOfAttorneyInformationModel.district = takerData.address.indiaAddress.district;
                        powerOfAttorneyInformationModel.city = takerData.address.indiaAddress.city;
                        powerOfAttorneyInformationModel.taluka = takerData.address.indiaAddress.taluka;
                        powerOfAttorneyInformationModel.flatno_plotno = takerData.address.indiaAddress.plotNo;
                        powerOfAttorneyInformationModel.societyname = takerData.address.indiaAddress.building;
                        powerOfAttorneyInformationModel.mainstreet = takerData.address.indiaAddress.mainRoad;
                        powerOfAttorneyInformationModel.landmark = takerData.address.indiaAddress.impSymbol;
                        powerOfAttorneyInformationModel.locality = takerData.address.indiaAddress.area;
                        powerOfAttorneyInformationModel.pincode = takerData.address.indiaAddress.pincode;
                        powerOfAttorneyInformationModel.postofficename = takerData.address.indiaAddress.postOfficeName;
                        powerOfAttorneyInformationModel.mobileno = takerData.address.indiaAddress.mobile;
                        powerOfAttorneyInformationModel.mobilenoverified = takerData.address.indiaAddress.mobileOTP!.Trim().ToUpper();
                        powerOfAttorneyInformationModel.emailid = "NA";
                        powerOfAttorneyInformationModel.emailidverified = "NA";
                        powerOfAttorneyInformationModel.address = "NA";
                    }
                    else if (takerData.address.addressType.Trim().ToUpper() == "FOREIGN")
                    {


                        if (string.IsNullOrEmpty(takerData.address.foreignAddress!.address) || string.IsNullOrEmpty(takerData.address.foreignAddress!.email))
                        {
                            return "पत्ता and ई मेल field is mandatory";
                        }
                        if (methodForFile.CheckForeignAddress(takerData.address.foreignAddress!.address))
                        {
                            return "पत्ता field contains special characters";
                        }
                        if (methodForFile.CheckEmail(takerData.address.foreignAddress!.email))
                        {
                            return "ई मेल field contains special characters";
                        }

                        powerOfAttorneyInformationModel.address = takerData.address.foreignAddress!.address;
                        powerOfAttorneyInformationModel.mobileno = takerData.address.foreignAddress.mobile;
                        powerOfAttorneyInformationModel.mobilenoverified = "NO";
                        powerOfAttorneyInformationModel.emailid = takerData.address.foreignAddress.email;
                        powerOfAttorneyInformationModel.emailidverified = takerData.address.foreignAddress.emailOTP!.Trim().ToUpper();

                        powerOfAttorneyInformationModel.state = "NA";
                        powerOfAttorneyInformationModel.district = "NA";
                        powerOfAttorneyInformationModel.city = "NA";
                        powerOfAttorneyInformationModel.taluka = "NA";
                        powerOfAttorneyInformationModel.flatno_plotno = "NA";
                        powerOfAttorneyInformationModel.societyname = "NA";
                        powerOfAttorneyInformationModel.mainstreet = "NA";
                        powerOfAttorneyInformationModel.landmark = "NA";
                        powerOfAttorneyInformationModel.locality = "NA";
                        powerOfAttorneyInformationModel.pincode = "NA";
                        powerOfAttorneyInformationModel.postofficename = "NA";
                    }

                    ApplicationDTL applicationDTL = _context.applicationDTL.FirstOrDefault(s => s.applicationid == takerData.applicationid)!;
                    powerOfAttorneyInformationModel.applicationDTL = applicationDTL;

                    UserMaster userMaster = _context.userMasters.FirstOrDefault(s => s.userid == takerData.userid)!;
                    powerOfAttorneyInformationModel.userMaster = userMaster;

                    powerOfAttorneyInformationModel.mutation_id = takerData.mutation_id;

                    powerOfAttorneyInformationModel.attornytype_code = takerData.attornytype!.poa_type_code;
                    powerOfAttorneyInformationModel.attornytype_desc = takerData.attornytype.poa_type_description;
                    powerOfAttorneyInformationModel.dastNo = takerData.dastNo!;
                    powerOfAttorneyInformationModel.dastNoYear = takerData.dastNoYear!;
                    powerOfAttorneyInformationModel.dastNoDate = takerData.dastNoDate!;
                    powerOfAttorneyInformationModel.isDastVerified = takerData.isDastVarified;
                    powerOfAttorneyInformationModel.verifiedDastData = takerData.verifiedDastData!;
                    powerOfAttorneyInformationModel.isPOAisPartofDast = takerData.isPOAisPartofDast!;
                    powerOfAttorneyInformationModel.isDeclerationInvolvedInPOA = takerData.isDeclerationInvolvedInPOA!;
                    powerOfAttorneyInformationModel.isPOAPermanant = takerData.isPOAPermanant!;
                    powerOfAttorneyInformationModel.isTransferRights = takerData.isTransferRights!;
                    powerOfAttorneyInformationModel.digcode = takerData.division!.digcode!;
                    powerOfAttorneyInformationModel.digname = takerData.division.dig!;
                    powerOfAttorneyInformationModel.poa_district_code = takerData.district!.jdrcode.ToString()!;
                    powerOfAttorneyInformationModel.poa_district_name = takerData.district.jdr!;
                    powerOfAttorneyInformationModel.sro_office_code = Convert.ToInt32(takerData.registrar!.srocode);
                    powerOfAttorneyInformationModel.sro_office_name = takerData.registrar.sro!;
                    powerOfAttorneyInformationModel.mutation_id = takerData.mutation_id;
                    //Generate The Power Of Attorney Code
                    powerOfAttorneyInformationModel.power_of_attorney_code = GeneratePowerOfAttorneyCode(takerData.applicationid!, true);
                    //End

                    //Assign Default Value

                    dbTable.mobileno = "NA";
                    dbTable.mobilenoverified = "NA";
                    dbTable.emailid = "NA";
                    dbTable.emailidverified = "NA";
                    dbTable.usertype = "NA";
                    dbTable.usertype_code = 0;
                    dbTable.prefix_in_eng = "NA";
                    dbTable.fname_in_eng = "NA";
                    dbTable.mname_in_eng = "NA";
                    dbTable.lname_in_eng = "NA";
                    dbTable.prefix_in_marathi = "NA";
                    dbTable.fname_in_marathi = "NA";
                    dbTable.mname_in_marathi = "NA";
                    dbTable.lname_in_marathi = "NA";
                    //New Fields For Taker
                    dbTable.company_name_in_marathi = "NA";
                    dbTable.company_name_in_eng = "NA";
                    //End
                    dbTable.username = "NA";
                    //New Fields for Taker
                    dbTable.alias_name = "NA";
                    dbTable.gender_code = "NA";
                    dbTable.gender_description = "NA";
                    dbTable.dob = "NA";
                    dbTable.mother_name_in_marathi = "NA";
                    dbTable.mother_name_in_eng = "NA";
                    dbTable.attornytype_desc = "NA";
                    dbTable.attornytype_code = 0;

                    dbTable.landBuyArea = "NA";
                    dbTable.address_type = "NA";
                    dbTable.address = "NA";
                    dbTable.state = "NA";
                    dbTable.district = "NA";
                    dbTable.taluka = "NA";
                    dbTable.city = "NA";
                    dbTable.flatno_plotno = "NA";
                    dbTable.societyname = "NA";
                    dbTable.mainstreet = "NA";
                    dbTable.landmark = "NA";
                    dbTable.locality = "NA";
                    dbTable.pincode = "NA";
                    dbTable.postofficename = "NA";
                    dbTable.address_proof_document_name = "NA";
                    dbTable.address_proof_document_path = "NA";

                    dbTable.owner_of_property_in_maharashtra = false;
                    //dbTable.PropertyTypeMaster = 0;
                    dbTable.property_district_code = "NA";
                    dbTable.property_district_name_in_marathi = "NA";
                    dbTable.property_district_name_in_english = "NA";
                    dbTable.property_taluka_code = "NA";
                    dbTable.property_taluka_name = "NA";
                    dbTable.property_city_code = "NA";
                    dbTable.property_city_name = "NA";
                    dbTable.khateno = "NA";
                    dbTable.ulpin = "NA";
                    dbTable.city_servey_no = "NA";
                    dbTable.lr_property_id = "NA";
                    dbTable.profile_pic_file_name = "NA";
                    dbTable.profile_pic_file_path = "NA";
                    dbTable.isPOAisPartofDast = "NA";
                    dbTable.isDeclerationInvolvedInPOA = "NA";
                    dbTable.isPOAPermanant = "NA";
                    dbTable.isTransferRights = "NA";
                    dbTable.dast_no = "NA";
                    dbTable.dast_no_date = "NA";
                    dbTable.dast_no_year = "NA";
                    dbTable.isDastVerified = false;
                    dbTable.verifieddastData = "NA";
                    dbTable.digcode = 0;
                    dbTable.digname = "NA";
                    dbTable.poa_district_code = "0";
                    dbTable.poa_district_name = "NA";
                    dbTable.sro_office_code = 0;
                    dbTable.sro_office_name = "NA";
                    dbTable.signed_file_path = "NA";
                    dbTable.signed_file_name = "NA";
                    //Assign Data to Table fields to insert new records
                    dbTable.usertype_code = powerOfAttorneyInformationModel.usertype_code;
                    dbTable.mutation_id = Convert.ToInt32(powerOfAttorneyInformationModel.mutation_id!);
                    dbTable.usertype = powerOfAttorneyInformationModel.usertype;
                    dbTable.mobileno = powerOfAttorneyInformationModel.mobileno!;
                    dbTable.mobilenoverified = powerOfAttorneyInformationModel.mobilenoverified!;
                    dbTable.emailid = powerOfAttorneyInformationModel.emailid!;
                    dbTable.emailidverified = powerOfAttorneyInformationModel.emailidverified!;
                    dbTable.prefixcode_eng = powerOfAttorneyInformationModel.prefixcode_eng!;
                    dbTable.prefix_in_eng = powerOfAttorneyInformationModel.prefix_in_eng!.Trim();
                    dbTable.fname_in_eng = textInfo.ToTitleCase(powerOfAttorneyInformationModel.fname_in_eng!.Trim());
                    dbTable.mname_in_eng = textInfo.ToTitleCase(powerOfAttorneyInformationModel.mname_in_eng!.Trim());
                    dbTable.lname_in_eng = textInfo.ToTitleCase(powerOfAttorneyInformationModel.lname_in_eng!.Trim());
                    dbTable.prefixcode_marathi = powerOfAttorneyInformationModel.prefixcode_marathi!;
                    dbTable.prefix_in_marathi = powerOfAttorneyInformationModel.prefix_in_marathi!.Trim();
                    dbTable.fname_in_marathi = powerOfAttorneyInformationModel.fname_in_marathi!.Trim();
                    dbTable.mname_in_marathi = powerOfAttorneyInformationModel.mname_in_marathi!.Trim();
                    dbTable.lname_in_marathi = powerOfAttorneyInformationModel.lname_in_marathi!.Trim();
                    dbTable.address_type = powerOfAttorneyInformationModel.address_type;
                    dbTable.address = string.IsNullOrEmpty(powerOfAttorneyInformationModel.address) ? "NA" : powerOfAttorneyInformationModel.address;
                    dbTable.state = powerOfAttorneyInformationModel.state!;
                    dbTable.district = powerOfAttorneyInformationModel.district!;
                    dbTable.taluka = powerOfAttorneyInformationModel.taluka!;
                    dbTable.city = powerOfAttorneyInformationModel.city!;
                    dbTable.flatno_plotno = powerOfAttorneyInformationModel.flatno_plotno!;
                    dbTable.societyname = powerOfAttorneyInformationModel.societyname!;
                    dbTable.mainstreet = powerOfAttorneyInformationModel.mainstreet!;
                    dbTable.landmark = powerOfAttorneyInformationModel.landmark!;
                    dbTable.locality = powerOfAttorneyInformationModel.locality!;
                    dbTable.pincode = powerOfAttorneyInformationModel.pincode!;
                    dbTable.postofficename = powerOfAttorneyInformationModel.postofficename!;
                    dbTable.owner_of_property_in_maharashtra = powerOfAttorneyInformationModel.owner_of_property_in_maharashtra;
                    dbTable.propertyType = powerOfAttorneyInformationModel.PropertyTypeMaster;
                    dbTable.property_district_code = powerOfAttorneyInformationModel.property_district_code;
                    dbTable.property_district_name_in_marathi = powerOfAttorneyInformationModel.property_district_name_in_marathi;
                    dbTable.property_district_name_in_english = powerOfAttorneyInformationModel.property_district_name_in_english;

                    dbTable.property_taluka_code = powerOfAttorneyInformationModel.property_taluka_code;
                    dbTable.property_taluka_name = powerOfAttorneyInformationModel.property_taluka_name;
                    dbTable.property_city_code = powerOfAttorneyInformationModel.property_city_code;
                    dbTable.property_city_name = powerOfAttorneyInformationModel.property_city_name;

                    dbTable.khateno = powerOfAttorneyInformationModel.khateno;
                    dbTable.city_servey_no = powerOfAttorneyInformationModel.city_servey_no;
                    dbTable.ulpin = powerOfAttorneyInformationModel.ulpin;
                    dbTable.username = string.IsNullOrEmpty(powerOfAttorneyInformationModel.username) ? "NA" : powerOfAttorneyInformationModel.username;
                    dbTable.company_name_in_eng = powerOfAttorneyInformationModel.company_name_in_eng;
                    dbTable.company_name_in_marathi = powerOfAttorneyInformationModel.company_name_in_marathi;
                    dbTable.userMaster = powerOfAttorneyInformationModel.userMaster;
                    dbTable.applicationDTL = powerOfAttorneyInformationModel.applicationDTL;
                    dbTable.is_taker = true;

                    dbTable.attornytype_code = powerOfAttorneyInformationModel.attornytype_code;
                    dbTable.attornytype_desc = powerOfAttorneyInformationModel.attornytype_desc;


                    dbTable.isPOAisPartofDast = powerOfAttorneyInformationModel.isPOAisPartofDast.ToUpper();
                    dbTable.isDeclerationInvolvedInPOA = powerOfAttorneyInformationModel.isDeclerationInvolvedInPOA.ToUpper();
                    dbTable.isPOAPermanant = powerOfAttorneyInformationModel.isPOAPermanant.ToUpper();
                    dbTable.isTransferRights = powerOfAttorneyInformationModel.isTransferRights.ToUpper();
                    dbTable.dast_no = powerOfAttorneyInformationModel.dastNo;
                    dbTable.dast_no_date = powerOfAttorneyInformationModel.dastNoDate;
                    dbTable.dast_no_year = powerOfAttorneyInformationModel.dastNoYear;
                    dbTable.isDastVerified = powerOfAttorneyInformationModel.isDastVerified;
                    dbTable.verifieddastData = powerOfAttorneyInformationModel.verifiedDastData!;
                    dbTable.digcode = powerOfAttorneyInformationModel.digcode;
                    dbTable.digname = powerOfAttorneyInformationModel.digname;
                    dbTable.poa_district_code = powerOfAttorneyInformationModel.poa_district_code;
                    dbTable.poa_district_name = powerOfAttorneyInformationModel.poa_district_name;
                    // dbTable.poa_district_name_in_eng = powerOfAttorneyInformationModel.poa_district_name_in_eng;
                    dbTable.sro_office_code = powerOfAttorneyInformationModel.sro_office_code;
                    dbTable.sro_office_name = powerOfAttorneyInformationModel.sro_office_name;

                    dbTable.alias_name = powerOfAttorneyInformationModel.alias_name!;
                    dbTable.gender_code = powerOfAttorneyInformationModel.gender_code!;
                    dbTable.gender_description = powerOfAttorneyInformationModel.gender_description!.Trim().ToUpper();
                    /*dbTable.khata_type_code = powerOfAttorneyInformationModel.khata_type_code!;
                    dbTable.khata_type_name = powerOfAttorneyInformationModel.khata_type_name!;

                    dbTable.owner_status_code = powerOfAttorneyInformationModel.holder_type_code!;
                    dbTable.owner_status_description = powerOfAttorneyInformationModel.holder_type_name!;*/

                    dbTable.dob = powerOfAttorneyInformationModel.dob!;
                    dbTable.mother_name_in_marathi = powerOfAttorneyInformationModel.mother_name_in_marathi.Trim();
                    dbTable.mother_name_in_eng = textInfo.ToTitleCase(powerOfAttorneyInformationModel.mother_name_in_eng.Trim());
                    /* dbTable.apk_code = powerOfAttorneyInformationModel.apk_code;
                     dbTable.apk_description = powerOfAttorneyInformationModel.apk_description;

                     dbTable.aapak = powerOfAttorneyInformationModel.aapak;*/
                    dbTable.landBuyArea = powerOfAttorneyInformationModel.landBuyArea;
                    dbTable.lr_property_id = "NA";
                    dbTable.power_of_attorney_code = powerOfAttorneyInformationModel.power_of_attorney_code;
                    dbTable.address_proof_document_name = "NA";
                    dbTable.address_proof_document_path = "NA";
                    dbTable.signed_file_name = "NA";
                    dbTable.signed_file_path = "NA";
                    dbTable.profile_pic_file_name = "NA";
                    dbTable.profile_pic_file_path = "NA";
                    dbTable.sub_property_no = powerOfAttorneyInformationModel.sub_property_id;
                    dbTable.cts_number = powerOfAttorneyInformationModel.cts_number;
                    dbTable.owner_number = powerOfAttorneyInformationModel.owner_number;
                    dbTable.mutation_srno = powerOfAttorneyInformationModel.mutation_srno;
                    dbTable.village_code = powerOfAttorneyInformationModel.village_code;
                    dbTable.village_name = powerOfAttorneyInformationModel.village_name;
                    dbTable.poa_giver_ids = powerOfAttorneyInformationModel.poa_giver_ids!;
                    _context.powerOfAttorneyInformation.Add(dbTable);
                    _context.SaveChanges();

                    //Get Saved Row ID
                    int power_of_attorney_id = dbTable.power_of_attorney_id;

                    string VerificationType = string.Empty;
                    bool checkAddressFlag = true;
                    //bool checkSignFlag = false;
                    string CurrentDateTime = DateTime.Now.ToString("yyyy-MMM-dd-HHmmss");
                    if (powerOfAttorneyInformationModel.address_type == "INDIA")
                    {
                        VerificationType = "MOBILENO";
                        if (!string.IsNullOrEmpty(takerData.address.indiaAddress!.addressProofSrc) && !string.IsNullOrEmpty(takerData.address.indiaAddress.addressProofName))
                        {
                            string[] AddressData = takerData.address.indiaAddress.addressProofSrc.Split(",");
                            checkAddressFlag = methodForFile.SaveImageForApplicant(AddressData[1], takerData.address.indiaAddress.addressProofName, power_of_attorney_id.ToString(), "AddressProof", FolderPath + @"\", CurrentDateTime);
                            string AddressProofExt = Path.GetExtension(takerData.address.indiaAddress.addressProofName);
                            powerOfAttorneyInformationModel.address_proof_document_name = "AddressProof" + power_of_attorney_id + "_" + CurrentDateTime + AddressProofExt;
                            powerOfAttorneyInformationModel.address_proof_document_path = FolderPath + power_of_attorney_id + @"\" + powerOfAttorneyInformationModel.address_proof_document_name;

                            var UpdateAddressFilePath = _context.powerOfAttorneyInformation.Where(w => w.power_of_attorney_id == power_of_attorney_id).FirstOrDefault();
                            if (UpdateAddressFilePath != null)
                            {
                                dbTable.address_proof_document_name = powerOfAttorneyInformationModel.address_proof_document_name;
                                dbTable.address_proof_document_path = powerOfAttorneyInformationModel.address_proof_document_path;
                                _context.Entry(dbTable).CurrentValues.SetValues(dbTable);
                                _context.SaveChanges();
                            }
                        }


                        //string[] signData = takerData.address.indiaAddress.signatureSrc.Split(",");
                        //checkSignFlag = methodForFile.SaveImageForApplicant(signData[1], takerData.address.indiaAddress.signatureName, power_of_attorney_id.ToString(), "Signature", FolderPath + @"\", CurrentDateTime);
                        //string SignatureExt = Path.GetExtension(takerData.address.indiaAddress.signatureName);
                        //powerOfAttorneyInformationModel.signed_file_name = "Signature" + power_of_attorney_id + "_" + CurrentDateTime + SignatureExt;
                        //powerOfAttorneyInformationModel.signed_file_path = FolderPath + @"\" + power_of_attorney_id + @"\" + powerOfAttorneyInformationModel.signed_file_name;

                        //var UpdateSignFilePath = _context.powerOfAttorneyInformation.Where(w => w.power_of_attorney_id == power_of_attorney_id).FirstOrDefault();
                        //if (UpdateSignFilePath != null)
                        //{
                        //    dbTable.signed_file_name = powerOfAttorneyInformationModel.signed_file_name;
                        //    dbTable.signed_file_path = powerOfAttorneyInformationModel.signed_file_path;
                        //    _context.Entry(dbTable).CurrentValues.SetValues(dbTable);
                        //    _context.SaveChanges();
                        //}
                    }
                    //if (powerOfAttorneyInformationModel.address_type == "FOREIGN")
                    //{
                    //    VerificationType = "EMAILID";
                    //    // applicantID = FetchApplicantID(applicantMasterModel.emailid, VerificationType);

                    //    string[] signData = takerData.address.foreignAddress.signatureSrc.Split(",");
                    //    checkSignFlag = methodForFile.SaveImageForApplicant(signData[1], takerData.address.foreignAddress.signatureName, power_of_attorney_id.ToString(), "Signature", FolderPath + @"\", CurrentDateTime);

                    //    string SignatureExt = Path.GetExtension(takerData.address.foreignAddress.signatureName);
                    //    powerOfAttorneyInformationModel.signed_file_name = "Signature" + power_of_attorney_id + "_" + CurrentDateTime + SignatureExt;
                    //    powerOfAttorneyInformationModel.signed_file_path = FolderPath + @"\" + power_of_attorney_id + @"\" + powerOfAttorneyInformationModel.signed_file_name;
                    //    var UpdateSignFilePath = _context.powerOfAttorneyInformation.Where(w => w.power_of_attorney_id == power_of_attorney_id).FirstOrDefault();
                    //    if (UpdateSignFilePath != null)
                    //    {
                    //        dbTable.signed_file_name = powerOfAttorneyInformationModel.signed_file_name;
                    //        dbTable.signed_file_path = powerOfAttorneyInformationModel.signed_file_path;
                    //        _context.Entry(dbTable).CurrentValues.SetValues(dbTable);
                    //        _context.SaveChanges();
                    //    }
                    //}
                    //string[] imgData = takerData.photo.passportSrc.Split(",");
                    //bool checkImgFlag = methodForFile.SaveImageForApplicant(imgData[1], takerData.photo.passportName, power_of_attorney_id.ToString(), "PassportPhoto", FolderPath + @"\", CurrentDateTime);
                    //string ImgExt = Path.GetExtension(takerData.photo.passportName);
                    //powerOfAttorneyInformationModel.profile_pic_file_name = "PassportPhoto" + power_of_attorney_id + "_" + CurrentDateTime + ImgExt;
                    //powerOfAttorneyInformationModel.profile_pic_file_path = FolderPath + @"\" + power_of_attorney_id + @"\" + powerOfAttorneyInformationModel.profile_pic_file_name;

                    //var UpdateImgFilePath = _context.powerOfAttorneyInformation.Where(w => w.power_of_attorney_id == power_of_attorney_id).FirstOrDefault();
                    //if (UpdateImgFilePath != null)
                    //{
                    //    dbTable.profile_pic_file_name = powerOfAttorneyInformationModel.profile_pic_file_name;
                    //    dbTable.profile_pic_file_path = powerOfAttorneyInformationModel.profile_pic_file_path;
                    //    _context.Entry(dbTable).CurrentValues.SetValues(dbTable);
                    //    _context.SaveChanges();
                    //}

                    //if (checkAddressFlag && checkImgFlag && checkSignFlag)
                    if (checkAddressFlag)
                    {
                        var applicationDTLdata = _context.applicationDTL.Where(data => data.applicationid!.Equals(takerData.applicationid)).FirstOrDefault();
                        if (applicationDTLdata != null)
                        {
                            if (!string.IsNullOrEmpty(applicationDTLdata.powerOfAttorneyIDs) && !applicationDTLdata.powerOfAttorneyIDs.Contains(power_of_attorney_id.ToString()))
                            {
                                applicationDTLdata.powerOfAttorneyIDs = applicationDTLdata.powerOfAttorneyIDs + "," + power_of_attorney_id.ToString();
                            }
                            else
                            {
                                applicationDTLdata.powerOfAttorneyIDs = power_of_attorney_id.ToString();
                            }
                            applicationDTLdata.status = 6;
                            _context.Entry(applicationDTLdata).CurrentValues.SetValues(applicationDTLdata);
                            _context.SaveChanges();
                        }
                        _context.SaveChanges();
                        scope.Complete();
                        return "Success";
                    }
                    else
                    {
                        _context.powerOfAttorneyInformation.Remove(dbTable);
                        _context.SaveChanges();
                        _context.Dispose();
                        return "Address Proof File Is Not Uploaded";
                    }
                    //if (!checkImgFlag)
                    //{
                    //    dbContextTransaction.Rollback();
                    //    return "Profile Picture Is Not Uploaded";
                    //}
                    //if (!checkAddressFlag)
                    //{
                    //    dbContextTransaction.Rollback();
                    //    return "Address Proof File Is Not Uploaded";
                    //}
                    //if (!checkSignFlag)
                    //{
                    //    dbContextTransaction.Rollback();
                    //    return "Signature File Is Not Uploaded";
                    //}
                    //else
                    //{
                    //    dbContextTransaction.Rollback();
                    //    return "Some Files Are Not Uploaded";
                    //}
                }
                catch (Exception ex)
                {
                    _context.powerOfAttorneyInformation.Remove(dbTable);
                    _context.SaveChanges();
                    _context.Dispose();
                    throw new HandleException(ex.Message.ToString());
                }
            }
        }

        public FetchPOAForTakerData FetchPowerOfAttorneyInfoForTaker(int powerOfAttorneyID)
        {
            try
            {
                MethodForFileUpload methodForFile = new MethodForFileUpload();
                PowerOfAttorneyInformation powerOfAttorneyInformation = new PowerOfAttorneyInformation();
                powerOfAttorneyInformation = _context.powerOfAttorneyInformation.Include(i => i.userMaster).Include(app => app.applicationDTL).Include(proptype => proptype.propertyType).Where(data => data.power_of_attorney_id.Equals(powerOfAttorneyID) && data.is_taker == true && data.isDeleted == false).FirstOrDefault()!;

                FetchPOAForTakerData fetchData = new FetchPOAForTakerData();
                List<string> giverNameInMarathiList = new List<string>();
                List<string> giverNameInEnglishList = new List<string>();
                if (powerOfAttorneyInformation != null)
                {
                    fetchData.power_of_attorney_id = powerOfAttorneyInformation.power_of_attorney_id;
                    fetchData.userid = powerOfAttorneyInformation.userMaster!.userid;
                    fetchData.applicationid = powerOfAttorneyInformation.applicationDTL!.applicationid;

                    fetchData.mobileNo = powerOfAttorneyInformation.mobileno;
                    fetchData.is_taker = powerOfAttorneyInformation.is_taker;
                    ViewModel.DharakForPOATaker dharakdata = new ViewModel.DharakForPOATaker();
                    fetchData.usertype_code = powerOfAttorneyInformation.usertype_code;
                    fetchData.usertype = powerOfAttorneyInformation.usertype;

                    if (fetchData.is_taker)
                    {
                        if (fetchData.usertype_code == 1)
                        {
                            fetchData.fullNameInMarathi = powerOfAttorneyInformation.fname_in_marathi.Trim() + " " + powerOfAttorneyInformation.mname_in_marathi.Trim() + " " + powerOfAttorneyInformation.lname_in_marathi.Trim();
                            fetchData.fullNameInEng = powerOfAttorneyInformation.fname_in_eng.Trim() + " " + powerOfAttorneyInformation.mname_in_eng.Trim() + " " + powerOfAttorneyInformation.lname_in_eng.Trim();
                        }
                        else
                        {
                            fetchData.fullNameInMarathi = powerOfAttorneyInformation.company_name_in_marathi.Trim();
                            fetchData.fullNameInEng = powerOfAttorneyInformation.company_name_in_eng.Trim();
                        }
                    }
                    //else
                    //{
                    //    fetchData.fullNameInMarathi = powerOfAttorneyInformation.fname_in_marathi.Trim() + " " + powerOfAttorneyInformation.mname_in_marathi.Trim() + " " + powerOfAttorneyInformation.lname_in_marathi.Trim();
                    //    fetchData.fullNameInEng = powerOfAttorneyInformation.fname_in_eng.Trim() + " " + powerOfAttorneyInformation.mname_in_eng.Trim() + " " + powerOfAttorneyInformation.lname_in_eng.Trim();
                    //}

                    if (fetchData.usertype_code == 1)
                    {
                        ViewModel.UserDharakForPOATaker userDharakData = new ViewModel.UserDharakForPOATaker();
                        userDharakData.aliceName = powerOfAttorneyInformation.alias_name;
                        Gender gender = new Gender();
                        gender.gender_code = powerOfAttorneyInformation.gender_code;
                        gender.gender_description = powerOfAttorneyInformation.gender_description;
                        userDharakData.gender = gender;
                        /*KhataTypeData khataTypeData = new KhataTypeData();
                        khataTypeData.khataCode = powerOfAttorneyInformation.khata_type_code;
                        khataTypeData.khataLabel = powerOfAttorneyInformation.khata_type_name;
                        userDharakData.khataType = khataTypeData;*/

                        /*HoldertypeData holdertypeData = new HoldertypeData();
                        holdertypeData.owner_status_code = powerOfAttorneyInformation.owner_status_code;
                        holdertypeData.owner_status_description = powerOfAttorneyInformation.owner_status_description;
                        userDharakData.holderType = holdertypeData;*/

                        userDharakData.dob = powerOfAttorneyInformation.dob;
                        userDharakData.motherName = powerOfAttorneyInformation.mother_name_in_marathi;
                        userDharakData.motherNameEng = powerOfAttorneyInformation.mother_name_in_eng;
                        dharakdata.userdharak = userDharakData;
                    }
                    else
                    {
                        ViewModel.CompanyDharakForPOATaker companyDharakData = new ViewModel.CompanyDharakForPOATaker();
                        /* KhataTypeData khataTypeData = new KhataTypeData();
                         khataTypeData.khataCode = powerOfAttorneyInformation.khata_type_code;
                         khataTypeData.khataLabel = powerOfAttorneyInformation.khata_type_name;
                         companyDharakData.khataType = khataTypeData;

                         HoldertypeData holdertypeData = new HoldertypeData();
                         holdertypeData.owner_status_code = powerOfAttorneyInformation.owner_status_code;
                         holdertypeData.owner_status_description = powerOfAttorneyInformation.owner_status_description;
                         companyDharakData.holderType = holdertypeData;

                         aapakDropdownData aapakDropdownData = new aapakDropdownData();
                         aapakDropdownData.apk_code = powerOfAttorneyInformation.apk_code;
                         aapakDropdownData.apk_description = powerOfAttorneyInformation.apk_description;
                         companyDharakData.aapakDropdown = aapakDropdownData;

                         companyDharakData.aapak = powerOfAttorneyInformation.aapak;*/
                        companyDharakData.landBuyArea = powerOfAttorneyInformation.landBuyArea;
                        dharakdata.companydharak = companyDharakData;
                    }
                    fetchData.dharak = dharakdata;
                    ViewModel.UserDetailsDataForPOATaker userDetails = new ViewModel.UserDetailsDataForPOATaker();
                    userDetails.suffix = powerOfAttorneyInformation.prefix_in_marathi;
                    userDetails.firstName = powerOfAttorneyInformation.fname_in_marathi;
                    userDetails.middleName = powerOfAttorneyInformation.mname_in_marathi;
                    userDetails.lastName = powerOfAttorneyInformation.lname_in_marathi;
                    userDetails.suffixEng = powerOfAttorneyInformation.prefix_in_eng;
                    userDetails.firstNameEng = powerOfAttorneyInformation.fname_in_eng;
                    userDetails.middleNameEng = powerOfAttorneyInformation.mname_in_eng;
                    userDetails.lastNameEng = powerOfAttorneyInformation.lname_in_eng;
                    userDetails.nabhu = powerOfAttorneyInformation.city_servey_no;
                    userDetails.userName = powerOfAttorneyInformation.username;
                    userDetails.lrPropertyUID = powerOfAttorneyInformation.lr_property_id;
                    userDetails.suffixcode = powerOfAttorneyInformation.prefixcode_marathi;
                    userDetails.suffixCodeEng = powerOfAttorneyInformation.prefixcode_eng;

                    if (powerOfAttorneyInformation.owner_of_property_in_maharashtra)
                    {
                        DistrictForPOATaker district = new DistrictForPOATaker();
                        district.district_code = powerOfAttorneyInformation.property_district_code;
                        district.district_name = powerOfAttorneyInformation.property_district_name_in_marathi;
                        district.district_english_name = powerOfAttorneyInformation.property_district_name_in_english;
                        userDetails.district = district;

                        TalukaForPOATaker taluka = new TalukaForPOATaker();
                        taluka.office_code = powerOfAttorneyInformation.property_taluka_code;
                        taluka.office_name = powerOfAttorneyInformation.property_taluka_name;
                        userDetails.taluka = taluka;

                        VillageForPOATaker village = new VillageForPOATaker();
                        village.village_code = powerOfAttorneyInformation.property_city_code;
                        village.village_name = powerOfAttorneyInformation.property_city_name;
                        userDetails.village = village;
                    }

                    fetchData.userDetails = userDetails;
                    ViewModel.AddressDataForPOATaker addressData = new ViewModel.AddressDataForPOATaker();
                    addressData.addressType = powerOfAttorneyInformation.address_type;
                    if (powerOfAttorneyInformation.address_type == "INDIA")
                    {
                        ViewModel.AddressForIndiaForPOATaker addressForIndia = new ViewModel.AddressForIndiaForPOATaker();
                        addressForIndia.state = powerOfAttorneyInformation.state;
                        addressForIndia.district = powerOfAttorneyInformation.district;
                        addressForIndia.city = powerOfAttorneyInformation.city;
                        addressForIndia.taluka = powerOfAttorneyInformation.taluka;
                        addressForIndia.plotNo = powerOfAttorneyInformation.flatno_plotno;
                        addressForIndia.building = powerOfAttorneyInformation.societyname;
                        addressForIndia.mainRoad = powerOfAttorneyInformation.mainstreet;
                        addressForIndia.impSymbol = powerOfAttorneyInformation.landmark;
                        addressForIndia.area = powerOfAttorneyInformation.locality;
                        addressForIndia.pincode = powerOfAttorneyInformation.pincode;
                        addressForIndia.postOfficeName = powerOfAttorneyInformation.postofficename;
                        addressForIndia.addressProofName = powerOfAttorneyInformation.address_proof_document_name;
                        addressForIndia.mobile = powerOfAttorneyInformation.mobileno;
                        addressForIndia.mobileOTP = powerOfAttorneyInformation.mobilenoverified;
                        addressForIndia.signatureName = powerOfAttorneyInformation.signed_file_name;

                        if (powerOfAttorneyInformation.address_proof_document_path != "NA")
                        {
                            string AddressProofExt = Path.GetExtension(powerOfAttorneyInformation.address_proof_document_path);
                            string AddressProof = methodForFile.ConvertImageToBase64(powerOfAttorneyInformation.address_proof_document_path);
                            powerOfAttorneyInformation.address_proof_document_path = string.IsNullOrEmpty(AddressProof) ? "NA" : "data:image/" + AddressProofExt.Replace(".", "") + ";base64," + AddressProof;
                            addressForIndia.addressProofSrc = powerOfAttorneyInformation.address_proof_document_path;
                        }
                        else
                        {
                            addressForIndia.addressProofSrc = powerOfAttorneyInformation.address_proof_document_path;
                        }

                        //string SignatureExt = Path.GetExtension(powerOfAttorneyInformation.signed_file_path);
                        //string Signature = methodForFile.ConvertImageToBase64(powerOfAttorneyInformation.signed_file_path);
                        //addressForIndia.signatureSrc = string.IsNullOrEmpty(Signature) ? "NA" : "data:image/" + SignatureExt.Replace(".", "") + ";base64," + Signature;
                        addressForIndia.signatureSrc = powerOfAttorneyInformation.signed_file_path;
                        addressData.indiaAddress = addressForIndia;
                    }
                    else if (powerOfAttorneyInformation.address_type == "FOREIGN")
                    {
                        ViewModel.AddressForForeignForPOATaker addressForForeign = new ViewModel.AddressForForeignForPOATaker();
                        addressForForeign.address = powerOfAttorneyInformation.address;
                        addressForForeign.mobile = powerOfAttorneyInformation.mobileno;
                        addressForForeign.email = powerOfAttorneyInformation.emailid;
                        addressForForeign.emailOTP = powerOfAttorneyInformation.emailidverified;
                        addressForForeign.signatureName = powerOfAttorneyInformation.signed_file_name;

                        //string SignatureExt = Path.GetExtension(powerOfAttorneyInformation.signed_file_path);
                        //string Signature = methodForFile.ConvertImageToBase64(powerOfAttorneyInformation.signed_file_path);
                        //addressForForeign.signatureSrc= string.IsNullOrEmpty(Signature) ? "NA" : "data:image/" + SignatureExt.Replace(".", "") + ";base64," + Signature;
                        addressForForeign.signatureSrc = powerOfAttorneyInformation.signed_file_path;
                        addressData.foreignAddress = addressForForeign;
                    }
                    fetchData.address = addressData;

                    AttroneyTypeForPOATaker attornytype = new AttroneyTypeForPOATaker();
                    attornytype.poa_type_code = powerOfAttorneyInformation.attornytype_code;
                    attornytype.poa_type_description = powerOfAttorneyInformation.attornytype_desc;
                    fetchData.attornytype = attornytype;

                    Division division = new Division();
                    division.digcode = powerOfAttorneyInformation.digcode;
                    division.dig = powerOfAttorneyInformation.digname;
                    fetchData.division = division;

                    DastDistrict districtData = new DastDistrict();
                    districtData.jdrcode = Convert.ToInt32(powerOfAttorneyInformation.poa_district_code);
                    districtData.jdr = powerOfAttorneyInformation.poa_district_name;
                    //  districtData.district_english_name = powerOfAttorneyInformation.poa_district_name_in_eng;
                    fetchData.district = districtData;

                    Dastregsiter registrarData = new Dastregsiter();
                    registrarData.srocode = powerOfAttorneyInformation.sro_office_code;
                    registrarData.sro = powerOfAttorneyInformation.sro_office_name;
                    fetchData.registrar = registrarData;

                    fetchData.dastNo = powerOfAttorneyInformation.dast_no;
                    fetchData.dastNoDate = powerOfAttorneyInformation.dast_no_date;
                    fetchData.dastNoYear = powerOfAttorneyInformation.dast_no_year;
                    fetchData.isDastVarified = powerOfAttorneyInformation.isDastVerified;
                    fetchData.varifiedDastData = powerOfAttorneyInformation.verifieddastData;
                    fetchData.isPOAisPartofDast = powerOfAttorneyInformation.isPOAisPartofDast;
                    fetchData.isDeclerationInvolvedInPOA = powerOfAttorneyInformation.isDeclerationInvolvedInPOA;
                    fetchData.isPOAPermanant = powerOfAttorneyInformation.isPOAPermanant;
                    fetchData.isTransferRights = powerOfAttorneyInformation.isTransferRights;
                    fetchData.passportName = powerOfAttorneyInformation.profile_pic_file_name;
                    fetchData.passportSrc = powerOfAttorneyInformation.profile_pic_file_path;
                    fetchData.poa_giver_ids = powerOfAttorneyInformation.poa_giver_ids;
                    //if (!string.IsNullOrEmpty(powerOfAttorneyInformation.profile_pic_file_name) && powerOfAttorneyInformation.profile_pic_file_name != "NA")
                    //{
                    //    fetchData.passportName = powerOfAttorneyInformation.profile_pic_file_name;
                    //    string ProfilePicExt = Path.GetExtension(powerOfAttorneyInformation.profile_pic_file_path);
                    //    string ProfilePic = methodForFile.ConvertImageToBase64(powerOfAttorneyInformation.profile_pic_file_path);
                    //    powerOfAttorneyInformation.profile_pic_file_path = string.IsNullOrEmpty(ProfilePic) ? "NA" : "data:image/" + ProfilePicExt.Replace(".", "") + ";base64," + ProfilePic;
                    //    fetchData.passportSrc = powerOfAttorneyInformation.profile_pic_file_path;
                    //}
                    //else
                    //{
                    //    fetchData.passportName = powerOfAttorneyInformation.profile_pic_file_name;
                    //    fetchData.passportSrc = powerOfAttorneyInformation.profile_pic_file_path;
                    //}
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
        //public string SavePowerOfAttorneyTaker(PowerOfAttorneyInformationDataForTaker takerData)
        //{
        //    using (var dbContextTransaction = _context.Database.BeginTransaction())
        //    //  using (var scope = new TransactionScope(TransactionScopeOption.Suppress))
        //    {
        //        MethodForFileUpload methodForFile = new MethodForFileUpload();
        //        PowerOfAttorneyInformation dbTable = new PowerOfAttorneyInformation();
        //        // Assign values to model
        //        PowerOfAttorneyInformationModel powerOfAttorneyInformationModel = new PowerOfAttorneyInformationModel();
        //        string FolderPath = @"\\10.10.248.2\pde_propertycard\MUTATIONDOCS\" + takerData.applicationid + @"\POWEROFATTORNEY\TAKER\";
        //        //string FolderPath = @"\\10.10.248.2\pde_propertycard\MUTATIONDOCS\" + takerData.applicationid + @"\POWEROFATTORNEY\TAKER\";
        //        try
        //        {
        //            // Insert POA Giver Data
        //            if (takerData.selectedOptions != null)
        //            {
        //                foreach (GiverTakerInfoData data in takerData.selectedOptions)
        //                {
        //                    PowerOfAttorneyInformationDataForGiver powerOfAttorneyInformationData = new PowerOfAttorneyInformationDataForGiver();
        //                    powerOfAttorneyInformationData = FetchPOAGiverDataByMutationID(data.code);
        //                    if (powerOfAttorneyInformationData != null)
        //                    {
        //                        _logger.LogInformation("POA Giver Data " + powerOfAttorneyInformationData);
        //                        string Response = SavePowerOfAttorneyGiver(powerOfAttorneyInformationData);
        //                        _logger.LogInformation("POA Giver Save Data - " + Response);
        //                        string[] po_giver_id = Response!.Split(",");
        //                        if (po_giver_id[0] == "Success")
        //                        {
        //                            if (!string.IsNullOrEmpty(powerOfAttorneyInformationModel.poa_giver_ids) && !powerOfAttorneyInformationModel.poa_giver_ids.Contains(data.code.ToString()))
        //                            {
        //                                powerOfAttorneyInformationModel.poa_giver_ids = powerOfAttorneyInformationModel.poa_giver_ids + "," + po_giver_id[1].ToString();
        //                            }
        //                            else
        //                            {
        //                                powerOfAttorneyInformationModel.poa_giver_ids = po_giver_id[1].ToString();
        //                            }
        //                        }
        //                        else
        //                        {
        //                            throw new HandleException(Response);
        //                        }
        //                    }
        //                }
        //            }
        //            //End

        //            //Assign Value
        //            powerOfAttorneyInformationModel.mobileno = "NA";
        //            powerOfAttorneyInformationModel.mobilenoverified = "NA";
        //            powerOfAttorneyInformationModel.emailid = "NA";
        //            powerOfAttorneyInformationModel.emailidverified = "NA";
        //            powerOfAttorneyInformationModel.usertype = "NA";
        //            powerOfAttorneyInformationModel.usertype_code = 0;
        //            powerOfAttorneyInformationModel.cts_number = "NA";
        //            powerOfAttorneyInformationModel.owner_number = "NA";
        //            powerOfAttorneyInformationModel.mutation_srno = "NA";
        //            powerOfAttorneyInformationModel.village_code = "NA";
        //            powerOfAttorneyInformationModel.village_name = "NA";
        //            powerOfAttorneyInformationModel.prefix_in_eng = "NA";
        //            powerOfAttorneyInformationModel.fname_in_eng = "NA";
        //            powerOfAttorneyInformationModel.mname_in_eng = "NA";
        //            powerOfAttorneyInformationModel.lname_in_eng = "NA";
        //            powerOfAttorneyInformationModel.prefix_in_marathi = "NA";
        //            powerOfAttorneyInformationModel.fname_in_marathi = "NA";
        //            powerOfAttorneyInformationModel.mname_in_marathi = "NA";
        //            powerOfAttorneyInformationModel.lname_in_marathi = "NA";
        //            //New Fields For Taker
        //            powerOfAttorneyInformationModel.company_name_in_marathi = "NA";
        //            powerOfAttorneyInformationModel.company_name_in_eng = "NA";
        //            //End
        //            powerOfAttorneyInformationModel.username = "NA";
        //            //New Fields for Taker
        //            powerOfAttorneyInformationModel.alias_name = "NA";
        //            powerOfAttorneyInformationModel.gender_code = "NA";
        //            powerOfAttorneyInformationModel.gender_description = "NA";
        //            powerOfAttorneyInformationModel.dob = "NA";
        //            powerOfAttorneyInformationModel.mother_name_in_marathi = "NA";
        //            powerOfAttorneyInformationModel.mother_name_in_eng = "NA";
        //            powerOfAttorneyInformationModel.attornytype_desc = "NA";
        //            powerOfAttorneyInformationModel.attornytype_code = 0;

        //            powerOfAttorneyInformationModel.landBuyArea = "NA";
        //            powerOfAttorneyInformationModel.address_type = "NA";
        //            powerOfAttorneyInformationModel.address = "NA";
        //            powerOfAttorneyInformationModel.state = "NA";
        //            powerOfAttorneyInformationModel.district = "NA";
        //            powerOfAttorneyInformationModel.taluka = "NA";
        //            powerOfAttorneyInformationModel.city = "NA";
        //            powerOfAttorneyInformationModel.flatno_plotno = "NA";
        //            powerOfAttorneyInformationModel.societyname = "NA";
        //            powerOfAttorneyInformationModel.mainstreet = "NA";
        //            powerOfAttorneyInformationModel.landmark = "NA";
        //            powerOfAttorneyInformationModel.locality = "NA";
        //            powerOfAttorneyInformationModel.pincode = "NA";
        //            powerOfAttorneyInformationModel.postofficename = "NA";
        //            powerOfAttorneyInformationModel.address_proof_document_name = "NA";
        //            powerOfAttorneyInformationModel.address_proof_document_path = "NA";

        //            powerOfAttorneyInformationModel.owner_of_property_in_maharashtra = false;
        //            // powerOfAttorneyInformationModel.PropertyTypeMaster = 0;
        //            powerOfAttorneyInformationModel.property_district_code = "NA";
        //            powerOfAttorneyInformationModel.property_district_name_in_marathi = "NA";
        //            powerOfAttorneyInformationModel.property_district_name_in_english = "NA";
        //            powerOfAttorneyInformationModel.property_taluka_code = "NA";
        //            powerOfAttorneyInformationModel.property_taluka_name = "NA";
        //            powerOfAttorneyInformationModel.property_city_code = "NA";
        //            powerOfAttorneyInformationModel.property_city_name = "NA";
        //            powerOfAttorneyInformationModel.khateno = "NA";
        //            powerOfAttorneyInformationModel.ulpin = "NA";
        //            powerOfAttorneyInformationModel.city_servey_no = "NA";
        //            powerOfAttorneyInformationModel.lr_property_id = "NA";
        //            powerOfAttorneyInformationModel.profile_pic_file_name = "NA";
        //            powerOfAttorneyInformationModel.profile_pic_file_path = "NA";
        //            powerOfAttorneyInformationModel.isPOAisPartofDast = "NA";
        //            powerOfAttorneyInformationModel.isDeclerationInvolvedInPOA = "NA";
        //            powerOfAttorneyInformationModel.isPOAPermanant = "NA";
        //            powerOfAttorneyInformationModel.isTransferRights = "NA";
        //            powerOfAttorneyInformationModel.dastNo = "NA";
        //            powerOfAttorneyInformationModel.dastNoDate = "NA";
        //            powerOfAttorneyInformationModel.dastNoYear = "NA";
        //            powerOfAttorneyInformationModel.isDastVerified = false;
        //            powerOfAttorneyInformationModel.verifiedDastData = "NA";
        //            powerOfAttorneyInformationModel.digcode = 0;
        //            powerOfAttorneyInformationModel.digname = "NA";
        //            powerOfAttorneyInformationModel.poa_district_code = "0";
        //            powerOfAttorneyInformationModel.poa_district_name = "NA";
        //            powerOfAttorneyInformationModel.sro_office_code = 0;
        //            powerOfAttorneyInformationModel.sro_office_name = "NA";
        //            powerOfAttorneyInformationModel.signed_file_path = "NA";
        //            powerOfAttorneyInformationModel.signed_file_name = "NA";
        //            powerOfAttorneyInformationModel.prefixcode_marathi = "0";
        //            powerOfAttorneyInformationModel.prefixcode_eng = "0";
        //            powerOfAttorneyInformationModel.sub_property_id = "999999";


        //            powerOfAttorneyInformationModel.usertype_code = takerData.usertype_code;
        //            powerOfAttorneyInformationModel.usertype = takerData.usertype!.Trim().ToUpper();
        //            //powerOfAttorneyInformationModel.profile_pic_file_name = takerData.photo.passportName;

        //            powerOfAttorneyInformationModel.owner_of_property_in_maharashtra = (takerData.isMHProperty!.hasProperty!.Trim().ToUpper() == "YES") ? true : false;
        //            PropertyTypeMaster proptype;
        //            //using (var transaction = _contextR.Database.BeginTransaction(System.Data.IsolationLevel.ReadCommitted)) // Changed here
        //            //{
        //                proptype = _context.propertyTypes.FirstOrDefault(s => s.propertytypeid == Convert.ToInt32(takerData.isMHProperty.propType))!;
        //                powerOfAttorneyInformationModel.PropertyTypeMaster = proptype;
        //            //    transaction.Commit();
        //            //}
        //            if (powerOfAttorneyInformationModel.owner_of_property_in_maharashtra)
        //            {
        //                if (powerOfAttorneyInformationModel.PropertyTypeMaster.propertytypeid == 1 && string.IsNullOrEmpty(takerData.isMHProperty.userDetails!.khataNo))
        //                {
        //                    return "When Property Type Is 7/12 Then Khate No Should Not Be Empty";
        //                }
        //                if (powerOfAttorneyInformationModel.PropertyTypeMaster.propertytypeid == 2 && string.IsNullOrEmpty(takerData.isMHProperty.userDetails!.naBhu))
        //                {
        //                    return "When Property Type Is Property Card Then City Servey No Should Not Be Empty";
        //                }
        //                if (powerOfAttorneyInformationModel.PropertyTypeMaster.propertytypeid == 3 && string.IsNullOrEmpty(takerData.isMHProperty.userDetails!.ulpin))
        //                {
        //                    return "When Property Type Is ULPIN Then ULPIN Should Not Be Empty";
        //                }
        //                powerOfAttorneyInformationModel.khateno = string.IsNullOrEmpty(takerData.isMHProperty.userDetails!.khataNo) ? "NA" : takerData.isMHProperty.userDetails.khataNo;
        //                powerOfAttorneyInformationModel.city_servey_no = string.IsNullOrEmpty(takerData.isMHProperty.userDetails.naBhu) ? "NA" : takerData.isMHProperty.userDetails.naBhu;
        //                powerOfAttorneyInformationModel.ulpin = string.IsNullOrEmpty(takerData.isMHProperty.userDetails.ulpin) ? "NA" : takerData.isMHProperty.userDetails.ulpin;
        //                if (powerOfAttorneyInformationModel.PropertyTypeMaster.propertytypeid == 1)
        //                {
        //                    powerOfAttorneyInformationModel.city_servey_no = "NA";
        //                    powerOfAttorneyInformationModel.ulpin = "NA";
        //                }
        //                else if (powerOfAttorneyInformationModel.PropertyTypeMaster.propertytypeid == 2)
        //                {
        //                    powerOfAttorneyInformationModel.khateno = "NA";
        //                    powerOfAttorneyInformationModel.ulpin = "NA";
        //                }
        //                else if (powerOfAttorneyInformationModel.PropertyTypeMaster.propertytypeid == 3)
        //                {
        //                    powerOfAttorneyInformationModel.khateno = "NA";
        //                    powerOfAttorneyInformationModel.city_servey_no = "NA";
        //                }
        //                powerOfAttorneyInformationModel.property_district_code = takerData.isMHProperty.userDetails.district!.district_code;
        //                powerOfAttorneyInformationModel.property_district_name_in_marathi = takerData.isMHProperty.userDetails.district.district_name;
        //                powerOfAttorneyInformationModel.property_district_name_in_english = takerData.isMHProperty.userDetails.district.district_english_name;

        //                powerOfAttorneyInformationModel.property_taluka_code = takerData.isMHProperty.userDetails.taluka!.office_code;
        //                powerOfAttorneyInformationModel.property_taluka_name = takerData.isMHProperty.userDetails.taluka.office_name;

        //                powerOfAttorneyInformationModel.property_city_code = takerData.isMHProperty.userDetails.village!.village_code;
        //                powerOfAttorneyInformationModel.property_city_name = takerData.isMHProperty.userDetails.village.village_name;
        //            }
        //            else
        //            {
        //                powerOfAttorneyInformationModel.khateno = "NA";
        //                //Gouri
        //                powerOfAttorneyInformationModel.city_servey_no = "NA";
        //                powerOfAttorneyInformationModel.ulpin = "NA";
        //                powerOfAttorneyInformationModel.property_district_code = "NA";
        //                powerOfAttorneyInformationModel.property_district_name_in_marathi = "NA";
        //                powerOfAttorneyInformationModel.property_district_name_in_english = "NA";
        //                powerOfAttorneyInformationModel.property_taluka_code = "NA";
        //                powerOfAttorneyInformationModel.property_taluka_name = "NA";
        //                powerOfAttorneyInformationModel.property_city_code = "NA";
        //                powerOfAttorneyInformationModel.property_city_name = "NA";
        //            }
        //            powerOfAttorneyInformationModel.username = takerData.isMHProperty.userDetails!.userName;
        //            powerOfAttorneyInformationModel.prefixcode_marathi = takerData.isMHProperty.userDetails!.suffixcode;
        //            powerOfAttorneyInformationModel.prefixcode_eng = takerData.isMHProperty.userDetails!.suffixCodeEng;

        //            if (takerData.usertype_code == 1)
        //            {
        //                powerOfAttorneyInformationModel.prefix_in_marathi = takerData.isMHProperty.userDetails.suffix;
        //                powerOfAttorneyInformationModel.fname_in_marathi = takerData.isMHProperty.userDetails.firstName;
        //                powerOfAttorneyInformationModel.mname_in_marathi = takerData.isMHProperty.userDetails.middleName;
        //                powerOfAttorneyInformationModel.lname_in_marathi = takerData.isMHProperty.userDetails.lastName;
        //                powerOfAttorneyInformationModel.prefix_in_eng = takerData.isMHProperty.userDetails.suffixEng;
        //                powerOfAttorneyInformationModel.fname_in_eng = takerData.isMHProperty.userDetails.firstNameEng;
        //                powerOfAttorneyInformationModel.mname_in_eng = takerData.isMHProperty.userDetails.middleNameEng;
        //                powerOfAttorneyInformationModel.lname_in_eng = takerData.isMHProperty.userDetails.lastNameEng;

        //                powerOfAttorneyInformationModel.alias_name = takerData.dharak!.userdharak!.aliceName;
        //                powerOfAttorneyInformationModel.gender_code = takerData.dharak.userdharak.gender!.gender_code;
        //                powerOfAttorneyInformationModel.gender_description = takerData.dharak.userdharak.gender.gender_description;

        //                // powerOfAttorneyInformationModel.khata_type_code = takerData.dharak.userdharak.khataType!.khataCode;
        //                // powerOfAttorneyInformationModel.khata_type_name = takerData.dharak.userdharak.khataType.khataLabel;

        //                //powerOfAttorneyInformationModel.holder_type_code = takerData.dharak.userdharak.holderType!.owner_status_code;
        //                //powerOfAttorneyInformationModel.holder_type_name = takerData.dharak.userdharak.holderType.owner_status_description;

        //                powerOfAttorneyInformationModel.dob = takerData.dharak.userdharak.dob;
        //                powerOfAttorneyInformationModel.mother_name_in_marathi = takerData.dharak.userdharak.motherName!.Trim();
        //                powerOfAttorneyInformationModel.mother_name_in_eng = takerData.dharak.userdharak.motherNameEng!.Trim();


        //                powerOfAttorneyInformationModel.company_name_in_marathi = "NA";
        //                powerOfAttorneyInformationModel.company_name_in_eng = "NA";
        //                /*powerOfAttorneyInformationModel.apk_code = 0;
        //                powerOfAttorneyInformationModel.apk_description = "NA";
        //                powerOfAttorneyInformationModel.aapak = "NA";*/
        //                powerOfAttorneyInformationModel.landBuyArea = "NA";
        //            }
        //            //if (takerData.usertype.Trim().ToUpper() == "COMPANY")
        //            else
        //            {
        //                powerOfAttorneyInformationModel.company_name_in_marathi = takerData.isMHProperty.userDetails.companyName!.Trim();
        //                powerOfAttorneyInformationModel.company_name_in_eng = takerData.isMHProperty.userDetails.companyNameEng!.Trim();
        //                // powerOfAttorneyInformationModel.holder_type_code = takerData.dharak!.companydharak!.holderType!.owner_status_code;
        //                //powerOfAttorneyInformationModel.holder_type_name = takerData.dharak.companydharak.holderType.owner_status_description;

        //                //powerOfAttorneyInformationModel.khata_type_code = takerData.dharak.companydharak.khataType!.khataCode;
        //                //powerOfAttorneyInformationModel.khata_type_name = takerData.dharak.companydharak.khataType.khataLabel;

        //                /*powerOfAttorneyInformationModel.apk_code = takerData.dharak.companydharak!.aapakDropdown!.apk_code;
        //                powerOfAttorneyInformationModel.apk_description = takerData.dharak.companydharak.aapakDropdown.apk_description;

        //                powerOfAttorneyInformationModel.aapak = takerData.dharak.companydharak.aapak!.Trim();*/
        //                powerOfAttorneyInformationModel.landBuyArea = takerData.dharak!.companydharak!.landBuyArea!.Trim();


        //                powerOfAttorneyInformationModel.prefix_in_marathi = "NA";
        //                powerOfAttorneyInformationModel.fname_in_marathi = "NA";
        //                powerOfAttorneyInformationModel.mname_in_marathi = "NA";
        //                powerOfAttorneyInformationModel.lname_in_marathi = "NA";
        //                powerOfAttorneyInformationModel.prefix_in_eng = "NA";
        //                powerOfAttorneyInformationModel.fname_in_eng = "NA";
        //                powerOfAttorneyInformationModel.mname_in_eng = "NA";
        //                powerOfAttorneyInformationModel.lname_in_eng = "NA";

        //                powerOfAttorneyInformationModel.alias_name = "NA";
        //                powerOfAttorneyInformationModel.gender_code = "NA";
        //                powerOfAttorneyInformationModel.gender_description = "NA";
        //                powerOfAttorneyInformationModel.dob = "NA";
        //                powerOfAttorneyInformationModel.mother_name_in_marathi = "NA";
        //                powerOfAttorneyInformationModel.mother_name_in_eng = "NA";
        //            }
        //            powerOfAttorneyInformationModel.address_type = takerData.address!.addressType!.Trim().ToUpper();
        //            if (takerData.address.addressType.Trim().ToUpper() == "INDIA")
        //            {
        //                powerOfAttorneyInformationModel.state = takerData.address.indiaAddress!.state;
        //                powerOfAttorneyInformationModel.district = takerData.address.indiaAddress.district;
        //                powerOfAttorneyInformationModel.city = takerData.address.indiaAddress.city;
        //                powerOfAttorneyInformationModel.taluka = takerData.address.indiaAddress.taluka;
        //                powerOfAttorneyInformationModel.flatno_plotno = takerData.address.indiaAddress.plotNo;
        //                powerOfAttorneyInformationModel.societyname = takerData.address.indiaAddress.building;
        //                powerOfAttorneyInformationModel.mainstreet = takerData.address.indiaAddress.mainRoad;
        //                powerOfAttorneyInformationModel.landmark = takerData.address.indiaAddress.impSymbol;
        //                powerOfAttorneyInformationModel.locality = takerData.address.indiaAddress.area;
        //                powerOfAttorneyInformationModel.pincode = takerData.address.indiaAddress.pincode;
        //                powerOfAttorneyInformationModel.postofficename = takerData.address.indiaAddress.postOfficeName;
        //                powerOfAttorneyInformationModel.mobileno = takerData.address.indiaAddress.mobile;
        //                powerOfAttorneyInformationModel.mobilenoverified = takerData.address.indiaAddress.mobileOTP!.Trim().ToUpper();
        //                powerOfAttorneyInformationModel.emailid = "NA";
        //                powerOfAttorneyInformationModel.emailidverified = "NA";
        //                powerOfAttorneyInformationModel.address = "NA";
        //            }
        //            else if (takerData.address.addressType.Trim().ToUpper() == "FOREIGN")
        //            {
        //                powerOfAttorneyInformationModel.address = takerData.address.foreignAddress!.address;
        //                powerOfAttorneyInformationModel.mobileno = takerData.address.foreignAddress.mobile;
        //                powerOfAttorneyInformationModel.mobilenoverified = "NO";
        //                powerOfAttorneyInformationModel.emailid = takerData.address.foreignAddress.email;
        //                powerOfAttorneyInformationModel.emailidverified = takerData.address.foreignAddress.emailOTP!.Trim().ToUpper();

        //                powerOfAttorneyInformationModel.state = "NA";
        //                powerOfAttorneyInformationModel.district = "NA";
        //                powerOfAttorneyInformationModel.city = "NA";
        //                powerOfAttorneyInformationModel.taluka = "NA";
        //                powerOfAttorneyInformationModel.flatno_plotno = "NA";
        //                powerOfAttorneyInformationModel.societyname = "NA";
        //                powerOfAttorneyInformationModel.mainstreet = "NA";
        //                powerOfAttorneyInformationModel.landmark = "NA";
        //                powerOfAttorneyInformationModel.locality = "NA";
        //                powerOfAttorneyInformationModel.pincode = "NA";
        //                powerOfAttorneyInformationModel.postofficename = "NA";
        //            }
        //            ApplicationDTL applicationDTL;
        //            UserMaster userMaster;
        //            //using (var transaction = _contextR.Database.BeginTransaction(System.Data.IsolationLevel.ReadCommitted)) // Changed here
        //            //{
        //                _logger.LogInformation("POA APP DTL ");
        //                applicationDTL = _context.applicationDTL.AsNoTracking().FirstOrDefault(s => s.applicationid == takerData.applicationid)!;
        //                powerOfAttorneyInformationModel.applicationDTL = applicationDTL;
        //                _logger.LogInformation("POA APP DTL 2 - " + applicationDTL.applicationid + " " + applicationDTL.powerOfAttorneyIDs);
        //            //    transaction.Commit();
        //            //}
        //            //using (var transaction = _contextR.Database.BeginTransaction(System.Data.IsolationLevel.ReadCommitted)) // Changed here
        //            //{
        //                userMaster = _context.userMasters.FirstOrDefault(s => s.userid == takerData.userid)!;
        //                powerOfAttorneyInformationModel.userMaster = userMaster;
        //            //    transaction.Commit();
        //            //}

        //            powerOfAttorneyInformationModel.mutation_id = takerData.mutation_id;
        //            powerOfAttorneyInformationModel.attornytype_code = takerData.attornytype!.poa_type_code;
        //            powerOfAttorneyInformationModel.attornytype_desc = takerData.attornytype.poa_type_description;
        //            powerOfAttorneyInformationModel.dastNo = takerData.dastNo!;
        //            powerOfAttorneyInformationModel.dastNoYear = takerData.dastNoYear!;
        //            powerOfAttorneyInformationModel.dastNoDate = takerData.dastNoDate!;
        //            powerOfAttorneyInformationModel.isDastVerified = takerData.isDastVarified;
        //            powerOfAttorneyInformationModel.verifiedDastData = takerData.verifiedDastData!;
        //            powerOfAttorneyInformationModel.isPOAisPartofDast = takerData.isPOAisPartofDast!;
        //            powerOfAttorneyInformationModel.isDeclerationInvolvedInPOA = takerData.isDeclerationInvolvedInPOA!;
        //            powerOfAttorneyInformationModel.isPOAPermanant = takerData.isPOAPermanant!;
        //            powerOfAttorneyInformationModel.isTransferRights = takerData.isTransferRights!;
        //            powerOfAttorneyInformationModel.digcode = takerData.division!.digcode!;
        //            powerOfAttorneyInformationModel.digname = takerData.division.dig!;
        //            powerOfAttorneyInformationModel.poa_district_code = takerData.district!.jdrcode.ToString()!;
        //            powerOfAttorneyInformationModel.poa_district_name = takerData.district.jdr!;
        //            powerOfAttorneyInformationModel.sro_office_code = Convert.ToInt32(takerData.registrar!.srocode);
        //            powerOfAttorneyInformationModel.sro_office_name = takerData.registrar.sro!;
        //            powerOfAttorneyInformationModel.mutation_id = takerData.mutation_id;
        //            //Generate The Power Of Attorney Code
        //            powerOfAttorneyInformationModel.power_of_attorney_code = GeneratePowerOfAttorneyCode(takerData.applicationid!, true);
        //            //End
        //            _logger.LogInformation("powerOfAttorneyInformationModel.power_of_attorney_code " + powerOfAttorneyInformationModel.power_of_attorney_code);
        //            //Assign Default Value

        //            dbTable.mobileno = "NA";
        //            dbTable.mobilenoverified = "NA";
        //            dbTable.emailid = "NA";
        //            dbTable.emailidverified = "NA";
        //            dbTable.usertype = "NA";
        //            dbTable.usertype_code = 0;
        //            dbTable.prefix_in_eng = "NA";
        //            dbTable.fname_in_eng = "NA";
        //            dbTable.mname_in_eng = "NA";
        //            dbTable.lname_in_eng = "NA";
        //            dbTable.prefix_in_marathi = "NA";
        //            dbTable.fname_in_marathi = "NA";
        //            dbTable.mname_in_marathi = "NA";
        //            dbTable.lname_in_marathi = "NA";
        //            //New Fields For Taker
        //            dbTable.company_name_in_marathi = "NA";
        //            dbTable.company_name_in_eng = "NA";
        //            //End
        //            dbTable.username = "NA";
        //            //New Fields for Taker
        //            dbTable.alias_name = "NA";
        //            dbTable.gender_code = "NA";
        //            dbTable.gender_description = "NA";
        //            dbTable.dob = "NA";
        //            dbTable.mother_name_in_marathi = "NA";
        //            dbTable.mother_name_in_eng = "NA";
        //            dbTable.attornytype_desc = "NA";
        //            dbTable.attornytype_code = 0;

        //            dbTable.landBuyArea = "NA";
        //            dbTable.address_type = "NA";
        //            dbTable.address = "NA";
        //            dbTable.state = "NA";
        //            dbTable.district = "NA";
        //            dbTable.taluka = "NA";
        //            dbTable.city = "NA";
        //            dbTable.flatno_plotno = "NA";
        //            dbTable.societyname = "NA";
        //            dbTable.mainstreet = "NA";
        //            dbTable.landmark = "NA";
        //            dbTable.locality = "NA";
        //            dbTable.pincode = "NA";
        //            dbTable.postofficename = "NA";
        //            dbTable.address_proof_document_name = "NA";
        //            dbTable.address_proof_document_path = "NA";

        //            dbTable.owner_of_property_in_maharashtra = false;
        //            //dbTable.PropertyTypeMaster = 0;
        //            dbTable.property_district_code = "NA";
        //            dbTable.property_district_name_in_marathi = "NA";
        //            dbTable.property_district_name_in_english = "NA";
        //            dbTable.property_taluka_code = "NA";
        //            dbTable.property_taluka_name = "NA";
        //            dbTable.property_city_code = "NA";
        //            dbTable.property_city_name = "NA";
        //            dbTable.khateno = "NA";
        //            dbTable.ulpin = "NA";
        //            dbTable.city_servey_no = "NA";
        //            dbTable.lr_property_id = "NA";
        //            dbTable.profile_pic_file_name = "NA";
        //            dbTable.profile_pic_file_path = "NA";
        //            dbTable.isPOAisPartofDast = "NA";
        //            dbTable.isDeclerationInvolvedInPOA = "NA";
        //            dbTable.isPOAPermanant = "NA";
        //            dbTable.isTransferRights = "NA";
        //            dbTable.dast_no = "NA";
        //            dbTable.dast_no_date = "NA";
        //            dbTable.dast_no_year = "NA";
        //            dbTable.isDastVerified = false;
        //            dbTable.verifieddastData = "NA";
        //            dbTable.digcode = 0;
        //            dbTable.digname = "NA";
        //            dbTable.poa_district_code = "0";
        //            dbTable.poa_district_name = "NA";
        //            dbTable.sro_office_code = 0;
        //            dbTable.sro_office_name = "NA";
        //            dbTable.signed_file_path = "NA";
        //            dbTable.signed_file_name = "NA";
        //            //Assign Data to Table fields to insert new records
        //            _logger.LogInformation("Test1 Log");
        //            dbTable.usertype_code = powerOfAttorneyInformationModel.usertype_code;
        //            dbTable.mutation_id = Convert.ToInt32(powerOfAttorneyInformationModel.mutation_id!);
        //            dbTable.usertype = powerOfAttorneyInformationModel.usertype;
        //            dbTable.mobileno = powerOfAttorneyInformationModel.mobileno!;
        //            dbTable.mobilenoverified = powerOfAttorneyInformationModel.mobilenoverified!;
        //            dbTable.emailid = powerOfAttorneyInformationModel.emailid!;
        //            dbTable.emailidverified = powerOfAttorneyInformationModel.emailidverified!;
        //            dbTable.prefixcode_eng = powerOfAttorneyInformationModel.prefixcode_eng!;
        //            dbTable.prefix_in_eng = powerOfAttorneyInformationModel.prefix_in_eng!.Trim();
        //            dbTable.fname_in_eng = textInfo.ToTitleCase(powerOfAttorneyInformationModel.fname_in_eng!.Trim());
        //            dbTable.mname_in_eng = textInfo.ToTitleCase(powerOfAttorneyInformationModel.mname_in_eng!.Trim());
        //            dbTable.lname_in_eng = textInfo.ToTitleCase(powerOfAttorneyInformationModel.lname_in_eng!.Trim());
        //            dbTable.prefixcode_marathi = powerOfAttorneyInformationModel.prefixcode_marathi!;
        //            dbTable.prefix_in_marathi = powerOfAttorneyInformationModel.prefix_in_marathi!.Trim();
        //            dbTable.fname_in_marathi = powerOfAttorneyInformationModel.fname_in_marathi!.Trim();
        //            dbTable.mname_in_marathi = powerOfAttorneyInformationModel.mname_in_marathi!.Trim();
        //            dbTable.lname_in_marathi = powerOfAttorneyInformationModel.lname_in_marathi!.Trim();
        //            dbTable.address_type = powerOfAttorneyInformationModel.address_type;
        //            dbTable.address = string.IsNullOrEmpty(powerOfAttorneyInformationModel.address) ? "NA" : powerOfAttorneyInformationModel.address;
        //            dbTable.state = powerOfAttorneyInformationModel.state!;
        //            dbTable.district = powerOfAttorneyInformationModel.district!;
        //            dbTable.taluka = powerOfAttorneyInformationModel.taluka!;
        //            dbTable.city = powerOfAttorneyInformationModel.city!;
        //            dbTable.flatno_plotno = powerOfAttorneyInformationModel.flatno_plotno!;
        //            dbTable.societyname = powerOfAttorneyInformationModel.societyname!;
        //            dbTable.mainstreet = powerOfAttorneyInformationModel.mainstreet!;
        //            dbTable.landmark = powerOfAttorneyInformationModel.landmark!;
        //            dbTable.locality = powerOfAttorneyInformationModel.locality!;
        //            dbTable.pincode = powerOfAttorneyInformationModel.pincode!;
        //            dbTable.postofficename = powerOfAttorneyInformationModel.postofficename!;
        //            dbTable.owner_of_property_in_maharashtra = powerOfAttorneyInformationModel.owner_of_property_in_maharashtra;
        //            dbTable.propertyType = powerOfAttorneyInformationModel.PropertyTypeMaster;
        //            dbTable.property_district_code = powerOfAttorneyInformationModel.property_district_code;
        //            dbTable.property_district_name_in_marathi = powerOfAttorneyInformationModel.property_district_name_in_marathi;
        //            dbTable.property_district_name_in_english = powerOfAttorneyInformationModel.property_district_name_in_english;

        //            dbTable.property_taluka_code = powerOfAttorneyInformationModel.property_taluka_code;
        //            dbTable.property_taluka_name = powerOfAttorneyInformationModel.property_taluka_name;
        //            dbTable.property_city_code = powerOfAttorneyInformationModel.property_city_code;
        //            dbTable.property_city_name = powerOfAttorneyInformationModel.property_city_name;

        //            dbTable.khateno = powerOfAttorneyInformationModel.khateno;
        //            dbTable.city_servey_no = powerOfAttorneyInformationModel.city_servey_no;
        //            dbTable.ulpin = powerOfAttorneyInformationModel.ulpin;
        //            dbTable.username = string.IsNullOrEmpty(powerOfAttorneyInformationModel.username) ? "NA" : powerOfAttorneyInformationModel.username;
        //            dbTable.company_name_in_eng = powerOfAttorneyInformationModel.company_name_in_eng;
        //            dbTable.company_name_in_marathi = powerOfAttorneyInformationModel.company_name_in_marathi;
        //            dbTable.userMaster = powerOfAttorneyInformationModel.userMaster;
        //            dbTable.applicationDTL = powerOfAttorneyInformationModel.applicationDTL;
        //            dbTable.is_taker = true;

        //            dbTable.attornytype_code = powerOfAttorneyInformationModel.attornytype_code;
        //            dbTable.attornytype_desc = powerOfAttorneyInformationModel.attornytype_desc;


        //            dbTable.isPOAisPartofDast = powerOfAttorneyInformationModel.isPOAisPartofDast.ToUpper();
        //            dbTable.isDeclerationInvolvedInPOA = powerOfAttorneyInformationModel.isDeclerationInvolvedInPOA.ToUpper();
        //            dbTable.isPOAPermanant = powerOfAttorneyInformationModel.isPOAPermanant.ToUpper();
        //            dbTable.isTransferRights = powerOfAttorneyInformationModel.isTransferRights.ToUpper();
        //            dbTable.dast_no = powerOfAttorneyInformationModel.dastNo;
        //            dbTable.dast_no_date = powerOfAttorneyInformationModel.dastNoDate;
        //            dbTable.dast_no_year = powerOfAttorneyInformationModel.dastNoYear;
        //            dbTable.isDastVerified = powerOfAttorneyInformationModel.isDastVerified;
        //            dbTable.verifieddastData = powerOfAttorneyInformationModel.verifiedDastData!;
        //            dbTable.digcode = powerOfAttorneyInformationModel.digcode;
        //            dbTable.digname = powerOfAttorneyInformationModel.digname;
        //            dbTable.poa_district_code = powerOfAttorneyInformationModel.poa_district_code;
        //            dbTable.poa_district_name = powerOfAttorneyInformationModel.poa_district_name;
        //            // dbTable.poa_district_name_in_eng = powerOfAttorneyInformationModel.poa_district_name_in_eng;
        //            dbTable.sro_office_code = powerOfAttorneyInformationModel.sro_office_code;
        //            dbTable.sro_office_name = powerOfAttorneyInformationModel.sro_office_name;

        //            dbTable.alias_name = powerOfAttorneyInformationModel.alias_name!;
        //            dbTable.gender_code = powerOfAttorneyInformationModel.gender_code!;
        //            dbTable.gender_description = powerOfAttorneyInformationModel.gender_description!.Trim().ToUpper();
        //            /*dbTable.khata_type_code = powerOfAttorneyInformationModel.khata_type_code!;
        //            dbTable.khata_type_name = powerOfAttorneyInformationModel.khata_type_name!;

        //            dbTable.owner_status_code = powerOfAttorneyInformationModel.holder_type_code!;
        //            dbTable.owner_status_description = powerOfAttorneyInformationModel.holder_type_name!;*/

        //            dbTable.dob = powerOfAttorneyInformationModel.dob!;
        //            dbTable.mother_name_in_marathi = powerOfAttorneyInformationModel.mother_name_in_marathi.Trim();
        //            dbTable.mother_name_in_eng = textInfo.ToTitleCase(powerOfAttorneyInformationModel.mother_name_in_eng.Trim());
        //            /* dbTable.apk_code = powerOfAttorneyInformationModel.apk_code;
        //             dbTable.apk_description = powerOfAttorneyInformationModel.apk_description;

        //             dbTable.aapak = powerOfAttorneyInformationModel.aapak;*/
        //            dbTable.landBuyArea = powerOfAttorneyInformationModel.landBuyArea;
        //            dbTable.lr_property_id = "NA";
        //            dbTable.power_of_attorney_code = powerOfAttorneyInformationModel.power_of_attorney_code;
        //            dbTable.address_proof_document_name = "NA";
        //            dbTable.address_proof_document_path = "NA";
        //            dbTable.signed_file_name = "NA";
        //            dbTable.signed_file_path = "NA";
        //            dbTable.profile_pic_file_name = "NA";
        //            dbTable.profile_pic_file_path = "NA";
        //            dbTable.sub_property_no = powerOfAttorneyInformationModel.sub_property_id;
        //            dbTable.cts_number = powerOfAttorneyInformationModel.cts_number;
        //            dbTable.owner_number = powerOfAttorneyInformationModel.owner_number;
        //            dbTable.mutation_srno = powerOfAttorneyInformationModel.mutation_srno;
        //            dbTable.village_code = powerOfAttorneyInformationModel.village_code;
        //            dbTable.village_name = powerOfAttorneyInformationModel.village_name;
        //            dbTable.poa_giver_ids = powerOfAttorneyInformationModel.poa_giver_ids!;
        //            _logger.LogInformation("POA Taker Save Data " + JsonConvert.SerializeObject(powerOfAttorneyInformationModel));
        //            _context.powerOfAttorneyInformation.Add(dbTable);
        //            _context.SaveChanges();

        //            //Get Saved Row ID
        //            int power_of_attorney_id = dbTable.power_of_attorney_id;
        //            _logger.LogInformation("POA Saved ID " + power_of_attorney_id.ToString());
        //            string VerificationType = string.Empty;
        //            bool checkAddressFlag = true;
        //            //bool checkSignFlag = false;
        //            string CurrentDateTime = DateTime.Now.ToString("yyyy-MMM-dd-HHmmss");
        //            if (powerOfAttorneyInformationModel.address_type == "INDIA")
        //            {
        //                VerificationType = "MOBILENO";
        //                if (!string.IsNullOrEmpty(takerData.address.indiaAddress!.addressProofSrc) && !string.IsNullOrEmpty(takerData.address.indiaAddress.addressProofName))
        //                {
        //                    string[] AddressData = takerData.address.indiaAddress.addressProofSrc.Split(",");
        //                    checkAddressFlag = methodForFile.SaveImageForApplicant(AddressData[1], takerData.address.indiaAddress.addressProofName, power_of_attorney_id.ToString(), "AddressProof", FolderPath + @"\", CurrentDateTime);
        //                    string AddressProofExt = Path.GetExtension(takerData.address.indiaAddress.addressProofName);
        //                    powerOfAttorneyInformationModel.address_proof_document_name = "AddressProof" + power_of_attorney_id + "_" + CurrentDateTime + AddressProofExt;
        //                    powerOfAttorneyInformationModel.address_proof_document_path = FolderPath + power_of_attorney_id + @"\" + powerOfAttorneyInformationModel.address_proof_document_name;

        //                    var UpdateAddressFilePath = _context.powerOfAttorneyInformation.Where(w => w.power_of_attorney_id == power_of_attorney_id).FirstOrDefault();
        //                    if (UpdateAddressFilePath != null)
        //                    {
        //                        dbTable.address_proof_document_name = powerOfAttorneyInformationModel.address_proof_document_name;
        //                        dbTable.address_proof_document_path = powerOfAttorneyInformationModel.address_proof_document_path;
        //                        _context.Entry(dbTable).CurrentValues.SetValues(dbTable);
        //                        _context.SaveChanges();
        //                    }
        //                }


        //                //string[] signData = takerData.address.indiaAddress.signatureSrc.Split(",");
        //                //checkSignFlag = methodForFile.SaveImageForApplicant(signData[1], takerData.address.indiaAddress.signatureName, power_of_attorney_id.ToString(), "Signature", FolderPath + @"\", CurrentDateTime);
        //                //string SignatureExt = Path.GetExtension(takerData.address.indiaAddress.signatureName);
        //                //powerOfAttorneyInformationModel.signed_file_name = "Signature" + power_of_attorney_id + "_" + CurrentDateTime + SignatureExt;
        //                //powerOfAttorneyInformationModel.signed_file_path = FolderPath + @"\" + power_of_attorney_id + @"\" + powerOfAttorneyInformationModel.signed_file_name;

        //                //var UpdateSignFilePath = _context.powerOfAttorneyInformation.Where(w => w.power_of_attorney_id == power_of_attorney_id).FirstOrDefault();
        //                //if (UpdateSignFilePath != null)
        //                //{
        //                //    dbTable.signed_file_name = powerOfAttorneyInformationModel.signed_file_name;
        //                //    dbTable.signed_file_path = powerOfAttorneyInformationModel.signed_file_path;
        //                //    _context.Entry(dbTable).CurrentValues.SetValues(dbTable);
        //                //    _context.SaveChanges();
        //                //}
        //            }
        //            //if (powerOfAttorneyInformationModel.address_type == "FOREIGN")
        //            //{
        //            //    VerificationType = "EMAILID";
        //            //    // applicantID = FetchApplicantID(applicantMasterModel.emailid, VerificationType);

        //            //    string[] signData = takerData.address.foreignAddress.signatureSrc.Split(",");
        //            //    checkSignFlag = methodForFile.SaveImageForApplicant(signData[1], takerData.address.foreignAddress.signatureName, power_of_attorney_id.ToString(), "Signature", FolderPath + @"\", CurrentDateTime);

        //            //    string SignatureExt = Path.GetExtension(takerData.address.foreignAddress.signatureName);
        //            //    powerOfAttorneyInformationModel.signed_file_name = "Signature" + power_of_attorney_id + "_" + CurrentDateTime + SignatureExt;
        //            //    powerOfAttorneyInformationModel.signed_file_path = FolderPath + @"\" + power_of_attorney_id + @"\" + powerOfAttorneyInformationModel.signed_file_name;
        //            //    var UpdateSignFilePath = _context.powerOfAttorneyInformation.Where(w => w.power_of_attorney_id == power_of_attorney_id).FirstOrDefault();
        //            //    if (UpdateSignFilePath != null)
        //            //    {
        //            //        dbTable.signed_file_name = powerOfAttorneyInformationModel.signed_file_name;
        //            //        dbTable.signed_file_path = powerOfAttorneyInformationModel.signed_file_path;
        //            //        _context.Entry(dbTable).CurrentValues.SetValues(dbTable);
        //            //        _context.SaveChanges();
        //            //    }
        //            //}
        //            //string[] imgData = takerData.photo.passportSrc.Split(",");
        //            //bool checkImgFlag = methodForFile.SaveImageForApplicant(imgData[1], takerData.photo.passportName, power_of_attorney_id.ToString(), "PassportPhoto", FolderPath + @"\", CurrentDateTime);
        //            //string ImgExt = Path.GetExtension(takerData.photo.passportName);
        //            //powerOfAttorneyInformationModel.profile_pic_file_name = "PassportPhoto" + power_of_attorney_id + "_" + CurrentDateTime + ImgExt;
        //            //powerOfAttorneyInformationModel.profile_pic_file_path = FolderPath + @"\" + power_of_attorney_id + @"\" + powerOfAttorneyInformationModel.profile_pic_file_name;

        //            //var UpdateImgFilePath = _context.powerOfAttorneyInformation.Where(w => w.power_of_attorney_id == power_of_attorney_id).FirstOrDefault();
        //            //if (UpdateImgFilePath != null)
        //            //{
        //            //    dbTable.profile_pic_file_name = powerOfAttorneyInformationModel.profile_pic_file_name;
        //            //    dbTable.profile_pic_file_path = powerOfAttorneyInformationModel.profile_pic_file_path;
        //            //    _context.Entry(dbTable).CurrentValues.SetValues(dbTable);
        //            //    _context.SaveChanges();
        //            //}

        //            //if (checkAddressFlag && checkImgFlag && checkSignFlag)
        //            if (checkAddressFlag)
        //            {
        //                _context.SaveChanges();
        //                dbContextTransaction.Commit();
        //                //scope.Complete();
        //                using (var scope1 = new TransactionScope(TransactionScopeOption.Suppress))
        //                {
        //                    var applicationDTLdata = _context.applicationDTL.AsNoTracking().Where(data => data.applicationid!.Equals(takerData.applicationid)).FirstOrDefault();
        //                    if (applicationDTLdata != null)
        //                    {
        //                        if (!string.IsNullOrEmpty(applicationDTLdata.powerOfAttorneyIDs) && !applicationDTLdata.powerOfAttorneyIDs.Contains(power_of_attorney_id.ToString()))
        //                        {
        //                            applicationDTLdata.powerOfAttorneyIDs = applicationDTLdata.powerOfAttorneyIDs + "," + power_of_attorney_id.ToString();
        //                        }
        //                        else
        //                        {
        //                            applicationDTLdata.powerOfAttorneyIDs = power_of_attorney_id.ToString();
        //                        }
        //                        applicationDTLdata.status = 6;
        //                        _context.Entry(applicationDTLdata).CurrentValues.SetValues(applicationDTLdata);
        //                        _context.SaveChanges();
        //                    }
        //                    scope1.Complete();
        //                }
        //                //_context.SaveChanges();
        //                //scope.Complete();
        //                return "Success";
        //            }
        //            else
        //            {
        //                _logger.LogInformation("Test POA-");
        //                //var entity =  _context.powerOfAttorneyInformation
        //                //  .FirstOrDefault(p => p.power_of_attorney_id == power_of_attorney_id);
        //                //if (entity != null)
        //                //{
        //                //    _context.powerOfAttorneyInformation.Remove(dbTable!);
        //                //    _context.SaveChanges();
        //                //}
        //                dbContextTransaction.Rollback();
        //                //scope.Dispose();
        //                //  scope.Complete();
        //                //_context.powerOfAttorneyInformation.Remove(dbTable);
        //                //_context.SaveChanges();
        //                _context.Dispose();
        //                return "Address Proof File Is Not Uploaded";
        //            }
        //            //if (!checkImgFlag)
        //            //{
        //            //    dbContextTransaction.Rollback();
        //            //    return "Profile Picture Is Not Uploaded";
        //            //}
        //            //if (!checkAddressFlag)
        //            //{
        //            //    dbContextTransaction.Rollback();
        //            //    return "Address Proof File Is Not Uploaded";
        //            //}
        //            //if (!checkSignFlag)
        //            //{
        //            //    dbContextTransaction.Rollback();
        //            //    return "Signature File Is Not Uploaded";
        //            //}
        //            //else
        //            //{
        //            //    dbContextTransaction.Rollback();
        //            //    return "Some Files Are Not Uploaded";
        //            //}
        //        }
        //        catch (Exception ex)
        //        {
        //            //_context.powerOfAttorneyInformation.Remove(dbTable);
        //            //_context.SaveChanges();
        //            _logger.LogInformation("Test POA Catch Block - " +ex.StackTrace.ToString());
        //            dbContextTransaction.Rollback();
        //            _context.Dispose();
        //            throw new HandleException(ex.Message.ToString());
        //        }
        //    }
        //}

        //public FetchPOAForTakerData FetchPowerOfAttorneyInfoForTaker(int powerOfAttorneyID)
        //{
        //    try
        //    {
        //        MethodForFileUpload methodForFile = new MethodForFileUpload();
        //        PowerOfAttorneyInformation powerOfAttorneyInformation = new PowerOfAttorneyInformation();
        //        powerOfAttorneyInformation = _contextR.powerOfAttorneyInformation.Include(i => i.userMaster).Include(app => app.applicationDTL).Include(proptype => proptype.propertyType).Where(data => data.power_of_attorney_id.Equals(powerOfAttorneyID) && data.is_taker == true && data.isDeleted == false).FirstOrDefault()!;

        //        FetchPOAForTakerData fetchData = new FetchPOAForTakerData();
        //        List<string> giverNameInMarathiList = new List<string>();
        //        List<string> giverNameInEnglishList = new List<string>();
        //        if (powerOfAttorneyInformation != null)
        //        {
        //            fetchData.power_of_attorney_id = powerOfAttorneyInformation.power_of_attorney_id;
        //            fetchData.userid = powerOfAttorneyInformation.userMaster!.userid;
        //            fetchData.applicationid = powerOfAttorneyInformation.applicationDTL!.applicationid;

        //            fetchData.mobileNo = powerOfAttorneyInformation.mobileno;
        //            fetchData.is_taker = powerOfAttorneyInformation.is_taker;
        //            ViewModel.DharakForPOATaker dharakdata = new ViewModel.DharakForPOATaker();
        //            fetchData.usertype_code = powerOfAttorneyInformation.usertype_code;
        //            fetchData.usertype = powerOfAttorneyInformation.usertype;

        //            if (fetchData.is_taker)
        //            {
        //                if (fetchData.usertype_code == 1)
        //                {
        //                    fetchData.fullNameInMarathi = powerOfAttorneyInformation.fname_in_marathi.Trim() + " " + powerOfAttorneyInformation.mname_in_marathi.Trim() + " " + powerOfAttorneyInformation.lname_in_marathi.Trim();
        //                    fetchData.fullNameInEng = powerOfAttorneyInformation.fname_in_eng.Trim() + " " + powerOfAttorneyInformation.mname_in_eng.Trim() + " " + powerOfAttorneyInformation.lname_in_eng.Trim();
        //                }
        //                else
        //                {
        //                    fetchData.fullNameInMarathi = powerOfAttorneyInformation.company_name_in_marathi.Trim();
        //                    fetchData.fullNameInEng = powerOfAttorneyInformation.company_name_in_eng.Trim();
        //                }
        //            }
        //            //else
        //            //{
        //            //    fetchData.fullNameInMarathi = powerOfAttorneyInformation.fname_in_marathi.Trim() + " " + powerOfAttorneyInformation.mname_in_marathi.Trim() + " " + powerOfAttorneyInformation.lname_in_marathi.Trim();
        //            //    fetchData.fullNameInEng = powerOfAttorneyInformation.fname_in_eng.Trim() + " " + powerOfAttorneyInformation.mname_in_eng.Trim() + " " + powerOfAttorneyInformation.lname_in_eng.Trim();
        //            //}

        //            if (fetchData.usertype_code == 1)
        //            {
        //                ViewModel.UserDharakForPOATaker userDharakData = new ViewModel.UserDharakForPOATaker();
        //                userDharakData.aliceName = powerOfAttorneyInformation.alias_name;
        //                Gender gender = new Gender();
        //                gender.gender_code = powerOfAttorneyInformation.gender_code;
        //                gender.gender_description = powerOfAttorneyInformation.gender_description;
        //                userDharakData.gender = gender;
        //                /*KhataTypeData khataTypeData = new KhataTypeData();
        //                khataTypeData.khataCode = powerOfAttorneyInformation.khata_type_code;
        //                khataTypeData.khataLabel = powerOfAttorneyInformation.khata_type_name;
        //                userDharakData.khataType = khataTypeData;*/

        //                /*HoldertypeData holdertypeData = new HoldertypeData();
        //                holdertypeData.owner_status_code = powerOfAttorneyInformation.owner_status_code;
        //                holdertypeData.owner_status_description = powerOfAttorneyInformation.owner_status_description;
        //                userDharakData.holderType = holdertypeData;*/

        //                userDharakData.dob = powerOfAttorneyInformation.dob;
        //                userDharakData.motherName = powerOfAttorneyInformation.mother_name_in_marathi;
        //                userDharakData.motherNameEng = powerOfAttorneyInformation.mother_name_in_eng;
        //                dharakdata.userdharak = userDharakData;
        //            }
        //            else
        //            {
        //                ViewModel.CompanyDharakForPOATaker companyDharakData = new ViewModel.CompanyDharakForPOATaker();
        //                /* KhataTypeData khataTypeData = new KhataTypeData();
        //                 khataTypeData.khataCode = powerOfAttorneyInformation.khata_type_code;
        //                 khataTypeData.khataLabel = powerOfAttorneyInformation.khata_type_name;
        //                 companyDharakData.khataType = khataTypeData;

        //                 HoldertypeData holdertypeData = new HoldertypeData();
        //                 holdertypeData.owner_status_code = powerOfAttorneyInformation.owner_status_code;
        //                 holdertypeData.owner_status_description = powerOfAttorneyInformation.owner_status_description;
        //                 companyDharakData.holderType = holdertypeData;

        //                 aapakDropdownData aapakDropdownData = new aapakDropdownData();
        //                 aapakDropdownData.apk_code = powerOfAttorneyInformation.apk_code;
        //                 aapakDropdownData.apk_description = powerOfAttorneyInformation.apk_description;
        //                 companyDharakData.aapakDropdown = aapakDropdownData;

        //                 companyDharakData.aapak = powerOfAttorneyInformation.aapak;*/
        //                companyDharakData.landBuyArea = powerOfAttorneyInformation.landBuyArea;
        //                dharakdata.companydharak = companyDharakData;
        //            }
        //            fetchData.dharak = dharakdata;
        //            ViewModel.UserDetailsDataForPOATaker userDetails = new ViewModel.UserDetailsDataForPOATaker();
        //            userDetails.suffix = powerOfAttorneyInformation.prefix_in_marathi;
        //            userDetails.firstName = powerOfAttorneyInformation.fname_in_marathi;
        //            userDetails.middleName = powerOfAttorneyInformation.mname_in_marathi;
        //            userDetails.lastName = powerOfAttorneyInformation.lname_in_marathi;
        //            userDetails.suffixEng = powerOfAttorneyInformation.prefix_in_eng;
        //            userDetails.firstNameEng = powerOfAttorneyInformation.fname_in_eng;
        //            userDetails.middleNameEng = powerOfAttorneyInformation.mname_in_eng;
        //            userDetails.lastNameEng = powerOfAttorneyInformation.lname_in_eng;
        //            userDetails.nabhu = powerOfAttorneyInformation.city_servey_no;
        //            userDetails.userName = powerOfAttorneyInformation.username;
        //            userDetails.lrPropertyUID = powerOfAttorneyInformation.lr_property_id;
        //            userDetails.suffixcode = powerOfAttorneyInformation.prefixcode_marathi;
        //            userDetails.suffixCodeEng = powerOfAttorneyInformation.prefixcode_eng;

        //            if (powerOfAttorneyInformation.owner_of_property_in_maharashtra)
        //            {
        //                DistrictForPOATaker district = new DistrictForPOATaker();
        //                district.district_code = powerOfAttorneyInformation.property_district_code;
        //                district.district_name = powerOfAttorneyInformation.property_district_name_in_marathi;
        //                district.district_english_name = powerOfAttorneyInformation.property_district_name_in_english;
        //                userDetails.district = district;

        //                TalukaForPOATaker taluka = new TalukaForPOATaker();
        //                taluka.office_code = powerOfAttorneyInformation.property_taluka_code;
        //                taluka.office_name = powerOfAttorneyInformation.property_taluka_name;
        //                userDetails.taluka = taluka;

        //                VillageForPOATaker village = new VillageForPOATaker();
        //                village.village_code = powerOfAttorneyInformation.property_city_code;
        //                village.village_name = powerOfAttorneyInformation.property_city_name;
        //                userDetails.village = village;
        //            }

        //            fetchData.userDetails = userDetails;
        //            ViewModel.AddressDataForPOATaker addressData = new ViewModel.AddressDataForPOATaker();
        //            addressData.addressType = powerOfAttorneyInformation.address_type;
        //            if (powerOfAttorneyInformation.address_type == "INDIA")
        //            {
        //                ViewModel.AddressForIndiaForPOATaker addressForIndia = new ViewModel.AddressForIndiaForPOATaker();
        //                addressForIndia.state = powerOfAttorneyInformation.state;
        //                addressForIndia.district = powerOfAttorneyInformation.district;
        //                addressForIndia.city = powerOfAttorneyInformation.city;
        //                addressForIndia.taluka = powerOfAttorneyInformation.taluka;
        //                addressForIndia.plotNo = powerOfAttorneyInformation.flatno_plotno;
        //                addressForIndia.building = powerOfAttorneyInformation.societyname;
        //                addressForIndia.mainRoad = powerOfAttorneyInformation.mainstreet;
        //                addressForIndia.impSymbol = powerOfAttorneyInformation.landmark;
        //                addressForIndia.area = powerOfAttorneyInformation.locality;
        //                addressForIndia.pincode = powerOfAttorneyInformation.pincode;
        //                addressForIndia.postOfficeName = powerOfAttorneyInformation.postofficename;
        //                addressForIndia.addressProofName = powerOfAttorneyInformation.address_proof_document_name;
        //                addressForIndia.mobile = powerOfAttorneyInformation.mobileno;
        //                addressForIndia.mobileOTP = powerOfAttorneyInformation.mobilenoverified;
        //                addressForIndia.signatureName = powerOfAttorneyInformation.signed_file_name;

        //                if (powerOfAttorneyInformation.address_proof_document_path != "NA")
        //                {
        //                    string AddressProofExt = Path.GetExtension(powerOfAttorneyInformation.address_proof_document_path);
        //                    string AddressProof = methodForFile.ConvertImageToBase64(powerOfAttorneyInformation.address_proof_document_path);
        //                    powerOfAttorneyInformation.address_proof_document_path = string.IsNullOrEmpty(AddressProof) ? "NA" : "data:image/" + AddressProofExt.Replace(".", "") + ";base64," + AddressProof;
        //                    addressForIndia.addressProofSrc = powerOfAttorneyInformation.address_proof_document_path;
        //                }
        //                else
        //                {
        //                    addressForIndia.addressProofSrc = powerOfAttorneyInformation.address_proof_document_path;
        //                }

        //                //string SignatureExt = Path.GetExtension(powerOfAttorneyInformation.signed_file_path);
        //                //string Signature = methodForFile.ConvertImageToBase64(powerOfAttorneyInformation.signed_file_path);
        //                //addressForIndia.signatureSrc = string.IsNullOrEmpty(Signature) ? "NA" : "data:image/" + SignatureExt.Replace(".", "") + ";base64," + Signature;
        //                addressForIndia.signatureSrc = powerOfAttorneyInformation.signed_file_path;
        //                addressData.indiaAddress = addressForIndia;
        //            }
        //            else if (powerOfAttorneyInformation.address_type == "FOREIGN")
        //            {
        //                ViewModel.AddressForForeignForPOATaker addressForForeign = new ViewModel.AddressForForeignForPOATaker();
        //                addressForForeign.address = powerOfAttorneyInformation.address;
        //                addressForForeign.mobile = powerOfAttorneyInformation.mobileno;
        //                addressForForeign.email = powerOfAttorneyInformation.emailid;
        //                addressForForeign.emailOTP = powerOfAttorneyInformation.emailidverified;
        //                addressForForeign.signatureName = powerOfAttorneyInformation.signed_file_name;

        //                //string SignatureExt = Path.GetExtension(powerOfAttorneyInformation.signed_file_path);
        //                //string Signature = methodForFile.ConvertImageToBase64(powerOfAttorneyInformation.signed_file_path);
        //                //addressForForeign.signatureSrc= string.IsNullOrEmpty(Signature) ? "NA" : "data:image/" + SignatureExt.Replace(".", "") + ";base64," + Signature;
        //                addressForForeign.signatureSrc = powerOfAttorneyInformation.signed_file_path;
        //                addressData.foreignAddress = addressForForeign;
        //            }
        //            fetchData.address = addressData;

        //            AttroneyTypeForPOATaker attornytype = new AttroneyTypeForPOATaker();
        //            attornytype.poa_type_code = powerOfAttorneyInformation.attornytype_code;
        //            attornytype.poa_type_description = powerOfAttorneyInformation.attornytype_desc;
        //            fetchData.attornytype = attornytype;

        //            Division division = new Division();
        //            division.digcode = powerOfAttorneyInformation.digcode;
        //            division.dig = powerOfAttorneyInformation.digname;
        //            fetchData.division = division;

        //            DastDistrict districtData = new DastDistrict();
        //            districtData.jdrcode = Convert.ToInt32(powerOfAttorneyInformation.poa_district_code);
        //            districtData.jdr = powerOfAttorneyInformation.poa_district_name;
        //            //  districtData.district_english_name = powerOfAttorneyInformation.poa_district_name_in_eng;
        //            fetchData.district = districtData;

        //            Dastregsiter registrarData = new Dastregsiter();
        //            registrarData.srocode = powerOfAttorneyInformation.sro_office_code;
        //            registrarData.sro = powerOfAttorneyInformation.sro_office_name;
        //            fetchData.registrar = registrarData;

        //            fetchData.dastNo = powerOfAttorneyInformation.dast_no;
        //            fetchData.dastNoDate = powerOfAttorneyInformation.dast_no_date;
        //            fetchData.dastNoYear = powerOfAttorneyInformation.dast_no_year;
        //            fetchData.isDastVarified = powerOfAttorneyInformation.isDastVerified;
        //            fetchData.varifiedDastData = powerOfAttorneyInformation.verifieddastData;
        //            fetchData.isPOAisPartofDast = powerOfAttorneyInformation.isPOAisPartofDast;
        //            fetchData.isDeclerationInvolvedInPOA = powerOfAttorneyInformation.isDeclerationInvolvedInPOA;
        //            fetchData.isPOAPermanant = powerOfAttorneyInformation.isPOAPermanant;
        //            fetchData.isTransferRights = powerOfAttorneyInformation.isTransferRights;
        //            fetchData.passportName = powerOfAttorneyInformation.profile_pic_file_name;
        //            fetchData.passportSrc = powerOfAttorneyInformation.profile_pic_file_path;
        //            fetchData.poa_giver_ids = powerOfAttorneyInformation.poa_giver_ids;
        //            //if (!string.IsNullOrEmpty(powerOfAttorneyInformation.profile_pic_file_name) && powerOfAttorneyInformation.profile_pic_file_name != "NA")
        //            //{
        //            //    fetchData.passportName = powerOfAttorneyInformation.profile_pic_file_name;
        //            //    string ProfilePicExt = Path.GetExtension(powerOfAttorneyInformation.profile_pic_file_path);
        //            //    string ProfilePic = methodForFile.ConvertImageToBase64(powerOfAttorneyInformation.profile_pic_file_path);
        //            //    powerOfAttorneyInformation.profile_pic_file_path = string.IsNullOrEmpty(ProfilePic) ? "NA" : "data:image/" + ProfilePicExt.Replace(".", "") + ";base64," + ProfilePic;
        //            //    fetchData.passportSrc = powerOfAttorneyInformation.profile_pic_file_path;
        //            //}
        //            //else
        //            //{
        //            //    fetchData.passportName = powerOfAttorneyInformation.profile_pic_file_name;
        //            //    fetchData.passportSrc = powerOfAttorneyInformation.profile_pic_file_path;
        //            //}
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

        //public int GeneratePowerOfAttorneyCode(string applicationid, bool giver_taker_flag)
        //{
        //    var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", optional: false);
        //    IConfiguration configuration = builder.Build();
        //    string ConnectionString = configuration.GetValue<string>("ConnectionStrings:PDEDBR")!;
        //    try
        //    {
        //        NpgsqlConnection connection = new NpgsqlConnection(ConnectionString);
        //        connection.Open();
        //        NpgsqlCommand npgsqlCommand = new NpgsqlCommand();
        //        npgsqlCommand.Connection = connection;
        //        npgsqlCommand.Parameters.Clear();
        //        npgsqlCommand.CommandType = CommandType.Text;
        //        npgsqlCommand.CommandText = "SELECT CASE WHEN MAX(power_of_attorney_code) IS NULL THEN 1 ELSE MAX(power_of_attorney_code)+1 END FROM public.power_of_attorney_information WHERE \"applicationDTLapplicationid\"='" + applicationid + "' AND is_taker=" + giver_taker_flag;
        //        int PowerOfAttorneyCode = Convert.ToInt32(npgsqlCommand.ExecuteScalar()!.ToString());
        //        npgsqlCommand.Dispose();
        //        connection.Close();
        //        return PowerOfAttorneyCode;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new HandleException(ex.Message.ToString());
        //    }
        //}
        public int GeneratePowerOfAttorneyCode(string applicationid, bool giver_taker_flag)
        {
            //var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", optional: false);
            //IConfiguration configuration = builder.Build();
            //string ConnectionString = configuration.GetValue<string>("ConnectionStrings:PDEDB")!;
            try
            {
                //NpgsqlConnection connection = new NpgsqlConnection(ConnectionString);
                //connection.Open();
                //NpgsqlCommand npgsqlCommand = new NpgsqlCommand();
                //npgsqlCommand.Connection = connection;
                //npgsqlCommand.Parameters.Clear();
                //npgsqlCommand.CommandType = CommandType.Text;
                //npgsqlCommand.CommandText = "SELECT CASE WHEN MAX(power_of_attorney_code) IS NULL THEN 1 ELSE MAX(power_of_attorney_code)+1 END FROM public.power_of_attorney_information WHERE \"applicationDTLapplicationid\"='" + applicationid + "' AND is_taker=" + giver_taker_flag;
                //int PowerOfAttorneyCode = Convert.ToInt32(npgsqlCommand.ExecuteScalar()!.ToString());
                //npgsqlCommand.Dispose();
                //connection.Close();
                using var command = _context.Database.GetDbConnection().CreateCommand();
                command.CommandText = "SELECT CASE WHEN MAX(power_of_attorney_code) IS NULL THEN 1 ELSE MAX(power_of_attorney_code)+1 END FROM public.power_of_attorney_information WHERE \"applicationDTLapplicationid\"='" + applicationid + "' AND is_taker=" + giver_taker_flag;
                _context.Database.OpenConnection();
                int PowerOfAttorneyCode = Convert.ToInt32(command.ExecuteScalar()!.ToString());
                _logger.LogInformation("GeneratePowerOfAttorneyCode POA ID - " + PowerOfAttorneyCode.ToString());
                _context.Database.CloseConnection();
                return PowerOfAttorneyCode;
            }
            catch (Exception ex)
            {
                throw new HandleException(ex.Message.ToString());
            }
        }

        // Edit Applicant Data
        public string EditApplicantData(EditApplicantData editApplicantData)
        {
            MethodForFileUpload methodForFile = new MethodForFileUpload();
            string FolderPath = @"\\10.10.248.2\pde_propertycard\MUTATIONDOCS\" + editApplicantData.applicationId + @"\APPLICANTS\";
            using (var dbContextTransaction = _context.Database.BeginTransaction())
            {
                try
                {
                    ApplicantMaster enitityData = new ApplicantMaster();
                    enitityData = _context.applicantMasters.Include(i => i.PropertyTypeMaster).Include(u => u.userMaster).Where(data => data.applicantid.Equals(editApplicantData.applicantid)).FirstOrDefault()!;
                    if (enitityData != null)
                    {
                        // Assign values to model
                        ApplicantMasterModel applicantMasterModel = new ApplicantMasterModel();
                        applicantMasterModel.usertype_code = editApplicantData.usertype_code;
                        applicantMasterModel.usertype = editApplicantData.usertype!.Trim().ToUpper();
                        applicantMasterModel.profile_pic_file_name = editApplicantData.photo!.passportName!;

                        applicantMasterModel.owner_of_property_in_maharashtra = (editApplicantData.isMHProperty!.hasProperty!.Trim().ToUpper() == "YES") ? true : false;
                        PropertyTypeMaster proptype = _context.propertyTypes.FirstOrDefault(s => s.propertytypeid == Convert.ToInt32(editApplicantData.isMHProperty.propType))!;
                        applicantMasterModel.PropertyTypeMaster = proptype;
                        if (applicantMasterModel.owner_of_property_in_maharashtra)
                        {
                            if (applicantMasterModel.PropertyTypeMaster.propertytypeid == 1 && string.IsNullOrEmpty(editApplicantData.isMHProperty.userDetails!.khataNo))
                            {
                                return "When Property Type Is 7/12 Then Khate No Should Not Be Empty";
                            }
                            if (applicantMasterModel.PropertyTypeMaster.propertytypeid == 2 && string.IsNullOrEmpty(editApplicantData.isMHProperty.userDetails!.naBhu))
                            {
                                return "When Property Type Is Property Card Then City Servey No Should Not Be Empty";
                            }
                            if (applicantMasterModel.PropertyTypeMaster.propertytypeid == 3 && string.IsNullOrEmpty(editApplicantData.isMHProperty.userDetails!.ulpin))
                            {
                                return "When Property Type Is ULPIN Then ULPIN Should Not Be Empty";
                            }
                            applicantMasterModel.khateno = string.IsNullOrEmpty(editApplicantData.isMHProperty.userDetails!.khataNo) ? "NA" : editApplicantData.isMHProperty.userDetails.khataNo;
                            applicantMasterModel.city_servey_no = string.IsNullOrEmpty(editApplicantData.isMHProperty.userDetails.naBhu) ? "NA" : editApplicantData.isMHProperty.userDetails.naBhu;
                            applicantMasterModel.ulpin = string.IsNullOrEmpty(editApplicantData.isMHProperty.userDetails.ulpin) ? "NA" : editApplicantData.isMHProperty.userDetails.ulpin;
                            if (applicantMasterModel.PropertyTypeMaster.propertytypeid == 1)
                            {
                                applicantMasterModel.city_servey_no = "NA";
                                applicantMasterModel.ulpin = "NA";
                            }
                            else if (applicantMasterModel.PropertyTypeMaster.propertytypeid == 2)
                            {
                                applicantMasterModel.khateno = "NA";
                                applicantMasterModel.ulpin = "NA";
                            }
                            else if (applicantMasterModel.PropertyTypeMaster.propertytypeid == 3)
                            {
                                applicantMasterModel.khateno = "NA";
                                applicantMasterModel.city_servey_no = "NA";
                            }
                            applicantMasterModel.property_district_code = editApplicantData.isMHProperty.userDetails.district!.district_code!;
                            applicantMasterModel.property_district_name = editApplicantData.isMHProperty.userDetails.district!.district_name!;

                            applicantMasterModel.property_taluka_code = editApplicantData.isMHProperty.userDetails.taluka!.office_code!;
                            applicantMasterModel.property_taluka_name = editApplicantData.isMHProperty.userDetails.taluka!.office_name!;

                            applicantMasterModel.property_village_code = editApplicantData.isMHProperty.userDetails.village!.village_code!;
                            applicantMasterModel.property_village_name = editApplicantData.isMHProperty.userDetails.village!.village_name!;

                        }
                        else
                        {
                            applicantMasterModel.khateno = "NA";
                            applicantMasterModel.city_servey_no = "NA";
                            applicantMasterModel.ulpin = "NA";
                            applicantMasterModel.property_district_code = "NA";
                            applicantMasterModel.property_district_name = "NA";
                            applicantMasterModel.property_taluka_code = "NA";
                            applicantMasterModel.property_taluka_name = "NA";
                            applicantMasterModel.property_village_code = "NA";
                            applicantMasterModel.property_village_name = "NA";
                            applicantMasterModel.city = "NA";
                        }
                        applicantMasterModel.username = editApplicantData.isMHProperty.userDetails!.userName!;
                        if (editApplicantData.usertype_code == 1)
                        {
                            applicantMasterModel.prefix_in_marathi = editApplicantData.isMHProperty.userDetails.suffix!;
                            applicantMasterModel.fname_in_marathi = editApplicantData.isMHProperty.userDetails.firstName!;
                            applicantMasterModel.mname_in_marathi = editApplicantData.isMHProperty.userDetails.middleName!;
                            applicantMasterModel.lname_in_marathi = editApplicantData.isMHProperty.userDetails.lastName!;
                            applicantMasterModel.prefix_in_eng = editApplicantData.isMHProperty.userDetails.suffixEng!;
                            applicantMasterModel.fname_in_eng = editApplicantData.isMHProperty.userDetails.firstNameEng!;
                            applicantMasterModel.mname_in_eng = editApplicantData.isMHProperty.userDetails.middleNameEng!;
                            applicantMasterModel.lname_in_eng = editApplicantData.isMHProperty.userDetails.lastNameEng!;

                            applicantMasterModel.company_name_in_marathi = "NA";
                            applicantMasterModel.company_name_in_eng = "NA";
                        }
                        //if (editApplicantData.usertype.Trim().ToUpper() == "COMPANY")
                        else
                        {
                            applicantMasterModel.company_name_in_marathi = editApplicantData.isMHProperty.userDetails.companyName!;
                            applicantMasterModel.company_name_in_eng = editApplicantData.isMHProperty.userDetails.companyNameEng!;

                            applicantMasterModel.prefix_in_marathi = "NA";
                            applicantMasterModel.fname_in_marathi = "NA";
                            applicantMasterModel.mname_in_marathi = "NA";
                            applicantMasterModel.lname_in_marathi = "NA";
                            applicantMasterModel.prefix_in_eng = "NA";
                            applicantMasterModel.fname_in_eng = "NA";
                            applicantMasterModel.mname_in_eng = "NA";
                            applicantMasterModel.lname_in_eng = "NA";
                        }
                        applicantMasterModel.address_type = editApplicantData.address!.addressType!.Trim().ToUpper();
                        if (editApplicantData.address.addressType.Trim().ToUpper() == "INDIA")
                        {
                            applicantMasterModel.state = editApplicantData.address.indiaAddress!.state!;
                            applicantMasterModel.district = editApplicantData.address.indiaAddress.district!;
                            applicantMasterModel.city = editApplicantData.address.indiaAddress.state!;
                            applicantMasterModel.taluka = editApplicantData.address.indiaAddress.taluka!;
                            applicantMasterModel.flatno_plotno = editApplicantData.address.indiaAddress.plotNo!;
                            applicantMasterModel.societyname = editApplicantData.address.indiaAddress.building!;
                            applicantMasterModel.mainstreet = editApplicantData.address.indiaAddress.mainRoad!;
                            applicantMasterModel.landmark = editApplicantData.address.indiaAddress.impSymbol!;
                            applicantMasterModel.locality = editApplicantData.address.indiaAddress.area!;
                            applicantMasterModel.pincode = editApplicantData.address.indiaAddress.pincode!;
                            applicantMasterModel.postofficename = editApplicantData.address.indiaAddress.postOfficeName!;
                            applicantMasterModel.address_proof_document_name = editApplicantData.address.indiaAddress.addressProofName!;
                            applicantMasterModel.mobileno = editApplicantData.address.indiaAddress.mobile!;
                            applicantMasterModel.mobilenoverified = editApplicantData.address.indiaAddress.mobileOTP!.Trim().ToUpper();
                            applicantMasterModel.emailid = editApplicantData.address.indiaAddress.email;
                            applicantMasterModel.emailidverified = editApplicantData.address.indiaAddress.emailOTP.Trim().ToUpper();
                            applicantMasterModel.securitypin = editApplicantData.address.indiaAddress.securityKey;
                            applicantMasterModel.signed_file_name = editApplicantData.address.indiaAddress.signatureName!;
                            applicantMasterModel.address = "NA";
                        }
                        else if (editApplicantData.address.addressType.Trim().ToUpper() == "FOREIGN")
                        {
                            applicantMasterModel.address = editApplicantData.address.foreignAddress!.address!;
                            applicantMasterModel.mobileno = editApplicantData.address.foreignAddress.mobile!;
                            applicantMasterModel.mobilenoverified = "NO";
                            applicantMasterModel.address_proof_document_name = "NA";
                            applicantMasterModel.address_proof_document_path = "NA";
                            applicantMasterModel.emailid = editApplicantData.address.foreignAddress.email!;
                            applicantMasterModel.emailidverified = editApplicantData.address.foreignAddress.emailOTP!.Trim().ToUpper();
                            applicantMasterModel.signed_file_name = editApplicantData.address.foreignAddress.signatureName!;

                            applicantMasterModel.securitypin = "NA";
                            applicantMasterModel.state = "NA";
                            applicantMasterModel.district = "NA";
                            applicantMasterModel.city = "NA";
                            applicantMasterModel.taluka = "NA";
                            applicantMasterModel.flatno_plotno = "NA";
                            applicantMasterModel.societyname = "NA";
                            applicantMasterModel.mainstreet = "NA";
                            applicantMasterModel.landmark = "NA";
                            applicantMasterModel.locality = "NA";
                            applicantMasterModel.pincode = "NA";
                            applicantMasterModel.postofficename = "NA";
                        }

                        ApplicationDTL applicationDTL = _context.applicationDTL.FirstOrDefault(s => s.applicationid == editApplicantData.applicationId)!;
                        applicantMasterModel.applicationDTL = applicationDTL;

                        UserMaster userMaster = _context.userMasters.FirstOrDefault(s => s.userid == editApplicantData.userId)!;
                        applicantMasterModel.userMaster = userMaster;


                        //Assign Data to Table fields to insert new records
                        enitityData.usertype_code = applicantMasterModel.usertype_code;
                        enitityData.usertype = applicantMasterModel.usertype;
                        enitityData.mobileno = applicantMasterModel.mobileno;
                        enitityData.mobilenoverified = applicantMasterModel.mobilenoverified;
                        enitityData.emailid = applicantMasterModel.emailid;
                        enitityData.emailidverified = applicantMasterModel.emailidverified;
                        enitityData.securitypin = applicantMasterModel.securitypin;
                        enitityData.prefix_in_eng = string.IsNullOrEmpty(applicantMasterModel.prefix_in_eng) ? "NA" : applicantMasterModel.prefix_in_eng.Trim();
                        enitityData.fname_in_eng = string.IsNullOrEmpty(applicantMasterModel.fname_in_eng) ? "NA" : applicantMasterModel.fname_in_eng.Trim();
                        enitityData.mname_in_eng = string.IsNullOrEmpty(applicantMasterModel.mname_in_eng) ? "NA" : applicantMasterModel.mname_in_eng.Trim();
                        enitityData.lname_in_eng = string.IsNullOrEmpty(applicantMasterModel.lname_in_eng) ? "NA" : applicantMasterModel.lname_in_eng.Trim();
                        enitityData.prefix_in_marathi = string.IsNullOrEmpty(applicantMasterModel.prefix_in_marathi) ? "NA" : applicantMasterModel.prefix_in_marathi.Trim();
                        enitityData.fname_in_marathi = string.IsNullOrEmpty(applicantMasterModel.fname_in_marathi) ? "NA" : applicantMasterModel.fname_in_marathi.Trim();
                        enitityData.mname_in_marathi = string.IsNullOrEmpty(applicantMasterModel.mname_in_marathi) ? "NA" : applicantMasterModel.mname_in_marathi.Trim();
                        enitityData.lname_in_marathi = string.IsNullOrEmpty(applicantMasterModel.lname_in_marathi) ? "NA" : applicantMasterModel.lname_in_marathi.Trim();
                        enitityData.address_type = applicantMasterModel.address_type;
                        enitityData.address = string.IsNullOrEmpty(applicantMasterModel.address) ? "NA" : applicantMasterModel.address;
                        enitityData.state = applicantMasterModel.state;
                        enitityData.district = applicantMasterModel.district;
                        enitityData.taluka = applicantMasterModel.taluka;
                        enitityData.city = applicantMasterModel.city;
                        enitityData.flatno_plotno = applicantMasterModel.flatno_plotno;
                        enitityData.societyname = applicantMasterModel.societyname;
                        enitityData.mainstreet = applicantMasterModel.mainstreet;
                        enitityData.landmark = applicantMasterModel.landmark;
                        enitityData.locality = applicantMasterModel.locality;
                        enitityData.pincode = applicantMasterModel.pincode;
                        enitityData.postofficename = applicantMasterModel.postofficename;
                        enitityData.owner_of_property_in_maharashtra = applicantMasterModel.owner_of_property_in_maharashtra;
                        enitityData.PropertyTypeMaster = applicantMasterModel.PropertyTypeMaster;
                        enitityData.property_district_code = applicantMasterModel.property_district_code;
                        enitityData.property_district_name = applicantMasterModel.property_district_name;
                        enitityData.property_taluka_code = applicantMasterModel.property_taluka_code;
                        enitityData.property_taluka_name = applicantMasterModel.property_taluka_name;
                        enitityData.property_village_code = applicantMasterModel.property_village_code;
                        enitityData.property_village_name = applicantMasterModel.property_village_name;
                        enitityData.khateno = applicantMasterModel.khateno;
                        enitityData.city_servey_no = applicantMasterModel.city_servey_no;
                        enitityData.ulpin = applicantMasterModel.ulpin;
                        enitityData.username = string.IsNullOrEmpty(applicantMasterModel.username) ? "NA" : applicantMasterModel.username;
                        enitityData.company_name_in_eng = string.IsNullOrEmpty(applicantMasterModel.company_name_in_eng) ? "NA" : applicantMasterModel.company_name_in_eng.Trim();
                        enitityData.company_name_in_marathi = string.IsNullOrEmpty(applicantMasterModel.company_name_in_marathi) ? "NA" : applicantMasterModel.company_name_in_marathi.Trim();
                        enitityData.userMaster = applicantMasterModel.userMaster;
                        enitityData.applicationDTL = applicantMasterModel.applicationDTL;
                        _context.applicantMasters.Attach(enitityData);
                        _context.SaveChanges();

                        //Get Saved Row ID
                        int applicantID = enitityData.applicantid;

                        string VerificationType = string.Empty;
                        bool checkAddressFlag = true;
                        bool checkSignFlag = false;
                        string CurrentDateTime = DateTime.Now.ToString("yyyy-MMM-dd-HHmmss");

                        if (applicantMasterModel.address_type == "INDIA")
                        {
                            VerificationType = "MOBILENO";
                            //applicantID = FetchApplicantID(applicantMasterModel.mobileno, VerificationType);

                            //adding special characters validation for document name - 
                            string imageName = System.IO.Path.GetFileNameWithoutExtension(editApplicantData.address.indiaAddress!.addressProofName!);
                            if (methodForFile.ContainsSpecialCharacters(imageName))
                            {
                                return editApplicantData.address.indiaAddress!.addressProofName! + " Uploaded Image name Contains Special Character!";
                            }
                            else
                            {
                                string[] AddressData = editApplicantData.address.indiaAddress!.addressProofSrc!.Split(",");
                                checkAddressFlag = methodForFile.SaveImageForApplicant(AddressData[1], editApplicantData.address.indiaAddress.addressProofName!, applicantID.ToString(), "AddressProof", FolderPath + @"\", CurrentDateTime);
                                string AddressProofExt = Path.GetExtension(editApplicantData.address.indiaAddress.addressProofName!);
                                applicantMasterModel.address_proof_document_name = "AddressProof" + applicantID + "_" + CurrentDateTime + AddressProofExt;
                                applicantMasterModel.address_proof_document_path = FolderPath + applicantID + @"\" + applicantMasterModel.address_proof_document_name;

                                var UpdateAddressFilePath = _context.applicantMasters.Where(w => w.applicantid == applicantID).FirstOrDefault();
                                if (UpdateAddressFilePath != null)
                                {
                                    enitityData.address_proof_document_name = applicantMasterModel.address_proof_document_name;
                                    enitityData.address_proof_document_path = applicantMasterModel.address_proof_document_path;
                                    _context.Entry(enitityData).CurrentValues.SetValues(enitityData);
                                    _context.SaveChanges();
                                }
                            }

                            //sign

                            string imgName = System.IO.Path.GetFileNameWithoutExtension(editApplicantData.address.indiaAddress!.addressProofName!);
                            if (methodForFile.ContainsSpecialCharacters(imgName))
                            {
                                return editApplicantData.address.indiaAddress!.addressProofName! + " Uploaded Image name Contains Special Character!";
                            }
                            else
                            {
                                string[] signData = editApplicantData.address.indiaAddress.signatureSrc!.Split(",");
                                checkSignFlag = methodForFile.SaveImageForApplicant(signData[1], editApplicantData.address.indiaAddress.signatureName!, applicantID.ToString(), "Signature", FolderPath + @"\", CurrentDateTime);
                                string SignatureExt = Path.GetExtension(editApplicantData.address.indiaAddress.signatureName!);
                                applicantMasterModel.signed_file_name = "Signature" + applicantID + "_" + CurrentDateTime + SignatureExt;
                                applicantMasterModel.signed_file_path = FolderPath + applicantID + @"\" + applicantMasterModel.signed_file_name;

                                var UpdateSignFilePath = _context.applicantMasters.Where(w => w.applicantid == applicantID).FirstOrDefault();
                                if (UpdateSignFilePath != null)
                                {
                                    enitityData.signed_file_name = applicantMasterModel.signed_file_name;
                                    enitityData.signed_file_path = applicantMasterModel.signed_file_path;
                                    _context.Entry(enitityData).CurrentValues.SetValues(enitityData);
                                    _context.SaveChanges();
                                }
                            }
                        }
                        if (applicantMasterModel.address_type == "FOREIGN")
                        {
                            VerificationType = "EMAILID";
                            // applicantID = FetchApplicantID(applicantMasterModel.emailid, VerificationType);
                            string imgName = System.IO.Path.GetFileNameWithoutExtension(editApplicantData.address.foreignAddress.signatureName!);
                            if (methodForFile.ContainsSpecialCharacters(imgName))
                            {
                                return editApplicantData.address.foreignAddress.signatureName! + " Uploaded Image name Contains Special Character!";
                            }
                            else
                            {

                                string[] signData = editApplicantData.address!.foreignAddress!.signatureSrc!.Split(",");
                                checkSignFlag = methodForFile.SaveImageForApplicant(signData[1], editApplicantData.address.foreignAddress.signatureName!, applicantID.ToString(), "Signature", FolderPath + @"\", CurrentDateTime);

                                string SignatureExt = Path.GetExtension(editApplicantData.address.foreignAddress.signatureName)!;
                                applicantMasterModel.signed_file_name = "Signature" + applicantID + "_" + CurrentDateTime + SignatureExt;
                                applicantMasterModel.signed_file_path = FolderPath + applicantID + @"\" + applicantMasterModel.signed_file_name;
                                var UpdateSignFilePath = _context.applicantMasters.Where(w => w.applicantid == applicantID).FirstOrDefault();
                                if (UpdateSignFilePath != null)
                                {
                                    enitityData.signed_file_name = applicantMasterModel.signed_file_name;
                                    enitityData.signed_file_path = applicantMasterModel.signed_file_path;
                                    _context.Entry(enitityData).CurrentValues.SetValues(enitityData);
                                    _context.SaveChanges();
                                }
                            }
                        }

                        //passport photo
                        bool checkImgFlag = false;
                        string imgName2 = System.IO.Path.GetFileNameWithoutExtension(editApplicantData.photo.passportName!);
                        if (methodForFile.ContainsSpecialCharacters(imgName2))
                        {
                            return editApplicantData.photo.passportName! + " Uploaded Image name Contains Special Character!";
                        }
                        else
                        {
                            string[] imgData = editApplicantData.photo.passportSrc!.Split(",");
                            checkImgFlag = methodForFile.SaveImageForApplicant(imgData[1], editApplicantData.photo.passportName!, applicantID.ToString(), "PassportPhoto", FolderPath + @"\", CurrentDateTime);
                            string ImgExt = Path.GetExtension(editApplicantData.photo.passportName)!;
                            applicantMasterModel.profile_pic_file_name = "PassportPhoto" + applicantID + "_" + CurrentDateTime + ImgExt;
                            applicantMasterModel.profile_pic_file_path = FolderPath + applicantID + @"\" + applicantMasterModel.profile_pic_file_name;

                            var UpdateImgFilePath = _context.applicantMasters.Where(w => w.applicantid == applicantID).FirstOrDefault();
                            if (UpdateImgFilePath != null)
                            {
                                enitityData.profile_pic_file_name = applicantMasterModel.profile_pic_file_name;
                                enitityData.profile_pic_file_path = applicantMasterModel.profile_pic_file_path;
                                _context.Entry(enitityData).CurrentValues.SetValues(enitityData);
                                _context.SaveChanges();
                            }
                        }

                        if (checkAddressFlag && checkImgFlag && checkSignFlag)
                        {
                            _context.SaveChanges();
                            dbContextTransaction.Commit();
                            dbContextTransaction.Dispose();
                            return "Success";
                        }
                        if (!checkImgFlag)
                        {
                            _context.SaveChanges();
                            dbContextTransaction.Rollback();
                            dbContextTransaction.Dispose();
                            methodForFile.DeleteFile(applicantMasterModel.address_proof_document_path);
                            methodForFile.DeleteFile(applicantMasterModel.signed_file_path);
                            return "Profile Picture Is Not Uploaded";
                        }
                        if (!checkAddressFlag)
                        {
                            _context.SaveChanges();
                            dbContextTransaction.Rollback();
                            dbContextTransaction.Dispose();
                            methodForFile.DeleteFile(applicantMasterModel.profile_pic_file_path);
                            methodForFile.DeleteFile(applicantMasterModel.signed_file_path);
                            return "Address Proof File Is Not Uploaded";
                        }
                        if (!checkSignFlag)
                        {
                            _context.SaveChanges();
                            dbContextTransaction.Rollback();
                            dbContextTransaction.Dispose();
                            methodForFile.DeleteFile(applicantMasterModel.profile_pic_file_path);
                            methodForFile.DeleteFile(applicantMasterModel.address_proof_document_path);
                            return "Signature File Is Not Uploaded";
                        }
                        else
                        {
                            _context.SaveChanges();
                            dbContextTransaction.Rollback();
                            dbContextTransaction.Dispose();
                            return "Some Files Are Not Uploaded";
                        }
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
        //public string EditApplicantData(EditApplicantData editApplicantData)
        //{
        //    MethodForFileUpload methodForFile = new MethodForFileUpload();
        //    string FolderPath = @"\\10.10.248.2\pde_propertycard\MUTATIONDOCS\" + editApplicantData.applicationId + @"\APPLICANTS\";
        //    using (var dbContextTransaction = _context.Database.BeginTransaction())
        //    {
        //        try
        //        {
        //            ApplicantMaster enitityData = new ApplicantMaster();
        //            enitityData = _context.applicantMasters.Include(i => i.PropertyTypeMaster).Include(u => u.userMaster).Where(data => data.applicantid.Equals(editApplicantData.applicantid)).FirstOrDefault()!;
        //            if (enitityData != null)
        //            {
        //                // Assign values to model
        //                ApplicantMasterModel applicantMasterModel = new ApplicantMasterModel();
        //                applicantMasterModel.usertype_code = editApplicantData.usertype_code;
        //                applicantMasterModel.usertype = editApplicantData.usertype!.Trim().ToUpper();
        //                applicantMasterModel.profile_pic_file_name = editApplicantData.photo!.passportName!;

        //                applicantMasterModel.owner_of_property_in_maharashtra = (editApplicantData.isMHProperty!.hasProperty!.Trim().ToUpper() == "YES") ? true : false;
        //                PropertyTypeMaster proptype = _context.propertyTypes.FirstOrDefault(s => s.propertytypeid == Convert.ToInt32(editApplicantData.isMHProperty.propType))!;
        //                applicantMasterModel.PropertyTypeMaster = proptype;
        //                if (applicantMasterModel.owner_of_property_in_maharashtra)
        //                {
        //                    if (applicantMasterModel.PropertyTypeMaster.propertytypeid == 1 && string.IsNullOrEmpty(editApplicantData.isMHProperty.userDetails!.khataNo))
        //                    {
        //                        return "When Property Type Is 7/12 Then Khate No Should Not Be Empty";
        //                    }
        //                    if (applicantMasterModel.PropertyTypeMaster.propertytypeid == 2 && string.IsNullOrEmpty(editApplicantData.isMHProperty.userDetails!.naBhu))
        //                    {
        //                        return "When Property Type Is Property Card Then City Servey No Should Not Be Empty";
        //                    }
        //                    if (applicantMasterModel.PropertyTypeMaster.propertytypeid == 3 && string.IsNullOrEmpty(editApplicantData.isMHProperty.userDetails!.ulpin))
        //                    {
        //                        return "When Property Type Is ULPIN Then ULPIN Should Not Be Empty";
        //                    }
        //                    applicantMasterModel.khateno = string.IsNullOrEmpty(editApplicantData.isMHProperty.userDetails!.khataNo) ? "NA" : editApplicantData.isMHProperty.userDetails.khataNo;
        //                    applicantMasterModel.city_servey_no = string.IsNullOrEmpty(editApplicantData.isMHProperty.userDetails.naBhu) ? "NA" : editApplicantData.isMHProperty.userDetails.naBhu;
        //                    applicantMasterModel.ulpin = string.IsNullOrEmpty(editApplicantData.isMHProperty.userDetails.ulpin) ? "NA" : editApplicantData.isMHProperty.userDetails.ulpin;
        //                    if (applicantMasterModel.PropertyTypeMaster.propertytypeid == 1)
        //                    {
        //                        applicantMasterModel.city_servey_no = "NA";
        //                        applicantMasterModel.ulpin = "NA";
        //                    }
        //                    else if (applicantMasterModel.PropertyTypeMaster.propertytypeid == 2)
        //                    {
        //                        applicantMasterModel.khateno = "NA";
        //                        applicantMasterModel.ulpin = "NA";
        //                    }
        //                    else if (applicantMasterModel.PropertyTypeMaster.propertytypeid == 3)
        //                    {
        //                        applicantMasterModel.khateno = "NA";
        //                        applicantMasterModel.city_servey_no = "NA";
        //                    }
        //                    applicantMasterModel.property_district_code = editApplicantData.isMHProperty.userDetails.district!.district_code!;
        //                    applicantMasterModel.property_district_name = editApplicantData.isMHProperty.userDetails.district!.district_name!;

        //                    applicantMasterModel.property_taluka_code = editApplicantData.isMHProperty.userDetails.taluka!.office_code!;
        //                    applicantMasterModel.property_taluka_name = editApplicantData.isMHProperty.userDetails.taluka!.office_name!;

        //                    applicantMasterModel.property_village_code = editApplicantData.isMHProperty.userDetails.village!.village_code!;
        //                    applicantMasterModel.property_village_name = editApplicantData.isMHProperty.userDetails.village!.village_name!;

        //                }
        //                else
        //                {
        //                    applicantMasterModel.khateno = "NA";
        //                    applicantMasterModel.city_servey_no = "NA";
        //                    applicantMasterModel.ulpin = "NA";
        //                    applicantMasterModel.property_district_code = "NA";
        //                    applicantMasterModel.property_district_name = "NA";
        //                    applicantMasterModel.property_taluka_code = "NA";
        //                    applicantMasterModel.property_taluka_name = "NA";
        //                    applicantMasterModel.property_village_code = "NA";
        //                    applicantMasterModel.property_village_name = "NA";
        //                    applicantMasterModel.city = "NA";
        //                }
        //                applicantMasterModel.username = editApplicantData.isMHProperty.userDetails!.userName!;
        //                if (editApplicantData.usertype_code == 1)
        //                {
        //                    applicantMasterModel.prefix_in_marathi = editApplicantData.isMHProperty.userDetails.suffix!;
        //                    applicantMasterModel.fname_in_marathi = editApplicantData.isMHProperty.userDetails.firstName!;
        //                    applicantMasterModel.mname_in_marathi = editApplicantData.isMHProperty.userDetails.middleName!;
        //                    applicantMasterModel.lname_in_marathi = editApplicantData.isMHProperty.userDetails.lastName!;
        //                    applicantMasterModel.prefix_in_eng = editApplicantData.isMHProperty.userDetails.suffixEng!;
        //                    applicantMasterModel.fname_in_eng = editApplicantData.isMHProperty.userDetails.firstNameEng!;
        //                    applicantMasterModel.mname_in_eng = editApplicantData.isMHProperty.userDetails.middleNameEng!;
        //                    applicantMasterModel.lname_in_eng = editApplicantData.isMHProperty.userDetails.lastNameEng!;

        //                    applicantMasterModel.company_name_in_marathi = "NA";
        //                    applicantMasterModel.company_name_in_eng = "NA";
        //                }
        //                //if (editApplicantData.usertype.Trim().ToUpper() == "COMPANY")
        //                else
        //                {
        //                    applicantMasterModel.company_name_in_marathi = editApplicantData.isMHProperty.userDetails.companyName!;
        //                    applicantMasterModel.company_name_in_eng = editApplicantData.isMHProperty.userDetails.companyNameEng!;

        //                    applicantMasterModel.prefix_in_marathi = "NA";
        //                    applicantMasterModel.fname_in_marathi = "NA";
        //                    applicantMasterModel.mname_in_marathi = "NA";
        //                    applicantMasterModel.lname_in_marathi = "NA";
        //                    applicantMasterModel.prefix_in_eng = "NA";
        //                    applicantMasterModel.fname_in_eng = "NA";
        //                    applicantMasterModel.mname_in_eng = "NA";
        //                    applicantMasterModel.lname_in_eng = "NA";
        //                }
        //                applicantMasterModel.address_type = editApplicantData.address!.addressType!.Trim().ToUpper();
        //                if (editApplicantData.address.addressType.Trim().ToUpper() == "INDIA")
        //                {
        //                    applicantMasterModel.state = editApplicantData.address.indiaAddress!.state!;
        //                    applicantMasterModel.district = editApplicantData.address.indiaAddress.district!;
        //                    applicantMasterModel.city = editApplicantData.address.indiaAddress.state!;
        //                    applicantMasterModel.taluka = editApplicantData.address.indiaAddress.taluka!;
        //                    applicantMasterModel.flatno_plotno = editApplicantData.address.indiaAddress.plotNo!;
        //                    applicantMasterModel.societyname = editApplicantData.address.indiaAddress.building!;
        //                    applicantMasterModel.mainstreet = editApplicantData.address.indiaAddress.mainRoad!;
        //                    applicantMasterModel.landmark = editApplicantData.address.indiaAddress.impSymbol!;
        //                    applicantMasterModel.locality = editApplicantData.address.indiaAddress.area!;
        //                    applicantMasterModel.pincode = editApplicantData.address.indiaAddress.pincode!;
        //                    applicantMasterModel.postofficename = editApplicantData.address.indiaAddress.postOfficeName!;
        //                    applicantMasterModel.address_proof_document_name = editApplicantData.address.indiaAddress.addressProofName!;
        //                    applicantMasterModel.mobileno = editApplicantData.address.indiaAddress.mobile!;
        //                    applicantMasterModel.mobilenoverified = editApplicantData.address.indiaAddress.mobileOTP!.Trim().ToUpper();
        //                    applicantMasterModel.emailid = editApplicantData.address.indiaAddress.email;
        //                    applicantMasterModel.emailidverified = editApplicantData.address.indiaAddress.emailOTP.Trim().ToUpper();
        //                    applicantMasterModel.securitypin = editApplicantData.address.indiaAddress.securityKey;
        //                    applicantMasterModel.signed_file_name = editApplicantData.address.indiaAddress.signatureName!;
        //                    applicantMasterModel.address = "NA";
        //                }
        //                else if (editApplicantData.address.addressType.Trim().ToUpper() == "FOREIGN")
        //                {
        //                    applicantMasterModel.address = editApplicantData.address.foreignAddress!.address!;
        //                    applicantMasterModel.mobileno = editApplicantData.address.foreignAddress.mobile!;
        //                    applicantMasterModel.mobilenoverified = "NO";
        //                    applicantMasterModel.address_proof_document_name = "NA";
        //                    applicantMasterModel.address_proof_document_path = "NA";
        //                    applicantMasterModel.emailid = editApplicantData.address.foreignAddress.email!;
        //                    applicantMasterModel.emailidverified = editApplicantData.address.foreignAddress.emailOTP!.Trim().ToUpper();
        //                    applicantMasterModel.signed_file_name = editApplicantData.address.foreignAddress.signatureName!;

        //                    applicantMasterModel.securitypin = "NA";
        //                    applicantMasterModel.state = "NA";
        //                    applicantMasterModel.district = "NA";
        //                    applicantMasterModel.city = "NA";
        //                    applicantMasterModel.taluka = "NA";
        //                    applicantMasterModel.flatno_plotno = "NA";
        //                    applicantMasterModel.societyname = "NA";
        //                    applicantMasterModel.mainstreet = "NA";
        //                    applicantMasterModel.landmark = "NA";
        //                    applicantMasterModel.locality = "NA";
        //                    applicantMasterModel.pincode = "NA";
        //                    applicantMasterModel.postofficename = "NA";
        //                }

        //                ApplicationDTL applicationDTL = _context.applicationDTL.FirstOrDefault(s => s.applicationid == editApplicantData.applicationId)!;
        //                applicantMasterModel.applicationDTL = applicationDTL;

        //                UserMaster userMaster = _context.userMasters.FirstOrDefault(s => s.userid == editApplicantData.userId)!;
        //                applicantMasterModel.userMaster = userMaster;


        //                //Assign Data to Table fields to insert new records
        //                enitityData.usertype_code = applicantMasterModel.usertype_code;
        //                enitityData.usertype = applicantMasterModel.usertype;
        //                enitityData.mobileno = applicantMasterModel.mobileno;
        //                enitityData.mobilenoverified = applicantMasterModel.mobilenoverified;
        //                enitityData.emailid = applicantMasterModel.emailid;
        //                enitityData.emailidverified = applicantMasterModel.emailidverified;
        //                enitityData.securitypin = applicantMasterModel.securitypin;
        //                enitityData.prefix_in_eng = string.IsNullOrEmpty(applicantMasterModel.prefix_in_eng) ? "NA" : applicantMasterModel.prefix_in_eng.Trim();
        //                enitityData.fname_in_eng = string.IsNullOrEmpty(applicantMasterModel.fname_in_eng) ? "NA" : applicantMasterModel.fname_in_eng.Trim();
        //                enitityData.mname_in_eng = string.IsNullOrEmpty(applicantMasterModel.mname_in_eng) ? "NA" : applicantMasterModel.mname_in_eng.Trim();
        //                enitityData.lname_in_eng = string.IsNullOrEmpty(applicantMasterModel.lname_in_eng) ? "NA" : applicantMasterModel.lname_in_eng.Trim();
        //                enitityData.prefix_in_marathi = string.IsNullOrEmpty(applicantMasterModel.prefix_in_marathi) ? "NA" : applicantMasterModel.prefix_in_marathi.Trim();
        //                enitityData.fname_in_marathi = string.IsNullOrEmpty(applicantMasterModel.fname_in_marathi) ? "NA" : applicantMasterModel.fname_in_marathi.Trim();
        //                enitityData.mname_in_marathi = string.IsNullOrEmpty(applicantMasterModel.mname_in_marathi) ? "NA" : applicantMasterModel.mname_in_marathi.Trim();
        //                enitityData.lname_in_marathi = string.IsNullOrEmpty(applicantMasterModel.lname_in_marathi) ? "NA" : applicantMasterModel.lname_in_marathi.Trim();
        //                enitityData.address_type = applicantMasterModel.address_type;
        //                enitityData.address = string.IsNullOrEmpty(applicantMasterModel.address) ? "NA" : applicantMasterModel.address;
        //                enitityData.state = applicantMasterModel.state;
        //                enitityData.district = applicantMasterModel.district;
        //                enitityData.taluka = applicantMasterModel.taluka;
        //                enitityData.city = applicantMasterModel.city;
        //                enitityData.flatno_plotno = applicantMasterModel.flatno_plotno;
        //                enitityData.societyname = applicantMasterModel.societyname;
        //                enitityData.mainstreet = applicantMasterModel.mainstreet;
        //                enitityData.landmark = applicantMasterModel.landmark;
        //                enitityData.locality = applicantMasterModel.locality;
        //                enitityData.pincode = applicantMasterModel.pincode;
        //                enitityData.postofficename = applicantMasterModel.postofficename;
        //                enitityData.owner_of_property_in_maharashtra = applicantMasterModel.owner_of_property_in_maharashtra;
        //                enitityData.PropertyTypeMaster = applicantMasterModel.PropertyTypeMaster;
        //                enitityData.property_district_code = applicantMasterModel.property_district_code;
        //                enitityData.property_district_name = applicantMasterModel.property_district_name;
        //                enitityData.property_taluka_code = applicantMasterModel.property_taluka_code;
        //                enitityData.property_taluka_name = applicantMasterModel.property_taluka_name;
        //                enitityData.property_village_code = applicantMasterModel.property_village_code;
        //                enitityData.property_village_name = applicantMasterModel.property_village_name;
        //                enitityData.khateno = applicantMasterModel.khateno;
        //                enitityData.city_servey_no = applicantMasterModel.city_servey_no;
        //                enitityData.ulpin = applicantMasterModel.ulpin;
        //                enitityData.username = string.IsNullOrEmpty(applicantMasterModel.username) ? "NA" : applicantMasterModel.username;
        //                enitityData.company_name_in_eng = string.IsNullOrEmpty(applicantMasterModel.company_name_in_eng) ? "NA" : applicantMasterModel.company_name_in_eng.Trim();
        //                enitityData.company_name_in_marathi = string.IsNullOrEmpty(applicantMasterModel.company_name_in_marathi) ? "NA" : applicantMasterModel.company_name_in_marathi.Trim();
        //                enitityData.userMaster = applicantMasterModel.userMaster;
        //                enitityData.applicationDTL = applicantMasterModel.applicationDTL;
        //                _context.applicantMasters.Attach(enitityData);
        //                _context.SaveChanges();

        //                //Get Saved Row ID
        //                int applicantID = enitityData.applicantid;

        //                string VerificationType = string.Empty;
        //                bool checkAddressFlag = true;
        //                bool checkSignFlag = false;
        //                string CurrentDateTime = DateTime.Now.ToString("yyyy-MMM-dd-HHmmss");

        //                if (applicantMasterModel.address_type == "INDIA")
        //                {
        //                    VerificationType = "MOBILENO";
        //                    //applicantID = FetchApplicantID(applicantMasterModel.mobileno, VerificationType);
        //                    string[] AddressData = editApplicantData.address.indiaAddress!.addressProofSrc!.Split(",");
        //                    checkAddressFlag = methodForFile.SaveImageForApplicant(AddressData[1], editApplicantData.address.indiaAddress.addressProofName!, applicantID.ToString(), "AddressProof", FolderPath + @"\", CurrentDateTime);
        //                    string AddressProofExt = Path.GetExtension(editApplicantData.address.indiaAddress.addressProofName!);
        //                    applicantMasterModel.address_proof_document_name = "AddressProof" + applicantID + "_" + CurrentDateTime + AddressProofExt;
        //                    applicantMasterModel.address_proof_document_path = FolderPath +  applicantID + @"\" + applicantMasterModel.address_proof_document_name;

        //                    var UpdateAddressFilePath = _context.applicantMasters.Where(w => w.applicantid == applicantID).FirstOrDefault();
        //                    if (UpdateAddressFilePath != null)
        //                    {
        //                        enitityData.address_proof_document_name = applicantMasterModel.address_proof_document_name;
        //                        enitityData.address_proof_document_path = applicantMasterModel.address_proof_document_path;
        //                        _context.Entry(enitityData).CurrentValues.SetValues(enitityData);
        //                        _context.SaveChanges();
        //                    }

        //                    string[] signData = editApplicantData.address.indiaAddress.signatureSrc!.Split(",");
        //                    checkSignFlag = methodForFile.SaveImageForApplicant(signData[1], editApplicantData.address.indiaAddress.signatureName!, applicantID.ToString(), "Signature", FolderPath + @"\", CurrentDateTime);
        //                    string SignatureExt = Path.GetExtension(editApplicantData.address.indiaAddress.signatureName!);
        //                    applicantMasterModel.signed_file_name = "Signature" + applicantID + "_" + CurrentDateTime + SignatureExt;
        //                    applicantMasterModel.signed_file_path = FolderPath + applicantID + @"\" + applicantMasterModel.signed_file_name;

        //                    var UpdateSignFilePath = _context.applicantMasters.Where(w => w.applicantid == applicantID).FirstOrDefault();
        //                    if (UpdateSignFilePath != null)
        //                    {
        //                        enitityData.signed_file_name = applicantMasterModel.signed_file_name;
        //                        enitityData.signed_file_path = applicantMasterModel.signed_file_path;
        //                        _context.Entry(enitityData).CurrentValues.SetValues(enitityData);
        //                        _context.SaveChanges();
        //                    }
        //                }
        //                if (applicantMasterModel.address_type == "FOREIGN")
        //                {
        //                    VerificationType = "EMAILID";
        //                    // applicantID = FetchApplicantID(applicantMasterModel.emailid, VerificationType);

        //                    string[] signData = editApplicantData.address!.foreignAddress!.signatureSrc!.Split(",");
        //                    checkSignFlag = methodForFile.SaveImageForApplicant(signData[1], editApplicantData.address.foreignAddress.signatureName!, applicantID.ToString(), "Signature", FolderPath + @"\", CurrentDateTime);

        //                    string SignatureExt = Path.GetExtension(editApplicantData.address.foreignAddress.signatureName)!;
        //                    applicantMasterModel.signed_file_name = "Signature" + applicantID + "_" + CurrentDateTime + SignatureExt;
        //                    applicantMasterModel.signed_file_path = FolderPath  + applicantID + @"\" + applicantMasterModel.signed_file_name;
        //                    var UpdateSignFilePath = _context.applicantMasters.Where(w => w.applicantid == applicantID).FirstOrDefault();
        //                    if (UpdateSignFilePath != null)
        //                    {
        //                        enitityData.signed_file_name = applicantMasterModel.signed_file_name;
        //                        enitityData.signed_file_path = applicantMasterModel.signed_file_path;
        //                        _context.Entry(enitityData).CurrentValues.SetValues(enitityData);
        //                        _context.SaveChanges();
        //                    }
        //                }
        //                string[] imgData = editApplicantData.photo.passportSrc!.Split(",");
        //                bool checkImgFlag = methodForFile.SaveImageForApplicant(imgData[1], editApplicantData.photo.passportName!, applicantID.ToString(), "PassportPhoto", FolderPath + @"\", CurrentDateTime);
        //                string ImgExt = Path.GetExtension(editApplicantData.photo.passportName)!;
        //                applicantMasterModel.profile_pic_file_name = "PassportPhoto" + applicantID + "_" + CurrentDateTime + ImgExt;
        //                applicantMasterModel.profile_pic_file_path = FolderPath +  applicantID + @"\" + applicantMasterModel.profile_pic_file_name;

        //                var UpdateImgFilePath = _context.applicantMasters.Where(w => w.applicantid == applicantID).FirstOrDefault();
        //                if (UpdateImgFilePath != null)
        //                {
        //                    enitityData.profile_pic_file_name = applicantMasterModel.profile_pic_file_name;
        //                    enitityData.profile_pic_file_path = applicantMasterModel.profile_pic_file_path;
        //                    _context.Entry(enitityData).CurrentValues.SetValues(enitityData);
        //                    _context.SaveChanges();
        //                }

        //                if (checkAddressFlag && checkImgFlag && checkSignFlag)
        //                {
        //                    _context.SaveChanges();
        //                    dbContextTransaction.Commit();
        //                    dbContextTransaction.Dispose();
        //                    return "Success";
        //                }
        //                if (!checkImgFlag)
        //                {
        //                    _context.SaveChanges();
        //                    dbContextTransaction.Rollback();
        //                    dbContextTransaction.Dispose();
        //                    methodForFile.DeleteFile(applicantMasterModel.address_proof_document_path);
        //                    methodForFile.DeleteFile(applicantMasterModel.signed_file_path);
        //                    return "Profile Picture Is Not Uploaded";
        //                }
        //                if (!checkAddressFlag)
        //                {
        //                    _context.SaveChanges();
        //                    dbContextTransaction.Rollback();
        //                    dbContextTransaction.Dispose();
        //                    methodForFile.DeleteFile(applicantMasterModel.profile_pic_file_path);
        //                    methodForFile.DeleteFile(applicantMasterModel.signed_file_path);
        //                    return "Address Proof File Is Not Uploaded";
        //                }
        //                if (!checkSignFlag)
        //                {
        //                    _context.SaveChanges();
        //                    dbContextTransaction.Rollback();
        //                    dbContextTransaction.Dispose();
        //                    methodForFile.DeleteFile(applicantMasterModel.profile_pic_file_path);
        //                    methodForFile.DeleteFile(applicantMasterModel.address_proof_document_path);
        //                    return "Signature File Is Not Uploaded";
        //                }
        //                else
        //                {
        //                    _context.SaveChanges();
        //                    dbContextTransaction.Rollback();
        //                    dbContextTransaction.Dispose();
        //                    return "Some Files Are Not Uploaded";
        //                }
        //            }
        //            else
        //            {
        //                return "Data Not Found";
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            dbContextTransaction.Rollback();
        //            dbContextTransaction.Dispose();
        //            throw new HandleException(ex.Message.ToString());
        //        }
        //    }
        //}

        /*   public string EditMutationCTSNoData(EditMutationCTSNoData mutationCTSNoData)
           {
               //using (var dbContextTransaction = _context.Database.BeginTransaction())
               //{
               try
               {
                   MutationCTSNoDTL entityData = new MutationCTSNoDTL();
                   entityData = _context.mutationCTSNoDTLs.Include(app => app.applicationDTL).Where(data => data.mutation_cts_no_id.Equals(mutationCTSNoData.mutation_cts_no_id)).FirstOrDefault()!;
                   if (entityData != null)
                   {
                       MutationCTSNoDTL fetchMutationCTSNoDTL = new MutationCTSNoDTL();
                       fetchMutationCTSNoDTL = _context.mutationCTSNoDTLs.Include(app => app.applicationDTL).Where(data => data.applicationDTL!.applicationid!.Equals(mutationCTSNoData.applicationid)
                       && data.village_or_peth_code != mutationCTSNoData.village!.village_code
                       ).FirstOrDefault()!;
                       if (fetchMutationCTSNoDTL != null)
                       {
                           return "Could Not Add Another Village For The Same Application";
                       }
                       // Assign Values to Model
                       MutationCTSNoDTLModel mutationCTSNoDTLModel = new MutationCTSNoDTLModel();
                       UserMaster userMaster = _context.userMasters.FirstOrDefault(s => s.userid == mutationCTSNoData.userid)!;
                       mutationCTSNoDTLModel.userMaster = userMaster;

                       ApplicationDTL applicationDTL = _context.applicationDTL.FirstOrDefault(s => s.applicationid == mutationCTSNoData.applicationid)!;
                       mutationCTSNoDTLModel.applicationDTL = applicationDTL;

                       if (mutationCTSNoData.inDast == "surveynoorgatno")
                       {
                           mutationCTSNoDTLModel.what_is_mentioned_in_the_doc = "SERVEYNO / GROUPNO";
                           mutationCTSNoDTLModel.servey_no = mutationCTSNoData.surveyNo;
                       }
                       if (mutationCTSNoData.inDast == "nabhu")
                       {
                           mutationCTSNoDTLModel.what_is_mentioned_in_the_doc = "CITY SERVEY NO";
                           mutationCTSNoDTLModel.city_servey_no_mentioned_in_application = mutationCTSNoData.nabhuNo;
                       }
                       mutationCTSNoDTLModel.village_or_peth_name = textInfo.ToTitleCase(mutationCTSNoData.village!.village_name!.Trim());
                       mutationCTSNoDTLModel.village_or_peth_code = mutationCTSNoData.village.village_code!.Trim();
                       mutationCTSNoDTLModel.mutation_modification_type = mutationCTSNoData.milkat!.Trim().ToUpper();
                       mutationCTSNoDTLModel.selected_city_servey_no = mutationCTSNoData.naBhu;
                       mutationCTSNoDTLModel.lr_property_uid = mutationCTSNoData.lrPropertyUID;
                       mutationCTSNoDTLModel.application_income_type = mutationCTSNoData.namud!.Trim();
                       mutationCTSNoDTLModel.city_servey_area_in_sq_m = mutationCTSNoData.cityServeyAreaInSqm;
                       if (mutationCTSNoData.flatDetails != null && mutationCTSNoDTLModel.application_income_type.Trim().ToUpper() == "OTHER")
                       {
                           mutationCTSNoDTLModel.building_name = string.IsNullOrEmpty(mutationCTSNoData.flatDetails.buildingName) ? "NA" : mutationCTSNoData.flatDetails.buildingName.Trim();
                           mutationCTSNoDTLModel.floor_type = string.IsNullOrEmpty(mutationCTSNoData.flatDetails.floorType) ? "NA" : mutationCTSNoData.flatDetails.floorType.Trim();
                           mutationCTSNoDTLModel.floor_no = string.IsNullOrEmpty(mutationCTSNoData.flatDetails.floorNo) ? "NA" : mutationCTSNoData.flatDetails.floorNo.Trim();
                           mutationCTSNoDTLModel.unit_type = string.IsNullOrEmpty(mutationCTSNoData.flatDetails.unitType) ? "NA" : mutationCTSNoData.flatDetails.unitType.Trim();
                           mutationCTSNoDTLModel.unit_no = string.IsNullOrEmpty(mutationCTSNoData.flatDetails.unitNo) ? "NA" : mutationCTSNoData.flatDetails.unitNo.Trim();
                           mutationCTSNoDTLModel.carpet_area_in_sq_m = string.IsNullOrEmpty(mutationCTSNoData.flatDetails.carpetArea) ? "NA" : mutationCTSNoData.flatDetails.carpetArea.Trim();
                           mutationCTSNoDTLModel.terrace_area_in_sq_m = string.IsNullOrEmpty(mutationCTSNoData.flatDetails.taraceArea) ? "NA" : mutationCTSNoData.flatDetails.taraceArea.Trim();
                           mutationCTSNoDTLModel.parking_no = string.IsNullOrEmpty(mutationCTSNoData.flatDetails.parkingNo) ? "NA" : mutationCTSNoData.flatDetails.parkingNo.Trim();
                           mutationCTSNoDTLModel.parking_area_in_sq_m = string.IsNullOrEmpty(mutationCTSNoData.flatDetails.parkingArea) ? "NA" : mutationCTSNoData.flatDetails.parkingArea.Trim();
                           mutationCTSNoDTLModel.shares_in_percent = string.IsNullOrEmpty(mutationCTSNoData.flatDetails.hissa) ? "NA" : mutationCTSNoData.flatDetails.hissa.Trim();
                           mutationCTSNoDTLModel.buildup_area_in_sq_m = string.IsNullOrEmpty(mutationCTSNoData.flatDetails.buildupArea) ? "NA" : mutationCTSNoData.flatDetails.buildupArea.Trim();
                       }
                       if (mutationCTSNoDTLModel.application_income_type.Trim().ToUpper() != "OTHER")
                       {
                           mutationCTSNoDTLModel.nic_flat_details = mutationCTSNoData.nic_flat_details;
                       }
                       //Assign Data to Table fields to insert new records
                       //Set Default Values

                       entityData.what_is_mentioned_in_the_doc = "NA";
                       entityData.village_or_peth_code = "NA";
                       entityData.village_or_peth_name = "NA";
                       entityData.mutation_modification_type = "NA";
                       entityData.city_servey_no_mentioned_in_application = "NA";
                       entityData.servey_no = "NA";
                       entityData.selected_city_servey_no = "NA";
                       entityData.lr_property_uid = "NA";
                       entityData.application_income_type = "NA";
                       entityData.city_servey_area_in_sq_m = "NA";
                       entityData.building_name = "NA";
                       entityData.floor_type = "NA";
                       entityData.floor_no = "NA";
                       entityData.unit_type = "NA";
                       entityData.unit_no = "NA";
                       entityData.buildup_area_in_sq_m = "NA";
                       entityData.carpet_area_in_sq_m = "NA";
                       entityData.terrace_area_in_sq_m = "NA";
                       entityData.parking_area_in_sq_m = "NA";
                       entityData.parking_no = "NA";
                       entityData.shares_in_percent = "NA";
                       entityData.nic_flat_details = "NA";

                       // Assign Values
                       entityData.userMaster = mutationCTSNoDTLModel.userMaster;
                       entityData.applicationDTL = mutationCTSNoDTLModel.applicationDTL;
                       entityData.what_is_mentioned_in_the_doc = mutationCTSNoDTLModel.what_is_mentioned_in_the_doc;
                       entityData.village_or_peth_code = mutationCTSNoDTLModel.village_or_peth_code;
                       entityData.village_or_peth_name = mutationCTSNoDTLModel.village_or_peth_name;
                       entityData.mutation_modification_type = mutationCTSNoDTLModel.mutation_modification_type;
                       entityData.city_servey_no_mentioned_in_application = string.IsNullOrEmpty(mutationCTSNoDTLModel.city_servey_no_mentioned_in_application) ? "NA" : mutationCTSNoDTLModel.city_servey_no_mentioned_in_application;
                       entityData.servey_no = string.IsNullOrEmpty(mutationCTSNoDTLModel.servey_no) ? "NA" : mutationCTSNoDTLModel.servey_no;
                       entityData.selected_city_servey_no = mutationCTSNoDTLModel.selected_city_servey_no;
                       entityData.lr_property_uid = mutationCTSNoDTLModel.lr_property_uid;
                       entityData.application_income_type = mutationCTSNoDTLModel.application_income_type;
                       entityData.city_servey_area_in_sq_m = mutationCTSNoDTLModel.city_servey_area_in_sq_m;

                       // Flat Details Entered By User
                       if (mutationCTSNoDTLModel.application_income_type.Trim().ToUpper() == "OTHER")
                       {
                           entityData.building_name = string.IsNullOrEmpty(mutationCTSNoDTLModel.building_name!.Trim()) ? "NA" : mutationCTSNoDTLModel.building_name.Trim();
                           entityData.floor_type = string.IsNullOrEmpty(mutationCTSNoDTLModel.floor_type!.Trim()) ? "NA" : mutationCTSNoDTLModel.floor_type;
                           entityData.floor_no = string.IsNullOrEmpty(mutationCTSNoDTLModel.floor_no) ? "NA" : mutationCTSNoDTLModel.floor_no;
                           entityData.unit_type = string.IsNullOrEmpty(mutationCTSNoDTLModel.unit_type) ? "NA" : mutationCTSNoDTLModel.unit_type;
                           entityData.unit_no = string.IsNullOrEmpty(mutationCTSNoDTLModel.unit_no) ? "NA" : mutationCTSNoDTLModel.unit_no;
                           entityData.buildup_area_in_sq_m = string.IsNullOrEmpty(mutationCTSNoDTLModel.buildup_area_in_sq_m) ? "NA" : mutationCTSNoDTLModel.buildup_area_in_sq_m;
                           entityData.carpet_area_in_sq_m = string.IsNullOrEmpty(mutationCTSNoDTLModel.carpet_area_in_sq_m) ? "NA" : mutationCTSNoDTLModel.carpet_area_in_sq_m;
                           entityData.terrace_area_in_sq_m = string.IsNullOrEmpty(mutationCTSNoDTLModel.terrace_area_in_sq_m) ? "NA" : mutationCTSNoDTLModel.terrace_area_in_sq_m;
                           entityData.parking_area_in_sq_m = string.IsNullOrEmpty(mutationCTSNoDTLModel.parking_area_in_sq_m) ? "NA" : mutationCTSNoDTLModel.parking_area_in_sq_m;
                           entityData.parking_no = string.IsNullOrEmpty(mutationCTSNoDTLModel.parking_no) ? "NA" : mutationCTSNoDTLModel.parking_no;
                           entityData.shares_in_percent = string.IsNullOrEmpty(mutationCTSNoDTLModel.shares_in_percent) ? "NA" : mutationCTSNoDTLModel.shares_in_percent;
                       }
                       // End
                       else
                       {
                           entityData.nic_flat_details = string.IsNullOrEmpty(mutationCTSNoDTLModel.nic_flat_details) ? "NA" : mutationCTSNoDTLModel.nic_flat_details;
                       }
                       _context.mutationCTSNoDTLs.Attach(entityData);
                       _context.SaveChanges();
                       //Get Saved Row ID
                       //dbContextTransaction.Commit();
                       return "Success";
                   }
                   else
                   {
                       return "Data Not Found";
                   }
               }
               catch (Exception ex)
               {
                   //dbContextTransaction.Rollback();
                   throw new HandleException(ex.Message.ToString());
               }
               //}
           }
   */
        public string EditMutationCTSNoData(EditMutationCTSNoData mutationCTSNoData)
        {
            using (var dbContextTransaction = _context.Database.BeginTransaction())
            {
                try
                {
                    MutationCTSNoDTL entityData = new MutationCTSNoDTL();
                    entityData = _context.mutationCTSNoDTLs.Include(app => app.applicationDTL).Where(data => data.mutation_cts_no_id.Equals(mutationCTSNoData.mutation_cts_no_id)).FirstOrDefault()!;
                    if (entityData != null)
                    {
                        MutationCTSNoDTL fetchMutationCTSNoDTL = new MutationCTSNoDTL();
                        fetchMutationCTSNoDTL = _context.mutationCTSNoDTLs.Include(app => app.applicationDTL).Where(data => data.applicationDTL!.applicationid!.Equals(mutationCTSNoData.applicationid)
                        && data.village_or_peth_code != mutationCTSNoData.village!.village_code
                        ).FirstOrDefault()!;
                        if (fetchMutationCTSNoDTL != null)
                        {
                            return "Could Not Add Another Village For The Same Application";
                        }
                        // Assign Values to Model
                        MutationCTSNoDTLModel mutationCTSNoDTLModel = new MutationCTSNoDTLModel();
                        UserMaster userMaster = _context.userMasters.FirstOrDefault(s => s.userid == mutationCTSNoData.userid)!;
                        mutationCTSNoDTLModel.userMaster = userMaster;

                        ApplicationDTL applicationDTL = _context.applicationDTL.FirstOrDefault(s => s.applicationid == mutationCTSNoData.applicationid)!;
                        mutationCTSNoDTLModel.applicationDTL = applicationDTL;

                        if (mutationCTSNoData.inDast == "surveynoorgatno")
                        {
                            mutationCTSNoDTLModel.what_is_mentioned_in_the_doc = "SERVEYNO / GROUPNO";
                            mutationCTSNoDTLModel.servey_no = mutationCTSNoData.surveyNo;
                        }
                        if (mutationCTSNoData.inDast == "nabhu")
                        {
                            mutationCTSNoDTLModel.what_is_mentioned_in_the_doc = "CITY SERVEY NO";
                            mutationCTSNoDTLModel.city_servey_no_mentioned_in_application = mutationCTSNoData.nabhuNo;
                        }
                        mutationCTSNoDTLModel.village_or_peth_name = textInfo.ToTitleCase(mutationCTSNoData.village!.village_name!.Trim());
                        mutationCTSNoDTLModel.village_or_peth_code = mutationCTSNoData.village.village_code!.Trim();
                        mutationCTSNoDTLModel.village_lgd_code = String.IsNullOrEmpty(mutationCTSNoData.village.village_lgd_code!.Trim()) ? "NA" : mutationCTSNoData.village.village_lgd_code;
                        mutationCTSNoDTLModel.village_english_name = String.IsNullOrEmpty(mutationCTSNoData.village.village_english_name!.Trim()) ? "NA" : mutationCTSNoData.village.village_english_name;
                        mutationCTSNoDTLModel.zone_code = String.IsNullOrEmpty(mutationCTSNoData.village.zone_code!.Trim()) ? "NA" : mutationCTSNoData.village.zone_code;
                        mutationCTSNoDTLModel.amount = String.IsNullOrEmpty(mutationCTSNoData.village.amount!.Trim()) ? "NA" : mutationCTSNoData.village.amount;

                        mutationCTSNoDTLModel.mutation_modification_type = mutationCTSNoData.milkat!.Trim().ToUpper();
                        mutationCTSNoDTLModel.selected_city_servey_no = mutationCTSNoData.naBhu;
                        mutationCTSNoDTLModel.lr_property_uid = mutationCTSNoData.lrPropertyUID;
                        mutationCTSNoDTLModel.application_income_type = mutationCTSNoData.namud!.Trim();
                        mutationCTSNoDTLModel.city_servey_area_in_sq_m = mutationCTSNoData.cityServeyAreaInSqm;
                        mutationCTSNoDTLModel.sub_property_no = mutationCTSNoData.sub_property_no;
                        if (mutationCTSNoData.flatDetails != null && mutationCTSNoDTLModel.application_income_type.Trim().ToUpper() == "OTHER")
                        {
                            mutationCTSNoDTLModel.building_name = string.IsNullOrEmpty(mutationCTSNoData.flatDetails.buildingName) ? "NA" : mutationCTSNoData.flatDetails.buildingName.Trim();
                            mutationCTSNoDTLModel.floor_type = mutationCTSNoData.flatDetails.floorType!.floor_type;
                            mutationCTSNoDTLModel.floor_desc = string.IsNullOrEmpty(mutationCTSNoData.flatDetails.floorType!.floor_desc) ? "NA" : mutationCTSNoData.flatDetails.floorType.floor_desc.Trim();
                            mutationCTSNoDTLModel.floor_order_by = mutationCTSNoData.flatDetails.floorType.floor_order_by;
                            mutationCTSNoDTLModel.floor_no = string.IsNullOrEmpty(mutationCTSNoData.flatDetails.floorNo) ? "NA" : mutationCTSNoData.flatDetails.floorNo.Trim();
                            mutationCTSNoDTLModel.unit_code_156 = mutationCTSNoData.flatDetails.unitType!.unit_code_156;
                            mutationCTSNoDTLModel.unit_name_156 = string.IsNullOrEmpty(mutationCTSNoData.flatDetails.unitType!.unit_name_156) ? "NA" : mutationCTSNoData.flatDetails.unitType.unit_name_156.Trim();
                            mutationCTSNoDTLModel.unit_no = string.IsNullOrEmpty(mutationCTSNoData.flatDetails.unitNo) ? "NA" : mutationCTSNoData.flatDetails.unitNo.Trim();
                            mutationCTSNoDTLModel.carpet_area_in_sq_m = string.IsNullOrEmpty(mutationCTSNoData.flatDetails.carpetArea) ? "NA" : mutationCTSNoData.flatDetails.carpetArea.Trim();
                            mutationCTSNoDTLModel.terrace_area_in_sq_m = string.IsNullOrEmpty(mutationCTSNoData.flatDetails.taraceArea) ? "NA" : mutationCTSNoData.flatDetails.taraceArea.Trim();
                            mutationCTSNoDTLModel.parking_no = string.IsNullOrEmpty(mutationCTSNoData.flatDetails.parkingNo) ? "NA" : mutationCTSNoData.flatDetails.parkingNo.Trim();
                            mutationCTSNoDTLModel.parking_area_in_sq_m = string.IsNullOrEmpty(mutationCTSNoData.flatDetails.parkingArea) ? "NA" : mutationCTSNoData.flatDetails.parkingArea.Trim();
                            mutationCTSNoDTLModel.shares_in_percent = string.IsNullOrEmpty(mutationCTSNoData.flatDetails.hissa) ? "NA" : mutationCTSNoData.flatDetails.hissa.Trim();
                            mutationCTSNoDTLModel.buildup_area_in_sq_m = string.IsNullOrEmpty(mutationCTSNoData.flatDetails.buildupArea) ? "NA" : mutationCTSNoData.flatDetails.buildupArea.Trim();
                        }
                        if (mutationCTSNoDTLModel.application_income_type.Trim().ToUpper() != "OTHER")
                        {
                            mutationCTSNoDTLModel.nic_flat_details = mutationCTSNoData.nic_flat_details;
                        }
                        //Assign Data to Table fields to insert new records
                        //Set Default Values
                        /*
                                            entityData.what_is_mentioned_in_the_doc = "NA";
                                            entityData.village_or_peth_code = "NA";
                                            entityData.village_or_peth_name = "NA";
                                            entityData.mutation_modification_type = "NA";
                                            entityData.city_servey_no_mentioned_in_application = "NA";
                                            entityData.servey_no = "NA";
                                            entityData.selected_city_servey_no = "NA";
                                            entityData.lr_property_uid = "NA";
                                            entityData.application_income_type = "NA";
                                            entityData.city_servey_area_in_sq_m = "NA";
                                            entityData.building_name = "NA";
                                            entityData.floor_no = "NA";
                                            entityData.unit_type = "NA";
                                            entityData.unit_no = "NA";
                                            entityData.buildup_area_in_sq_m = "NA";
                                            entityData.carpet_area_in_sq_m = "NA";
                                            entityData.terrace_area_in_sq_m = "NA";
                                            entityData.parking_area_in_sq_m = "NA";
                                            entityData.parking_no = "NA";
                                            entityData.shares_in_percent = "NA";
                                            entityData.nic_flat_details = "NA";*/

                        // Assign Values
                        entityData.userMaster = mutationCTSNoDTLModel.userMaster;
                        entityData.applicationDTL = mutationCTSNoDTLModel.applicationDTL;
                        entityData.what_is_mentioned_in_the_doc = mutationCTSNoDTLModel.what_is_mentioned_in_the_doc;
                        entityData.village_or_peth_code = mutationCTSNoDTLModel.village_or_peth_code;
                        entityData.village_or_peth_name = mutationCTSNoDTLModel.village_or_peth_name;
                        entityData.village_lgd_code = mutationCTSNoDTLModel.village_lgd_code;
                        entityData.village_english_name = mutationCTSNoDTLModel.village_english_name;
                        entityData.zone_code = mutationCTSNoDTLModel.zone_code;
                        entityData.amount = mutationCTSNoDTLModel.amount;
                        entityData.mutation_modification_type = mutationCTSNoDTLModel.mutation_modification_type;
                        entityData.city_servey_no_mentioned_in_application = string.IsNullOrEmpty(mutationCTSNoDTLModel.city_servey_no_mentioned_in_application) ? "NA" : mutationCTSNoDTLModel.city_servey_no_mentioned_in_application;
                        entityData.servey_no = string.IsNullOrEmpty(mutationCTSNoDTLModel.servey_no) ? "NA" : mutationCTSNoDTLModel.servey_no;
                        entityData.selected_city_servey_no = mutationCTSNoDTLModel.selected_city_servey_no;
                        entityData.lr_property_uid = mutationCTSNoDTLModel.lr_property_uid;
                        entityData.application_income_type = mutationCTSNoDTLModel.application_income_type;
                        entityData.city_servey_area_in_sq_m = mutationCTSNoDTLModel.city_servey_area_in_sq_m;
                        //  entityData.sub_property_id = mutationCTSNoDTLModel.sub_property_no;
                        entityData.sub_property_id = mutationCTSNoDTLModel.sub_property_no;

                        // Flat Details Entered By User
                        if (mutationCTSNoDTLModel.application_income_type.Trim().ToUpper() == "OTHER")
                        {
                            entityData.building_name = string.IsNullOrEmpty(mutationCTSNoDTLModel.building_name!.Trim()) ? "NA" : mutationCTSNoDTLModel.building_name.Trim();
                            entityData.floor_type = mutationCTSNoDTLModel.floor_type;
                            entityData.floor_desc = string.IsNullOrEmpty(mutationCTSNoDTLModel.floor_desc!.Trim()) ? "NA" : mutationCTSNoDTLModel.floor_desc;
                            entityData.floor_order_by = mutationCTSNoDTLModel.floor_order_by;
                            entityData.floor_no = string.IsNullOrEmpty(mutationCTSNoDTLModel.floor_no) ? "NA" : mutationCTSNoDTLModel.floor_no;
                            entityData.unit_code_156 = mutationCTSNoDTLModel.unit_code_156;
                            entityData.unit_name_156 = string.IsNullOrEmpty(mutationCTSNoDTLModel.unit_name_156) ? "NA" : mutationCTSNoDTLModel.unit_name_156;
                            entityData.unit_no = string.IsNullOrEmpty(mutationCTSNoDTLModel.unit_no) ? "NA" : mutationCTSNoDTLModel.unit_no;
                            entityData.buildup_area_in_sq_m = string.IsNullOrEmpty(mutationCTSNoDTLModel.buildup_area_in_sq_m) ? "NA" : mutationCTSNoDTLModel.buildup_area_in_sq_m;
                            entityData.carpet_area_in_sq_m = string.IsNullOrEmpty(mutationCTSNoDTLModel.carpet_area_in_sq_m) ? "NA" : mutationCTSNoDTLModel.carpet_area_in_sq_m;
                            entityData.terrace_area_in_sq_m = string.IsNullOrEmpty(mutationCTSNoDTLModel.terrace_area_in_sq_m) ? "NA" : mutationCTSNoDTLModel.terrace_area_in_sq_m;
                            entityData.parking_area_in_sq_m = string.IsNullOrEmpty(mutationCTSNoDTLModel.parking_area_in_sq_m) ? "NA" : mutationCTSNoDTLModel.parking_area_in_sq_m;
                            entityData.parking_no = string.IsNullOrEmpty(mutationCTSNoDTLModel.parking_no) ? "NA" : mutationCTSNoDTLModel.parking_no;
                            entityData.shares_in_percent = string.IsNullOrEmpty(mutationCTSNoDTLModel.shares_in_percent) ? "NA" : mutationCTSNoDTLModel.shares_in_percent;
                        }
                        // End
                        else
                        {
                            entityData.nic_flat_details = string.IsNullOrEmpty(mutationCTSNoDTLModel.nic_flat_details) ? "NA" : mutationCTSNoDTLModel.nic_flat_details;
                        }
                        _context.mutationCTSNoDTLs.Attach(entityData);
                        _context.SaveChanges();
                        //Get Saved Row ID
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

        public string EditDastInformation(EditDastInformationData dastInformationData)
        {
            using (var dbContextTransaction = _context.Database.BeginTransaction())
            {
                try
                {
                    DastInformation entityData = new DastInformation();
                    entityData = _context.dastInformation.Include(i => i.userMaster).Include(app => app.applicationDTL).Where(data => data.dast_id.Equals(dastInformationData.dastid) && data.isDeleted == false).FirstOrDefault()!;
                    if (entityData != null)
                    {
                        // Assign Values to Model
                        DastInformationModel dastInformationModel = new DastInformationModel();
                        UserMaster userMaster = _context.userMasters.FirstOrDefault(s => s.userid == dastInformationData.userid)!;
                        dastInformationModel.userMaster = userMaster;

                        ApplicationDTL applicationDTL = _context.applicationDTL.FirstOrDefault(s => s.applicationid == dastInformationData.applicationid)!;
                        dastInformationModel.applicationDTL = applicationDTL;

                        dastInformationModel.dastType = dastInformationData.dastType!.Trim();
                        dastInformationModel.digcode = dastInformationData.division!.digcode;
                        dastInformationModel.dig_name = dastInformationData.division!.dig;
                        dastInformationModel.districtCode = dastInformationData.district!.jdrcode.ToString();
                        dastInformationModel.districtName = dastInformationData.district.jdr!.Trim();
                        //dastInformationModel.districtNameInEnglish = dastInformationData.district.district_english_name!.Trim();
                        dastInformationModel.office_of_the_second_registrar_code = dastInformationData.registrar!.srocode.ToString();
                        dastInformationModel.office_of_the_second_registrar_name = dastInformationData.registrar.sro!.Trim();
                        dastInformationModel.registered_dast_no = dastInformationData.dastNo;
                        dastInformationModel.registered_dast_date = dastInformationData.dastNoDate;
                        dastInformationModel.registered_dast_year = dastInformationData.dastNoYear;
                        dastInformationModel.dastNabhu = dastInformationData.dastNabhu;
                        dastInformationModel.remarks = dastInformationData.remarks!.Trim();
                        dastInformationModel.isDastVerified = dastInformationData.isDastVarified;
                        dastInformationModel.verifiedDastData = dastInformationData.verifiedDastData;


                        //Assign Data to Table fields to insert new records
                        entityData.userMaster = dastInformationModel.userMaster;
                        entityData.applicationDTL = dastInformationModel.applicationDTL;
                        entityData.dastType = dastInformationModel.dastType;
                        entityData.division_code = dastInformationModel.digcode;
                        entityData.division_name = dastInformationModel.dig_name;
                        entityData.districtCode = dastInformationModel.districtCode;
                        entityData.districtName = dastInformationModel.districtName;
                        //entityData.districtNameInEnglish = dastInformationModel.districtNameInEnglish;
                        entityData.office_of_the_second_registrar_code = dastInformationModel.office_of_the_second_registrar_code;
                        entityData.office_of_the_second_registrar_name = dastInformationModel.office_of_the_second_registrar_name;
                        entityData.registered_dast_no = dastInformationModel.registered_dast_no;
                        entityData.registered_dast_date = dastInformationModel.registered_dast_date;
                        entityData.registered_dast_year = dastInformationModel.registered_dast_year;
                        entityData.dastNabhu = dastInformationModel.dastNabhu;
                        entityData.remarks = dastInformationModel.remarks;
                        entityData.isDastVerified = dastInformationModel.isDastVerified;
                        entityData.verifiedDastData = dastInformationModel.verifiedDastData;
                        _context.dastInformation.Attach(entityData);
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

        public string EditPowerOfAttorneyGiver(EditPowerOfAttorneyInformationDataForGiver powerOfAttorneyInformationData)
        {
            MethodForFileUpload methodForFile = new MethodForFileUpload();
            PowerOfAttorneyInformation entityData = new PowerOfAttorneyInformation();
            string FolderPath = @"\\10.10.248.2\pde_propertycard\MUTATIONDOCS\" + powerOfAttorneyInformationData.applicationid + @"\POWEROFATTORNEY\GIVER\";
            //string FolderPath = @"\\10.10.248.2\pde_propertycard\MUTATIONDOCS\" + powerOfAttorneyInformationData.applicationid + @"\POWEROFATTORNEY\GIVER\";
            using (var dbContextTransaction = _context.Database.BeginTransaction())
            {
                try
                {
                    entityData = _context.powerOfAttorneyInformation.Include(i => i.userMaster).Include(app => app.applicationDTL).Where(data => data.power_of_attorney_id.Equals(powerOfAttorneyInformationData.power_of_attorney_id) && data.is_taker == false).FirstOrDefault()!;
                    if (entityData != null)
                    {
                        // Assign Values to Model
                        PowerOfAttorneyInformationModel powerOfAttorneyInformationModel = new PowerOfAttorneyInformationModel();
                        UserMaster userMaster = _context.userMasters.FirstOrDefault(s => s.userid == powerOfAttorneyInformationData.userid)!;
                        powerOfAttorneyInformationModel.userMaster = userMaster;

                        ApplicationDTL applicationDTL = _context.applicationDTL.FirstOrDefault(s => s.applicationid == powerOfAttorneyInformationData.applicationid)!;
                        powerOfAttorneyInformationModel.applicationDTL = applicationDTL;

                        powerOfAttorneyInformationModel.address_type = powerOfAttorneyInformationData.address!.addressType!.Trim().ToUpper();
                        if (powerOfAttorneyInformationModel.address_type == "INDIA")
                        {
                            powerOfAttorneyInformationModel.mobileno = "NA";
                            powerOfAttorneyInformationModel.mobilenoverified = "NA";
                            powerOfAttorneyInformationModel.emailid = "NA";
                            powerOfAttorneyInformationModel.emailidverified = "NA";

                            powerOfAttorneyInformationModel.address = "NA";
                            powerOfAttorneyInformationModel.state = powerOfAttorneyInformationData.address.indiaAddress!.state;
                            powerOfAttorneyInformationModel.district = powerOfAttorneyInformationData.address.indiaAddress.district;
                            powerOfAttorneyInformationModel.taluka = powerOfAttorneyInformationData.address.indiaAddress.taluka;
                            powerOfAttorneyInformationModel.city = powerOfAttorneyInformationData.address.indiaAddress.city;
                            powerOfAttorneyInformationModel.flatno_plotno = powerOfAttorneyInformationData.address.indiaAddress.plotNo;
                            powerOfAttorneyInformationModel.societyname = powerOfAttorneyInformationData.address.indiaAddress.building;
                            powerOfAttorneyInformationModel.mainstreet = powerOfAttorneyInformationData.address.indiaAddress.mainRoad;
                            powerOfAttorneyInformationModel.landmark = powerOfAttorneyInformationData.address.indiaAddress.impSymbol;
                            powerOfAttorneyInformationModel.locality = powerOfAttorneyInformationData.address.indiaAddress.area;
                            powerOfAttorneyInformationModel.pincode = powerOfAttorneyInformationData.address.indiaAddress.pincode;
                            powerOfAttorneyInformationModel.postofficename = powerOfAttorneyInformationData.address.indiaAddress.postOfficeName;
                        }
                        else if (powerOfAttorneyInformationModel.address_type == "FOREIGN")
                        {
                            powerOfAttorneyInformationModel.address = powerOfAttorneyInformationData.address.foreignAddress!.address;
                            powerOfAttorneyInformationModel.mobileno = powerOfAttorneyInformationData.address.foreignAddress.mobile;
                            powerOfAttorneyInformationModel.emailid = powerOfAttorneyInformationData.address.foreignAddress.email;
                            powerOfAttorneyInformationModel.emailidverified = powerOfAttorneyInformationData.address.foreignAddress.emailOTP;

                            powerOfAttorneyInformationModel.state = "NA";
                            powerOfAttorneyInformationModel.district = "NA";
                            powerOfAttorneyInformationModel.taluka = "NA";
                            powerOfAttorneyInformationModel.city = "NA";
                            powerOfAttorneyInformationModel.flatno_plotno = "NA";
                            powerOfAttorneyInformationModel.societyname = "NA";
                            powerOfAttorneyInformationModel.mainstreet = "NA";
                            powerOfAttorneyInformationModel.landmark = "NA";
                            powerOfAttorneyInformationModel.locality = "NA";
                            powerOfAttorneyInformationModel.pincode = "NA";
                            powerOfAttorneyInformationModel.postofficename = "NA";
                        }
                        powerOfAttorneyInformationModel.prefixcode_eng = powerOfAttorneyInformationData.userDetails!.suffixCodeEng;
                        powerOfAttorneyInformationModel.prefixcode_marathi = powerOfAttorneyInformationData.userDetails.suffixcode;
                        powerOfAttorneyInformationModel.prefix_in_eng = powerOfAttorneyInformationData.userDetails!.suffixEng;
                        powerOfAttorneyInformationModel.fname_in_eng = powerOfAttorneyInformationData.userDetails.firstNameEng;
                        powerOfAttorneyInformationModel.mname_in_eng = powerOfAttorneyInformationData.userDetails.middleNameEng;
                        powerOfAttorneyInformationModel.lname_in_eng = powerOfAttorneyInformationData.userDetails.lastNameEng;
                        powerOfAttorneyInformationModel.prefix_in_marathi = powerOfAttorneyInformationData.userDetails.suffix;
                        powerOfAttorneyInformationModel.fname_in_marathi = powerOfAttorneyInformationData.userDetails.firstName;
                        powerOfAttorneyInformationModel.mname_in_marathi = powerOfAttorneyInformationData.userDetails.middleName;
                        powerOfAttorneyInformationModel.lname_in_marathi = powerOfAttorneyInformationData.userDetails.lastName;
                        powerOfAttorneyInformationModel.username = powerOfAttorneyInformationData.userDetails.userName;
                        powerOfAttorneyInformationModel.city_servey_no = powerOfAttorneyInformationData.userDetails.nabhu;
                        powerOfAttorneyInformationModel.lr_property_id = powerOfAttorneyInformationData.userDetails.lrPropertyUID;
                        powerOfAttorneyInformationModel.sub_property_id = powerOfAttorneyInformationData.userDetails.subPropNo;


                        //Assign Data to Table fields to insert new records
                        entityData.userMaster = powerOfAttorneyInformationModel.userMaster;
                        entityData.applicationDTL = powerOfAttorneyInformationModel.applicationDTL;
                        entityData.mobileno = powerOfAttorneyInformationModel.mobileno!;
                        entityData.mobilenoverified = string.IsNullOrEmpty(powerOfAttorneyInformationModel.mobilenoverified) ? "NO" : powerOfAttorneyInformationModel.mobilenoverified;
                        entityData.emailid = powerOfAttorneyInformationModel.emailid!;
                        entityData.emailidverified = powerOfAttorneyInformationModel.emailidverified!;
                        entityData.prefixcode_eng = powerOfAttorneyInformationModel.prefixcode_eng!;
                        entityData.prefix_in_eng = string.IsNullOrEmpty(powerOfAttorneyInformationModel.prefix_in_eng) ? "NA" : powerOfAttorneyInformationModel.prefix_in_eng;
                        entityData.fname_in_eng = string.IsNullOrEmpty(powerOfAttorneyInformationModel.fname_in_eng) ? "NA" : textInfo.ToTitleCase(powerOfAttorneyInformationModel.fname_in_eng.Trim());
                        entityData.mname_in_eng = string.IsNullOrEmpty(powerOfAttorneyInformationModel.mname_in_eng) ? "NA" : textInfo.ToTitleCase(powerOfAttorneyInformationModel.mname_in_eng.Trim());
                        entityData.lname_in_eng = string.IsNullOrEmpty(powerOfAttorneyInformationModel.lname_in_eng) ? "NA" : textInfo.ToTitleCase(powerOfAttorneyInformationModel.lname_in_eng.Trim());
                        entityData.prefixcode_marathi = powerOfAttorneyInformationModel.prefixcode_marathi!;
                        entityData.prefix_in_marathi = string.IsNullOrEmpty(powerOfAttorneyInformationModel.prefix_in_marathi) ? "NA" : powerOfAttorneyInformationModel.prefix_in_marathi.Trim();
                        entityData.fname_in_marathi = string.IsNullOrEmpty(powerOfAttorneyInformationModel.fname_in_marathi) ? "NA" : powerOfAttorneyInformationModel.fname_in_marathi.Trim();
                        entityData.mname_in_marathi = string.IsNullOrEmpty(powerOfAttorneyInformationModel.mname_in_marathi) ? "NA" : powerOfAttorneyInformationModel.mname_in_marathi.Trim();
                        entityData.lname_in_marathi = string.IsNullOrEmpty(powerOfAttorneyInformationModel.lname_in_marathi) ? "NA" : powerOfAttorneyInformationModel.lname_in_marathi.Trim();
                        entityData.username = powerOfAttorneyInformationModel.username!;
                        entityData.city_servey_no = powerOfAttorneyInformationModel.city_servey_no!;
                        entityData.lr_property_id = powerOfAttorneyInformationModel.lr_property_id!;
                        entityData.sub_property_no = powerOfAttorneyInformationModel.sub_property_id!;
                        entityData.address_type = powerOfAttorneyInformationModel.address_type;
                        entityData.address = powerOfAttorneyInformationModel.address!;
                        entityData.state = powerOfAttorneyInformationModel.state!;
                        entityData.district = powerOfAttorneyInformationModel.district!;
                        entityData.taluka = powerOfAttorneyInformationModel.taluka!;
                        entityData.city = powerOfAttorneyInformationModel.city!;
                        entityData.flatno_plotno = powerOfAttorneyInformationModel.flatno_plotno!;
                        entityData.societyname = powerOfAttorneyInformationModel.societyname!;
                        entityData.mainstreet = powerOfAttorneyInformationModel.mainstreet!;
                        entityData.landmark = powerOfAttorneyInformationModel.landmark!;
                        entityData.locality = powerOfAttorneyInformationModel.locality!;
                        entityData.pincode = powerOfAttorneyInformationModel.pincode!;
                        entityData.postofficename = powerOfAttorneyInformationModel.postofficename!;
                        entityData.attornytype_code = 0;
                        entityData.attornytype_desc = "NA";
                        entityData.is_taker = false;
                        entityData.alias_name = "NA";
                        entityData.gender_code = "NA";
                        entityData.gender_description = "NA";
                        /*entityData.khata_type_code = "NA";
                        entityData.khata_type_name = "NA";*/

                        entityData.owner_status_code = "NA";
                        entityData.owner_status_description = "NA";

                        entityData.dob = "NA";
                        entityData.mother_name_in_marathi = "NA";
                        entityData.mother_name_in_eng = "NA";

                        /* entityData.apk_code = 0;
                         entityData.apk_description = "NA";

                         entityData.aapak = "NA";*/
                        entityData.landBuyArea = "NA";
                        entityData.profile_pic_file_name = "NA";
                        entityData.profile_pic_file_path = "NA";
                        _context.powerOfAttorneyInformation.Attach(entityData);
                        _context.SaveChanges();

                        //Get Saved Row ID
                        int powerOfAttorneyID = entityData.power_of_attorney_id;

                        string CurrentDateTime = DateTime.Now.ToString("yyyy-MMM-dd-HHmmss");
                        bool checkAddressFlag = true;
                        //bool checkSignFlag = false;
                        if (powerOfAttorneyInformationModel.address_type == "INDIA")
                        {
                            if (!string.IsNullOrEmpty(powerOfAttorneyInformationData.address.indiaAddress!.addressProofSrc) && !string.IsNullOrEmpty(powerOfAttorneyInformationData.address.indiaAddress.addressProofName))
                            {
                                string[] AddressData = powerOfAttorneyInformationData.address.indiaAddress.addressProofSrc.Split(",");
                                checkAddressFlag = methodForFile.SaveImageForApplicant(AddressData[1], powerOfAttorneyInformationData.address.indiaAddress.addressProofName, powerOfAttorneyID.ToString(), "AddressProof", FolderPath + @"\", CurrentDateTime);
                                string AddressProofExt = Path.GetExtension(powerOfAttorneyInformationData.address.indiaAddress.addressProofName);
                                powerOfAttorneyInformationModel.address_proof_document_name = "AddressProof" + powerOfAttorneyID + "_" + CurrentDateTime + AddressProofExt;
                                powerOfAttorneyInformationModel.address_proof_document_path = FolderPath + powerOfAttorneyID + @"\" + powerOfAttorneyInformationModel.address_proof_document_name;

                                var UpdateAddressFilePath = _context.powerOfAttorneyInformation.Where(w => w.power_of_attorney_id == powerOfAttorneyID).FirstOrDefault();
                                if (UpdateAddressFilePath != null)
                                {
                                    entityData.address_proof_document_name = powerOfAttorneyInformationModel.address_proof_document_name;
                                    entityData.address_proof_document_path = powerOfAttorneyInformationModel.address_proof_document_path;
                                    _context.Entry(entityData).CurrentValues.SetValues(entityData);
                                    _context.SaveChanges();
                                }
                            }

                            //string[] signData = powerOfAttorneyInformationData.address.indiaAddress.signatureSrc.Split(",");
                            //checkSignFlag = methodForFile.SaveImageForApplicant(signData[1], powerOfAttorneyInformationData.address.indiaAddress.signatureName, powerOfAttorneyID.ToString(), "Signature", FolderPath + @"\", CurrentDateTime);
                            //string SignatureExt = Path.GetExtension(powerOfAttorneyInformationData.address.indiaAddress.signatureName);
                            //powerOfAttorneyInformationModel.signed_file_name = "Signature" + powerOfAttorneyID + "_" + CurrentDateTime + SignatureExt;
                            //powerOfAttorneyInformationModel.signed_file_path = FolderPath + powerOfAttorneyID + @"\" + powerOfAttorneyInformationModel.signed_file_name;

                            //var UpdateSignFilePath = _context.powerOfAttorneyInformation.Where(w => w.power_of_attorney_id == powerOfAttorneyID).FirstOrDefault();
                            //if (UpdateSignFilePath != null)
                            //{
                            //    entityData.signed_file_name = powerOfAttorneyInformationModel.signed_file_name;
                            //    entityData.signed_file_path = powerOfAttorneyInformationModel.signed_file_path;
                            //    _context.Entry(entityData).CurrentValues.SetValues(entityData);
                            //    _context.SaveChanges();
                            //}
                        }
                        //if (powerOfAttorneyInformationModel.address_type == "FOREIGN")
                        //{
                        //    string[] signData = powerOfAttorneyInformationData.address.foreignAddress.signatureSrc.Split(",");
                        //    checkSignFlag = methodForFile.SaveImageForApplicant(signData[1], powerOfAttorneyInformationData.address.foreignAddress.signatureName, powerOfAttorneyID.ToString(), "Signature", FolderPath + @"\", CurrentDateTime);

                        //    string SignatureExt = Path.GetExtension(powerOfAttorneyInformationData.address.foreignAddress.signatureName);
                        //    powerOfAttorneyInformationModel.signed_file_name = "Signature" + powerOfAttorneyID + "_" + CurrentDateTime + SignatureExt;
                        //    powerOfAttorneyInformationModel.signed_file_path = FolderPath  + powerOfAttorneyID + @"\" + powerOfAttorneyInformationModel.signed_file_name;
                        //    var UpdateSignFilePath = _context.powerOfAttorneyInformation.Where(w => w.power_of_attorney_id == powerOfAttorneyID).FirstOrDefault();
                        //    if (UpdateSignFilePath != null)
                        //    {
                        //        entityData.signed_file_name = powerOfAttorneyInformationModel.signed_file_name;
                        //        entityData.signed_file_path = powerOfAttorneyInformationModel.signed_file_path;
                        //        _context.Entry(entityData).CurrentValues.SetValues(entityData);
                        //        _context.SaveChanges();
                        //    }
                        //}
                        if (checkAddressFlag)
                        {
                            _context.SaveChanges();
                            dbContextTransaction.Commit();
                            dbContextTransaction.Dispose();
                            return "Success";
                        }
                        else
                        {
                            dbContextTransaction.Rollback();
                            dbContextTransaction.Dispose();
                            methodForFile.DeleteFile(powerOfAttorneyInformationModel.signed_file_path!);
                            return "Address Proof File Is Not Uploaded";
                        }
                        //if (!checkAddressFlag)
                        //{
                        //    dbContextTransaction.Rollback();
                        //    methodForFile.DeleteFile(powerOfAttorneyInformationModel.signed_file_path);
                        //    return "Address Proof File Is Not Uploaded";
                        //}
                        //if (!checkSignFlag)
                        //{
                        //    dbContextTransaction.Rollback();
                        //    methodForFile.DeleteFile(powerOfAttorneyInformationModel.address_proof_document_path);
                        //    return "Signature File Is Not Uploaded";
                        //}
                        //else
                        //{
                        //    dbContextTransaction.Rollback();
                        //    return "Some Files Are Not Uploaded";
                        //}
                    }
                    else
                    {
                        return "Data Not Found";
                    }
                }
                catch (Exception ex)
                {
                    dbContextTransaction.Rollback();
                    throw new HandleException(ex.Message.ToString());
                }
            }
        }

        public string EditPowerOfAttorneyTaker(EditPowerOfAttorneyInformationDataForTaker takerData)
        {
            using (var dbContextTransaction = _context.Database.BeginTransaction())
            {
                MethodForFileUpload methodForFile = new MethodForFileUpload();
                PowerOfAttorneyInformation entityData = new PowerOfAttorneyInformation();
                string FolderPath = @"\\10.10.248.2\pde_propertycard\MUTATIONDOCS\" + takerData.applicationid + @"\POWEROFATTORNEY\TAKER\";
                try
                {
                    entityData = _context.powerOfAttorneyInformation.Include(i => i.userMaster).Include(app => app.applicationDTL).Include(proptype => proptype.propertyType).Where(data => data.power_of_attorney_id.Equals(takerData.power_of_attorney_id) && data.is_taker == true).FirstOrDefault()!;

                    if (entityData != null)
                    {
                        // Assign values to model
                        PowerOfAttorneyInformationModel powerOfAttorneyInformationModel = new PowerOfAttorneyInformationModel();
                        powerOfAttorneyInformationModel.usertype_code = takerData.usertype_code;
                        powerOfAttorneyInformationModel.usertype = takerData.usertype!.Trim().ToUpper();
                        powerOfAttorneyInformationModel.profile_pic_file_name = takerData.photo!.passportName;

                        powerOfAttorneyInformationModel.owner_of_property_in_maharashtra = (takerData.isMHProperty!.hasProperty!.Trim().ToUpper() == "YES") ? true : false;
                        PropertyTypeMaster proptype = _context.propertyTypes.FirstOrDefault(s => s.propertytypeid == Convert.ToInt32(takerData.isMHProperty.propType))!;
                        powerOfAttorneyInformationModel.PropertyTypeMaster = proptype;
                        if (powerOfAttorneyInformationModel.owner_of_property_in_maharashtra)
                        {
                            if (powerOfAttorneyInformationModel.PropertyTypeMaster.propertytypeid == 1 && string.IsNullOrEmpty(takerData.isMHProperty.userDetails!.khataNo))
                            {
                                return "When Property Type Is 7/12 Then Khate No Should Not Be Empty";
                            }
                            if (powerOfAttorneyInformationModel.PropertyTypeMaster.propertytypeid == 2 && string.IsNullOrEmpty(takerData.isMHProperty.userDetails!.naBhu))
                            {
                                return "When Property Type Is Property Card Then City Servey No Should Not Be Empty";
                            }
                            if (powerOfAttorneyInformationModel.PropertyTypeMaster.propertytypeid == 3 && string.IsNullOrEmpty(takerData.isMHProperty.userDetails!.ulpin))
                            {
                                return "When Property Type Is ULPIN Then ULPIN Should Not Be Empty";
                            }
                            powerOfAttorneyInformationModel.khateno = string.IsNullOrEmpty(takerData.isMHProperty.userDetails!.khataNo) ? "NA" : takerData.isMHProperty.userDetails.khataNo;
                            powerOfAttorneyInformationModel.city_servey_no = string.IsNullOrEmpty(takerData.isMHProperty.userDetails.naBhu) ? "NA" : takerData.isMHProperty.userDetails.naBhu;
                            powerOfAttorneyInformationModel.ulpin = string.IsNullOrEmpty(takerData.isMHProperty.userDetails.ulpin) ? "NA" : takerData.isMHProperty.userDetails.ulpin;
                            if (powerOfAttorneyInformationModel.PropertyTypeMaster.propertytypeid == 1)
                            {
                                powerOfAttorneyInformationModel.city_servey_no = "NA";
                                powerOfAttorneyInformationModel.ulpin = "NA";
                            }
                            else if (powerOfAttorneyInformationModel.PropertyTypeMaster.propertytypeid == 2)
                            {
                                powerOfAttorneyInformationModel.khateno = "NA";
                                powerOfAttorneyInformationModel.ulpin = "NA";
                            }
                            else if (powerOfAttorneyInformationModel.PropertyTypeMaster.propertytypeid == 3)
                            {
                                powerOfAttorneyInformationModel.khateno = "NA";
                                powerOfAttorneyInformationModel.city_servey_no = "NA";
                            }
                            powerOfAttorneyInformationModel.property_district_code = takerData.isMHProperty.userDetails.district!.district_code;
                            powerOfAttorneyInformationModel.property_district_name_in_marathi = takerData.isMHProperty.userDetails.district.district_name;
                            powerOfAttorneyInformationModel.property_district_name_in_english = takerData.isMHProperty.userDetails.district.district_english_name;

                            powerOfAttorneyInformationModel.property_taluka_code = takerData.isMHProperty.userDetails.taluka!.office_code;
                            powerOfAttorneyInformationModel.property_taluka_name = takerData.isMHProperty.userDetails.taluka.office_name;

                            powerOfAttorneyInformationModel.property_city_code = takerData.isMHProperty.userDetails.village!.village_code;
                            powerOfAttorneyInformationModel.property_city_name = takerData.isMHProperty.userDetails.village.village_name;
                        }
                        else
                        {
                            powerOfAttorneyInformationModel.khateno = "NA";
                            //Gouri
                            powerOfAttorneyInformationModel.city_servey_no = "NA";
                            powerOfAttorneyInformationModel.ulpin = "NA";
                            powerOfAttorneyInformationModel.property_district_code = "NA";
                            powerOfAttorneyInformationModel.property_district_name_in_marathi = "NA";
                            powerOfAttorneyInformationModel.property_district_name_in_english = "NA";
                            powerOfAttorneyInformationModel.property_taluka_code = "NA";
                            powerOfAttorneyInformationModel.property_taluka_name = "NA";
                            powerOfAttorneyInformationModel.property_city_code = "NA";
                            powerOfAttorneyInformationModel.property_city_name = "NA";
                        }
                        powerOfAttorneyInformationModel.username = takerData.isMHProperty.userDetails!.userName;
                        if (takerData.usertype_code == 1)
                        {
                            powerOfAttorneyInformationModel.prefix_in_marathi = takerData.isMHProperty.userDetails.suffix;
                            powerOfAttorneyInformationModel.fname_in_marathi = takerData.isMHProperty.userDetails.firstName;
                            powerOfAttorneyInformationModel.mname_in_marathi = takerData.isMHProperty.userDetails.middleName;
                            powerOfAttorneyInformationModel.lname_in_marathi = takerData.isMHProperty.userDetails.lastName;
                            powerOfAttorneyInformationModel.prefix_in_eng = takerData.isMHProperty.userDetails.suffixEng;
                            powerOfAttorneyInformationModel.fname_in_eng = takerData.isMHProperty.userDetails.firstNameEng;
                            powerOfAttorneyInformationModel.mname_in_eng = takerData.isMHProperty.userDetails.middleNameEng;
                            powerOfAttorneyInformationModel.lname_in_eng = takerData.isMHProperty.userDetails.lastNameEng;

                            powerOfAttorneyInformationModel.alias_name = takerData.dharak!.userdharak!.aliceName;
                            powerOfAttorneyInformationModel.gender_code = takerData.dharak.userdharak.gender!.gender_code;
                            powerOfAttorneyInformationModel.gender_description = takerData.dharak.userdharak.gender.gender_description;

                            /*powerOfAttorneyInformationModel.khata_type_code = takerData.dharak.userdharak.khataType!.khataCode;
                            powerOfAttorneyInformationModel.khata_type_name = takerData.dharak.userdharak.khataType.khataLabel;

                            powerOfAttorneyInformationModel.holder_type_code = takerData.dharak.userdharak.holderType!.owner_status_code;
                            powerOfAttorneyInformationModel.holder_type_name = takerData.dharak.userdharak.holderType.owner_status_description;*/

                            powerOfAttorneyInformationModel.dob = takerData.dharak.userdharak.dob;
                            powerOfAttorneyInformationModel.mother_name_in_marathi = takerData.dharak.userdharak.motherName!.Trim();
                            powerOfAttorneyInformationModel.mother_name_in_eng = takerData.dharak.userdharak.motherNameEng!.Trim();


                            powerOfAttorneyInformationModel.company_name_in_marathi = "NA";
                            powerOfAttorneyInformationModel.company_name_in_eng = "NA";
                            /*powerOfAttorneyInformationModel.apk_code = 0;
                            powerOfAttorneyInformationModel.apk_description = "NA";
                            powerOfAttorneyInformationModel.aapak = "NA";*/
                            powerOfAttorneyInformationModel.landBuyArea = "NA";
                        }
                        else
                        {
                            powerOfAttorneyInformationModel.company_name_in_marathi = takerData.isMHProperty.userDetails.companyName!.Trim();
                            powerOfAttorneyInformationModel.company_name_in_eng = takerData.isMHProperty.userDetails.companyNameEng!.Trim();
                            /*powerOfAttorneyInformationModel.holder_type_code = takerData.dharak!.companydharak!.holderType!.owner_status_code;
                            powerOfAttorneyInformationModel.holder_type_name = takerData.dharak.companydharak.holderType.owner_status_description;

                            powerOfAttorneyInformationModel.khata_type_code = takerData.dharak.companydharak.khataType!.khataCode;
                            powerOfAttorneyInformationModel.khata_type_name = takerData.dharak.companydharak.khataType.khataLabel;

                            powerOfAttorneyInformationModel.apk_code = takerData.dharak.companydharak.aapakDropdown!.apk_code;
                            powerOfAttorneyInformationModel.apk_description = takerData.dharak.companydharak.aapakDropdown.apk_description;

                            powerOfAttorneyInformationModel.aapak = takerData.dharak.companydharak.aapak!.Trim();*/
                            powerOfAttorneyInformationModel.landBuyArea = takerData.dharak!.companydharak!.landBuyArea!.Trim();


                            powerOfAttorneyInformationModel.prefix_in_marathi = "NA";
                            powerOfAttorneyInformationModel.fname_in_marathi = "NA";
                            powerOfAttorneyInformationModel.mname_in_marathi = "NA";
                            powerOfAttorneyInformationModel.lname_in_marathi = "NA";
                            powerOfAttorneyInformationModel.prefix_in_eng = "NA";
                            powerOfAttorneyInformationModel.fname_in_eng = "NA";
                            powerOfAttorneyInformationModel.mname_in_eng = "NA";
                            powerOfAttorneyInformationModel.lname_in_eng = "NA";

                            powerOfAttorneyInformationModel.alias_name = "NA";
                            powerOfAttorneyInformationModel.gender_code = "NA";
                            powerOfAttorneyInformationModel.gender_description = "NA";
                            powerOfAttorneyInformationModel.dob = "NA";
                            powerOfAttorneyInformationModel.mother_name_in_marathi = "NA";
                            powerOfAttorneyInformationModel.mother_name_in_eng = "NA";
                        }
                        powerOfAttorneyInformationModel.address_type = takerData.address!.addressType!.Trim().ToUpper();
                        if (takerData.address.addressType.Trim().ToUpper() == "INDIA")
                        {
                            powerOfAttorneyInformationModel.state = takerData.address.indiaAddress!.state;
                            powerOfAttorneyInformationModel.district = takerData.address.indiaAddress.district;
                            powerOfAttorneyInformationModel.city = takerData.address.indiaAddress.city;
                            powerOfAttorneyInformationModel.taluka = takerData.address.indiaAddress.taluka;
                            powerOfAttorneyInformationModel.flatno_plotno = takerData.address.indiaAddress.plotNo;
                            powerOfAttorneyInformationModel.societyname = takerData.address.indiaAddress.building;
                            powerOfAttorneyInformationModel.mainstreet = takerData.address.indiaAddress.mainRoad;
                            powerOfAttorneyInformationModel.landmark = takerData.address.indiaAddress.impSymbol;
                            powerOfAttorneyInformationModel.locality = takerData.address.indiaAddress.area;
                            powerOfAttorneyInformationModel.pincode = takerData.address.indiaAddress.pincode;
                            powerOfAttorneyInformationModel.postofficename = takerData.address.indiaAddress.postOfficeName;
                            powerOfAttorneyInformationModel.mobileno = takerData.address.indiaAddress.mobile;
                            powerOfAttorneyInformationModel.mobilenoverified = takerData.address.indiaAddress.mobileOTP!.Trim().ToUpper();
                            powerOfAttorneyInformationModel.emailid = "NA";
                            powerOfAttorneyInformationModel.emailidverified = "NA";
                            powerOfAttorneyInformationModel.address = "NA";
                        }
                        else if (takerData.address.addressType.Trim().ToUpper() == "FOREIGN")
                        {
                            powerOfAttorneyInformationModel.address = takerData.address.foreignAddress!.address;
                            powerOfAttorneyInformationModel.mobileno = takerData.address.foreignAddress.mobile;
                            powerOfAttorneyInformationModel.mobilenoverified = "NO";
                            powerOfAttorneyInformationModel.emailid = takerData.address.foreignAddress.email;
                            powerOfAttorneyInformationModel.emailidverified = takerData.address.foreignAddress.emailOTP!.Trim().ToUpper();

                            powerOfAttorneyInformationModel.state = "NA";
                            powerOfAttorneyInformationModel.district = "NA";
                            powerOfAttorneyInformationModel.city = "NA";
                            powerOfAttorneyInformationModel.taluka = "NA";
                            powerOfAttorneyInformationModel.flatno_plotno = "NA";
                            powerOfAttorneyInformationModel.societyname = "NA";
                            powerOfAttorneyInformationModel.mainstreet = "NA";
                            powerOfAttorneyInformationModel.landmark = "NA";
                            powerOfAttorneyInformationModel.locality = "NA";
                            powerOfAttorneyInformationModel.pincode = "NA";
                            powerOfAttorneyInformationModel.postofficename = "NA";
                        }

                        ApplicationDTL applicationDTL = _context.applicationDTL.FirstOrDefault(s => s.applicationid == takerData.applicationid)!;
                        powerOfAttorneyInformationModel.applicationDTL = applicationDTL;

                        UserMaster userMaster = _context.userMasters.FirstOrDefault(s => s.userid == takerData.userid)!;
                        powerOfAttorneyInformationModel.userMaster = userMaster;

                        powerOfAttorneyInformationModel.attornytype_code = takerData.attornytype!.poa_type_code;
                        powerOfAttorneyInformationModel.attornytype_desc = takerData.attornytype.poa_type_description;
                        powerOfAttorneyInformationModel.dastNo = takerData.dastNo!;
                        powerOfAttorneyInformationModel.dastNoYear = takerData.dastNoYear!;
                        powerOfAttorneyInformationModel.dastNoDate = takerData.dastNoDate!;
                        powerOfAttorneyInformationModel.isDastVerified = (bool)takerData.isDastVerified!;
                        powerOfAttorneyInformationModel.verifiedDastData = takerData.verifiedDastData!;
                        powerOfAttorneyInformationModel.isPOAisPartofDast = takerData.isPOAisPartofDast!;
                        powerOfAttorneyInformationModel.isDeclerationInvolvedInPOA = takerData.isDeclerationInvolvedInPOA!;
                        powerOfAttorneyInformationModel.isPOAPermanant = takerData.isPOAPermanant!;
                        powerOfAttorneyInformationModel.isTransferRights = takerData.isTransferRights!;

                        powerOfAttorneyInformationModel.digcode = takerData.division!.digcode;
                        powerOfAttorneyInformationModel.digname = takerData.division!.dig!;
                        powerOfAttorneyInformationModel.poa_district_code = takerData.district!.jdrcode!.ToString()!;
                        powerOfAttorneyInformationModel.poa_district_name = takerData.district.jdr!;
                        //powerOfAttorneyInformationModel.poa_district_name_in_eng = takerData.district.district_english_name!;
                        powerOfAttorneyInformationModel.sro_office_code = Convert.ToInt32(takerData.registrar!.srocode.ToString()!);
                        powerOfAttorneyInformationModel.sro_office_name = takerData.registrar.sro!;

                        //Assign Data to Table fields to insert new records
                        entityData.usertype_code = powerOfAttorneyInformationModel.usertype_code;
                        entityData.usertype = powerOfAttorneyInformationModel.usertype;
                        entityData.mobileno = powerOfAttorneyInformationModel.mobileno!;
                        entityData.mobilenoverified = powerOfAttorneyInformationModel.mobilenoverified!;
                        entityData.emailid = powerOfAttorneyInformationModel.emailid!;
                        entityData.emailidverified = powerOfAttorneyInformationModel.emailidverified!;
                        entityData.prefix_in_eng = powerOfAttorneyInformationModel.prefix_in_eng!.Trim();
                        entityData.fname_in_eng = textInfo.ToTitleCase(powerOfAttorneyInformationModel.fname_in_eng!.Trim());
                        entityData.mname_in_eng = textInfo.ToTitleCase(powerOfAttorneyInformationModel.mname_in_eng!.Trim());
                        entityData.lname_in_eng = textInfo.ToTitleCase(powerOfAttorneyInformationModel.lname_in_eng!.Trim());
                        entityData.prefix_in_marathi = powerOfAttorneyInformationModel.prefix_in_marathi!.Trim();
                        entityData.fname_in_marathi = powerOfAttorneyInformationModel.fname_in_marathi!.Trim();
                        entityData.mname_in_marathi = powerOfAttorneyInformationModel.mname_in_marathi!.Trim();
                        entityData.lname_in_marathi = powerOfAttorneyInformationModel.lname_in_marathi!.Trim();
                        entityData.address_type = powerOfAttorneyInformationModel.address_type;
                        entityData.address = string.IsNullOrEmpty(powerOfAttorneyInformationModel.address) ? "NA" : powerOfAttorneyInformationModel.address;
                        entityData.state = powerOfAttorneyInformationModel.state!;
                        entityData.district = powerOfAttorneyInformationModel.district!;
                        entityData.taluka = powerOfAttorneyInformationModel.taluka!;
                        entityData.city = powerOfAttorneyInformationModel.city!;
                        entityData.flatno_plotno = powerOfAttorneyInformationModel.flatno_plotno!;
                        entityData.societyname = powerOfAttorneyInformationModel.societyname!;
                        entityData.mainstreet = powerOfAttorneyInformationModel.mainstreet!;
                        entityData.landmark = powerOfAttorneyInformationModel.landmark!;
                        entityData.locality = powerOfAttorneyInformationModel.locality!;
                        entityData.pincode = powerOfAttorneyInformationModel.pincode!;
                        entityData.postofficename = powerOfAttorneyInformationModel.postofficename!;
                        entityData.owner_of_property_in_maharashtra = powerOfAttorneyInformationModel.owner_of_property_in_maharashtra;
                        entityData.propertyType = powerOfAttorneyInformationModel.PropertyTypeMaster;
                        entityData.property_district_code = powerOfAttorneyInformationModel.property_district_code;
                        entityData.property_district_name_in_marathi = powerOfAttorneyInformationModel.property_district_name_in_marathi;
                        entityData.property_district_name_in_english = powerOfAttorneyInformationModel.property_district_name_in_english;
                        entityData.property_taluka_code = powerOfAttorneyInformationModel.property_taluka_code;
                        entityData.property_taluka_name = powerOfAttorneyInformationModel.property_taluka_name;
                        entityData.property_city_code = powerOfAttorneyInformationModel.property_city_code;
                        entityData.property_city_name = powerOfAttorneyInformationModel.property_city_name;
                        entityData.khateno = powerOfAttorneyInformationModel.khateno;
                        entityData.city_servey_no = powerOfAttorneyInformationModel.city_servey_no;
                        entityData.ulpin = powerOfAttorneyInformationModel.ulpin;
                        entityData.username = string.IsNullOrEmpty(powerOfAttorneyInformationModel.username) ? "NA" : powerOfAttorneyInformationModel.username;
                        entityData.company_name_in_eng = powerOfAttorneyInformationModel.company_name_in_eng;
                        entityData.company_name_in_marathi = powerOfAttorneyInformationModel.company_name_in_marathi;
                        entityData.userMaster = powerOfAttorneyInformationModel.userMaster;
                        entityData.applicationDTL = powerOfAttorneyInformationModel.applicationDTL;
                        entityData.is_taker = true;
                        entityData.attornytype_code = powerOfAttorneyInformationModel.attornytype_code;
                        entityData.attornytype_desc = powerOfAttorneyInformationModel.attornytype_desc;
                        entityData.isPOAisPartofDast = powerOfAttorneyInformationModel.isPOAisPartofDast.ToUpper();
                        entityData.isDeclerationInvolvedInPOA = powerOfAttorneyInformationModel.isDeclerationInvolvedInPOA.ToUpper();
                        entityData.isPOAPermanant = powerOfAttorneyInformationModel.isPOAPermanant.ToUpper();
                        entityData.isTransferRights = powerOfAttorneyInformationModel.isTransferRights.ToUpper();
                        entityData.dast_no = powerOfAttorneyInformationModel.dastNo;
                        entityData.dast_no_date = powerOfAttorneyInformationModel.dastNoDate;
                        entityData.dast_no_year = powerOfAttorneyInformationModel.dastNoYear;
                        entityData.isDastVerified = powerOfAttorneyInformationModel.isDastVerified;
                        entityData.verifieddastData = powerOfAttorneyInformationModel.verifiedDastData;
                        entityData.digcode = powerOfAttorneyInformationModel.digcode;
                        entityData.digname = powerOfAttorneyInformationModel.digname!;
                        entityData.poa_district_code = powerOfAttorneyInformationModel.poa_district_code;
                        entityData.poa_district_name = powerOfAttorneyInformationModel.poa_district_name;
                        //entityData.poa_district_name_in_eng = powerOfAttorneyInformationModel.poa_district_name_in_eng;
                        entityData.sro_office_code = powerOfAttorneyInformationModel.sro_office_code;
                        entityData.sro_office_name = powerOfAttorneyInformationModel.sro_office_name;
                        entityData.alias_name = powerOfAttorneyInformationModel.alias_name!;
                        entityData.gender_code = powerOfAttorneyInformationModel.gender_code!;
                        entityData.gender_description = powerOfAttorneyInformationModel.gender_description!.Trim().ToUpper();
                        /*entityData.khata_type_code = powerOfAttorneyInformationModel.khata_type_code!;
                        entityData.khata_type_name = powerOfAttorneyInformationModel.khata_type_name!;
                        entityData.owner_status_code = powerOfAttorneyInformationModel.holder_type_code!;
                        entityData.owner_status_description = powerOfAttorneyInformationModel.holder_type_name!;*/
                        entityData.dob = powerOfAttorneyInformationModel.dob!;
                        entityData.mother_name_in_marathi = powerOfAttorneyInformationModel.mother_name_in_marathi.Trim();
                        entityData.mother_name_in_eng = textInfo.ToTitleCase(powerOfAttorneyInformationModel.mother_name_in_eng.Trim());
                        /*entityData.apk_code = powerOfAttorneyInformationModel.apk_code;
                        entityData.apk_description = powerOfAttorneyInformationModel.apk_description;
                        entityData.aapak = powerOfAttorneyInformationModel.aapak;*/
                        entityData.landBuyArea = powerOfAttorneyInformationModel.landBuyArea;
                        entityData.lr_property_id = "NA";
                        _context.powerOfAttorneyInformation.Attach(entityData);
                        _context.SaveChanges();

                        //Get Saved Row ID
                        int power_of_attorney_id = entityData.power_of_attorney_id;

                        string VerificationType = string.Empty;
                        bool checkAddressFlag = true;
                        //bool checkSignFlag = false;
                        string CurrentDateTime = DateTime.Now.ToString("yyyy-MMM-dd-HHmmss");
                        if (powerOfAttorneyInformationModel.address_type == "INDIA")
                        {
                            VerificationType = "MOBILENO";
                            if (!string.IsNullOrEmpty(takerData.address.indiaAddress!.addressProofSrc) && !string.IsNullOrEmpty(takerData.address.indiaAddress.addressProofName))
                            {
                                string[] AddressData = takerData.address.indiaAddress.addressProofSrc.Split(",");
                                checkAddressFlag = methodForFile.SaveImageForApplicant(AddressData[1], takerData.address.indiaAddress.addressProofName, power_of_attorney_id.ToString(), "AddressProof", FolderPath + @"\", CurrentDateTime);
                                string AddressProofExt = Path.GetExtension(takerData.address.indiaAddress.addressProofName);
                                powerOfAttorneyInformationModel.address_proof_document_name = "AddressProof" + power_of_attorney_id + "_" + CurrentDateTime + AddressProofExt;
                                powerOfAttorneyInformationModel.address_proof_document_path = FolderPath + power_of_attorney_id + @"\" + powerOfAttorneyInformationModel.address_proof_document_name;

                                var UpdateAddressFilePath = _context.powerOfAttorneyInformation.Where(w => w.power_of_attorney_id == power_of_attorney_id).FirstOrDefault();
                                if (UpdateAddressFilePath != null)
                                {
                                    entityData.address_proof_document_name = powerOfAttorneyInformationModel.address_proof_document_name;
                                    entityData.address_proof_document_path = powerOfAttorneyInformationModel.address_proof_document_path;
                                    _context.Entry(entityData).CurrentValues.SetValues(entityData);
                                    _context.SaveChanges();
                                }
                            }

                            //string[] signData = takerData.address.indiaAddress.signatureSrc.Split(",");
                            //checkSignFlag = methodForFile.SaveImageForApplicant(signData[1], takerData.address.indiaAddress.signatureName, power_of_attorney_id.ToString(), "Signature", FolderPath + @"\", CurrentDateTime);
                            //string SignatureExt = Path.GetExtension(takerData.address.indiaAddress.signatureName);
                            //powerOfAttorneyInformationModel.signed_file_name = "Signature" + power_of_attorney_id + "_" + CurrentDateTime + SignatureExt;
                            //powerOfAttorneyInformationModel.signed_file_path = FolderPath + power_of_attorney_id + @"\" + powerOfAttorneyInformationModel.signed_file_name;

                            //var UpdateSignFilePath = _context.powerOfAttorneyInformation.Where(w => w.power_of_attorney_id == power_of_attorney_id).FirstOrDefault();
                            //if (UpdateSignFilePath != null)
                            //{
                            //    entityData.signed_file_name = powerOfAttorneyInformationModel.signed_file_name;
                            //    entityData.signed_file_path = powerOfAttorneyInformationModel.signed_file_path;
                            //    _context.Entry(entityData).CurrentValues.SetValues(entityData);
                            //    _context.SaveChanges();
                            //}
                        }
                        //if (powerOfAttorneyInformationModel.address_type == "FOREIGN")
                        //{
                        //    VerificationType = "EMAILID";
                        //    string[] signData = takerData.address.foreignAddress.signatureSrc.Split(",");
                        //    checkSignFlag = methodForFile.SaveImageForApplicant(signData[1], takerData.address.foreignAddress.signatureName, power_of_attorney_id.ToString(), "Signature", FolderPath + @"\", CurrentDateTime);

                        //    string SignatureExt = Path.GetExtension(takerData.address.foreignAddress.signatureName);
                        //    powerOfAttorneyInformationModel.signed_file_name = "Signature" + power_of_attorney_id + "_" + CurrentDateTime + SignatureExt;
                        //    powerOfAttorneyInformationModel.signed_file_path = FolderPath + power_of_attorney_id + @"\" + powerOfAttorneyInformationModel.signed_file_name;
                        //    var UpdateSignFilePath = _context.powerOfAttorneyInformation.Where(w => w.power_of_attorney_id == power_of_attorney_id).FirstOrDefault();
                        //    if (UpdateSignFilePath != null)
                        //    {
                        //        entityData.signed_file_name = powerOfAttorneyInformationModel.signed_file_name;
                        //        entityData.signed_file_path = powerOfAttorneyInformationModel.signed_file_path;
                        //        _context.Entry(entityData).CurrentValues.SetValues(entityData);
                        //        _context.SaveChanges();
                        //    }
                        //}
                        //string[] imgData = takerData.photo.passportSrc.Split(",");
                        //bool checkImgFlag = methodForFile.SaveImageForApplicant(imgData[1], takerData.photo.passportName, power_of_attorney_id.ToString(), "PassportPhoto", FolderPath + @"\", CurrentDateTime);
                        //string ImgExt = Path.GetExtension(takerData.photo.passportName);
                        //powerOfAttorneyInformationModel.profile_pic_file_name = "PassportPhoto" + power_of_attorney_id + "_" + CurrentDateTime + ImgExt;
                        //powerOfAttorneyInformationModel.profile_pic_file_path = FolderPath +  power_of_attorney_id + @"\" + powerOfAttorneyInformationModel.profile_pic_file_name;

                        //var UpdateImgFilePath = _context.powerOfAttorneyInformation.Where(w => w.power_of_attorney_id == power_of_attorney_id).FirstOrDefault();
                        //if (UpdateImgFilePath != null)
                        //{
                        //    entityData.profile_pic_file_name = powerOfAttorneyInformationModel.profile_pic_file_name;
                        //    entityData.profile_pic_file_path = powerOfAttorneyInformationModel.profile_pic_file_path;
                        //    _context.Entry(entityData).CurrentValues.SetValues(entityData);
                        //    _context.SaveChanges();
                        //}

                        if (checkAddressFlag)
                        {
                            _context.SaveChanges();
                            dbContextTransaction.Commit();
                            dbContextTransaction.Dispose();
                            return "Success";
                        }
                        else
                        {
                            //dbContextTransaction.Rollback();
                            methodForFile.DeleteFile(powerOfAttorneyInformationModel.profile_pic_file_path!);
                            //methodForFile.DeleteFile(powerOfAttorneyInformationModel.signed_file_path);
                            return "Address Proof File Is Not Uploaded";
                        }
                        //if (!checkImgFlag)
                        //{
                        //    dbContextTransaction.Rollback();
                        //    methodForFile.DeleteFile(powerOfAttorneyInformationModel.address_proof_document_path);
                        //    methodForFile.DeleteFile(powerOfAttorneyInformationModel.signed_file_path);
                        //    return "Profile Picture Is Not Uploaded";
                        //}
                        //if (!checkAddressFlag)
                        //{
                        //    dbContextTransaction.Rollback();
                        //    methodForFile.DeleteFile(powerOfAttorneyInformationModel.profile_pic_file_path);
                        //    methodForFile.DeleteFile(powerOfAttorneyInformationModel.signed_file_path);
                        //    return "Address Proof File Is Not Uploaded";
                        //}
                        //if (!checkSignFlag)
                        //{
                        //    dbContextTransaction.Rollback();
                        //    methodForFile.DeleteFile(powerOfAttorneyInformationModel.profile_pic_file_path);
                        //    methodForFile.DeleteFile(powerOfAttorneyInformationModel.address_proof_document_path);
                        //    return "Signature File Is Not Uploaded";
                        //}
                        //else
                        //{
                        //    dbContextTransaction.Rollback();
                        //    return "Some Files Are Not Uploaded";
                        //}
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

        public string EditCourtClaimInformation(EditCourtClaimInformationData courtClaimInformationData)
        {
            using (var dbContextTransaction = _context.Database.BeginTransaction())
            {
                try
                {
                    CourtClaimInformation entityData = new CourtClaimInformation();
                    entityData = _context.courtClaimInformation.Include(i => i.userMaster).Include(app => app.applicationDTL).Where(data => data.court_claim_id.Equals(courtClaimInformationData.court_claim_id)).FirstOrDefault()!;
                    if (entityData != null)
                    {
                        // Assign Values to Model
                        CourtClaimInformationModel courtClaimInformationModel = new CourtClaimInformationModel();
                        UserMaster userMaster = _context.userMasters.FirstOrDefault(s => s.userid == courtClaimInformationData.userid)!;
                        courtClaimInformationModel.userMaster = userMaster;

                        ApplicationDTL applicationDTL = _context.applicationDTL.FirstOrDefault(s => s.applicationid == courtClaimInformationData.applicationid)!;
                        courtClaimInformationModel.applicationDTL = applicationDTL;

                        courtClaimInformationModel.court_case_code = courtClaimInformationData.caseDawa!.caseNoCode;
                        courtClaimInformationModel.court_case_name = courtClaimInformationData.caseDawa.caseNolabel;
                        courtClaimInformationModel.court_case_type_code = courtClaimInformationData.caseType!.caseTypeCode;
                        courtClaimInformationModel.court_case_type_name = courtClaimInformationData.caseType.caseTypeLabel;
                        courtClaimInformationModel.lr_property_uid = courtClaimInformationData.lrPropertyUID;
                        courtClaimInformationModel.city_servey_no = courtClaimInformationData.nabhu;
                        courtClaimInformationModel.order_details = courtClaimInformationData.orderDetails;
                        courtClaimInformationModel.stay_order = courtClaimInformationData.stayOrder;
                        courtClaimInformationModel.sub_property_no = courtClaimInformationData.subPropNo;

                        //Assign Data to Table fields to Update record
                        entityData.userMaster = courtClaimInformationModel.userMaster;
                        entityData.applicationDTL = courtClaimInformationModel.applicationDTL;
                        entityData.court_case_code = courtClaimInformationModel.court_case_code;
                        entityData.court_case_name = courtClaimInformationModel.court_case_name;
                        entityData.court_case_type_code = courtClaimInformationModel.court_case_type_code;
                        entityData.court_case_type_name = courtClaimInformationModel.court_case_type_name;
                        entityData.lr_property_uid = courtClaimInformationModel.lr_property_uid;
                        entityData.city_servey_no = courtClaimInformationModel.city_servey_no;
                        entityData.order_details = courtClaimInformationModel.order_details;
                        entityData.stay_order = courtClaimInformationModel.stay_order!.Trim().ToUpper();
                        entityData.sub_property_no = courtClaimInformationModel.sub_property_no;
                        _context.courtClaimInformation.Attach(entityData);
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

        public string EditDocUploadedData(EditAddUploadedDocumentsDTLData addUploadedDocuments)
        {

            MethodForFileUpload methodForFile = new MethodForFileUpload();
            string DocUploadStatus = string.Empty;
            string FolderPath = @"\\10.10.248.2\pde_propertycard\MUTATIONDOCS\" + addUploadedDocuments.applicationid + @"\DOCUMENTS\";
            UploadedDocumentsDTL entityData = new UploadedDocumentsDTL();
            using (var dbContextTransaction = _context.Database.BeginTransaction())
            {
                try
                {
                    entityData = _context.uploadedDocumentsDTLs.Include(i => i.userMaster).Include(app => app.applicationDTL).Where(data => data.uploaded_doc_id.Equals(addUploadedDocuments.uploaded_doc_id)).FirstOrDefault()!;
                    if (entityData != null)
                    {
                        // Assign Values to Model
                        UploadedDocumentsDTLModel uploadedDocumentsDTLModel = new UploadedDocumentsDTLModel();
                        UserMaster userMaster = _context.userMasters.FirstOrDefault(s => s.userid == addUploadedDocuments.userid)!;
                        uploadedDocumentsDTLModel.userMaster = userMaster;

                        ApplicationDTL applicationDTL = _context.applicationDTL.FirstOrDefault(s => s.applicationid == addUploadedDocuments.applicationid)!;
                        uploadedDocumentsDTLModel.applicationDTL = applicationDTL;

                        //DocumentTypeMaster documentTypeMaster = _context.documentTypeMasters.FirstOrDefault(s => s.document_type_id == Convert.ToInt32(addUploadedDocuments.docType.document_code));
                        //uploadedDocumentsDTLModel.documentType = documentTypeMaster;

                        uploadedDocumentsDTLModel.documentTypeCode = addUploadedDocuments.docType!.document_code;
                        uploadedDocumentsDTLModel.documentType = addUploadedDocuments.docType.document_name;

                        //uploadedDocumentsDTLModel.city_servey_no = addUploadedDocuments.nabhu;
                        uploadedDocumentsDTLModel.document_name = addUploadedDocuments.docUpload!.docName;
                        uploadedDocumentsDTLModel.document_path = FolderPath + uploadedDocumentsDTLModel.document_name;

                        //Assign Data to Table fields to insert new records
                        entityData.userMaster = uploadedDocumentsDTLModel.userMaster;
                        entityData.applicationDTL = uploadedDocumentsDTLModel.applicationDTL;
                        entityData.document_type_code = uploadedDocumentsDTLModel.documentTypeCode;
                        entityData.document_type = uploadedDocumentsDTLModel.documentType;

                        entityData.city_servey_no = uploadedDocumentsDTLModel.city_servey_no;
                        entityData.document_name = uploadedDocumentsDTLModel.document_name;
                        entityData.document_path = uploadedDocumentsDTLModel.document_path;
                        _context.uploadedDocumentsDTLs.Attach(entityData);

                        DocUploadStatus = methodForFile.ConvertBase64ToPdf(FolderPath, addUploadedDocuments.docUpload.docName!, addUploadedDocuments.docUpload.docSrc!);//methodForFile.UploadPDFFile(FolderPath, addUploadedDocuments.docUpload);
                        if (DocUploadStatus == "Success")
                        {
                            _context.SaveChanges();
                            dbContextTransaction.Commit();
                            dbContextTransaction.Dispose();
                            return "Success";
                        }
                        else
                        {
                            return DocUploadStatus;
                        }
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


        //Delete
        public string DeleteApplicant(DeleteApplicant deletdata)
        {
            using (var scope = new TransactionScope())
            {
                try
                {
                    var entity = _context.applicantMasters.FirstOrDefault(s => s.applicantid == deletdata.applicantid && s.isDeleted == false)!;
                    if (entity != null)
                    {
                        var applicationDTLdata = _context.applicationDTL.Where(data => data.applicationid!.Equals(deletdata.applicationid)).FirstOrDefault();
                        if (applicationDTLdata != null)
                        {
                            string applicantIDs = applicationDTLdata.applicantIDs!;
                            string[] applicantids = applicantIDs.Split(',');
                            var updatedIds = applicantids.Where(id => id != deletdata.applicantid.ToString());

                            // Join the remaining IDs back into a string
                            string result = string.Join(",", updatedIds);
                            applicationDTLdata.applicantIDs = result;
                            _context.Entry(applicationDTLdata).CurrentValues.SetValues(applicationDTLdata);
                            _context.SaveChanges();
                        }
                        entity.isDeleted = true;
                        entity.deleteddate = DateOnly.FromDateTime(DateTime.Now);
                        _context.applicantMasters.Attach(entity);
                        _context.SaveChanges();
                        scope.Complete();
                        return "Success";
                    }
                    else
                    {
                        return "Data Not Found";
                    }
                }
                catch (Exception ex)
                {
                    throw new HandleException(ex.Message.ToString());
                }
            }
        }

        public string DeleteMutationCTSNoData(DeleteMutationCTSNoData deletdata)
        {
            using (var scope = new TransactionScope())
            {
                try
                {
                    var entity = _context.mutationCTSNoDTLs.FirstOrDefault(s => s.mutation_cts_no_id == deletdata.mutation_cts_no_id && s.isDeleted == false)!;
                    if (entity != null)
                    {
                        var applicationDTLdata = _context.applicationDTL.Where(data => data.applicationid!.Equals(deletdata.applicationid)).FirstOrDefault();
                        if (applicationDTLdata != null)
                        {
                            string getIDS = applicationDTLdata.mutation_cts_nos!;
                            string[] IdsArray = getIDS.Split(',');
                            var updatedIds = IdsArray.Where(id => id != deletdata.mutation_cts_no_id.ToString());

                            // Join the remaining IDs back into a string
                            string result = string.Join(",", updatedIds);
                            applicationDTLdata.mutation_cts_nos = result;
                            _context.Entry(applicationDTLdata).CurrentValues.SetValues(applicationDTLdata);
                            _context.SaveChanges();
                        }
                        entity.isDeleted = true;
                        entity.deleteddate = DateOnly.FromDateTime(DateTime.Now);
                        _context.mutationCTSNoDTLs.Attach(entity);
                        _context.SaveChanges();
                        scope.Complete();
                        return "Success";
                    }
                    else
                    {
                        return "Data Not Found";
                    }
                }
                catch (Exception ex)
                {
                    throw new HandleException(ex.Message.ToString());
                }
            }
        }

        public string DeleteDastInformationData(DeleteDastInformationData deletdata)
        {
            using (var scope = new TransactionScope())
            {
                try
                {
                    var entity = _context.dastInformation.FirstOrDefault(s => s.dast_id == deletdata.dastid && s.isDeleted == false)!;
                    if (entity != null)
                    {
                        var applicationDTLdata = _context.applicationDTL.Where(data => data.applicationid!.Equals(deletdata.applicationid)).FirstOrDefault();
                        if (applicationDTLdata != null)
                        {
                            string getIDS = applicationDTLdata.dastIDs!;
                            string[] IdsArray = getIDS.Split(',');
                            var updatedIds = IdsArray.Where(id => id != deletdata.dastid.ToString());

                            // Join the remaining IDs back into a string
                            string result = string.Join(",", updatedIds);
                            applicationDTLdata.dastIDs = result;
                            _context.Entry(applicationDTLdata).CurrentValues.SetValues(applicationDTLdata);
                            _context.SaveChanges();
                        }
                        entity.isDeleted = true;
                        entity.deleteddate = DateOnly.FromDateTime(DateTime.Now);
                        _context.dastInformation.Attach(entity);
                        _context.SaveChanges();
                        scope.Complete();
                        return "Success";
                    }
                    else
                    {
                        return "Data Not Found";
                    }
                }
                catch (Exception ex)
                {
                    throw new HandleException(ex.Message.ToString());
                }
            }
        }

        public string DeletePowerOfAttorneyGiver(DeletePowerOfAttorneyInformationDataForGiver deletdata)
        {
            using (var scope = new TransactionScope())
            {
                try
                {
                    var entity = _context.powerOfAttorneyInformation.Include(i => i.userMaster).Include(app => app.applicationDTL).Where(data => data.power_of_attorney_id.Equals(deletdata.power_of_attorney_id) && data.is_taker == false && data.isDeleted == false).FirstOrDefault();
                    if (entity != null)
                    {
                        var applicationDTLdata = _context.applicationDTL.Where(data => data.applicationid!.Equals(deletdata.applicationid)).FirstOrDefault();
                        if (applicationDTLdata != null)
                        {
                            string getIDS = applicationDTLdata.powerOfAttorneyIDs!;
                            string[] IdsArray = getIDS.Split(',');
                            var updatedIds = IdsArray.Where(id => id != deletdata.power_of_attorney_id.ToString());

                            // Join the remaining IDs back into a string
                            string result = string.Join(",", updatedIds);
                            applicationDTLdata.powerOfAttorneyIDs = result;
                            _context.Entry(applicationDTLdata).CurrentValues.SetValues(applicationDTLdata);
                            _context.SaveChanges();
                        }
                        entity.isDeleted = true;
                        entity.deleteddate = DateOnly.FromDateTime(DateTime.Now);
                        _context.powerOfAttorneyInformation.Attach(entity);
                        _context.SaveChanges();
                        scope.Complete();
                        return "Success";
                    }
                    else
                    {
                        return "Data Not Found";
                    }
                }
                catch (Exception ex)
                {
                    throw new HandleException(ex.Message.ToString());
                }
            }
        }

        public string DeletePowerOfAttorneyTaker(DeletePowerOfAttorneyInformationDataForTaker deletdata)
        {
            using (var scope = new TransactionScope())
            {
                try
                {
                    var entity = _context.powerOfAttorneyInformation.Include(i => i.userMaster).Include(app => app.applicationDTL).Include(proptype => proptype.propertyType).Where(data => data.power_of_attorney_id.Equals(deletdata.power_of_attorney_id) && data.is_taker == true && data.isDeleted == false).FirstOrDefault();
                    if (entity != null)
                    {
                        var applicationDTLdata = _context.applicationDTL.Where(data => data.applicationid!.Equals(deletdata.applicationid)).FirstOrDefault();
                        if (applicationDTLdata != null)
                        {
                            string getIDS = applicationDTLdata.powerOfAttorneyIDs!;
                            string[] IdsArray = getIDS.Split(',');
                            var updatedIds = IdsArray.Where(id => id != deletdata.power_of_attorney_id.ToString());

                            // Join the remaining IDs back into a string
                            string result = string.Join(",", updatedIds);
                            applicationDTLdata.powerOfAttorneyIDs = result;
                            //_context.Entry(applicationDTLdata).CurrentValues.SetValues(applicationDTLdata);
                            //_context.SaveChanges();

                            // Delete Giver Data Function
                            string[] giverids = entity.poa_giver_ids.Split(',');
                            for (int i = 0; i < giverids.Length; i++)
                            {
                                var deleteGiverData = _context.powerOfAttorneyInformation.Include(i => i.userMaster).Include(app => app.applicationDTL).Where(data => data.power_of_attorney_id.Equals(Convert.ToInt32(giverids[i])) && data.is_taker == false && data.isDeleted == false).FirstOrDefault();
                                if (deleteGiverData != null)
                                {
                                    deleteGiverData.isDeleted = true;
                                    deleteGiverData.deleteddate = DateOnly.FromDateTime(DateTime.Now);
                                    _context.powerOfAttorneyInformation.Attach(deleteGiverData);
                                    _context.SaveChanges();

                                    getIDS = applicationDTLdata.powerOfAttorneyIDs!;
                                    IdsArray = getIDS.Split(',');
                                    updatedIds = IdsArray.Where(id => id != giverids[i]);

                                    // Join the remaining IDs back into a string
                                    result = string.Join(",", updatedIds);
                                    applicationDTLdata.powerOfAttorneyIDs = result;
                                    _context.Entry(applicationDTLdata).CurrentValues.SetValues(applicationDTLdata);
                                    _context.SaveChanges();
                                }
                            }
                            //End
                        }
                        entity.isDeleted = true;
                        entity.deleteddate = DateOnly.FromDateTime(DateTime.Now);
                        _context.powerOfAttorneyInformation.Attach(entity);
                        _context.SaveChanges();
                        scope.Complete();
                        return "Success";
                    }
                    else
                    {
                        return "Data Not Found";
                    }
                }
                catch (Exception ex)
                {
                    throw new HandleException(ex.Message.ToString());
                }
            }
        }

        public string DeleteCourtClaimInformationData(DeleteCourtClaimInformationData deletdata)
        {
            using (var scope = new TransactionScope())
            {
                try
                {
                    var entity = _context.courtClaimInformation.Include(i => i.userMaster).Include(app => app.applicationDTL).Where(data => data.court_claim_id.Equals(deletdata.court_claim_id) && data.isDeleted == false).FirstOrDefault();
                    if (entity != null)
                    {
                        var applicationDTLdata = _context.applicationDTL.Where(data => data.applicationid!.Equals(deletdata.applicationid)).FirstOrDefault();
                        if (applicationDTLdata != null)
                        {
                            string getIDS = applicationDTLdata.courtClaimIDs!;
                            string[] IdsArray = getIDS.Split(',');
                            var updatedIds = IdsArray.Where(id => id != deletdata.court_claim_id.ToString());

                            // Join the remaining IDs back into a string
                            string result = string.Join(",", updatedIds);
                            applicationDTLdata.courtClaimIDs = result;
                            _context.Entry(applicationDTLdata).CurrentValues.SetValues(applicationDTLdata);
                            _context.SaveChanges();
                        }
                        entity.isDeleted = true;
                        entity.deleteddate = DateOnly.FromDateTime(DateTime.Now);
                        _context.courtClaimInformation.Attach(entity);
                        _context.SaveChanges();
                        scope.Complete();
                        return "Success";
                    }
                    else
                    {
                        return "Data Not Found";
                    }
                }
                catch (Exception ex)
                {
                    throw new HandleException(ex.Message.ToString());
                }
            }
        }

        public string DeleteDocUploadedData(DeleteAddUploadedDocumentsDTLData deletdata)
        {
            MethodForFileUpload methodForFile = new MethodForFileUpload();
            using (var scope = new TransactionScope())
            {
                try
                {
                    var entity = _context.uploadedDocumentsDTLs.Include(app => app.applicationDTL).Where(data => data.uploaded_doc_id.Equals(deletdata.uploaded_doc_id) && data.isDeleted == false).FirstOrDefault();
                    if (entity != null)
                    {
                        var applicationDTLdata = _context.applicationDTL.Where(data => data.applicationid!.Equals(deletdata.applicationid)).FirstOrDefault();
                        if (applicationDTLdata != null)
                        {
                            string getIDS = applicationDTLdata.uploadedDocIDs!;
                            string[] IdsArray = getIDS.Split(',');
                            var updatedIds = IdsArray.Where(id => id != deletdata.uploaded_doc_id.ToString());

                            // Join the remaining IDs back into a string
                            string result = string.Join(",", updatedIds);
                            applicationDTLdata.uploadedDocIDs = result;
                            _context.Entry(applicationDTLdata).CurrentValues.SetValues(applicationDTLdata);
                            _context.SaveChanges();
                        }
                        bool isDeleted = methodForFile.PermanatlyDeleteFile(entity.document_path!);
                        if (isDeleted)
                        {
                            //entity.isDeleted = true;
                            //entity.deleteddate = DateOnly.FromDateTime(DateTime.Now);
                            _context.uploadedDocumentsDTLs.Remove(entity);
                            _context.SaveChanges();
                            scope.Complete();
                            return "Success";
                        }
                        else
                        {
                            return "File is not deleted from Directory.";
                        }
                    }
                    else
                    {
                        return "Data Not Found";
                    }
                }
                catch (Exception ex)
                {
                    throw new HandleException(ex.Message.ToString());
                }
            }
        }

        // Self Declaration
        public string SelfDeclaration(SelfDeclarationData declarationData)
        {
            using (var scope = new TransactionScope())
            {
                try
                {
                    MethodForFileUpload methodForFile = new MethodForFileUpload();
                    string DocUploadStatus = string.Empty;
                    string FolderPath = @"\\10.10.248.2\pde_propertycard\MUTATIONDOCS\" + declarationData.applicationid + @"\DOCUMENTS\SELFDECLARATION\";
                    //string FolderPath = @"D:\WWW\MUTATIONDOCS\" + declarationData.applicationid + @"\DOCUMENTS\SELFDECLARATION\";
                    // Assign Values to Model
                    SelfDeclarationModel selfDeclarationModel = new SelfDeclarationModel();

                    ApplicationDTL applicationDTL = _context.applicationDTL.FirstOrDefault(s => s.applicationid == declarationData.applicationid)!;
                    selfDeclarationModel.self_declaration_doc_name = declarationData.docUpload!.docName;
                    selfDeclarationModel.self_declaration_doc_path = FolderPath + selfDeclarationModel.self_declaration_doc_name;

                    if (applicationDTL != null)
                    {
                        //DocUploadStatus = methodForFile.UploadPDFFile(FolderPath, declarationData.docUpload);
                        string imageName = System.IO.Path.GetFileNameWithoutExtension(selfDeclarationModel.self_declaration_doc_name!);
                        if (methodForFile.ContainsSpecialCharacters(imageName))
                        {
                            return selfDeclarationModel.self_declaration_doc_name! + " Image Name contains special charactes.";
                        }
                        else
                        {

                            DocUploadStatus = methodForFile.ConvertBase64ToPdf(FolderPath, selfDeclarationModel.self_declaration_doc_name!, declarationData.docUpload.docSrc!);
                            applicationDTL.self_declaration_doc_name = selfDeclarationModel.self_declaration_doc_name;
                            applicationDTL.self_declaration_doc_path = selfDeclarationModel.self_declaration_doc_path;
                            applicationDTL.status = 9;
                            _context.applicationDTL.Attach(applicationDTL);
                            if (DocUploadStatus == "Success")
                            {
                                _context.SaveChanges();
                                ApplicationStatusHistory applicationStatusHistory = new ApplicationStatusHistory();
                                applicationStatusHistory.applicationid = declarationData.applicationid;
                                applicationStatusHistory.application_status = "Created";
                                applicationStatusHistory.mutation_type_code = applicationDTL.mutation_type_code;
                                applicationStatusHistory.mutation_type_name = applicationDTL.mutation_type_name;
                                _context.applicationStatusHistories.Add(applicationStatusHistory);
                                _context.SaveChanges();

                                // Below code is added to store Document Uploaded Details. ( On 26 May 2026 )
                                UserMaster userMaster = _context.userMasters.FirstOrDefault(s => s.userid == declarationData.userid)!;
                                UploadedDocumentsDTL uploadedDocumentsDTL = new UploadedDocumentsDTL();
                                uploadedDocumentsDTL.userMaster = userMaster;
                                uploadedDocumentsDTL.applicationDTL = applicationDTL;
                                uploadedDocumentsDTL.document_type_code = "102";
                                uploadedDocumentsDTL.document_type = "इप्सित अर्ज";
                                uploadedDocumentsDTL.document_name = selfDeclarationModel.self_declaration_doc_name;
                                uploadedDocumentsDTL.document_path = selfDeclarationModel.self_declaration_doc_path;
                                _context.uploadedDocumentsDTLs.Add(uploadedDocumentsDTL);
                                _context.SaveChanges();
                                int uploadedDocID = uploadedDocumentsDTL.uploaded_doc_id;
                                var applicationDTLdata = _context.applicationDTL.Where(data => data.applicationid!.Equals(declarationData.applicationid)).FirstOrDefault();
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
                                    _context.Entry(applicationDTLdata).CurrentValues.SetValues(applicationDTLdata);
                                }
                                _context.SaveChanges();
                                // End Code

                                scope.Complete();
                                return "Success";
                            }
                            else
                            {
                                return DocUploadStatus;
                            }
                        }
                    }
                    else
                    {
                        return "Application ID Not Found";
                    }
                }
                catch (Exception ex)
                {
                    throw new HandleException(ex.Message.ToString());
                }
            }
        }

        public async Task<List<ApplicationResultDto>> GetMutationApplicationDataForValidate(string applicationid)
        {
            return await (
                from app in _contextR.applicationDTL
                join cts in _contextR.mutationCTSNoDTLs
                     on app.applicationid equals cts.applicationDTL!.applicationid
                join gt in _contextR.mutationDTL
                     .Where(g => g.isTaker == 0)
                     on app.applicationid equals gt.applicationDTL!.applicationid
                where
                         //app.inwardno == "NA"
                         //        && new[] { 6, 7, 8, 9 }.Contains(app.status)&&
                         app.applicationid == applicationid
                select new ApplicationResultDto
                {
                    ApplicationId = app.applicationid,
                    IsTaker = gt.isTaker,
                    DistrictCode = cts.village_or_peth_code!.Substring(0, 2),
                    OfficeCode = cts.village_or_peth_code.Substring(0, 4),
                    VillageOrPethCode = cts.village_or_peth_code,
                    CityServeyNo = gt.city_servey_no,
                    SubPropertyNo = gt.sub_property_no,
                    MutationSrNo = gt.mutation_srno,
                    OwnerNumber = gt.owner_number
                }
            )
            .Distinct()
            .ToListAsync();
        }

        public FetchDashboardData FetchDashboard(string applicationid)
        {
            try
            {
                ApplicationDTL applicationDTL = _contextR.applicationDTL.Include(apptype => apptype.applicationTypeMaster).FirstOrDefault(s => s.applicationid == applicationid)!;
                FetchDashboardData fetchData = new FetchDashboardData();
                if (applicationDTL != null)
                {
                    fetchData.applicationID = applicationDTL.applicationid;
                    //fetchData.inwardno = (applicationDTL.inwardno == "NA") ? "-" : applicationDTL.inwardno!;
                    fetchData.slashinwardno = (applicationDTL.inwardno == "NA") ? "-" : AddSlashInwardNo(applicationDTL.inwardno!)!;
                    fetchData.inwardno = (applicationDTL.inwardno == "NA") ? "-" : applicationDTL.inwardno!;
                    // fetchData.status = (applicationDTL.status == 9) ? "Completed" : "Partially Completed";
                    fetchData.applicationsubmitted = (applicationDTL.status == 10) ? true : false;
                    fetchData.application_type = (applicationDTL.applicationTypeMaster!.applicationtypeid == 1) ? "Y" : "N";
                    Status status = new Status();
                    if (applicationDTL.status == 10)
                    {
                        status.code = "1";
                        status.label = "Submitted";
                    }
                    else if (applicationDTL.status == 9)
                    {
                        status.code = "3";
                        status.label = "Please Re-Submit";
                    }
                    else if (applicationDTL.status == 11)
                    {
                        status.code = "4";
                        status.label = "Truti Patra";
                    }
                    else if (applicationDTL.status == 12)
                    {
                        status.code = "5";
                        status.label = "Rejected";
                    }
                    else if (applicationDTL.status == 13)
                    {
                        status.code = "6";
                        status.label = "Nikali Patra";
                    }
                    else if (applicationDTL.status == 14)
                    {
                        status.code = "7";
                        status.label = "Notice 9";
                    }
                    else if (applicationDTL.status == 15)
                    {
                        status.code = "8";
                        status.label = "Unable To Generate Inward No";
                    }
                    else
                    {
                        status.code = "2";
                        status.label = "Partially Submitted";
                    }
                    fetchData.status = status;
                    fetchData.docName = applicationDTL.self_declaration_doc_name;
                    fetchData.docSrc = applicationDTL.self_declaration_doc_path;
                    fetchData.application_date = applicationDTL.createddatetime.ToString("dd/MM/yyyy");
                    fetchData.mutation_type_code = applicationDTL.mutation_type_code;
                    fetchData.mutation_type = applicationDTL.mutation_type_name;
                    fetchData.district_name_in_marathi = applicationDTL.district_name_in_marathi;
                    fetchData.district_name_in_english = applicationDTL.district_name_in_english;
                    fetchData.isCourtDawa = applicationDTL.Is_the_claim_pending_before_the_court == true ? "YES" : "NO";
                    fetchData.isMainPatra = applicationDTL.do_you_have_power_of_attorney == true ? "YES" : "NO";
                    fetchData.isDastApplicable = applicationDTL.does_the_original_charter_info_apply == true ? "YES" : "NO";
                    //File
                    //if (applicationDTL.self_declaration_doc_path != "NA")
                    //{
                    //    Byte[] fileBytes = File.ReadAllBytes(applicationDTL.self_declaration_doc_path!);
                    //    string FileExt = Path.GetExtension(applicationDTL.self_declaration_doc_path)!;
                    //    var content = Convert.ToBase64String(fileBytes);
                    //    fetchData.docSrc = string.IsNullOrEmpty(content) ? "NA" : "data:pdf/" + FileExt.Replace(".", "") + ";base64," + content;
                    //}
                    fetchData.taluka = applicationDTL.office_name;

                    if (!string.IsNullOrEmpty(applicationDTL.mutation_cts_nos))
                    {
                        string[] mutationCTSNoIDs = applicationDTL.mutation_cts_nos.Split(",");
                        string nabhunos = string.Empty;
                        if (mutationCTSNoIDs.Length > 0)
                        {
                            for (int i = 0; i < mutationCTSNoIDs.Length; i++)
                            {
                                MutationCTSNoDTL mutationCTSNoDTL = new MutationCTSNoDTL();
                                mutationCTSNoDTL = _contextR.mutationCTSNoDTLs.Include(app => app.applicationDTL).Where(data => data.mutation_cts_no_id.Equals(Convert.ToInt32(mutationCTSNoIDs[i]))).FirstOrDefault()!;
                                if (mutationCTSNoDTL != null)
                                {
                                    fetchData.village = mutationCTSNoDTL.village_or_peth_name;
                                    if (i == 0)
                                    {
                                        nabhunos = mutationCTSNoDTL.selected_city_servey_no!;
                                    }
                                    else
                                    {
                                        nabhunos = nabhunos + " ," + mutationCTSNoDTL.selected_city_servey_no;
                                    }
                                }
                            }
                        }
                        fetchData.nabhunos = nabhunos;
                    }
                    List<string> excludedDocumentTypeIds = new List<string> { "902", "903", "909", "905" };
                    // var result = _context.uploadedDocumentsDTLs.GroupBy(p => p.applicationDTL.applicationid).Select(g => new { applicationid = g.Key, Names = string.Join(",", g.Select(p => p.document_name)) }).ToList();
                    var uploadedDocNamesList = _contextR.uploadedDocumentsDTLs.Where(a => a.applicationDTL!.applicationid == applicationid && a.isDeleted == false
                    && a.truti_patra_flag == "NA" && !excludedDocumentTypeIds.Contains(a.document_type_code!)).Select(a => a.document_name).ToList();
                    string docNames = String.Join(", ", uploadedDocNamesList);
                    fetchData.uploaded_docName = docNames;

                    // Fetch NIC Documents uploaded by nic
                    List<UploadedDocumentsDTL> uploadedDocumentsDTL = new List<UploadedDocumentsDTL>();
                    List<NICDocdata> nicDocDataList = new List<NICDocdata>();
                    if (applicationDTL.status == 11)
                    {
                        uploadedDocumentsDTL = _contextR.uploadedDocumentsDTLs.OrderByDescending(o => o.createddatetime)
                        .Where(data => data.applicationDTL!.applicationid!.Equals(applicationid) && data.isDeleted == false && data.document_type_code == "902").ToList();
                        foreach (var data in uploadedDocumentsDTL)
                        {
                            NICDocdata nicDocData = new NICDocdata();
                            if (File.Exists(data.document_path))
                            {
                                //File
                                nicDocData.docName = data.document_name;
                                Byte[] fileBytes = File.ReadAllBytes(data.document_path!);
                                string FileExt = Path.GetExtension(data.document_path)!;
                                var content = Convert.ToBase64String(fileBytes);
                                nicDocData.docSrc = string.IsNullOrEmpty(content) ? "NA" : "data:pdf/" + FileExt.Replace(".", "") + ";base64," + content;
                                nicDocDataList.Add(nicDocData);
                            }
                        }
                    }
                    if (applicationDTL.status == 12)
                    {
                        uploadedDocumentsDTL = _contextR.uploadedDocumentsDTLs.OrderByDescending(o => o.createddatetime)
                        .Where(data => data.applicationDTL!.applicationid!.Equals(applicationid) && data.isDeleted == false && data.document_type_code == "903").ToList();
                        foreach (var data in uploadedDocumentsDTL)
                        {
                            NICDocdata nicDocData = new NICDocdata();
                            if (File.Exists(data.document_path))
                            {
                                //File
                                nicDocData.docName = data.document_name;
                                Byte[] fileBytes = File.ReadAllBytes(data.document_path!);
                                string FileExt = Path.GetExtension(data.document_path)!;
                                var content = Convert.ToBase64String(fileBytes);
                                nicDocData.docSrc = string.IsNullOrEmpty(content) ? "NA" : "data:pdf/" + FileExt.Replace(".", "") + ";base64," + content;
                                nicDocDataList.Add(nicDocData);
                            }
                        }
                    }
                    if (applicationDTL.status == 13)
                    {
                        uploadedDocumentsDTL = _contextR.uploadedDocumentsDTLs.OrderByDescending(o => o.createddatetime)
                        .Where(data => data.applicationDTL!.applicationid!.Equals(applicationid) && data.isDeleted == false && data.document_type_code == "909").ToList();
                        foreach (var data in uploadedDocumentsDTL)
                        {
                            NICDocdata nicDocData = new NICDocdata();
                            if (File.Exists(data.document_path))
                            {
                                //File
                                nicDocData.docName = data.document_name;
                                Byte[] fileBytes = File.ReadAllBytes(data.document_path!);
                                string FileExt = Path.GetExtension(data.document_path)!;
                                var content = Convert.ToBase64String(fileBytes);
                                nicDocData.docSrc = string.IsNullOrEmpty(content) ? "NA" : "data:pdf/" + FileExt.Replace(".", "") + ";base64," + content;
                                nicDocDataList.Add(nicDocData);
                            }
                        }
                    }
                    if (applicationDTL.status == 14)
                    {
                        uploadedDocumentsDTL = _contextR.uploadedDocumentsDTLs.OrderByDescending(o => o.createddatetime)
                        .Where(data => data.applicationDTL!.applicationid!.Equals(applicationid) && data.isDeleted == false && data.document_type_code == "905").ToList();
                        foreach (var data in uploadedDocumentsDTL)
                        {
                            NICDocdata nicDocData = new NICDocdata();
                            if (File.Exists(data.document_path))
                            {
                                //File
                                nicDocData.docName = data.document_name;
                                Byte[] fileBytes = File.ReadAllBytes(data.document_path!);
                                string FileExt = Path.GetExtension(data.document_path)!;
                                var content = Convert.ToBase64String(fileBytes);
                                nicDocData.docSrc = string.IsNullOrEmpty(content) ? "NA" : "data:pdf/" + FileExt.Replace(".", "") + ";base64," + content;
                                nicDocDataList.Add(nicDocData);
                            }
                        }
                    }
                    fetchData.nicDocData = nicDocDataList;
                    InwardNoStatusMaster inwardNoStatus = new InwardNoStatusMaster();
                    inwardNoStatus = _context.inwardNoStatusMasters.Where(u => u.inwardno!.Equals(applicationDTL.inwardno)).OrderByDescending(o => o.srno).FirstOrDefault()!;
                    if (inwardNoStatus != null)
                    {
                        fetchData.inwardno_status = inwardNoStatus.status;
                    }
                    //End Fetch NIC Doc Data 
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

        public List<string> FetchApplicationIDS(int userid)
        {
            try
            {
                var result = _contextR.applicationDTL.Where(u => u.userMaster!.userid == userid && u.isDeleted == false).OrderByDescending(e => e.createddatetime).Select(u => string.Join(",", u.applicationid)).ToList();
                return result;
            }
            catch (Exception ex)
            {
                throw new HandleException(ex.Message.ToString());
            }
        }

        public string RequestOTPForApp(VerificationData verificationData, long OTP)
        {
            string response = string.Empty;
            try
            {
                string NUM = string.Empty;
                if (verificationData.VERIFICATIONTYPE.Trim().ToUpper() == "MOBILENO")
                {
                    NUM = userServices.GenerateOTP();
                }
                if (verificationData.VERIFICATIONTYPE.Trim().ToUpper() == "EMAILID")
                {
                    NUM = userServices.GenerateOTP();
                }
                response = "Success";
                return response;
            }
            catch (Exception ex)
            {
                throw new HandleException(ex.Message.ToString());
            }
        }

        // New Services Created
        public List<string> FetchDocumentTypeIDs(string applicationid)
        {
            try
            {
                List<string> documentTypeIDs = new List<string>();
                var groupedData = _contextR.uploadedDocumentsDTLs.Include(app => app.applicationDTL).Where(data => data.applicationDTL.applicationid.Equals(applicationid))
                .GroupBy(o => o.document_type_code)
                .Select(orderGroup => new
                {
                    documentTypeCode = orderGroup.Key
                }).ToList();

                foreach (var item in groupedData)
                {
                    documentTypeIDs.Add(item.documentTypeCode!.ToString());
                    // Console.WriteLine($"CustomerID: {item.documentTypeCode}");
                }
                return documentTypeIDs;
            }
            catch (Exception ex)
            {
                throw new HandleException(ex.Message.ToString());
            }
        }
        public FetchUploadedDocDataDocTypeWise FetchDocTypeWise(string documentTypeID, string applicationID)
        {
            try
            {
                UploadedDocumentsDTL uploadedDocumentsDTL = new UploadedDocumentsDTL();
                uploadedDocumentsDTL = _contextR.uploadedDocumentsDTLs.Include(i => i.userMaster).Include(app => app.applicationDTL).Where(data => data.document_type_code.Equals(documentTypeID) && data.applicationDTL.applicationid.Equals(applicationID) && data.isDeleted == false).FirstOrDefault()!;
                var documentData = _contextR.uploadedDocumentsDTLs.Include(i => i.userMaster).Include(app => app.applicationDTL).Where(data => data.document_type_code.Equals(documentTypeID) && data.applicationDTL.applicationid.Equals(applicationID) && data.isDeleted == false).ToList();
                FetchUploadedDocDataDocTypeWise fetchData = new FetchUploadedDocDataDocTypeWise();
                if (uploadedDocumentsDTL != null)
                {
                    fetchData.applicationID = uploadedDocumentsDTL.applicationDTL!.applicationid;
                    fetchData.documentID = uploadedDocumentsDTL.uploaded_doc_id.ToString();
                    fetchData.documentTypeCode = Convert.ToInt32(uploadedDocumentsDTL.document_type_code);
                    fetchData.documentType = uploadedDocumentsDTL.document_type;

                    MutationCTSNoDTL mutationCTSNoDTL = new MutationCTSNoDTL();
                    mutationCTSNoDTL = _contextR.mutationCTSNoDTLs.Where(data => data.applicationDTL!.applicationid!.Equals(uploadedDocumentsDTL.applicationDTL.applicationid)).FirstOrDefault()!;
                    //fetchData.city = mutationCTSNoDTL.village_or_peth_name;
                    fetchData.nabhuno = uploadedDocumentsDTL.city_servey_no == null || uploadedDocumentsDTL.city_servey_no == "" ? "NA" : uploadedDocumentsDTL.city_servey_no;
                    List<DocumentData> docData = new List<DocumentData>();
                    if (documentData.Count > 0)
                    {
                        documentData.ForEach(row => docData.Add(new DocumentData()
                        {
                            docName = row.document_name.ToString(),
                            docSrc = row.document_path.ToString(),
                        }));
                        //docData.Add(docData);
                    }

                    foreach (DocumentData Data in docData)
                    {
                        //File
                        Byte[] fileBytes = File.ReadAllBytes(Data.docSrc!);
                        string FileExt = Path.GetExtension(uploadedDocumentsDTL.document_path)!;
                        var content = Convert.ToBase64String(fileBytes);
                        Data.docSrc = string.IsNullOrEmpty(content) ? "NA" : "data:pdf/" + FileExt.Replace(".", "") + ";base64," + content;
                    }
                    fetchData.doc = docData;
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

        public PowerOfAttorneyInformationDataForGiver FetchPOAGiverDataByMutationID(int givertakerid)
        {
            try
            {
                PowerOfAttorneyInformationDataForGiver poadata = new PowerOfAttorneyInformationDataForGiver();
                MutationGiverTakerDTL mutation = new MutationGiverTakerDTL();
                mutation = _context.mutationDTL.Include(i => i.userMaster).Include(app => app.applicationDTL).Include(prop => prop.prop_type).Where(data => data.mutation_givertaker_id.Equals(givertakerid) && data.isDeleted == false).FirstOrDefault()!;

                ApplicationDTL applicationDTL = new ApplicationDTL();
                applicationDTL = FetchApplicationDataForPOA(mutation.applicationDTL!.applicationid!);

                if (mutation != null)
                {
                    poadata.userid = mutation.userMaster!.userid;
                    poadata.applicationid = mutation.applicationDTL!.applicationid;
                    poadata.ctsNo = string.IsNullOrEmpty(mutation.cts_number) ? "NA" : mutation.cts_number;
                    poadata.mutationSroNo = string.IsNullOrEmpty(mutation.mutation_srno) ? "NA" : mutation.mutation_srno;
                    poadata.ownerNo = string.IsNullOrEmpty(mutation.owner_number) ? "NA" : mutation.owner_number;
                    poadata.village_code = string.IsNullOrEmpty(mutation.village_code) ? "NA" : mutation.village_code;
                    poadata.village_name = string.IsNullOrEmpty(mutation.village_name) ? "NA" : mutation.village_name;

                    UserDataForPOAGiver userDataForPOAGiver = new UserDataForPOAGiver();
                    userDataForPOAGiver.suffixcode = string.IsNullOrEmpty(mutation.prefixcode_marathi) ? "NA" : mutation.prefixcode_marathi;
                    userDataForPOAGiver.suffixCodeEng = string.IsNullOrEmpty(mutation.prefixcode_eng) ? "NA" : mutation.prefixcode_eng;
                    userDataForPOAGiver.suffix = string.IsNullOrEmpty(mutation.prefix_in_marathi) ? "NA" : mutation.prefix_in_marathi;
                    userDataForPOAGiver.firstName = string.IsNullOrEmpty(mutation.fname_in_marathi) ? "NA" : mutation.fname_in_marathi;
                    userDataForPOAGiver.middleName = string.IsNullOrEmpty(mutation.mname_in_marathi) ? "NA" : mutation.mname_in_marathi;
                    userDataForPOAGiver.lastName = string.IsNullOrEmpty(mutation.lname_in_marathi) ? "NA" : mutation.lname_in_marathi;
                    userDataForPOAGiver.suffixEng = string.IsNullOrEmpty(mutation.prefix_in_eng) ? "NA" : mutation.prefix_in_eng;
                    userDataForPOAGiver.firstNameEng = string.IsNullOrEmpty(mutation.fname_in_eng) ? "NA" : mutation.fname_in_eng;
                    userDataForPOAGiver.middleNameEng = string.IsNullOrEmpty(mutation.mname_in_eng) ? "NA" : mutation.mname_in_eng;
                    userDataForPOAGiver.lastNameEng = string.IsNullOrEmpty(mutation.lname_in_eng) ? "NA" : mutation.lname_in_eng;
                    userDataForPOAGiver.userName = string.IsNullOrEmpty(mutation.userName) ? "NA" : mutation.userName;
                    userDataForPOAGiver.nabhu = string.IsNullOrEmpty(mutation.city_servey_no) ? "NA" : mutation.city_servey_no;
                    userDataForPOAGiver.lrPropertyUID = string.IsNullOrEmpty(mutation.lr_property_id) ? "NA" : mutation.lr_property_id;
                    userDataForPOAGiver.subPropNo = string.IsNullOrEmpty(mutation.mother_name_in_eng) ? "NA" : mutation.mother_name_in_eng;
                    userDataForPOAGiver.usertype_code = mutation.user_type_code;
                    userDataForPOAGiver.usertype = string.IsNullOrEmpty(mutation.user_type) ? "NA" : mutation.user_type;

                    if (applicationDTL.mutation_type_code == "06" || applicationDTL.mutation_type_code == "07")
                    {
                        userDataForPOAGiver.company_name_in_eng = string.IsNullOrEmpty(mutation.bank_name_in_english) ? "NA" : mutation.bank_name_in_english;
                        userDataForPOAGiver.company_name_in_marathi = string.IsNullOrEmpty(mutation.bank_name_in_marathi) ? "NA" : mutation.bank_name_in_marathi;
                    }
                    else
                    {
                        userDataForPOAGiver.company_name_in_eng = string.IsNullOrEmpty(mutation.company_name_in_eng) ? "NA" : mutation.company_name_in_eng;
                        userDataForPOAGiver.company_name_in_marathi = string.IsNullOrEmpty(mutation.company_name_in_marathi) ? "NA" : mutation.company_name_in_marathi;
                    }
                    poadata.userDetails = userDataForPOAGiver;

                    AddressData address = new AddressData();
                    address.addressType = mutation.address_type;

                    MethodForFileUpload methodForFile = new MethodForFileUpload();
                    if (mutation.address_type == "FOREIGN")
                    {
                        AddressForForeign addressForForeign = new AddressForForeign();
                        addressForForeign.address = string.IsNullOrEmpty(mutation.address) ? "NA" : mutation.address;
                        addressForForeign.mobile = string.IsNullOrEmpty(mutation.mobileno) ? "NA" : mutation.mobileno;
                        addressForForeign.email = string.IsNullOrEmpty(mutation.emailid) ? "NA" : mutation.emailid;
                        addressForForeign.emailOTP = string.IsNullOrEmpty(mutation.emailidverified) ? "NA" : mutation.emailidverified;
                        if (!string.IsNullOrEmpty(mutation.signed_file_path) && mutation.signed_file_path != "NA")
                        {
                            string SignedFileExt = Path.GetExtension(mutation.signed_file_path);
                            string Signed = methodForFile.ConvertImageToBase64(mutation.signed_file_path);
                            addressForForeign.signatureSrc = string.IsNullOrEmpty(Signed) ? "NA" : "data:image/" + SignedFileExt.Replace(".", "") + ";base64," + Signed;
                            addressForForeign.signatureName = mutation.signed_file_name;
                        }
                        else
                        {
                            addressForForeign.signatureSrc = string.IsNullOrEmpty(mutation.signed_file_path) ? "NA" : mutation.signed_file_path;
                            addressForForeign.signatureName = string.IsNullOrEmpty(mutation.signed_file_name) ? "NA" : mutation.signed_file_name;
                        }
                        address.foreignAddress = addressForForeign;
                    }
                    if (mutation.address_type == "INDIA")
                    {
                        AddressForIndia addressForIndia = new AddressForIndia();
                        addressForIndia.state = string.IsNullOrEmpty(mutation.state) ? "NA" : mutation.state;
                        addressForIndia.district = string.IsNullOrEmpty(mutation.district) ? "NA" : mutation.district;
                        addressForIndia.city = string.IsNullOrEmpty(mutation.city) ? "NA" : mutation.city;
                        addressForIndia.taluka = string.IsNullOrEmpty(mutation.taluka) ? "NA" : mutation.taluka;
                        addressForIndia.plotNo = string.IsNullOrEmpty(mutation.flatno_plotno) ? "NA" : mutation.flatno_plotno;
                        addressForIndia.building = string.IsNullOrEmpty(mutation.societyname) ? "NA" : mutation.societyname;
                        addressForIndia.mainRoad = string.IsNullOrEmpty(mutation.mainstreet) ? "NA" : mutation.mainstreet;
                        addressForIndia.impSymbol = string.IsNullOrEmpty(mutation.landmark) ? "NA" : mutation.landmark;
                        addressForIndia.area = string.IsNullOrEmpty(mutation.locality) ? "NA" : mutation.locality;
                        addressForIndia.pincode = string.IsNullOrEmpty(mutation.pincode) ? "NA" : mutation.pincode;
                        addressForIndia.postOfficeName = string.IsNullOrEmpty(mutation.post_office_name) ? "NA" : mutation.post_office_name;


                        if (!string.IsNullOrEmpty(mutation.address_proof_document_path) && mutation.address_proof_document_path != "NA")
                        {
                            addressForIndia.addressProofName = mutation.address_proof_document_name;
                            string AddressFileExt = Path.GetExtension(mutation.address_proof_document_path);
                            string AddressFile = methodForFile.ConvertImageToBase64(mutation.address_proof_document_path);
                            addressForIndia.addressProofSrc = string.IsNullOrEmpty(AddressFile) ? "NA" : "data:image/" + AddressFileExt.Replace(".", "") + ";base64," + AddressFile;
                        }
                        else
                        {
                            addressForIndia.addressProofName = string.IsNullOrEmpty(mutation.address_proof_document_name) ? "NA" : mutation.address_proof_document_name;
                            addressForIndia.addressProofSrc = string.IsNullOrEmpty(mutation.address_proof_document_path) ? "NA" : mutation.address_proof_document_path;
                        }

                        if (!string.IsNullOrEmpty(mutation.signed_file_path) && mutation.signed_file_path != "NA")
                        {
                            string SignedFileExt = Path.GetExtension(mutation.signed_file_path);
                            string Signed = methodForFile.ConvertImageToBase64(mutation.signed_file_path);
                            addressForIndia.signatureSrc = string.IsNullOrEmpty(Signed) ? "NA" : "data:image/" + SignedFileExt.Replace(".", "") + ";base64," + Signed;
                            addressForIndia.signatureName = mutation.signed_file_name;
                        }
                        else
                        {
                            addressForIndia.signatureSrc = string.IsNullOrEmpty(mutation.signed_file_path) ? "NA" : mutation.signed_file_path;
                            addressForIndia.signatureName = string.IsNullOrEmpty(mutation.signed_file_name) ? "NA" : mutation.signed_file_name;
                        }
                        addressForIndia.mobile = string.IsNullOrEmpty(mutation.mobileno) ? "NA" : mutation.mobileno;
                        addressForIndia.mobileOTP = string.IsNullOrEmpty(mutation.mobilenoverified) ? "NA" : mutation.mobilenoverified;
                        address.indiaAddress = addressForIndia;
                    }
                    poadata.address = address;
                }
                else
                {
                    poadata = null!;
                }
                return poadata;
            }
            catch (Exception ex)
            {
                throw new HandleException(ex.Message.ToString());
            }
        }
        //public PowerOfAttorneyInformationDataForGiver FetchPOAGiverDataByMutationID(int givertakerid)
        //{
        //    try
        //    {
        //        PowerOfAttorneyInformationDataForGiver poadata = new PowerOfAttorneyInformationDataForGiver();
        //        MutationGiverTakerDTL mutation = new MutationGiverTakerDTL();
        //        using (var transaction = _contextR.Database.BeginTransaction(System.Data.IsolationLevel.ReadCommitted)) // Changed here
        //        {
        //            mutation = _contextR.mutationDTL.Include(i => i.userMaster).Include(app => app.applicationDTL).Include(prop => prop.prop_type).Where(data => data.mutation_givertaker_id.Equals(givertakerid) && data.isDeleted == false).FirstOrDefault()!;
        //            transaction.Commit();
        //        }
        //        ApplicationDTL applicationDTL = new ApplicationDTL();
        //        applicationDTL = FetchApplicationData(mutation.applicationDTL!.applicationid!);

        //        if (mutation != null)
        //        {
        //            poadata.userid = mutation.userMaster!.userid;
        //            poadata.applicationid = mutation.applicationDTL!.applicationid;
        //            poadata.ctsNo = string.IsNullOrEmpty(mutation.cts_number) ? "NA" : mutation.cts_number;
        //            poadata.mutationSroNo = string.IsNullOrEmpty(mutation.mutation_srno) ? "NA" : mutation.mutation_srno;
        //            poadata.ownerNo = string.IsNullOrEmpty(mutation.owner_number) ? "NA" : mutation.owner_number;
        //            poadata.village_code = string.IsNullOrEmpty(mutation.village_code) ? "NA" : mutation.village_code;
        //            poadata.village_name = string.IsNullOrEmpty(mutation.village_name) ? "NA" : mutation.village_name;

        //            UserDataForPOAGiver userDataForPOAGiver = new UserDataForPOAGiver();
        //            userDataForPOAGiver.suffixcode = string.IsNullOrEmpty(mutation.prefixcode_marathi) ? "NA" : mutation.prefixcode_marathi;
        //            userDataForPOAGiver.suffixCodeEng = string.IsNullOrEmpty(mutation.prefixcode_eng) ? "NA" : mutation.prefixcode_eng;
        //            userDataForPOAGiver.suffix = string.IsNullOrEmpty(mutation.prefix_in_marathi) ? "NA" : mutation.prefix_in_marathi;
        //            userDataForPOAGiver.firstName = string.IsNullOrEmpty(mutation.fname_in_marathi) ? "NA" : mutation.fname_in_marathi;
        //            userDataForPOAGiver.middleName = string.IsNullOrEmpty(mutation.mname_in_marathi) ? "NA" : mutation.mname_in_marathi;
        //            userDataForPOAGiver.lastName = string.IsNullOrEmpty(mutation.lname_in_marathi) ? "NA" : mutation.lname_in_marathi;
        //            userDataForPOAGiver.suffixEng = string.IsNullOrEmpty(mutation.prefix_in_eng) ? "NA" : mutation.prefix_in_eng;
        //            userDataForPOAGiver.firstNameEng = string.IsNullOrEmpty(mutation.fname_in_eng) ? "NA" : mutation.fname_in_eng;
        //            userDataForPOAGiver.middleNameEng = string.IsNullOrEmpty(mutation.mname_in_eng) ? "NA" : mutation.mname_in_eng;
        //            userDataForPOAGiver.lastNameEng = string.IsNullOrEmpty(mutation.lname_in_eng) ? "NA" : mutation.lname_in_eng;
        //            userDataForPOAGiver.userName = string.IsNullOrEmpty(mutation.userName) ? "NA" : mutation.userName;
        //            userDataForPOAGiver.nabhu = string.IsNullOrEmpty(mutation.city_servey_no) ? "NA" : mutation.city_servey_no;
        //            userDataForPOAGiver.lrPropertyUID = string.IsNullOrEmpty(mutation.lr_property_id) ? "NA" : mutation.lr_property_id;
        //            userDataForPOAGiver.subPropNo = string.IsNullOrEmpty(mutation.mother_name_in_eng) ? "NA" : mutation.mother_name_in_eng;
        //            userDataForPOAGiver.usertype_code = mutation.user_type_code;
        //            userDataForPOAGiver.usertype = string.IsNullOrEmpty(mutation.user_type) ? "NA" : mutation.user_type;

        //            if (applicationDTL.mutation_type_code == "06")
        //            {
        //                userDataForPOAGiver.company_name_in_eng = string.IsNullOrEmpty(mutation.bank_name_in_english) ? "NA" : mutation.bank_name_in_english;
        //                userDataForPOAGiver.company_name_in_marathi = string.IsNullOrEmpty(mutation.bank_name_in_marathi) ? "NA" : mutation.bank_name_in_marathi;
        //            }
        //            else
        //            {
        //                userDataForPOAGiver.company_name_in_eng = string.IsNullOrEmpty(mutation.company_name_in_eng) ? "NA" : mutation.company_name_in_eng;
        //                userDataForPOAGiver.company_name_in_marathi = string.IsNullOrEmpty(mutation.company_name_in_marathi) ? "NA" : mutation.company_name_in_marathi;
        //            }
        //            poadata.userDetails = userDataForPOAGiver;

        //            AddressData address = new AddressData();
        //            address.addressType = mutation.address_type;

        //            MethodForFileUpload methodForFile = new MethodForFileUpload();
        //            if (mutation.address_type == "FOREIGN")
        //            {
        //                AddressForForeign addressForForeign = new AddressForForeign();
        //                addressForForeign.address = string.IsNullOrEmpty(mutation.address) ? "NA" : mutation.address;
        //                addressForForeign.mobile = string.IsNullOrEmpty(mutation.mobileno) ? "NA" : mutation.mobileno;
        //                addressForForeign.email = string.IsNullOrEmpty(mutation.emailid) ? "NA" : mutation.emailid;
        //                addressForForeign.emailOTP = string.IsNullOrEmpty(mutation.emailidverified) ? "NA" : mutation.emailidverified;
        //                if (!string.IsNullOrEmpty(mutation.signed_file_path) && mutation.signed_file_path != "NA")
        //                {
        //                    string SignedFileExt = Path.GetExtension(mutation.signed_file_path);
        //                    string Signed = methodForFile.ConvertImageToBase64(mutation.signed_file_path);
        //                    addressForForeign.signatureSrc = string.IsNullOrEmpty(Signed) ? "NA" : "data:image/" + SignedFileExt.Replace(".", "") + ";base64," + Signed;
        //                    addressForForeign.signatureName = mutation.signed_file_name;
        //                }
        //                else
        //                {
        //                    addressForForeign.signatureSrc = string.IsNullOrEmpty(mutation.signed_file_path) ? "NA" : mutation.signed_file_path;
        //                    addressForForeign.signatureName = string.IsNullOrEmpty(mutation.signed_file_name) ? "NA" : mutation.signed_file_name;
        //                }
        //                address.foreignAddress = addressForForeign;
        //            }
        //            if (mutation.address_type == "INDIA")
        //            {
        //                AddressForIndia addressForIndia = new AddressForIndia();
        //                addressForIndia.state = string.IsNullOrEmpty(mutation.state) ? "NA" : mutation.state;
        //                addressForIndia.district = string.IsNullOrEmpty(mutation.district) ? "NA" : mutation.district;
        //                addressForIndia.city = string.IsNullOrEmpty(mutation.city) ? "NA" : mutation.city;
        //                addressForIndia.taluka = string.IsNullOrEmpty(mutation.taluka) ? "NA" : mutation.taluka;
        //                addressForIndia.plotNo = string.IsNullOrEmpty(mutation.flatno_plotno) ? "NA" : mutation.flatno_plotno;
        //                addressForIndia.building = string.IsNullOrEmpty(mutation.societyname) ? "NA" : mutation.societyname;
        //                addressForIndia.mainRoad = string.IsNullOrEmpty(mutation.mainstreet) ? "NA" : mutation.mainstreet;
        //                addressForIndia.impSymbol = string.IsNullOrEmpty(mutation.landmark) ? "NA" : mutation.landmark;
        //                addressForIndia.area = string.IsNullOrEmpty(mutation.locality) ? "NA" : mutation.locality;
        //                addressForIndia.pincode = string.IsNullOrEmpty(mutation.pincode) ? "NA" : mutation.pincode;
        //                addressForIndia.postOfficeName = string.IsNullOrEmpty(mutation.post_office_name) ? "NA" : mutation.post_office_name;


        //                if (!string.IsNullOrEmpty(mutation.address_proof_document_path) && mutation.address_proof_document_path != "NA")
        //                {
        //                    addressForIndia.addressProofName = mutation.address_proof_document_name;
        //                    string AddressFileExt = Path.GetExtension(mutation.address_proof_document_path);
        //                    string AddressFile = methodForFile.ConvertImageToBase64(mutation.address_proof_document_path);
        //                    addressForIndia.addressProofSrc = string.IsNullOrEmpty(AddressFile) ? "NA" : "data:image/" + AddressFileExt.Replace(".", "") + ";base64," + AddressFile;
        //                }
        //                else
        //                {
        //                    addressForIndia.addressProofName = string.IsNullOrEmpty(mutation.address_proof_document_name) ? "NA" : mutation.address_proof_document_name;
        //                    addressForIndia.addressProofSrc = string.IsNullOrEmpty(mutation.address_proof_document_path) ? "NA" : mutation.address_proof_document_path;
        //                }

        //                if (!string.IsNullOrEmpty(mutation.signed_file_path) && mutation.signed_file_path != "NA")
        //                {
        //                    string SignedFileExt = Path.GetExtension(mutation.signed_file_path);
        //                    string Signed = methodForFile.ConvertImageToBase64(mutation.signed_file_path);
        //                    addressForIndia.signatureSrc = string.IsNullOrEmpty(Signed) ? "NA" : "data:image/" + SignedFileExt.Replace(".", "") + ";base64," + Signed;
        //                    addressForIndia.signatureName = mutation.signed_file_name;
        //                }
        //                else
        //                {
        //                    addressForIndia.signatureSrc = string.IsNullOrEmpty(mutation.signed_file_path) ? "NA" : mutation.signed_file_path;
        //                    addressForIndia.signatureName = string.IsNullOrEmpty(mutation.signed_file_name) ? "NA" : mutation.signed_file_name;
        //                }
        //                addressForIndia.mobile = string.IsNullOrEmpty(mutation.mobileno) ? "NA" : mutation.mobileno;
        //                addressForIndia.mobileOTP = string.IsNullOrEmpty(mutation.mobilenoverified) ? "NA" : mutation.mobilenoverified;
        //                address.indiaAddress = addressForIndia;
        //            }
        //            poadata.address = address;
        //        }
        //        else
        //        {
        //            poadata = null!;
        //        }
        //        return poadata;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new HandleException(ex.Message.ToString());
        //    }
        //}

        public FetchGiverDataForPOA FetchPOAGiverDataByID(int giverid)
        {
            FetchGiverDataForPOA fetchData = new FetchGiverDataForPOA();
            var giverNamesData = _context.powerOfAttorneyInformation.Include(i => i.userMaster).Include(app => app.applicationDTL).Where(data => data.power_of_attorney_id.Equals(giverid) && data.isDeleted == false).FirstOrDefault();
            if (giverNamesData != null)
            {
                string giverNameInMarathiList = string.Empty;
                string giverNameInEnglishList = string.Empty;
                if (giverNamesData.usertype_code == 1 || giverNamesData.usertype_code == 0)
                {
                    giverNameInMarathiList = commonFunctions.ReplaceNA(giverNamesData.fname_in_marathi.Trim()) + " " + commonFunctions.ReplaceNA(giverNamesData.mname_in_marathi.Trim()) + " " + commonFunctions.ReplaceNA(giverNamesData.lname_in_marathi.Trim());
                    giverNameInEnglishList = commonFunctions.ReplaceNA(giverNamesData.fname_in_eng.Trim()) + " " + commonFunctions.ReplaceNA(giverNamesData.mname_in_eng.Trim()) + " " + commonFunctions.ReplaceNA(giverNamesData.lname_in_eng.Trim());
                }
                else
                {
                    giverNameInMarathiList = giverNamesData.company_name_in_marathi.Trim();
                    giverNameInEnglishList = giverNamesData.company_name_in_eng.Trim();
                }
                fetchData.giver_names_in_marathi = giverNameInMarathiList;
                fetchData.giver_names_in_english = giverNameInEnglishList;
            }
            else
            {
                fetchData = null!;
            }
            return fetchData;
        }

        // Test Folder Access
        //public string UploadFileOnProduction(string file)
        //{
        //    try
        //    {
        //        string FolderPath = @"\\10.10.248.2\pde_propertycard\MUTATIONDOCS\Test";
        //        //string FolderPath = @"\\10.10.248.2\pde_propertycard\test";
        //        string[] AddressData = file.Split(",");
        //        TestFileUpload methodForFile = new TestFileUpload(_logger);
        //        string CurrentDateTime = DateTime.Now.ToString("yyyy-MMM-dd-HHmmss");
        //        bool checkAddressFlag = methodForFile.TestFile(AddressData[1], "Abc.jpeg", FolderPath, CurrentDateTime);
        //        return "Success";
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError("File Upload Exception - " + ex.Message!.ToString());
        //        throw new HandleException(ex.Message.ToString());
        //    }
        //}
        //public string UploadFileOnProduction1(string file)
        //{
        //    try
        //    {
        //        string FolderPath = @"\\10.10.248.2\pde_propertycard\MUTATIONDOCS";
        //        //string FolderPath = @"//10.10.248.2/pde_propertycard";
        //        //string FolderPath = @"\10.10.248.2\pde_propertycard";
        //        //  string FolderPath = @"\10.10.248.2\";
        //        //string FolderPath = @"\\10.10.248.2\pde_propertycard\test";
        //        string[] AddressData = file.Split(",");
        //        TestFileUpload methodForFile = new TestFileUpload(_logger);
        //        string CurrentDateTime = DateTime.Now.ToString("yyyy-MMM-dd-HHmmss");
        //        bool checkAddressFlag = methodForFile.TestFile(AddressData[1], "Abc.jpeg", FolderPath, CurrentDateTime);
        //        return "Success";
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError("File Upload Exception - " + ex.Message!.ToString());
        //        throw new HandleException(ex.Message.ToString());
        //    }
        //}
        //public string UploadFileOnProduction3(string file)
        //{
        //    try
        //    {
        //        string FolderPath = @"//10.10.248.2/pde_propertycard/MUTATIONDOCS";
        //        //string FolderPath = @"\\10.10.248.2\pde_propertycard\MUTATIONDOCS";
        //        //@"\10.10.248.2\MUTATIONDOCS\";
        //        //string FolderPath = @"\\10.10.248.2\pde_propertycard\test";
        //        string[] AddressData = file.Split(",");
        //        TestFileUpload methodForFile = new TestFileUpload(_logger);
        //        string CurrentDateTime = DateTime.Now.ToString("yyyy-MMM-dd-HHmmss");
        //        bool checkAddressFlag = methodForFile.TestFile(AddressData[1], "Abc.jpeg", FolderPath, CurrentDateTime);
        //        return "Success";
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError("File Upload Exception - " + ex.Message!.ToString());
        //        throw new HandleException(ex.Message.ToString());
        //    }
        //}
        //public string UploadFileOnProduction4(string file)
        //{
        //    try
        //    {
        //        string FolderPath = @"\\10.10.248.2\pde_propertycard";
        //        //string FolderPath = @"//10.10.248.2/pde_propertycard";
        //        //string FolderPath = @"\10.10.248.2\pde_propertycard";
        //        //  string FolderPath = @"\10.10.248.2\";
        //        //string FolderPath = @"\\10.10.248.2\pde_propertycard\test";
        //        string[] AddressData = file.Split(",");
        //        TestFileUpload methodForFile = new TestFileUpload(_logger);
        //        string CurrentDateTime = DateTime.Now.ToString("yyyy-MMM-dd-HHmmss");
        //        bool checkAddressFlag = methodForFile.TestFile(AddressData[1], "Abc.jpeg", FolderPath, CurrentDateTime);
        //        return "Success";
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError("File Upload Exception - " + ex.Message!.ToString());
        //        throw new HandleException(ex.Message.ToString());
        //    }
        //}
        //public string UploadFileOnProduction5(string file)
        //{
        //    try
        //    {
        //        string FolderPath = @"D:\MUTATIONDOCS\";
        //        //string FolderPath = @"\\10.10.248.2\pde_propertycard\test";
        //        string[] AddressData = file.Split(",");
        //        TestFileUpload methodForFile = new TestFileUpload(_logger);
        //        string CurrentDateTime = DateTime.Now.ToString("yyyy-MMM-dd-HHmmss");
        //        bool checkAddressFlag = methodForFile.TestFile(AddressData[1], "Abc.jpeg", FolderPath, CurrentDateTime);
        //        return "Success";
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError("File Upload Exception - " + ex.Message!.ToString());
        //        throw new HandleException(ex.Message.ToString());
        //    }
        //}

        //public string CheckFileExists(string folderpath)
        //{
        //    try
        //    {
        //        TestFileUpload methodForFile = new TestFileUpload(_logger);
        //        bool checkAddressFlag = methodForFile.CheckFileExists(folderpath);
        //        return "Success";
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError("File Upload Exception - " + ex.Message!.ToString());
        //        throw new HandleException(ex.Message.ToString());
        //    }
        //}

        public string SaveTrutiDocUploadedData(AddTrutiUploadedDocumentsDTLData addUploadedDocuments)
        {
            using (var scope = new TransactionScope())
            {
                try
                {
                    MethodForFileUpload methodForFile = new MethodForFileUpload();
                    string DocUploadStatus = string.Empty;
                    string FolderPath = @"\\10.10.248.2\pde_propertycard\MUTATIONDOCS\" + addUploadedDocuments.applicationid + @"\TRUTIDOCUMENTS\";
                    // Assign Values to Model
                    UploadedDocumentsDTLModel uploadedDocumentsDTLModel = new UploadedDocumentsDTLModel();
                    UserMaster userMaster = _context.userMasters.FirstOrDefault(s => s.userid == addUploadedDocuments.userid)!;
                    uploadedDocumentsDTLModel.userMaster = userMaster;

                    ApplicationDTL applicationDTL = _context.applicationDTL.FirstOrDefault(s => s.applicationid == addUploadedDocuments.applicationid && s.inwardno == addUploadedDocuments.inwardNo)!;
                    if (applicationDTL != null)
                    {
                        uploadedDocumentsDTLModel.applicationDTL = applicationDTL;

                        uploadedDocumentsDTLModel.documentTypeCode = addUploadedDocuments.docType!.document_code;
                        uploadedDocumentsDTLModel.documentType = addUploadedDocuments.docType.document_name;

                        uploadedDocumentsDTLModel.document_name = addUploadedDocuments.docUpload!.docName;
                        uploadedDocumentsDTLModel.document_path = FolderPath + uploadedDocumentsDTLModel.document_name;

                        //Assign Data to Table fields to insert new records
                        UploadedDocumentsDTL dbTable = new UploadedDocumentsDTL();
                        dbTable.userMaster = uploadedDocumentsDTLModel.userMaster;
                        dbTable.applicationDTL = uploadedDocumentsDTLModel.applicationDTL;
                        dbTable.document_type_code = uploadedDocumentsDTLModel.documentTypeCode;
                        dbTable.document_type = uploadedDocumentsDTLModel.documentType;
                        dbTable.city_servey_no = uploadedDocumentsDTLModel.city_servey_no;
                        dbTable.document_name = uploadedDocumentsDTLModel.document_name;
                        dbTable.document_path = uploadedDocumentsDTLModel.document_path;
                        dbTable.truti_patra_flag = "TRUE";
                        _context.uploadedDocumentsDTLs.Add(dbTable);
                        DocUploadStatus = methodForFile.ConvertBase64ToPdf(FolderPath, addUploadedDocuments.docUpload.docName!, addUploadedDocuments.docUpload.docSrc!);//methodForFile.UploadPDFFile(FolderPath, addUploadedDocuments.docUpload);
                        if (DocUploadStatus == "Success")
                        {
                            _context.SaveChanges();
                            //Get Saved Row ID
                            int uploadedDocID = dbTable.uploaded_doc_id;
                            var applicationDTLdata = _context.applicationDTL.Where(data => data.applicationid!.Equals(addUploadedDocuments.applicationid)).FirstOrDefault();
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
                                _context.Entry(applicationDTLdata).CurrentValues.SetValues(applicationDTLdata);
                                _context.SaveChanges();
                            }
                            scope.Complete();
                            return "Success";
                        }
                        else
                        {
                            return DocUploadStatus;
                        }
                    }
                    else
                    {
                        return "Application Data Not Found";
                    }
                }
                catch (Exception ex)
                {
                    throw new HandleException(ex.Message.ToString());
                }
            }
        }

        public List<FetchTrutiUploadedDocumentsData> FetchTrutiUploadedDocumentIDs(string applicationid)
        {
            List<string> fetchfileName = new List<string>();
            List<InputDataModel.docUpload> docDataList = new List<InputDataModel.docUpload>();
            List<string> fetchfilePaths = new List<string>();
            List<FetchTrutiUploadedDocumentsData> fetchDataList = new List<FetchTrutiUploadedDocumentsData>();
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
                        fetchfileName.Add(Path.GetFileName(file));
                        docDataList.Add(docData);
                    }

                    foreach (InputDataModel.docUpload docData in docDataList)
                    {
                        UploadedDocumentsDTL uploadedDocumentsDTL = new UploadedDocumentsDTL();
                        uploadedDocumentsDTL = _context.uploadedDocumentsDTLs.Include(i => i.userMaster).Include(app => app.applicationDTL).Where(data => data.applicationDTL!.applicationid!.Equals(applicationid) && data.isDeleted == false
                        && data.document_name == docData.docName && data.document_path == docData.docSrc
                           && data.truti_patra_flag == "TRUE").FirstOrDefault()!;
                        if (uploadedDocumentsDTL != null)
                        {
                            FetchTrutiUploadedDocumentsData fetchData = new FetchTrutiUploadedDocumentsData();
                            fetchData.applicationID = uploadedDocumentsDTL.applicationDTL!.applicationid;
                            fetchData.documentID = uploadedDocumentsDTL.uploaded_doc_id.ToString();
                            fetchData.documentTypeCode = Convert.ToInt32(uploadedDocumentsDTL.document_type_code);
                            fetchData.documentType = uploadedDocumentsDTL.document_type;

                            MutationCTSNoDTL mutationCTSNoDTL = new MutationCTSNoDTL();
                            mutationCTSNoDTL = _context.mutationCTSNoDTLs.Where(data => data.applicationDTL!.applicationid!.Equals(uploadedDocumentsDTL.applicationDTL.applicationid)).FirstOrDefault()!;
                            //fetchData.city = mutationCTSNoDTL.village_or_peth_name;
                            fetchData.nabhuno = uploadedDocumentsDTL.city_servey_no == null || uploadedDocumentsDTL.city_servey_no == "" ? "NA" : uploadedDocumentsDTL.city_servey_no;
                            fetchData.docName = uploadedDocumentsDTL.document_name;

                            //File
                            Byte[] fileBytes = File.ReadAllBytes(uploadedDocumentsDTL.document_path!);
                            string FileExt = Path.GetExtension(uploadedDocumentsDTL.document_path)!;
                            var content = Convert.ToBase64String(fileBytes);
                            fetchData.docSrc = string.IsNullOrEmpty(content) ? "NA" : "data:pdf/" + FileExt.Replace(".", "") + ";base64," + content;
                            fetchDataList.Add(fetchData);
                        }
                    }
                }
                else
                {
                    fetchDataList = null!;
                }
                return fetchDataList;
            }
            catch (Exception ex)
            {
                throw new HandleException(ex.Message.ToString());
            }
        }

        public string DeleteTrutiDocUploadedData(DeleteTrutiPatraUploadedDocumentsDTLData deletdata)
        {
            MethodForFileUpload methodForFile = new MethodForFileUpload();
            using (var scope = new TransactionScope())
            {
                try
                {
                    var entity = _context.uploadedDocumentsDTLs.Include(app => app.applicationDTL).Where(data => data.uploaded_doc_id.Equals(deletdata.uploaded_doc_id) && data.isDeleted == false).FirstOrDefault();
                    if (entity != null)
                    {
                        var applicationDTLdata = _context.applicationDTL.Where(data => data.applicationid!.Equals(deletdata.applicationid)).FirstOrDefault();
                        if (applicationDTLdata != null)
                        {
                            string getIDS = applicationDTLdata.uploadedDocIDs!;
                            string[] IdsArray = getIDS.Split(',');
                            var updatedIds = IdsArray.Where(id => id != deletdata.uploaded_doc_id.ToString());

                            // Join the remaining IDs back into a string
                            string result = string.Join(",", updatedIds);
                            applicationDTLdata.uploadedDocIDs = result;
                            _context.Entry(applicationDTLdata).CurrentValues.SetValues(applicationDTLdata);
                            _context.SaveChanges();
                        }
                        methodForFile.DeleteFile(entity.document_path!);
                        //entity.isDeleted = true;
                        //entity.deleteddate = DateOnly.FromDateTime(DateTime.Now);
                        _context.uploadedDocumentsDTLs.Remove(entity);
                        _context.SaveChanges();
                        scope.Complete();
                        return "Success";
                    }
                    else
                    {
                        return "Data Not Found";
                    }
                }
                catch (Exception ex)
                {
                    throw new HandleException(ex.Message.ToString());
                }
            }
        }

        public string AddSlashInwardNo(string Inwardno)
        {
            string result = string.Empty;
            //= Regex.Replace(numberStr, ".{4}", "$0/");
            for (int i = 0; i < 2 && i * 4 < Inwardno.Length; i++)
            {
                result += Inwardno.Substring(i * 4, 4) + "/";
            }
            // Add the remaining part
            int remainingIndex = 2 * 4;
            if (remainingIndex < Inwardno.Length)
            {
                result += Inwardno.Substring(remainingIndex);
            }
            else
            {
                // Remove trailing slash if no remaining digits
                result = result.TrimEnd('/');
            }
            return result;
        }

        public void SaveApplicationDataSubmittedHistory(string applicationid, string application_submitted_form_name, string application_submitted_type)
        {
            //using (var scope = new TransactionScope())
            //{
            try
            {
                ApplicationDTL applicationDTL = _context.applicationDTL.FirstOrDefault(s => s.applicationid == applicationid && s.isDeleted == false)!;
                if (applicationDTL != null)
                {
                    ApplicationDataSubmittedHistory applicationDataSubmittedHistory = new ApplicationDataSubmittedHistory();
                    applicationDataSubmittedHistory.applicationid = applicationid;
                    applicationDataSubmittedHistory.application_submitted_form_name = application_submitted_form_name;
                    applicationDataSubmittedHistory.mutation_type_code = applicationDTL.mutation_type_code;
                    applicationDataSubmittedHistory.mutation_type_name = applicationDTL.mutation_type_name;
                    applicationDataSubmittedHistory.application_submitted_type = application_submitted_type.ToUpper();
                    _context.applicationDataSubmittedHistories.Add(applicationDataSubmittedHistory);
                    _context.SaveChanges();
                }
                //scope.Complete();
                //dbContextTransaction.Dispose();
            }
            catch (Exception ex)
            {
                // dbContextTransaction.Rollback();
                //dbContextTransaction.Dispose();
                throw new HandleException(ex.Message.ToString());
            }
            //}
        }

        public void SaveApplicationDataSubmittedHistoryForBhadepatta(string applicationid, string application_submitted_form_name, string application_submitted_type)
        {
            using (var dbContextTransaction = _context.Database.BeginTransaction())
            {
                try
                {
                    ApplicationDTL applicationDTL = _context.applicationDTL.FirstOrDefault(s => s.applicationid == applicationid && s.isDeleted == false)!;
                    if (applicationDTL != null)
                    {
                        ApplicationDataSubmittedHistory applicationDataSubmittedHistory = new ApplicationDataSubmittedHistory();
                        applicationDataSubmittedHistory.applicationid = applicationid;
                        applicationDataSubmittedHistory.application_submitted_form_name = application_submitted_form_name;
                        applicationDataSubmittedHistory.mutation_type_code = applicationDTL.mutation_type_code;
                        applicationDataSubmittedHistory.mutation_type_name = applicationDTL.mutation_type_name;
                        applicationDataSubmittedHistory.application_submitted_type = application_submitted_type.ToUpper();
                        _context.applicationDataSubmittedHistories.Add(applicationDataSubmittedHistory);
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
            //using (var scope = new TransactionScope( TransactionScopeAsyncFlowOption.Enabled))
            //using var transaction = await _context.Database.BeginTransactionAsync();
            //{
            //    try
            //    {
            //        ApplicationDTL applicationDTL = _context.applicationDTL.FirstOrDefault(s => s.applicationid == applicationid && s.isDeleted == false)!;
            //        if (applicationDTL != null)
            //        {
            //            ApplicationDataSubmittedHistory applicationDataSubmittedHistory = new ApplicationDataSubmittedHistory();
            //            applicationDataSubmittedHistory.applicationid = applicationid;
            //            applicationDataSubmittedHistory.application_submitted_form_name = application_submitted_form_name;
            //            applicationDataSubmittedHistory.mutation_type_code = applicationDTL.mutation_type_code;
            //            applicationDataSubmittedHistory.mutation_type_name = applicationDTL.mutation_type_name;
            //            applicationDataSubmittedHistory.application_submitted_type = application_submitted_type.ToUpper();
            //            _context.applicationDataSubmittedHistories.Add(applicationDataSubmittedHistory);
            //            //_context.SaveChanges();
            //            await _context.SaveChangesAsync();
            //        }
            //        await transaction.CommitAsync();
            //        transaction.Dispose();
            //        //scope.Complete();
            //    }
            //    catch (Exception ex)
            //    {
            //        await transaction.RollbackAsync();
            //        throw new HandleException(ex.Message.ToString());
            //    }
            //}
        }

    }
}
