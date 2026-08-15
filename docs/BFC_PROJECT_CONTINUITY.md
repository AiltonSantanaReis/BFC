# BFC — Documento Mestre de Continuidade, Status e Handoff

**Versão:** 1.2  
**Última atualização operacional:** 2026-08-15 07:20 (BRT / UTC-03:00)  
**Repositório:** `AiltonSantanaReis/BFC`  
**Workspace local principal:** `F:\Projetos\BFC`  
**Branch canônica:** `main`  
**HEAD confirmado de `main`:** `68279f8b89c5ecd70d380cc83071d2c2a5dfa6cf`  
**Último merge:** PR #10 — `Implement Phase 4 Windows performance gate`  
**Merge SHA do PR #10:** `68279f8b89c5ecd70d380cc83071d2c2a5dfa6cf`  
**Estado da Fase 4:** **CONCLUÍDA E INTEGRADA EM `main`**  
**Branch documental atual:** `agent/post-phase4-continuity`  
**Próximo milestone:** Fase 5 — `Advanced Actions`  
**Estado local confirmado pelo Product Owner antes desta atualização:** branch `agent/post-phase4-continuity`, HEAD `68279f8b89c5ecd70d380cc83071d2c2a5dfa6cf`, working tree limpa.

> Este é o handoff operacional oficial do BFC. Ele existe para permitir retomar o projeto em uma nova conversa sem depender do histórico anterior.
>
> Ele **não substitui documentos normativos**. Em caso de conflito, prevalece a ordem de autoridade registrada abaixo e nos documentos normativos do repositório.

---

## 1. Retomada rápida em uma nova conversa

Use este prompt:

```text
Estamos continuando o projeto BFC no repositório AiltonSantanaReis/BFC.
Use docs/BFC_PROJECT_CONTINUITY.md como handoff operacional.
Antes de alterar código, confira no GitHub o HEAD atual de main, PRs abertos, issues relevantes
e os documentos normativos.
Não mude regras LOCKED por conveniência técnica.
OPEN-002 e OPEN-003 permanecem abertas.
Não faça merge de PR sem minha autorização explícita.
Continue a partir da seção "Próximo passo seguro" do documento de continuidade.
```

Ao retomar:

1. verificar `main`, PRs e issues no GitHub;
2. comparar a realidade do repositório com este handoff;
3. se houver divergência, atualizar primeiro o handoff;
4. não reconstruir decisões críticas por suposição;
5. nunca inferir autorização de merge de uma autorização de escopo/fase.

---

## 2. Identidade e direção oficial do produto

O produto oficial chama-se **BFC** e está sendo reconstruído do zero em **Unity**, usando os materiais anteriores como referência funcional e visual, sem transportar a arquitetura legada como dependência de produção.

### Referência funcional

`BFC(1).zip` / BFC legado serve como referência de intenção para:

- seleção + drag/aim + força + release;
- potência e sensação física;
- chute, passe, spin e chip;
- resolução física antes da próxima ação;
- até 3 ações por posse quando o ruleset exigir;
- goleiro automático;
- P1 vs COM e P1 vs P2;
- Liga Justa;
- Campeonato;
- Treino e Desafios;
- progressão;
- economia;
- missões;
- conquistas;
- troféus/vitrine;
- recompensas inclusive em derrota;
- save e perfil.

Não transportar como requisito:

- física dependente de FPS;
- monólitos/acoplamentos;
- pseudo-Z do chip;
- IA fraca;
- online fake;
- números antigos não aprovados;
- armazenamento legado como arquitetura final.

### Referência visual

`Interface e Menu.zip` é a referência visual oficial. Preservar:

- identidade cyber/neon;
- cyan e magenta;
- composição e layout;
- hierarquia;
- cards;
- painéis;
- modais;
- fundos;
- vitrine de troféus;
- densidade visual;
- animações e atmosfera.

Migrar para Unity **não autoriza redesign** da identidade aprovada.

