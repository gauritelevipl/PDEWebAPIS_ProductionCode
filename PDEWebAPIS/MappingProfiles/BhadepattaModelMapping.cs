using AutoMapper;
using PDEWebAPIS.InputDataModel;
using PDEWebAPIS.Model;

namespace PDEWebAPIS.MappingProfiles
{
    public class BhadepattaModelMapping : Profile
    {
        public BhadepattaModelMapping()
        {
            CreateMap<BhadepattaGiverInputModel, BhadepattaModel>()


                .ForMember(dest => dest.owner_village_code, opt => opt.MapFrom(src => src.village_code))

                //mapping user details
                .ForMember(dest => dest.firstName, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.userDetails!.firstName) ? "NA" : src.userDetails!.firstName))
                .ForMember(dest => dest.middleName, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.userDetails!.middleName) ? "NA" : src.userDetails!.middleName))
                .ForMember(dest => dest.lastName, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.userDetails!.lastName) ? "NA" : src.userDetails!.lastName))
                .ForMember(dest => dest.firstNameEng, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.userDetails!.firstNameEng) ? "NA" : src.userDetails!.firstNameEng))
                .ForMember(dest => dest.middleNameEng, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.userDetails!.middleNameEng) ? "NA" : src.userDetails!.middleNameEng))
                .ForMember(dest => dest.lastNameEng, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.userDetails!.lastNameEng) ? "NA" : src.userDetails!.lastNameEng))
                .ForMember(dest => dest.aliceName, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.userDetails!.aliceName) ? "NA" : src.userDetails!.aliceName))
                .ForMember(dest => dest.motherName, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.userDetails!.motherName) ? "NA" : src.userDetails!.motherName))
                .ForMember(dest => dest.motherNameEng, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.userDetails!.motherNameEng) ? "NA" : src.userDetails!.motherNameEng))
                .ForMember(dest => dest.userName, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.userDetails!.userName) ? "NA" : src.userDetails!.userName))
                .ForMember(dest => dest.suffix, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.userDetails!.suffix) ? "NA" : src.userDetails!.suffix))
                .ForMember(dest => dest.suffixEng, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.userDetails!.suffixEng) ? "NA" : src.userDetails!.suffixEng))
                .ForMember(dest => dest.suffixcode, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.userDetails!.suffixcode) ? "NA" : src.userDetails!.suffixcode))
                .ForMember(dest => dest.suffixCodeEng, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.userDetails!.suffixCodeEng) ? "NA" : src.userDetails!.suffixCodeEng))
                .ForMember(dest => dest.nabhu, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.userDetails!.nabhu) ? "NA" : src.userDetails!.nabhu))
                .ForMember(dest => dest.lrPropertyUID, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.userDetails!.lrPropertyUID) ? "NA" : src.userDetails!.lrPropertyUID))
                .ForMember(dest => dest.milkat, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.userDetails!.milkat) ? "NA" : src.userDetails!.milkat))
                .ForMember(dest => dest.namud, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.userDetails!.namud) ? "NA" : src.userDetails!.namud))
                .ForMember(dest => dest.holderType, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.userDetails!.holderType) ? "NA" : src.userDetails!.holderType))
                .ForMember(dest => dest.dob, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.userDetails!.dob) ? "NA" : src.userDetails!.dob))
                .ForMember(dest => dest.subPropNo, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.userDetails!.subPropNo) ? "999999" : src.userDetails!.subPropNo))

                //mapping area of mutation
                .ForMember(dest => dest.isFullAreaGiven, opt => opt.MapFrom(src => src.areaOfMutation!.isFullAreaGiven))
                .ForMember(dest => dest.actualArea, opt => opt.MapFrom(src => src.areaOfMutation!.actualArea))
                .ForMember(dest => dest.mutationArea, opt => opt.MapFrom(src => src.areaOfMutation!.mutationArea))
                .ForMember(dest => dest.availableArea, opt => opt.MapFrom(src => src.areaOfMutation!.availableArea))
                //address mapping
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
                .ForMember(dest => dest.mobilenoverified, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.address!.indiaAddress!.mobileOTP) ? "NA" : src.address!.indiaAddress!.mobileOTP))
                .ForMember(dest => dest.mobileOTP, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.address!.indiaAddress!.mobileOTP) ? "NA" : src.address!.indiaAddress!.mobileOTP))
                .ForMember(dest => dest.pincode, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.address!.indiaAddress!.pincode) ? "NA" : src.address!.indiaAddress!.pincode))
                .ForMember(dest => dest.postOfficeName, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.address!.indiaAddress!.postOfficeName) ? "NA" : src.address!.indiaAddress!.postOfficeName))
                .ForMember(dest => dest.city, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.address!.indiaAddress!.city) ? "NA" : src.address!.indiaAddress!.city))
                .ForMember(dest => dest.taluka, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.address!.indiaAddress!.taluka) ? "NA" : src.address!.indiaAddress!.taluka))
                .ForMember(dest => dest.district, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.address!.indiaAddress!.district) ? "NA" : src.address!.indiaAddress!.district))
                .ForMember(dest => dest.state, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.address!.indiaAddress!.state) ? "NA" : src.address!.indiaAddress!.state))
                .ForMember(dest => dest.addressProofName, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.address!.indiaAddress!.addressProofName) ? "NA" : src.address!.indiaAddress!.addressProofName))
                .ForMember(dest => dest.addressProofSrc, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.address!.indiaAddress!.addressProofSrc) ? "NA" : src.address!.indiaAddress!.addressProofSrc))
                .ForMember(dest => dest.signatureName, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.address!.indiaAddress!.signatureName) ? "NA" : src.address!.indiaAddress!.signatureName))
                .ForMember(dest => dest.signatureSrc, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.address!.indiaAddress!.signatureSrc) ? "NA" : src.address!.indiaAddress!.signatureSrc))
                //foreign Address
                .ForMember(dest => dest.emailid, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.address!.foreignAddress!.email) ? "NA" : src.address!.foreignAddress!.email))
                .ForMember(dest => dest.email, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.address!.foreignAddress!.email) ? "NA" : src.address!.foreignAddress!.email))
                .ForMember(dest => dest.emailOTP, opt => opt.MapFrom(src => src.address!.foreignAddress!.emailOTP ?? "NA"))
                .ForMember(dest => dest.address, opt => opt.MapFrom(src => src.address!.foreignAddress!.address ?? "NA"));
        }
    }
}
