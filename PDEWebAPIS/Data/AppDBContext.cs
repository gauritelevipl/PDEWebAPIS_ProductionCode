using Microsoft.EntityFrameworkCore;
using PDEWebAPIS.Repository;

namespace PDEWebAPIS.Data
{
    public class AppDBContext : DbContext
    {
        public AppDBContext(DbContextOptions<AppDBContext> options) : base(options) { }

        [Obsolete]
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<PropertyTypeMaster>().HasIndex(u => u.propertytype).IsUnique();
            modelBuilder.Entity<UserMaster>(entity =>
            {
                //  entity.HasCheckConstraint("CK_User_Type", "(usertype='PERSON' OR usertype='COMPANY')");
                entity.HasCheckConstraint("CK_Address_Type", "(address_type='INDIA'OR address_type='FOREIGN')");
                entity.HasCheckConstraint("CK_Mobile_Verified_Flag", "(mobilenoverified='YES'OR mobilenoverified='NO')");
                entity.HasCheckConstraint("CK_Email_Verified_Flag", "(emailidverified='YES'OR emailidverified='NO')");
            });
            modelBuilder.Entity<UserMaster>()
            .Property(b => b.createddatetime)
            .HasDefaultValueSql("current_timestamp");
            modelBuilder.Entity<Mailid_And_Mobileno_Verification>().HasKey(table => new
            {
                table.description
            });
            modelBuilder.Entity<Mailid_And_Mobileno_Verification>(entity =>
            {
                entity.HasCheckConstraint("CK_Verification_Type", "(verificationtype='MOBILENO' OR verificationtype='EMAILID')");
            });
            modelBuilder.Entity<LabelMaster>()
            .Property(b => b.createddatetime)
            .HasDefaultValueSql("current_timestamp");
            modelBuilder.Entity<LabelMaster>()
            .Property(b => b.updateddatetime)
            .HasDefaultValueSql("current_timestamp");

            modelBuilder.Entity<MutationTypeMaster>().HasIndex(u => u.mutationtype).IsUnique();

            modelBuilder.Entity<ApplicationDTL>()
            .Property(b => b.createddatetime)
            .HasDefaultValueSql("current_timestamp");

            modelBuilder.Entity<ApplicantMaster>(entity =>
            {
                //entity.HasCheckConstraint("CK_User_Type", "(usertype='PERSON' OR usertype='COMPANY')");
                entity.HasCheckConstraint("CK_Address_Type", "(address_type='INDIA'OR address_type='FOREIGN')");
            });
            modelBuilder.Entity<ApplicantMaster>()
            .Property(b => b.createddatetime)
            .HasDefaultValueSql("current_timestamp");

            modelBuilder.Entity<MutationCTSNoDTL>(entity =>
            {
                entity.HasCheckConstraint("CHK_what_is_mentioned_in_the_doc", "(what_is_mentioned_in_the_doc='CITY SERVEY NO' OR what_is_mentioned_in_the_doc='SERVEYNO / GROUPNO')");
                entity.HasCheckConstraint("CHK_mutation_modification_type", "(mutation_modification_type='LAND'OR mutation_modification_type='FLAT')");
            });

            modelBuilder.Entity<MutationCTSNoDTL>()
            .Property(b => b.createddatetime)
            .HasDefaultValueSql("current_timestamp");

            modelBuilder.Entity<DastInformation>()
            .Property(b => b.createddatetime)
            .HasDefaultValueSql("current_timestamp");

            modelBuilder.Entity<CourtClaimInformation>()
            .Property(b => b.createddatetime)
            .HasDefaultValueSql("current_timestamp");

            modelBuilder.Entity<PowerOfAttorneyInformation>(entity =>
            {
                entity.HasCheckConstraint("CK_Address_Type", "(address_type='INDIA'OR address_type='FOREIGN')");
            });
            modelBuilder.Entity<PowerOfAttorneyInformation>(entity =>
            {
                // entity.HasCheckConstraint("CK_User_Type", "(usertype='PERSON' OR usertype='COMPANY')");
                entity.HasCheckConstraint("CK_Address_Type", "(address_type='INDIA'OR address_type='FOREIGN')");
            });

            modelBuilder.Entity<PowerOfAttorneyInformation>()
            .Property(b => b.createddatetime)
            .HasDefaultValueSql("current_timestamp");

            modelBuilder.Entity<DocumentTypeMaster>()
            .Property(b => b.createddatetime)
            .HasDefaultValueSql("current_timestamp");

            modelBuilder.Entity<UploadedDocumentsDTL>()
            .Property(b => b.createddatetime)
            .HasDefaultValueSql("current_timestamp");