### Princípios de produto

- física legível;
- decisões táticas;
- habilidade de execução;
- competitivo sem pay-to-win;
- cosméticos não alteram desempenho competitivo;
- regras autoritativas não ficam na UI;
- IA usa as mesmas regras válidas do jogador;
- online real futuro exige autoridade de backend/server.

---

## 3. Ordem de autoridade

Quando houver conflito:

1. decisão explícita mais recente do Product Owner registrada no repositório;
2. `governance/rules.json`;
3. `docs/00-PRODUCT_CHARTER.md`;
4. `docs/01-GAMEPLAY_CONSTITUTION.md`;
5. `docs/02-VISUAL_CONSTITUTION.md`;
6. ADRs/RFCs aceitos;
7. `docs/04-DEVELOPMENT_PLAN.md`;
8. comportamento de referência do legado;
9. implementação atual.

**Código não tem autoridade superior à regra aprovada.**

---

## 4. Baseline técnico e arquitetura

- Unity **6.3 LTS**;
- editor exato **6000.3.21f1**;
- URP **17.3.0**;
- Input System **1.20.0**;
- Test Framework **1.6.0**;
- C#;
- Windows x64 primeiro;
- Windows 11 no ambiente local principal;
- PowerShell + Git;
- workspace `F:\Projetos\BFC`.

Regras arquiteturais principais:

- `BFC.Core` independente de `UnityEngine`;
- regras centrais testáveis sem UI;
- lógica central não depende de GameObject/cena;
- física usa fixed-step;
- regra competitiva não depende de FPS;
- Bootstrap é o composition root;
- evitar ECS/DOTS sem necessidade mensurável;
- Physics não decide posse, score ou recompensa;
- Presentation não decide regra;
- IA deve emitir comandos válidos pelas mesmas regras do jogador.

Estrutura conceitual:

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

Responsabilidades resumidas:

- `BFC.Core`: entidades, value objects, identificadores, contratos e definições independentes da engine;
- `BFC.Gameplay`: lifecycle, turno, posse, comandos, validação, score, relógio e transições;
- `BFC.Physics`: impulso, colisão, desaceleração, repouso, spin, bola aérea e bridge com PhysX;
- `BFC.AI`: percepção, avaliação, seleção, planejamento, força/direção e dificuldade;
- `BFC.Modes`: composição de rulesets;
- `BFC.Progression`: perfil, inventário, moedas, missões, conquistas, troféus e recompensas;
- `BFC.Presentation`: UI, HUD, navegação, câmeras, VFX e feedback;
- `BFC.Infrastructure`: save, armazenamento, plataforma e serviços futuros;
- `BFC.Bootstrap`: composition root autorizado.

---

## 5. Regras de gameplay já protegidas

### Núcleo

- futebol de botão por ações;
- controle moderno: selecionar + drag/aim + força + release;
- aguardar resolução física antes da próxima ação;
- competitivo oficial: até **3 ações por posse** quando o ruleset usar essa regra;
- `ação` não é automaticamente igual a `toque na bola`;
- P1 vs COM;
- P1 vs P2.

### Ações

- chute;
- passe com força máxima menor que o chute, percentual final ainda tunable;
- spin/efeito;
- chip/bola por cima 3D;
- menu radial/contextual;
- feedback de força/direção.

### Goleiro

- goleiro automático em competitivo oficial;
- reação, alcance e velocidade são tuning;
- controle manual não pode surgir silenciosamente em ruleset que define goleiro automático.

### Competitivo e progressão

- Liga Justa;
- Campeonato;
- sem pay-to-win competitivo;
- derrota ainda pode gerar progressão/recompensa;
- perfil persistente;
- missões;
- conquistas;
- troféus/vitrine;
- loja com estados locked / available / owned / equipped;
- cosméticos não alteram hitbox, massa, atrito, potência ou spin competitivo.

---

