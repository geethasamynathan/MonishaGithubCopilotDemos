# GitHub Copilot Chat — History, Storage and Session Management in VS Code

## Banking Application: Detailed Explanation and Practical Labs

When you work on a large banking application with GitHub Copilot in VS Code, you may give hundreds of prompts across several days. How is that history maintained, where is it stored, and how can you reopen or inspect it?

**Important distinction:** Saved chat history and the context currently available to an AI model are not the same thing.

## 1. Understand the architecture

```text
VS Code — Banking Application Workspace
    |
    +-- Copilot Chat / Agent Sessions
          |
          +-- Customer Registration Chat
          +-- JWT Login Chat
          +-- Fund Transfer Chat
          |
          +-- Saved History: prompts, replies, tool activity
          +-- Model Context: limited window, possibly compacted
```

| Concept | Meaning |
|---|---|
| Workspace | Banking application files and folders |
| Session | Managed unit of agent work associated with a task and workspace |
| Chat history | Earlier prompts, model responses, and tool activity |
| Context window | Limited information the model can consider during a request |

A chat can remain saved for review even though the model no longer has every earlier turn verbatim in its active context.

## 2. Where is history stored?

| Location | Purpose | Inspection |
|---|---|---|
| VS Code local application data | Persistent chat and session records | VS Code UI and diagnostic files |
| Local session index | Searchable history metadata | Supported session-search features |
| Optional GitHub session sync | Copies of supported sessions in cloud | Supported history UI |
| AI processing service | Processes prompts/context | Not a local chat transcript |
| Git repository | Committed source and project instructions | Git tools; chat is **not** automatically committed |

**Security:** A disabled cloud *session sync* does not imply that prompts never leave the computer. Copilot still sends relevant context to its model service under applicable product settings and organizational policy.

## 3. View previous sessions within VS Code

1. Open your Banking workspace.
2. Open Copilot Chat (`Ctrl+Alt+I` in typical Windows bindings, or use the Chat icon).
3. Find the **Sessions** list in the Chat view.
4. Select an earlier session to inspect its prompts, replies, and recorded changes.
5. Use the session menu for available actions (for example rename, pin, archive, delete).
6. In supported builds, `Ctrl+P`, then `agent ` (with a trailing space), opens agent-session Quick Open.

**Demo:** Make three distinct sessions: `Banking - Customer Registration`, `Banking - JWT Login`, and `Banking - Fund Transfer`.

## 4. Inspect local storage on Windows

### Step 1 — Find workspace storage

Use `Win+R`:

```text
%APPDATA%\Code\User\workspaceStorage
```

A **possible** layout (varies by VS Code release and chat backend):

```text
Code/User/
  globalStorage/
  workspaceStorage/
    <hashed-workspace-id>/
      workspace.json
      state.vscdb
      chatSessions/
        <session-id>.jsonl
```

Recent releases and some Copilot-backed agents may store session data in other locations/formats, including folders under `.copilot`. This illustration is not a guaranteed schema.

### Step 2 — Match hashed workspace directory to banking project

Run PowerShell:

```powershell
$root = "$env:APPDATA\Code\User\workspaceStorage"

Get-ChildItem $root -Directory | ForEach-Object {
    $workspaceFile = Join-Path $_.FullName "workspace.json"
    if (Test-Path $workspaceFile) {
        [PSCustomObject]@{
            StorageFolder = $_.Name
            WorkspaceInfo = Get-Content $workspaceFile -Raw
        }
    }
} | Format-List
```

**Explanation:** Lists workspace storage directories, reads workspace mapping metadata, and helps identify the record for your Banking project.

### Step 3 — Search for transcript-like files

```powershell
$root = "$env:APPDATA\Code\User\workspaceStorage"

Get-ChildItem $root -Recurse -File -ErrorAction SilentlyContinue |
    Where-Object {
        $_.FullName -match 'chatSessions|transcripts' -and
        $_.Extension -in '.json', '.jsonl'
    } |
    Select-Object FullName, Length, LastWriteTime |
    Sort-Object LastWriteTime -Descending |
    Format-Table -AutoSize
```

These results are **candidate diagnostic files**, not proof that a specific file is the authoritative record for the active chat.

### Step 4 — Examine safely

Copy a session file to another directory before viewing it. Open the **copy** in VS Code. A `.jsonl` file consists of line-delimited JSON records; available fields depend on version. **Do not modify live VS Code storage files.**

### Step 5 — Examine potential Copilot-specific storage

```powershell
$copilotRoot = Join-Path $env:USERPROFILE ".copilot"

if (Test-Path $copilotRoot) {
    Get-ChildItem $copilotRoot -Recurse -File -ErrorAction SilentlyContinue |
        Where-Object { $_.Extension -in ".json", ".jsonl" } |
        Select-Object FullName, Length, LastWriteTime |
        Sort-Object LastWriteTime -Descending |
        Select-Object -First 30
}
```

The VS Code session UI is the preferred supported way to manage history.

## 5. Export a chat to JSON (practical demo)

1. Open your previous Login session.
2. Press `Ctrl+Shift+P`.
3. Find **Chat: Export Chat...** (if your VS Code release supports it).
4. Save to `banking-login-chat.json`.
5. Open JSON in VS Code and search for a prompt such as `Create a login service with JWT authentication`.

This is safer and often more portable than editing internal storage. Export details vary by version.

## 6. Session continuity demo

### A. Reopen the same session

Prompt:

```text
Continue our previous JWT authentication implementation.
Identify which files were updated and what work remains.
Do not make changes yet.
```

Copilot can use available context from the reopened conversation, subject to the context-window limit and compaction.

### B. Start an entirely new session

