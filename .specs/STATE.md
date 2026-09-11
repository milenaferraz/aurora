# Aurora AI — Project State

## Decisions

### AD-001: SSE for Chat Streaming

**Status**: active
**Context**: Need real-time streaming of Aurora's responses to the frontend.
**Decision**: Use Server-Sent Events (SSE) over WebSockets for the chat streaming endpoint.
**Rationale**: SSE is simpler (one-way server→client), works over standard HTTP/1.1, requires no upgrade handshake, and matches Hermes's streaming output model. The frontend uses `fetch` + `ReadableStream` since `EventSource` doesn't support POST.
**Consequences**: All future streaming endpoints use SSE. Frontend streaming always uses the fetch/ReadableStream pattern.

---

### AD-002: Modular Monolith — Pragmatic Clean Architecture

**Status**: active
**Context**: Aurora is a personal AI OS; architecture must evolve without rewrites.
**Decision**: Single deployable modular monolith with Clean Architecture-inspired layer separation (Api, Application, Infrastructure, Domain, Contracts). No CQRS handlers, no event bus, no generic repositories in MVP.
**Rationale**: Avoids overengineering for a personal project; layers allow service extraction later.
**Consequences**: All new backend features follow Api → Application → Infrastructure → Domain dependency direction. No cross-layer shortcuts.

---

### AD-003: Real Hermes Only

**Status**: active
**Context**: Aurora now targets the real Hermes service in all environments.
**Decision**: `HermesHttpClient` is the only runtime `IHermesClient` implementation.
**Rationale**: Keep the API flow explicit and production-like; avoid dual runtime paths.
**Consequences**: Development, CI, and production must all configure `Hermes:BaseUrl` (or equivalent env vars). Tests should use mocked HTTP handlers for Hermes calls.

---

## Handoff

**Status**: Ready for Execute — tasks.md validated (0 errors).
**Feature**: aurora-mvp
**Current phase**: Execute (tasks.md approved, awaiting batch execution)
**Completed tasks**: None
**Next step**: Continue with the current phase using the real Hermes configuration.
**Branch**: main (no git repo initialized yet)
**Blockers**: None
**Config**: `Hermes__BaseUrl` is required for runtime Hermes calls; tests mock HTTP dependencies
