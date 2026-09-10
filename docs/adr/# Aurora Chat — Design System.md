# Aurora Chat — Design System

## 1. Conceito

A página `/chat` deve transmitir:

**Presença + Inteligência + Energia + Movimento + Voz**

A referência conceitual é uma interface de IA cinematográfica, porém com identidade própria da Aurora.

A tela deve parecer um **núcleo de inteligência ativo**, não um chat convencional.

---

# 2. Direção visual

Combinação principal:

```text
Deep Space
+
Holographic HUD
+
Aurora Borealis
+
Neural Energy
+
Voice Reactive Interface
```

Características:

* fundo extremamente escuro;
* muito espaço negativo;
* elementos centrais;
* iluminação azul, roxa e rosa;
* partículas sutis;
* linhas orbitais;
* elementos HUD discretos;
* animação contínua;
* sensação de profundidade.

---

# 3. Paleta principal

## Background

```css
--aurora-bg-deep: #01040D;
--aurora-bg-primary: #020617;
--aurora-bg-secondary: #050B18;
--aurora-bg-surface: #081020;
```

Evitar preto puro.

---

# 4. Cores da Aurora

## Cyan / Intelligence

```css
--aurora-cyan: #38BDF8;
```

## Electric Blue

```css
--aurora-blue: #2563EB;
```

## Violet

```css
--aurora-violet: #7C3AED;
```

## Purple

```css
--aurora-purple: #A855F7;
```

## Pink

```css
--aurora-pink: #EC4899;
```

## Light Pink

```css
--aurora-pink-light: #F472B6;
```

---

# 5. Gradient principal

```css
--aurora-gradient:
linear-gradient(
  135deg,
  #38BDF8 0%,
  #2563EB 25%,
  #7C3AED 50%,
  #A855F7 70%,
  #EC4899 100%
);
```

Gradient de energia:

```css
--aurora-energy-gradient:
linear-gradient(
  90deg,
  #38BDF8,
  #6366F1,
  #A855F7,
  #F472B6
);
```

---

# 6. Tipografia

Fonte principal:

```text
Inter
```

Fonte opcional para HUD:

```text
Space Grotesk
```

Logo:

```text
A U R O R A
```

Letter spacing:

```css
letter-spacing: 0.32em;
```

Labels HUD:

```css
font-size: 10px;
letter-spacing: 0.24em;
text-transform: uppercase;
```

---

# 7. Hierarquia tipográfica

```text
Logo                20px
Core title          18px
Main status         16px
Body                14px
HUD labels          10px
Metadata            11px
```

A página deve ter pouco texto.

O núcleo visual é o protagonista.

---

# 8. Aurora Core

O componente central deve ser:

```text
AuroraCore
```

Ele representa a própria Aurora.

Estrutura visual:

```text
Outer Orbit
    ↓
Energy Ring
    ↓
Neural Ribbon
    ↓
Core Glow
    ↓
Aurora State
```

Visual conceitual:

```text
          ·       ·
      ◌────────────◌
    ╱                ╲
   ◌     ∞ ENERGY     ◌
    ╲                ╱
      ◌────────────◌
          AURORA
```

---

# 9. Aurora Core — camadas

## Layer 1 — Outer Orbit

Linhas extremamente finas.

Cor:

```css
rgba(99, 102, 241, 0.24)
```

Rotação:

```text
20–30 segundos
```

---

## Layer 2 — Energy Orbit

Gradient:

```text
Blue → Purple → Pink
```

Rotação:

```text
8–14 segundos
```

Glow forte somente nesse layer.

---

## Layer 3 — Neural Ribbon

Forma irregular.

Representa atividade da IA.

Movimento:

```text
morph
rotate
breathe
```

---

## Layer 4 — Core

Parte central.

Background:

```css
radial-gradient(
  circle,
  rgba(30,41,59,.8),
  rgba(2,6,23,.94)
);
```

---

# 10. Estados da Aurora

```ts
type AuroraState =
  | 'idle'
  | 'listening'
  | 'thinking'
  | 'working'
  | 'speaking'
  | 'offline';
```

---

# 11. Idle

Texto:

```text
A U R O R A
```

Subtexto:

```text
Pronta.
```

Movimento:

* respiração suave;
* rotação lenta;
* poucas partículas.

Gradient:

```text
Blue + Purple
```

---

# 12. Listening

Texto:

```text
Estou ouvindo...
```

Predominância:

```text
Purple + Pink
```

Comportamento:

* core aumenta levemente;
* rings pulsam;
* waveform reage;
* glow fica mais forte;
* partículas aproximam-se do centro.

---

# 13. Thinking

Texto:

```text
Pensando...
```

Predominância:

```text
Blue + Violet
```

Comportamento:

* rings rotacionam em velocidades diferentes;
* energia converge para o centro;
* pequenos pontos orbitam;
* brilho central aumenta.

---

# 14. Working

Texto:

```text
Executando...
```

Gradient completo.

Comportamento:

* atividade mais intensa;
* mais partículas;
* pequenos pulsos externos.

---

# 15. Speaking

Texto:

```text
Aurora está falando
```

Predominância:

```text
Pink + Purple + Blue
```

Comportamento:

* ring reage ao áudio;
* expansão radial;
* waveform circular;
* glow sincronizado com amplitude.

---

# 16. Animação de fala

A animação de fala deve reagir à amplitude de áudio.

Exemplo de comportamento:

```text
volume baixo
    ↓
core scale 1.01

volume médio
    ↓
core scale 1.05

volume alto
    ↓
core scale 1.09
```

Aplicar também em:

```text
glow intensity
ring displacement
waveform amplitude
```

Evitar vibração brusca.

---

# 17. Voice Visualizer

Criar:

```text
AuroraVoiceVisualizer
```

Pode existir como ring em torno do core.

Estrutura:

```text
32–64 segmentos
```

Cada segmento reage ao volume/frequência.

Visual:

```text
        │ │
     │       │
   │           │
  │    CORE     │
   │           │
     │       │
        │ │
```

---

# 18. Fundo

O fundo deve ser profundo e pouco chamativo.

Base:

```css
background:
radial-gradient(
  circle at 50% 35%,
  rgba(124,58,237,.12),
  transparent 34%
),
radial-gradient(
  circle at 70% 70%,
  rgba(236,72,153,.06),
  transparent 28%
),
#01040D;
```

---

# 19. Estrelas

Quantidade:

```text
baixa
```

Opacidade:

```text
0.08–0.25
```

Movimento:

```text
extremamente lento
```

Nunca parecer screensaver.

---

# 20. Partículas

Partículas devem existir principalmente perto do Aurora Core.

Tamanhos:

```text
1px
2px
3px
```

Cores:

```text
Blue
Purple
Pink
```

Algumas partículas podem usar glow.

---

# 21. HUD

HUD deve ser discreto.

Exemplos:

```text
ONLINE

PENSAR
PLANEJAR
REALIZAR
EVOLUIR
```

ou:

```text
AURORA SYSTEM
CORE ONLINE
VOICE READY
```

Não usar muita informação operacional.

---

# 22. Layout

Desktop:

```text
┌────────────────────────────────────────────┐
│ AURORA                         ONLINE      │
│                                            │
│ PENSAR                   STATUS / PROFILE  │
│ PLANEJAR                                   │
│ REALIZAR                                   │
│ EVOLUIR                 quote / HUD        │
│                                            │
│              AURORA CORE                   │
│                                            │
│                                            │
│          [ Fale com a Aurora... ]          │
│                                            │
│        Inteligência em movimento           │
└────────────────────────────────────────────┘
```

---

# 23. Sem botões de ação

Nesta página não usar:

* Conversar
* Ideias
* Código
* Pesquisa
* Imagem
* Cards
* Quick actions

A interação principal deve ser:

```text
Aurora Core
+
Chat Input
+
Voice Button
```

---

# 24. Chat Input

Formato:

```text
pill
```

Altura:

```text
64–72px
```

Background:

```css
rgba(8, 15, 30, 0.72)
```

Border:

```text
gradient sutil
```

Radius:

```text
9999px
```

---

# 25. Chat Input — Focus

Quando usuário clicar:

```text
border glow
```

Gradient:

```text
Blue → Purple → Pink
```

Glow:

```css
0 0 30px rgba(168,85,247,.12)
```

---

# 26. Voice Button

Formato:

```text
circular
```

Tamanho:

```text
52–60px
```

Gradient:

```text
Purple → Pink
```

Glow:

```text
moderado
```

---

# 27. Voice Button Listening

Quando estiver ouvindo:

```text
pulse
+
outer ring
+
wave
```

Animação:

```text
1.5–2 segundos
```

---

# 28. Conversa

Quando houver mensagens, não substituir completamente o núcleo.

O Aurora Core pode reduzir:

```text
500px → 240px
```

e subir para o topo.

Layout:

```text
          Aurora Core
             ↓
       Conversation
             ↓
          Input
```

---

# 29. Mensagem do usuário

Alinhamento:

```text
right
```

Background:

```css
rgba(37,99,235,.12)
```

Border:

```css
rgba(59,130,246,.20)
```

---

# 30. Mensagem da Aurora

Alinhamento:

```text
left
```

Background:

```css
rgba(168,85,247,.08)
```

Border:

```css
rgba(168,85,247,.18)
```

---

# 31. Aurora pensando

Nunca mostrar spinner.

Mostrar o Core reagindo.

