# Aurora AI MVP Tasks

## Execution Protocol (MANDATORY — do not skip)

Implement these tasks with the `tlc-spec-driven` skill: **activate it by name and follow its Execute flow and Critical Rules.** Do not search for skill files by filesystem path. The skill is the source of truth for the full flow (per-task cycle, sub-agent delegation, adequacy review, Verifier, discrimination sensor).

**If the skill cannot be activated, STOP and tell the user — do not proceed without it.**

---

**Design**: `.specs/features/aurora-mvp/design.md`
**Status**: Approved

> **Configuration**: `Hermes__UseFake=true` for all environments until Milestone 4.

---

## Test Coverage Matrix

> Generated from spec and design — fresh project, no existing tests. Strong defaults applied. Guidelines found: none.
> Frontend components are classified by whether they have testable behavior (logic/events/state) or are display-only.

| Code Layer | Required Test Type | Coverage Expectation | Location Pattern | Run Command |
| ---------- | ------------------ | -------------------- | ---------------- | ----------- |
| Application services (ChatService, DashboardService, ChatStreamingService) | unit | All branches; 1:1 to spec ACs; all listed edge cases | `tests/Aurora.UnitTests/**/*.cs` | `dotnet test tests/Aurora.UnitTests` |
| Infrastructure (FakeHermesClient, HermesHttpClient) | unit | Key paths + error paths | `tests/Aurora.UnitTests/**/*.cs` | `dotnet test tests/Aurora.UnitTests` |
| Middleware (CorrelationId, ExceptionHandler) | unit | ID generation/propagation; 500 on unhandled; 502 on HermesException | `tests/Aurora.UnitTests/**/*.cs` | `dotnet test tests/Aurora.UnitTests` |
| Controllers / endpoints | integration | All routes: happy + validation error + Hermes-offline paths | `tests/Aurora.IntegrationTests/**/*.cs` | `dotnet test tests/Aurora.IntegrationTests` |
| Domain / Contracts / Config / DI | none | Build gate only | — | build gate only |
| Vue components with logic (AuroraCore, ChatInput, SystemStatusCard, ActivityFeed, VoiceButton) | unit | State transitions; event emissions; empty/populated/error states | `src/aurora-web/src/**/*.spec.ts` | `npm run test --prefix src/aurora-web` |
| Vue composables (useChat) | unit | SSE delta/complete/error paths; abort on unmount; aurora state transitions | `src/aurora-web/src/**/*.spec.ts` | `npm run test --prefix src/aurora-web` |
| Vue stores (auroraStore, chatStore, dashboardStore) | unit | State mutations and action behavior | `src/aurora-web/src/**/*.spec.ts` | `npm run test --prefix src/aurora-web` |
| Vue display-only components (ChatMessage, ChatMessages, AuroraThinking, ToolActivity, HomeView, cards) | none | Build gate only — no independent logic | — | build gate only |
| Vue views / layout / router | none | Build gate only | — | build gate only |

## Gate Check Commands

> Generated from project design — confirm before Execute.

| Gate Level | When to Use | Command |
| ---------- | ----------- | ------- |
| Quick (backend) | After tasks with unit tests only | `dotnet test tests/Aurora.UnitTests --no-build` |
| Full (backend) | After tasks with integration tests | `dotnet test tests/Aurora.UnitTests --no-build && dotnet test tests/Aurora.IntegrationTests --no-build` |
| Build (backend) | After phase completion or no-test tasks | `dotnet build Aurora.sln && dotnet test Aurora.sln` |
| Quick (frontend) | After tasks with frontend unit tests | `npm run test --prefix src/aurora-web` |
| Build (frontend) | After frontend phase completion or no-test tasks | `npm run lint --prefix src/aurora-web && npm run test --prefix src/aurora-web && npm run build --prefix src/aurora-web` |
| Build (full) | Final gates | `dotnet build Aurora.sln && dotnet test Aurora.sln && npm run lint --prefix src/aurora-web && npm run test --prefix src/aurora-web && npm run build --prefix src/aurora-web` |

---

## Milestones

| Milestone | After task | Demo |
| --------- | ---------- | ---- |
| M1: "Hello Aurora" | T12 | Type "Aurora, boa noite." → backend responds via FakeHermes → text appears |
| M2: "Aurora is Streaming" | T18 | Full SSE streaming with "Aurora está pensando..." animation |
| M3: "Dashboard & Home" | T24 | Full home screen: greeting, AuroraCore, system status, cards |
| M4: "Real Hermes Ready" | T26 | Set `Hermes__UseFake=false` + `Hermes__BaseUrl` → real Hermes works |

---

## Execution Plan

Phases run sequentially. Milestone boundaries land at batch edges.

**Batch 1 (B1):** Phases 1–3, T1–T8 — Foundation + FakeHermes
**Batch 2 (B2):** Phases 4–5, T9–T12 → **Milestone 1**
**Batch 3 (B3):** Phases 6–7, T13–T18 → **Milestone 2**
**Batch 4 (B4):** Phases 8–9, T19–T24 → **Milestone 3**
**Batch 5 (B5):** Phases 10–11, T25–T28 → **Milestone 4** + Backend Tests
**Batch 6 (B6):** Phases 12–14, T29–T34 → Quality & Ship

### Phase 1: Foundation

```
T1 → T2 → T3
```

### Phase 2: Domain + Contracts

```
T3 → T4 → T5 → T6
```

### Phase 3: FakeHermes + Chat Logic

```
T6 → T7 → T8
```

### Phase 4: Middleware + Chat API (non-streaming)

```
T8 → T9 → T10
```

### Phase 5: Minimal Frontend ← [M1 DEMO after T12]

```
T10 → T11 → T12
```

### Phase 6: Streaming Backend

```
T12 → T13 → T14
```

### Phase 7: Aurora Visual + Full Chat UI ← [M2 DEMO after T18]

```
T12 → T15 → T16 → T17 → T18
T14 → T17
```

### Phase 8: Dashboard Backend

```
T18 → T19 → T20 → T21
```

### Phase 9: Home Screen ← [M3 DEMO after T24]

```
T21 → T22 → T23 → T24
```

### Phase 10: HermesHttpClient + Full API Polish ← [M4 after T26]

```
T24 → T25 → T26
```

### Phase 11: Backend Tests

```
T26 → T27 → T28
```

### Phase 12: Frontend Tests

```
T28 → T29
```

### Phase 13: Docker + CI

```
T29 → T30 → T31 → T32
```

### Phase 14: Documentation

```
T32 → T33 → T34
```

---

## Task Breakdown

---

### T1: Initialize Git Repo and Monorepo Folder Structure

**What**: Create root folder layout, `.gitignore`, `.editorconfig`, and initialize git.
**Where**: `/` (repo root)
**Depends on**: None
**Reuses**: N/A