## 6. OPEN-001 — resolvida e LOCKED

Decisão do Product Owner em 2026-08-14:

- BFC **não possui uma contagem universal de peças para todos os modos**;
- perfil oficial de campo grande = **11 peças totais por equipe**;
- composição = **10 jogadores de linha + 1 goleiro**;
- goleiro incluído nas 11;
- modos derivados do legado podem ter composição própria por configuração;
- Treino e Desafios podem variar campo, quantidade e formação por cenário;
- lógica central não deve possuir número mágico global de peças.

Registros:

- `OPEN-001` em `governance/rules.json`: `locked`;
- `docs/changes/RFC-0001-phase4-team-composition.md`;
- `docs/decisions/ADR-0003-mode-specific-formation-profiles.md`;
- `docs/OPEN_DECISIONS.md`;
- `docs/01-GAMEPLAY_CONSTITUTION.md`.

Não foram congelados:

- dimensão final do campo grande;
- formação tática única;
- quantidade exata de todos os perfis derivados do legado;
- regras de saída/reposição;
- faltas/pênaltis;
- tuning físico final.

---

## 7. Decisões ainda abertas

### OPEN-002 — bola fora e reposições

Ainda deve definir, entre outros pontos:

- quais rulesets permitem saída;
- transferência de posse;
- lateral/escanteio ou ausência deles;
- tiro de meta;
- reposicionamento.

### OPEN-003 — faltas, vantagem e pênaltis

Ainda deve definir:

- o que constitui falta;
- vantagem;
- pênalti;
- posicionamento de cobrança;
- elegibilidade;
- impacto em posse/turno.

### OPEN-004 — baseline numérico competitivo

Duração, limite de gols, rating, entradas, recompensas e demais números continuam benchmark/tuning.

### OPEN-005 — tecnologia final da UI

Ainda requer spike de fidelidade entre UI Toolkit, uGUI ou combinação controlada. A escolha técnica não pode obrigar redesign da Interface e Menu.

---

## 8. BFC Classic Simulation — direção registrada

Issue oficial: **#7 — `Specify BFC Classic Simulation mode`**.

Esse modo **não substitui** o controle moderno.

Princípio:

```text
impact point + strike direction/angle + force
                    ↓
movimento linear + rotação da peça
                    ↓
contato físico peça × bola
                    ↓
trajetória emergente da bola
```

Não implementar como:

```text
ball -> target X/Y
```

Objetivos:

- ponto de impacto importa;
- ângulo importa;
- força importa;
- erro emerge principalmente da execução física, não de RNG artificial;
- a bola resulta da colisão física, não de trajetória escolhida diretamente.

Goleiro no Classic:

- reposicionável apenas em janelas explícitas autorizadas por ruleset;
- não existe autorização para movimento manual contínuo;
- situações exatas dependem das futuras decisões de restart/falta/pênalti.

Conceito futuro:

```text
GoalkeeperPlacementWindow
- Team
- AllowedArea
- MaxDisplacement
- CanRotate
- TimeLimit
- ConfirmationRequired
```

A especificação do **Classic Strike Model** deve ser preparada antes ou durante a Fase 5, sem antecipar OPEN-002/003.

---

## 9. Histórico de fases e merges

### Fase 0 — Foundation & Governance

**Status:** concluída e mesclada.

- PR #1 — `Establish BFC Unity foundation and governance`;
- merge SHA: `ae3a87236bf1b0742bb98e131fbd4729909c2724`;
- documentos normativos, arquitetura, governança, PR template, CODEOWNERS e baseline Unity estabelecidos.

PR #2 foi noop/acidental e foi fechado sem efeito de produto.

### Fase 1 — Unity Bootstrap

**Status:** concluída e mesclada.

- PR #3 — `Implement Phase 1 Unity bootstrap`;
- merge SHA: `408a4b1f281b025d991c76303c83f63d6e514672`;
- Unity abre/compila;
- URP/Input System ativos;
- EditMode **2/2**;
- PlayMode **1/1**;
- Windows x64 build e smoke test aprovados;
- CI final verde.

