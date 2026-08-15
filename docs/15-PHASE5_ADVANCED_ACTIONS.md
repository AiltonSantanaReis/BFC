# BFC — Fase 5: Advanced Actions

Status: **DRAFT ENGINEERING SPECIFICATION — NO RULE CHANGE**

Branch de preparação: `agent/phase5-advanced-actions-spec`

Base canônica desta especificação: `main` em `4476640cb7a08239bbd0b87ff07a1a8e7e135988`.

## 1. Objetivo

A Fase 5 deve recuperar e superar as mecânicas de habilidade do BFC legado sem quebrar as fronteiras arquiteturais já aprovadas.

O escopo normativo da fase é:

- chute;
- passe com força reduzida;
- spin/efeito;
- chip/bola por cima 3D;
- menu radial/contextual;
- feedback de força e direção;
- regras de elegibilidade.

O gate da fase exige:

- teste de regra para cada ação;
- input sem lógica autoritativa de resultado;
- parâmetros ajustáveis centralizados;
- comportamento comparado ao legado.

Esta especificação não altera nenhuma regra `LOCKED`. Ela detalha uma implementação compatível com `GAME-002`, `GAME-003` e `GAME-010` a `GAME-014`.

## 2. Autoridade e documentos de referência

Ordem aplicada nesta fase:

1. decisões explícitas do Product Owner;
2. `governance/rules.json`;
3. `docs/00-PRODUCT_CHARTER.md`;
4. `docs/01-GAMEPLAY_CONSTITUTION.md`;
5. `docs/03-ARCHITECTURE.md`;
6. ADRs/RFCs aceitos;
7. `docs/04-DEVELOPMENT_PLAN.md`;
8. comportamento de referência do legado;
9. implementação atual.

Também deve ser preservada a direção registrada na issue #7 — `Specify BFC Classic Simulation mode`.

## 3. Não objetivos

Esta fase não deve:

- resolver `OPEN-002` — bola fora/reposições;
- resolver `OPEN-003` — faltas/vantagem/pênaltis;
- congelar números de `OPEN-004`;
- decidir tecnologia final de UI de `OPEN-005`;
- implementar IA completa ou goleiro automático da Fase 6;
- transformar o menu radial de engenharia em UI final da Fase 10/11;
- criar trajetória direta de bola para alvo;
- colocar regra autoritativa em `MonoBehaviour`, Input System ou Presentation;
- alterar a regra de até 3 ações por posse;
- fundir Modern Control e Classic Simulation em um único modelo de comando.

## 4. Separação obrigatória de responsabilidades

Fluxo alvo:

```text
Input device
    ↓
Presentation/Input Adapter
    ↓  intenção normalizada; sem resultado físico
Gameplay
    ├─ valida fase/posse
    ├─ valida elegibilidade
    ├─ valida ActionKind/payload
    ├─ aplica perfil/tuning autorizado
    └─ produz pedido físico explícito
    ↓
Gameplay → Physics bridge
    ↓
BFC.Physics
    ├─ movimento/impulso planar
    ├─ resposta angular/spin
    ├─ componente vertical do chip
    └─ resolução física/repouso
    ↓
Gameplay recebe PhysicalActionResolution
```

Princípios:

- Input informa intenção; não calcula resultado de jogo.
- Gameplay é dono da elegibilidade e do significado da ação.
- Physics é dono da execução física, não de posse, score ou prêmio.
- Presentation exibe direção/força/ação selecionada, mas não é fonte de verdade.
- parâmetros de balanceamento ficam em configuração centralizada, nunca duplicados em UI, IA ou cena.

## 5. Taxonomia de ações modernas

Introduzir um conceito puro, sem `UnityEngine`, equivalente a:

```text
ActionKind
- Shot
- Pass
- Spin
- Chip
```

`ActionKind` descreve a intenção de gameplay. Não descreve botão, tecla, gesto, animação ou shader.

### 5.1 Shot

Regra preservada: `GAME-010`.

Semântica:

- ação padrão;
- usa potência normal disponível ao jogador;
- direção e potência são definidas pela intenção normalizada;
- não possui redução obrigatória de força em relação ao baseline da ação padrão.

Não congelado nesta especificação:

- velocidade máxima final;
- curva exata de potência;
- massa/restituição finais;
- resposta visual final.

### 5.2 Pass

Regra preservada: `GAME-011`.

Semântica:

- mesma família de controle direcional do Modern Control;
- potência máxima efetiva deve ser inferior à do Shot sob o mesmo perfil competitivo;
- o fator exato é `TUNABLE` e centralizado;
- Presentation não multiplica nem reduz potência por conta própria.

