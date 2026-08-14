# BFC Phase 4 — Formation & Field

Status: **ACTIVE — DOMAIN SLICE VALIDATED / RUNTIME MATERIALIZATION IMPLEMENTED, UNITY VALIDATION PENDING**

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

## Primeira fatia — domínio validado

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

A primeira fatia foi validada no Unity 6000.3.21f1 com EditMode 21/21, PlayMode 1/1, metadados Unity revisados e CI de Governance/Unity Structure verde antes do merge.

## Segunda fatia — materialização de runtime

Foi criado um harness dedicado `FormationLab` que consome as definições de domínio em vez de duplicar contagem e posicionamento na cena.

A materialização atual cria em runtime:

- superfície de campo configurável;
- marcações principais e áreas de gol;
- duas estruturas de gol e volumes de gol;
- Team A e Team B a partir de `FormationSpawnPlanner`;
- 11 peças por equipe no perfil Campo Grande, totalizando 22 peças;
- 10 peças de linha + 1 goleiro por equipe, com papel explícito em `FormationPieceRuntime`;
- bola com `PlanarKineticBody`;
- peças com root sem escala e visual cilíndrico como filho, evitando repetir o problema de collider sob escala não uniforme do laboratório inicial.

`FormationLabPreviewProfiles` fornece um campo e uma formação balanceada apenas para visualização e testes da Fase 4. As dimensões `28 x 18` e o layout usado no preview são **não normativos** e não congelam tamanho oficial nem uma formação tática obrigatória.

A cena `Assets/BFC/Scenes/FormationLab.unity` permanece como laboratório de desenvolvimento e fica desabilitada no build settings; o `Bootstrap` continua sendo a única cena de entrada habilitada.

## Restrições de arquitetura

- nenhum número de peças pode ficar hardcoded em lógica central de partida;
- o número 11 existe somente na definição do perfil campo grande e em expectativas explícitas de teste desse perfil;
- `BFC.Core` continua independente de `UnityEngine`;
- física não decide formação nem regras de posse;
- goleiro é papel de domínio, não inferência por nome de GameObject;
- conteúdo de formação/campo é configurável por definição/ruleset;
- o FormationLab é um adapter/harness Unity e não fonte normativa de regras;
- `OPEN-002` e `OPEN-003` permanecem fora do escopo desta fase.

## Decisões ainda não congeladas

A resolução de `OPEN-001` e o preview de runtime não escolhem silenciosamente:

- dimensão final do campo grande;
- formação tática única obrigatória (4-4-2, 4-3-3 etc.);
- quantidade exata dos perfis derivados do legado quando sua definição específica ainda não tiver sido materializada;
- regras de saída/reposição;
- faltas/pênaltis;
- tuning físico final das peças, bola, goleiro ou campo.

## Gate da Fase 4

- [x] `OPEN-001` resolvido e documentado;
- [x] composição configurável por modo;
- [x] perfil campo grande com 11 total = 10 linha + 1 goleiro;
- [x] formação validada contra composição;
- [x] goleiro identificado como função distinta;
- [x] definição lógica configurável de campo;
- [x] planejamento de spawn espelhado e com margem segura;
- [x] primeira fatia validada no Unity 6000.3.21f1;
- [x] integração visual/física implementada em `FormationLab`;
- [x] PlayMode smoke test para 22 peças, 2 goleiros e 1 bola implementado;
- [ ] Unity 6000.3.21f1 importa/compila a segunda fatia sem erros;
- [ ] EditMode existente permanece 21/21 ou melhor;
- [ ] PlayMode inclui e aprova o novo smoke test;
- [ ] novos `.meta` gerados pelo Unity são revisados e commitados;
- [ ] inspeção manual do FormationLab confirma campo, 22 peças, 2 goleiros, bola e gols visíveis;
- [ ] performance do preview não apresenta regressão evidente no target Windows;
- [ ] CI Governance e Unity Structure verdes no head final.

## Próxima decisão após esta fatia

Depois de validar o FormationLab, a Fase 4 pode avançar para perfis adicionais de campo/formação e integração gradual com fluxo de partida. O `BFC Classic Simulation` permanece registrado separadamente para especificação do modelo de golpe antes/durante a Fase 5, sem interromper esta fundação.
