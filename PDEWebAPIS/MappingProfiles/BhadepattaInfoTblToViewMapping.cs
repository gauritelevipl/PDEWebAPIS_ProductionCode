using AutoMapper;
using PDEWebAPIS.Repository;
using PDEWebAPIS.ViewModel;

namespace PDEWebAPIS.MappingProfiles
{
    public class BhadepattaInfoTblToViewMapping : Profile
    {
        public BhadepattaInfoTblToViewMapping()
        {
            CreateMap<BhadepattaInfoDtl, FetchBhadepattaInfoData>()
            .ForMember(dest => dest.applicationid, opt => opt.MapFrom(src => src.applicationid))
            .ForMember(dest => dest.Info_id, opt => opt.MapFrom(src => src.Info_id))
            .ForMember(dest => dest.bhadepattaTenureYear, opt => opt.MapFrom(src => src.bhadepattaTenureYear))
            .ForMember(dest => dest.bhadepattaTenureMonth, opt => opt.MapFrom(src => src.bhadepattaTenureMonth))
            //.ForMember(
            //dest => dest.bhadepattaToDate,
            //opt => opt.MapFrom(src =>
            //src.bhadepattaToDate.HasValue
            //? src.bhadepattaToDate.Value.ToString("yyyy-MM-dd")
            //: null))
            //.ForMember(
            //dest => dest.bhadepattaFromDate,
            //opt => opt.MapFrom(src =>
            //src.bhadepattaToDate.HasValue
            //? src.bhadepattaToDate.Value.ToString("yyyy-MM-dd")
            //: null))
            .ForMember(dest => dest.bhadepattaToDate, opt => opt.MapFrom(src => src.bhadepattaToDate))
            .ForMember(dest => dest.bhadepattaFromDate, opt => opt.MapFrom(src => src.bhadepattaFromDate))
            .ForMember(dest => dest.bhadepattaAmount, opt => opt.MapFrom(src => src.bhadepattaAmount));
        }
    }
}