            modelBuilder.Entity<MutationMaster>()
            .Property(b => b.createddatetime)
            .HasDefaultValueSql("current_timestamp");

            modelBuilder.Entity<PowerOfAttorneyInformation>(entity =>
            {
                //entity.HasCheckConstraint("CK_User_Type", "(usertype='PERSON' OR usertype='COMPANY')");
                entity.HasCheckConstraint("CK_Address_Type", "(address_type='INDIA'OR address_type='FOREIGN')");
            });

            modelBuilder.Entity<MutationGiverTakerDTL>()
            .Property(b => b.createddatetime)
            .HasDefaultValueSql("current_timestamp");

            modelBuilder.Entity<MayatDTL>()
            .Property(b => b.createddatetime)
            .HasDefaultValueSql("current_timestamp");

            modelBuilder.Entity<ApplicationDTL>()
            .Property(b => b.inwardno)
            .HasDefaultValue("NA");

            //Set Default values to Below columns
            modelBuilder.Entity<MutationGiverTakerDTL>()
            .Property(b => b.institute_code)
            .HasDefaultValue("0");
            modelBuilder.Entity<MutationGiverTakerDTL>()
            .Property(b => b.institute_description)
            .HasDefaultValue("NA");
            modelBuilder.Entity<MutationGiverTakerDTL>()
            .Property(b => b.bank_name_in_marathi)
            .HasDefaultValue("NA");
            modelBuilder.Entity<MutationGiverTakerDTL>()
            .Property(b => b.bank_name_in_english)
            .HasDefaultValue("NA");
            modelBuilder.Entity<MutationGiverTakerDTL>()
            .Property(b => b.ifsc)
            .HasDefaultValue("NA");
            //modelBuilder.Entity<MutationGiverTakerDTL>()
            //.Property(b => b.boja_area)
            //.HasDefaultValueSql("NA");
            modelBuilder.Entity<MutationGiverTakerDTL>()
            .Property(b => b.boja_value)
            .HasDefaultValue("NA");
            modelBuilder.Entity<MutationGiverTakerDTL>()
            .Property(b => b.boja_date)
            .HasDefaultValue("NA");
            modelBuilder.Entity<MutationGiverTakerDTL>()
            .Property(b => b.boja_period)
            .HasDefaultValue("NA");
            modelBuilder.Entity<MutationGiverTakerDTL>()
                .Property(b => b.benefit_amt)
                .HasDefaultValue("NA");

            modelBuilder.Entity<ApplicationDTL>()
            .Property(b => b.isDeleted)
            .HasDefaultValue(false);

            modelBuilder.Entity<ApplicationDTL>()
           .Property(b => b.deleteddate)
           .HasDefaultValueSql("1900-01-01");

            modelBuilder.Entity<MutationGiverTakerDTL>()
            .Property(b => b.isDeleted)
            .HasDefaultValue(false);

            modelBuilder.Entity<MutationGiverTakerDTL>()
          .Property(b => b.deleteddate)
          .HasDefaultValueSql("1900-01-01");

            modelBuilder.Entity<ApplicantMaster>()
            .Property(b => b.isDeleted)
            .HasDefaultValue(false);

            modelBuilder.Entity<ApplicantMaster>()
          .Property(b => b.deleteddate)
          .HasDefaultValueSql("1900-01-01");

            modelBuilder.Entity<MutationCTSNoDTL>()
            .Property(b => b.isDeleted)
            .HasDefaultValue(false);

            modelBuilder.Entity<MutationCTSNoDTL>()
          .Property(b => b.deleteddate)
          .HasDefaultValueSql("1900-01-01");

            modelBuilder.Entity<DastInformation>()
            .Property(b => b.isDeleted)
            .HasDefaultValue(false);

            modelBuilder.Entity<DastInformation>()
          .Property(b => b.deleteddate)
          .HasDefaultValueSql("1900-01-01");

            modelBuilder.Entity<PowerOfAttorneyInformation>()
           .Property(b => b.mutation_srno)
           .HasDefaultValue("NA");
            modelBuilder.Entity<PowerOfAttorneyInformation>()
             .Property(b => b.owner_number)
             .HasDefaultValue("NA");
            modelBuilder.Entity<PowerOfAttorneyInformation>()
             .Property(b => b.cts_number)
             .HasDefaultValue("NA");
            modelBuilder.Entity<PowerOfAttorneyInformation>()
          .Property(b => b.sub_property_no)
          .HasDefaultValue("999999");
            modelBuilder.Entity<PowerOfAttorneyInformation>().Property(b => b.village_code).HasDefaultValue("0");
            modelBuilder.Entity<PowerOfAttorneyInformation>().Property(b => b.village_name).HasDefaultValue("NA");
            modelBuilder.Entity<PowerOfAttorneyInformation>()
           .Property(b => b.isDeleted)
           .HasDefaultValue(false);