**Tools**: MCP: NONE / Skill: NONE

**Done when**:
- [x] Root contains `src/`, `tests/`, `docker/`, `docs/adr/`, `scripts/`, `.github/workflows/`
- [x] `.gitignore` covers `bin/`, `obj/`, `node_modules/`, `.env`, `appsettings.*.json` (not default), `*.user`, `dist/`
- [x] `.editorconfig`: indent_style=space, C# indent_size=4, TS/Vue/JSON indent_size=2
- [x] `git init && git commit -m "chore: initialize aurora monorepo"` exits 0

**Tests**: none
**Gate**: Build (backend)

---

### T2: Create .NET Solution and Backend Projects

**What**: Create `Aurora.sln` and all five backend projects with nullable enabled and `net9.0` target.
**Where**: `Aurora.sln`, `src/Aurora.Api/`, `src/Aurora.Application/`, `src/Aurora.Infrastructure/`, `src/Aurora.Domain/`, `src/Aurora.Contracts/`
**Depends on**: T1
**Reuses**: N/A

**Tools**: MCP: NONE / Skill: NONE

**Done when**:
- [x] `Aurora.sln` created with `dotnet new sln -n Aurora`
- [x] Five projects scaffolded: Aurora.Api (webapi), Aurora.Application (classlib), Aurora.Infrastructure (classlib), Aurora.Domain (classlib), Aurora.Contracts (classlib)
- [x] All target `net8.0`; `<Nullable>enable</Nullable>` in every `.csproj`
- [x] All projects added to solution; `dotnet build Aurora.slnx` exits 0

**Tests**: none
**Gate**: Build (backend)

---

### T3: Configure Project References and Core NuGet Packages

**What**: Wire project references and install all required NuGet packages including test projects.
**Where**: All `.csproj` files + `tests/Aurora.UnitTests/` + `tests/Aurora.IntegrationTests/`
**Depends on**: T2
**Reuses**: N/A

**Tools**: MCP: NONE / Skill: NONE

**Done when**:
- [x] References: `Api → Application, Infrastructure, Contracts`; `Application → Domain, Contracts`; `Infrastructure → Application, Domain`
- [x] Aurora.Api NuGets: `Serilog.AspNetCore`, `Serilog.Sinks.Console`, `FluentValidation.AspNetCore`, `Swashbuckle.AspNetCore`, `Microsoft.Extensions.Diagnostics.HealthChecks`
- [x] Aurora.Infrastructure NuGets: `Microsoft.Extensions.Http`
- [x] Test projects in `tests/` with `xunit`, `FluentAssertions`, `NSubstitute`, `Microsoft.AspNetCore.Mvc.Testing`
- [x] `dotnet build Aurora.slnx` exits 0

**Tests**: none
**Gate**: Build (backend)

---

### T4: Define HermesException

**What**: Create `HermesException` in Aurora.Domain.
**Where**: `src/Aurora.Domain/Exceptions/HermesException.cs`
**Depends on**: T3
**Reuses**: N/A

**Tools**: MCP: NONE / Skill: NONE

**Done when**:
- [x] `HermesException : Exception` with `int StatusCode` property
- [x] Constructor `(string message, int statusCode = 0)` — both overloads work
- [x] Build passes

**Tests**: none
**Gate**: Build (backend)

---

### T5: Define Contract Types

**What**: Create all public request/response/DTO types in Aurora.Contracts.
**Where**: `src/Aurora.Contracts/Chat/`, `src/Aurora.Contracts/Dashboard/`
**Depends on**: T4
**Reuses**: N/A

**Tools**: MCP: NONE / Skill: NONE

**Done when**:
- [x] `ChatRequest`: `record ChatRequest(string Message, string? ConversationId)`
- [x] `ChatResponse`: `record ChatResponse(string ConversationId, string Message)`
- [x] `ChatStreamEvent`: `record ChatStreamEvent(string EventType, string? Content, string? Tool, string? ConversationId)`
- [x] `DashboardResponse`: class with nested `AuroraStatus`, `AgendaSummary`, `TasksSummary`, `EmailsSummary`, `SystemStatus` matching AURORA-05 schema
- [x] Build passes

**Tests**: none
**Gate**: Build (backend)

---

### T6: Define Application Interfaces

**What**: Define `IHermesClient`, `ICalendarProvider`, `ITaskProvider`, `IEmailProvider` in Aurora.Application.
**Where**: `src/Aurora.Application/Interfaces/`
**Depends on**: T5
**Reuses**: N/A

**Tools**: MCP: NONE / Skill: NONE

**Done when**:
- [x] `IHermesClient`: `ChatAsync(ChatRequest, CancellationToken): Task<ChatResponse>`, `StreamChatAsync(ChatRequest, CancellationToken): IAsyncEnumerable<ChatStreamEvent>`, `IsHealthyAsync(CancellationToken): Task<bool>`
- [x] `ICalendarProvider`: `GetTodaySummaryAsync(CancellationToken): Task<CalendarSummary>`
- [x] `ITaskProvider`: `GetPendingCountAsync(CancellationToken): Task<TaskSummary>`
- [x] `IEmailProvider`: `GetImportantCountAsync(CancellationToken): Task<EmailSummary>`
- [x] Summary record types defined: `CalendarSummary(int Count, NextEvent? Next)`, `TaskSummary(int Count)`, `EmailSummary(int Count)`
- [x] Build passes

**Tests**: none
**Gate**: Build (backend)

---

### T7: Implement FakeHermesClient

**What**: Implement `FakeHermesClient` that yields a fixed streaming response simulating Aurora's greeting.
**Where**: `src/Aurora.Infrastructure/Hermes/FakeHermesClient.cs`
**Depends on**: T6
**Reuses**: `IHermesClient`, `ChatStreamEvent`, `ChatResponse`

**Tools**: MCP: NONE / Skill: NONE

**Done when**:
- [x] `StreamChatAsync` yields exactly 4 events in order: `message.started` → `message.delta` ("Boa noite! ✨") → `message.delta` ("\n\nEstou online e pronta para ajudar.") → `message.completed` (new Guid ConversationId)
- [x] `ChatAsync` returns a `ChatResponse` with a canned message after 200ms fake delay
- [x] `IsHealthyAsync` always returns `true`
- [x] Unit test: `StreamChatAsync` yields 4 events with correct `EventType` values in order
- [x] Unit test: `ChatAsync` returns non-null, non-empty `ConversationId`
- [x] `dotnet test tests/Aurora.UnitTests` exits 0

**Tests**: unit
**Gate**: Quick (backend)

---

### T8: Implement ChatService

**What**: Implement `ChatService` that wraps `IHermesClient.ChatAsync` and assigns ConversationId when missing.
**Where**: `src/Aurora.Application/Chat/ChatService.cs`
**Depends on**: T7
**Reuses**: `IHermesClient`, `ChatRequest`, `ChatResponse`

