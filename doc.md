# Projeto Aurora AI

Você é um arquiteto de software e engenheiro full stack sênior.

Sua tarefa é criar a fundação técnica do projeto **Aurora AI**, uma assistente pessoal de inteligência artificial inspirada na experiência do JARVIS.

Não queremos apenas um chatbot.

Aurora deve evoluir para um **Personal AI Operating System**, conectando:

* interface;
* chat;
* voz;
* memória;
* contexto;
* agenda;
* e-mail;
* tarefas;
* ferramentas;
* MCP;
* Hermes Agent;
* automações;
* notificações;
* agentes especializados.

O projeto deve começar simples, mas sua arquitetura precisa permitir essa evolução sem grandes reescritas.

---

# Objetivo inicial

Criar um MVP funcional contendo:

1. Frontend Vue 3.
2. Backend ASP.NET Core.
3. Comunicação entre frontend e backend.
4. Backend preparado para comunicação com Hermes Agent.
5. Chat com streaming.
6. Dashboard inicial da Aurora.
7. Health checks.
8. Estrutura preparada para memória, agenda, e-mail, tarefas e notificações.
9. Docker para desenvolvimento.
10. Testes.
11. Documentação.

---

# Tecnologias

## Frontend

Utilizar:

* Vue 3
* TypeScript
* Vite
* Pinia
* Vue Router
* Tailwind CSS
* Axios
* Composition API

Preferir:

```text
<script setup lang="ts">
```

Não utilizar Options API.

---

# Backend

Utilizar:

* .NET 10 ou versão LTS mais atual disponível no ambiente
* ASP.NET Core Web API
* C#
* Dependency Injection nativa
* HttpClientFactory
* FluentValidation
* Serilog
* OpenAPI / Swagger
* Health Checks

Para testes:

* xUnit
* FluentAssertions
* NSubstitute ou Moq

---

# Arquitetura geral

Implementar:

```text
┌────────────────────────────────────────────┐
│                Aurora Web                  │
│            Vue 3 + TypeScript              │
└───────────────────┬────────────────────────┘
                    │
               HTTPS / SSE
                    │
┌───────────────────▼────────────────────────┐
│                Aurora API                  │
│             ASP.NET Core                   │
│                                            │
│  Chat                                      │
│  Dashboard                                 │
│  Memory                                    │
│  Calendar                                  │
│  Tasks                                     │
│  Notifications                             │
│  System                                    │
└───────────────────┬────────────────────────┘
                    │
              Hermes Client
                    │
┌───────────────────▼────────────────────────┐
│              Hermes Agent                  │
│                                            │
│ SOUL.md                                    │
│ USER.md                                    │
│ MEMORY.md                                  │
│ Skills                                     │
│ MCP                                        │
│ Tools                                      │
│ Agents                                     │
└────────────────────────────────────────────┘
```

O frontend NÃO deve acessar Hermes diretamente.

Toda comunicação deve passar pelo `Aurora API`.

---

# Estrutura do repositório

Criar um monorepo:

```text
aurora/
│
├── src/
│   │
│   ├── Aurora.Api/
│   │
│   ├── Aurora.Application/
│   │
│   ├── Aurora.Domain/
│   │
│   ├── Aurora.Infrastructure/
│   │
│   └── Aurora.Contracts/
│   │
│   └── aurora-web/
│
├── tests/
│   │
│   ├── Aurora.UnitTests/
│   └── Aurora.IntegrationTests/
│
├── docker/
│
├── docs/
│
├── scripts/
│
├── .github/
│   └── workflows/
│
├── docker-compose.yml
├── .editorconfig
├── .gitignore
├── README.md
└── Aurora.sln
```

---

# Backend

Utilizar uma arquitetura pragmática inspirada em Clean Architecture.

Não criar abstrações sem necessidade.

Responsabilidades:

## Aurora.Api

Responsável por:

* Controllers / Endpoints
* Middleware
* autenticação
* Swagger
* Health Checks
* SSE
* configuração da aplicação

---

## Aurora.Application

Responsável por:

* casos de uso;
* serviços de aplicação;
* handlers;
* validações;
* interfaces necessárias pela aplicação.

Exemplos:

```text
Chat/
Dashboard/
Memory/
Calendar/
Tasks/
Notifications/
```

