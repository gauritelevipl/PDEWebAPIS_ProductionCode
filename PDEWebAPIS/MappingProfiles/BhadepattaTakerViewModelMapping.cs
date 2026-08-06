using PDEWebAPIS.InputDataModel;
using PDEWebAPIS.Model;
using PDEWebAPIS.Repository;
using AutoMapper;
using Microsoft.IdentityModel.Tokens;

public class BhadepattaTakerViewModelMapping : Profile
{
    public BhadepattaTakerViewModelMapping()
    {
        CreateMap<BhadepattaTakerInputModel, BhadepattaModel>()
            .ForMember(dest => dest.usertype, opt => opt.MapFrom(src => src.usertype))
            .ForMember(dest => dest.usertype_code, opt => opt.MapFrom(src => src.usertype_code))
            .ForMember(dest => dest.applicationid, opt => opt.MapFrom(src => src.applicationid))
            .ForPath(dest => dest.userMaster!.userid, opt => opt.MapFrom(src => src.userid))
            .ForPath(dest => dest.passport_name, opt => opt.MapFrom(src => src.photo!.passportName))
            .ForPath(dest => dest.passport_src, opt => opt.MapFrom(src => src.photo!.passportSrc))
            .ForPath(dest => dest.hasProperty, opt => opt.MapFrom(src => src.isMHProperty!.hasProperty))
            .ForPath(dest => dest.propType, opt => opt.Ignore())

            //user details
            .ForMember(dest => dest.suffix, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.isMHProperty!.userDetails!.suffix) ? "NA" : src.isMHProperty!.userDetails!.suffix))
            .ForMember(dest => dest.suffixcode, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.isMHProperty!.userDetails!.suffixcode) ? "NA" : src.isMHProperty!.userDetails!.suffixcode))
            .ForMember(dest => dest.suffixCodeEng, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.isMHProperty!.userDetails!.suffixCodeEng) ? "NA" : src.isMHProperty!.userDetails!.suffixCodeEng))
            .ForMember(dest => dest.suffixEng, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.isMHProperty!.userDetails!.suffixEng) ? "NA" : src.isMHProperty!.userDetails!.suffixEng))
            .ForMember(dest => dest.firstName, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.isMHProperty!.userDetails!.firstName) ? "NA" : src.isMHProperty!.userDetails!.firstName))
            .ForMember(dest => dest.middleName, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.isMHProperty!.userDetails!.middleName) ? "NA" : src.isMHProperty!.userDetails!.middleName))
            .ForMember(dest => dest.lastName, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.isMHProperty!.userDetails!.lastName) ? "NA" : src.isMHProperty!.userDetails!.lastName))
            .ForMember(dest => dest.firstNameEng, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.isMHProperty!.userDetails!.firstNameEng) ? "NA" : src.isMHProperty!.userDetails!.firstNameEng))
            .ForMember(dest => dest.middleNameEng, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.isMHProperty!.userDetails!.middleNameEng) ? "NA" : src.isMHProperty!.userDetails!.middleNameEng))
            .ForMember(dest => dest.lastNameEng, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.isMHProperty!.userDetails!.lastNameEng) ? "NA" : src.isMHProperty!.userDetails!.lastNameEng))
            .ForMember(dest => dest.aliceName, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.dharak!.userdharak!.aliceName) ? "NA" : src.dharak!.userdharak!.aliceName))

            .ForMember(dest => dest.apk_code, opt => opt.MapFrom(src => src.dharak!.userdharak!.aapakDropdown!.apk_code == 0 ? 1 : src.dharak!.userdharak!.aapakDropdown!.apk_code))
            .ForMember(dest => dest.apk_description, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.dharak!.userdharak!.aapakDropdown!.apk_description!.ToString()) ? "स्वतः" : src.dharak!.userdharak!.aapakDropdown!.apk_description.ToString()))
            .ForMember(dest => dest.aapak, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.dharak!.userdharak!.aapak) ? "NA" : src.dharak!.userdharak!.aapak))
            .ForMember(dest => dest.relation_code, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.dharak!.userdharak!.aapakRelation!.relation_code) ? "0" : src.dharak!.userdharak!.aapakRelation!.relation_code))
            .ForMember(dest => dest.relation_name, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.dharak!.userdharak!.aapakRelation!.relation_name) ? "NA" : src.dharak!.userdharak!.aapakRelation!.relation_name))
            .ForMember(dest => dest.gender_code, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.dharak!.userdharak!.gender!.gender_code) ? "NA" : src.dharak!.userdharak!.gender!.gender_code))
            .ForMember(dest => dest.gender_description, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.dharak!.userdharak!.gender!.gender_description) ? "NA" : src.dharak!.userdharak!.gender!.gender_description))
            .ForMember(dest => dest.owner_status_code, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.dharak!.userdharak!.holderType!.owner_status_code) ? "NA" : src.dharak!.userdharak!.holderType!.owner_status_code))
            .ForMember(dest => dest.owner_status_description, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.dharak!.userdharak!.holderType!.owner_status_description) ? "NA" : src.dharak!.userdharak!.holderType!.owner_status_description))
            .ForMember(dest => dest.dob, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.dharak!.userdharak!.dob) ? "NA" : src.dharak!.userdharak!.dob))
            .ForMember(dest => dest.motherName, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.dharak!.userdharak!.motherName) ? "NA" : src.dharak!.userdharak!.motherName))
            .ForMember(dest => dest.motherNameEng, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.dharak!.userdharak!.motherNameEng) ? "NA" : src.dharak!.userdharak!.motherNameEng))
            .ForMember(dest => dest.userName, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.isMHProperty!.userDetails!.userName) ? "NA" : src.isMHProperty!.userDetails!.userName))

            //sanstha

            .ForMember(dest => dest.companyNameEng, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.isMHProperty!.userDetails!.companyNameEng) ? "NA" : src.isMHProperty!.userDetails!.companyNameEng))
            .ForMember(dest => dest.companyName, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.isMHProperty!.userDetails!.companyName) ? "NA" : src.isMHProperty!.userDetails!.companyName))

            //Address details

            .ForMember(dest => dest.addressType, opt => opt.MapFrom(src => src.address!.addressType!.Trim().ToUpper()))
            .ForMember(dest => dest.plotNo, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.address!.indiaAddress!.plotNo) ? "NA" : src.address!.indiaAddress!.plotNo))
            .ForMember(dest => dest.building, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.address!.indiaAddress!.building) ? "NA" : src.address!.indiaAddress!.building))
            .ForMember(dest => dest.mainRoad, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.address!.indiaAddress!.mainRoad) ? "NA" : src.address!.indiaAddress!.mainRoad))
            .ForMember(dest => dest.impSymbol, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.address!.indiaAddress!.impSymbol) ? "NA" : src.address!.indiaAddress!.impSymbol))
            .ForMember(dest => dest.area, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.address!.indiaAddress!.area) ? "NA" : src.address!.indiaAddress!.area))
            .AfterMap((src, dest) =>
            {
                string addressType = src.address?.addressType?.Trim().ToUpper() ?? "";
                if (addressType == "INDIA")
                {
                    dest.mobile = !string.IsNullOrEmpty(src.address?.indiaAddress?.mobile)
                    ? src.address.indiaAddress.mobile : "NA";

                    dest.mobileno = !string.IsNullOrEmpty(src.address?.indiaAddress?.mobile)
                        ? src.address.indiaAddress.mobile
                        : "NA";
                }
                else if (addressType == "FOREIGN")
                {
                    dest.mobile = !string.IsNullOrEmpty(src.address?.foreignAddress?.mobile)
                        ? src.address.foreignAddress.mobile
                        : "NA";

                    dest.mobileno = !string.IsNullOrEmpty(src.address?.foreignAddress?.mobile)
                        ? src.address.foreignAddress.mobile
                        : "NA";
                }
                else
                {
                    dest.mobile = "NA";
                    dest.mobileno = "NA";
                }
            })
            .ForMember(dest => dest.address, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.address!.foreignAddress!.address) ? "NA" : src.address!.foreignAddress!.address))
            .ForMember(dest => dest.mobileOTP, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.address!.indiaAddress!.mobileOTP) ? "NA" : src.address!.indiaAddress!.mobileOTP))
            .ForMember(dest => dest.mobilenoverified, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.address!.indiaAddress!.mobileOTP) ? "NA" : src.address!.indiaAddress!.mobileOTP))
            .ForMember(dest => dest.email, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.address!.foreignAddress!.email) ? "NA" : src.address!.foreignAddress!.email))
            .ForMember(dest => dest.emailid, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.address!.foreignAddress!.email) ? "NA" : src.address!.foreignAddress!.email))

            .ForMember(dest => dest.emailOTP, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.address!.foreignAddress!.emailOTP) ? "NA" : src.address!.foreignAddress!.emailOTP))
            .ForMember(dest => dest.emailidverified, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.address!.foreignAddress!.emailOTP) ? "NA" : src.address!.foreignAddress!.emailOTP))
            .ForMember(dest => dest.pincode, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.address!.indiaAddress!.pincode) ? "NA" : src.address!.indiaAddress!.pincode))
            .ForMember(dest => dest.postOfficeName, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.address!.indiaAddress!.postOfficeName) ? "NA" : src.address!.indiaAddress!.postOfficeName))
            .ForMember(dest => dest.city, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.address!.indiaAddress!.city) ? "NA" : src.address!.indiaAddress!.city))
            .ForMember(dest => dest.taluka, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.address!.indiaAddress!.taluka) ? "NA" : src.address!.indiaAddress!.taluka))
            .ForMember(dest => dest.district, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.address!.indiaAddress!.district) ? "NA" : src.address!.indiaAddress!.district))
            .ForMember(dest => dest.state, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.address!.indiaAddress!.state) ? "NA" : src.address!.indiaAddress!.state))
            .ForMember(dest => dest.addressProofName, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.address!.indiaAddress!.addressProofName) ? "NA" : src.address!.indiaAddress!.addressProofName))
            .ForMember(dest => dest.addressProofSrc, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.address!.indiaAddress!.addressProofSrc) ? "NA" : src.address!.indiaAddress!.addressProofSrc))
            .ForMember(dest => dest.signatureName, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.address!.indiaAddress!.signatureName) ? "NA" : src.address!.indiaAddress!.signatureName))
            .ForMember(dest => dest.signatureSrc, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.address!.indiaAddress!.signatureSrc) ? "NA" : src.address!.indiaAddress!.signatureSrc));
    }

}