            modelBuilder.Entity<PowerOfAttorneyInformation>()
          .Property(b => b.deleteddate)
          .HasDefaultValueSql("1900-01-01");

            modelBuilder.Entity<CourtClaimInformation>()
          .Property(b => b.isDeleted)
          .HasDefaultValue(false);

            modelBuilder.Entity<CourtClaimInformation>()
          .Property(b => b.deleteddate)
          .HasDefaultValueSql("1900-01-01");

            modelBuilder.Entity<UploadedDocumentsDTL>()
       .Property(b => b.isDeleted)
       .HasDefaultValue(false);

            modelBuilder.Entity<UploadedDocumentsDTL>()
          .Property(b => b.deleteddate)
          .HasDefaultValueSql("1900-01-01");


            modelBuilder.Entity<MayatDTL>()
           .Property(b => b.mutation_srno)
           .HasDefaultValue("NA");
            modelBuilder.Entity<MayatDTL>()
             .Property(b => b.owner_number)
             .HasDefaultValue("NA");
            modelBuilder.Entity<MayatDTL>()
             .Property(b => b.cts_number)
             .HasDefaultValue("NA");
            modelBuilder.Entity<MayatDTL>().Property(b => b.sub_property_no).HasDefaultValue("999999");
            modelBuilder.Entity<MayatDTL>()
            .Property(b => b.isDeleted)
            .HasDefaultValue(false);

            modelBuilder.Entity<MayatDTL>()
            .Property(b => b.deleteddate)
            .HasDefaultValueSql("1900-01-01");

            modelBuilder.Entity<ApplicationDTL>()
               .Property(b => b.self_declaration_doc_name)
               .HasDefaultValue("NA");
            modelBuilder.Entity<ApplicationDTL>()
               .Property(b => b.self_declaration_doc_path)
               .HasDefaultValue("NA");
            modelBuilder.Entity<ApplicationDTL>().Property(b => b.is_sentnic).HasDefaultValue(false);
            modelBuilder.Entity<ApplicationDTL>().Property(b => b.sentnic_attempts).HasDefaultValue(0);

            modelBuilder.Entity<MutationGiverTakerDTL>().Property(b => b.mutation_cts_no_id).HasDefaultValue(0);
            modelBuilder.Entity<MutationGiverTakerDTL>()
             .Property(b => b.sellerid)
             .HasDefaultValue("NA");
            modelBuilder.Entity<MutationGiverTakerDTL>()
             .Property(b => b.buyerid)
             .HasDefaultValue("NA");
            modelBuilder.Entity<MutationGiverTakerDTL>()
             .Property(b => b.mutation_srno)
             .HasDefaultValue("NA");
            modelBuilder.Entity<MutationGiverTakerDTL>()
             .Property(b => b.owner_number)
             .HasDefaultValue("NA");
            modelBuilder.Entity<MutationGiverTakerDTL>()
             .Property(b => b.cts_number)
             .HasDefaultValue("NA");
            modelBuilder.Entity<MutationCTSNoDTL>()
          .Property(b => b.sub_property_id)
          .HasDefaultValue("999999");
            modelBuilder.Entity<NICAPIResponse>()
        .Property(b => b.createddatetime)
        .HasDefaultValueSql("current_timestamp");

            modelBuilder.Entity<UploadedDocumentsDTL>()
         .Property(b => b.nicdate)
         .HasDefaultValueSql("1900-01-01");

            modelBuilder.Entity<OtpVerify>()
          .Property(b => b.createddatetime)
          .HasDefaultValueSql("current_timestamp");
            modelBuilder.Entity<OtpVerify>().HasKey(table => new
            {
                table.mobileno
            });

            modelBuilder.Entity<ExternalAPIResponse>().Property(b => b.createddatetime)
              .HasDefaultValueSql("current_timestamp");

            modelBuilder.Entity<ApplicationStatusHistory>().Property(b => b.createddatetime)
             .HasDefaultValueSql("current_timestamp");

            modelBuilder.Entity<UploadedDocumentsDTL>()
        .Property(b => b.truti_patra_flag)
        .HasDefaultValue("NA");
            modelBuilder.Entity<InwardNoStatusMaster>().HasIndex(u => new { u.srno, u.inwardno }).IsUnique();

            modelBuilder.Entity<InwardNoStatusMaster>().Property(b => b.createddatetime)
              .HasDefaultValueSql("current_timestamp");