Invariante testável:

```text
0 < PassMaxPowerScale < ShotMaxPowerScale
```

A implementação pode normalizar Shot como `1.0`, mas isso é convenção de configuração, não novo requisito normativo.

### 5.3 Spin / efeito

Regra preservada: `GAME-012`.

Semântica:

- a intenção do jogador inclui sentido e intensidade de efeito;
- sentido deve ser preservado como valor assinado;
- resultado deve ser físico e legível;
- o sistema não pode substituir spin por erro aleatório de trajetória.

Contrato conceitual:

```text
SpinIntent ∈ [-1, +1]
```

Onde sinal representa sentido e magnitude representa intensidade normalizada.

Não congelado:

- coeficiente angular;
- duração do efeito;
- modelo final de transferência peça→bola;
- damping angular;
- mapping de mouse/gamepad/touch.

### 5.4 Chip

Regra preservada: `GAME-013`.

Semântica:

- deve produzir componente vertical física real;
- deve preservar direção planar e potência como intenção do jogador;
- não pode ser apenas animação, troca de sprite ou pseudo-Z;
- não pode calcular um arco visual desconectado da física autoritativa.

Contrato conceitual:

```text
ChipIntent
- planar direction
- normalized power
- normalized lift ∈ [0, 1]
```

`normalized lift` é intenção; o valor de velocidade/impulso vertical continua `TUNABLE`.

## 6. Payload de intenção puro

O contrato de input deve ser independente da Unity. A forma exata dos tipos C# será definida na fatia de implementação, mas deve representar semanticamente:

```text
PlayerActionIntent
- Team
- PieceId
- ActionKind
- PlanarDirection
- NormalizedPower
- SignedSpinIntent       // somente quando aplicável
- NormalizedLiftIntent   // somente quando aplicável
```

Requisitos:

- sem `Vector2`/`Vector3` de `UnityEngine` no Core/Gameplay puro;
- direção deve ser finita e normalizável;
- potência deve ser finita e limitada ao intervalo aceito;
- payload incompatível com a ação deve ser rejeitado ou normalizado explicitamente, nunca ignorado silenciosamente;
- `NaN` e infinito são inválidos;
- PieceId vazio é inválido;
- `TeamId.None` é inválido.

## 7. Relação com `PlayerActionCommand`

O `PlayerActionCommand` atual contém apenas `Team` e `PieceId`, o que foi suficiente para a Fase 3.

Na Fase 5, ele deve evoluir sem importar tipos Unity.

Opções compatíveis aceitas para a fatia de implementação:

1. `PlayerActionCommand` passa a compor um `PlayerActionIntent` puro; ou
2. o Match Core mantém identidade/fase em `PlayerActionCommand` e um comando de ação puro explicitamente associado carrega o payload avançado.

Critério de escolha:

- um único caminho autoritativo de submissão;
- sem duplicação de validação;
- sem dependência Unity em `BFC.Core`/`BFC.Gameplay`;
- MatchController continua impedindo nova ação durante `ResolvingAction`;
- `ActionsUsedInPossession` continua independente de semântica de toque na bola.

Não é permitido criar um segundo caminho que aplique física sem passar pelo fluxo de aceitação da partida.

## 8. Elegibilidade

A Fase 5 precisa introduzir contrato explícito de elegibilidade por ação.

Conceito esperado:

```text
IActionEligibilityPolicy
Evaluate(matchState, actor, actionIntent) -> allowed / rejection reason
```

Validações mínimas comuns:

- partida iniciada;
- fase aceita nova ação;
- time possui a posse quando o ruleset exige;
- peça existe e pertence ao time correto;
- peça está autorizada a agir;
- ação está habilitada no ruleset/profile;
- direção/potência/payload são válidos.

Não incluir nesta política regras ainda abertas de lateral, escanteio, faltas, vantagem, pênalti ou janela de reposicionamento de goleiro.

Rejeições devem possuir razão explícita e testável; a UI apenas apresenta o resultado.

## 9. Tuning centralizado

Criar uma definição de ação centralizada, equivalente conceitualmente a:

```text
AdvancedActionProfile
- ShotMaxPowerScale
- PassMaxPowerScale
- SpinStrengthScale
- ChipPlanarPowerScale
- ChipLiftScale
```

Todos os valores físicos finais continuam `TUNABLE`.

Requisitos:

- perfil imutável durante uma ação já aceita;
- competitivo usa perfil simétrico para os dois lados;
- IA e jogador humano consomem o mesmo perfil;
- Presentation pode ler valores para feedback, mas não alterá-los;
- nenhum multiplicador paralelo em Input, HUD ou AI.

