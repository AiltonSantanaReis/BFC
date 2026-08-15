# BFC Phase 4 — Formation & Field

Status: **COMPLETE — PRODUCT OWNER CLOSURE APPROVED; PR #10 READY FOR REVIEW / MERGE AUTHORIZATION**

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

## Terceira fatia — gate objetivo de performance Windows

O PR #10 adiciona um build Windows x64 dedicado ao `FormationLab` e um probe opt-in de frame time. O harness permanece fora do build de produção e não fixa um requisito numérico de FPS.

A validação real em Windows/Unity 6000.3.21f1 expôs dois defeitos player-only antes da captura funcionar:

1. shaders localizados apenas por `Shader.Find(...)` não estavam garantidos no player standalone;
2. o `defaultMaterial` do Render Pipeline Asset não era uma fonte utilizável no standalone dedicado.

A infraestrutura do PR passou então a criar, somente durante o build diagnóstico, um Material URP Lit temporário sob `Resources`, garantindo uma referência concreta no player. O diretório temporário é removido ao final do build e não permanece no repositório.

### Evidência real de performance

Captura concluída com sucesso no target Windows do Product Owner:

- Unity: **6000.3.21f1**;
- resolução: **1280 x 720**;
- warmup: **2 s**;
- janela de amostragem: **10 s**;
- VSync desativado apenas para o diagnóstico;
- CPU registrada: **AMD Ryzen 7 5700X3D 8-Core Processor**;
- GPU registrada: **NVIDIA GeForce RTX 3070 Ti**;
- amostras: **23595**;
- frame time médio: **0.424 ms**;
- mediana: **0.383 ms**;
- p95: **0.527 ms**;
- p99: **1.012 ms**;
- máximo observado: **275.347 ms**;
- FPS médio derivado do frame time: **2359.451**.

Interpretação de gate: a distribuição central e os percentis permanecem baixos durante a janela de 10 s e a captura completa sem timeout, crash ou erro de runtime. O valor máximo isolado é registrado como outlier e não é apagado da evidência. Nenhum desses números é declarado requisito de produção; o objetivo é demonstrar estabilidade do preview atual no target Windows dentro deste escopo de laboratório.

Após o build/captura, `Assets/BFC/Settings/FormationLabBuildAssets` foi confirmado como ausente (`Test-Path` = `False`), portanto o asset temporário não deixou ruído no workspace.

Na mesma revisão do head `96464f0`:

- EditMode: **21/21 passed**;
- PlayMode: **2/2 passed**.

O novo `FormationLabPerformanceProbe.cs.meta` também foi revisado e versionado:

- GUID: `b9bed3d5a30ce2249be94c1bc153064d`;
- formato: 32 dígitos hexadecimais;
- ocorrência sob `Assets/`: única;
- `git diff --cached --check`: sem erro;
- commit local/push: `f5e854b` (`materialize FormationLab performance probe metadata`);
- working tree local informado após o push: limpa.

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
- [x] novos `.meta` gerados pelo Unity revisados e commitados na segunda fatia;
- [x] inspeção manual do FormationLab confirma campo, 22 peças, 2 goleiros, bola e gols visíveis;
- [x] CI Governance e Unity Structure verdes no head final da segunda fatia;
- [x] evidência objetiva de performance do preview capturada com sucesso no target Windows;
- [x] `.meta` do novo `FormationLabPerformanceProbe.cs` revisado e versionado;
- [x] Product Owner decidiu explicitamente que o escopo atual satisfaz o gate completo, sem fatia adicional de Match Core/perfis nesta fase;
- [x] CI `Governance` e `Unity Structure` verdes no head `4151bbe0d390b0a14524260cbe5da0c0c7feeca1` imediatamente anterior a este registro final; como este commit altera apenas documentação, os checks do novo head ainda devem ser conferidos antes do merge.

## Decisão de encerramento da Fase 4

Em 2026-08-14, o Product Owner autorizou explicitamente: **encerrar a Fase 4 com o escopo atual**.

A decisão é compatível com `docs/04-DEVELOPMENT_PLAN.md`: o gate normativo exige configuração de composição, campo/formação, spawn seguro, testes e performance estável no target Windows. Integração adicional com Match Core não aparece como entrega/gate obrigatório da Fase 4 e, portanto, não será introduzida apenas para ampliar escopo depois que o gate definido foi satisfeito.

A Fase 4 está **concluída em escopo e validação real**. O PR #10 pode ser colocado como Ready for review assim que os checks do head documental atual terminarem verdes. O merge continua sujeito a autorização explícita separada do Product Owner.

## Próximo passo

- confirmar `Governance` e `Unity Structure` verdes no head documental atual do PR #10;
- marcar o PR #10 como Ready for review;
- não mesclar sem autorização explícita do Product Owner;
- após o merge, sincronizar `main` local e registrar o merge SHA no handoff;
- iniciar a preparação da Fase 5 — Advanced Actions, incluindo a especificação do `BFC Classic Simulation` / Classic Strike Model antes ou durante a fase, sem antecipar decisões de `OPEN-002`/`OPEN-003`.