---

## Aurora.Domain

Deve conter apenas regras centrais e entidades quando forem realmente necessárias.

Evitar criar entidades artificiais apenas para seguir Clean Architecture.

---

## Aurora.Infrastructure

Responsável por integrações externas.

Criar inicialmente:

```text
Hermes/
Persistence/
Logging/
```

Posteriormente:

```text
Google/
Gmail/
Calendar/
GitHub/
Notifications/
Voice/
```

---

## Aurora.Contracts

Conter:

* requests;
* responses;
* DTOs;
* eventos públicos da API.

---

# Integração Hermes

Criar uma abstração:

```csharp
public interface IHermesClient
{
    Task<ChatResponse> ChatAsync(
        ChatRequest request,
        CancellationToken cancellationToken);

    IAsyncEnumerable<ChatStreamEvent> StreamChatAsync(
        ChatRequest request,
        CancellationToken cancellationToken);

    Task<bool> IsHealthyAsync(
        CancellationToken cancellationToken);
}
```

Criar implementação:

```text
HermesHttpClient
```

A URL do Hermes deverá vir por configuração:

```text
Hermes__BaseUrl
```

Nunca colocar URLs hardcoded.

---

# Chat

Criar endpoint:

```http
POST /api/chat
```

Request:

```json
{
  "message": "Como está meu dia?",
  "conversationId": "opcional"
}
```

Response:

```json
{
  "conversationId": "uuid",
  "message": "Resposta da Aurora"
}
```

---

# Streaming

Criar:

```http
POST /api/chat/stream
```

Utilizar Server-Sent Events.

Eventos sugeridos:

```text
message.started
message.delta
tool.started
tool.completed
message.completed
error
```

Exemplo:

```text
event: message.delta
data: {"content":"Bom dia"}

event: tool.started
data: {"tool":"calendar"}

event: message.completed
data: {"conversationId":"..."}
```

Não enviar raciocínio interno ou chain-of-thought.

Mostrar somente ações e estados seguros para o usuário.

---

# Dashboard

Criar endpoint:

```http
GET /api/dashboard
```

Modelo inicial:

```json
{
  "greeting": "Boa noite",
  "aurora": {
    "status": "online"
  },
  "agenda": {
    "eventsToday": 0,
    "nextEvent": null
  },
  "tasks": {
    "pending": 0
  },
  "emails": {
    "important": 0
  },
  "system": {
    "api": "online",
    "hermes": "online",
    "memory": "unknown"
  }
}
```

Neste primeiro momento, dados que ainda não tiverem integração podem utilizar providers mockados.

Arquitetar de forma que posteriormente os mocks possam ser substituídos pelas integrações reais.

---

# Health Checks

Criar:

```http
GET /health
```

Verificar:

* Aurora API
* Hermes Agent

Retornar status individual.

---

# Frontend

Criar aplicação chamada:

```text
aurora-web
```

Estrutura sugerida:

```text
src/
│
├── api/
│   ├── auroraApi.ts
│   ├── chatApi.ts
│   └── dashboardApi.ts
│
├── assets/
│
├── components/
│   │
│   ├── aurora/
│   ├── chat/
│   ├── dashboard/
│   └── shared/
│
├── composables/
│
├── layouts/
│
├── router/
│
├── stores/
│
├── types/
│
├── views/
│
├── App.vue
└── main.ts
```

---

# Estado global

Utilizar Pinia.

Criar inicialmente:

```text
auroraStore
chatStore
dashboardStore
```

---

# Interface

A UI NÃO deve parecer um dashboard administrativo tradicional.

Aurora deve parecer uma inteligência artificial viva.

Direção visual:

```text
futuristic
minimal
dark
glass
holographic
AI command center
```

Características:

* fundo escuro;
* tons azul elétrico;
* cyan;
* roxo;
* transparência;
* glassmorphism;
* glow sutil;
* partículas sutis;
* animações leves;
* elementos orbitais;
* HUD;
* bastante espaço vazio;
* tipografia moderna.

Evitar:

* aparência de CRM;
* sidebar administrativa tradicional;
* dezenas de cards;
* tabelas na tela principal;
* excesso de informação.

---

# Home

A tela principal deve possuir uma área central representando Aurora.

Exemplo conceitual:

```text
                   ◉

               A U R O R A

           Boa noite, Milena.

       Como posso ajudar você?


          [ 🎙 Falar comigo ]


 Hoje                       Sistema

 3 compromissos             ● Aurora Online
 4 tarefas                  ● Hermes Online
 2 emails                   ● API Online
```

---

# Aurora Core Visual

Criar componente:

```text
AuroraCore.vue
```

Ele deve representar visualmente Aurora.

Estados:

```text
idle
listening
thinking
speaking
working
error
```

O componente pode utilizar:

* círculos;
* ondas;
* glow;
* rotações;
* pulsos;
* partículas.

Não utilizar imagens externas neste momento.

Preferir CSS/SVG.

---

# Chat

Criar página:

```text
/chat
```

Componentes:

```text
ChatView
ChatMessages
ChatMessage
ChatInput
AuroraThinking
ToolActivity
```

O chat deve suportar streaming.

Enquanto Aurora estiver trabalhando, exibir algo parecido com:

```text
Aurora está trabalhando

● Consultando agenda
● Analisando informações
◌ Preparando resposta
```

Nunca mostrar raciocínio interno.

---

# Activity Feed

Criar componente:

```text
ActivityFeed
```

Exemplo:

```text
18:32  📅 Agenda consultada
18:31  🧠 Memória utilizada
18:30  🔍 Pesquisa realizada
```

No MVP, poderá utilizar dados mockados.

---

# Context Cards

Criar componentes pequenos para:

```text
NextEventCard
TasksCard
SystemStatusCard
ProjectFocusCard
```

Esses cards devem ser minimalistas.

---

# API client

Criar configuração centralizada de Axios.

Variável:

```text
VITE_AURORA_API_URL
```

Nunca colocar URL da API diretamente nos componentes.

---

# Tratamento de erros

Criar tratamento consistente de:

* timeout;
* API offline;
* Hermes offline;
* erro de rede;
* resposta inválida.

Exemplo visual:

```text
Não consegui falar com o núcleo da Aurora.

Hermes parece estar offline.
```

---

# Observabilidade

Backend:

* Serilog
* correlation ID
* request logging
* exception handler centralizado

Todo request deverá possuir:

```text
X-Correlation-Id
```

Se não for recebido, criar automaticamente.

---

# Segurança

Não armazenar:

* API keys;
* tokens;
* passwords;
* secrets;

no código.

Utilizar:

```text
.env
appsettings.Development.json
environment variables
```

Adicionar secrets ao `.gitignore`.

---

# CORS

Configurar CORS apenas para as origens necessárias.

Desenvolvimento:

```text
http://localhost:5173
```

---

# Docker

Criar:

```text
Dockerfile backend
Dockerfile frontend
docker-compose.yml
```

Serviços:

```text
aurora-api
aurora-web
```

Hermes inicialmente pode continuar rodando externamente.

A URL deve ser configurável.

---

# Desenvolvimento local

Esperado:

```bash
docker compose up
```

ou:

```bash
dotnet run
```

e:

```bash
npm install
npm run dev
```

---

# Qualidade

Ativar:

* nullable reference types;
* warnings importantes;
* eslint;
* prettier;
* TypeScript strict;
* editorconfig.

---

# Testes Backend

Criar testes para:

```text
ChatService
DashboardService
HermesClient
HealthCheck
```

Mockar Hermes quando necessário.

---

# Testes Frontend

Utilizar:

* Vitest
* Vue Test Utils

Criar pelo menos testes básicos para:

```text
AuroraCore
ChatInput
SystemStatus
```

---

# GitHub Actions

Criar pipeline:

```text
restore
build
backend tests
frontend install
frontend lint
frontend tests
frontend build
```

Falhar pipeline quando testes falharem.

---

# README

Criar documentação completa contendo:

* visão do projeto;
* arquitetura;
* estrutura;
* tecnologias;
* como executar;
* variáveis de ambiente;
* integração Hermes;
* endpoints;
* roadmap.

---

# ADR

Criar:

```text
docs/adr/
```

Adicionar:

```text
0001-use-vue.md
0002-use-dotnet-api.md
0003-use-sse-for-chat-streaming.md
0004-use-hermes-as-agent-runtime.md
```

Cada ADR deverá explicar:

```text
Context
Decision
Consequences
```

---

# Roadmap

Adicionar no README:

## Phase 1

```text
Frontend
Aurora API
Hermes Chat
Streaming
Dashboard
```

## Phase 2

```text
Memory
Google Calendar
Gmail
Tasks
Notifications
```

## Phase 3

```text
Voice
Speech-to-Text
Text-to-Speech
Wake Word
```

## Phase 4

```text
Skills
Subagents
Automations
Proactive Aurora
```

## Phase 5

```text
Mobile/PWA
Home Assistant
Personal AI OS
```

---

# Preparação para voz

Não implementar toda a infraestrutura de voz agora.

Porém preparar o frontend para possuir:

```text
VoiceButton
AudioVisualizer
Aurora listening state
Aurora speaking state
```

---

# Preparação para autenticação

No MVP local não é obrigatório implementar login completo.

Porém o backend deve estar arquitetado para posteriormente suportar:

```text
JWT
OAuth
Google Authentication
```

Não acoplar identidade da usuária diretamente às controllers.

---

# Convenções

C#:

```text
PascalCase para classes
camelCase para variáveis
Async suffix em métodos async
CancellationToken nos métodos async
```

Vue:

```text
PascalCase components
camelCase variables
Composition API
script setup
TypeScript strict
```

---

# Princípios arquiteturais

Priorizar:

1. simplicidade;
2. legibilidade;
3. testabilidade;
4. observabilidade;
5. segurança;
6. baixo acoplamento;
7. evolução incremental.

Evitar:

* overengineering;
* abstrações sem uso;
* generic repository;
* event sourcing no MVP;
* microservices;
* Kubernetes;
* brokers;
* CQRS complexo.

Aurora ainda é um projeto pessoal.

Começar como **modular monolith**.

Separar serviços somente quando houver motivo real.

---

# Resultado esperado

Ao finalizar a primeira implementação, deve ser possível:

1. iniciar backend;
2. iniciar frontend;
3. abrir Aurora Web;
4. visualizar status do sistema;
5. escrever uma mensagem;
6. frontend enviar para Aurora API;
7. Aurora API enviar para Hermes;
8. resposta aparecer progressivamente na tela;
9. visualizar activity states;
10. detectar quando Hermes estiver offline.

---

# Primeiro fluxo

O fluxo de demonstração deve ser:

Usuária digita:

```text
Aurora, boa noite.
```

UI:

```text
Aurora está pensando...
```

Backend:

```text
Aurora Web
    ↓
POST /api/chat/stream
    ↓
Aurora API
    ↓
HermesHttpClient
    ↓
Hermes
```

Resposta aparece via streaming:

```text
Boa noite! ✨

Estou online e pronta para ajudar.
```

---

# Importante

Não apenas gere uma estrutura vazia.

Implemente um **vertical slice funcional completo**:

```text
Frontend
   ↓
Aurora API
   ↓
Hermes
   ↓
stream
   ↓
Frontend
```

Se a integração real com Hermes não puder ser concluída imediatamente por falta de configuração específica do ambiente, criar:

```text
IHermesClient
HermesHttpClient
HermesHttpClient
```

E permitir alternar via configuração:

```text
Hermes__BaseUrl=https://your-hermes-host
```

Assim o frontend deve funcionar mesmo antes da integração real.

---

# Modo de execução

Execute o trabalho, não apenas explique o que deveria ser feito.

Ordem:

1. analisar o repositório atual;
2. criar solution;
3. criar projetos backend;
4. configurar referências;
5. implementar API;
6. implementar Hermes Client;
7. criar testes;
8. criar frontend;
9. implementar layout;
10. implementar chat;
11. implementar streaming;
12. integrar frontend/backend;
13. adicionar Docker;
14. adicionar CI;
15. escrever documentação;
16. executar builds;
17. executar testes;
18. corrigir erros encontrados.

Ao final apresentar:

```text
Arquivos criados
Arquitetura implementada
Como executar
Como configurar Hermes
Testes executados
Pendências
Próximos passos
```

Não interrompa a implementação para pedir autorização a cada arquivo.

Quando houver uma decisão pequena de arquitetura, escolha a alternativa mais simples e documente.

O objetivo é entregar uma primeira versão executável da **Aurora AI**.