## 10. Pedido físico explícito

Gameplay deve transformar intenção válida + profile em um pedido físico explícito e engine-agnostic na fronteira.

Conceito:

```text
PhysicalActionRequest
- PieceId
- ActionKind
- PlanarDirection
- EffectivePower
- SpinRequest
- LiftRequest
```

Esse pedido descreve **o que a física deve executar**, não o resultado esperado da jogada.

Proibido:

```text
BallTargetPosition
GuaranteedGoal
ForcedBallTrajectory
DesiredFinalBallPosition
```

O resultado continua emergindo da simulação e retorna ao Match Core por `PhysicalActionResolution`.

## 11. Extensão de `BFC.Physics`

O `PlanarKineticBody` atual é adequado ao slice planar da Fase 2, mas não é contrato suficiente para toda a Fase 5.

A implementação deve separar pelo menos:

- lançamento/impulso planar da peça;
- resposta angular necessária a spin;
- componente vertical da bola necessária ao chip;
- detecção de repouso compatível com movimento 3D relevante.

Não remover a independência de FPS nem migrar lógica para `Update()` variável.

Qualquer alteração de timestep, solver, materiais físicos ou coeficientes deve seguir o benchmark de regressão já exigido pela arquitetura.

## 12. Modern Control

`GAME-002` permanece intacta:

```text
selecionar peça
    ↓
drag/aim
    ↓
feedback de direção + força
    ↓
release
    ↓
PlayerActionIntent
```

A seleção de `ActionKind` pode vir do menu contextual/radial, teclado, gamepad ou touch, mas o mapeamento de dispositivo não altera o contrato autoritativo.

O Modern Control continua sendo a família principal de controle já aprovada.

## 13. Menu radial/contextual

`GAME-014` exige intenção visual-funcional, não tecnologia final nesta fase.

Para o gate da Fase 5, o radial pode ser um harness funcional de engenharia desde que:

- permita selecionar ações especiais suportadas;
- reflita elegibilidade real vinda de Gameplay;
- não aplique multiplicadores ou física diretamente;
- mostre estados indisponíveis de forma inequívoca;
- não seja declarado UI final da Fase 10/11.

A tecnologia final continua protegida por `OPEN-005`.

## 14. Feedback de força e direção

Feedback deve derivar da mesma intenção normalizada que será submetida.

Obrigatório:

- direção visível;
- força relativa visível;
- ação selecionada identificável;
- para Spin, sentido/intensidade devem ser representáveis;
- para Chip, intenção de elevação deve ser representável.

Presentation não deve prever gol garantido ou corrigir trajetória autoritativamente.

## 15. Classic Strike Model — separação obrigatória

A issue #7 registra uma família de controle distinta.

Classic Simulation não reutiliza `PlayerActionIntent` moderno como se fosse apenas outro `ActionKind`.

Contrato conceitual separado:

```text
ClassicStrikeIntent
- Team
- PieceId
- ImpactPointLocal
- StrikeDirection
- NormalizedForce
```

Fluxo:

```text
impact point + direction/angle + force
                    ↓
linear + angular response da peça
                    ↓
contato físico peça × bola
                    ↓
trajetória emergente da bola
```

Requisitos:

- comando atua na peça selecionada, não na trajetória da bola;
- ponto de impacto influencia resposta angular/linear;
- erro emerge principalmente da relação física entre input e resultado;
- RNG artificial não substitui execução;
- nenhuma propriedade `BallTarget`;
- coeficientes exatos continuam `TUNABLE`;
- Modern Control permanece separado e disponível.

### 15.1 Goleiro no Classic

A Fase 5 pode definir apenas o contrato futuro de janela de posicionamento.

Não implementar quando ela abre, porque isso depende de `OPEN-002`/`OPEN-003`.

Conceito preservado:

```text
GoalkeeperPlacementWindow
- Team
- AllowedArea
- MaxDisplacement
- CanRotate
- TimeLimit
- ConfirmationRequired
```

Nenhum movimento manual contínuo de goleiro é autorizado por esta especificação.

## 16. Estratégia de testes

### 16.1 Testes puros de Gameplay

Cada `ActionKind` deve possuir teste de regra.

Matriz mínima:

```text
Shot
- aceita payload válido
- rejeita direção inválida
- rejeita potência inválida

Pass
- aceita payload válido
- effective max power < Shot no mesmo profile
- usa o mesmo profile para ambos os times

Spin
- preserva sinal do spin intent
- limita magnitude ao intervalo permitido
- rejeita NaN/infinito

Chip
- aceita lift normalizado válido
- produz pedido físico com lift positivo quando solicitado
- não produz propriedade de trajetória/alvo de bola
```

