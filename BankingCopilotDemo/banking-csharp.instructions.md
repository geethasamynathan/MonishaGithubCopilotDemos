---
description: Rules for C# code in the banking demonstration
applyTo: "**/*.cs"
---

- Use `decimal` for all monetary values.
- Use nullable reference types.
- Use readable names and small methods.
- Validate inputs before changing account state.
- Protect the entire read-modify-write balance operation from concurrent calls.
- Return copies of account data instead of exposing mutable internal state.
- Use a transaction entry only for a successful deposit or withdrawal.