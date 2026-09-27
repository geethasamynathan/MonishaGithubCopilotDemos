namespace BankingApi.Models
{
    public class CreateAccountRequest
    {
        public string? HolderName { get; set; }
        public decimal OpeningBalance { get; set; }
    }
}