**Tools**: MCP: NONE / Skill: NONE

**Done when**:
- [x] `ChatService.ChatAsync(ChatRequest, CancellationToken): Task<ChatResponse>` delegates to `IHermesClient.ChatAsync`
- [x] When `request.ConversationId` is null or empty, assigns a new `Guid.NewGuid().ToString()` before passing to Hermes and returns it in the response
- [x] When `ConversationId` is provided, passes it through unchanged
- [x] Unit test (NSubstitute mocked `IHermesClient`): null ConversationId → new UUID assigned; response ConversationId matches what Hermes returned
- [x] Unit test: provided ConversationId passes through unchanged
- [x] `dotnet test tests/Aurora.UnitTests` exits 0

**Tests**: unit
**Gate**: Quick (backend)

---

### T9: Implement CorrelationId and ExceptionHandler Middleware

**What**: Create `CorrelationIdMiddleware` and `ExceptionHandlerMiddleware`.
**Where**: `src/Aurora.Api/Middleware/CorrelationIdMiddleware.cs`, `src/Aurora.Api/Middleware/ExceptionHandlerMiddleware.cs`
**Depends on**: T8
**Reuses**: `HermesException`

**Tools**: MCP: NONE / Skill: NONE

**Done when**:
- [x] `CorrelationIdMiddleware`: reads `X-Correlation-Id`; generates new UUID if absent; stores in `HttpContext.Items["CorrelationId"]`; attaches to response header
- [x] `ExceptionHandlerMiddleware`: catches unhandled exceptions; logs via `ILogger<ExceptionHandlerMiddleware>` with correlation ID; returns `500 {"error":"An unexpected error occurred","correlationId":"..."}` with no stack trace exposed; maps `HermesException` → `502 {"error":"Hermes is unreachable"}`
- [x] Unit test (CorrelationId): absent header → UUID generated and set in Items + response
- [x] Unit test (CorrelationId): present header → same value echoed in response
- [x] Unit test (ExceptionHandler): unhandled exception → 500 JSON body with `correlationId` field
- [x] Unit test (ExceptionHandler): `HermesException` → 502 JSON body
- [x] `dotnet test tests/Aurora.UnitTests` exits 0

**Tests**: unit
**Gate**: Quick (backend)

---

### T10: Implement ChatController (non-streaming) and Minimal Program.cs

**What**: Create `ChatController` with `POST /api/chat` and wire the minimal `Program.cs` so the backend starts.
**Where**: `src/Aurora.Api/Controllers/ChatController.cs`, `src/Aurora.Api/Program.cs`, `src/Aurora.Api/appsettings.json`
**Depends on**: T9
**Reuses**: `ChatService`, `ChatRequest`, `ChatResponse`

**Tools**: MCP: NONE / Skill: NONE

**Done when**:
- [x] `POST /api/chat`: validates `message` non-empty (FluentValidation); calls `ChatService.ChatAsync`; returns 200 `ChatResponse`; empty message → 400 with validation errors
- [x] `Program.cs`: registers DI (Infrastructure services with `Hermes__UseFake=true` default), CORS (`http://localhost:5173`), FluentValidation, Serilog minimal console, controllers; middleware order: ExceptionHandler → CorrelationId → CORS → Controllers
- [x] `appsettings.json` has `Hermes` section (`BaseUrl`, `UseFake`), `AllowedOrigins` array — no secrets or hardcoded URLs
- [x] Integration test (`WebApplicationFactory`): `POST /api/chat` returns 200 with `conversationId` and `message` (using FakeHermesClient)
- [x] Integration test: empty `message` returns 400
- [x] Integration test: response has `X-Correlation-Id` header
- [x] `dotnet test tests/Aurora.IntegrationTests` exits 0

**Tests**: integration
**Gate**: Full (backend)

---

### T11: Scaffold Vue 3 Frontend and Axios API Client

**What**: Initialize `aurora-web` with all frontend dependencies and create the centralized Axios API client.
**Where**: `src/aurora-web/`
**Depends on**: T10
**Reuses**: N/A

**Tools**: MCP: NONE / Skill: NONE

**Done when**:
- [x] `npm create vite@latest aurora-web -- --template vue-ts` (or equivalent) with TypeScript strict
- [x] Installed: Pinia, Vue Router, Axios, Tailwind CSS (+ Vite plugin or PostCSS), Vitest, @vue/test-utils
- [x] ESLint + Prettier configured
- [x] `src/api/auroraApi.ts`: Axios instance with `baseURL` from `import.meta.env.VITE_AURORA_API_URL` falling back to `http://localhost:8080` with `console.warn`
- [x] `src/api/chatApi.ts`: `chat(request: ChatRequest): Promise<ChatResponse>` using `auroraApi`
- [x] TypeScript strict mode: zero type errors
- [x] `npm run build --prefix src/aurora-web` exits 0

**Tests**: none
**Gate**: Build (frontend)

---

### T12: Minimal ChatView — First Vertical Slice Demo

**What**: Create a minimal chat page at `/chat` that posts to `/api/chat` and displays Aurora's response — first working end-to-end vertical slice.
**Where**: `src/aurora-web/src/views/ChatView.vue`, `src/aurora-web/src/router/index.ts`, `src/aurora-web/src/App.vue`
**Depends on**: T11
**Reuses**: `chatApi.ts`

**Tools**: MCP: NONE / Skill: NONE

**Done when**:
- [x] Router configured: `/` redirects to `/chat` temporarily; `/chat` renders `ChatView`
- [x] `ChatView.vue`: text input + "Enviar" button; on submit calls `chatApi.chat({ message })`; displays Aurora's response below; shows "Aurora está pensando..." while loading; dark background (`#050510`); basic styling (not full glassmorphism yet — that comes in T18)
- [x] `App.vue`: renders `<RouterView />`
- [x] `.env.development` created with `VITE_AURORA_API_URL=http://localhost:8080`
- [x] **MILESTONE 1 DEMO**: `dotnet run` (backend) + `npm run dev` (frontend) → type "Aurora, boa noite." → response appears
- [x] `npm run build --prefix src/aurora-web` exits 0

**Tests**: none
**Gate**: Build (frontend)

---

> **🎯 MILESTONE 1: "Hello Aurora"** — Aurora responds to a typed message. Demo now.

---

### T13: Implement ChatStreamingService

**What**: Implement `ChatStreamingService` that wraps `IHermesClient.StreamChatAsync` and assigns ConversationId.
**Where**: `src/Aurora.Application/Chat/ChatStreamingService.cs`
**Depends on**: T12
**Reuses**: `IHermesClient`, `ChatRequest`, `ChatStreamEvent`

**Tools**: MCP: NONE / Skill: NONE

