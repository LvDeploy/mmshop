namespace MusicMasterShop.Domain.Core.Result
{
    public class SuccessResult<T>
    {
        public SuccessResult(string correlationId, T? data, string? message)
        {
            CorrelationId = correlationId;
            Data = data;
            Message = message;
        }

        public string CorrelationId { get; set; }
        public T? Data { get; set; }
        public string? Message { get; set; }
    }
}
