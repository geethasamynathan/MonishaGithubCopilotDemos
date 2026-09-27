using System.Collections.Generic;
using BankingApi.Models;

namespace BankingApi.Services
{
    public interface IAccountStore
    {
        IEnumerable<Account> GetAll();
        Account? GetById(int id);
        Account Create(string holderName, decimal openingBalance);
        bool TryDeposit(int accountId, decimal amount, out Account? updatedAccount, out string? error);
        bool TryWithdraw(int accountId, decimal amount, out Account? updatedAccount, out string? error);
        IEnumerable<Models.Transaction> GetTransactions(int accountId);
    }
}