**Done when**:
- [x] `StreamAsync(ChatRequest, CancellationToken): IAsyncEnumerable<ChatStreamEvent>` delegates to `IHermesClient.StreamChatAsync`
- [x] Assigns `ConversationId` if request's is null (same logic as `ChatService`)
- [x] Unit test (mocked IHermesClient): yields all events from mock; ConversationId assigned when null
- [x] Unit test: provided ConversationId passes through unchanged
- [x] `dotnet test tests/Aurora.UnitTests` exits 0

**Tests**: unit
**Gate**: Quick (backend)

---

### T14: Add ChatController Streaming Endpoint

**What**: Add `POST /api/chat/stream` SSE endpoint to `ChatController`.
**Where**: `src/Aurora.Api/Controllers/ChatController.cs` (modify)
**Depends on**: T13
**Reuses**: `ChatStreamingService`

**Tools**: MCP: NONE / Skill: NONE

**Done when**:
- [x] `POST /api/chat/stream`: sets `Content-Type: text/event-stream; charset=utf-8`; disables response buffering (`IHttpBodyControlFeature`); sets `Cache-Control: no-cache`
- [x] Iterates `ChatStreamingService.StreamAsync`; writes `event: {type}\ndata: {json}\n\n` per event; flushes after each
- [x] `OperationCanceledException` (client disconnect) exits silently
- [x] `HermesException` emits `error` SSE event with safe message, then returns
- [x] Integration test: `POST /api/chat/stream` returns `Content-Type: text/event-stream`; response body contains `message.started` and `message.completed` events from FakeHermesClient
- [x] Integration test: verifies no internal chain-of-thought in any event `Content` field
- [x] `dotnet test tests/Aurora.IntegrationTests` exits 0

**Tests**: integration
**Gate**: Full (backend)

---

### T15: Create AuroraCore.vue Animated Component

**What**: Create the `AuroraCore.vue` CSS/SVG animated visual with all 6 states — no external images.
**Where**: `src/aurora-web/src/components/aurora/AuroraCore.vue`
**Depends on**: T12
**Reuses**: N/A

**Tools**: MCP: NONE / Skill: NONE

**Done when**:
- [x] Accepts `state` prop: `'idle'|'listening'|'thinking'|'speaking'|'working'|'error'`
- [x] 3 concentric SVG/CSS rings; `idle`: slow pulse, electric blue (`#0ea5e9`) glow; `listening`: cyan (`#06b6d4`), faster pulse; `thinking`: purple (`#8b5cf6`), rotation; `speaking`: outward wave; `working`: orbiting dot; `error`: red (`#ef4444`), static
- [x] Pure CSS `@keyframes`; state drives class binding via computed; no external images
- [x] Unit test: renders without errors in all 6 states (mount with each state prop value)
- [x] Unit test: prop `state='thinking'` → root element has class that triggers rotation (assert class name applied)
- [x] `npm run test --prefix src/aurora-web` exits 0

**Tests**: unit
**Gate**: Quick (frontend)

---

### T16: Create Pinia Stores and Enhance Router

**What**: Create `auroraStore`, `chatStore` with state management logic; update router to define `/` and `/chat`.
**Where**: `src/aurora-web/src/stores/aurora.store.ts`, `src/aurora-web/src/stores/chat.store.ts`, `src/aurora-web/src/router/index.ts`
**Depends on**: T15
**Reuses**: N/A

**Tools**: MCP: NONE / Skill: NONE

**Done when**:
- [ ] `auroraStore` (Pinia): `state: AuroraState` (`'idle'|'listening'|'thinking'|'speaking'|'working'|'error'`); `setState(s: AuroraState)` action; initialized to `'idle'`
- [ ] `chatStore` (Pinia): `messages: ChatMessage[]`; `streaming: boolean`; `abortController: AbortController | null`; `addMessage(m)`, `appendDelta(content)` (appends to last assistant message), `setStreaming(b)`, `clearAbort()` actions
- [ ] Router: `/` → `HomeView` (placeholder), `/chat` → `ChatView`; `ChatView` now uses `chatStore` instead of local state
- [ ] Unit test (`auroraStore`): `setState('thinking')` updates state; initialized to `'idle'`
- [ ] Unit test (`chatStore`): `addMessage` appends; `appendDelta` updates last assistant message content; `setStreaming(true)` flips flag
- [ ] `npm run test --prefix src/aurora-web` exits 0

**Tests**: unit
**Gate**: Quick (frontend)

---

### T17: Implement useChat Composable (fetch + ReadableStream SSE)

**What**: Create `useChat.ts` composable that streams from `/api/chat/stream` using `fetch` + `ReadableStream`.
**Where**: `src/aurora-web/src/composables/useChat.ts`
**Depends on**: T14, T16
**Reuses**: `chatStore`, `auroraStore`, `chatApi.ts`

**Tools**: MCP: NONE / Skill: NONE

**Done when**:
- [ ] `sendMessage(message: string)`: sets `auroraStore.state = 'thinking'`; sets `chatStore.streaming = true`; adds user message to store; creates `AbortController`; calls `fetch` POST `/api/chat/stream`; reads body as `ReadableStream<Uint8Array>`; parses SSE lines (`event:` and `data:` pairs) via `TextDecoder`
- [ ] On `message.delta`: calls `chatStore.appendDelta(content)` (progressive rendering)
- [ ] On `tool.started` / `tool.completed`: updates a `activeTool` ref in the store or returns via reactive
- [ ] On `message.completed`: `auroraStore.setState('idle')`; `chatStore.setStreaming(false)`
- [ ] On `error` SSE event or fetch error: adds error message to chat; `auroraStore.setState('error')`; `chatStore.setStreaming(false)`
- [ ] Exposes `activeTool: Ref<string|null>` for `ToolActivity` to consume
- [ ] Unit test: mock `fetch` returns a ReadableStream with FakeHermes SSE bytes → `message.delta` calls `appendDelta` with correct content
- [ ] Unit test: `error` SSE event → error message added to chatStore; aurora state set to `'error'`
- [ ] Unit test: after `message.completed` → auroraStore state is `'idle'`; streaming is false
- [ ] `npm run test --prefix src/aurora-web` exits 0

**Tests**: unit
**Gate**: Quick (frontend)

---

### T18: Build Full Chat UI Components

**What**: Create `ChatMessages`, `ChatMessage`, `ChatInput`, `AuroraThinking`, `ToolActivity`; wire `ChatView` to use streaming and stores.
**Where**: `src/aurora-web/src/components/chat/`, `src/aurora-web/src/views/ChatView.vue` (replace minimal version)
**Depends on**: T17
**Reuses**: `useChat`, `chatStore`, `auroraStore`

**Tools**: MCP: NONE / Skill: NONE

