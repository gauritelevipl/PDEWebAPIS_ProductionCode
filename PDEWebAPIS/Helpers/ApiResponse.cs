namespace PDEWebAPIS.Helpers
{
    public class ApiResponse
    {
        public string Code { set; get; } = string.Empty;
        public string Message { set; get; } = string.Empty; 
        public object? ResponseData { set; get; }
    }
    public enum ReponseType
    {
        Success,
        NotFound,
        Failure,
        UserNotFound
    }
    public class ApiResponseForNIC
    {
        public object? ResponseData { set; get; }
    }
}
