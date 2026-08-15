# BFC Phase 4 — Formation & Field

Status: **ACTIVE — DOMAIN + RUNTIME SLICES VALIDATED / MERGED; FINAL PHASE GATE STILL OPEN**

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

## Primeira fatia — domínio validado e mesclado

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

### Validação da primeira fatia

Validada no Unity 6000.3.21f1 antes do merge:

- EditMode: **21/21 passed**;
- PlayMode: **1/1 passed**;
- 10 `.meta` gerados pelo Unity revisados e commitados;
- CI `Governance` verde;
- CI `Unity Structure` verde.

Merge:

- PR #6: `[GOVERNANCE CHANGE] Implement Phase 4 formation and field`;
- merge por squash: `6b80ee5bfe65d9fb183c9f05db175b864db005e9`.

## Segunda fatia — materialização de runtime validada e mesclada

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

A cena `Assets/BFC/Scenes/FormationLab.unity` é laboratório de desenvolvimento e permanece **fora do player build**. `Bootstrap` continua sendo a entrada de produção. A serialização real do Unity 6000.3.21f1 para `EditorBuildSettings.asset` foi aceita e o validador estrutural foi ajustado para exigir essa separação.

### Problemas encontrados pela validação real

A primeira execução no Unity detectou erros que o CI estático não havia detectado:

- dependência desnecessária de `AudioListener` em um assembly que não precisava do módulo de áudio;
- uso de `FormationSpawn.SlotId`, embora o contrato correto exponha `PieceId`.

A correção foi feita no commit `6ab8355` e revalidada no Unity.

### Validação final da segunda fatia

Em Unity 6000.3.21f1:

- importação/compilação concluída sem os erros anteriores;
- EditMode: **21/21 passed**;
- PlayMode: **2/2 passed**;
- smoke test confirma 22 peças de formação, 2 goleiros, 20 peças de linha, roots em escala unitária e 23 corpos físicos incluindo a bola;
- inspeção manual do `FormationLab` aprovada: campo inteiro enquadrado, 11 peças de cada lado, goleiros, bola, áreas e gols visíveis;
- Console da evidência do FormationLab sem erro/warning visível;
- 6 novos `.meta` revisados, com GUIDs presentes e únicos;
- metadata commitada em `0be852b`;
- CI `Governance` verde no head final;
- CI `Unity Structure` verde no head final.

Merge:

- PR #8: `Implement Phase 4 runtime field materialization`;
- merge por squash: `5ab0db9583aad4c7695dea1aeeb8863208dcfec1`.

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
- [x] primeira fatia validada no Unity 6000.3.21f1 e mesclada;
- [x] integração visual/física implementada em `FormationLab`;
- [x] PlayMode smoke test para 22 peças, 2 goleiros e 1 bola implementado e aprovado;
- [x] Unity 6000.3.21f1 importa/compila a segunda fatia após as correções;
- [x] EditMode permanece **21/21**;
- [x] PlayMode passa **2/2**;
- [x] novos `.meta` gerados pelo Unity revisados e commitados;
- [x] inspeção manual do FormationLab confirma campo, 22 peças, 2 goleiros, bola e gols visíveis;
- [x] CI Governance e Unity Structure verdes no head final da segunda fatia;
- [ ] registrar evidência objetiva suficiente de performance estável do preview no target Windows;
- [ ] decidir explicitamente se o gate da Fase 4 exige uma fatia adicional de integração com o fluxo de partida ou perfis adicionais de campo/formação antes de encerrar a fase.

## Estado da fase após PR #8

As duas primeiras fatias da Fase 4 estão concluídas e mescladas. A fase permanece **ACTIVE** porque o plano normativo exige performance estável no target Windows e ainda deve haver uma decisão explícita sobre a suficiência do escopo atual para fechar o gate completo.

Não iniciar silenciosamente a Fase 5 como se a Fase 4 já estivesse encerrada.

## Próximo passo

- capturar evidência mínima e objetiva de performance do FormationLab em Windows;
- revisar se materialização + testes atuais já satisfazem o requisito de campo/formação jogável ou se falta integração adicional com o Match Core;
- decidir se perfis adicionais precisam ser materializados antes do gate;
- quando o Product Owner aprovar o fechamento, atualizar status/documentação da Fase 4 para concluída;
- preparar a especificação do `BFC Classic Simulation` / Classic Strike Model antes ou durante a Fase 5, sem antecipar decisões de `OPEN-002`/`OPEN-003`.