**Done when**:
- [ ] `ChatInput.vue`: styled text input + send button; emits `submit(message)`; disabled while `chatStore.streaming === true`; clears input on submit; dark glassmorphism style
- [ ] `ChatMessage.vue`: accepts `{ role, content }`; different visual styles for user (right-aligned, blue accent) vs assistant (left-aligned, glass card)
- [ ] `ChatMessages.vue`: renders `chatStore.messages` as `ChatMessage` list; auto-scrolls to bottom on new message
- [ ] `AuroraThinking.vue`: shown while `chatStore.streaming === true`; cycles through labels ("Consultando agenda", "Analisando informações", "Preparando resposta") with animated dots
- [ ] `ToolActivity.vue`: displays `activeTool` from `useChat` with icon when non-null
- [ ] `ChatView.vue` (full replacement): renders all components; uses `useChat().sendMessage` on `ChatInput` submit; full dark futuristic aesthetic (glassmorphism, electric blue/purple, generous space)
- [ ] **MILESTONE 2 DEMO**: streaming works — type message → "Aurora está pensando..." → text appears progressively
- [ ] `npm run build --prefix src/aurora-web` exits 0

**Tests**: none
**Gate**: Build (frontend)

---

> **🎯 MILESTONE 2: "Aurora is Streaming"** — Full SSE streaming with animation. Demo now.

---

### T19: Implement Mock Providers and Infrastructure DI Extension

**What**: Create `MockCalendarProvider`, `MockTaskProvider`, `MockEmailProvider` and the `InfrastructureServiceExtensions` DI registration method.
**Where**: `src/Aurora.Infrastructure/Mocks/`, `src/Aurora.Infrastructure/Extensions/InfrastructureServiceExtensions.cs`, `src/Aurora.Infrastructure/Hermes/HermesOptions.cs`
**Depends on**: T18
**Reuses**: `ICalendarProvider`, `ITaskProvider`, `IEmailProvider`, `IHermesClient`, `FakeHermesClient`

**Tools**: MCP: NONE / Skill: NONE

**Done when**:
- [ ] `HermesOptions`: `record HermesOptions { string BaseUrl; bool UseFake; }` bound from `"Hermes"` config section
- [ ] All three mock providers return zero counts and null next event
- [ ] `AddInfrastructure(IConfiguration)` extension registers: `HermesOptions` (validates `BaseUrl` not empty when `UseFake=false`), named HttpClient "hermes", `IHermesClient` → `FakeHermesClient` when `UseFake=true`, all three mock providers
- [ ] Build passes with zero infrastructure warnings
- [ ] `Program.cs` updated to call `services.AddInfrastructure(configuration)` replacing manual registrations from T10

**Tests**: none
**Gate**: Build (backend)

---

### T20: Implement DashboardService

**What**: Implement `DashboardService` aggregating providers and computing time-based greeting.
**Where**: `src/Aurora.Application/Dashboard/DashboardService.cs`
**Depends on**: T19
**Reuses**: `IHermesClient`, `ICalendarProvider`, `ITaskProvider`, `IEmailProvider`, `DashboardResponse`

**Tools**: MCP: NONE / Skill: NONE

**Done when**:
- [ ] `GetDashboardAsync(CancellationToken): Task<DashboardResponse>` calls `IsHealthyAsync` + all providers (parallel `Task.WhenAll`)
- [ ] Greeting: `hour < 12 → "Bom dia"`, `12 ≤ hour < 18 → "Boa tarde"`, `hour ≥ 18 → "Boa noite"` using `DateTime.Now.Hour`
- [ ] `system.hermes` = `"online"` / `"offline"` based on `IsHealthyAsync` result
- [ ] `system.api` always `"online"`; `system.memory` always `"unknown"` in MVP
- [ ] Unit test: greeting returns "Bom dia" for hour 8, "Boa tarde" for hour 14, "Boa noite" for hour 20 (inject clock abstraction or use UTC offset)
- [ ] Unit test: `system.hermes = "offline"` when `IsHealthyAsync` returns false
- [ ] Unit test: all provider values mapped correctly into `DashboardResponse` fields
- [ ] `dotnet test tests/Aurora.UnitTests` exits 0

**Tests**: unit
**Gate**: Quick (backend)

---

### T21: Implement DashboardController and Health Checks

**What**: Create `DashboardController` with `GET /api/dashboard` and wire health checks at `GET /health`.
**Where**: `src/Aurora.Api/Controllers/DashboardController.cs`, `src/Aurora.Api/Program.cs` (update)
**Depends on**: T20
**Reuses**: `DashboardService`

**Tools**: MCP: NONE / Skill: NONE

**Done when**:
- [ ] `GET /api/dashboard` returns 200 with `DashboardResponse` JSON
- [ ] Health checks registered: `aurora-api` (always healthy) + `hermes` (calls `IHermesClient.IsHealthyAsync`)
- [ ] `GET /health` returns ASP.NET Core health check JSON format with both entries
- [ ] Integration test: `GET /api/dashboard` returns 200 with all required fields (`greeting`, `aurora.status`, `agenda`, `tasks`, `emails`, `system`)
- [ ] Integration test: `system.hermes = "online"` with FakeHermesClient
- [ ] Integration test: `GET /health` returns 200 with `aurora-api: Healthy` and `hermes: Healthy`
- [ ] `dotnet test tests/Aurora.IntegrationTests` exits 0

**Tests**: integration
**Gate**: Full (backend)

---

### T22: Create dashboardStore and dashboardApi

**What**: Create `dashboardStore` Pinia store and `dashboardApi.ts` that fetches from `/api/dashboard`.
**Where**: `src/aurora-web/src/stores/dashboard.store.ts`, `src/aurora-web/src/api/dashboardApi.ts`
**Depends on**: T21
**Reuses**: `auroraApi`, `DashboardResponse` TypeScript type

**Tools**: MCP: NONE / Skill: NONE

**Done when**:
- [ ] `dashboardApi.ts`: `getDashboard(): Promise<DashboardResponse>` using `auroraApi.get('/api/dashboard')`
- [ ] `src/aurora-web/src/types/dashboard.ts`: TypeScript interfaces matching `DashboardResponse` schema (greeting, aurora, agenda, tasks, emails, system)
- [ ] `dashboardStore` (Pinia): `data: DashboardResponse | null`; `loading: boolean`; `error: string | null`; `fetchDashboard()` action calling `dashboardApi.getDashboard()` — sets loading, populates data, handles error
- [ ] Unit test: `fetchDashboard()` with mocked `dashboardApi` → sets `data` and clears `loading`
- [ ] Unit test: on fetch error → sets `error` string and clears `loading`
- [ ] `npm run test --prefix src/aurora-web` exits 0

**Tests**: unit
**Gate**: Quick (frontend)

---

### T23: Create HomeView with AuroraCore, Greeting, and VoiceButton