### Fase 2 — Physics Vertical Slice

**Status:** concluída e mesclada.

- PR #4 — `Implement Phase 2 physics vertical slice`;
- merge SHA: `43c3f0961805ce177a770d73d56e337fe9b469ae`;
- drag/release, potência, peça→bola, peça→peça, peça→parede, desaceleração, repouso, parâmetros centralizados e fixed-step;
- EditMode **5/5**;
- PlayMode **1/1**;
- CI final verde.

Caveats preservados:

- `PhysicsLabTuning` ainda é provisório;
- movimento principal planar XZ;
- collision detection do laboratório ficou `Discrete`, não congelada como decisão final de produção.

### Fase 3 — Match Core

**Status:** concluída e mesclada.

- PR #5 — `Implement Phase 3 match core`;
- merge SHA: `c6d636a2ce93461b8034c17db6a01c8518e65ea4`;
- `MatchState`, fases, posse, `PlayerActionCommand`, validação, `ResolvingAction`, espera por resolução física, contador/limite de ações vindo do ruleset, score, gol, `AwaitingRestart`, `ResumeAfterRestart(nextPossession)`, relógio lógico, eventos e fim explícito;
- 8 testes próprios do MatchController;
- EditMode total **13/13**;
- PlayMode **1/1**;
- metadata revisada;
- CI verde;
- OPEN-002/003 não decididas.

### Fase 4 — Formation & Field

**Status:** **CONCLUÍDA E INTEGRADA EM `main`**.

#### Fatia 1 — domínio

- PR #6 — `[GOVERNANCE CHANGE] Implement Phase 4 formation and field`;
- merge SHA: `6b80ee5bfe65d9fb183c9f05db175b864db005e9`;
- resolveu OPEN-001;
- adicionou `PieceRole`, `TeamCompositionDefinition`, `LargeFieldEleven`, `FormationSlot`, `FormationDefinition`, `FieldDefinition`, `FormationSpawn`, `FormationSpawnPlanner`, RFC-0001 e ADR-0003;
- EditMode **21/21**;
- PlayMode **1/1**;
- 10 `.meta` revisados;
- Governance/Unity Structure verdes.

#### Fatia 2 — runtime materialization

- PR #8 — `Implement Phase 4 runtime field materialization`;
- merge SHA: `5ab0db9583aad4c7695dea1aeeb8863208dcfec1`;
- `FormationLab` materializa campo, marcações, áreas, gols, volumes de gol, bola, Team A e Team B;
- 22 peças = 11 por equipe;
- 20 outfield + 2 goleiros;
- papel explícito via `FormationPieceRuntime`;
- spawns derivados de `FormationSpawnPlanner`;
- `PlanarKineticBody` em peças e bola;
- root físico das peças em escala unitária e visual cilíndrico em filho escalado.

Validação real:

- primeira execução encontrou `AudioListener` desnecessário e uso incorreto de `FormationSpawn.SlotId` em vez de `PieceId`;
- correção principal: commit `6ab8355`;
- EditMode **21/21**;
- PlayMode **2/2**;
- inspeção visual aprovada;
- 11 peças por lado, goleiros, bola, áreas e gols visíveis;
- sem erro/warning visível na evidência do FormationLab;
- 6 `.meta` revisados;
- CI verde.

Preview da Fase 4:

- 10 outfield + 1 GK por equipe;
- campo de preview 28×18;
- layout e dimensões **não normativos**.

`FormationLab` permanece fora do build de produção. `Bootstrap` é a entrada de produção.

#### Fatia 3 — Windows performance gate / PR #10

PR #10 adicionou:

- build Windows x64 dedicado do `FormationLab`, sem colocá-lo no build de produção;
- `FormationLabPerformanceProbe` opt-in via `-bfcFormationPerf`;
- VSync desativado somente no diagnóstico;
- warmup e amostragem por tempo;
- métricas `avg`, `median`, `p95`, `p99`, `max`, `avgFps`;
- `scripts/run-formationlab-performance.ps1` com timeout e exigência de linha `RESULT`;
- material URP Lit temporário sob build-only `Resources` para garantir inclusão do material/shader no standalone;
- cleanup automático do asset temporário após o build.

Defeitos player-only encontrados e corrigidos durante validação real:

1. `Shader.Find("Universal Render Pipeline/Lit")` / fallback `Standard` não garantiam shader utilizável após stripping;
2. `RenderPipelineAsset.defaultMaterial` também não forneceu material utilizável no dedicated standalone;
3. solução final: material URP Lit temporário em `Resources` durante o build diagnóstico, carregado pelo FormationLab e removido após o build.

Erros observados durante as tentativas:

```text
InvalidOperationException: FormationLab could not resolve a runtime material shader.
```

```text
InvalidOperationException: FormationLab could not resolve the active render pipeline default material.
```

Cleanup confirmado:

```text
Test-Path Assets/BFC/Settings/FormationLabBuildAssets
False
```

Validação Unity real no runtime head antes dos commits finais de metadata/documentação:

```text
EditMode: 21/21 passed
PlayMode:  2/2 passed
```

Captura Windows real bem-sucedida:

```text
Unity:       6000.3.21f1
CPU:         AMD Ryzen 7 5700X3D 8-Core Processor
GPU:         NVIDIA GeForce RTX 3070 Ti
resolution:  1280x720
warmup:      2 s
sample:      10 s
samples:     23595
avgMs:       0.424
medianMs:    0.383
p95Ms:       0.527
p99Ms:       1.012
maxMs:       275.347
avgFps:      2359.451
```

Interpretação aprovada:

- captura completou normalmente;
- distribuição central/p95/p99 sustenta estabilidade do preview atual no target Windows dentro deste laboratório;
- `maxMs=275.347` permanece registrado como outlier real;
- `avgFps=2359.451` **não é requisito de produção**;
- o harness não representa UI final, IA, VFX, áudio final e demais sistemas futuros.

Metadata do probe:

```text
Assets/BFC/FormationLab/FormationLabPerformanceProbe.cs.meta
fileFormatVersion: 2
guid: b9bed3d5a30ce2249be94c1bc153064d
```

Validação do GUID:

- 32 caracteres hexadecimais;
- ocorrência única em `Assets/`;
- `git diff --cached --check` sem saída antes do commit;
- commit `f5e854b`.

Decisão formal do Product Owner:

```text
Autorizo encerrar a Fase 4 com o escopo atual.
```

Consequência:

- nenhuma fatia adicional de integração Match Core/perfis foi exigida para estender a Fase 4;
- essa integração ocorrerá quando a fase de ações/orquestração realmente precisar dela;
- OPEN-002 e OPEN-003 permaneceram intocadas.

Fechamento do PR #10:

- head final antes do merge: `bf0ce52196186f31145d60f7d9cd81c8600b8f44`;
- `Governance`: success;
- `Unity Structure`: success;
- PR marcado Ready for review;
- merge explicitamente autorizado pelo Product Owner;
- squash merge concluído com sucesso;
- merge/main SHA: `68279f8b89c5ecd70d380cc83071d2c2a5dfa6cf`.

Importante: os testes Unity reais e a captura Windows foram executados antes dos commits finais exclusivamente de metadata/documentação. Não declarar que o merge SHA em si foi reexecutado em Unity.

---

## 10. PR #9 — continuidade operacional

PR #9 — `Add project continuity handoff and refresh phase status` foi mesclado em:

```text
eb62dc4f93526685e41ab22a96aa591fa465c3c8
```

Ele introduziu o documento de continuidade no repositório e atualizou os status das Fases 3 e 4 antes do gate final da Fase 4.

