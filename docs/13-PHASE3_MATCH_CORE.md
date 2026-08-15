# Fase 3 — Match Core

Status: **VALIDADA / MERGED**

## Objetivo

Construir a máquina de estado lógica de uma partida BFC sem depender da UI final, de cena Unity ou de manipulação direta de Rigidbody.

Esta fase segue `docs/04-DEVELOPMENT_PLAN.md`, `docs/01-GAMEPLAY_CONSTITUTION.md` e `docs/03-ARCHITECTURE.md`.

## Escopo implementado

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
- evento de expiração do relógio sem inventar a política exata de encerramento durante uma resolução física;
- encerramento explícito da partida;
- eventos de domínio drenáveis;
- testes EditMode das transições centrais.

## Decisões deliberadamente não tomadas

### OPEN-001 — composição de equipe

Na Fase 3, `PlayerActionCommand` foi deliberadamente mantido com um identificador lógico de peça (`pieceId`) sem codificar uma contagem de formação no Match Core.

`OPEN-001` foi resolvido posteriormente na Fase 4; essa resolução não exigiu reescrever o Match Core porque a contagem permaneceu fora da lógica central de partida.

### OPEN-002 — bola fora e reposições

Após um gol, o Match Core entra em `AwaitingRestart` e deixa `Possession = None`. A posse seguinte só é definida por `ResumeAfterRestart(nextPossession)`.

Isto é proposital: esta fase não decide lateral, escanteio, tiro de meta, regra de saída ou equipe que repõe.

### OPEN-003 — faltas, vantagem e pênaltis

Não implementados nesta fase.

## Fronteira com física

`TrySubmitAction(...)` aceita uma ação lógica e muda o estado para `ResolvingAction`.

A camada de física executa a intenção e, apenas quando atingir condição segura de continuação, deve chamar `ResolvePhysicalAction(...)` com um resultado compatível, por exemplo:

- continuar a posse;
- transferir posse;
- gol.

Isso preserva a separação entre regra e PhysX. A física não escolhe placar, recompensa ou regras de modo.

## Relógio

O relógio usa `TimeSpan` e avanço explícito; não usa `DateTime.Now`, `Update()` variável ou wall clock.

Ao chegar a zero, `MatchClockExpired` é emitido. `FinishMatch(TimeExpired)` permanece explícito para não decidir silenciosamente se uma partida deve encerrar no meio de uma resolução física ou somente após ela.

Essa política poderá ser formalizada por ruleset sem reescrever o relógio.

## Testes implementados

A suite `BFC.Gameplay.EditMode.Tests` cobre:

1. início com posse explícita;
2. rejeição de equipe sem posse;
3. bloqueio enquanto a física resolve;
4. três ações por ruleset e transferência após o limite;
5. transferência antecipada de posse;
6. gol, score e restart explícito;
7. expiração do relógio sem regra implícita de encerramento;
8. bloqueio de ação após fim da partida.

## Validação final

A Fase 3 foi validada em Unity 6000.3.21f1 antes do merge:

- Unity importou/compilou a implementação;
- os 8 testes de `MatchControllerTests` passaram;
- suite EditMode completa da época: **13/13 passed**;
- suite PlayMode da época: **1/1 passed**;
- `.meta` gerados pelo Unity foram revisados e materializados;
- CI `Governance` verde no head final;
- CI `Unity Structure` verde no head final.

## Merge

- PR #5: `Implement Phase 3 match core`;
- head final antes do merge: `40e0daa70ed367a1af84ed651d3400fdee6e7f66`;
- merge por squash: `c6d636a2ce93461b8034c17db6a01c8518e65ea4`.

## Estado

O gate planejado para o Match Core foi atendido. Integrações posteriores com física, formação, modos e regras devem consumir este núcleo sem duplicar suas regras em UI/IA.