Prompt:

```text
What JWT authentication decisions did we make yesterday?
```

A new session ordinarily has separate conversation context. It may access project files and, if enabled, previous session search, but does **not** necessarily inherit the old transcript.

### C. Fork an existing chat

Fork at an appropriate turn, then compare:

- Fork A: JWT authentication using ASP.NET Core Identity.
- Fork B: JWT authentication using a custom authentication service.

Each fork evolves independently with the shared prior context up to the fork point, where supported.

## 7. Context compaction

For a very long chat (many prompts, source files, and tool results), the context window can fill up. Older parts may be summarized to preserve the important decisions.

Try:

```text
/compact
```

Or:

```text
/compact Retain the banking database schema,
JWT authentication decisions, security requirements,
important file paths, and pending tasks.
```

Compaction is **not identical to deleting saved chat history**. Inspect the context usage indicator near the Chat input, when available.

## 8. Session search (Chronicle, where available)

Some newer VS Code/Copilot configurations provide session-history indexing and Chronicle features. Availability and exact commands vary. Try these **only if advertised in your command picker**:

```text
/chronicle:search JWT authentication
/chronicle:search FundTransferService
/chronicle:standup
/chronicle:tips
/chronicle:reindex
```

If those commands are absent, it does not mean your chat history has disappeared. Use the Sessions UI instead.

## 9. Syncing sessions and security settings

Newer VS Code configurations may include experimental session sync settings. Inspect them in `Preferences: Open User Settings (JSON)` and verify that they are recognized by your installed version.

Example configuration (supported keys depend on build):

```json
{
  "github.copilot.chat.localIndex.enabled": true,
  "chat.sessionSync.enabled": false,
  "chat.sessionSync.excludeRepositories": [
    "mybank/banking-application"
  ]
}
```

Review enterprise policies before changing settings. Disabling sync may not remove older synchronized copies. Session sync and model processing are separate functions.

**Banking security:** Never use production customer records, credentials, real account numbers, production secrets, or sensitive logs in demos. Prefer fake test accounts and nonproduction configuration.

## 10. Preserve knowledge across sessions with repository instructions

Example project structure:

```text
BankingApplication/
  .github/
    copilot-instructions.md
  docs/
    architecture.md
    database-schema.md
    project-decisions.md
  Banking.API/
  Banking.Services/
  Banking.Data/
  Banking.Tests/
```

Contents of `.github/copilot-instructions.md`:

```markdown
# Banking Application Instructions

Technology:
- C# and ASP.NET Core
- Entity Framework Core
- SQL Server
- xUnit

Architecture:
- API controllers
- Service layer
- Repository/data-access layer

Banking requirements:
- Monetary values use decimal
- Transfers must be atomic
- Validate available balance
- Enforce authorization
- Never log sensitive account information
- Include unit tests for business rules

Development standards:
- Prefer dependency injection
- Use async database operations
- Explain significant design decisions
```

In a fresh session, attach `docs/project-decisions.md` through the context picker or use the supported `#file` reference and ask:

```text
Read the project-decisions document and existing FundTransferService.
Continue implementing the transfer module while following
established architecture. First explain the approach.
Do not change code yet.
```

Repository instructions and decisions are version-controlled team knowledge; they do **not** automatically save the complete conversation history.

## 11. Which session-management feature should you use?

| Requirement | Recommended capability |
|---|---|
| Continue yesterday's work | Reopen a session |
| Start a separate module | New session |
| Create a new discussion in related work | New chat (if supported) |
| Compare alternatives | Fork |
| Find an older JWT discussion | Sessions / supported search |
| Shorten active context | `/compact` |
| Preserve conventions | `.github/copilot-instructions.md` |
| Preserve architecture decisions | Commit Markdown docs |
| Back up / inspect a conversation | Export Chat to JSON |
| Inspect proposed code edits | File diffs and checkpoints |
| Evaluate cloud copies | Session-sync settings and enterprise policy |

## 12. Instructor-led 20-minute lab

| Time | Demo | Expected learning |
|---|---|---|
| 0–3 min | Create a Banking session | Understand session creation |
| 3–6 min | Supply C#/EF Core/transaction context | See context accumulation |
| 6–9 min | Export conversation | Inspect persisted transcript |
| 9–12 min | Restart VS Code, reopen session | Demonstrate history persistence |
| 12–15 min | Create separate new session | Distinguish sessions from context |
| 15–18 min | Inspect local storage | Understand diagnostic persistence |
| 18–20 min | Create `copilot-instructions.md` | Preserve knowledge across sessions |

## 13. Important conclusions

1. **Persistence:** VS Code generally saves chat sessions for reopening and review.
2. **Model context:** Not all saved history remains in the model's live context; compaction can summarize earlier exchanges.
3. **Cross-session continuity:** New sessions are not guaranteed to know the entire previous chat; use files, instructions, and session search where available.
4. **Git versus chat:** A Git commit saves selected files, not Copilot conversations automatically.
5. **Banking compliance:** Local persistence, cloud synchronization, model processing, and organizational retention controls must be evaluated separately.

## Official reference links

- [Manage agent sessions — VS Code](https://code.visualstudio.com/docs/agents/run/sessions/manage-sessions)
- [Agent sessions overview — VS Code](https://code.visualstudio.com/docs/agents/concepts/sessions)
- [Session history — VS Code](https://code.visualstudio.com/docs/agents/run/sessions/session-history)
- [GitHub Copilot context management](https://docs.github.com/en/copilot/concepts/prompting-and-context/context-management)

*Note: VS Code and GitHub Copilot interfaces, feature flags, filesystem layouts, and commands change between releases. Confirm options against your installed version.*