---

## 11. Estado atual do repositório

Após o merge do PR #10, o Product Owner executou:

```powershell
git switch main
git pull --ff-only
git log -1 --oneline
git status --short
```

Resultado confirmado:

```text
68279f8 (HEAD -> main, origin/main, origin/HEAD) Implement Phase 4 Windows performance gate (#10)
working tree: limpa
```

Depois foi criada a branch documental:

```text
agent/post-phase4-continuity
```

a partir exatamente de `68279f8b89c5ecd70d380cc83071d2c2a5dfa6cf`.

Situação funcional:

- Fases 0, 1, 2, 3 e 4 concluídas e integradas;
- nenhuma fatia técnica pendente da Fase 4;
- OPEN-002 e OPEN-003 continuam abertas;
- próxima fase técnica: **Fase 5 — Advanced Actions**.

---

## 12. FormationLab — arquivos centrais

```text
Assets/BFC/FormationLab/
├── BFC.FormationLab.asmdef
├── FormationLabPreviewProfiles.cs
├── FormationLabRuntimeBootstrap.cs
├── FormationLabRuntimeBuilder.cs
├── FormationLabPerformanceProbe.cs
└── FormationPieceRuntime.cs

Assets/BFC/Scenes/
└── FormationLab.unity

Assets/BFC/Tests/PlayMode/
└── FormationLabSmokeTests.cs

Assets/BFC/Editor/Build/
└── BfcBuild.cs

scripts/
└── run-formationlab-performance.ps1
```

O `FormationLab` é harness de engenharia, não tela final nem fonte normativa de regras, e permanece fora do build de produção.

---

## 13. Processo padrão de desenvolvimento

### Antes de uma nova fatia/fase

```powershell
cd F:\Projetos\BFC
git status --short
git fetch origin
git switch main
git pull --ff-only
git log -1 --oneline
```

Depois:

- confirmar HEAD;
- criar branch específica;
- abrir Draft PR cedo quando apropriado;
- documentar objetivo e não-decisões;
- não decidir OPEN fora do escopo.

### Durante implementação

- código em inglês;
- docs de produto/arquitetura em português;
- regras fora de MonoBehaviour quando possível;
- dependências explícitas;
- parâmetros centralizados;
- testes junto das regras;
- sem números mágicos duplicados;
- UI não autoritativa;
- Physics não autoritativo para competição;
- IA usa comandos válidos.

### Validação Unity

```powershell
.\scripts\run-unity-tests.ps1
```

Validador estrutural:

```powershell
python .\scripts\validate_unity_structure.py
```

### Metadata `.meta`

Para novos `.meta`:

1. deixar Unity gerar;
2. verificar GUID não vazio e único;
3. fazer stage;
4. executar `git diff --cached --check`;
5. não modificar metadata manualmente sem necessidade objetiva;
6. quando necessário, conferir problemas fora de `.meta` com:

```powershell
git diff --cached --check -- . ':(exclude)*.meta'
```

### PR pronto

Quando aplicável, exigir:

- Unity real compila;
- EditMode verde;
- PlayMode verde;
- inspeção visual para cena/apresentação;
- metadata revisada;
- Governance verde;
- Unity Structure verde;
- documentação atualizada;
- PR body com evidências;
- working tree limpa.

### Merge

**Nenhum PR deve ser mesclado sem autorização explícita do Product Owner.**

Quando autorizado:

1. revalidar PR aberto, mergeable e not draft;
2. confirmar head exato;
3. confirmar CI do head final;
4. preferir squash conforme padrão atual;
5. usar `expected_head_sha` quando disponível;
6. após merge, sincronizar `main` local;
7. atualizar este handoff com o merge SHA.

---

## 14. CI e proteção de `main`

`main` é protegida para fluxo por PR.

- PR obrigatório;
- `Governance` obrigatório;
- `Unity Structure` obrigatório;
- force push/deleção de `main` bloqueados.

