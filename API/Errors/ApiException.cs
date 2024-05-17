namespace API.Errors;

public class ApiException
{
    public ApiException(int statuscode, string message, string details)
    {
        StatusCode = statuscode;
        Message = message;
        Details = details;
    }

    public int StatusCode { set; get; }

    public string Message { set; get; }

     public string Details { set; get; }
}
