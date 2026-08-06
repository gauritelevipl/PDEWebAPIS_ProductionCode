using static PDEWebAPIS.InputDataModel.INameWithCodeFormatData;
namespace PDEWebAPIS.InputDataModel
{
    public class CreateApplicationData : INameWithCodeFormatData
    {
        public string? applicationType { set; get; }
        public DistrictData? district { set; get; }
        public MutationTypeData? mutationType { set; get; }
        public string? isCourtDawa { set; get; }
        public string? isDastApplicable { set; get; }
        public string? isMainPatra { set; get; }
        public OfficeData? office { set; get; }
        public string? userId { set; get; }
    }
    public class OfficeData
    {
        public string? office_code { set; get; }
        public string? office_name { set; get; }
    }
    public class MutationTypeData
    {
        public string? mutationTypeCode { set; get; }
        public string? mutationTypeName { set; get; }
    }
}
