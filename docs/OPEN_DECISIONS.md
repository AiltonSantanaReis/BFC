# BFC Open Decisions

Este arquivo contém decisões que **não podem ser resolvidas por suposição de implementação**.

Quando resolvida, a decisão deve gerar ADR/RFC/regra conforme o impacto e mover seu resultado para o documento normativo adequado.

## OPEN-001 — Quantidade oficial de peças por equipe

**Status:** RESOLVED — 2026-08-14

Decisão aprovada:

- não existe uma contagem global única de peças para todos os modos do BFC;
- modos derivados do baseline legado preservam seus padrões de campo/composição por configuração própria;
- o perfil oficial de **campo grande** usa **11 peças no total por equipe**;
- essas 11 peças são **10 jogadores de linha + 1 goleiro**, com o goleiro incluído na contagem;
- Treino e Desafios podem definir campo, quantidade de peças e formação conforme cada cenário;
- contagem e formação devem permanecer configuráveis por modo/ruleset, sem número mágico em lógica central.

Registro normativo:

- `OPEN-001` agora está `locked` em `governance/rules.json` com a decisão aprovada;
- `docs/changes/RFC-0001-phase4-team-composition.md`;
- `docs/decisions/ADR-0003-mode-specific-formation-profiles.md`.

Não bloqueia mais:

- Fase 4 — Formation & Field.

## OPEN-002 — Bola fora e reposições

**Status:** OPEN

Contexto:

Há intenções históricas diferentes para arena com bordas, modo clássico, ausência/presença de lateral/escanteio e reposicionamento após saída.

Decisão necessária:

- quais rulesets permitem bola sair;
- como posse é transferida;
- se existem lateral/escanteio formais;
- como funciona tiro de meta;
- local e regras de reposicionamento.

Bloqueia:

- Fase 7 — Rules & Restarts definitiva.

## OPEN-003 — Faltas, vantagem e pênaltis

**Status:** OPEN

Decisão necessária:

- eventos que constituem falta;
- quando vantagem existe;
- critério de pênalti;
- posicionamento na cobrança;
- elegibilidade das peças;
- impacto em turno/posse.

Bloqueia:

- implementação definitiva dessas regras.

## OPEN-004 — Baseline numérico de competitivo

**Status:** OPEN/TUNING

O legado possui valores de duração, limite de gols, entradas e recompensas. Esses valores serão registrados como benchmark e depois confirmados ou calibrados.

A identidade de Liga Justa/Campeonato e ausência de pay-to-win são bloqueadas; números exatos são tuning até congelamento de balanceamento.

## OPEN-005 — Tecnologia final de UI dentro da Unity

**Status:** OPEN ARCHITECTURE DETAIL

A direção visual é bloqueada, mas a escolha entre UI Toolkit, uGUI ou combinação controlada deve ser validada por um spike de fidelidade com uma tela representativa da referência.

Critérios:

- fidelidade;
- gamepad navigation;
- animação;
- performance;
- manutenção;
- suporte a resolução/aspect ratio.

Não é permitido escolher tecnologia e depois alterar o design para caber na limitação da ferramenta sem RFC visual.
