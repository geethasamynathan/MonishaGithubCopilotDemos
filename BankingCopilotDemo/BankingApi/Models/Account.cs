using System;

namespace BankingApi.Models
{
    public class Account
    {
        public int Id { get; set; }
        public string HolderName { get; set; } = string.Empty;
        public string Currency { get; set; } = "INR";
        public decimal Balance { get; set; }
    }
}
