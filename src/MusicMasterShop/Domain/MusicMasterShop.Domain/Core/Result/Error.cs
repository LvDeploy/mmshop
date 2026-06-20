namespace MusicMasterShop.Domain.Core.Result
{
    public class Error
    {
        public string Detail { get; set; } = string.Empty;

        public static Error Set(string? detail)
        {
            return new Error
            {
                Detail = detail ?? string.Empty
            };
        }
    }
}