Regra operacional importante:

```text
static CI green != runtime/Unity validation complete
```

O FormationLab já demonstrou esse ponto: CI estático verde não detectou erros que a execução real da Unity encontrou.

---

## 15. Caveats que não podem ser perdidos

- `PhysicsLabTuning` é provisório;
- collision detection `Discrete` no laboratório não é regra final de produção;
- não congelar 28×18 como dimensão oficial;
- não transformar a formação de preview em formação obrigatória;
- campo grande é **11 total**, não 11 linha + goleiro;
- não forçar 11 peças em todos os modos;
- não decidir OPEN-002/003 por conveniência;
- não duplicar regra de 3 ações em UI/IA;
- não deixar Physics decidir posse/score;
- não deixar Presentation decidir regra;
- não usar legado como dependência de produção;
- não redesenhar Interface e Menu por causa da Unity;
- não criar online fake;
- não declarar teste que não foi executado;
- não inventar benchmark;
- não transformar `avgFps=2359.451` em requisito de produção;
- preservar `maxMs=275.347` como outlier real da captura;
- `Shader.Find` isolado não foi suficiente para garantir material do standalone FormationLab;
- `RenderPipelineAsset.defaultMaterial` também não resolveu o dedicated player;
- FormationLab continua fora do player build de produção.

---

## 16. Próximas fases

### Fase 5 — Advanced Actions

Próximo milestone técnico.

Entregas normativas:

- chute;
- passe com força reduzida;
- spin/efeito;
- chip/bola por cima 3D;
- menu radial/contextual;
- feedback de força e direção;
- regras de elegibilidade.

Gate da fase:

- teste de regra para cada ação;
- input não contém lógica autoritativa de resultado;
- parâmetros centralizados;
- comportamento comparado ao legado.

Direção de implementação:

- preservar Modern Control (`drag/aim/release`);
- modelar `Shot`, `Pass`, `Spin` e `Chip` como ações/contratos testáveis antes de ampliar apresentação;
- preparar o **Classic Strike Model** da issue #7 antes ou durante a fase;
- manter Modern Control e Classic Simulation separados;
- não antecipar decisões de OPEN-002/003.

### Fase 6 — Goalkeeper & AI

- goleiro automático;
- percepção;
- seleção de peça;
- shot planner;
- força/direção;
- dificuldade;
- limite de decisão;
- fallback válido;
- sem deadlock.

### Fase 7 — Rules & Restarts

Depende de OPEN-002/003.

### Fases 8–15

- Modes;
- Progression/Economy/Save;
- Official Interface & Menu;
- Match Presentation;
- Audio & Feel;
- Windows Production Build;
- Optimization/RC;
- Secondary Platforms.

---

## 17. Próximo passo seguro

1. concluir esta atualização documental pós-merge em PR pequeno e isolado;
2. validar diff e CI dessa branch documental;
3. não fazer merge sem autorização explícita do Product Owner;
4. após o merge documental, sincronizar `main`;
5. iniciar formalmente a Fase 5 em nova branch a partir do `main` atualizado;
6. antes de ampliar runtime, revisar Development Plan, Gameplay Constitution, Architecture e issue #7;
7. especificar os contratos de `Shot`, `Pass`, `Spin` e `Chip` e o limite de responsabilidade entre Gameplay/Input/Physics;
8. preparar a especificação do Classic Strike Model;
9. somente depois implementar a primeira fatia técnica com testes.

---

## 18. Protocolo obrigatório de atualização deste documento

Atualizar este arquivo sempre que uma etapa/fase relevante for concluída.

Registrar:

- data/hora;
- branch/PR;
- head conhecido;
- merge SHA quando existir;
- testes realmente executados;
- CI;
- build;
- inspeções;
- metadata;
- decisões do Product Owner;
- bugs encontrados e correções;
- OPEN DECISIONS afetadas;
- caveats;
- próximo passo.