            //grievance
            modelBuilder.Entity<GrievanceTbl>()
               .Property(b => b.issueReportDate)
               .HasDefaultValueSql("current_timestamp");

            modelBuilder.Entity<GrievanceUserMaster>()
    .Property(b => b.registerdatetime)
    .HasDefaultValueSql("current_timestamp");

            modelBuilder.Entity<BlacklistTokenForGrievance>()
               .Property(b => b.createdDateTime)
               .HasDefaultValueSql("current_timestamp");

            //----------------------------------------------
            modelBuilder.Entity<GrievanceTbl>()
           .Property(b => b.IssueSeenDateTime)
            .HasDefaultValueSql("'1900-01-01 00:00:00 UTC'::timestamptz");

            modelBuilder.Entity<GrievanceTbl>()
            .Property(b => b.IssueAssignDateTime)
            .HasDefaultValueSql("'1900-01-01 00:00:00 UTC'::timestamptz");

            modelBuilder.Entity<GrievanceTbl>()
            .Property(b => b.IssueReassignedDateTime)
            .HasDefaultValueSql("'1900-01-01 00:00:00 UTC'::timestamptz");

            modelBuilder.Entity<GrievanceTbl>()
            .Property(b => b.IssueResolvedDateTime)
            .HasDefaultValueSql("'1900-01-01 00:00:00 UTC'::timestamptz");


            //----------------------------------------------
            modelBuilder.Entity<GrievanceIssueDatesDetails>()
            .Property(b => b.Datetime)
            .HasDefaultValueSql("current_timestamp");

            modelBuilder.Entity<GrievanceIssueDatesDetails>()
            .Property(b => b.StatusDatetime)
             .HasDefaultValueSql("'1900-01-01 00:00:00 UTC'::timestamptz");

            modelBuilder.Entity<GrievanceTbl>()
            .Property(b => b.secondaryMoNo)
            .HasDefaultValue("NA");

            modelBuilder.Entity<GrievanveIssueTransactionDtl>().Property(b => b.datetime)
            .HasDefaultValueSql("current_timestamp");

            modelBuilder.Entity<GrievanceTbl>()
           .Property(b => b.takrarPurtataStatusInYOrN)
           .HasDefaultValue("NA");

            //modelBuilder.Entity<pinCodeApiResponseTbl>()
            //.Property(b => b.createdDateTime).
            //HasDefaultValueSql("current_timestamp");



            //modelBuilder.Entity<pinCodeMaster>().Property(b => b.createdDateTime)
            //  .HasDefaultValueSql("current_timestamp");

            modelBuilder.Entity<MutationGiverTakerDTL>()
         .Property(b => b.owner_village_code)
         .HasDefaultValue("NA");

            modelBuilder.Entity<GrievanceUserMaster>()
            .Property(b => b.district_code)
            .HasDefaultValue("0");

            modelBuilder.Entity<GrievanceUserMaster>()
            .Property(b => b.district_english_name)
            .HasDefaultValue("NA");

            modelBuilder.Entity<GrievanceUserMaster>()
            .Property(b => b.district_name)
            .HasDefaultValue("NA");

            modelBuilder.Entity<GrievanceUserMaster>()
            .Property(b => b.region_code)
            .HasDefaultValue("0");

            modelBuilder.Entity<GrievanceUserMaster>()
            .Property(b => b.region_english_name)
            .HasDefaultValue("NA");

            modelBuilder.Entity<GrievanceUserMaster>()
            .Property(b => b.region_name)
            .HasDefaultValue("NA");

            modelBuilder.Entity<ApplicationDataSubmittedHistory>(entity =>
            {
                entity.HasCheckConstraint("CK_application_submitted_type", "(application_submitted_type='WEB' OR application_submitted_type='MOBILE')");
            });

            modelBuilder.Entity<ApplicationDataSubmittedHistory>()
          .Property(b => b.createddatetime)
          .HasDefaultValueSql("current_timestamp");

            modelBuilder.Entity<pinCodeMaster>().Property(b => b.createdDateTime)
            .HasDefaultValueSql("current_timestamp");

            // Below code added on 16 Feb 2026
            modelBuilder.Entity<BhadepattaInfoDtl>(entity =>
            {
                entity.Property(e => e.createdDateTime)
                      .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.isDeleted)
                      .HasColumnType("boolean")
                      .HasDefaultValue(false);

                entity.Property(e => e.deletedDateTime)
                      .HasDefaultValueSql("'1900-01-01 00:00:00'");
            });

