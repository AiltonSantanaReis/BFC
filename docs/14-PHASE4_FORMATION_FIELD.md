# BFC Phase 4 — Formation & Field

Status: **KICKOFF / BLOCKED BY OPEN-001 FOR DEFINITIVE FORMATION**

## Objetivo

Ampliar o laboratório físico para uma estrutura de campo e formação jogável, preservando configuração por ruleset e sem hardcode da quantidade oficial de peças.

## Pré-condição normativa

A Fase 4 possui uma pré-condição explícita: `OPEN-001 — Quantidade oficial de peças por equipe` precisa ser resolvida pelo Product Owner antes de a formação oficial ser tratada como definitiva.

Enquanto `OPEN-001` permanecer aberto, esta branch pode preparar somente infraestrutura neutra quanto à contagem de peças. Não pode registrar um número temporário como regra oficial.

## Decisão necessária — OPEN-001

É necessário confirmar explicitamente:

- quantidade total oficial de peças por equipe;
- se o goleiro está incluído nessa quantidade ou é adicional;
- se treino/desafios podem usar formações alternativas, mantendo a formação competitiva oficial intacta.

## Escopo técnico após a decisão

- definição configurável de formação;
- identificação explícita da função goleiro;
- formação inicial por equipe;
- definição de campo de jogo;
- áreas de gol;
- traves e volumes de gol;
- limites geométricos do campo;
- spawn/reposition seguro;
- suporte a formações alternativas em treino/desafios sem duplicar lógica central;
- integração com o Match Core e a física já validados.

## Restrições de arquitetura

- nenhum número de peças pode ficar hardcoded em lógica central;
- `BFC.Core`/`BFC.Gameplay` continuam independentes de cena e apresentação;
- física não decide formação nem regras de posse;
- goleiro é papel de domínio, não inferência por nome de GameObject;
- conteúdo de formação deve ser configurável por definição/ruleset;
- `OPEN-002` e `OPEN-003` permanecem fora do escopo desta fase.

## Gate previsto

- `OPEN-001` resolvido e documentado;
- testes de definição/spawn/formação;
- nenhuma contagem fixa duplicada fora de definição/ruleset;
- formação oficial materializada no campo;
- goleiro identificado como função distinta;
- performance estável no target Windows;
- EditMode/PlayMode existentes permanecem verdes;
- CI de Governance e Unity Structure verde no head final.

## Estado atual

Branch de trabalho criada a partir do `main` consolidado da Fase 3. Nenhuma decisão de quantidade de peças foi inferida durante o kickoff.