Nunca registrar como concluído algo que ainda depende de evidência futura.

---

## 19. Checklist de handoff

Antes de abandonar uma conversa longa:

- [x] contexto de produto preservado;
- [x] arquitetura preservada;
- [x] decisões LOCKED registradas;
- [x] OPEN DECISIONS registradas;
- [x] histórico das Fases 0–4 registrado;
- [x] testes mais recentes registrados;
- [x] performance Windows registrada;
- [x] defeitos standalone e solução registrados;
- [x] metadata final da Fase 4 registrada;
- [x] decisão de encerramento da Fase 4 registrada;
- [x] merge SHA do PR #10 registrado;
- [x] regra de autorização de merge registrada;
- [x] próximo passo explícito.

---

## 20. Referências principais

```text
docs/00-PRODUCT_CHARTER.md
docs/01-GAMEPLAY_CONSTITUTION.md
docs/02-VISUAL_CONSTITUTION.md
docs/03-ARCHITECTURE.md
docs/04-DEVELOPMENT_PLAN.md
docs/05-CHANGE_CONTROL.md
docs/10-CODING-STANDARDS.md
docs/OPEN_DECISIONS.md
docs/13-PHASE3_MATCH_CORE.md
docs/14-PHASE4_FORMATION_FIELD.md
docs/BFC_PROJECT_CONTINUITY.md

docs/changes/RFC-0001-phase4-team-composition.md
docs/decisions/ADR-0003-mode-specific-formation-profiles.md

governance/rules.json
scripts/validate_unity_structure.py
scripts/run-unity-tests.ps1
scripts/run-formationlab-performance.ps1
scripts/build-windows.ps1
```

Issue relevante:

```text
#7 — Specify BFC Classic Simulation mode
```

PRs/merges principais:

```text
#1 Foundation & Governance  -> ae3a87236bf1b0742bb98e131fbd4729909c2724
#3 Phase 1 Unity Bootstrap -> 408a4b1f281b025d991c76303c83f63d6e514672
#4 Phase 2 Physics          -> 43c3f0961805ce177a770d73d56e337fe9b469ae
#5 Phase 3 Match Core       -> c6d636a2ce93461b8034c17db6a01c8518e65ea4
#6 Phase 4 Domain           -> 6b80ee5bfe65d9fb183c9f05db175b864db005e9
#8 Phase 4 Runtime          -> 5ab0db9583aad4c7695dea1aeeb8863208dcfec1
#9 Continuity handoff       -> eb62dc4f93526685e41ab22a96aa591fa465c3c8
#10 Phase 4 final gate      -> 68279f8b89c5ecd70d380cc83071d2c2a5dfa6cf
```

---

## 21. Resumo de 30 segundos

```text
Projeto: BFC em Unity 6.3 LTS / 6000.3.21f1.
Referência funcional: BFC legado.
Referência visual: Interface e Menu.
Windows primeiro.
Fases 0, 1, 2, 3 e 4 concluídas e integradas.
main confirmado = 68279f8b89c5ecd70d380cc83071d2c2a5dfa6cf.
PR #10 foi mesclado e encerrou formalmente a Fase 4.
Campo grande = 11 total por time = 10 linha + 1 goleiro.
FormationLab: EditMode 21/21; PlayMode 2/2; inspeção visual aprovada.
Windows performance: 23595 samples, avg 0.424ms, p95 0.527ms, p99 1.012ms, max 275.347ms.
Esses números são evidência do harness, não requisito de produção.
OPEN-002/003 continuam abertas.
Classic Simulation continua registrado na issue #7.
Modern Control e Classic Simulation não devem ser fundidos silenciosamente.
Próximo milestone: Fase 5 — Advanced Actions.
Primeiro passo técnico: especificar contratos de Shot, Pass, Spin e Chip e preparar o Classic Strike Model.
Nenhum PR deve ser mesclado sem autorização explícita do Product Owner.
```
