using Humanizer;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PDEWebAPIS.CommonMethods;
using PDEWebAPIS.Data;
using PDEWebAPIS.Helpers;
using PDEWebAPIS.InputDataModel;
using PDEWebAPIS.Model;
using PDEWebAPIS.Repository;
using PDEWebAPIS.ViewModel;
using System.Data;
using System.Globalization;
using System.Text;
using System.Transactions;
using static System.Net.WebRequestMethods;

namespace PDEWebAPIS.Services
{
    public class UserServices
    {
        //private AppDBContext _context;
        //public UserServices(AppDBContext context)
        //{
        //    _context = context;
        //}

        private AppDBContext _context;
        private AppDBContextR _contextR;
        private readonly ILogger<UserServices> _logger;
        private readonly SMSService sMSService;
        public UserServices(AppDBContext context, AppDBContextR contextR, ILogger<UserServices> logger)
        {
            _context = context;
            _contextR = contextR;
            _logger = logger;
            sMSService = new SMSService(context);
        }

        //public string GetUserID(string description, string VerificationType)
        //{
        //    int FetchedUserID = 0;
        //    string StatusCode = "1";
        //    ReponseType type = ReponseType.Success;
        //    try
        //    {
        //        if (VerificationType.Trim().ToUpper() == "MOBILENO")
        //        {
        //            var row = _context.userMasters.Where(data => data.mobileno.Equals(description)).FirstOrDefault();
        //            if (row != null)
        //            {
        //                FetchedUserID = row.userid;
        //            }
        //            else
        //            {
        //                FetchedUserID = 0;
        //            }
        //        }
        //        else if (VerificationType.Trim().ToUpper() == "EMAILID")
        //        {
        //            var row = _context.userMasters.Where(data => data.emailid.Equals(description.Trim())).FirstOrDefault();
        //            //var row1 = _context.userMasters.Where(data => data.mobileno.Equals(MobileNo)).First();
        //            if (row != null)
        //            {
        //                FetchedUserID = row.userid;
        //            }
        //            else
        //            {
        //                FetchedUserID = 0;
        //            }
        //        }
        //        if (FetchedUserID == 0)
        //        {
        //            StatusCode = "0";
        //        }
        //        return StatusCode;

        //    }
        //    catch (Exception ex)
        //    {
        //        return ex.Message.ToString();
        //    }
        //}
        public int FetchUserID(string description, string VerificationType)
        {
            try
            {
                int FetchedUserID = 0;
                if (VerificationType.Trim().ToUpper() == "MOBILENO")
                {
                    var row = _contextR.userMasters.Where(data => data.mobileno.Equals(description) && data.address_type == "INDIA" && data.mobilenoverified == "YES").FirstOrDefault();
                    if (row != null)
                    {
                        FetchedUserID = row.userid;
                    }
                    else
                    {
                        FetchedUserID = 0;
                    }
                }
                else if (VerificationType.Trim().ToUpper() == "EMAILID")
                {
                    var row = _contextR.userMasters.Where(data => data.emailid.Equals(description.Trim()) && (data.address_type == "FOREIGN" || data.address_type == "INDIA") && data.emailidverified == "YES").FirstOrDefault();
                    //var row1 = _context.userMasters.Where(data => data.mobileno.Equals(MobileNo)).First();
                    if (row != null)
                    {
                        FetchedUserID = row.userid;
                    }
                    else
                    {
                        FetchedUserID = 0;
                    }
                }
                return FetchedUserID;
            }
            catch (Exception ex)
            {
                throw new HandleException(ex.Message.ToString());
            }
        }

        public int FetchUserIDThroughToken(string Token, string Flag)
        {
            try
            {
                UserMaster userData = new UserMaster();
                int FetchedUserID = 0;

                if (Flag.Trim().ToUpper() == "WEB")
                {
                    userData = _contextR.userMasters.Where(data => data.web_token.Equals(Token)).FirstOrDefault()!;
                }
                if (Flag.Trim().ToUpper() == "MOBILE")
                {
                    userData = _contextR.userMasters.Where(data => data.mobile_token.Equals(Token)).FirstOrDefault()!;
                }

                if (userData != null)
                {
                    FetchedUserID = userData.userid;
                }
                else
                {
                    userData = _contextR.userMasters.Where(data => data.mobileno.Equals("8788206624")).FirstOrDefault()!;
                    _logger.LogInformation("User Data Found - " + userData.mobileno + " Token -" + userData.web_token);
                    throw new HandleException("User Not Found");
                }
                return FetchedUserID;
            }
            catch (Exception ex)
            {
                throw new HandleException(ex.Message.ToString());
            }
        }