**What**: Create `HomeView.vue` and `VoiceButton.vue` with the futuristic dark AI aesthetic.
**Where**: `src/aurora-web/src/views/HomeView.vue`, `src/aurora-web/src/components/aurora/VoiceButton.vue`
**Depends on**: T22
**Reuses**: `AuroraCore`, `auroraStore`, `dashboardStore`

**Tools**: MCP: NONE / Skill: NONE

**Done when**:
- [ ] `HomeView.vue`: `onMounted` calls `dashboardStore.fetchDashboard()`; renders greeting from `dashboardStore.data?.greeting` + "Milena." suffix; renders `<AuroraCore :state="auroraStore.state" />` at center; renders `<VoiceButton />`; dark layout with generous whitespace, electric blue/cyan/purple palette, glassmorphism accents
- [ ] `VoiceButton.vue`: microphone icon button; `@mousedown` → `auroraStore.setState('listening')`; `@mouseup` / `@mouseleave` → `auroraStore.setState('idle')`; visual glow when listening
- [ ] Unit test (`VoiceButton`): mousedown → store state becomes `'listening'`; mouseup → state becomes `'idle'`
- [ ] `npm run test --prefix src/aurora-web` exits 0

**Tests**: unit
**Gate**: Quick (frontend)

---

### T24: Create Context Cards, ActivityFeed, and AudioVisualizer

**What**: Create `SystemStatusCard`, `NextEventCard`, `TasksCard`, `ProjectFocusCard`, `ActivityFeed`, and `AudioVisualizer`.
**Where**: `src/aurora-web/src/components/dashboard/`, `src/aurora-web/src/components/shared/`
**Depends on**: T23
**Reuses**: `dashboardStore`

**Tools**: MCP: NONE / Skill: NONE

**Done when**:
- [ ] `SystemStatusCard.vue`: reads `dashboardStore.data.system`; displays status dot (green = online, red = offline) for Aurora, Hermes, API; shows "Hermes parece estar offline" message when hermes = "offline"
- [ ] `NextEventCard.vue`: shows `agenda.nextEvent` or "Nenhum evento hoje" empty state
- [ ] `TasksCard.vue`: shows `tasks.pending` count or "Sem tarefas pendentes"
- [ ] `ProjectFocusCard.vue`: placeholder "Em breve" for MVP
- [ ] `ActivityFeed.vue`: 3 mocked entries (timestamp + emoji + label); empty state "Nenhuma atividade recente" when array empty
- [ ] `AudioVisualizer.vue`: animated bars shown while `auroraStore.state === 'listening'`; CSS-only animation
- [ ] Cards wired into `HomeView.vue`
- [ ] Unit test (`SystemStatusCard`): renders "Aurora Online" and "Hermes Online" when both online; renders error message when `hermes = "offline"`
- [ ] Unit test (`ActivityFeed`): renders 3 entries with mocked data; renders empty state with empty array
- [ ] **MILESTONE 3 DEMO**: Home screen complete with greeting, AuroraCore, status, cards
- [ ] `npm run test --prefix src/aurora-web` exits 0

**Tests**: unit
**Gate**: Quick (frontend)

---

> **🎯 MILESTONE 3: "Dashboard & Home"** — Full home screen working. Demo now.

---

### T25: Implement HermesHttpClient

**What**: Implement `HermesHttpClient` — real HTTP integration with Hermes Agent.
**Where**: `src/Aurora.Infrastructure/Hermes/HermesHttpClient.cs`
**Depends on**: T24
**Reuses**: `IHermesClient`, `HermesOptions`, `HermesException`, `ChatRequest`, `ChatStreamEvent`

**Tools**: MCP: NONE / Skill: NONE

**Done when**:
- [ ] Constructor receives `IOptions<HermesOptions>` and `IHttpClientFactory`; throws `InvalidOperationException` if `BaseUrl` is null/empty when `UseFake=false`
- [ ] `ChatAsync`: POSTs to `{BaseUrl}/api/chat`; on non-2xx status throws `HermesException(message, (int)statusCode)`
- [ ] `StreamChatAsync`: POSTs with `HttpCompletionOption.ResponseHeadersRead`; reads body line-by-line as SSE; parses `event:` and `data:` lines; yields `ChatStreamEvent`; on `OperationCanceledException` stops cleanly
- [ ] `IsHealthyAsync`: GETs `{BaseUrl}/health`; returns `true` on 2xx; returns `false` on any exception (never throws)
- [ ] DI extension (`AddInfrastructure`) registers `HermesHttpClient` when `UseFake=false`
- [ ] Unit test (mocked `HttpMessageHandler`): non-2xx ChatAsync → throws `HermesException`
- [ ] Unit test: `IsHealthyAsync` returns `false` when HTTP call throws `HttpRequestException`
- [ ] `dotnet test tests/Aurora.UnitTests` exits 0

**Tests**: unit
**Gate**: Quick (backend)

---

### T26: Full Program.cs Polish — Serilog, Swagger, Auth Stub

**What**: Complete `Program.cs` with structured Serilog logging, Swagger, and auth preparation stub.
**Where**: `src/Aurora.Api/Program.cs`, `src/Aurora.Api/appsettings.json`, `src/Aurora.Api/appsettings.Development.json`
**Depends on**: T25
**Reuses**: All middleware and services

**Tools**: MCP: NONE / Skill: NONE

**Done when**:
- [ ] Serilog: console sink with structured output; request logging enriched with `CorrelationId`; minimum level configurable from `appsettings.json`
- [ ] Swagger/OpenAPI: title "Aurora API", version "v1"; accessible at `/swagger` in development
- [ ] Middleware order finalized: ExceptionHandler → CorrelationId → CORS → Swagger (dev) → Controllers
- [ ] `appsettings.Development.json` has `Hermes__UseFake=true` (no Hermes needed for local dev)
- [ ] Auth stub: `app.UseAuthorization()` wired but no policy enforced (controllers remain anonymous for MVP)
- [ ] Integration test: `GET /health` still returns healthy; `GET /api/dashboard` returns full response
- [ ] **MILESTONE 4**: set `Hermes__UseFake=false` + `Hermes__BaseUrl=<real>` in env → real Hermes responds
- [ ] `dotnet build Aurora.sln && dotnet test Aurora.sln` exits 0

**Tests**: integration
**Gate**: Build (backend)

---

> **🎯 MILESTONE 4: "Real Hermes Ready"** — Switch config to connect to real Hermes.

---

### T27: Complete Backend Unit Tests

**What**: Review and complete unit test coverage for all spec ACs not yet covered in previous tasks.
**Where**: `tests/Aurora.UnitTests/`
**Depends on**: T26
**Reuses**: All unit test files from T7–T25

**Tools**: MCP: NONE / Skill: NONE