Testes comuns:

- time sem posse rejeitado pelo Match Core quando aplicável;
- ação durante `ResolvingAction` rejeitada;
- PieceId inválido rejeitado;
- ação desabilitada pelo ruleset rejeitada;
- action counter incrementa uma vez por ação aceita;
- nenhuma ação conta automaticamente como toque na bola.

### 16.2 Testes de Physics

Adicionar testes específicos para:

- impulso planar bounded;
- resposta angular bounded;
- chip possui componente vertical real;
- repouso ocorre sem aceleração indefinida;
- fixed-step permanece fonte temporal de simulação;
- mesmos inputs em taxas de render diferentes permanecem equivalentes dentro da tolerância definida.

### 16.3 PlayMode/harness

Harness de Fase 5 deve demonstrar:

- seleção de peça;
- troca de ActionKind;
- feedback;
- execução de Shot/Pass/Spin/Chip;
- bloqueio de nova ação enquanto física resolve;
- nenhuma exceção/erro de console relevante.

## 17. Comparação com o legado

Antes do fechamento da Fase 5, registrar comparação qualitativa/quantitativa disponível para:

- relação drag → força;
- diferença Shot vs Pass;
- legibilidade do spin;
- utilidade do chip para superar interação/obstáculo;
- velocidade de resolução/repouso;
- sensação de precisão.

Não inventar números que não tenham sido medidos no legado.

Quando o artefato legado não oferecer dado mensurável, registrar explicitamente comparação qualitativa e a limitação.

## 18. Sequenciamento de implementação

### Fatia A — especificação e contratos puros

- este documento;
- limpar autorreferências transitórias do handoff de continuidade;
- definir tipos puros de ação;
- definir validação/elegibilidade;
- testes EditMode de domínio/gameplay.

### Fatia B — bridge e Physics primitives

- pedido físico explícito;
- extensão planar/angular;
- componente vertical real do chip;
- testes de physics.

### Fatia C — integração Modern Control

- input adapter;
- seleção de ActionKind;
- submissão pelo Match Core;
- feedback de força/direção.

### Fatia D — radial funcional e harness

- menu contextual/radial funcional;
- estados de elegibilidade;
- PlayMode/smoke;
- comparação com legado.

### Fatia E — Classic Strike Model experimental

- somente após contratos modernos estáveis;
- comando separado;
- impacto na peça produz resposta linear/angular;
- sem resolver regras de goalkeeper placement.

## 19. Critério de conclusão da Fase 5

A Fase 5 só pode ser encerrada quando houver evidência de:

- Shot funcional e testado;
- Pass funcional, testado e com força máxima reduzida;
- Spin funcional e testado;
- Chip físico 3D funcional e testado;
- elegibilidade testada;
- radial/contextual funcional;
- feedback funcional;
- parâmetros centralizados;
- input sem lógica autoritativa de resultado;
- Match Core preserva resolução física antes da próxima ação;
- comparação com legado registrada;
- Unity/EditMode/PlayMode aplicáveis verdes;
- Governance e Unity Structure verdes;
- nenhuma decisão silenciosa de `OPEN-002`, `OPEN-003`, `OPEN-004` ou `OPEN-005`.

## 20. Change Control

Classificação esperada desta especificação: **Classe A — implementação compatível**.

Nenhum RFC de regra é necessário enquanto a implementação permanecer dentro das regras já aprovadas.

Se durante a Fase 5 surgir necessidade de:

- mudar `GAME-002` ou `GAME-010` a `GAME-014`;
- redefinir o significado de ação por posse;
- tornar Classic o controle obrigatório;
- alterar goleiro competitivo;
- decidir restart/falta/pênalti;

interromper a implementação afetada e seguir o fluxo de RFC/decisão explícita correspondente.

## 21. Decisões propositalmente não tomadas aqui

Esta especificação não congela:

- valores finais de potência;
- percentual final do passe;
- curva final de drag/power;
- intensidade final de spin;
- modelo final de transferência de spin peça→bola;
- altura/duração final do chip;
- materiais físicos finais;
- tecnologia final do radial;
- regras de bola fora/restart;
- regras de falta/vantagem/pênalti;
- janelas reais de reposicionamento de goleiro;
- elegibilidade competitiva final do Classic;
- dimensões finais de campo por modo.

Esses pontos permanecem `TUNABLE`, `OPEN` ou dependentes de fases posteriores conforme a governança existente.