        public int FetchUserIDThroughTokenForLogout(string Token, string Flag)
        {
            try
            {
                UserMaster userData = new UserMaster();
                int FetchedUserID = 0;

                if (Flag.Trim().ToUpper() == "WEB")
                {
                    userData = _context.userMasters.Where(data => data.web_token.Equals(Token)).FirstOrDefault()!;
                }
                if (Flag.Trim().ToUpper() == "MOBILE")
                {
                    userData = _context.userMasters.Where(data => data.mobile_token.Equals(Token)).FirstOrDefault()!;
                }
                if (userData != null)
                {
                    FetchedUserID = userData.userid;
                }
                //else
                //{
                //    throw new HandleException("User Not Found");
                //}
                return FetchedUserID;
            }
            catch (Exception ex)
            {
                throw new HandleException(ex.Message.ToString());
            }
        }
        public string CheckOTP(VerifyOTPData VerifyOTPData)
        {
            string Status = "0";
            try
            {
                _logger.LogInformation("CheckOTP Request Data - " + VerifyOTPData.DESCRIPTION);
                _logger.LogInformation("CheckOTP Request Data - " + VerifyOTPData.VERIFICATIONTYPE);
                _logger.LogInformation("CheckOTP Request Data - " + VerifyOTPData.OTP);
                var row = _contextR.mailid_And_Mobileno_Verifications.Where(w => w.description == VerifyOTPData.DESCRIPTION
           && w.verificationtype == VerifyOTPData.VERIFICATIONTYPE!.Trim().ToUpper()
           && w.otp == VerifyOTPData.OTP).FirstOrDefault();
                if (row != null)
                {
                    _context.mailid_And_Mobileno_Verifications.Where(w => w.description == VerifyOTPData.DESCRIPTION
        && w.verificationtype == VerifyOTPData.VERIFICATIONTYPE!.Trim().ToUpper()).ExecuteDelete();
                    _context.SaveChanges();
                    Status = "1";
                }
                return Status;
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
        }

        //async Task<string>
        public string SaveVerificationData(Mailid_And_Mobileno_VerificationModel Mailid_And_Mobileno_VerificationModel)
        {
            string SMSresponse = "Success";
            try
            {
                Mailid_And_Mobileno_Verification dbTable = new Mailid_And_Mobileno_Verification();
                dbTable = _context.mailid_And_Mobileno_Verifications.Where(d => d.description.Equals(Mailid_And_Mobileno_VerificationModel.description)).FirstOrDefault()!;
                if (dbTable != null)
                {
                    _context.Remove(dbTable);
                    _context.SaveChanges();
                    dbTable.verificationtype = Mailid_And_Mobileno_VerificationModel.verificationtype.Trim().ToUpper();
                    dbTable.otp = Mailid_And_Mobileno_VerificationModel.otp;
                    dbTable.description = Mailid_And_Mobileno_VerificationModel.description;
                    _context.mailid_And_Mobileno_Verifications.Add(dbTable);
                    int affectedRows = _context.SaveChanges();
                    if (affectedRows > 0)
                    {
                        _logger.LogInformation("OTP Data Saved Successfully");
                    }
                    else
                    {
                        _logger.LogInformation("OTP Data Not Saved Successfully");
                    }
                    _logger.LogInformation("OTP Saved Successfully");
                    _logger.LogInformation("OTP Saved Successfully" + dbTable.otp + " " + dbTable.description);
                }
                else
                {
                    Mailid_And_Mobileno_Verification dbTable1 = new Mailid_And_Mobileno_Verification();
                    dbTable1.verificationtype = Mailid_And_Mobileno_VerificationModel.verificationtype.Trim().ToUpper();
                    dbTable1.otp = Mailid_And_Mobileno_VerificationModel.otp;
                    dbTable1.description = Mailid_And_Mobileno_VerificationModel.description;
                    _context.mailid_And_Mobileno_Verifications.Add(dbTable1);
                    int affectedRows = _context.SaveChanges();
                    if (affectedRows > 0)
                    {
                        _logger.LogInformation("OTP1 Data Saved Successfully");
                    }
                    else
                    {
                        _logger.LogInformation("OTP1 Data Not Saved Successfully");
                    }
                    _logger.LogInformation("OTP Saved Successfully" + dbTable1.otp + " " + dbTable1.description);
                }
                if (Mailid_And_Mobileno_VerificationModel.verificationtype == "MOBILENO")
                {
                    //SMSresponse = await sMSService.SendSMSRequest(HttpMethod.Get, _logger, Mailid_And_Mobileno_VerificationModel.description, Mailid_And_Mobileno_VerificationModel.otp.ToString());
                    SMSresponse = sMSService.sendOTPMSGUsingCDAC(Mailid_And_Mobileno_VerificationModel.description, Mailid_And_Mobileno_VerificationModel.otp.ToString());
                }
                return SMSresponse;
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
        }

        public string GenerateOTP()
        {
            //Random generator = new Random();
            //string OTP = generator.Next(0, 1000000).ToString("D6");
            Random random = new Random();
            int number = random.Next(100000, 1000000);
            return number.ToString();
        }

        public string SaveUserData(CreateUserData userData)
        {
            MethodForFileUpload methodForFile = new MethodForFileUpload();
            string FolderPath = @"\\10.10.248.2\pde_propertycard\USERDOCS\";
            //@"D:\WWW\USERDOCS\";
            try
            {

                if (userData.address!.addressType!.Trim().ToUpper() == "INDIA")
                {
                    if (string.IsNullOrEmpty(userData.address!.indiaAddress!.mobile))
                    {
                        return "Please Enter Mobile No";
                    }
                    if (methodForFile.CheckMobNo(userData.address!.indiaAddress!.mobile))
                    {
                        return "Please enter valid mobile number.";
                    }

                    var CheckMobileNoData = _context.userMasters.Where(w => w.mobileno == userData.address.indiaAddress.mobile).FirstOrDefault();
                    //var CheckEmailData = _context.userMasters.Where(w => w.emailid == userData.address.indiaAddress.email).FirstOrDefault();
                    if (CheckMobileNoData != null)
                    {
                        return "User Is Already Registered For The Entered Mobile No";
                    }
                    //if (CheckEmailData != null)
                    //{
                    //    return "User Is Already Registered For The Entered Email ID";
                    //}
                }
                if (userData.address.addressType.Trim().ToUpper() == "FOREIGN")
                {
                    if (string.IsNullOrEmpty(userData.address.foreignAddress!.email))
                    {
                        return "Please Enter Email ID";
                    }
                    if (methodForFile.CheckEmail(userData.address!.indiaAddress!.email))
                    {
                        return "Please enter valid Email Id.";
                    }
                    //var CheckMobileNoData = _context.userMasters.Where(w => w.mobileno == userData.address.foreignAddress.mobile).FirstOrDefault();
                    var CheckEmailData = _context.userMasters.Where(w => w.emailid == userData.address.foreignAddress.email).FirstOrDefault();
                    //if (CheckMobileNoData != null)
                    //{
                    //    return "User Is Already Registered For The Entered Mobile No";
                    //}
                    if (CheckEmailData != null)
                    {
                        return "User Is Already Registered For The Entered Email ID";
                    }
                }
                // Assign values to model
                UserMasterModel userMasterModel = new UserMasterModel();
                userMasterModel.usertype = userData.usertype!.Trim().ToUpper();
                userMasterModel.usertype_code = userData.usertype_code;
                userMasterModel.profile_pic_file_name = userData.photo!.passportName!;

                userMasterModel.owner_of_property_in_maharashtra = (userData.isMHProperty!.hasProperty!.Trim().ToUpper() == "YES") ? true : false;
                PropertyTypeMaster proptype = _context.propertyTypes.FirstOrDefault(s => s.propertytypeid == Convert.ToInt32(userData.isMHProperty.propType))!;
                userMasterModel.PropertyTypeMaster = proptype;
                if (userMasterModel.owner_of_property_in_maharashtra)
                {
                    if (userMasterModel.PropertyTypeMaster.propertytypeid == 1 && string.IsNullOrEmpty(userData.isMHProperty.userDetails!.khataNo))
                    {
                        return "When Property Type Is 7/12 Then Khate No Should Not Be Empty";
                    }
                    if (userMasterModel.PropertyTypeMaster.propertytypeid == 2 && string.IsNullOrEmpty(userData.isMHProperty.userDetails!.naBhu))
                    {
                        return "When Property Type Is Property Card Then City Servey No Should Not Be Empty";
                    }
                    if (userMasterModel.PropertyTypeMaster.propertytypeid == 3 && string.IsNullOrEmpty(userData.isMHProperty.userDetails!.ulpin))
                    {
                        return "When Property Type Is ULPIN Then ULPIN Should Not Be Empty";
                    }
                    userMasterModel.khateno = string.IsNullOrEmpty(userData.isMHProperty.userDetails!.khataNo) ? "NA" : userData.isMHProperty.userDetails.khataNo;
                    userMasterModel.city_servey_no = string.IsNullOrEmpty(userData.isMHProperty.userDetails.naBhu) ? "NA" : userData.isMHProperty.userDetails.naBhu;
                    userMasterModel.ulpin = string.IsNullOrEmpty(userData.isMHProperty.userDetails.ulpin) ? "NA" : userData.isMHProperty.userDetails.ulpin;
                    if (userMasterModel.PropertyTypeMaster.propertytypeid == 1)
                    {
                        userMasterModel.city_servey_no = "NA";
                        userMasterModel.ulpin = "NA";
                    }
                    else if (userMasterModel.PropertyTypeMaster.propertytypeid == 2)
                    {
                        userMasterModel.khateno = "NA";
                        userMasterModel.ulpin = "NA";
                    }
                    else if (userMasterModel.PropertyTypeMaster.propertytypeid == 3)
                    {
                        userMasterModel.khateno = "NA";
                        userMasterModel.city_servey_no = "NA";
                    }
                    userMasterModel.property_district_code = userData.isMHProperty.userDetails.district!.district_code!;
                    userMasterModel.property_district_name = userData.isMHProperty.userDetails.district!.district_name!;

                    userMasterModel.property_taluka_code = userData.isMHProperty.userDetails.taluka!.office_code!;
                    userMasterModel.property_taluka_name = userData.isMHProperty.userDetails.taluka!.office_name!;

                    userMasterModel.property_village_code = userData.isMHProperty.userDetails.village!.village_code!;
                    userMasterModel.property_village_name = userData.isMHProperty.userDetails.village!.village_name!;
                }
                else
                {
                    userMasterModel.khateno = "NA";
                    userMasterModel.city_servey_no = "NA";
                    userMasterModel.ulpin = "NA";
                    userMasterModel.city = "NA";
                    userMasterModel.property_district_code = "NA";
                    userMasterModel.property_district_name = "NA";

                    userMasterModel.property_taluka_code = "NA";
                    userMasterModel.property_taluka_name = "NA";

                    userMasterModel.property_village_code = "NA";
                    userMasterModel.property_village_name = "NA";
                }
                //if (string.IsNullOrEmpty(userData.isMHProperty.userDetails.userName))
                //{
                //    throw new HandleException("User Name Should Not Be Empty");
                //}

                userMasterModel.username = userData.isMHProperty.userDetails!.userName!;
                if (userData.usertype_code == 1)
                {
                    if (methodForFile.ContainsSpecialCharactersInMarathiName(userData.isMHProperty!.userDetails!.firstName!)
                    || methodForFile.ContainsSpecialCharactersInMarathiName(userData.isMHProperty!.userDetails!.middleName!) ||
                    methodForFile.ContainsSpecialCharactersInMarathiName(userData.isMHProperty!.userDetails!.lastName!))
                    {
                        return "वापरकर्त्याचे नाव (मराठी मध्ये) Feild contains English Letters / special characters!";
                    }
                    if (methodForFile.ContainsSpecialCharactersInName(userData.isMHProperty!.userDetails!.firstNameEng!)
                        || methodForFile.ContainsSpecialCharactersInName(userData.isMHProperty!.userDetails!.middleNameEng!) ||
                        methodForFile.ContainsSpecialCharactersInName(userData.isMHProperty!.userDetails!.lastNameEng!))
                    {
                        return "वापरकर्त्याचे नाव (इंग्रजी मध्ये) Feild contains Marathi letters /  special characters!";
                    }
                    userMasterModel.prefixcode_marathi = userData.isMHProperty.userDetails!.suffixcode!;
                    userMasterModel.prefix_in_marathi = userData.isMHProperty.userDetails.suffix!;
                    userMasterModel.fname_in_marathi = userData.isMHProperty.userDetails.firstName!;
                    userMasterModel.mname_in_marathi = userData.isMHProperty.userDetails.middleName!;
                    userMasterModel.lname_in_marathi = userData.isMHProperty.userDetails.lastName!;
                    userMasterModel.prefixcode_eng = userData.isMHProperty.userDetails!.suffixCodeEng!;
                    userMasterModel.prefix_in_eng = userData.isMHProperty.userDetails.suffixEng!;
                    userMasterModel.fname_in_eng = userData.isMHProperty.userDetails.firstNameEng!;
                    userMasterModel.mname_in_eng = userData.isMHProperty.userDetails.middleNameEng!;
                    userMasterModel.lname_in_eng = userData.isMHProperty.userDetails.lastNameEng!;

                    userMasterModel.company_name_in_marathi = "NA";
                    userMasterModel.company_name_in_eng = "NA";
                }
                //if (userData.usertype.Trim().ToUpper() == "COMPANY")
                else
                {
                    if (methodForFile.ContainsSpecialCharactersInMarathiName(userData.isMHProperty!.userDetails!.companyName!))
                    {
                        return "वापरकर्त्याचे नाव (मराठी मध्ये) Feild contains English Letters / special characters!";
                    }

                    if (methodForFile.ContainsSpecialCharactersInName(userData.isMHProperty!.userDetails!.companyNameEng!))
                    {
                        return "वापरकर्त्याचे नाव (इंग्रजी मध्ये) Feild contains Marathi letters / special characters!";
                    }
                    userMasterModel.company_name_in_marathi = userData.isMHProperty.userDetails.companyName!;
                    userMasterModel.company_name_in_eng = userData.isMHProperty.userDetails.companyNameEng!;
                    userMasterModel.prefixcode_eng = "NA";
                    userMasterModel.prefixcode_marathi = "NA";
                    userMasterModel.prefix_in_marathi = "NA";
                    userMasterModel.fname_in_marathi = "NA";
                    userMasterModel.mname_in_marathi = "NA";
                    userMasterModel.lname_in_marathi = "NA";
                    userMasterModel.prefix_in_eng = "NA";
                    userMasterModel.fname_in_eng = "NA";
                    userMasterModel.mname_in_eng = "NA";
                    userMasterModel.lname_in_eng = "NA";
                }
                userMasterModel.address_type = userData.address.addressType.Trim().ToUpper();
                if (userMasterModel.address_type == "INDIA")
                {
                    if (!string.IsNullOrEmpty(userData.address.indiaAddress!.plotNo!) && methodForFile.CheckIndianAddress(userData.address.indiaAddress!.plotNo!))
                    {
                        return "सदनिका / घर / प्लॉट नं. field contains special characters / length is more than 100 characters.";
                    }
                    if (!string.IsNullOrEmpty(userData.address.indiaAddress!.building!) && methodForFile.CheckIndianAddress(userData.address.indiaAddress.building!))
                    {
                        return "इमारत / सोसायटी क्रमांक किंवा नाव field contains special characters. / Length is more that 100 characters";
                    }
                    if (!string.IsNullOrEmpty(userData.address.indiaAddress!.mainRoad!) && methodForFile.CheckIndianAddress(userData.address.indiaAddress.mainRoad!))
                    {
                        return "मुख्य रस्ता field contains special characters / length is more than 100 characters.";
                    }
                    if (!string.IsNullOrEmpty(userData.address.indiaAddress!.impSymbol!) && methodForFile.CheckIndianAddress(userData.address.indiaAddress.impSymbol!))
                    {
                        return "महत्त्वाची खूण field contains special characters/ length is more than 100 characters.";
                    }
                    if (!string.IsNullOrEmpty(userData.address.indiaAddress!.area!) && methodForFile.CheckIndianAddress(userData.address.indiaAddress.area!))
                    {
                        return "परिसर / गावाचे नाव / वाडी field contains special characters/ length is more than 100 characters.";
                    }
                    if (!string.IsNullOrEmpty(userData.address.indiaAddress!.pincode!) && methodForFile.CheckPinCode(userData.address.indiaAddress.pincode!))
                    {
                        if (string.IsNullOrEmpty(userData.address.indiaAddress.postOfficeName!))
                        {
                            return "Please select Post Office Name / Enter correct Pin Code.";
                        }
                        return "पिन कोड field contains special characters/ length is more than 6 numbers.";
                    }
                    userMasterModel.state = userData.address.indiaAddress!.state!;
                    userMasterModel.district = userData.address.indiaAddress.district!;
                    userMasterModel.city = userData.address.indiaAddress.city!;
                    userMasterModel.taluka = userData.address.indiaAddress.taluka!;
                    userMasterModel.flatno_plotno = userData.address.indiaAddress.plotNo!;
                    userMasterModel.societyname = userData.address.indiaAddress.building!;
                    userMasterModel.mainstreet = userData.address.indiaAddress.mainRoad!;
                    userMasterModel.landmark = userData.address.indiaAddress.impSymbol!;
                    userMasterModel.locality = userData.address.indiaAddress.area!;
                    userMasterModel.pincode = userData.address.indiaAddress.pincode!;
                    userMasterModel.postofficename = userData.address.indiaAddress.postOfficeName!;
                    userMasterModel.address_proof_document_name = userData.address.indiaAddress.addressProofName!;
                    //userMasterModel.address_proof_document_path = AddressDocFolderPath + userData.address.indiaAddress.addressProofName;
                    userMasterModel.mobileno = userData.address.indiaAddress.mobile!;
                    //GW
                    if (userData.address.indiaAddress.mobileOTP!.Trim().ToUpper() == "NO")
                    {
                        return "Please verify OTP first.";
                    }
                    userMasterModel.mobilenoverified = userData.address.indiaAddress.mobileOTP!.Trim().ToUpper();
                    userMasterModel.emailid = string.IsNullOrEmpty(userData.address.indiaAddress.email) ? "NA" : userData.address.indiaAddress.email;
                    userMasterModel.emailidverified = string.IsNullOrEmpty(userData.address.indiaAddress.emailOTP.Trim().ToUpper()) ? "NO" : userData.address.indiaAddress.emailOTP.Trim().ToUpper();
                    userMasterModel.securitypin = userData.address.indiaAddress.securityKey;
                    userMasterModel.signed_file_name = userData.address.indiaAddress.signatureName!;
                    userMasterModel.address = "NA";
                }
                else if (userMasterModel.address_type == "FOREIGN")
                {
                    if (!string.IsNullOrEmpty(userData.address.indiaAddress!.plotNo!) && methodForFile.CheckForeignAddress(userData.address.foreignAddress!.address!))
                    {
                        return "पत्ता field contains special characters / length is more than 100 characters.";
                    }
                    userMasterModel.address = userData.address.foreignAddress!.address!;
                    userMasterModel.mobileno = string.IsNullOrEmpty(userData.address.foreignAddress.mobile) ? "NA" : userData.address.foreignAddress.mobile;
                    userMasterModel.mobilenoverified = "NO";
                    userMasterModel.address_proof_document_name = "NA";
                    userMasterModel.address_proof_document_path = "NA";
                    userMasterModel.emailid = userData.address.foreignAddress.email!;
                    userMasterModel.emailidverified = userData.address.foreignAddress.emailOTP!.Trim().ToUpper();
                    userMasterModel.signed_file_name = userData.address.foreignAddress.signatureName!;

                    userMasterModel.securitypin = "NA";
                    userMasterModel.state = "NA";
                    userMasterModel.district = "NA";
                    userMasterModel.city = "NA";
                    userMasterModel.taluka = "NA";
                    userMasterModel.flatno_plotno = "NA";
                    userMasterModel.societyname = "NA";
                    userMasterModel.mainstreet = "NA";
                    userMasterModel.landmark = "NA";
                    userMasterModel.locality = "NA";
                    userMasterModel.pincode = "NA";
                    userMasterModel.postofficename = "NA";
                }
                TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;

                //Assign Data to Table fields to insert new records
                UserMaster dbTable = new UserMaster();
                dbTable.usertype_code = userMasterModel.usertype_code;
                dbTable.usertype = userMasterModel.usertype;
                dbTable.mobileno = userMasterModel.mobileno;
                dbTable.mobilenoverified = userMasterModel.mobilenoverified;
                dbTable.emailid = userMasterModel.emailid;
                dbTable.emailidverified = userMasterModel.emailidverified;
                dbTable.securitypin = userMasterModel.securitypin;
                dbTable.prefixcode_eng = userMasterModel.prefixcode_eng;
                dbTable.prefix_in_eng = textInfo.ToTitleCase(userMasterModel.prefix_in_eng.Trim());
                dbTable.fname_in_eng = textInfo.ToTitleCase(userMasterModel.fname_in_eng.Trim());
                dbTable.mname_in_eng = textInfo.ToTitleCase(userMasterModel.mname_in_eng.Trim());
                dbTable.lname_in_eng = textInfo.ToTitleCase(userMasterModel.lname_in_eng.Trim());
                dbTable.prefixcode_marathi = userMasterModel.prefixcode_marathi;
                dbTable.prefix_in_marathi = userMasterModel.prefix_in_marathi.Trim();
                dbTable.fname_in_marathi = userMasterModel.fname_in_marathi.Trim();
                dbTable.mname_in_marathi = userMasterModel.mname_in_marathi.Trim();
                dbTable.lname_in_marathi = userMasterModel.lname_in_marathi.Trim();
                dbTable.address_type = userMasterModel.address_type;
                dbTable.address = string.IsNullOrEmpty(userMasterModel.address) ? "NA" : userMasterModel.address;
                dbTable.state = userMasterModel.state;
                dbTable.district = userMasterModel.district;
                dbTable.taluka = userMasterModel.taluka;
                dbTable.city = userMasterModel.city;
                dbTable.flatno_plotno = userMasterModel.flatno_plotno;
                dbTable.societyname = userMasterModel.societyname;
                dbTable.mainstreet = userMasterModel.mainstreet;
                dbTable.landmark = userMasterModel.landmark;
                dbTable.locality = userMasterModel.locality;
                dbTable.pincode = userMasterModel.pincode;
                dbTable.postofficename = userMasterModel.postofficename;
                dbTable.owner_of_property_in_maharashtra = userMasterModel.owner_of_property_in_maharashtra;
                dbTable.PropertyTypeMaster = userMasterModel.PropertyTypeMaster;

                //Gauri
                dbTable.property_district_code = userMasterModel.property_district_code;
                dbTable.property_district_name = userMasterModel.property_district_name;
                dbTable.property_taluka_code = userMasterModel.property_taluka_code;
                dbTable.property_taluka_name = userMasterModel.property_taluka_name;
                dbTable.property_village_code = userMasterModel.property_village_code;
                dbTable.property_village_name = userMasterModel.property_village_name;

                dbTable.khateno = userMasterModel.khateno;
                dbTable.city_servey_no = userMasterModel.city_servey_no;
                dbTable.ulpin = userMasterModel.ulpin;
                dbTable.username = string.IsNullOrEmpty(userMasterModel.username) ? "NA" : userMasterModel.username;
                dbTable.company_name_in_eng = userMasterModel.company_name_in_eng;
                dbTable.company_name_in_marathi = userMasterModel.company_name_in_marathi;
                dbTable.web_token = "NA";
                dbTable.mobile_token = "NA";
                _context.userMasters.Add(dbTable);
                _context.SaveChanges();

                //Get Saved Row ID
                int UserID = dbTable.userid;

                string VerificationType = string.Empty;
                bool checkImgFlag = true;
                bool checkAddressFlag = true;
                bool checkSignFlag = true;
                if (userMasterModel.address_type == "INDIA")
                {
                    VerificationType = "MOBILENO";
                    if (!string.IsNullOrEmpty(userData.address.indiaAddress!.addressProofSrc) && !string.IsNullOrEmpty(userData.address.indiaAddress.addressProofName))
                    {
                        //UserID = FetchUserID(userMasterModel.mobileno, VerificationType);
                        string image1 = System.IO.Path.GetFileNameWithoutExtension(userData.address.indiaAddress.addressProofName);

                        if (methodForFile.ContainsSpecialCharacters(image1))
                        {
                            return userData.address.indiaAddress.addressProofName + " Image name contains special Characters";
                        }
                        else
                        {
                            string[] AddressData = userData.address.indiaAddress.addressProofSrc.Split(",");
                            checkAddressFlag = methodForFile.SaveImage(AddressData[1], userData.address.indiaAddress.addressProofName, UserID.ToString(), "AddressProof", FolderPath);
                            string AddressProofExt = Path.GetExtension(userData.address.indiaAddress.addressProofName);
                            userMasterModel.address_proof_document_name = "AddressProof" + UserID + AddressProofExt;
                            userMasterModel.address_proof_document_path = FolderPath + UserID + @"\" + userMasterModel.address_proof_document_name;

                            dbTable.address_proof_document_name = userMasterModel.address_proof_document_name;
                            dbTable.address_proof_document_path = userMasterModel.address_proof_document_path;
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
                    if (!string.IsNullOrEmpty(userData.address.indiaAddress.signatureSrc) && !string.IsNullOrEmpty(userData.address.indiaAddress.signatureName))
                    {
                        string image1 = System.IO.Path.GetFileNameWithoutExtension(userData.address.indiaAddress.signatureName);

                        if (methodForFile.ContainsSpecialCharacters(image1))
                        {
                            return userData.address.indiaAddress.signatureName + " Image name contains special Characters";
                        }
                        else
                        {

                            string[] signData = userData.address.indiaAddress.signatureSrc.Split(",");
                            checkSignFlag = methodForFile.SaveImage(signData[1], userData.address.indiaAddress.signatureName, UserID.ToString(), "Signature", FolderPath);
                            string SignatureExt = Path.GetExtension(userData.address.indiaAddress.signatureName);
                            userMasterModel.signed_file_name = "Signature" + UserID + SignatureExt;
                            userMasterModel.signed_file_path = FolderPath + UserID + @"\" + userMasterModel.signed_file_name;

                            dbTable.signed_file_name = userMasterModel.signed_file_name;
                            dbTable.signed_file_path = userMasterModel.signed_file_path;
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
                if (userMasterModel.address_type == "FOREIGN")
                {
                    VerificationType = "EMAILID";
                    //UserID = FetchUserID(userMasterModel.emailid, VerificationType);
                    //userMasterModel.signed_file_path = FolderPath + UserID + @"\" + userData.address.foreignAddress.singnatureName;

                    if (!string.IsNullOrEmpty(userData.address.foreignAddress!.signatureSrc) && !string.IsNullOrEmpty(userData.address.foreignAddress.signatureName))
                    {
                        string image1 = System.IO.Path.GetFileNameWithoutExtension(userData.address.foreignAddress.signatureName);

                        if (methodForFile.ContainsSpecialCharacters(image1))
                        {
                            return userData.address.foreignAddress.signatureName + " Image name contains special Characters";
                        }
                        else
                        {
                            string[] signData = userData.address.foreignAddress.signatureSrc.Split(",");
                            checkSignFlag = methodForFile.SaveImage(signData[1], userData.address.foreignAddress.signatureName, UserID.ToString(), "Signature", FolderPath);

                            string SignatureExt = Path.GetExtension(userData.address.foreignAddress.signatureName);
                            userMasterModel.signed_file_name = "Signature" + UserID + SignatureExt;
                            userMasterModel.signed_file_path = FolderPath + UserID + @"\" + userMasterModel.signed_file_name;

                            dbTable.address_proof_document_name = "NA";
                            dbTable.address_proof_document_path = "NA";
                            dbTable.signed_file_name = userMasterModel.signed_file_name;
                            dbTable.signed_file_path = userMasterModel.signed_file_path;
                            _context.Entry(dbTable).CurrentValues.SetValues(dbTable);
                            _context.SaveChanges();
                        }
                    }
                    else
                    {
                        dbTable.address_proof_document_name = "NA";
                        dbTable.address_proof_document_path = "NA";
                        dbTable.signed_file_name = "NA";
                        dbTable.signed_file_path = "NA";
                        _context.Entry(dbTable).CurrentValues.SetValues(dbTable);
                        _context.SaveChanges();
                    }
                }
                if (!string.IsNullOrEmpty(userData.photo.passportSrc) && !string.IsNullOrEmpty(userData.photo.passportName))
                {
                    string image1 = System.IO.Path.GetFileNameWithoutExtension(userData.photo.passportName);

                    if (methodForFile.ContainsSpecialCharacters(image1))
                    {
                        return userData.photo.passportName + " Image name contains special Characters";
                    }
                    else
                    {
                        string[] imgData = userData.photo.passportSrc.Split(",");
                        checkImgFlag = methodForFile.SaveImage(imgData[1], userData.photo.passportName, UserID.ToString(), "PassportPhoto", FolderPath);
                        string ImgExt = Path.GetExtension(userData.photo.passportName);
                        userMasterModel.profile_pic_file_name = "PassportPhoto" + UserID + ImgExt;
                        userMasterModel.profile_pic_file_path = FolderPath + UserID + @"\" + userMasterModel.profile_pic_file_name;

                        dbTable.profile_pic_file_name = userMasterModel.profile_pic_file_name;
                        dbTable.profile_pic_file_path = userMasterModel.profile_pic_file_path;
                        _context.Entry(dbTable).CurrentValues.SetValues(dbTable);
                        _context.SaveChanges();
                    }
                }
                else
                {
                    dbTable.profile_pic_file_name = "NA";
                    dbTable.profile_pic_file_path = "NA";
                    _context.Entry(dbTable).CurrentValues.SetValues(dbTable);
                    _context.SaveChanges();
                }
                if (checkAddressFlag && checkImgFlag && checkSignFlag)
                {
                    return "Success";
                }
                if (!checkImgFlag)
                {
                    _context.userMasters.Remove(dbTable);
                    _context.SaveChanges();
                    return "Profile Picture Is Not Uploaded";
                }
                if (!checkAddressFlag)
                {
                    _context.userMasters.Remove(dbTable);
                    _context.SaveChanges();
                    return "Address Proof File Is Not Uploaded";
                }
                if (!checkSignFlag)
                {
                    _context.userMasters.Remove(dbTable);
                    _context.SaveChanges();
                    return "Signature File Is Not Uploaded";
                }
                else
                {
                    _context.userMasters.Remove(dbTable);
                    _context.SaveChanges();
                    return "Some Files Are Not Uploaded";
                }
            }
            catch (Exception ex)
            {
                throw new HandleException(ex.Message.ToString());
            }
        }

        public string SaveLabelData(AddLabelData labelData)
        {
            try
            {
                LabelMaster dbTable = new LabelMaster();
                // Assign values to model
                LabelMasterModel labelMasterModel = new LabelMasterModel();
                //labelMasterModel.pagename = labelData.pagename;
                //labelMasterModel.mutationtype = labelData.mutationtype;
                //labelMasterModel.mutationname = labelData.mutationname;
                labelMasterModel.englishname = labelData.englishname;
                labelMasterModel.marathiname = labelData.marathiname;
                labelMasterModel.createdby = labelData.createdby;
                ScreenMaster screenMaster = _context.screenMasters.FirstOrDefault(s => s.screenid == labelData.screenid)!;
                if (screenMaster != null)
                {
                    labelMasterModel.screenMaster = screenMaster;
                    MutationTypeMaster mutationTypeMaster = _context.mutationTypeMasters.FirstOrDefault(s => s.mutationid == labelData.mutationtypeid)!;
                    if (mutationTypeMaster != null)
                    {
                        labelMasterModel.mutationTypeMaster = mutationTypeMaster;
                        dbTable.screenMaster = labelMasterModel.screenMaster;
                        dbTable.mutationTypeMaster = labelMasterModel.mutationTypeMaster;
                        //dbTable.pagename = labelMasterModel.pagename;
                        //dbTable.mutationtype = labelMasterModel.mutationtype;
                        //dbTable.mutationname = labelMasterModel.mutationname;
                        dbTable.englishname = labelMasterModel.englishname;
                        dbTable.marathiname = labelMasterModel.marathiname;
                        dbTable.createdby = labelMasterModel.createdby;
                        //dbTable.guid = Guid.NewGuid().ToString();
                        _context.labelMasters.Add(dbTable);
                        _context.SaveChanges();
                        return "Success";
                    }
                    else
                    {
                        return "Mutation Type ID Is Not Exists";
                    }
                }
                else
                {
                    return "Screen ID Is Not Exists";
                }
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
        }

        public FetchLabelDataInArray FetchLabels(int screenid, int mutationtypeid)
        {
            List<FetchLabelData> fetchLabels = new List<FetchLabelData>();
            FetchLabelDataInArray labelDataInArray = new FetchLabelDataInArray();
            var dataList = _context.labelMasters.Where(w => w.screenMaster!.screenid == screenid
          && w.mutationTypeMaster!.mutationid == mutationtypeid).ToList();
            if (dataList.Count > 0)
            {
                dataList.ForEach(row => fetchLabels.Add(new FetchLabelData()
                {
                    EnglishLabelName = row.englishname,
                    MarathiLabelName = row.marathiname
                }));
            }
            labelDataInArray.EnglishLabelName = new string[fetchLabels.Count];
            labelDataInArray.MarathiLabelName = new string[fetchLabels.Count];
            for (int i = 0; i < fetchLabels.Count; i++)
            {
                labelDataInArray.EnglishLabelName[i] = fetchLabels[i].EnglishLabelName!;
                labelDataInArray.MarathiLabelName[i] = fetchLabels[i].MarathiLabelName!;
            }
            return labelDataInArray;
        }

        public string UpdateToken(string Token, int UserID, string APIFlag)
        {
            try
            {
                if (APIFlag.Trim().ToUpper() == "WEB")
                {
                    _context.userMasters.Where(t => t.userid == UserID).ExecuteUpdate(mt => mt.SetProperty(e => e.web_token, e => Token));
                }
                if (APIFlag.Trim().ToUpper() == "MOBILE")
                {
                    _context.userMasters.Where(t => t.userid == UserID).ExecuteUpdate(mt => mt.SetProperty(e => e.mobile_token, e => Token));
                }
                _context.SaveChanges();
                return "Success";
            }
            catch (Exception ex)
            {
                throw new HandleException(ex.Message.ToString());
            }
        }

        public FetchUserData FetchUserData(int UserID)
        {
            try
            {
                FetchUserData fetchUserData = new FetchUserData();
                UserMaster userData = new UserMaster();
                MethodForFileUpload methodForFile = new MethodForFileUpload();
                userData = _contextR.userMasters.Include(i => i.PropertyTypeMaster).Where(data => data.userid.Equals(UserID)).FirstOrDefault()!;

                string ProfilePicExt = Path.GetExtension(userData.profile_pic_file_path);
                string ProfilePic = methodForFile.ConvertImageToBase64(userData.profile_pic_file_path);
                fetchUserData.profile_pic_file_path = string.IsNullOrEmpty(ProfilePic) ? "NA" : "data:image/" + ProfilePicExt.Replace(".", "") + ";base64," + ProfilePic;

                string AddressProofExt = Path.GetExtension(userData.address_proof_document_path);
                string AddressProof = methodForFile.ConvertImageToBase64(userData.address_proof_document_path);
                fetchUserData.address_proof_document_path = string.IsNullOrEmpty(AddressProof) ? "NA" : "data:image/" + AddressProofExt.Replace(".", "") + ";base64," + AddressProof;

                string SignatureExt = Path.GetExtension(userData.signed_file_path);
                string Signature = methodForFile.ConvertImageToBase64(userData.signed_file_path);
                fetchUserData.signed_file_path = string.IsNullOrEmpty(Signature) ? "NA" : "data:image/" + SignatureExt.Replace(".", "") + ";base64," + Signature;

                fetchUserData.usertype_code = userData.usertype_code;
                fetchUserData.usertype = userData.usertype;
                fetchUserData.mobileno = userData.mobileno;
                fetchUserData.mobilenoverified = userData.mobilenoverified;
                fetchUserData.emailid = userData.emailid;
                fetchUserData.emailidverified = userData.emailidverified;
                fetchUserData.securitypin = userData.securitypin;
                fetchUserData.prefix_in_eng = userData.prefix_in_eng;
                fetchUserData.fname_in_eng = userData.fname_in_eng;
                fetchUserData.mname_in_eng = userData.mname_in_eng;
                fetchUserData.lname_in_eng = userData.lname_in_eng;
                fetchUserData.prefix_in_marathi = userData.prefix_in_marathi;
                fetchUserData.fname_in_marathi = userData.fname_in_marathi;
                fetchUserData.mname_in_marathi = userData.mname_in_marathi;
                fetchUserData.lname_in_marathi = userData.lname_in_marathi;
                fetchUserData.company_name_in_marathi = userData.company_name_in_marathi;
                fetchUserData.company_name_in_eng = userData.company_name_in_eng;
                fetchUserData.username = userData.username;
                fetchUserData.address_type = userData.address_type;
                fetchUserData.address = userData.address;
                fetchUserData.state = userData.state;
                fetchUserData.district = userData.district;
                fetchUserData.taluka = userData.taluka;
                fetchUserData.city = userData.city;
                fetchUserData.flatno_plotno = userData.flatno_plotno;
                fetchUserData.societyname = userData.societyname;
                fetchUserData.mainstreet = userData.mainstreet;
                fetchUserData.landmark = userData.landmark;
                fetchUserData.locality = userData.locality;
                fetchUserData.pincode = userData.pincode;
                fetchUserData.postofficename = userData.postofficename;
                fetchUserData.address_proof_document_name = userData.address_proof_document_name;
                //fetchUserData.address_proof_document_path = userData.address_proof_document_path;
                fetchUserData.owner_of_property_in_maharashtra = userData.owner_of_property_in_maharashtra;
                fetchUserData.propertyType = userData.PropertyTypeMaster.propertytype;
                fetchUserData.propertyType_code = userData.PropertyTypeMaster.propertytypeid;
                //Gauri
                fetchUserData.property_district_code = userData.property_district_code;
                fetchUserData.property_district_name = userData.property_district_name;
                fetchUserData.property_taluka_code = userData.property_taluka_code;
                fetchUserData.property_taluka_name = userData.property_taluka_name;
                fetchUserData.property_village_code = userData.property_village_code;
                fetchUserData.property_village_name = userData.property_village_name;
                fetchUserData.khateno = userData.khateno;
                fetchUserData.city_servey_no = userData.city_servey_no;
                fetchUserData.ulpin = userData.ulpin;
                fetchUserData.profile_pic_file_name = userData.profile_pic_file_name;
                //fetchUserData.profile_pic_file_path = userData.profile_pic_file_path;
                // fetchUserData.signed_file_path = userData.signed_file_path;
                fetchUserData.signed_file_name = userData.signed_file_name;
                return fetchUserData;
            }
            catch (Exception ex)
            {
                throw new HandleException(ex.Message.ToString());
            }

        }

        public string DeleteToken(int UserID, string APIFlag)
        {
            using (var scope = new TransactionScope())
            {
                try
                {
                    if (APIFlag.Trim().ToUpper() == "WEB")
                    {
                        _context.userMasters.Where(t => t.userid == UserID).ExecuteUpdate(mt => mt.SetProperty(e => e.web_token, e => ""));
                    }
                    if (APIFlag.Trim().ToUpper() == "MOBILE")
                    {
                        _context.userMasters.Where(t => t.userid == UserID).ExecuteUpdate(mt => mt.SetProperty(e => e.mobile_token, e => ""));
                    }
                    _context.SaveChanges();
                    scope.Complete();
                    return "Success";
                }
                catch (Exception ex)
                {
                    throw new HandleException(ex.Message.ToString());
                }
            }
        }


        //public string SendRequestAsync(HttpMethod method, ILogger _logger, object? body = null)
        //{
        //    using (HttpClient client = new HttpClient())
        //    {
        //        try
        //        {
        //            string apiUrl = "https://jsonplaceholder.typicode.com/posts";
        //            var postData = new
        //            {
        //                title = "foo",
        //                body = "bar",
        //                userId = 1
        //            };

        //            // Serialize the object to JSON
        //            string json = JsonSerializer.Serialize(postData);
        //            var content = new StringContent(json, Encoding.UTF8, "application/json");

        //            // Send POST request
        //            HttpResponseMessage response = await client.PostAsync(apiUrl, content);
        //            response.EnsureSuccessStatusCode();

        //            string responseBody = await response.Content.ReadAsStringAsync();
        //            Console.WriteLine(responseBody);
        //        }
        //        catch (Exception ex)
        //        {
        //            _logger.LogError($"Attempt {attempt}: Exception occurred - {ex.Message}");
        //        }
        //    }
        //}
    }
}

