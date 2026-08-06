using AutoMapper;
using PDEWebAPIS.CommonMethods;
using PDEWebAPIS.Repository;
using PDEWebAPIS.ViewModel;

namespace PDEWebAPIS.MappingProfiles
{
    public class BhadepattaViewMapping : Profile
    {
        private CommonFunctions commonFunctions = new CommonFunctions();
        public BhadepattaViewMapping()
        {
            CreateMap<MutationGiverTakerDTL, FetchBhadepattaData>()

            .ForMember(dest => dest.mutation_givertaker_id, opt => opt.MapFrom(src => src.mutation_givertaker_id))
            //.ForMember(dest => dest.mutation_dtl_id, opt => opt.MapFrom(src => src.mutation_givertaker_id))
            .ForMember(dest => dest.userid, opt => opt.MapFrom(src => src.userMaster!.userid))
            .ForMember(dest => dest.applicationid, opt => opt.MapFrom(src => src.applicationDTL!.applicationid))
            .ForMember(dest => dest.usertype, opt => opt.MapFrom(src => src.user_type))
            .ForMember(dest => dest.usertype_code, opt => opt.MapFrom(src => src.user_type_code))

            .ForPath(dest => dest.userDetails!.suffixcode, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.userMaster!.prefixcode_marathi) ? "NA" : src.prefixcode_marathi))
            .ForPath(dest => dest.userDetails!.suffixCodeEng, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.userMaster!.prefixcode_eng) ? "NA" : src.prefixcode_eng))
            .ForPath(dest => dest.userDetails!.suffix, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.prefix_in_marathi) ? "NA" : src.prefix_in_marathi))
            .ForPath(dest => dest.userDetails!.suffixEng, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.prefix_in_eng) ? "NA" : src.prefix_in_eng))
            .ForPath(dest => dest.userDetails!.firstName, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.fname_in_marathi) ? "NA" : src.fname_in_marathi))
            .ForPath(dest => dest.userDetails!.middleName, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.mname_in_marathi) ? "NA" : src.mname_in_marathi))
            .ForPath(dest => dest.userDetails!.lastName, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.lname_in_marathi) ? "NA" : src.lname_in_marathi))
            .ForPath(dest => dest.userDetails!.firstNameEng, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.fname_in_eng) ? "NA" : src.fname_in_eng))
            .ForPath(dest => dest.userDetails!.middleNameEng, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.mname_in_eng) ? "NA" : src.mname_in_eng))
            .ForPath(dest => dest.userDetails!.lastNameEng, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.lname_in_eng) ? "NA" : src.lname_in_eng))
            .ForPath(dest => dest.userDetails!.aliceName, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.alias_name) ? "NA" : src.alias_name))
            .ForPath(dest => dest.userDetails!.holderType, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.holder_type) ? "NA" : src.holder_type))
            .ForPath(dest => dest.userDetails!.dob, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.dob) ? "NA" : src.dob))
            .ForPath(dest => dest.userDetails!.motherName, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.mother_name_in_marathi) ? "NA" : src.mother_name_in_marathi))
            .ForPath(dest => dest.userDetails!.motherNameEng, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.mother_name_in_eng) ? "NA" : src.mother_name_in_eng))
            .ForPath(dest => dest.userDetails!.nabhu, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.city_servey_no) ? "NA" : src.city_servey_no))
            .ForPath(dest => dest.userDetails!.userName, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.userName) ? "NA" : src.userName))
            .ForPath(dest => dest.userDetails!.lrPropertyUID, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.lr_property_id) ? "NA" : src.lr_property_id))
            .ForPath(dest => dest.userDetails!.milkat, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.milkat) ? "NA" : src.milkat))
            .ForPath(dest => dest.userDetails!.namud, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.namud) ? "NA" : src.namud))
            .ForPath(dest => dest.userDetails!.subPropNo, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.sub_property_no) ? "NA" : src.sub_property_no))

            .ForPath(dest => dest.areaForMutation!.isFullAreaGiven, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.isFullAreaGiven) ? "NA" : src.isFullAreaGiven))
            .ForPath(dest => dest.areaForMutation!.actualArea, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.actual_area) ? "NA" : src.actual_area))
            .ForPath(dest => dest.areaForMutation!.mutationArea, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.mutation_area) ? "NA" : src.mutation_area))
            .ForPath(dest => dest.areaForMutation!.availableArea, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.available_area) ? "NA" : src.available_area))

            .ForPath(dest => dest.address!.addressType, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.address_type) ? "NA" : src.address_type))
            //.AfterMap((src, dest) =>
            //{
            //    string addressType = src.address_type?.Trim().ToUpper() ?? "";
            //    if (addressType == "INDIA")
            //    {
            //        dest.address!.indiaAddress!.state = !string.IsNullOrEmpty(src.state)
            //            ? src.state
            //            : "NA";
            //        dest.address!.indiaAddress!.district = !string.IsNullOrEmpty(src.state)? src.district: "NA";
            //        dest.address!.indiaAddress!.city= !string.IsNullOrEmpty(src.state) ? src.city : "NA";
            //        dest.address!.indiaAddress!.taluka= !string.IsNullOrEmpty(src.state) ? src.taluka : "NA";
            //        dest.address!.indiaAddress!.plotNo= !string.IsNullOrEmpty(src.state) ? src.flatno_plotno : "NA";
            //        dest.address!.indiaAddress!.building= !string.IsNullOrEmpty(src.state) ? src.societyname : "NA";
            //        dest.address!.indiaAddress!.mainRoad= !string.IsNullOrEmpty(src.state) ? src.mainstreet : "NA";
            //        dest.address!.indiaAddress!.impSymbol= !string.IsNullOrEmpty(src.state) ? src.landmark: "NA";
            //        dest.address!.indiaAddress!.area= !string.IsNullOrEmpty(src.state) ? src.locality : "NA";
            //        dest.address!.indiaAddress!.pincode= !string.IsNullOrEmpty(src.state) ? src.pincode : "NA";
            //        dest.address!.indiaAddress!.postOfficeName= !string.IsNullOrEmpty(src.state) ? src.post_office_name : "NA";
            //        dest.address!.indiaAddress!.addressProofName= !string.IsNullOrEmpty(src.state) ? src.address_proof_document_name : "NA";
            //        dest.address!.indiaAddress!.signatureName= !string.IsNullOrEmpty(src.state) ? src.signed_file_name : "NA";
            //        dest.address!.indiaAddress!.mobile= !string.IsNullOrEmpty(src.state) ? src.mobileno : "NA";
            //    }
            //    else if (addressType == "FOREIGN")
            //    {
            //        dest.address!.foreignAddress!.address = !string.IsNullOrEmpty(src.state) ? src.address : "NA";
            //        dest.address!.foreignAddress!.mobile = !string.IsNullOrEmpty(src.state) ? src.mobileno : "NA";
            //        dest.address!.foreignAddress!.email = !string.IsNullOrEmpty(src.state) ? src.emailid : "NA";
            //        dest.address!.foreignAddress!.emailOTP = !string.IsNullOrEmpty(src.state) ? src.emailidverified : "NA";

            //    }

            //});

            .ForPath(dest => dest.address!.indiaAddress!.state, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.state) ? "NA" : src.state))
            .ForPath(dest => dest.address!.indiaAddress!.district, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.district) ? "NA" : src.district))
            .ForPath(dest => dest.address!.indiaAddress!.city, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.city) ? "NA" : src.city))
            .ForPath(dest => dest.address!.indiaAddress!.taluka, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.taluka) ? "NA" : src.taluka))
            .ForPath(dest => dest.address!.indiaAddress!.plotNo, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.flatno_plotno) ? "NA" : src.flatno_plotno))
            .ForPath(dest => dest.address!.indiaAddress!.building, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.societyname) ? "NA" : src.societyname))
            .ForPath(dest => dest.address!.indiaAddress!.mainRoad, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.mainstreet) ? "NA" : src.mainstreet))
            .ForPath(dest => dest.address!.indiaAddress!.impSymbol, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.landmark) ? "NA" : src.landmark))
            .ForPath(dest => dest.address!.indiaAddress!.area, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.locality) ? "NA" : src.locality))
            .ForPath(dest => dest.address!.indiaAddress!.pincode, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.pincode) ? "NA" : src.pincode))
            .ForPath(dest => dest.address!.indiaAddress!.postOfficeName, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.post_office_name) ? "NA" : src.post_office_name))
            .ForPath(dest => dest.address!.indiaAddress!.addressProofName, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.address_proof_document_name) ? "NA" : src.address_proof_document_name))
            .ForPath(dest => dest.address!.indiaAddress!.signatureName, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.signed_file_name) ? "NA" : src.signed_file_name))
            .ForPath(dest => dest.address!.indiaAddress!.mobile, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.mobileno) ? "NA" : src.mobileno))

            .ForPath(dest => dest.address!.foreignAddress!.address, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.address) ? "NA" : src.address))
            .ForPath(dest => dest.address!.foreignAddress!.mobile, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.mobileno) ? "NA" : src.mobileno))
            .ForPath(dest => dest.address!.foreignAddress!.email, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.emailid) ? "NA" : src.emailid))

            //.ForPath(dest => dest.address!.foreignAddress!.signatureName, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.signed_file_name) ? "NA" : src.signed_file_name))
            .ForPath(dest => dest.address!.foreignAddress!.signatureSrc, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.signed_file_path) ? "NA" : src.signed_file_path))

            //dharak
            .ForPath(dest => dest.dharak!.userdharak!.aapakDropdown!.apk_code, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.apk_code.ToString()) ? "NA" : src.apk_code.ToString()))
            .ForPath(dest => dest.dharak!.userdharak!.aapakDropdown!.apk_description, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.apk_description!.ToString()) ? "NA" : src.apk_description.ToString()))
            .ForPath(dest => dest.dharak!.userdharak!.aapak, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.aapak) ? "NA" : src.aapak))
            .ForPath(dest => dest.dharak!.userdharak!.gender!.gender_code, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.gender_code) ? "NA" : src.gender_code))
            .ForPath(dest => dest.dharak!.userdharak!.gender!.gender_description, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.gender_description) ? "NA" : src.gender_description))
            .ForPath(dest => dest.dharak!.userdharak!.holderType!.owner_status_code, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.owner_status_code) ? "NA" : src.owner_status_code))
            .ForPath(dest => dest.dharak!.userdharak!.holderType!.owner_status_description, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.owner_status_description) ? "NA" : src.owner_status_description))

            //sanstha

            .ForPath(dest => dest.companyName, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.company_name_in_marathi) ? "NA" : src.company_name_in_marathi))
            .ForPath(dest => dest.companyNameEng, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.company_name_in_eng) ? "NA" : src.company_name_in_eng))
            .ForPath(dest => dest.dharak!.companydharak!.holderType!.owner_status_code, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.owner_status_code) ? "NA" : src.owner_status_code))
            .ForPath(dest => dest.dharak!.companydharak!.holderType!.owner_status_description, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.owner_status_description) ? "NA" : src.owner_status_description));
        }
    }
}
