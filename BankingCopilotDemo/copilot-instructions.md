# Repository guidance for Copilot

This is a teaching project built with ASP.NET Core Web API and C#.

Before implementing banking behavior, read `docs/rules.md`.

Keep this example small and in memory. Put account state and balance
changes in `BankingStore`, not directly in HTTP handlers.

Use clear HTTP status codes and return JSON from API endpoints.
Do not add a database, login, payment gateway, or external banking service.

After changing C# code, build the solution and explain which files changed.
Do not describe this demonstration as production-ready banking software.