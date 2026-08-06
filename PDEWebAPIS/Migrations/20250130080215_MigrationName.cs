using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace PDEWebAPIS.Migrations
{
    /// <inheritdoc />
    public partial class MigrationName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "applicationtypemaster",
                columns: table => new
                {
                    applicationtypeid = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    application_type_name_in_eng = table.Column<string>(type: "text", nullable: true),
                    application_type_name_in_marathi = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_applicationtypemaster", x => x.applicationtypeid);
                });

            migrationBuilder.CreateTable(
                name: "document_type_master",
                columns: table => new
                {
                    document_type_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    document_type_name = table.Column<string>(type: "text", nullable: true),
                    mutation_type_code = table.Column<string>(type: "text", nullable: true),
                    mutation_description = table.Column<string>(type: "text", nullable: true),
                    createddatetime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "current_timestamp")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_document_type_master", x => x.document_type_id);
                });

            migrationBuilder.CreateTable(
                name: "mailid_and_mobileno_verification",
                columns: table => new
                {
                    description = table.Column<string>(type: "text", nullable: false),
                    verificationtype = table.Column<string>(type: "text", nullable: false),
                    otp = table.Column<long>(type: "bigint", maxLength: 6, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mailid_and_mobileno_verification", x => x.description);
                    table.CheckConstraint("CK_Verification_Type", "(verificationtype='MOBILENO' OR verificationtype='EMAILID')");
                });

            migrationBuilder.CreateTable(
                name: "mutationtypemaster",
                columns: table => new
                {
                    mutationid = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    mutationtype = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mutationtypemaster", x => x.mutationid);
                });

            migrationBuilder.CreateTable(
                name: "nic_api_response",
                columns: table => new
                {
                    response_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    applicationid = table.Column<string>(type: "text", nullable: true),
                    inwardno = table.Column<string>(type: "text", nullable: true),
                    response = table.Column<string>(type: "text", nullable: true),
                    apicallcount = table.Column<int>(type: "integer", nullable: false),
                    createddatetime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "current_timestamp")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_nic_api_response", x => x.response_id);
                });

            migrationBuilder.CreateTable(
                name: "propertytypemaster",
                columns: table => new
                {
                    propertytypeid = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    propertytype = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_propertytypemaster", x => x.propertytypeid);
                });

            migrationBuilder.CreateTable(
                name: "screenmaster",
                columns: table => new
                {
                    screenid = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    screenname = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_screenmaster", x => x.screenid);
                });

            migrationBuilder.CreateTable(
                name: "mutationmaster",
                columns: table => new
                {
                    mutation_code = table.Column<string>(type: "text", nullable: false),
                    mutation_name = table.Column<string>(type: "text", nullable: true),
                    applicationtypeid = table.Column<int>(type: "integer", nullable: true),
                    createddatetime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "current_timestamp")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mutationmaster", x => x.mutation_code);
                    table.ForeignKey(
                        name: "FK_mutationmaster_applicationtypemaster_applicationtypeid",
                        column: x => x.applicationtypeid,
                        principalTable: "applicationtypemaster",
                        principalColumn: "applicationtypeid");
                });

            migrationBuilder.CreateTable(
                name: "usermaster",
                columns: table => new
                {
                    userid = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    usertype_code = table.Column<int>(type: "integer", nullable: false),
                    usertype = table.Column<string>(type: "text", nullable: false),
                    mobileno = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false),
                    mobilenoverified = table.Column<string>(type: "text", nullable: false),
                    emailid = table.Column<string>(type: "text", nullable: false),
                    emailidverified = table.Column<string>(type: "text", nullable: false),
                    securitypin = table.Column<string>(type: "character varying(6)", maxLength: 6, nullable: false),
                    prefixcode_eng = table.Column<string>(type: "text", nullable: false),
                    prefix_in_eng = table.Column<string>(type: "text", nullable: false),
                    fname_in_eng = table.Column<string>(type: "text", nullable: false),
                    mname_in_eng = table.Column<string>(type: "text", nullable: false),
                    lname_in_eng = table.Column<string>(type: "text", nullable: false),
                    prefixcode_marathi = table.Column<string>(type: "text", nullable: false),
                    prefix_in_marathi = table.Column<string>(type: "text", nullable: false),
                    fname_in_marathi = table.Column<string>(type: "text", nullable: false),
                    mname_in_marathi = table.Column<string>(type: "text", nullable: false),
                    lname_in_marathi = table.Column<string>(type: "text", nullable: false),
                    company_name_in_marathi = table.Column<string>(type: "text", nullable: false),
                    company_name_in_eng = table.Column<string>(type: "text", nullable: false),
                    username = table.Column<string>(type: "text", nullable: false),
                    address_type = table.Column<string>(type: "text", nullable: false),
                    address = table.Column<string>(type: "text", nullable: false),
                    state = table.Column<string>(type: "text", nullable: false),
                    district = table.Column<string>(type: "text", nullable: false),
                    taluka = table.Column<string>(type: "text", nullable: false),
                    city = table.Column<string>(type: "text", nullable: false),
                    flatno_plotno = table.Column<string>(type: "text", nullable: false),
                    societyname = table.Column<string>(type: "text", nullable: false),
                    mainstreet = table.Column<string>(type: "text", nullable: false),
                    landmark = table.Column<string>(type: "text", nullable: false),
                    locality = table.Column<string>(type: "text", nullable: false),
                    pincode = table.Column<string>(type: "text", nullable: false),
                    postofficename = table.Column<string>(type: "text", nullable: false),
                    address_proof_document_name = table.Column<string>(type: "text", nullable: false),
                    address_proof_document_path = table.Column<string>(type: "text", nullable: false),
                    owner_of_property_in_maharashtra = table.Column<bool>(type: "boolean", nullable: false),
                    PropertyTypeMasterpropertytypeid = table.Column<int>(type: "integer", nullable: false),
                    property_district_code = table.Column<string>(type: "text", nullable: true),
                    property_district_name = table.Column<string>(type: "text", nullable: true),
                    property_taluka_code = table.Column<string>(type: "text", nullable: true),
                    property_taluka_name = table.Column<string>(type: "text", nullable: true),
                    property_village_code = table.Column<string>(type: "text", nullable: true),
                    property_village_name = table.Column<string>(type: "text", nullable: true),
                    khateno = table.Column<string>(type: "text", nullable: true),
                    city_servey_no = table.Column<string>(type: "text", nullable: true),
                    ulpin = table.Column<string>(type: "text", nullable: true),
                    profile_pic_file_name = table.Column<string>(type: "text", nullable: false),
                    profile_pic_file_path = table.Column<string>(type: "text", nullable: false),
                    signed_file_path = table.Column<string>(type: "text", nullable: false),
                    signed_file_name = table.Column<string>(type: "text", nullable: false),
                    createddatetime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "current_timestamp"),
                    web_token = table.Column<string>(type: "text", nullable: false),
                    mobile_token = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_usermaster", x => x.userid);
                    table.CheckConstraint("CK_Address_Type", "(address_type='INDIA'OR address_type='FOREIGN')");
                    table.CheckConstraint("CK_Email_Verified_Flag", "(emailidverified='YES'OR emailidverified='NO')");
                    table.CheckConstraint("CK_Mobile_Verified_Flag", "(mobilenoverified='YES'OR mobilenoverified='NO')");
                    table.ForeignKey(
                        name: "FK_usermaster_propertytypemaster_PropertyTypeMasterpropertytyp~",
                        column: x => x.PropertyTypeMasterpropertytypeid,
                        principalTable: "propertytypemaster",
                        principalColumn: "propertytypeid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "labelmaster",
                columns: table => new
                {
                    labelid = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    screenMasterscreenid = table.Column<int>(type: "integer", nullable: true),
                    mutationTypeMastermutationid = table.Column<int>(type: "integer", nullable: true),
                    englishname = table.Column<string>(type: "text", nullable: true),
                    marathiname = table.Column<string>(type: "text", nullable: true),
                    createdby = table.Column<string>(type: "text", nullable: true),
                    createddatetime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "current_timestamp"),
                    updateddatetime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "current_timestamp")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_labelmaster", x => x.labelid);
                    table.ForeignKey(
                        name: "FK_labelmaster_mutationtypemaster_mutationTypeMastermutationid",
                        column: x => x.mutationTypeMastermutationid,
                        principalTable: "mutationtypemaster",
                        principalColumn: "mutationid");
                    table.ForeignKey(
                        name: "FK_labelmaster_screenmaster_screenMasterscreenid",
                        column: x => x.screenMasterscreenid,
                        principalTable: "screenmaster",
                        principalColumn: "screenid");
                });

            migrationBuilder.CreateTable(
                name: "applicationdtl",
                columns: table => new
                {
                    applicationid = table.Column<string>(type: "text", nullable: false),
                    userMasteruserid = table.Column<int>(type: "integer", nullable: true),
                    applicationTypeMasterapplicationtypeid = table.Column<int>(type: "integer", nullable: true),
                    mutation_type_code = table.Column<string>(type: "text", nullable: true),
                    mutation_type_name = table.Column<string>(type: "text", nullable: true),
                    district_code = table.Column<string>(type: "text", nullable: true),
                    district_name_in_marathi = table.Column<string>(type: "text", nullable: true),
                    district_name_in_english = table.Column<string>(type: "text", nullable: true),
                    office_code = table.Column<string>(type: "text", nullable: true),
                    office_name = table.Column<string>(type: "text", nullable: true),
                    do_you_have_power_of_attorney = table.Column<bool>(type: "boolean", nullable: false),
                    Is_the_claim_pending_before_the_court = table.Column<bool>(type: "boolean", nullable: false),
                    does_the_original_charter_info_apply = table.Column<bool>(type: "boolean", nullable: false),
                    applicantIDs = table.Column<string>(type: "text", nullable: true),
                    mutation_cts_nos = table.Column<string>(type: "text", nullable: true),
                    dastIDs = table.Column<string>(type: "text", nullable: true),
                    courtClaimIDs = table.Column<string>(type: "text", nullable: true),
                    powerOfAttorneyIDs = table.Column<string>(type: "text", nullable: true),
                    uploadedDocIDs = table.Column<string>(type: "text", nullable: true),
                    mutationgiverIDs = table.Column<string>(type: "text", nullable: true),
                    mutationtakerIDs = table.Column<string>(type: "text", nullable: true),
                    mayatIDs = table.Column<string>(type: "text", nullable: true),
                    varasIDS = table.Column<string>(type: "text", nullable: true),
                    status = table.Column<int>(type: "integer", nullable: false),
                    self_declaration_doc_name = table.Column<string>(type: "text", nullable: true, defaultValue: "NA"),
                    self_declaration_doc_path = table.Column<string>(type: "text", nullable: true, defaultValue: "NA"),
                    inwardno = table.Column<string>(type: "text", nullable: true, defaultValue: "NA"),
                    is_sentnic = table.Column<bool>(type: "boolean", nullable: true, defaultValue: false),
                    sentnic_attempts = table.Column<int>(type: "integer", nullable: true, defaultValue: 0),
                    createddatetime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "current_timestamp"),
                    isDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    deleteddate = table.Column<DateOnly>(type: "date", nullable: false, defaultValueSql: "1900-01-01")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_applicationdtl", x => x.applicationid);
                    table.ForeignKey(
                        name: "FK_applicationdtl_applicationtypemaster_applicationTypeMastera~",
                        column: x => x.applicationTypeMasterapplicationtypeid,
                        principalTable: "applicationtypemaster",
                        principalColumn: "applicationtypeid");
                    table.ForeignKey(
                        name: "FK_applicationdtl_usermaster_userMasteruserid",
                        column: x => x.userMasteruserid,
                        principalTable: "usermaster",
                        principalColumn: "userid");
                });

            migrationBuilder.CreateTable(
                name: "applicantmaster",
                columns: table => new
                {
                    applicantid = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    userMasteruserid = table.Column<int>(type: "integer", nullable: true),
                    PropertyTypeMasterpropertytypeid = table.Column<int>(type: "integer", nullable: true),
                    applicationDTLapplicationid = table.Column<string>(type: "text", nullable: true),
                    usertype_code = table.Column<int>(type: "integer", nullable: false),
                    usertype = table.Column<string>(type: "text", nullable: false),
                    mobileno = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false),
                    mobilenoverified = table.Column<string>(type: "text", nullable: false),
                    emailid = table.Column<string>(type: "text", nullable: false),
                    emailidverified = table.Column<string>(type: "text", nullable: false),
                    securitypin = table.Column<string>(type: "character varying(6)", maxLength: 6, nullable: false),
                    prefix_in_eng = table.Column<string>(type: "text", nullable: false),
                    fname_in_eng = table.Column<string>(type: "text", nullable: false),
                    mname_in_eng = table.Column<string>(type: "text", nullable: false),
                    lname_in_eng = table.Column<string>(type: "text", nullable: false),
                    prefix_in_marathi = table.Column<string>(type: "text", nullable: false),
                    fname_in_marathi = table.Column<string>(type: "text", nullable: false),
                    mname_in_marathi = table.Column<string>(type: "text", nullable: false),
                    lname_in_marathi = table.Column<string>(type: "text", nullable: false),
                    company_name_in_marathi = table.Column<string>(type: "text", nullable: false),
                    company_name_in_eng = table.Column<string>(type: "text", nullable: false),
                    username = table.Column<string>(type: "text", nullable: false),
                    address_type = table.Column<string>(type: "text", nullable: false),
                    address = table.Column<string>(type: "text", nullable: false),
                    state = table.Column<string>(type: "text", nullable: false),
                    district = table.Column<string>(type: "text", nullable: false),
                    taluka = table.Column<string>(type: "text", nullable: false),
                    city = table.Column<string>(type: "text", nullable: false),
                    flatno_plotno = table.Column<string>(type: "text", nullable: false),
                    societyname = table.Column<string>(type: "text", nullable: false),
                    mainstreet = table.Column<string>(type: "text", nullable: false),
                    landmark = table.Column<string>(type: "text", nullable: false),
                    locality = table.Column<string>(type: "text", nullable: false),
                    pincode = table.Column<string>(type: "text", nullable: false),
                    postofficename = table.Column<string>(type: "text", nullable: false),
                    address_proof_document_name = table.Column<string>(type: "text", nullable: false),
                    address_proof_document_path = table.Column<string>(type: "text", nullable: false),
                    owner_of_property_in_maharashtra = table.Column<bool>(type: "boolean", nullable: false),
                    khateno = table.Column<string>(type: "text", nullable: true),
                    city_servey_no = table.Column<string>(type: "text", nullable: true),
                    ulpin = table.Column<string>(type: "text", nullable: true),
                    property_district_code = table.Column<string>(type: "text", nullable: false),
                    property_district_name = table.Column<string>(type: "text", nullable: false),
                    property_taluka_code = table.Column<string>(type: "text", nullable: false),
                    property_taluka_name = table.Column<string>(type: "text", nullable: false),
                    property_village_code = table.Column<string>(type: "text", nullable: false),
                    property_village_name = table.Column<string>(type: "text", nullable: false),
                    profile_pic_file_name = table.Column<string>(type: "text", nullable: false),
                    profile_pic_file_path = table.Column<string>(type: "text", nullable: false),
                    signed_file_path = table.Column<string>(type: "text", nullable: false),
                    signed_file_name = table.Column<string>(type: "text", nullable: false),
                    createddatetime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "current_timestamp"),
                    isDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    deleteddate = table.Column<DateOnly>(type: "date", nullable: false, defaultValueSql: "1900-01-01")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_applicantmaster", x => x.applicantid);
                    table.CheckConstraint("CK_Address_Type", "(address_type='INDIA'OR address_type='FOREIGN')");
                    table.ForeignKey(
                        name: "FK_applicantmaster_applicationdtl_applicationDTLapplicationid",
                        column: x => x.applicationDTLapplicationid,
                        principalTable: "applicationdtl",
                        principalColumn: "applicationid");
                    table.ForeignKey(
                        name: "FK_applicantmaster_propertytypemaster_PropertyTypeMasterproper~",
                        column: x => x.PropertyTypeMasterpropertytypeid,
                        principalTable: "propertytypemaster",
                        principalColumn: "propertytypeid");
                    table.ForeignKey(
                        name: "FK_applicantmaster_usermaster_userMasteruserid",
                        column: x => x.userMasteruserid,
                        principalTable: "usermaster",
                        principalColumn: "userid");
                });

            migrationBuilder.CreateTable(
                name: "court_claim_information",
                columns: table => new
                {
                    court_claim_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    userMasteruserid = table.Column<int>(type: "integer", nullable: true),
                    applicationDTLapplicationid = table.Column<string>(type: "text", nullable: true),
                    court_case_code = table.Column<string>(type: "text", nullable: true),
                    court_case_name = table.Column<string>(type: "text", nullable: true),
                    court_case_type_code = table.Column<string>(type: "text", nullable: true),
                    court_case_type_name = table.Column<string>(type: "text", nullable: true),
                    lr_property_uid = table.Column<string>(type: "text", nullable: true),
                    city_servey_no = table.Column<string>(type: "text", nullable: true),
                    order_details = table.Column<string>(type: "text", nullable: true),
                    stay_order = table.Column<string>(type: "text", nullable: true),
                    sub_property_no = table.Column<string>(type: "text", nullable: true),
                    createddatetime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "current_timestamp"),
                    isDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    deleteddate = table.Column<DateOnly>(type: "date", nullable: false, defaultValueSql: "1900-01-01")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_court_claim_information", x => x.court_claim_id);
                    table.ForeignKey(
                        name: "FK_court_claim_information_applicationdtl_applicationDTLapplic~",
                        column: x => x.applicationDTLapplicationid,
                        principalTable: "applicationdtl",
                        principalColumn: "applicationid");
                    table.ForeignKey(
                        name: "FK_court_claim_information_usermaster_userMasteruserid",
                        column: x => x.userMasteruserid,
                        principalTable: "usermaster",
                        principalColumn: "userid");
                });

            migrationBuilder.CreateTable(
                name: "dast_information",
                columns: table => new
                {
                    dast_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    userMasteruserid = table.Column<int>(type: "integer", nullable: true),
                    applicationDTLapplicationid = table.Column<string>(type: "text", nullable: true),
                    dastType = table.Column<string>(type: "text", nullable: true),
                    division_code = table.Column<int>(type: "integer", nullable: true),
                    division_name = table.Column<string>(type: "text", nullable: true),
                    districtCode = table.Column<string>(type: "text", nullable: true),
                    districtName = table.Column<string>(type: "text", nullable: true),
                    office_of_the_second_registrar_code = table.Column<string>(type: "text", nullable: true),
                    office_of_the_second_registrar_name = table.Column<string>(type: "text", nullable: true),
                    registered_dast_no = table.Column<string>(type: "text", nullable: true),
                    registered_dast_date = table.Column<string>(type: "text", nullable: true),
                    registered_dast_year = table.Column<string>(type: "text", nullable: true),
                    dastNabhu = table.Column<string>(type: "text", nullable: true),
                    remarks = table.Column<string>(type: "text", nullable: true),
                    isDastVerified = table.Column<bool>(type: "boolean", nullable: true),
                    verifiedDastData = table.Column<string>(type: "text", nullable: true),
                    createddatetime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "current_timestamp"),
                    isDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    deleteddate = table.Column<DateOnly>(type: "date", nullable: false, defaultValueSql: "1900-01-01")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dast_information", x => x.dast_id);
                    table.ForeignKey(
                        name: "FK_dast_information_applicationdtl_applicationDTLapplicationid",
                        column: x => x.applicationDTLapplicationid,
                        principalTable: "applicationdtl",
                        principalColumn: "applicationid");
                    table.ForeignKey(
                        name: "FK_dast_information_usermaster_userMasteruserid",
                        column: x => x.userMasteruserid,
                        principalTable: "usermaster",
                        principalColumn: "userid");
                });

            migrationBuilder.CreateTable(
                name: "mayatdtl",
                columns: table => new
                {
                    mayat_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    userMasteruserid = table.Column<int>(type: "integer", nullable: true),
                    applicationDTLapplicationid = table.Column<string>(type: "text", nullable: true),
                    mutation_cts_no_id = table.Column<int>(type: "integer", nullable: false),
                    mobileno = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false),
                    mobilenoverified = table.Column<string>(type: "text", nullable: true),
                    emailid = table.Column<string>(type: "text", nullable: true),
                    emailidverified = table.Column<string>(type: "text", nullable: true),
                    prefixcode_marathi = table.Column<string>(type: "text", nullable: true),
                    prefix_in_marathi = table.Column<string>(type: "text", nullable: true),
                    fname_in_marathi = table.Column<string>(type: "text", nullable: true),
                    mname_in_marathi = table.Column<string>(type: "text", nullable: true),
                    lname_in_marathi = table.Column<string>(type: "text", nullable: true),
                    prefixcode_eng = table.Column<string>(type: "text", nullable: true),
                    prefix_in_eng = table.Column<string>(type: "text", nullable: true),
                    fname_in_eng = table.Column<string>(type: "text", nullable: true),
                    mname_in_eng = table.Column<string>(type: "text", nullable: true),
                    lname_in_eng = table.Column<string>(type: "text", nullable: true),
                    alias_name = table.Column<string>(type: "text", nullable: true),
                    address_type = table.Column<string>(type: "text", nullable: true),
                    address = table.Column<string>(type: "text", nullable: true),
                    state = table.Column<string>(type: "text", nullable: true),
                    district = table.Column<string>(type: "text", nullable: true),
                    taluka = table.Column<string>(type: "text", nullable: true),
                    city = table.Column<string>(type: "text", nullable: true),
                    flatno_plotno = table.Column<string>(type: "text", nullable: true),
                    societyname = table.Column<string>(type: "text", nullable: true),
                    mainstreet = table.Column<string>(type: "text", nullable: true),
                    landmark = table.Column<string>(type: "text", nullable: true),
                    locality = table.Column<string>(type: "text", nullable: true),
                    pincode = table.Column<string>(type: "text", nullable: true),
                    post_office_name = table.Column<string>(type: "text", nullable: false),
                    city_servey_no = table.Column<string>(type: "text", nullable: true),
                    lr_property_id = table.Column<string>(type: "text", nullable: true),
                    milkat = table.Column<string>(type: "text", nullable: true),
                    namud = table.Column<string>(type: "text", nullable: true),
                    sub_property_no = table.Column<string>(type: "text", nullable: true, defaultValue: "999999"),
                    mutation_srno = table.Column<string>(type: "text", nullable: true, defaultValue: "NA"),
                    owner_number = table.Column<string>(type: "text", nullable: true, defaultValue: "NA"),
                    cts_number = table.Column<string>(type: "text", nullable: true, defaultValue: "NA"),
                    actual_area = table.Column<string>(type: "text", nullable: true),
                    mrutyu_date = table.Column<string>(type: "text", nullable: true),
                    certificate_authority_code = table.Column<string>(type: "text", nullable: true),
                    certificate_authority_name = table.Column<string>(type: "text", nullable: true),
                    mrutyucert_no = table.Column<string>(type: "text", nullable: true),
                    mrutyu_certificate_date = table.Column<string>(type: "text", nullable: true),
                    mrutyu_certificate__name = table.Column<string>(type: "text", nullable: true),
                    mrutyu_certificate_path = table.Column<string>(type: "text", nullable: true),
                    is_name_same = table.Column<string>(type: "text", nullable: true),
                    reason = table.Column<string>(type: "text", nullable: true),
                    namecorrect_docname = table.Column<string>(type: "text", nullable: true),
                    namecorrect_docpath = table.Column<string>(type: "text", nullable: true),
                    address_proof_document_name = table.Column<string>(type: "text", nullable: true),
                    address_proof_document_path = table.Column<string>(type: "text", nullable: true),
                    signed_file_name = table.Column<string>(type: "text", nullable: true),
                    signed_file_path = table.Column<string>(type: "text", nullable: true),
                    createddatetime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "current_timestamp"),
                    isDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    deleteddate = table.Column<DateOnly>(type: "date", nullable: false, defaultValueSql: "1900-01-01")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mayatdtl", x => x.mayat_id);
                    table.ForeignKey(
                        name: "FK_mayatdtl_applicationdtl_applicationDTLapplicationid",
                        column: x => x.applicationDTLapplicationid,
                        principalTable: "applicationdtl",
                        principalColumn: "applicationid");
                    table.ForeignKey(
                        name: "FK_mayatdtl_usermaster_userMasteruserid",
                        column: x => x.userMasteruserid,
                        principalTable: "usermaster",
                        principalColumn: "userid");
                });

            migrationBuilder.CreateTable(
                name: "mutation_cts_no_dtl",
                columns: table => new
                {
                    mutation_cts_no_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    userMasteruserid = table.Column<int>(type: "integer", nullable: true),
                    applicationDTLapplicationid = table.Column<string>(type: "text", nullable: true),
                    what_is_mentioned_in_the_doc = table.Column<string>(type: "text", nullable: false),
                    village_or_peth_code = table.Column<string>(type: "text", nullable: false),
                    village_or_peth_name = table.Column<string>(type: "text", nullable: false),
                    village_lgd_code = table.Column<string>(type: "text", nullable: true),
                    village_english_name = table.Column<string>(type: "text", nullable: true),
                    zone_code = table.Column<string>(type: "text", nullable: true),
                    amount = table.Column<string>(type: "text", nullable: true),
                    mutation_modification_type = table.Column<string>(type: "text", nullable: false),
                    city_servey_no_mentioned_in_application = table.Column<string>(type: "text", nullable: false),
                    servey_no = table.Column<string>(type: "text", nullable: true),
                    selected_city_servey_no = table.Column<string>(type: "text", nullable: false),
                    lr_property_uid = table.Column<string>(type: "text", nullable: false),
                    application_income_type = table.Column<string>(type: "text", nullable: false),
                    city_servey_area_in_sq_m = table.Column<string>(type: "text", nullable: false),
                    building_name = table.Column<string>(type: "text", nullable: true),
                    floor_type = table.Column<int>(type: "integer", nullable: true),
                    floor_desc = table.Column<string>(type: "text", nullable: true),
                    floor_order_by = table.Column<int>(type: "integer", nullable: true),
                    floor_no = table.Column<string>(type: "text", nullable: true),
                    unit_code_156 = table.Column<int>(type: "integer", nullable: true),
                    unit_name_156 = table.Column<string>(type: "text", nullable: true),
                    unit_no = table.Column<string>(type: "text", nullable: true),
                    buildup_area_in_sq_m = table.Column<string>(type: "text", nullable: true),
                    carpet_area_in_sq_m = table.Column<string>(type: "text", nullable: true),
                    terrace_area_in_sq_m = table.Column<string>(type: "text", nullable: true),
                    parking_no = table.Column<string>(type: "text", nullable: true),
                    parking_area_in_sq_m = table.Column<string>(type: "text", nullable: true),
                    shares_in_percent = table.Column<string>(type: "text", nullable: true),
                    nic_flat_details = table.Column<string>(type: "text", nullable: true),
                    flat_bulit_up_area = table.Column<string>(type: "text", nullable: true),
                    sub_property_id = table.Column<string>(type: "text", nullable: true, defaultValue: "999999"),
                    createddatetime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "current_timestamp"),
                    isDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    deleteddate = table.Column<DateOnly>(type: "date", nullable: false, defaultValueSql: "1900-01-01")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mutation_cts_no_dtl", x => x.mutation_cts_no_id);
                    table.CheckConstraint("CHK_mutation_modification_type", "(mutation_modification_type='LAND'OR mutation_modification_type='FLAT')");
                    table.CheckConstraint("CHK_what_is_mentioned_in_the_doc", "(what_is_mentioned_in_the_doc='CITY SERVEY NO' OR what_is_mentioned_in_the_doc='SERVEYNO / GROUPNO')");
                    table.ForeignKey(
                        name: "FK_mutation_cts_no_dtl_applicationdtl_applicationDTLapplicatio~",
                        column: x => x.applicationDTLapplicationid,
                        principalTable: "applicationdtl",
                        principalColumn: "applicationid");
                    table.ForeignKey(
                        name: "FK_mutation_cts_no_dtl_usermaster_userMasteruserid",
                        column: x => x.userMasteruserid,
                        principalTable: "usermaster",
                        principalColumn: "userid");
                });

            migrationBuilder.CreateTable(
                name: "mutationgivertakerDtls",
                columns: table => new
                {
                    mutation_givertaker_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    userMasteruserid = table.Column<int>(type: "integer", nullable: true),
                    applicationDTLapplicationid = table.Column<string>(type: "text", nullable: true),
                    mutation_cts_no_id = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    user_type_code = table.Column<int>(type: "integer", nullable: false),
                    user_type = table.Column<string>(type: "text", nullable: true),
                    prop_typepropertytypeid = table.Column<int>(type: "integer", nullable: true),
                    isTaker = table.Column<int>(type: "integer", nullable: false),
                    mobileno = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false),
                    mobilenoverified = table.Column<string>(type: "text", nullable: true),
                    emailid = table.Column<string>(type: "text", nullable: true),
                    emailidverified = table.Column<string>(type: "text", nullable: true),
                    prefixcode_marathi = table.Column<string>(type: "text", nullable: false),
                    prefix_in_marathi = table.Column<string>(type: "text", nullable: true),
                    fname_in_marathi = table.Column<string>(type: "text", nullable: true),
                    mname_in_marathi = table.Column<string>(type: "text", nullable: true),
                    lname_in_marathi = table.Column<string>(type: "text", nullable: true),
                    prefixcode_eng = table.Column<string>(type: "text", nullable: true),
                    prefix_in_eng = table.Column<string>(type: "text", nullable: true),
                    fname_in_eng = table.Column<string>(type: "text", nullable: true),
                    mname_in_eng = table.Column<string>(type: "text", nullable: true),
                    lname_in_eng = table.Column<string>(type: "text", nullable: true),
                    alias_name = table.Column<string>(type: "text", nullable: true),
                    company_name_in_marathi = table.Column<string>(type: "text", nullable: true),
                    company_name_in_eng = table.Column<string>(type: "text", nullable: true),
                    gender_code = table.Column<string>(type: "text", nullable: true),
                    gender_description = table.Column<string>(type: "text", nullable: true),
                    holder_type = table.Column<string>(type: "text", nullable: true),
                    dob = table.Column<string>(type: "text", nullable: true),
                    mother_name_in_marathi = table.Column<string>(type: "text", nullable: true),
                    mother_name_in_eng = table.Column<string>(type: "text", nullable: true),
                    userName = table.Column<string>(type: "text", nullable: true),
                    city_servey_no = table.Column<string>(type: "text", nullable: true),
                    lr_property_id = table.Column<string>(type: "text", nullable: true),
                    sub_property_no = table.Column<string>(type: "text", nullable: true),
                    sellerid = table.Column<string>(type: "text", nullable: true, defaultValue: "NA"),
                    buyerid = table.Column<string>(type: "text", nullable: true, defaultValue: "NA"),
                    mutation_srno = table.Column<string>(type: "text", nullable: true, defaultValue: "NA"),
                    owner_number = table.Column<string>(type: "text", nullable: true, defaultValue: "NA"),
                    cts_number = table.Column<string>(type: "text", nullable: true, defaultValue: "NA"),
                    milkat = table.Column<string>(type: "text", nullable: true),
                    namud = table.Column<string>(type: "text", nullable: true),
                    isFullAreaGiven = table.Column<string>(type: "text", nullable: true),
                    actual_area = table.Column<string>(type: "text", nullable: true),
                    available_area = table.Column<string>(type: "text", nullable: true),
                    mutation_area = table.Column<string>(type: "text", nullable: true),
                    address_type = table.Column<string>(type: "text", nullable: true),
                    address = table.Column<string>(type: "text", nullable: true),
                    state = table.Column<string>(type: "text", nullable: true),
                    district = table.Column<string>(type: "text", nullable: true),
                    taluka = table.Column<string>(type: "text", nullable: true),
                    city = table.Column<string>(type: "text", nullable: true),
                    flatno_plotno = table.Column<string>(type: "text", nullable: true),
                    societyname = table.Column<string>(type: "text", nullable: true),
                    mainstreet = table.Column<string>(type: "text", nullable: true),
                    landmark = table.Column<string>(type: "text", nullable: true),
                    locality = table.Column<string>(type: "text", nullable: true),
                    pincode = table.Column<string>(type: "text", nullable: true),
                    post_office_name = table.Column<string>(type: "text", nullable: false),
                    address_proof_document_name = table.Column<string>(type: "text", nullable: true),
                    address_proof_document_path = table.Column<string>(type: "text", nullable: true),
                    has_property = table.Column<string>(type: "text", nullable: true),
                    aapak = table.Column<string>(type: "text", nullable: true),
                    land_buy_area = table.Column<string>(type: "text", nullable: true),
                    account_type_code = table.Column<int>(type: "integer", nullable: true),
                    account_type_description = table.Column<string>(type: "text", nullable: true),
                    apk_code = table.Column<int>(type: "integer", nullable: true),
                    apk_description = table.Column<string>(type: "text", nullable: true),
                    khata_type_code = table.Column<string>(type: "text", nullable: true),
                    khata_type_name = table.Column<string>(type: "text", nullable: true),
                    owner_status_code = table.Column<string>(type: "text", nullable: true),
                    owner_status_description = table.Column<string>(type: "text", nullable: true),
                    khatano = table.Column<string>(type: "text", nullable: true),
                    ulpin = table.Column<string>(type: "text", nullable: true),
                    district_code = table.Column<string>(type: "text", nullable: true),
                    district_name_in_marathi = table.Column<string>(type: "text", nullable: true),
                    district_name_in_eng = table.Column<string>(type: "text", nullable: true),
                    village_code = table.Column<string>(type: "text", nullable: true),
                    village_name = table.Column<string>(type: "text", nullable: true),
                    ofc_code = table.Column<string>(type: "text", nullable: true),
                    ofc_name = table.Column<string>(type: "text", nullable: true),
                    relation_code = table.Column<int>(type: "integer", nullable: false),
                    relation_name = table.Column<string>(type: "text", nullable: true),
                    varas_relation_code = table.Column<int>(type: "integer", nullable: false),
                    varas_relation_name = table.Column<string>(type: "text", nullable: true),
                    institute_code = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    institute_description = table.Column<string>(type: "text", nullable: true, defaultValue: "NA"),
                    bank_name_in_marathi = table.Column<string>(type: "text", nullable: true, defaultValue: "NA"),
                    bank_name_in_english = table.Column<string>(type: "text", nullable: true, defaultValue: "NA"),
                    ifsc = table.Column<string>(type: "text", nullable: true, defaultValue: "NA"),
                    boja_value = table.Column<string>(type: "text", nullable: true, defaultValue: "NA"),
                    boja_date = table.Column<string>(type: "text", nullable: true, defaultValue: "NA"),
                    boja_period = table.Column<string>(type: "text", nullable: true, defaultValue: "NA"),
                    benefit_amt = table.Column<string>(type: "text", nullable: true, defaultValue: "NA"),
                    isDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    deleteddate = table.Column<DateOnly>(type: "date", nullable: false, defaultValueSql: "1900-01-01"),
                    createddatetime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "current_timestamp"),
                    signed_file_name = table.Column<string>(type: "text", nullable: true),
                    signed_file_path = table.Column<string>(type: "text", nullable: true),
                    profile_pic_file_name = table.Column<string>(type: "text", nullable: true),
                    profile_pic_file_path = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mutationgivertakerDtls", x => x.mutation_givertaker_id);
                    table.ForeignKey(
                        name: "FK_mutationgivertakerDtls_applicationdtl_applicationDTLapplica~",
                        column: x => x.applicationDTLapplicationid,
                        principalTable: "applicationdtl",
                        principalColumn: "applicationid");
                    table.ForeignKey(
                        name: "FK_mutationgivertakerDtls_propertytypemaster_prop_typeproperty~",
                        column: x => x.prop_typepropertytypeid,
                        principalTable: "propertytypemaster",
                        principalColumn: "propertytypeid");
                    table.ForeignKey(
                        name: "FK_mutationgivertakerDtls_usermaster_userMasteruserid",
                        column: x => x.userMasteruserid,
                        principalTable: "usermaster",
                        principalColumn: "userid");
                });

            migrationBuilder.CreateTable(
                name: "power_of_attorney_information",
                columns: table => new
                {
                    power_of_attorney_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    userMasteruserid = table.Column<int>(type: "integer", nullable: true),
                    applicationDTLapplicationid = table.Column<string>(type: "text", nullable: true),
                    power_of_attorney_code = table.Column<int>(type: "integer", nullable: false),
                    is_taker = table.Column<bool>(type: "boolean", nullable: false),
                    usertype_code = table.Column<int>(type: "integer", nullable: false),
                    usertype = table.Column<string>(type: "text", nullable: false),
                    mutation_id = table.Column<int>(type: "integer", nullable: false),
                    mobileno = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false),
                    mobilenoverified = table.Column<string>(type: "text", nullable: false),
                    emailid = table.Column<string>(type: "text", nullable: false),
                    emailidverified = table.Column<string>(type: "text", nullable: false),
                    prefixcode_marathi = table.Column<string>(type: "text", nullable: false),
                    prefix_in_marathi = table.Column<string>(type: "text", nullable: false),
                    fname_in_marathi = table.Column<string>(type: "text", nullable: false),
                    mname_in_marathi = table.Column<string>(type: "text", nullable: false),
                    lname_in_marathi = table.Column<string>(type: "text", nullable: false),
                    prefixcode_eng = table.Column<string>(type: "text", nullable: false),
                    prefix_in_eng = table.Column<string>(type: "text", nullable: false),
                    fname_in_eng = table.Column<string>(type: "text", nullable: false),
                    mname_in_eng = table.Column<string>(type: "text", nullable: false),
                    lname_in_eng = table.Column<string>(type: "text", nullable: false),
                    company_name_in_marathi = table.Column<string>(type: "text", nullable: false),
                    company_name_in_eng = table.Column<string>(type: "text", nullable: false),
                    username = table.Column<string>(type: "text", nullable: false),
                    alias_name = table.Column<string>(type: "text", nullable: false),
                    gender_code = table.Column<string>(type: "text", nullable: false),
                    gender_description = table.Column<string>(type: "text", nullable: false),
                    dob = table.Column<string>(type: "text", nullable: false),
                    mother_name_in_marathi = table.Column<string>(type: "text", nullable: false),
                    mother_name_in_eng = table.Column<string>(type: "text", nullable: false),
                    address_type = table.Column<string>(type: "text", nullable: false),
                    address = table.Column<string>(type: "text", nullable: false),
                    state = table.Column<string>(type: "text", nullable: false),
                    district = table.Column<string>(type: "text", nullable: false),
                    taluka = table.Column<string>(type: "text", nullable: false),
                    city = table.Column<string>(type: "text", nullable: false),
                    flatno_plotno = table.Column<string>(type: "text", nullable: false),
                    societyname = table.Column<string>(type: "text", nullable: false),
                    mainstreet = table.Column<string>(type: "text", nullable: false),
                    landmark = table.Column<string>(type: "text", nullable: false),
                    locality = table.Column<string>(type: "text", nullable: false),
                    pincode = table.Column<string>(type: "text", nullable: false),
                    postofficename = table.Column<string>(type: "text", nullable: false),
                    address_proof_document_name = table.Column<string>(type: "text", nullable: false),
                    address_proof_document_path = table.Column<string>(type: "text", nullable: false),
                    city_servey_no = table.Column<string>(type: "text", nullable: false),
                    lr_property_id = table.Column<string>(type: "text", nullable: false),
                    sub_property_no = table.Column<string>(type: "text", nullable: false, defaultValue: "999999"),
                    mutation_srno = table.Column<string>(type: "text", nullable: true, defaultValue: "NA"),
                    owner_number = table.Column<string>(type: "text", nullable: true, defaultValue: "NA"),
                    cts_number = table.Column<string>(type: "text", nullable: true, defaultValue: "NA"),
                    village_code = table.Column<string>(type: "text", nullable: true, defaultValue: "0"),
                    village_name = table.Column<string>(type: "text", nullable: true, defaultValue: "NA"),
                    owner_of_property_in_maharashtra = table.Column<bool>(type: "boolean", nullable: false),
                    propertytypeid = table.Column<int>(type: "integer", nullable: true),
                    property_district_code = table.Column<string>(type: "text", nullable: true),
                    property_district_name_in_marathi = table.Column<string>(type: "text", nullable: true),
                    property_district_name_in_english = table.Column<string>(type: "text", nullable: true),
                    property_taluka_code = table.Column<string>(type: "text", nullable: true),
                    property_taluka_name = table.Column<string>(type: "text", nullable: true),
                    property_city_code = table.Column<string>(type: "text", nullable: true),
                    property_city_name = table.Column<string>(type: "text", nullable: true),
                    khateno = table.Column<string>(type: "text", nullable: true),
                    ulpin = table.Column<string>(type: "text", nullable: true),
                    khata_type_code = table.Column<string>(type: "text", nullable: false),
                    khata_type_name = table.Column<string>(type: "text", nullable: false),
                    owner_status_code = table.Column<string>(type: "text", nullable: false),
                    owner_status_description = table.Column<string>(type: "text", nullable: false),
                    attornytype_code = table.Column<int>(type: "integer", nullable: false),
                    attornytype_desc = table.Column<string>(type: "text", nullable: true),
                    landBuyArea = table.Column<string>(type: "text", nullable: true),
                    isPOAisPartofDast = table.Column<string>(type: "text", nullable: false),
                    isDeclerationInvolvedInPOA = table.Column<string>(type: "text", nullable: false),
                    isPOAPermanant = table.Column<string>(type: "text", nullable: false),
                    isTransferRights = table.Column<string>(type: "text", nullable: false),
                    dast_no = table.Column<string>(type: "text", nullable: false),
                    dast_no_date = table.Column<string>(type: "text", nullable: false),
                    dast_no_year = table.Column<string>(type: "text", nullable: false),
                    isDastVerified = table.Column<bool>(type: "boolean", nullable: false),
                    verifieddastData = table.Column<string>(type: "text", nullable: false),
                    digcode = table.Column<int>(type: "integer", nullable: true),
                    digname = table.Column<string>(type: "text", nullable: false),
                    poa_district_code = table.Column<string>(type: "text", nullable: false),
                    poa_district_name = table.Column<string>(type: "text", nullable: false),
                    sro_office_code = table.Column<int>(type: "integer", nullable: false),
                    sro_office_name = table.Column<string>(type: "text", nullable: false),
                    createddatetime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "current_timestamp"),
                    isDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    deleteddate = table.Column<DateOnly>(type: "date", nullable: false, defaultValueSql: "1900-01-01"),
                    signed_file_path = table.Column<string>(type: "text", nullable: false),
                    signed_file_name = table.Column<string>(type: "text", nullable: false),
                    profile_pic_file_name = table.Column<string>(type: "text", nullable: false),
                    profile_pic_file_path = table.Column<string>(type: "text", nullable: false),
                    poa_giver_ids = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_power_of_attorney_information", x => x.power_of_attorney_id);
                    table.CheckConstraint("CK_Address_Type", "(address_type='INDIA'OR address_type='FOREIGN')");
                    table.ForeignKey(
                        name: "FK_power_of_attorney_information_applicationdtl_applicationDTL~",
                        column: x => x.applicationDTLapplicationid,
                        principalTable: "applicationdtl",
                        principalColumn: "applicationid");
                    table.ForeignKey(
                        name: "FK_power_of_attorney_information_propertytypemaster_propertyty~",
                        column: x => x.propertytypeid,
                        principalTable: "propertytypemaster",
                        principalColumn: "propertytypeid");
                    table.ForeignKey(
                        name: "FK_power_of_attorney_information_usermaster_userMasteruserid",
                        column: x => x.userMasteruserid,
                        principalTable: "usermaster",
                        principalColumn: "userid");
                });

            migrationBuilder.CreateTable(
                name: "uploaded_documents_dtl",
                columns: table => new
                {
                    uploaded_doc_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    userMasteruserid = table.Column<int>(type: "integer", nullable: true),
                    applicationDTLapplicationid = table.Column<string>(type: "text", nullable: true),
                    document_type_code = table.Column<string>(type: "text", nullable: true),
                    document_type = table.Column<string>(type: "text", nullable: true),
                    city_servey_no = table.Column<string>(type: "text", nullable: true),
                    document_name = table.Column<string>(type: "text", nullable: true),
                    document_path = table.Column<string>(type: "text", nullable: true),
                    createddatetime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "current_timestamp"),
                    isDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    deleteddate = table.Column<DateOnly>(type: "date", nullable: false, defaultValueSql: "1900-01-01")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_uploaded_documents_dtl", x => x.uploaded_doc_id);
                    table.ForeignKey(
                        name: "FK_uploaded_documents_dtl_applicationdtl_applicationDTLapplica~",
                        column: x => x.applicationDTLapplicationid,
                        principalTable: "applicationdtl",
                        principalColumn: "applicationid");
                    table.ForeignKey(
                        name: "FK_uploaded_documents_dtl_usermaster_userMasteruserid",
                        column: x => x.userMasteruserid,
                        principalTable: "usermaster",
                        principalColumn: "userid");
                });

            migrationBuilder.CreateIndex(
                name: "IX_applicantmaster_applicationDTLapplicationid",
                table: "applicantmaster",
                column: "applicationDTLapplicationid");

            migrationBuilder.CreateIndex(
                name: "IX_applicantmaster_PropertyTypeMasterpropertytypeid",
                table: "applicantmaster",
                column: "PropertyTypeMasterpropertytypeid");

            migrationBuilder.CreateIndex(
                name: "IX_applicantmaster_userMasteruserid",
                table: "applicantmaster",
                column: "userMasteruserid");

            migrationBuilder.CreateIndex(
                name: "IX_applicationdtl_applicationTypeMasterapplicationtypeid",
                table: "applicationdtl",
                column: "applicationTypeMasterapplicationtypeid");

            migrationBuilder.CreateIndex(
                name: "IX_applicationdtl_userMasteruserid",
                table: "applicationdtl",
                column: "userMasteruserid");

            migrationBuilder.CreateIndex(
                name: "IX_court_claim_information_applicationDTLapplicationid",
                table: "court_claim_information",
                column: "applicationDTLapplicationid");

            migrationBuilder.CreateIndex(
                name: "IX_court_claim_information_userMasteruserid",
                table: "court_claim_information",
                column: "userMasteruserid");

            migrationBuilder.CreateIndex(
                name: "IX_dast_information_applicationDTLapplicationid",
                table: "dast_information",
                column: "applicationDTLapplicationid");

            migrationBuilder.CreateIndex(
                name: "IX_dast_information_userMasteruserid",
                table: "dast_information",
                column: "userMasteruserid");

            migrationBuilder.CreateIndex(
                name: "IX_labelmaster_mutationTypeMastermutationid",
                table: "labelmaster",
                column: "mutationTypeMastermutationid");

            migrationBuilder.CreateIndex(
                name: "IX_labelmaster_screenMasterscreenid",
                table: "labelmaster",
                column: "screenMasterscreenid");

            migrationBuilder.CreateIndex(
                name: "IX_mayatdtl_applicationDTLapplicationid",
                table: "mayatdtl",
                column: "applicationDTLapplicationid");

            migrationBuilder.CreateIndex(
                name: "IX_mayatdtl_userMasteruserid",
                table: "mayatdtl",
                column: "userMasteruserid");

            migrationBuilder.CreateIndex(
                name: "IX_mutation_cts_no_dtl_applicationDTLapplicationid",
                table: "mutation_cts_no_dtl",
                column: "applicationDTLapplicationid");

            migrationBuilder.CreateIndex(
                name: "IX_mutation_cts_no_dtl_userMasteruserid",
                table: "mutation_cts_no_dtl",
                column: "userMasteruserid");

            migrationBuilder.CreateIndex(
                name: "IX_mutationgivertakerDtls_applicationDTLapplicationid",
                table: "mutationgivertakerDtls",
                column: "applicationDTLapplicationid");

            migrationBuilder.CreateIndex(
                name: "IX_mutationgivertakerDtls_prop_typepropertytypeid",
                table: "mutationgivertakerDtls",
                column: "prop_typepropertytypeid");

            migrationBuilder.CreateIndex(
                name: "IX_mutationgivertakerDtls_userMasteruserid",
                table: "mutationgivertakerDtls",
                column: "userMasteruserid");

            migrationBuilder.CreateIndex(
                name: "IX_mutationmaster_applicationtypeid",
                table: "mutationmaster",
                column: "applicationtypeid");

            migrationBuilder.CreateIndex(
                name: "IX_mutationtypemaster_mutationtype",
                table: "mutationtypemaster",
                column: "mutationtype",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_power_of_attorney_information_applicationDTLapplicationid",
                table: "power_of_attorney_information",
                column: "applicationDTLapplicationid");

            migrationBuilder.CreateIndex(
                name: "IX_power_of_attorney_information_propertytypeid",
                table: "power_of_attorney_information",
                column: "propertytypeid");

            migrationBuilder.CreateIndex(
                name: "IX_power_of_attorney_information_userMasteruserid",
                table: "power_of_attorney_information",
                column: "userMasteruserid");

            migrationBuilder.CreateIndex(
                name: "IX_propertytypemaster_propertytype",
                table: "propertytypemaster",
                column: "propertytype",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_uploaded_documents_dtl_applicationDTLapplicationid",
                table: "uploaded_documents_dtl",
                column: "applicationDTLapplicationid");

            migrationBuilder.CreateIndex(
                name: "IX_uploaded_documents_dtl_userMasteruserid",
                table: "uploaded_documents_dtl",
                column: "userMasteruserid");

            migrationBuilder.CreateIndex(
                name: "IX_usermaster_PropertyTypeMasterpropertytypeid",
                table: "usermaster",
                column: "PropertyTypeMasterpropertytypeid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "applicantmaster");

            migrationBuilder.DropTable(
                name: "court_claim_information");

            migrationBuilder.DropTable(
                name: "dast_information");

            migrationBuilder.DropTable(
                name: "document_type_master");

            migrationBuilder.DropTable(
                name: "labelmaster");

            migrationBuilder.DropTable(
                name: "mailid_and_mobileno_verification");

            migrationBuilder.DropTable(
                name: "mayatdtl");

            migrationBuilder.DropTable(
                name: "mutation_cts_no_dtl");

            migrationBuilder.DropTable(
                name: "mutationgivertakerDtls");

            migrationBuilder.DropTable(
                name: "mutationmaster");

            migrationBuilder.DropTable(
                name: "nic_api_response");

            migrationBuilder.DropTable(
                name: "power_of_attorney_information");

            migrationBuilder.DropTable(
                name: "uploaded_documents_dtl");

            migrationBuilder.DropTable(
                name: "mutationtypemaster");

            migrationBuilder.DropTable(
                name: "screenmaster");

            migrationBuilder.DropTable(
                name: "applicationdtl");

            migrationBuilder.DropTable(
                name: "applicationtypemaster");

            migrationBuilder.DropTable(
                name: "usermaster");

            migrationBuilder.DropTable(
                name: "propertytypemaster");
        }
    }
}
