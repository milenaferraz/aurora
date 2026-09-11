# Aurora AI MVP Specification

## Problem Statement

Milena needs a personal AI assistant that feels alive — not a chatbot dashboard. Aurora must serve as the foundation for a Personal AI Operating System, starting as a functional MVP that connects a futuristic Vue 3 frontend to an ASP.NET Core backend, which in turn delegates intelligence to Hermes Agent via streaming. The architecture must support future integrations (voice, memory, calendar, email, tasks) without major rewrites.

## Goals

- [ ] Deliver a vertical slice: user types a message → frontend sends to Aurora API → API sends to Hermes → streaming response appears progressively in the UI
- [ ] Establish a modular monolith backend (Aurora.Api, Aurora.Application, Aurora.Infrastructure, Aurora.Domain, Aurora.Contracts) that can evolve without restructuring
- [ ] Create a futuristic, dark AI interface with AuroraCore visual component and a chat page with live streaming
- [ ] Provide a dashboard endpoint with system status and mocked data for future integrations
- [ ] Ship with Docker Compose, CI pipeline, health checks, and documentation

## Out of Scope

| Feature | Reason |
| ------- | ------- |
| User authentication / JWT / OAuth | Architecture prepared but not implemented in MVP |
| Google Calendar integration | Phase 2 |
| Gmail integration | Phase 2 |
| Voice / Speech-to-Text / TTS | Phase 3 |
| MCP integration beyond Hermes | Phase 4+ |
| Kubernetes / microservices | Not this project |
| CQRS complex / Event Sourcing | Overengineering for MVP |
| Generic repository pattern | Explicitly excluded by doc |
| Real memory/calendar/email data | Mocked providers for MVP |
| Mobile / PWA | Phase 5 |

---

## Assumptions & Open Questions

| Assumption / decision | Chosen default | Rationale | Confirmed? |
| --------------------- | -------------- | --------- | ---------- |
| .NET version | .NET 9 LTS (if 10 unavailable in env) | doc.md says ".NET 10 or latest LTS"; check env at build time | y |
| Hermes BaseUrl default | `http://localhost:5000` (configurable via `Hermes__BaseUrl`) | Never hardcoded; doc specifies env-var approach | y |
| HermesHttpClient behavior | Streams 3 SSE delta events then a completed event with a canned response | Enables full frontend demo without real Hermes | y |
| Auth middleware | Stub middleware wired (no logic); controllers accept anonymous | Prepared but not blocking MVP | y |
| Dashboard mock providers | Return zeroed counts / null next event | Real integrations replace providers later | y |
| CORS dev origin | `http://localhost:5173` (Vite default) | doc specifies this explicitly | y |
| Frontend port | 5173 (Vite default) | Standard; configurable via env | y |
| SSE content type | `text/event-stream; charset=utf-8` | Standard SSE spec | y |
| Correlation ID header | `X-Correlation-Id` — generated if absent | doc specifies this explicitly | y |
| Activity feed in MVP | Mocked static data | doc permits mock for MVP | y |
| Git initialized | Yes, monorepo at repo root | doc specifies monorepo structure | y |

**Open questions:** none — all resolved or logged above.

---

## User Stories

### P1: Backend Solution & Project Structure ⭐ MVP

**User Story**: As a developer, I want a solution with all backend projects and their references configured so that I can build the entire backend with `dotnet build`.

**Why P1**: Nothing else runs without this foundation.

**Acceptance Criteria**:

1. The system SHALL contain projects Aurora.Api, Aurora.Application, Aurora.Infrastructure, Aurora.Domain, and Aurora.Contracts in `src/`, plus Aurora.UnitTests and Aurora.IntegrationTests in `tests/`.
2. WHEN `dotnet build Aurora.sln` runs THEN the system SHALL exit with code 0 and zero errors.
3. The system SHALL have project references: Api → Application → Infrastructure → Domain, and Contracts referenced by Api and Application.
4. The system SHALL enable nullable reference types and treat selected warnings as errors in all projects.

**Independent Test**: `dotnet build` exits 0.

---

### P1: Hermes Client Abstraction ⭐ MVP

**User Story**: As a developer, I want a clean IHermesClient interface with HttpClient and Fake implementations so that the frontend works end-to-end even when Hermes is not configured.

**Why P1**: Enables the vertical slice with or with Hermes configured.

**Acceptance Criteria**:

1. The system SHALL define `IHermesClient` with `ChatAsync`, `StreamChatAsync`, and `IsHealthyAsync` methods in Aurora.Application.
2. The system SHALL provide `HermesHttpClient` implementing `IHermesClient`, reading `Hermes__BaseUrl` from configuration — never hardcoded.
3. 5. IF `Hermes__BaseUrl` is missing or empty THEN the system SHALL throw a configuration exception at startup with a descriptive message.
6. IF Hermes returns a non-2xx response THEN `HermesHttpClient` SHALL throw a domain exception that the API layer translates to a 502 response.

**Independent Test**: Unit test `HermesHttpClient` with mocked `HttpMessageHandler` handles non-2xx correctly.

---

### P1: Chat API Endpoint ⭐ MVP

**User Story**: As the Aurora Web frontend, I want to POST a message to `/api/chat` and receive a JSON response with a conversation ID and Aurora's reply.

**Why P1**: Core chat capability (non-streaming path).

**Acceptance Criteria**:

1. WHEN `POST /api/chat` receives `{ "message": "<text>", "conversationId": "<optional>" }` THEN the system SHALL return `200 OK` with `{ "conversationId": "<uuid>", "message": "<reply>" }`.
2. IF `message` is null or empty THEN the system SHALL return `400 Bad Request` with a validation error body.
3. The system SHALL assign a new UUID as `conversationId` when none is provided.
4. The system SHALL pass a `CancellationToken` to `IHermesClient.ChatAsync`.
5. WHEN the request includes `X-Correlation-Id` THEN the system SHALL echo the same ID in the response header; WHEN absent the system SHALL generate and attach a new UUID.

**Independent Test**: Integration test hitting the endpoint with HermesHttpClient returns 200 with valid body; missing message returns 400.

---

### P1: Chat Streaming Endpoint ⭐ MVP

**User Story**: As the Aurora Web frontend, I want to POST to `/api/chat/stream` and receive a Server-Sent Events stream so that Aurora's response appears progressively.

**Why P1**: Core UX requirement — streaming is the main interaction mode.

**Acceptance Criteria**:

1. WHEN `POST /api/chat/stream` receives a valid request THEN the system SHALL respond with `Content-Type: text/event-stream` and stream SSE events.
2. The system SHALL emit events of types: `message.started`, `message.delta`, `tool.started`, `tool.completed`, `message.completed`, and `error`.
3. The system SHALL NOT emit chain-of-thought or internal reasoning in any event payload.
4. WHILE streaming is in progress THEN the system SHALL flush each event immediately (no buffering).
5. IF the client disconnects THEN the system SHALL cancel the `CancellationToken` and stop streaming.
6. IF Hermes is unreachable THEN the system SHALL emit a single `error` event with a safe user-facing message and close the stream.

**Independent Test**: Integration test reads SSE events from the endpoint using HermesHttpClient; verifies event order and no internal reasoning in payloads.

---

### P1: Dashboard API Endpoint ⭐ MVP

**User Story**: As the Aurora Web frontend, I want to GET `/api/dashboard` and receive the greeting, Aurora status, and system status so that I can render the home screen.

**Why P1**: Home screen cannot render without this data.

**Acceptance Criteria**:

1. WHEN `GET /api/dashboard` is called THEN the system SHALL return `200 OK` with a body matching the schema: `greeting`, `aurora.status`, `agenda.eventsToday`, `agenda.nextEvent`, `tasks.pending`, `emails.important`, `system.api`, `system.hermes`, `system.memory`.
2. The system SHALL compute `greeting` based on the current server hour ("Bom dia" / "Boa tarde" / "Boa noite").
3. The system SHALL check `IHermesClient.IsHealthyAsync` to populate `system.hermes` ("online" / "offline").
4. WHERE mock providers are used the system SHALL return `0` for all counts and `null` for `nextEvent`.
5. The system SHALL be architected so that mock providers can be replaced by real integrations without modifying the controller or service signature.

**Independent Test**: Unit test DashboardService with mocked IHermesClient; integration test endpoint returns valid schema.

---

### P1: Health Check Endpoint ⭐ MVP

**User Story**: As an operator, I want `GET /health` to return individual health status for Aurora API and Hermes so that I can detect when any component is offline.

**Why P1**: Required for system status display on the home screen and for operations.

**Acceptance Criteria**:

1. WHEN `GET /health` is called THEN the system SHALL return `200 OK` with individual status entries for `aurora-api` and `hermes`.
2. The system SHALL return `Healthy` for `aurora-api` when the API process is running.
3. WHEN Hermes is reachable THEN system SHALL return `Healthy` for `hermes`; WHEN unreachable SHALL return `Unhealthy`.
4. The response SHALL conform to the ASP.NET Core Health Checks JSON format (status, entries, totalDuration).

**Independent Test**: Integration test hits `/health` with HermesHttpClient returning healthy; verifies both entries present.

