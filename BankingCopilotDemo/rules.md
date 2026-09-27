# Banking demo business rules

This is an educational, in-memory banking application.

## Accounts

- Each account has an integer ID, holder name, currency INR, and balance.
- Seed account 1001 for Asha with 1000.00 INR.
- Seed account 1002 for Ravi with 500.00 INR.
- A new account needs a nonempty holder name.
- Its opening balance cannot be negative.

## Money operations

- Use decimal values, never double or float.
- Money values can have at most two decimal places.
- Deposit amount must be greater than zero.
- Withdrawal amount must be greater than zero.
- A withdrawal cannot exceed the available balance.
- A rejected operation must not change the balance or create a transaction.
- Concurrent balance changes must not overwrite each other.

## HTTP behavior

- GET /api/accounts returns accounts.
- GET /api/accounts/{id} returns one account or 404.
- POST /api/accounts creates an account and returns 201.
- POST /api/accounts/{id}/deposit returns the updated account.
- POST /api/accounts/{id}/withdraw returns the updated account.
- GET /api/accounts/{id}/transactions returns successful transactions.
- Invalid input returns 400.
- Unknown account returns 404.
- Insufficient funds returns 409.

## Browser demonstration

Provide a small page to create an account, deposit, withdraw,
display the balance, and display transactions.