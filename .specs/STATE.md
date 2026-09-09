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

### AD-003: FakeHermesClient Pattern

**Status**: active
**Context**: Hermes Agent may not be available during development or CI.
**Decision**: `Hermes__UseFake=true` environment variable switches registration from `HermesHttpClient` to `FakeHermesClient`. Both implement `IHermesClient`.
**Rationale**: Full vertical slice works without Hermes; CI uses fake; production uses real.
**Consequences**: Every environment that doesn't have Hermes sets `Hermes__UseFake=true`. Tests always use the fake or a mock.

---

## Handoff

**Status**: Ready for Execute — tasks.md validated (0 errors).
**Feature**: aurora-mvp
**Current phase**: Execute (tasks.md approved, awaiting batch execution)
**Completed tasks**: None
**Next step**: Execute B1 (T1–T8): Foundation + FakeHermes — first batch
**Branch**: main (no git repo initialized yet)
**Blockers**: None
**Config**: `Hermes__UseFake=true` for all batches until Milestone 4