**Done when**:
- [ ] All ACs for AURORA-02 (Hermes client), AURORA-03 (chat), AURORA-05 (dashboard), AURORA-07 (observability) have at least one unit test with `file:line` evidence
- [ ] Edge cases covered: empty message, Hermes offline in DashboardService, greeting at boundary hours (11, 12, 17, 18), HermesException propagation
- [ ] `dotnet test tests/Aurora.UnitTests` exits 0 with ≥ 25 tests passing

**Tests**: unit
**Gate**: Quick (backend)

---

### T28: Complete Backend Integration Tests

**What**: Review and complete integration test coverage for all endpoints and error paths.
**Where**: `tests/Aurora.IntegrationTests/`
**Depends on**: T27
**Reuses**: `WebApplicationFactory`, all controller tests

**Tools**: MCP: NONE / Skill: NONE

**Done when**:
- [ ] All routes tested: `POST /api/chat` (200 + 400), `POST /api/chat/stream` (SSE events present + no internal reasoning), `GET /api/dashboard` (full schema), `GET /health` (both entries)
- [ ] CORS test: request from `http://localhost:5173` receives `Access-Control-Allow-Origin` header
- [ ] Hermes-offline path: DashboardService with `IsHealthyAsync=false` → `system.hermes = "offline"` in response
- [ ] `dotnet test tests/Aurora.IntegrationTests` exits 0

**Tests**: integration
**Gate**: Full (backend)

---

### T29: Frontend Unit Tests — Complete Coverage

**What**: Complete frontend unit tests for all components and composables per the Test Coverage Matrix.
**Where**: `src/aurora-web/src/**/*.spec.ts`
**Depends on**: T28
**Reuses**: All existing spec files from T15–T24

**Tools**: MCP: NONE / Skill: NONE

**Done when**:
- [ ] `ChatInput.spec.ts`: submit event emitted with correct message; input cleared after submit; button disabled while streaming
- [ ] `AuroraThinking.spec.ts`: shown when `streaming=true`; hidden when `streaming=false`
- [ ] `useChat.spec.ts`: all 3 SSE paths covered (delta, completed, error) per T17 tests — review and fill any gaps
- [ ] `dashboardStore.spec.ts`: fetchDashboard success and error paths — review T22 tests, fill gaps
- [ ] Total frontend tests: ≥ 20 passing
- [ ] `npm run test --prefix src/aurora-web` exits 0

**Tests**: unit
**Gate**: Quick (frontend)

---

### T30: Full Frontend Build Gate

**What**: Run complete frontend quality gate: lint, tests, and production build.
**Where**: `src/aurora-web/`
**Depends on**: T29
**Reuses**: N/A

**Done when**:
- [ ] `npm run lint --prefix src/aurora-web` exits 0 (zero ESLint errors)
- [ ] `npm run test --prefix src/aurora-web` exits 0 (all tests passing)
- [ ] `npm run build --prefix src/aurora-web` exits 0 (zero TypeScript errors; dist/ generated)
- [ ] No hardcoded API URLs, secrets, or tokens in any frontend source file

**Tests**: none
**Gate**: Build (frontend)

---

### T31: Create Dockerfiles and docker-compose.yml

**What**: Create multi-stage Dockerfiles for backend and frontend, and a docker-compose.yml.
**Where**: `docker/Dockerfile.api`, `docker/Dockerfile.web`, `docker-compose.yml`
**Depends on**: T30
**Reuses**: N/A

**Tools**: MCP: NONE / Skill: NONE

**Done when**:
- [ ] `Dockerfile.api`: multi-stage (sdk → publish → runtime `mcr.microsoft.com/dotnet/aspnet:9.0`); exposes port 8080; `ENTRYPOINT ["dotnet", "Aurora.Api.dll"]`
- [ ] `Dockerfile.web`: multi-stage (node:20-alpine build → nginx:alpine serve); nginx serves `dist/` on port 80
- [ ] `docker-compose.yml`: `aurora-api` (8080:8080), `aurora-web` (5173:80); `Hermes__BaseUrl`, `Hermes__UseFake`, `AllowedOrigins`, `VITE_AURORA_API_URL` as env vars; no secrets in file
- [ ] `docker compose config` exits 0 (valid compose file)

**Tests**: none
**Gate**: Build (full)

---

### T32: Create GitHub Actions CI Pipeline

**What**: Create `.github/workflows/ci.yml` that builds, tests, and lints on every push and PR.
**Where**: `.github/workflows/ci.yml`
**Depends on**: T31
**Reuses**: N/A

**Tools**: MCP: NONE / Skill: NONE

**Done when**:
- [ ] Triggered on `push` and `pull_request` to `main`
- [ ] Jobs in order: `backend-build` (restore + `dotnet build`) → `backend-test` (`dotnet test` with `Hermes__UseFake=true`) → `frontend-install` (`npm ci`) → `frontend-lint` (`npm run lint`) → `frontend-test` (`npm run test`) → `frontend-build` (`npm run build`)
- [ ] Each job depends on its predecessor; pipeline fails fast
- [ ] Uses `actions/checkout@v4`, `actions/setup-dotnet@v4` (9.0), `actions/setup-node@v4` (20), `--prefix src/aurora-web` for npm commands
- [ ] YAML is valid (no syntax errors)

**Tests**: none
**Gate**: Build (full)

---

### T33: Write README

**What**: Create comprehensive `README.md` with all sections required by AURORA-19.
**Where**: `README.md`
**Depends on**: T32
**Reuses**: Design and spec artifacts

**Done when**:
- [ ] Sections: Project Vision, Architecture (text diagram matching design.md), Repository Structure, Tech Stack, How to Run (local dev + Docker), Environment Variables table, Hermes Integration guide, API Endpoints table (`POST /api/chat`, `POST /api/chat/stream`, `GET /api/dashboard`, `GET /health`), Roadmap (Phases 1–5)
- [ ] Environment variable table includes: `Hermes__BaseUrl`, `Hermes__UseFake`, `AllowedOrigins`, `VITE_AURORA_API_URL`
- [ ] File is complete and non-empty

**Tests**: none
**Gate**: Build (full)

---

### T34: Write Architecture Decision Records

**What**: Create the four ADR files in `docs/adr/`.
**Where**: `docs/adr/0001-use-vue.md`, `docs/adr/0002-use-dotnet-api.md`, `docs/adr/0003-use-sse-for-chat-streaming.md`, `docs/adr/0004-use-hermes-as-agent-runtime.md`
**Depends on**: T33
**Reuses**: STATE.md decisions

**Done when**:
- [ ] All four ADRs have Context, Decision, and Consequences sections
- [ ] ADR-003 references the fetch+ReadableStream pattern for POST SSE in the frontend
- [ ] Final gate: `dotnet build Aurora.sln && dotnet test Aurora.sln && npm run build --prefix src/aurora-web` exits 0

**Tests**: none
**Gate**: Build (full)

---