Opcionalmente:

```text
Aurora está pensando...
```

com três pontos animados.

---

# 32. Transições

Padrão:

```text
200–300ms
```

Transições cinematográficas:

```text
500–700ms
```

Exemplo:

```text
idle
 ↓
listening
 ↓
thinking
 ↓
speaking
```

deve acontecer suavemente.

---

# 33. Animações oficiais

Criar:

```text
aurora-breathe
aurora-orbit
aurora-orbit-reverse
aurora-energy-flow
aurora-wave
aurora-listen
aurora-speak
aurora-glow
aurora-particle
aurora-core-enter
```

---

# 34. Glow principal

Blue:

```css
0 0 30px rgba(56,189,248,.18)
```

Purple:

```css
0 0 45px rgba(168,85,247,.22)
```

Pink:

```css
0 0 40px rgba(236,72,153,.18)
```

---

# 35. Glow do Core

Pode ser mais forte:

```css
box-shadow:
0 0 40px rgba(59,130,246,.25),
0 0 90px rgba(168,85,247,.20),
0 0 140px rgba(236,72,153,.10);
```

---

# 36. Bordas HUD

```css
rgba(148,163,184,.08)
```

Muito sutis.

---

# 37. Glass

Usar somente em:

```text
chat input
message surfaces
status pill
```

Não envolver o Aurora Core em card.

---

# 38. Logo

O logo no topo deve usar o mesmo DNA do Core.

Símbolo:

```text
3 energy loops
```

Cores:

```text
Blue
Purple
Pink
```

O logo deve parecer uma versão reduzida do Aurora Core.

---

# 39. Princípio do logo

```text
Aurora logo = Aurora Core em miniatura
```

Isso cria consistência de marca.

---

# 40. Responsividade

Mobile:

```text
Aurora Core menor
HUD lateral oculto
status compacto
input fixado embaixo
```

O Core continua sendo protagonista.

---

# 41. Reduced Motion

Obrigatório:

```css
@media (prefers-reduced-motion: reduce)
```

Desabilitar:

* partículas;
* orbit;
* pulse;
* parallax.

Manter apenas mudanças de estado simples.

---

# 42. Áudio

Quando houver integração real com voz:

```text
Web Audio API
```

Fluxo:

```text
audio output
    ↓
AnalyserNode
    ↓
frequency data
    ↓
AuroraVoiceVisualizer
    ↓
Core animation
```

---

# 43. Arquitetura de componentes

```text
components/
└── aurora/
    ├── AuroraLogo.vue
    ├── AuroraCore.vue
    ├── AuroraOrbit.vue
    ├── AuroraParticles.vue
    ├── AuroraVoiceVisualizer.vue
    ├── AuroraChatInput.vue
    ├── AuroraVoiceButton.vue
    ├── AuroraStatus.vue
    ├── AuroraHud.vue
    └── AuroraBackground.vue
```

---

# 44. Estado central

Pinia:

```ts
interface AuroraState {
  status:
    | 'idle'
    | 'listening'
    | 'thinking'
    | 'working'
    | 'speaking'
    | 'offline';

  volume: number;
  connected: boolean;
}
```

---

# 45. Integração com streaming

Eventos SSE podem alterar o estado visual.

Exemplo:

```text
message.started
→ thinking

tool.started
→ working

message.delta
→ speaking

message.completed
→ idle

error
→ error
```

---

# 46. Integração com voz

Exemplo:

```text
microphone.start
→ listening

speech.detected
→ listening

request.sent
→ thinking

audio.started
→ speaking

audio.completed
→ idle
```

---

# 47. Regra principal de experiência

O usuário não deve depender de texto como:

```text
"Aurora está processando"
```

para entender o estado.

O próprio Core deve comunicar visualmente.

---

# 48. Filosofia

A tela `/chat` deve seguir:

```text
Less UI.
More presence.
```

e:

```text
Aurora is not inside the interface.

Aurora IS the interface.
```

---

# 49. Evitar

Não criar:

❌ cards

❌ dashboard

❌ sidebar

❌ botões de funções

❌ menu enorme

❌ widgets

❌ excesso de texto

❌ animação frenética

❌ partículas demais

❌ neon exagerado

❌ aparência gamer

---

# 50. Resultado esperado

Ao abrir `/chat`, o usuário deve sentir:

```text
"Estou diante da Aurora."
```

E não:

```text
"Estou diante de um aplicativo de chat."
```

A interação principal deve acontecer entre:

```text
Usuário
  ↓
Aurora Core
  ↓
Voz / Texto
```

O núcleo deve respirar quando parado, reagir quando ouvir, reorganizar sua energia quando pensar e responder visualmente ao ritmo da voz quando falar.

Essa reação é o elemento mais importante da experiência.
