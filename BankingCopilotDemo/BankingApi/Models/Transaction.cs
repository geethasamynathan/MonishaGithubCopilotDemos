using System;

namespace BankingApi.Models
{
    public enum TransactionType
    {
        Deposit,
        Withdrawal
    }

    public class Transaction
    {
        public int Id { get; set; }
        public int AccountId { get; set; }
        public TransactionType Type { get; set; }
        public decimal Amount { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
