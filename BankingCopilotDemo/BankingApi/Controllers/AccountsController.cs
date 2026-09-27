using Microsoft.AspNetCore.Mvc;
using BankingApi.Models;
using BankingApi.Services;

namespace BankingApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountsController : ControllerBase
    {
        private readonly IAccountStore _store;

        public AccountsController(IAccountStore store)
        {
            _store = store;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Account>> GetAll()
        {
            var accounts = _store.GetAll();
            return Ok(accounts);
        }

        [HttpGet("{id}")]
        public ActionResult<Account> GetById(int id)
        {
            var account = _store.GetById(id);
            if (account is null) return NotFound();
            return Ok(account);
        }

        [HttpPost]
        public ActionResult<Account> Create(CreateAccountRequest req)
        {
            if (string.IsNullOrWhiteSpace(req.HolderName))
            {
                return BadRequest(new { error = "HolderName is required" });
            }

            if (req.OpeningBalance < 0)
            {
                return BadRequest(new { error = "OpeningBalance cannot be negative" });
            }

            // at most two decimal places
            if (decimal.Round(req.OpeningBalance, 2) != req.OpeningBalance)
            {
                return BadRequest(new { error = "OpeningBalance must have at most two decimal places" });
            }

            var created = _store.Create(req.HolderName.Trim(), req.OpeningBalance);

            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPost("{id}/deposit")]
        public ActionResult<Account> Deposit(int id, MoneyRequest req)
        {
            if (decimal.Round(req.Amount, 2) != req.Amount || req.Amount <= 0)
            {
                return BadRequest(new { error = "Amount must be > 0 and at most two decimal places" });
            }

            var ok = _store.TryDeposit(id, req.Amount, out var updated, out var error);
            if (!ok)
            {
                if (error == "not_found") return NotFound();
                return BadRequest(new { error });
            }

            return Ok(updated);
        }

        [HttpPost("{id}/withdraw")]
        public ActionResult<Account> Withdraw(int id, MoneyRequest req)
        {
            if (decimal.Round(req.Amount, 2) != req.Amount || req.Amount <= 0)
            {
                return BadRequest(new { error = "Amount must be > 0 and at most two decimal places" });
            }

            var ok = _store.TryWithdraw(id, req.Amount, out var updated, out var error);
            if (!ok)
            {
                if (error == "not_found") return NotFound();
                if (error == "insufficient_funds") return Conflict(new { error = "insufficient_funds" });
                return BadRequest(new { error });
            }

            return Ok(updated);
        }

        [HttpGet("{id}/transactions")]
        public ActionResult<IEnumerable<Models.Transaction>> GetTransactions(int id)
        {
            var account = _store.GetById(id);
            if (account is null) return NotFound();

            var tx = _store.GetTransactions(id);
            return Ok(tx);
        }
    }
}
