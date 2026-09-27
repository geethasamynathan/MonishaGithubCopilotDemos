using System.Collections.Concurrent;
using System.Threading;
using BankingApi.Models;

namespace BankingApi.Services
{
    public class AccountStore : IAccountStore
    {
        private readonly ConcurrentDictionary<int, Account> _accounts = new();
        private readonly ConcurrentDictionary<int, List<Models.Transaction>> _transactions = new();
        private int _nextId;
        private int _nextTransactionId;

        public AccountStore()
        {
            // Seed accounts as per rules
            var a1 = new Account { Id = 1001, HolderName = "Asha", Balance = 1000.00m };
            var a2 = new Account { Id = 1002, HolderName = "Ravi", Balance = 500.00m };
            _accounts[a1.Id] = a1;
            _accounts[a2.Id] = a2;
            _transactions[a1.Id] = new List<Models.Transaction>();
            _transactions[a2.Id] = new List<Models.Transaction>();
            _nextId = 1002;
            _nextTransactionId = 0;
        }

        public IEnumerable<Account> GetAll() => _accounts.Values.OrderBy(a => a.Id);

        public Account? GetById(int id)
        {
            _accounts.TryGetValue(id, out var account);
            return account;
        }

        public Account Create(string holderName, decimal openingBalance)
        {
            var id = Interlocked.Increment(ref _nextId);
            var account = new Account
            {
                Id = id,
                HolderName = holderName,
                Balance = openingBalance
            };

            if (!_accounts.TryAdd(account.Id, account))
            {
                // unlikely, but retry with new id
                return Create(holderName, openingBalance);
            }

            _transactions[account.Id] = new List<Models.Transaction>();

            return account;
        }

        public bool TryDeposit(int accountId, decimal amount, out Account? updatedAccount, out string? error)
        {
            updatedAccount = null;
            error = null;

            if (amount <= 0)
            {
                error = "amount_must_be_positive";
                return false;
            }

            if (decimal.Round(amount, 2) != amount)
            {
                error = "amount_max_two_decimal_places";
                return false;
            }

            if (!_accounts.TryGetValue(accountId, out var account))
            {
                error = "not_found";
                return false;
            }

            lock (account)
            {
                // perform update
                account.Balance += amount;

                var tx = new Models.Transaction
                {
                    Id = Interlocked.Increment(ref _nextTransactionId),
                    AccountId = accountId,
                    Type = Models.TransactionType.Deposit,
                    Amount = amount,
                    Timestamp = DateTime.UtcNow
                };

                _transactions.GetOrAdd(accountId, _ => new List<Models.Transaction>()).Add(tx);
                updatedAccount = account;
                return true;
            }
        }

        public bool TryWithdraw(int accountId, decimal amount, out Account? updatedAccount, out string? error)
        {
            updatedAccount = null;
            error = null;

            if (amount <= 0)
            {
                error = "amount_must_be_positive";
                return false;
            }

            if (decimal.Round(amount, 2) != amount)
            {
                error = "amount_max_two_decimal_places";
                return false;
            }

            if (!_accounts.TryGetValue(accountId, out var account))
            {
                error = "not_found";
                return false;
            }

            lock (account)
            {
                if (account.Balance < amount)
                {
                    error = "insufficient_funds";
                    return false;
                }

                account.Balance -= amount;

                var tx = new Models.Transaction
                {
                    Id = Interlocked.Increment(ref _nextTransactionId),
                    AccountId = accountId,
                    Type = Models.TransactionType.Withdrawal,
                    Amount = amount,
                    Timestamp = DateTime.UtcNow
                };

                _transactions.GetOrAdd(accountId, _ => new List<Models.Transaction>()).Add(tx);
                updatedAccount = account;
                return true;
            }
        }

        public IEnumerable<Models.Transaction> GetTransactions(int accountId)
        {
            if (!_accounts.ContainsKey(accountId)) return Enumerable.Empty<Models.Transaction>();
            _transactions.TryGetValue(accountId, out var list);
            return (list ?? new List<Models.Transaction>()).OrderBy(t => t.Timestamp);
        }
    }
}
