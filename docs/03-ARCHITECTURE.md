# BFC Architecture

Status: **NORMATIVE ARCHITECTURE BASELINE**

## 1. Objetivo

A arquitetura do BFC deve permitir evoluir física, regras, IA, apresentação e plataformas sem transformar cada mudança em reescrita transversal.

A engine é Unity, mas o domínio do jogo não deve ser um amontoado de `MonoBehaviour` acoplados à cena.

## 2. Princípios

1. Regra de jogo não depende de UI.
2. UI não é dona do estado autoritativo da partida.
3. Core não conhece cenas, GameObjects ou assets.
4. Física não define regras de competição.
5. IA usa as mesmas regras disponíveis a um jogador válido.
6. Save persiste domínio; não serializa arbitrariamente GameObjects de runtime.
7. Bootstrap é o único composition root autorizado.
8. Dependências apontam para dentro, não para conveniência.
9. Dados de conteúdo devem ser separados de comportamento quando possível.
10. Mudanças de plataforma ficam atrás de adapters/ports.

## 3. Estrutura

```text
Assets/BFC/
├── Core/
├── Gameplay/
├── Physics/
├── AI/
├── Modes/
├── Progression/
├── Presentation/
├── Infrastructure/
├── Bootstrap/
└── Tests/
```

## 4. Módulos

### 4.1 BFC.Core

Responsável por:

- entidades e value objects de domínio;
- estados lógicos;
- contratos de regras;
- identificadores;
- eventos de domínio;
- portas necessárias pelo domínio.

**Proibido:**

- `MonoBehaviour`;
- acesso a `SceneManager`;
- UI;
- Input System;
- PlayerPrefs;
- arquivos;
- rede;
- física Unity;
- singletons globais de engine.

Core deve permanecer testável em EditMode com o mínimo possível de infraestrutura Unity.

### 4.2 BFC.Gameplay

Responsável por orquestrar uma partida:

- lifecycle;
- turno;
- posse;
- comandos;
- validação de ação;
- transições de estado;
- score;
- relógio de partida;
- reinícios/reposições após regras definidas.

Pode depender de `BFC.Core` e de contratos explícitos de física/serviços. Não deve renderizar.

### 4.3 BFC.Physics

Responsável por:

- corpos físicos;
- aplicação de impulso;
- colisão;
- atrito/damping;
- spin;
- bola aérea;
- superfície;
- condição de repouso;
- bridge entre física Unity e estado de gameplay.

Não decide sozinho:

- quem ganhou;
- quando trocar turno;
- prêmio;
- rating;
- regras de modo.

### 4.4 BFC.AI

Responsável por:

- percepção do estado permitido;
- avaliação de opções;
- planejamento;
- escolha de peça;
- força/direção;
- estratégia por dificuldade.

A IA produz **comandos válidos de jogador**. Não manipula diretamente placar, turno ou física para obter vantagem.

### 4.5 BFC.Modes

Responsável por composição de rulesets:

- casual;
- P1 vs COM;
- P1 vs P2;
- Liga Justa;
- Campeonato;
- Treino;
- Desafios.

Modo configura comportamento por dados/regras. Evitar `if(mode == ...)` espalhado pelo projeto.

### 4.6 BFC.Progression

Responsável por:

- perfil;
- inventário;
- moedas/fichas;
- missões;
- conquistas;
- troféus;
- temporada;
- recompensas;
- ownership/equipamento.

Progressão nunca altera atributos competitivos quando o ruleset exige padronização.

### 4.7 BFC.Presentation

Responsável por:

- Interface e Menu;
- HUD;
- telas;
- navegação;
- feedback;
- câmeras;
- VFX de apresentação;
- animações de UI;
- acessibilidade visual.

Presentation observa estado e emite intents/commands. Não contém regra autoritativa.

### 4.8 BFC.Infrastructure

Responsável por implementar portas de:

- save;
- armazenamento;
- plataforma;
- telemetria futura;
- rede futura;
- relógio externo quando necessário;
- serviços Unity específicos.

### 4.9 BFC.Bootstrap

Único ponto de composição.

É permitido conhecer todos os módulos para construir dependências. Nenhum outro módulo deve funcionar como service locator global.

## 5. Dependency graph

```text
                 Presentation
                      |
                      v
                   Gameplay <------ AI
                      |
                      v
                    Core
                     ^  ^
                    /    \
             Physics      Progression
                    \      /
                     \    /
                 Infrastructure

Bootstrap -> compõe todos
```

O diagrama representa intenção, não autorização para referência circular.

## 6. Assemblies

Cada módulo de produção recebe um `.asmdef` próprio.

Objetivos:

- impedir dependência acidental;
- reduzir recompilação;
- tornar arquitetura verificável;
- permitir testes isolados.

Referência circular entre assemblies é proibida.

## 7. Estado

### Autoritativo de partida

Pertence a Core/Gameplay.

### Estado físico

Pertence a Physics e é sincronizado explicitamente com Gameplay.

### Estado de UI

Pertence a Presentation e inclui apenas coisas como modal aberto, foco, animação e seleção visual.

### Estado persistente

Pertence a Progression/Core e é serializado por Infrastructure.

## 8. Eventos e comandos

Entrada externa deve virar comando/intenção:

```text
Input -> Presentation/Input Adapter -> Gameplay Command
```

Saída do domínio pode virar evento:

```text
GoalScored
TurnChanged
PossessionChanged
MatchFinished
RewardGranted
AchievementUnlocked
```

A UI responde ao evento; não reimplementa a regra.

## 9. Física e determinismo

A simulação deve usar passo fixo e parâmetros centralizados.

Regras críticas não podem depender de `Update()` variável.

Toda alteração de timestep, solver, material físico ou coeficientes exige benchmark de regressão.

## 10. Dados

Conteúdo ajustável deve preferir assets/configurações dedicadas, por exemplo:

- `BallDefinition`;
- `PieceDefinition`;
- `FieldDefinition`;
- `ChallengeDefinition`;
- `RewardDefinition`;
- `RulesetDefinition`.

Dados não devem esconder código arbitrário.

## 11. Anti-patterns proibidos

- `FindObjectOfType`/busca global como arquitetura de dependência;
- `GameManager` monolítico com todas as regras;
- UI alterando diretamente Rigidbody para executar regra;
- física concedendo recompensa;
- ScriptableObject usado como estado mutável global de partida sem contrato;
- `PlayerPrefs` espalhado pelo código;
- strings mágicas para modos/regras;
- números de balanceamento duplicados em múltiplos scripts;
- lógica de modo baseada em nome da cena;
- cópia de regra para cada plataforma.

## 12. Migração de arquitetura

Uma arquitetura mais sofisticada (DOTS/ECS, multiplayer server-authoritative, novo renderer, etc.) só deve ser adotada quando existir requisito mensurável. Complexidade não é objetivo por si só.
