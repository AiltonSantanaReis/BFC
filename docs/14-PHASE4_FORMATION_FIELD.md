# BFC Phase 4 — Formation & Field

Status: **ACTIVE — OPEN-001 RESOLVED / FIRST DOMAIN SLICE IMPLEMENTED**

## Objetivo

Ampliar o laboratório físico para uma estrutura de campo e formação jogável, preservando configuração por modo/ruleset e sem hardcode de uma quantidade global de peças.

## Decisão normativa — OPEN-001

`OPEN-001` foi resolvido pelo Product Owner em 2026-08-14.

Decisão:

- BFC não possui uma única contagem global de peças para todos os modos;
- modos derivados do baseline legado preservam seus padrões de campo/composição por configuração própria;
- o perfil oficial de **campo grande** usa **11 peças no total por equipe**;
- o perfil campo grande contém **10 jogadores de linha + 1 goleiro**, com o goleiro incluído nas 11 peças;
- Treino e Desafios podem definir campo, quantidade de peças e formação conforme cada cenário.

Registros:

- `docs/changes/RFC-0001-phase4-team-composition.md`;
- `docs/decisions/ADR-0003-mode-specific-formation-profiles.md`;
- `OPEN-001` está `locked` no registro de governança.

## Primeira fatia implementada

### Composição de equipe

`TeamCompositionDefinition` concentra quantidade total, goleiros e jogadores de linha. O perfil `LargeFieldEleven` é uma definição específica do campo grande e não uma constante global da partida.

### Formação

`FormationDefinition` valida slots contra a composição escolhida. Cada slot possui:

- identificador lógico;
- papel (`Outfield` ou `Goalkeeper`);
- coordenada longitudinal normalizada;
- coordenada lateral normalizada.

A formação não depende de GameObject, cor, sprite, prefab ou nome de cena.

### Campo

`FieldDefinition` descreve geometria lógica configurável:

- comprimento;
- largura;
- largura da boca do gol;
- profundidade do gol;
- comprimento e largura da área de gol.

As dimensões finais de produção continuam sendo dados de conteúdo/tuning; esta fase não declarou um tamanho universal de campo.

### Spawn seguro

`FormationSpawnPlanner` converte slots normalizados em coordenadas XZ respeitando margem de segurança e espelha longitudinalmente a mesma formação para Team A e Team B.

Isso permite usar a mesma infraestrutura em campo grande, modos derivados do legado e cenários de Treino/Desafios.

## Restrições de arquitetura

- nenhum número de peças pode ficar hardcoded em lógica central de partida;
- o número 11 existe somente na definição do perfil campo grande;
- `BFC.Core` continua independente de `UnityEngine`;
- física não decide formação nem regras de posse;
- goleiro é papel de domínio, não inferência por nome de GameObject;
- conteúdo de formação/campo é configurável por definição/ruleset;
- `OPEN-002` e `OPEN-003` permanecem fora do escopo desta fase.

## Decisões ainda não congeladas

A resolução de `OPEN-001` não escolhe silenciosamente:

- dimensão final do campo grande;
- formação tática única obrigatória (4-4-2, 4-3-3 etc.);
- quantidade exata dos perfis derivados do legado quando sua definição específica ainda não tiver sido materializada;
- regras de saída/reposição;
- faltas/pênaltis.

## Gate da Fase 4

- [x] `OPEN-001` resolvido e documentado;
- [x] composição configurável por modo;
- [x] perfil campo grande com 11 total = 10 linha + 1 goleiro;
- [x] formação validada contra composição;
- [x] goleiro identificado como função distinta;
- [x] definição lógica configurável de campo;
- [x] planejamento de spawn espelhado e com margem segura;
- [ ] Unity 6000.3.21f1 importa/compila a nova fatia;
- [ ] novos testes EditMode passam;
- [ ] testes existentes permanecem verdes;
- [ ] `.meta` gerados pelo Unity revisados e commitados;
- [ ] integração visual/física materializada em um campo de runtime;
- [ ] performance estável no target Windows;
- [ ] CI Governance e Unity Structure verdes no head final.

## Próxima fatia

Depois da validação desta camada de domínio, materializar um `FieldLab`/harness de runtime que consuma `FieldDefinition` e `FormationDefinition`, criando peças e goleiro sem duplicar contagem/posicionamento na cena.