---

### P1: Observability — Correlation ID & Structured Logging ⭐ MVP

**User Story**: As a developer, I want every HTTP request to carry a correlation ID and structured Serilog logs so that I can trace issues across components.

**Why P1**: Foundation for debugging in a distributed-ish system.

**Acceptance Criteria**:

1. The system SHALL log all HTTP requests with method, path, status code, duration, and correlation ID using Serilog.
2. WHEN a request arrives without `X-Correlation-Id` THEN the system SHALL generate a new UUID and attach it to the request context and response header.
3. WHEN a request arrives with `X-Correlation-Id` THEN the system SHALL propagate that ID through all log entries for that request.
4. The system SHALL have a centralized exception handler that logs unhandled exceptions and returns a safe `500` JSON response without stack traces.

**Independent Test**: Middleware unit test verifies ID generation and propagation; integration test verifies header is present in response.

---

### P1: CORS Configuration ⭐ MVP

**User Story**: As the frontend developer, I want the backend to allow requests from `http://localhost:5173` in development so that the browser does not block API calls.

**Why P1**: Without CORS the frontend cannot call the backend.

**Acceptance Criteria**:

1. The system SHALL configure CORS to allow requests from the origins listed in `AllowedOrigins` configuration.
2. WHEN the request origin is `http://localhost:5173` in development THEN the system SHALL allow the request.
3. The system SHALL NOT allow all origins via wildcard in production configuration.

**Independent Test**: Integration test with CORS headers verifies allowed origin gets `Access-Control-Allow-Origin` in response.

---

### P1: Vue 3 Frontend Foundation ⭐ MVP

**User Story**: As Milena, I want the frontend scaffolded with Vue 3, TypeScript, Vite, Pinia, Vue Router, and Tailwind CSS so that I can build on a proper foundation.

**Why P1**: Nothing else in the frontend runs without this.

**Acceptance Criteria**:

1. The system SHALL initialize `aurora-web` using Vite with Vue 3, TypeScript strict mode, and Composition API (`<script setup lang="ts">`).
2. The system SHALL configure Tailwind CSS, ESLint, and Prettier.
3. The system SHALL configure Axios with a base URL from `VITE_AURORA_API_URL` — never hardcoded in components.
4. The system SHALL expose Pinia stores: `auroraStore`, `chatStore`, `dashboardStore`.
5. The system SHALL configure Vue Router with routes for `/` (home) and `/chat`.
6. WHEN `npm run build` executes THEN the system SHALL exit with code 0.

**Independent Test**: `npm run build` exits 0; TypeScript strict mode has zero type errors.

---

### P1: Aurora Home Screen ⭐ MVP

**User Story**: As Milena, I want the home screen to display a futuristic dark interface with AuroraCore, a greeting, and system status cards so that Aurora feels alive.

**Why P1**: This is the primary interface impression.

**Acceptance Criteria**:

1. The system SHALL render a home view (`/`) with a dark background, electric blue / cyan / purple color scheme, and glassmorphism elements.
2. The system SHALL render `AuroraCore.vue` — a CSS/SVG animated component with `idle`, `listening`, `thinking`, `speaking`, `working`, and `error` states — using no external images.
3. WHEN the page loads THEN `AuroraCore` SHALL display in `idle` state with a subtle pulse animation.
4. The system SHALL display a greeting from the dashboard API ("Boa noite, Milena."), system status (Aurora Online / Hermes Online / API Online), and summary counts (events, tasks, emails).
5. The system SHALL NOT resemble a traditional CRM dashboard (no white backgrounds, no administrative sidebar, no data tables on the main screen).
6. WHEN Hermes is offline THEN the system SHALL display "Hermes parece estar offline" in the system status area.

**Independent Test**: Component test renders `AuroraCore` in each state without errors; visual check of home screen in browser.

---

### P1: Chat Page with Streaming ⭐ MVP

**User Story**: As Milena, I want to type a message on `/chat`, send it, and see Aurora's response appear progressively with activity states while she works.

**Why P1**: Core interaction mode.

**Acceptance Criteria**:

1. WHEN the user navigates to `/chat` THEN the system SHALL render `ChatView` with `ChatMessages`, `ChatInput`, `AuroraThinking`, and `ToolActivity` components.
2. WHEN the user submits a message THEN the system SHALL POST to `/api/chat/stream` and render each `message.delta` event progressively in `ChatMessages`.
3. WHILE Aurora is processing THEN the system SHALL display `AuroraThinking` with activity states (e.g., "Consultando agenda", "Analisando informações", "Preparando resposta").
4. The system SHALL NOT display internal reasoning or chain-of-thought in any chat message.
5. WHEN a `tool.started` or `tool.completed` event arrives THEN `ToolActivity` SHALL display the tool name and state.
6. IF the SSE connection is lost or an `error` event is received THEN the system SHALL display "Não consegui falar com o núcleo da Aurora." in the chat.
7. WHEN streaming completes (`message.completed`) THEN `AuroraThinking` SHALL hide and the complete message SHALL remain in `ChatMessages`.