            // Below fields for Generic
            modelBuilder.Entity<MutationGiverTakerDTL>()
                .Property(b => b.entry_bracketed)
                .HasDefaultValue("NA");
            modelBuilder.Entity<MutationGiverTakerDTL>()
                .Property(b => b.entry_date)
                .HasDefaultValue("NA");
            modelBuilder.Entity<MutationGiverTakerDTL>()
                .Property(b => b.owner_bracketed)
                .HasDefaultValue("NA");
            modelBuilder.Entity<MutationGiverTakerDTL>()
                .Property(b => b.owner_name)
                .HasDefaultValue("NA");

            modelBuilder.Entity<MayatDTL>()
            .Property(b => b.probet_file_name)
            .HasDefaultValue("NA");

            modelBuilder.Entity<MayatDTL>()
           .Property(b => b.probet_file_path)
           .HasDefaultValue("NA");

            modelBuilder.Entity<MayatDTL>()
         .Property(b => b.isprobet)
         .HasDefaultValue(false);

            modelBuilder.Entity<BhadepattaInfoDtl>()
         .Property(b => b.bhadepattaFromDate)
         .HasDefaultValue("NA");

            modelBuilder.Entity<BhadepattaInfoDtl>()
         .Property(b => b.bhadepattaToDate)
         .HasDefaultValue("NA");

            modelBuilder.Entity<BhadepattaInfoDtl>()
       .Property(b => b.leaseperiod)
       .HasDefaultValue(false);
        }

        public DbSet<PropertyTypeMaster> propertyTypes { get; set; }
        public DbSet<UserMaster> userMasters { get; set; }
        public DbSet<Mailid_And_Mobileno_Verification> mailid_And_Mobileno_Verifications { get; set; }
        public DbSet<LabelMaster> labelMasters { get; set; }
        public DbSet<ScreenMaster> screenMasters { get; set; }
        public DbSet<MutationTypeMaster> mutationTypeMasters { get; set; }
        public DbSet<ApplicationTypeMaster> applicationTypeMasters { get; set; }
        public DbSet<ApplicationDTL> applicationDTL { get; set; }
        public DbSet<ApplicantMaster> applicantMasters { set; get; }
        public DbSet<MutationCTSNoDTL> mutationCTSNoDTLs { set; get; }
        public DbSet<DastInformation> dastInformation { set; get; }
        public DbSet<CourtClaimInformation> courtClaimInformation { set; get; }
        public DbSet<PowerOfAttorneyInformation> powerOfAttorneyInformation { set; get; }
        public DbSet<DocumentTypeMaster> documentTypeMasters { set; get; }
        public DbSet<UploadedDocumentsDTL> uploadedDocumentsDTLs { set; get; }
        public DbSet<MutationMaster> mutationMasters { set; get; }
        public DbSet<MutationGiverTakerDTL> mutationDTL { set; get; }
        public DbSet<MayatDTL> mayatDTL { set; get; }
        public DbSet<NICAPIResponse> nICAPIResponses { get; set; }
        public DbSet<OtpVerify> otpVerifies { set; get; }
        public DbSet<ExternalAPIResponse> externalAPIResponses { get; set; }
        public DbSet<ApplicationStatusHistory> applicationStatusHistories { get; set; }

        //Gauri W
        public string tickitid;
        public DbSet<GrievanceTbl> grievanceData { get; set; }
        public DbSet<GrievanceStatusMaster> grievanceStatusMaster { get; set; }

        public DbSet<GrievanceReasonMaster> grievanceReasonMaster { get; set; }
        public DbSet<GrievanceUserMaster> grievanceUserMaster { get; set; }
        public DbSet<BlacklistTokenForGrievance> blacklistTokensForGrievance { get; set; }
        public DbSet<GrievanceIssueDatesDetails> grievanceIssueDatesDetails { get; set; }
        //public DbSet<ApplicationStatusHistory> applicationStatusHistories { get; set; }
        //public DbSet<pinCodeApiResponseTbl> pinCodeApiResponseTbl { get; set; }

        //public DbSet<InwardNoStatusMaster> inwardNoStatusMasters { get; set; }

        //public DbSet<pinCodeMaster> pincodemaster { get; set; }
        public DbSet<GrievanveIssueTransactionDtl> grievanveIssueTransactionDtls { get; set; }
        public DbSet<InwardNoStatusMaster> inwardNoStatusMasters { get; set; }
        public DbSet<pinCodeMaster> pincodemaster { get; set; }
        public DbSet<ApplicationDataSubmittedHistory> applicationDataSubmittedHistories { get; set; }
        public DbSet<BhadepattaInfoDtl> bhadepattaInfoDtl { get; set; }
    }
}
