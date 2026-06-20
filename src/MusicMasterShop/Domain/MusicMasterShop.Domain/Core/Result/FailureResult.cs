namespace MusicMasterShop.Domain.Core.Result
{
    public class FailureResult
    {
        public FailureResult(string correlationId, IEnumerable<Error> errors)
        {
            CorrelationId = correlationId;
            Errors = errors.ToArray();
        }

        public string CorrelationId { get; set; }
        public Error[] Errors { get; set; }
    }
}
