using AutoMapper;
using PDEWebAPIS.Helpers;
using PDEWebAPIS.Model;
using PDEWebAPIS.Repository;

public class BhadepattaToMutationGiverTakerDTLProfile : Profile
{
    public BhadepattaToMutationGiverTakerDTLProfile()
    {
        CreateMap<BhadepattaModel, MutationGiverTakerDTL>()

            .MapNA(dest => dest.address_proof_document_path!)
             //.MapNA(dest => dest.user_type!)
             //.MapZero(dest=>dest.varas_relation_code)
             //.MapNA(dest => dest.varas_relation_name!)
             .MapZero(dest => dest.institute_code)
            .MapNA(dest => dest.institute_description!)
            .MapNA(dest => dest.bank_name_in_english!)
            .MapNA(dest => dest.bank_name_in_marathi!)
            .MapNA(dest => dest.profile_pic_file_name!)
            .MapNA(dest => dest.profile_pic_file_path!)


            .ForMember(dest => dest.user_type, opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.usertype) ? "NA" : src.usertype))
            .ForMember(dest => dest.user_type_code, opt => opt.MapFrom(src => src.usertype_code == 0 ? 0 : src.usertype_code))

            .ForMember(dest => dest.userMaster, opt => opt.MapFrom(src => src.userMaster))
            .ForMember(dest => dest.applicationDTL, opt => opt.MapFrom(src => src.applicationDTL))
            .ForMember(dest => dest.relation_code, opt => opt.MapFrom(src => src.relation_code == 0 ? 0 : src.relation_code))

            .ForMember(dest => dest.relation_name, opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.relation_name) ? "NA" : src.relation_name))

            .ForMember(dest => dest.mobileno,
                opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.mobile) ? "NA" : src.mobile))
            .ForMember(dest => dest.mobileno,
                opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.mobileno) ? "NA" : src.mobileno))
            .ForMember(dest => dest.mobilenoverified,
                opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.mobilenoverified) ? "NA" : src.mobilenoverified))
            .ForMember(dest => dest.emailid,
                opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.email) ? "NA" : src.email))
            .ForMember(dest => dest.emailid,
                opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.emailid) ? "NA" : src.emailid))
            .ForMember(dest => dest.emailidverified,
                opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.emailidverified) ? "NA" : src.emailidverified))
            .ForMember(dest => dest.fname_in_marathi,
                opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.firstName) ? "NA" : src.firstName))
            .ForMember(dest => dest.mname_in_marathi,
                opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.middleName) ? "NA" : src.middleName))
            .ForMember(dest => dest.lname_in_marathi,
                opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.lastName) ? "NA" : src.lastName))
            .ForMember(dest => dest.fname_in_eng,
                opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.firstNameEng) ? "NA" : src.firstNameEng))
            .ForMember(dest => dest.mname_in_eng,
                opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.middleNameEng) ? "NA" : src.middleNameEng))
            .ForMember(dest => dest.lname_in_eng,
                opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.lastNameEng) ? "NA" : src.lastNameEng))
            .ForMember(dest => dest.gender_code,
                opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.gender_code) ? "NA" : src.gender_code))
            .ForMember(dest => dest.gender_description,
                opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.gender_description) ? "NA" : src.gender_description))
            .ForMember(dest => dest.holder_type,
                opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.holderType) ? "NA" : src.holderType))

            .ForMember(dest => dest.isFullAreaGiven,
                opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.isFullAreaGiven) ? "NA" : src.isFullAreaGiven))
             .ForMember(dest => dest.actual_area,
                opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.actualArea) ? "NA" : src.actualArea))
             .ForMember(dest => dest.available_area,
                opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.availableArea) ? "NA" : src.availableArea))
             .ForMember(dest => dest.mutation_area,
                opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.mutationArea) ? "NA" : src.mutationArea))

             .ForMember(dest => dest.address_type,
                opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.addressType) ? "NA" : src.addressType!.Trim().ToUpper()))
             .ForMember(dest => dest.address,
                opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.address) ? "NA" : src.address))
             .ForMember(dest => dest.state,
                opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.state) ? "NA" : src.state))
             .ForMember(dest => dest.district,
                opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.district) ? "NA" : src.district))
             .ForMember(dest => dest.taluka,
                opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.taluka) ? "NA" : src.taluka))
             .ForMember(dest => dest.city,
                opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.state) ? "NA" : src.city))
             .ForMember(dest => dest.pincode,
                opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.pincode) ? "NA" : src.pincode))

             .ForMember(dest => dest.has_property,
                opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.hasProperty) ? "NA" : src.hasProperty))
             .ForMember(dest => dest.aapak,
                opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.aapak) ? "NA" : src.aapak))
              .ForMember(dest => dest.land_buy_area,
                opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.landBuyArea) ? "NA" : src.landBuyArea))

              .ForMember(dest => dest.account_type_code,
                opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.account_type_code.ToString()) ? 0 : src.account_type_code))

              .ForMember(dest => dest.apk_code,
                opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.apk_code.ToString()) ? 1 : src.apk_code))
              .ForMember(dest => dest.apk_description,
                opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.apk_description) ? "स्वतः" : src.apk_description))
               .ForMember(dest => dest.khata_type_code,
                opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.khataCode) ? "NA" : src.khataCode))
                .ForMember(dest => dest.khata_type_name,
                opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.khataLabel) ? "NA" : src.khataLabel))
                 .ForMember(dest => dest.company_name_in_eng,
                opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.companyNameEng) ? "NA" : src.companyNameEng))
                .ForMember(dest => dest.company_name_in_marathi,
                opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.companyName) ? "NA" : src.companyName))

                 .ForMember(dest => dest.owner_status_code,
                opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.owner_status_code) ? "NA" : src.owner_status_code))
                  .ForMember(dest => dest.owner_status_description,
                opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.owner_status_description) ? "NA" : src.owner_status_description))
                   .ForMember(dest => dest.khatano,
                opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.Khatano) ? "NA" : src.Khatano))
                .ForMember(dest => dest.ulpin,
                opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.ulpin) ? "NA" : src.ulpin))
                 .ForMember(dest => dest.district_code,
                opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.district_code) ? "NA" : src.district_code))
                  .ForMember(dest => dest.ulpin,
                opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.ulpin) ? "NA" : src.ulpin))
                .ForMember(dest => dest.district_name_in_eng,
                opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.district_name_in_eng) ? "NA" : src.district_name_in_eng))
                 .ForMember(dest => dest.district_name_in_marathi,
                opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.district_name_in_marathi) ? "NA" : src.district_name_in_marathi))
                .ForMember(dest => dest.account_type_description,
                opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.account_type_description) ? "NA" : src.account_type_description))

                .ForMember(dest => dest.village_name,
                opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.village_name) ? "NA" : src.village_name))
                    .ForMember(dest => dest.ofc_code,
                opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.office_code) ? "NA" : src.office_code))
                    .ForMember(dest => dest.ofc_name,
                opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.office_name) ? "NA" : src.office_name))
                        .ForMember(dest => dest.relation_code,
                opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.relation_code.ToString()) ? 0 : src.relation_code))
               .ForMember(dest => dest.relation_name,
                opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.relation_name) ? "NA" : src.relation_name))


               .ForMember(dest => dest.relation_name,
                opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.relation_name) ? "NA" : src.relation_name))

             .ForMember(dest => dest.post_office_name,
                opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.postOfficeName) ? "NA" : src.postOfficeName))

            .ForMember(dest => dest.cts_number,
                opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.nabhu) ? "NA" : src.nabhu))
             .ForMember(dest => dest.city_servey_no,
                opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.nabhu) ? "NA" : src.nabhu))
            .ForMember(dest => dest.mutation_srno,
                opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.mutationSroNo) ? "NA" : src.mutationSroNo))
            .ForMember(dest => dest.owner_number,
                opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.ownerNo) ? "NA" : src.ownerNo))

            .ForMember(dest => dest.prefix_in_marathi,
                opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.suffix) ? "NA" : src.suffix))

            .ForMember(dest => dest.prefix_in_eng,
                opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.suffixEng) ? "NA" : src.suffixEng))

            .ForMember(dest => dest.prefixcode_marathi,
                opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.suffixcode) ? "NA" : src.suffixcode))

            .ForMember(dest => dest.prefixcode_eng,
                opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.suffixCodeEng) ? "NA" : src.suffixCodeEng))

            .ForMember(dest => dest.alias_name,
                opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.aliceName) ? "NA" : src.aliceName))

            .ForMember(dest => dest.mother_name_in_marathi,
                opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.motherName) ? "NA" : src.motherName))

            .ForMember(dest => dest.mother_name_in_eng,
                opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.motherNameEng) ? "NA" : src.motherNameEng))

            .ForMember(dest => dest.lr_property_id,
                opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.lrPropertyUID) ? "NA" : src.lrPropertyUID))

            .ForMember(dest => dest.sub_property_no,
                opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.subPropNo) ? "NA" : src.subPropNo))

            .ForMember(dest => dest.flatno_plotno,
                opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.plotNo) ? "NA" : src.plotNo))

            .ForMember(dest => dest.societyname,
                opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.building) ? "NA" : src.building))

            .ForMember(dest => dest.mainstreet,
                opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.mainRoad) ? "NA" : src.mainRoad))

            .ForMember(dest => dest.landmark,
                opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.impSymbol) ? "NA" : src.impSymbol))

            .ForMember(dest => dest.locality,
                opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.area) ? "NA" : src.area))

            .ForMember(dest => dest.address_proof_document_name,
                opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.addressProofName) ? "NA" : src.addressProofName))

            .ForMember(dest => dest.signed_file_name,
                opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.signatureName) ? "NA" : src.signatureName))

            .ForMember(dest => dest.signed_file_path,
                opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.signatureSrc) ? "NA" : src.signatureSrc))

            .ForMember(dest => dest.owner_village_code,
                opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.owner_village_code) ? "NA" : src.owner_village_code))

              .ForMember(dest => dest.dob,
                opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.dob) ? "NA" : src.dob));


    }
}
