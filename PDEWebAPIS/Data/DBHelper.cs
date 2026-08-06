using PDEWebAPIS.InputDataModel;
using PDEWebAPIS.Model;
using PDEWebAPIS.Repository;

namespace PDEWebAPIS.Data
{
    public class DBHelper
    {
        private AppDBContext _context;
        public DBHelper(AppDBContext context)
        {
            _context = context;
        }
        //public int GetUserIDForMobileNo(string MobileNo)
        //{
        //    UserMasterModel response = new UserMasterModel();
        //    int FetchedUserID = 0;
        //    //List<UserMasterModel> response = new List<UserMasterModel>();
        //    //var dataList = _context.userMasters.ToList();
        //    //dataList.ForEach(row => response.Add(new UserMasterModel()
        //    //{
        //    //    userid = row.userid
        //    //}));

        //    var row = _context.userMasters.Where(data => data.mobileno.Equals(MobileNo)).FirstOrDefault();
        //    //var row1 = _context.userMasters.Where(data => data.mobileno.Equals(MobileNo)).First();

        //    if (row != null)
        //    {
        //        FetchedUserID = row.userid;
        //    }
        //    else
        //    {
        //        FetchedUserID = 0;
        //    }
        //    return FetchedUserID;
        //}

        //public int VerifyMobileNo(CheckMobileNoVerificationData CheckMobileNoVerificationData)
        //{
        //    UserMasterModel response = new UserMasterModel();
        //    int FetchedUserID = 0;
        //    var row = (from d in _context.mailid_And_Mobileno_Verifications
        //                  where (d.description == CheckMobileNoVerificationData.MobileNO
        //                  && d.verificationtype== CheckMobileNoVerificationData.VerificationType) 
        //                  select d).Single();

        //    //var row = _context.mailid_And_Mobileno_Verifications.Where(data => data.description.Equals(MobileNo)).FirstOrDefault();
        //    if (row != null)
        //    {
        //        Mailid_And_Mobileno_Verification dbTable = new Mailid_And_Mobileno_Verification();
        //        _context.Remove(dbTable.verificationtype);
        //    }
        //    else
        //    {
        //        FetchedUserID = 0;
        //    }
        //    return FetchedUserID;
        //}

        //public int GetUserIDForEmailID(string EmailID)
        //{
        //    UserMasterModel response = new UserMasterModel();
        //    int FetchedUserID = 0;
        //    var row = _context.userMasters.Where(data => data.emailid.Equals(EmailID.Trim())).FirstOrDefault();
        //    //var row1 = _context.userMasters.Where(data => data.mobileno.Equals(MobileNo)).First();
        //    if (row != null)
        //    {
        //        FetchedUserID = row.userid;
        //    }
        //    else
        //    {
        //        FetchedUserID = 0;
        //    }
        //    return FetchedUserID;
        //}

        //public void SaveVerificationData(Mailid_And_Mobileno_VerificationModel Mailid_And_Mobileno_VerificationModel)
        //{
        //    Mailid_And_Mobileno_Verification dbTable = new Mailid_And_Mobileno_Verification();

        //    //Put operation
        //    dbTable = _context.mailid_And_Mobileno_Verifications.Where(d => d.description.Equals(Mailid_And_Mobileno_VerificationModel.description)).FirstOrDefault()!;
        //    if (dbTable != null)
        //    {
        //        dbTable.verificationtype = Mailid_And_Mobileno_VerificationModel.verificationtype.ToUpper();
        //        dbTable.otp = Mailid_And_Mobileno_VerificationModel.otp;
        //    }
        //    else
        //    {
        //        Mailid_And_Mobileno_Verification dbTable1 = new Mailid_And_Mobileno_Verification();
        //        dbTable1.verificationtype = Mailid_And_Mobileno_VerificationModel.verificationtype.ToUpper();
        //        dbTable1.otp = Mailid_And_Mobileno_VerificationModel.otp;
        //        dbTable1.description = Mailid_And_Mobileno_VerificationModel.description;
        //        _context.mailid_And_Mobileno_Verifications.Add(dbTable1);
        //    }
        //    _context.SaveChanges();
        //}
    }
}
