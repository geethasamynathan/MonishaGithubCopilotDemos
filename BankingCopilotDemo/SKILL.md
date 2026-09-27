---
name: banking-balance-check
description: Implement and verify deposits and withdrawals in the banking ASP.NET Core API. Use when changing account balances or transaction history.
---

When working on a deposit or withdrawal:

1. Read `docs/rules.md`.
2. Check positive amounts and the maximum of two decimal places.
3. Check missing accounts and insufficient funds.
4. Ensure a rejected operation leaves balance and history unchanged.
5. Make the balance update and transaction entry one atomic operation.
6. Check that concurrent requests cannot lose an update.
7. Add or run tests for the changed behavior.
8. Report the actual build and test results.