**Independent Test**: Component test simulates SSE events and verifies progressive rendering; error event triggers error message.

---

### P1: Activity Feed Component ⭐ MVP

**User Story**: As Milena, I want to see a timestamped activity feed showing what Aurora has done (agenda consulted, memory used, etc.) so that I understand what's happening.

**Why P1**: Required by doc as MVP component (may use mocked data).

**Acceptance Criteria**:

1. The system SHALL render `ActivityFeed` with timestamped entries showing tool/action names.
2. WHEN no activity has occurred THEN `ActivityFeed` SHALL display an empty state without errors.
3. WHERE mocked data is used the system SHALL show at least 3 sample entries on first render.

**Independent Test**: Component test renders feed with mocked data and empty state.

---

### P1: Context Cards ⭐ MVP

**User Story**: As Milena, I want small minimal context cards for next event, tasks, and system status so that I have at-a-glance information without visual noise.

**Why P1**: Required by doc as MVP components.

**Acceptance Criteria**:

1. The system SHALL render `NextEventCard`, `TasksCard`, `SystemStatusCard`, and `ProjectFocusCard` as minimal, glassmorphism-styled components.
2. WHEN dashboard data is loading THEN cards SHALL display a skeleton/loading state.
3. WHEN dashboard data is unavailable THEN cards SHALL display a graceful empty state.

**Independent Test**: Component tests render each card in loading, empty, and populated states.

---

### P1: Voice Preparation ⭐ MVP

**User Story**: As a developer, I want the frontend to include `VoiceButton` and `AudioVisualizer` component shells so that voice can be wired in Phase 3 without UI rewrites.

**Why P1**: doc specifies this is required even without voice logic.

**Acceptance Criteria**:

1. The system SHALL render `VoiceButton` — a button that transitions `auroraStore.state` to `listening` on press and back to `idle` on release (no actual audio capture in MVP).
2. The system SHALL render `AudioVisualizer` — a placeholder component with a visual bar/wave that is shown while `auroraStore.state === 'listening'`.

**Independent Test**: Component test verifies state transitions on VoiceButton interaction.

---

### P2: Docker Compose

**User Story**: As a developer, I want `docker compose up` to start both `aurora-api` and `aurora-web` so that I can run the full stack without installing .NET or Node locally.

**Why P2**: Improves developer experience; not blocking MVP local dev.

**Acceptance Criteria**:

1. WHEN `docker compose up` runs THEN the system SHALL start `aurora-api` on port 8080 and `aurora-web` on port 5173.
2. The system SHALL configure `Hermes__BaseUrl` and `AllowedOrigins` as environment variables in `docker-compose.yml` — never hardcoded.
3. WHEN `Hermes__BaseUrl` is set THEN the stack SHALL work without an external Hermes instance.

**Independent Test**: `docker compose up` starts both services; `curl http://localhost:8080/health` returns healthy.

---

### P2: Backend Tests

**User Story**: As a developer, I want unit and integration tests for ChatService, DashboardService, HermesClient, and HealthCheck so that I can trust the backend logic.

**Why P2**: Quality gate; Hermes mocked in tests.

**Acceptance Criteria**:

1. The system SHALL have xUnit tests for `ChatService` covering chat request forwarding and error handling.
2. The system SHALL have xUnit tests for `DashboardService` covering greeting logic and hermes status mapping.
3. The system SHALL have xUnit tests for `HermesHttpClient` (mocked HttpMessageHandler) covering success and non-2xx paths.
4. The system SHALL have xUnit tests for `HermesHttpClient` verifying it yields the correct SSE event sequence.
5. The system SHALL have an integration test for the `/health` endpoint verifying both entries are present.
6. WHEN `dotnet test` runs THEN the system SHALL exit with code 0 and all tests pass.

**Independent Test**: `dotnet test` exits 0.

---

### P2: Frontend Tests

**User Story**: As a developer, I want Vitest tests for AuroraCore, ChatInput, and SystemStatus so that regressions are caught.

**Why P2**: Quality gate.

**Acceptance Criteria**:

