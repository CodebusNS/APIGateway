namespace APIGateway.Core.Model
{
    public class OperationResult
    {
        public bool Succeeded { get; set; }

        public string Message { get; set; }

        public object Data { get; set; }

        public static OperationResult CreateSuccess(string message, object data = null)
        {
            return new OperationResult { Message = message, Succeeded = true, Data = data };
        }

        public static OperationResult CreateError(string message)
        {
            return new OperationResult { Succeeded = false, Message = message };
        }
    }
}
