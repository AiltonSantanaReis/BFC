# BFC Open Decisions

Este arquivo contém decisões que **não podem ser resolvidas por suposição de implementação**.

Quando resolvida, a decisão deve gerar ADR/RFC/regra conforme o impacto e mover seu resultado para o documento normativo adequado.

## OPEN-001 — Quantidade oficial de peças por equipe

**Status:** OPEN

Contexto:

- o BFC legado usa formação reduzida;
- a visão posterior do produto aponta para campo maior e formação ampliada;
- a arquitetura deve suportar quantidade configurável.

Decisão necessária:

- quantidade total oficial por equipe;
- se o goleiro está incluído nessa contagem ou é adicional;
- formações alternativas permitidas por treino/desafio.

Bloqueia:

- Fase 4 — Formation & Field como regra definitiva.

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