1. The system SHALL use Vitest and Vue Test Utils.
2. The system SHALL have tests for `AuroraCore` verifying it renders in all 6 states without errors.
3. The system SHALL have tests for `ChatInput` verifying message submit event and disabled state while streaming.
4. The system SHALL have tests for `SystemStatusCard` verifying online/offline state display.
5. WHEN `npm run test` runs THEN the system SHALL exit with code 0 and all tests pass.

**Independent Test**: `npm run test` exits 0.

---

### P2: GitHub Actions CI Pipeline

**User Story**: As a developer, I want a CI pipeline that builds, tests, and lints both frontend and backend on every push so that broken code is caught automatically.

**Why P2**: Standard quality gate.

**Acceptance Criteria**:

1. The system SHALL have a `.github/workflows/ci.yml` pipeline with jobs: restore, build, backend-test, frontend-install, frontend-lint, frontend-test, frontend-build.
2. WHEN backend tests fail THEN the pipeline SHALL fail.
3. WHEN frontend lint fails THEN the pipeline SHALL fail.
4. WHEN frontend build fails THEN the pipeline SHALL fail.

**Independent Test**: Pipeline file is valid YAML; jobs defined with correct steps.

---

### P2: Documentation (README + ADRs)

**User Story**: As a developer, I want a README and ADRs so that I understand how to run Aurora and why key decisions were made.

**Why P2**: Required by doc; must be complete.

**Acceptance Criteria**:

1. The system SHALL have a `README.md` with: project vision, architecture diagram, repo structure, tech stack, how-to-run instructions, environment variables, Hermes integration guide, API endpoints, and roadmap.
2. The system SHALL have `docs/adr/0001-use-vue.md`, `0002-use-dotnet-api.md`, `0003-use-sse-for-chat-streaming.md`, and `0004-use-hermes-as-agent-runtime.md`, each with Context, Decision, and Consequences sections.

**Independent Test**: Files exist and are non-empty.

---

## Edge Cases

- IF `message` field is empty string (not null) THEN system SHALL return `400 Bad Request`.
- IF Hermes stream yields no events within a timeout THEN system SHALL emit an `error` SSE event and close the connection.
- WHEN `conversationId` is provided THEN system SHALL pass it through to Hermes and return it in the response.
- IF `VITE_AURORA_API_URL` is not set THEN frontend SHALL fall back to `http://localhost:8080` with a console warning.
- WHEN SSE stream is active and user navigates away THEN frontend SHALL close the EventSource connection.

---

## Requirement Traceability

| Requirement ID | Story | Phase | Status |
| -------------- | ----- | ----- | ------ |
| AURORA-01 | P1: Backend Solution Structure | Design | Pending |
| AURORA-02 | P1: Hermes Client Abstraction | Design | Pending |
| AURORA-03 | P1: Chat API Endpoint | Design | Pending |
| AURORA-04 | P1: Chat Streaming Endpoint | Design | Pending |
| AURORA-05 | P1: Dashboard API Endpoint | Design | Pending |
| AURORA-06 | P1: Health Check Endpoint | Design | Pending |
| AURORA-07 | P1: Observability | Design | Pending |
| AURORA-08 | P1: CORS Configuration | Design | Pending |
| AURORA-09 | P1: Vue 3 Frontend Foundation | Design | Pending |
| AURORA-10 | P1: Aurora Home Screen | Design | Pending |
| AURORA-11 | P1: Chat Page with Streaming | Design | Pending |
| AURORA-12 | P1: Activity Feed Component | Design | Pending |
| AURORA-13 | P1: Context Cards | Design | Pending |
| AURORA-14 | P1: Voice Preparation | Design | Pending |
| AURORA-15 | P2: Docker Compose | Design | Pending |
| AURORA-16 | P2: Backend Tests | Design | Pending |
| AURORA-17 | P2: Frontend Tests | Design | Pending |
| AURORA-18 | P2: GitHub Actions CI | Design | Pending |
| AURORA-19 | P2: Documentation | Design | Pending |

**Coverage:** 19 total, 0 mapped to tasks, 19 unmapped ⚠️

---

## Success Criteria

- [ ] `dotnet build Aurora.sln` exits 0
- [ ] `dotnet test` exits 0 with all tests passing
- [ ] `npm run build` exits 0 with zero TypeScript errors
- [ ] `npm run test` exits 0 with all tests passing
- [ ] `docker compose up` starts both services and `/health` returns healthy for both
- [ ] User can type "Aurora, boa noite." in the chat and see a progressive streaming response
- [ ] When `Hermes__BaseUrl` is configured, the full vertical slice works end-to-end with Hermes
- [ ] Home screen visually matches the futuristic dark AI aesthetic described in doc.md
- [ ] Hermes offline state is detectable and shown in the UI
