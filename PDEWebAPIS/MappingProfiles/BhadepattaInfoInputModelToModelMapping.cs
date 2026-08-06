using AutoMapper;
using PDEWebAPIS.InputDataModel;
using PDEWebAPIS.Model;

namespace PDEWebAPIS.MappingProfiles
{
    public class BhadepattaInfoInputModelToModelMapping : Profile
    {
        public BhadepattaInfoInputModelToModelMapping()
        {
            CreateMap<BhadepattaInfoInputModel, BhadepattaInfoModel>();
        }
    }
}
