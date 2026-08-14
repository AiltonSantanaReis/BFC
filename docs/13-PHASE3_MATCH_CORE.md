# Fase 3 — Match Core

Status: **PRIMEIRA FATIA IMPLEMENTADA / VALIDAÇÃO UNITY PENDENTE**

## Objetivo

Construir a máquina de estado lógica de uma partida BFC sem depender da UI final, de cena Unity ou de manipulação direta de Rigidbody.

Esta fase segue `docs/04-DEVELOPMENT_PLAN.md`, `docs/01-GAMEPLAY_CONSTITUTION.md` e `docs/03-ARCHITECTURE.md`.

## Escopo desta primeira fatia

- `MatchState` somente leitura;
- fases `NotStarted`, `AwaitingAction`, `ResolvingAction`, `AwaitingRestart` e `Finished`;
- posse explícita por `TeamId`;
- comando lógico de jogador com identificador de peça configurável;
- rejeição de comando fora da posse;
- bloqueio de nova ação enquanto a física ainda não foi resolvida (`GAME-003`);
- contador de ações independente de toque na bola (`GAME-004`);
- limite de ações obtido exclusivamente de `IMatchRules.MaxActionsPerPossession`;
- troca antecipada de posse quando a resolução física determinar;
- score lógico;
- gol vindo como resultado da resolução física;
- estado `AwaitingRestart` após gol;
- retomada com posse informada explicitamente pelo chamador/ruleset;
- relógio lógico com avanço explícito e clamp no fim da duração;
- evento de expiração do relógio sem inventar nesta fatia a política exata de encerramento durante uma resolução física;
- encerramento explícito da partida;
- eventos de domínio drenáveis;
- testes EditMode das transições centrais.

## Decisões deliberadamente não tomadas

### OPEN-001 — quantidade final de peças

`PlayerActionCommand` usa um identificador lógico de peça (`pieceId`). Nenhuma contagem de formação é codificada no Match Core.

### OPEN-002 — bola fora e reposições

Após um gol, o Match Core entra em `AwaitingRestart` e deixa `Possession = None`. A posse seguinte só é definida por `ResumeAfterRestart(nextPossession)`.

Isto é proposital: esta fase não decide lateral, escanteio, tiro de meta, regra de saída ou equipe que repõe.

### OPEN-003 — faltas, vantagem e pênaltis

Não implementados nesta fatia.

## Fronteira com física

`TrySubmitAction(...)` aceita uma ação lógica e muda o estado para `ResolvingAction`.

A camada de física futura executará a intenção e, apenas quando atingir condição segura de continuação, chamará `ResolvePhysicalAction(...)` com um dos resultados:

- continuar a posse;
- transferir posse;
- gol.

Isso preserva a separação entre regra e PhysX. A física não escolhe placar, recompensa ou regras de modo.

## Relógio

O relógio usa `TimeSpan` e avanço explícito; não usa `DateTime.Now`, `Update()` variável ou wall clock.

Ao chegar a zero, `MatchClockExpired` é emitido. `FinishMatch(TimeExpired)` é explícito nesta fatia para não decidir silenciosamente se uma partida deve encerrar no meio de uma resolução física ou somente após ela.

Essa política poderá ser formalizada por ruleset sem reescrever o relógio.

## Testes adicionados

A suite `BFC.Gameplay.EditMode.Tests` cobre:

1. início com posse explícita;
2. rejeição de equipe sem posse;
3. bloqueio enquanto a física resolve;
4. três ações por ruleset e transferência após o limite;
5. transferência antecipada de posse;
6. gol, score e restart explícito;
7. expiração do relógio sem regra implícita de encerramento;
8. bloqueio de ação após fim da partida.

## Gate desta fatia

Antes de marcar o PR como Ready:

- Unity 6000.3.21f1 importa e compila sem erros;
- novos testes Gameplay EditMode passam;
- suites EditMode/PlayMode existentes continuam verdes;
- `.meta` gerados pelo Unity são revisados e materializados;
- CI de Governança e Unity Structure passam no head final.

A Fase 3 completa ainda poderá receber integração explícita com o bridge físico e refinamentos de relógio/ruleset antes do encerramento do milestone.