## Task Granularity Check

| Task | Scope | Status |
| ---- | ----- | ------ |
| T1: Git + folders | 1 setup | ✅ Granular |
| T2: Solution + 5 projects | 1 scaffold | ✅ Granular |
| T3: Project refs + packages | 1 config layer | ✅ Granular |
| T4: HermesException | 1 file | ✅ Granular |
| T5: Contracts | 1 layer, cohesive types | ✅ Granular |
| T6: Application interfaces | 1 layer, 4 related interfaces | ✅ Granular |
| T7: FakeHermesClient | 1 class | ✅ Granular |
| T8: ChatService | 1 class | ✅ Granular |
| T9: CorrelationId + ExceptionHandler | 2 cohesive middleware files | ✅ Granular |
| T10: ChatController + Program.cs minimal | 1 endpoint + minimal wiring | ✅ Granular |
| T11: Vue scaffold + API client | 1 scaffold + 2 API files | ✅ Granular |
| T12: Minimal ChatView | 1 minimal view (replaced in T18) | ✅ Granular |
| T13: ChatStreamingService | 1 class | ✅ Granular |
| T14: ChatController streaming | 1 endpoint (modifies existing controller) | ✅ Granular |
| T15: AuroraCore.vue | 1 component | ✅ Granular |
| T16: Pinia stores + Router | 2 stores + router (cohesive state layer) | ✅ Granular |
| T17: useChat.ts | 1 composable | ✅ Granular |
| T18: Full Chat UI | 5 display components + ChatView wiring | ✅ Granular |
| T19: Mock providers + DI ext | 1 concern (infra registration) | ✅ Granular |
| T20: DashboardService | 1 class | ✅ Granular |
| T21: DashboardController + health | 1 controller + health wiring | ✅ Granular |
| T22: dashboardStore + dashboardApi | 1 store + 1 API file (cohesive) | ✅ Granular |
| T23: HomeView + VoiceButton | 1 view + 1 related component | ✅ Granular |
| T24: Context cards + ActivityFeed + AudioVisualizer | 6 small display components (1 concern) | ✅ Granular |
| T25: HermesHttpClient | 1 class | ✅ Granular |
| T26: Full Program.cs polish | 1 file (host wiring) | ✅ Granular |
| T27: Backend unit test review | 1 review pass | ✅ Granular |
| T28: Backend integration test review | 1 review pass | ✅ Granular |
| T29: Frontend test review | 1 review pass | ✅ Granular |
| T30: Frontend build gate | 1 gate | ✅ Granular |
| T31: Docker + compose | 1 infra concern | ✅ Granular |
| T32: CI pipeline | 1 file | ✅ Granular |
| T33: README | 1 doc | ✅ Granular |
| T34: ADRs | 4 small docs (1 concern) | ✅ Granular |

---

## Diagram-Definition Cross-Check

| Task | Depends On (body) | Diagram Shows | Status |
| ---- | ----------------- | ------------- | ------ |
| T1 | None | Phase 1 start | ✅ Match |
| T2 | T1 | T1 → T2 | ✅ Match |
| T3 | T2 | T2 → T3 | ✅ Match |
| T4 | T3 | T3 → T4 | ✅ Match |
| T5 | T4 | T4 → T5 | ✅ Match |
| T6 | T5 | T5 → T6 | ✅ Match |
| T7 | T6 | T6 → T7 | ✅ Match |
| T8 | T7 | T7 → T8 | ✅ Match |
| T9 | T8 | T8 → T9 | ✅ Match |
| T10 | T9 | T9 → T10 | ✅ Match |
| T11 | T10 | T10 → T11 | ✅ Match |
| T12 | T11 | T11 → T12 | ✅ Match |
| T13 | T12 | T12 → T13 | ✅ Match |
| T14 | T13 | T13 → T14 | ✅ Match |
| T15 | T12 | T12 → T15 | ✅ Match |
| T16 | T15 | T15 → T16 | ✅ Match |
| T17 | T14, T16 | T14 → T17, T16 → T17 | ✅ Match |
| T18 | T17 | T17 → T18 | ✅ Match |
| T19 | T18 | T18 → T19 | ✅ Match |
| T20 | T19 | T19 → T20 | ✅ Match |
| T21 | T20 | T20 → T21 | ✅ Match |
| T22 | T21 | T21 → T22 | ✅ Match |
| T23 | T22 | T22 → T23 | ✅ Match |
| T24 | T23 | T23 → T24 | ✅ Match |
| T25 | T24 | T24 → T25 | ✅ Match |
| T26 | T25 | T25 → T26 | ✅ Match |
| T27 | T26 | T26 → T27 | ✅ Match |
| T28 | T27 | T27 → T28 | ✅ Match |
| T29 | T28 | T28 → T29 | ✅ Match |
| T30 | T29 | T29 → T30 | ✅ Match |
| T31 | T30 | T30 → T31 | ✅ Match |
| T32 | T31 | T31 → T32 | ✅ Match |
| T33 | T32 | T32 → T33 | ✅ Match |
| T34 | T33 | T33 → T34 | ✅ Match |

---

## Test Co-location Validation

| Task | Code Layer | Matrix Requires | Task Says | Status |
| ---- | ---------- | --------------- | --------- | ------ |
| T7: FakeHermesClient | Infrastructure | unit | unit | ✅ OK |
| T8: ChatService | Application service | unit | unit | ✅ OK |
| T9: Middlewares | Middleware | unit | unit | ✅ OK |
| T10: ChatController | Controller/endpoint | integration | integration | ✅ OK |
| T13: ChatStreamingService | Application service | unit | unit | ✅ OK |
| T14: ChatController streaming | Controller/endpoint | integration | integration | ✅ OK |
| T15: AuroraCore | Vue component w/ logic | unit | unit | ✅ OK |
| T16: Pinia stores | Vue stores | unit | unit | ✅ OK |
| T17: useChat | Vue composable | unit | unit | ✅ OK |
| T18: Chat UI components | Vue display-only | none | none | ✅ OK |
| T20: DashboardService | Application service | unit | unit | ✅ OK |
| T21: DashboardController + health | Controller/endpoint | integration | integration | ✅ OK |
| T22: dashboardStore | Vue stores | unit | unit | ✅ OK |
| T23: HomeView + VoiceButton | VoiceButton = Vue component w/ logic | unit | unit | ✅ OK |
| T24: Context cards | SystemStatusCard + ActivityFeed = Vue components w/ logic | unit | unit | ✅ OK |
| T25: HermesHttpClient | Infrastructure | unit | unit | ✅ OK |
| T26: Program.cs polish | Config + wiring | integration | integration | ✅ OK |
| T1-T6, T11-T12, T19, T23(HomeView), T24(display cards), T27-T34 | Scaffold/Config/Docs/Gates | none | none | ✅ OK |
