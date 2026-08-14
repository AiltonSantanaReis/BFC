# BFC

**BFC** é a reconstrução oficial em Unity do jogo de futebol de botão competitivo anteriormente prototipado em tecnologias web.

Este repositório tem duas referências de produto obrigatórias:

1. **BFC legado** — referência funcional para mecânicas, regras, jogabilidade, progressão, economia, IA e comportamento de partida.
2. **Interface e Menu** — referência visual oficial para layout, cores, composição, estilo, hierarquia, modais, cards, vitrine e linguagem visual.

O código legado **não é a arquitetura de destino**. Os conceitos aprovados são reconstruídos em Unity com uma arquitetura modular, testável e preparada para múltiplas plataformas.

## Estado

`FOUNDATION / PRE-PRODUCTION`

A primeira etapa do repositório estabelece arquitetura, governança, regras de produto, baseline de referência e plano de desenvolvimento antes da implementação de gameplay.

## Engine

- Unity 6.3 LTS
- Editor fixado inicialmente em `6000.3.21f1`
- C#
- Universal Render Pipeline (URP)
- Input System
- Testes EditMode e PlayMode como requisito de produção

A versão do Editor só pode ser alterada por decisão arquitetural registrada.

## Princípio central

> **Preservar o jogo. Melhorar a implementação.**

Uma refatoração, troca de tecnologia, melhoria gráfica ou otimização não autoriza alterar silenciosamente uma regra, mecânica, comportamento aprovado ou a identidade visual do BFC.

## Estrutura alvo

```text
Assets/BFC/
├── Core/              # domínio e regras puras
├── Gameplay/          # orquestração da partida
├── Physics/           # movimento, colisões, spin, superfície
├── AI/                # percepção, decisão e planejamento
├── Modes/             # casual, competitivo, treino, desafios
├── Progression/       # perfil, economia, missões, conquistas
├── Presentation/      # UI, HUD, câmeras, feedback visual
├── Infrastructure/    # save, plataforma, serviços externos
├── Bootstrap/         # composição e inicialização
└── Tests/             # testes automatizados
```

A regra de dependência está documentada em [`docs/03-ARCHITECTURE.md`](docs/03-ARCHITECTURE.md).

## Documentos normativos

Os documentos abaixo fazem parte da especificação do produto e **não são documentação opcional**:

- [`docs/00-PRODUCT_CHARTER.md`](docs/00-PRODUCT_CHARTER.md) — identidade, autoridade e não negociáveis.
- [`docs/01-GAMEPLAY_CONSTITUTION.md`](docs/01-GAMEPLAY_CONSTITUTION.md) — regras e mecânicas oficiais.
- [`docs/02-VISUAL_CONSTITUTION.md`](docs/02-VISUAL_CONSTITUTION.md) — contrato visual da Interface e Menu.
- [`docs/03-ARCHITECTURE.md`](docs/03-ARCHITECTURE.md) — arquitetura e limites entre módulos.
- [`docs/04-DEVELOPMENT_PLAN.md`](docs/04-DEVELOPMENT_PLAN.md) — plano por fases e gates.
- [`docs/05-CHANGE_CONTROL.md`](docs/05-CHANGE_CONTROL.md) — processo obrigatório para mudar regra ou direção.
- [`docs/06-REFERENCE_BASELINE.md`](docs/06-REFERENCE_BASELINE.md) — como os projetos anteriores são usados como referência.
- [`docs/07-DEFINITION_OF_DONE.md`](docs/07-DEFINITION_OF_DONE.md) — critérios objetivos de conclusão.
- [`docs/08-REGRESSION_POLICY.md`](docs/08-REGRESSION_POLICY.md) — política de não regressão.
- [`docs/09-PLATFORM_STRATEGY.md`](docs/09-PLATFORM_STRATEGY.md) — estratégia de plataformas.
- [`governance/rules.json`](governance/rules.json) — registro legível por máquina das regras bloqueadas.

## Regra de mudança

Regras marcadas como `LOCKED` não podem ser alteradas, removidas ou reinterpretadas em PR comum.

Uma mudança desse tipo exige:

1. PR explicitamente identificado como **RULE CHANGE** ou **GOVERNANCE CHANGE**;
2. RFC em `docs/changes/` descrevendo motivo, impacto e regressões;
3. comparação com as referências do BFC;
4. atualização dos testes afetados;
5. aprovação explícita do Product Owner.

O CI de governança verifica parte dessas exigências automaticamente.

## Branches

- `main`: linha estável e revisada.
- `agent/*`: trabalho automatizado/assistido.
- `feature/*`: funcionalidades.
- `fix/*`: correções.
- `rule-change/*`: alteração deliberada de regra aprovada.

Mudanças relevantes entram por Pull Request.

## Próximo marco

O primeiro marco de implementação é um **Vertical Slice de Física**, não o menu completo:

- campo de teste;
- duas peças;
- uma bola;
- mira/arrasto;
- força;
- colisão;
- desaceleração determinística;
- benchmark contra o BFC legado.

A interface oficial será reconstruída depois que os contratos centrais estiverem testáveis, sem redesenhar a referência visual aprovada.
