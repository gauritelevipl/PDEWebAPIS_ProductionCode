namespace PDEWebAPIS.Helpers
{
    public class ResponseHandler
    {
        public static ApiResponse GetExceptionResponse(string Exception)
        {
            ApiResponse response = new ApiResponse();
            response.Code = "3";
            response.Message = Exception;
            return response;
        }
        public static ApiResponse GetUnauthorisedResponse(string Exception)
        {
            ApiResponse response = new ApiResponse();
            response.Code = "4";
            response.Message = Exception;
            return response;
        }
        public static ApiResponse GetFailure(string FailureMSG)
        {
            ApiResponse response = new ApiResponse();
            response.Code = "0";
            response.Message = FailureMSG;
            return response;
        }
        public static ApiResponse GetAppResponse(ReponseType type, string Message, object? contract)
        {
            ApiResponse response = new ApiResponse();
            response = new ApiResponse { ResponseData = contract };
            switch (type)
            {
                case ReponseType.Success:
                    response.Code = "1";
                    response.Message = Message;
                    break;
                case ReponseType.Failure:
                    response.Code = "0";
                    response.Message = Message;
                    break;
                case ReponseType.NotFound:
                    response.Code = "2";
                    response.Message = Message;
                        //"No Record Available";
                    break;
                //case ReponseType.UserNotFound:
                //    response.Code = "3";
                //    response.Message = "User Not Found";
                //    break;
            }
            return response;
        }
    }
}
