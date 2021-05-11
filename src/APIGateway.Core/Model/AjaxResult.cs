namespace APIGateway.Core.Model
{
    public class AjaxResult
    {
        public string Message { get; set; }

        public object Data { get; set; }

        public AjaxResultType Type { get; set; }

        public AjaxResult()
        {

        }

        public AjaxResult(string message, AjaxResultType type = AjaxResultType.Success, object data = null)
        {
            this.Message = message;
            this.Data = data;
            this.Type = type;
        }

        public static AjaxResult Success(string message = "操作成功", object data = null)
        {
            return new AjaxResult() { Data = data, Message = message, Type = AjaxResultType.Success };
        }

        public static AjaxResult Error(string message, object data = null)
        {
            return new AjaxResult(message, AjaxResultType.Error, data);
        }
    }

    public enum AjaxResultType
    {
        Info = 203,

        Success = 200,

        Error = 500,

        UnAuth = 401,

        Forbidden = 403,

        NoFound = 404,

        Locked = 423
    }
}
