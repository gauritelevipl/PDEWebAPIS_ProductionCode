using AutoMapper;
using PDEWebAPIS.Model;
using PDEWebAPIS.Repository;

namespace PDEWebAPIS.MappingProfiles
{
    public class BhadepattaInfoModelToTableMapping : Profile
    {
        public BhadepattaInfoModelToTableMapping()
        {
            CreateMap<BhadepattaInfoModel, BhadepattaInfoDtl>()
            //CreateMap<BhadepattaInfoModel, BhadepattaInfoDtl>()
            //   .ForMember(
            //d => d.bhadepattaFromDate,
            //o => o.MapFrom(s =>
            //    s.bhadepattaFromDate ?? DateTime.Now))
            //   .ForMember(
            //d => d.bhadepattaToDate,
            //o => o.MapFrom(s =>
            //    s.bhadepattaToDate ?? DateTime.Now))
            //.ForMember(dest => dest.bhadepattaFromDate, opt => opt.MapFrom(src => (!string.IsNullOrEmpty(src.bhadepattaFromDate))?
            //DateTime.SpecifyKind(Convert.ToDateTime(src.bhadepattaFromDate), DateTimeKind.Utc):"NA"))
            //.ForMember(dest => dest.bhadepattaToDate, opt => opt.MapFrom(src => DateTime.SpecifyKind(Convert.ToDateTime(src.bhadepattaToDate), DateTimeKind.Utc)))
            //.ForMember(dest => dest.bhadepattaFromDate, opt => opt.MapFrom(src => DateTime.SpecifyKind(Convert.ToDateTime(src.bhadepattaFromDate), DateTimeKind.Utc)))
            //.ForMember(dest => dest.bhadepattaToDate, opt => opt.MapFrom(src => DateTime.SpecifyKind(Convert.ToDateTime(src.bhadepattaToDate), DateTimeKind.Utc)))
            .ForMember(dest => dest.createdDateTime, opt => opt.Ignore())
            .ForMember(dest => dest.deletedDateTime, opt => opt.Ignore())
            .ForMember(dest => dest.leaseperiod, opt => opt.MapFrom(src => (src.leaseperiod!.ToUpper() == "NO") ? false : true))
            .ForMember(dest => dest.bhadepattaFromDate, opt => opt.MapFrom(src => (!string.IsNullOrEmpty(src.bhadepattaFromDate)) ? src.bhadepattaFromDate : "NA"))
            .ForMember(dest => dest.bhadepattaToDate, opt => opt.MapFrom(src => (!string.IsNullOrEmpty(src.bhadepattaToDate)) ? src.bhadepattaToDate : "NA"))
            .ForMember(dest => dest.bhadepattaTenureYear, opt => opt.MapFrom(src => (!string.IsNullOrEmpty(src.bhadepattaTenureYear)) ? src.bhadepattaTenureYear : "NA"))
            .ForMember(dest => dest.bhadepattaTenureMonth, opt => opt.MapFrom(src => (!string.IsNullOrEmpty(src.bhadepattaTenureMonth)) ? src.bhadepattaTenureMonth : "NA"));
        }
    }
}
