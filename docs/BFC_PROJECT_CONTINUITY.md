# BFC — Documento Mestre de Continuidade, Status e Handoff

**Versão:** 1.1  
**Última atualização operacional:** 2026-08-14 22:51 (BRT / UTC-03:00)  
**Repositório:** `AiltonSantanaReis/BFC`  
**Workspace local principal:** `F:\Projetos\BFC`  
**Branch canônica:** `main`  
**HEAD conhecido de `main`:** `eb62dc4f93526685e41ab22a96aa591fa465c3c8` — PR #9  
**Branch de trabalho atual:** `agent/phase4-final-gate`  
**PR atual:** #10 — `Implement Phase 4 Windows performance gate`  
**Estado do PR #10:** Draft, aberto, não mesclado; merge exige autorização explícita do Product Owner.  
**Estado da Fase 4:** escopo e validação real **CONCLUÍDOS**; encerramento explicitamente autorizado pelo Product Owner; resta o fechamento administrativo do PR #10 (CI final, Ready for review e merge autorizado).  
**HEAD local informado pelo Product Owner antes das atualizações documentais remotas finais:** `f5e854b` com working tree limpa.  
**HEAD remoto imediatamente antes desta atualização de continuidade:** `3a10c720af26a53e7e1fd75624cec76a04a1f863`.

> Este é o handoff operacional oficial do BFC. Ele existe para permitir retomar o projeto em uma nova conversa sem depender do histórico anterior.
>
> Ele **não substitui documentos normativos**. Em caso de conflito, prevalecem as decisões explícitas do Product Owner registradas no repositório, `governance/rules.json`, Product Charter, Gameplay Constitution, Visual Constitution, ADRs/RFCs e o Development Plan.

---

## 1. Retomada rápida em uma nova conversa

Use este prompt:

```text
Estamos continuando o projeto BFC no repositório AiltonSantanaReis/BFC.
Use docs/BFC_PROJECT_CONTINUITY.md como handoff operacional.
Antes de alterar código, confira no GitHub o HEAD atual de main, o PR #10 (se ainda existir),
issues relevantes e os documentos normativos.
Não mude regras LOCKED por conveniência técnica.
OPEN-002 e OPEN-003 permanecem abertas.
Não faça merge de PR sem minha autorização explícita.
Continue a partir da seção "Próximo passo seguro" do documento de continuidade.
```

Ao retomar:

1. verificar `main` e PRs no GitHub;
2. comparar com este documento;
3. se houver divergência, atualizar primeiro o handoff com a realidade do repositório;
4. não reconstruir decisões críticas por suposição;
5. nunca inferir autorização de merge de uma autorização de escopo/fase.

---

## 2. Identidade e direção oficial do produto

O produto oficial chama-se **BFC**.

BFC está sendo reconstruído do zero em **Unity**, usando materiais anteriores como referência funcional e visual, sem transportar a arquitetura legada como dependência de produção.

### Referência funcional

`BFC(1).zip` / BFC legado é referência para preservar intenção de:

- drag/aim/release;
- potência;
- colisões e sensação física;
- chute, passe, spin e chip;
- regra de até 3 ações por posse onde o ruleset exigir;
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

Bugs, monólitos, acoplamentos, física dependente de FPS, pseudo-Z, IA fraca, online fake, números antigos e armazenamento legado **não são requisitos**.

### Referência visual

`Interface e Menu.zip` é a referência visual oficial para preservar:

- identidade cyber/neon;
- cyan e magenta;
- composição;
- layout;
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

## 4. Baseline técnico fixado

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

Regras arquiteturais:

- `BFC.Core` independente de `UnityEngine`;
- regras testáveis sem UI;
- lógica central não depende de GameObject/cena;
- física usa fixed-step;
- regra competitiva não depende de FPS;
- Bootstrap é composition root;
- evitar ECS/DOTS sem evidência de necessidade;
- Physics não decide posse/score/recompensa;
- Presentation não decide regra;
- IA não contorna validação de regras.

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

---

## 5. Regras de gameplay que já devem ser preservadas

### Núcleo

- futebol de botão por ações;
- controle moderno: selecionar + drag/aim + força + release;
- resolução física antes da próxima ação;
- até **3 ações por posse** quando o ruleset usar essa regra;
- `ação` não é automaticamente `toque na bola`;
- P1 vs COM;
- P1 vs P2.

### Ações previstas

- chute;
- passe com força reduzida em relação ao chute;
- spin/efeito;
- chip/bola por cima 3D;
- menu radial/contextual;
- feedback de força/direção.

### Goleiro

- goleiro automático no competitivo oficial;
- reação, alcance e velocidade são tuning;
- controle manual não pode aparecer silenciosamente em ruleset que define goleiro automático.

### Competitivo/progressão

- Liga Justa;
- Campeonato;
- sem pay-to-win;
- derrota ainda pode gerar progressão/recompensa;
- perfil persistente;
- missões;
- conquistas;
- troféus/vitrine;
- loja com estados locked / available / owned / equipped.

---

## 6. OPEN-001 — resolvida e LOCKED

Decisão do Product Owner em 2026-08-14:

- BFC **não possui uma contagem universal de peças para todos os modos**;
- perfil de campo grande = **11 peças totais por equipe**;
- composição do campo grande = **10 jogadores de linha + 1 goleiro**;
- o goleiro está incluído nas 11;
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

- tamanho final do campo grande;
- formação tática única;
- números exatos de todos os perfis legados;
- regras de saída/reposição;
- faltas/pênaltis;
- tuning físico final.

---

## 7. Decisões ainda abertas

### OPEN-002 — bola fora e reposições

Ainda deve definir:

- quais rulesets permitem saída;
- transferência de posse;
- lateral/escanteio ou ausência deles;
- tiro de meta;
- reposicionamento.

### OPEN-003 — faltas, vantagem e pênaltis

Ainda deve definir:

- o que é falta;
- vantagem;
- pênalti;
- posicionamento de cobrança;
- elegibilidade;
- impacto em posse/turno.

### OPEN-004 — baseline numérico competitivo

Ainda é tuning/benchmark:

- duração;
- limite de gols;
- rating;
- entradas;
- recompensas e demais números competitivos.

### OPEN-005 — tecnologia final da UI

Ainda requer spike de fidelidade entre:

- UI Toolkit;
- uGUI;
- combinação controlada.

A escolha técnica não pode obrigar redesign da Interface e Menu.

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
- bola resulta da colisão física, não de trajetória diretamente escolhida.

Goleiro no Classic:

- poderá ser reposicionado apenas em janelas explícitas autorizadas por ruleset;
- não existe autorização para movimento manual contínuo;
- situações exatas dependem das futuras regras de restart/falta/pênalti.

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

Timing:

- especificação do **Classic Strike Model** deve ser preparada antes ou durante a Fase 5;
- não antecipar OPEN-002/003 para implementá-lo.

---

## 9. Histórico de fases e merges

### Fase 0 — Foundation & Governance

**Status:** concluída e mesclada.

- PR #1 — `Establish BFC Unity foundation and governance`;
- merge SHA: `ae3a87236bf1b0742bb98e131fbd4729909c2724`;
- documentos normativos;
- governança;
- arquitetura;
- versão Unity fixada;
- PR template;
- CODEOWNERS;
- CI de governança.

PR #2 foi noop/acidental e foi fechado sem efeito de produto.

### Fase 1 — Unity Bootstrap

**Status:** concluída e mesclada.

- PR #3 — `Implement Phase 1 Unity bootstrap`;
- merge SHA: `408a4b1f281b025d991c76303c83f63d6e514672`.

Validação real:

- Unity abre/compila;
- URP/Input System ativos;
- EditMode **2/2**;
- PlayMode **1/1**;
- Windows x64 build funcional;
- Governance verde;
- Unity Structure verde.

### Fase 2 — Physics Vertical Slice

**Status:** concluída e mesclada.

- PR #4 — `Implement Phase 2 physics vertical slice`;
- merge SHA: `43c3f0961805ce177a770d73d56e337fe9b469ae`.

Incluiu:

- drag/release;
- potência;
- peça→bola;
- peça→peça;
- peça→parede;
- desaceleração;
- repouso;
- parâmetros centralizados;
- fixed-step.

Validação:

- EditMode total **5/5**;
- PlayMode **1/1**;
- CI final verde.

Caveats:

- `PhysicsLabTuning` ainda provisório;
- movimento principal planar XZ;
- collision detection do laboratório estabilizado em **Discrete**, não decisão final de produção;
- não inventar benchmark numérico não capturado.

### Fase 3 — Match Core

**Status:** concluída e mesclada.

- PR #5 — `Implement Phase 3 match core`;
- merge SHA: `c6d636a2ce93461b8034c17db6a01c8518e65ea4`.

Implementação:

- `MatchState`;
- fases;
- posse;
- `PlayerActionCommand`;
- validação;
- `ResolvingAction`;
- espera por resolução física;
- contador/limite de ações vindo do ruleset;
- score;
- gol;
- `AwaitingRestart`;
- `ResumeAfterRestart(nextPossession)`;
- relógio lógico;
- eventos;
- fim explícito.

Validação:

- 8 testes próprios;
- EditMode total **13/13**;
- PlayMode **1/1**;
- metadata revisada;
- CI verde.

Nenhuma decisão de OPEN-002/003 foi tomada.

### Fase 4 — Formation & Field

**Status de produto/engenharia:** **CONCLUÍDA** com autorização explícita do Product Owner em 2026-08-14.  
**Status de integração em `main`:** PR #10 ainda precisa concluir CI final e ser mesclado com autorização separada.

#### Fatia 1 — domínio

- PR #6 — `[GOVERNANCE CHANGE] Implement Phase 4 formation and field`;
- merge SHA: `6b80ee5bfe65d9fb183c9f05db175b864db005e9`.

Incluiu:

- resolução formal de OPEN-001;
- `PieceRole`;
- `TeamCompositionDefinition`;
- `LargeFieldEleven`;
- `FormationSlot`;
- `FormationDefinition`;
- `FieldDefinition`;
- `FormationSpawn`;
- `FormationSpawnPlanner`;
- RFC-0001;
- ADR-0003.

Validação:

- EditMode **21/21**;
- PlayMode **1/1**;
- 10 `.meta` revisados;
- Governance/Unity Structure verdes.

#### Fatia 2 — runtime materialization

- PR #8 — `Implement Phase 4 runtime field materialization`;
- merge SHA: `5ab0db9583aad4c7695dea1aeeb8863208dcfec1`.

`FormationLab` materializa:

- campo;
- linhas;
- áreas;
- gols;
- volumes de gol;
- bola;
- Team A;
- Team B;
- 22 peças;
- 2 goleiros;
- 20 outfield;
- papel explícito via `FormationPieceRuntime`;
- spawns via `FormationSpawnPlanner`;
- `PlanarKineticBody` em peças e bola.

Preview:

- 10 outfield + 1 GK por equipe;
- campo de preview 28×18;
- layout e dimensões **não normativos**.

Correção arquitetural:

- root da peça em escala 1;
- collider/Rigidbody no root;
- cilindro visual como filho escalado.

Primeira execução real encontrou:

- `AudioListener` desnecessário;
- uso incorreto de `FormationSpawn.SlotId` em vez de `PieceId`.

Correção: commit `6ab8355`.

Validação final da fatia 2:

- EditMode **21/21**;
- PlayMode **2/2**;
- inspeção visual aprovada;
- 11 peças de cada lado;
- goleiros, bola, áreas e gols visíveis;
- sem erro/warning visível na evidência do FormationLab;
- 6 `.meta` revisados;
- CI final verde.

`FormationLab` permanece fora do build de produção. `Bootstrap` é a entrada de produção.

#### Fatia 3 — Windows performance gate / PR #10

Branch:

```text
agent/phase4-final-gate
```

PR:

```text
#10 — Implement Phase 4 Windows performance gate
```

Objetivo:

- evidência objetiva de estabilidade/performance no target Windows;
- sem inventar threshold de FPS;
- sem alterar OPEN-002/003;
- sem colocar FormationLab no build de produção.

Implementado:

- build Windows x64 dedicado do FormationLab;
- `FormationLabPerformanceProbe` opt-in por `-bfcFormationPerf`;
- VSync desativado somente no diagnóstico;
- warmup configurável;
- amostragem por tempo;
- métricas `avg`, `median`, `p95`, `p99`, `max`, `avgFps`;
- timeout no PowerShell;
- exigência de linha concreta `[BFC FormationPerf] RESULT`;
- material URP Lit temporário em `Resources` durante o build diagnóstico;
- cleanup automático do asset temporário após o build.

##### Tentativa standalone 1

Falhou antes da medição:

```text
InvalidOperationException: FormationLab could not resolve a runtime material shader.
```

Causa:

- `Shader.Find("Universal Render Pipeline/Lit")` / fallback `Standard` não garantia shader no player após stripping.

##### Tentativa standalone 2

Após primeira correção, Unity continuou:

- EditMode **21/21**;
- PlayMode **2/2**.

Standalone falhou antes do probe:

```text
InvalidOperationException: FormationLab could not resolve the active render pipeline default material.
```

Conclusão:

- o `defaultMaterial` do pipeline não era fonte runtime confiável para esse player dedicado.

##### Correção final do material do harness

O build diagnóstico passou a:

1. criar Material URP Lit temporário;
2. colocá-lo sob build-only `Resources`;
3. deixar Unity incluir material + shader concretamente;
4. carregar esse material no FormationLab standalone;
5. remover os assets temporários depois do build.

Resultado de cleanup:

```text
Test-Path Assets/BFC/Settings/FormationLabBuildAssets
False
```

##### Captura real bem-sucedida

Ambiente reportado pelo probe:

- Unity **6000.3.21f1**;
- CPU **AMD Ryzen 7 5700X3D 8-Core Processor**;
- GPU **NVIDIA GeForce RTX 3070 Ti**;
- resolução **1280×720**;
- warmup **2 s**;
- sample window **10 s**;
- VSync desligado somente para diagnóstico.

Resultado completo:

```text
[BFC FormationPerf] RESULT samples=23595 durationSeconds=10 avgMs=0.424 medianMs=0.383 p95Ms=0.527 p99Ms=1.012 maxMs=275.347 avgFps=2359.451 resolution=1280x720
```

Interpretação:

- captura completou normalmente;
- distribuição central/p95/p99 sustentam estabilidade do **preview atual** no target Windows;
- `maxMs=275.347` permanece registrado como outlier real;
- `avgFps=2359.451` **não é requisito de produção**;
- esse harness ainda não representa UI final, IA, VFX, áudio final e demais sistemas futuros.

Validação Unity na mesma linha de trabalho:

- EditMode **21/21**;
- PlayMode **2/2**.

##### Metadata final da Fase 4

Arquivo:

```text
Assets/BFC/FormationLab/FormationLabPerformanceProbe.cs.meta
```

Conteúdo:

```text
fileFormatVersion: 2
guid: b9bed3d5a30ce2249be94c1bc153064d
```

Validação:

- GUID com 32 hex;
- ocorrência única em `Assets/`;
- `git diff --cached --check` sem saída;
- commit: `f5e854b` — `materialize FormationLab performance probe metadata`;
- push concluído;
- `git status --short` local vazio após push.

##### Decisão formal de fechamento

Em 2026-08-14, o Product Owner declarou:

```text
Autorizo encerrar a Fase 4 com o escopo atual.
```

Consequência:

- não haverá fatia adicional de integração Match Core/perfis apenas para estender a Fase 4;
- o Development Plan não exige essa integração como gate da Fase 4;
- integração será feita quando as fases de ações/orquestração realmente precisarem dela;
- Fase 4 está concluída em escopo e validação real;
- merge do PR #10 continua exigindo autorização separada.

---

## 10. PR #9 — continuidade operacional

PR #9 — `Add project continuity handoff and refresh phase status` foi mesclado.

Merge SHA:

```text
eb62dc4f93526685e41ab22a96aa591fa465c3c8
```

Ele introduziu este documento no repositório e corrigiu status defasados das Fases 3 e 4.

Após o merge, o Product Owner sincronizou localmente:

```text
main = eb62dc4
origin/main = eb62dc4
working tree = limpa
```

---

## 11. Estado atual do repositório nesta atualização

### `main`

```text
HEAD conhecido: eb62dc4f93526685e41ab22a96aa591fa465c3c8
```

O PR #10 ainda não foi mesclado, portanto o fechamento da Fase 4 ainda não está integrado em `main`.

### Branch do PR #10

Antes das atualizações documentais finais remotas:

```text
commit de metadata: f5e854b
working tree local do Product Owner: limpa
```

Depois do push da metadata, a documentação da Fase 4 foi atualizada remotamente, avançando a branch para:

```text
3a10c720af26a53e7e1fd75624cec76a04a1f863
```

Esta atualização do próprio handoff avançará novamente o head. Portanto, **sempre verificar o head atual do PR #10 no GitHub antes de qualquer merge**; não usar `f5e854b` ou `3a10c720` como expected head depois desta atualização.

### Situação local do Product Owner

A última evidência local antes das atualizações remotas documentais:

```text
branch: agent/phase4-final-gate
HEAD: f5e854b
working tree: limpa
```

Será necessário `git pull --ff-only` para receber as atualizações documentais finais da branch.

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

O FormationLab:

- é harness de engenharia;
- prova materialização de domínio em Unity;
- não é tela final;
- não é fonte normativa de regras;
- fica fora do build de produção.

---

## 13. Processo padrão de desenvolvimento

### Antes de uma fatia/fase

```powershell
cd F:\Projetos\BFC
git status --short
git fetch origin
git switch main
git pull --ff-only
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
2. verificar GUID;
3. exigir GUID não vazio e único;
4. fazer stage;
5. executar `git diff --cached --check`;
6. trailing whitespace padrão do Unity em `.meta` não deve ser editado manualmente apenas para agradar o check;
7. quando necessário, conferir problemas fora de `.meta` com:

```powershell
git diff --cached --check -- . ':(exclude)*.meta'
```

### PR pronto

Quando aplicável, exigir:

- Unity compila de verdade;
- EditMode verde;
- PlayMode verde;
- inspeção visual para cena/apresentação;
- metadata revisada;
- Governance verde;
- Unity Structure verde;
- documentação atualizada;
- PR body com evidência;
- working tree local limpa.

### Merge

**Nenhum PR deve ser mesclado sem autorização explícita do Product Owner.**

Quando autorizado:

1. revalidar PR aberto/mergeable/not draft;
2. confirmar head exato;
3. confirmar CI do head final;
4. usar squash conforme padrão atual;
5. usar expected head SHA quando possível;
6. após merge, sincronizar `main` local;
7. registrar merge SHA neste handoff.

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

Já ocorreu no FormationLab: CI estático verde não detectou erros que Unity real encontrou.

---

## 15. Caveats e aprendizados que não podem ser perdidos

### PhysicsLab

- `PhysicsLabTuning` é provisório;
- collision detection Discrete não é regra de produção congelada;
- não inventar benchmark que não foi capturado.

### FormationLab runtime

Problemas reais já corrigidos:

- `AudioListener` desnecessário;
- `FormationSpawn.SlotId` inexistente; usar `PieceId`;
- root escalado para peça deve ser evitado; visual filho pode receber escala.

### Standalone Windows

Problemas player-only encontrados:

1. `Shader.Find` não garantia shader/material após stripping;
2. `RenderPipelineAsset.defaultMaterial` não foi suficiente no dedicated player;
3. solução final do harness: referência concreta temporária em `Resources` durante build diagnóstico + cleanup.

### Build Settings

Unity 6000.3.21f1 serializou:

```text
m_UseUCBPForAssetBundles: 0
```

FormationLab permanece fora do player build de produção.

### Performance

Não transformar:

```text
avgFps=2359.451
```

em meta ou requisito de produto. É resultado de harness leve, uncapped, 1280×720, sem sistemas finais.

Preservar também o outlier:

```text
maxMs=275.347
```

sem apagá-lo ou reinterpretá-lo como comportamento típico.

---

## 16. O que NÃO fazer

- não decidir OPEN-002/003 por conveniência;
- não congelar 28×18 como campo oficial;
- não transformar formação de preview em formação obrigatória;
- não interpretar campo grande como 11 linha + goleiro: é **11 total**;
- não forçar 11 peças em todos os modos;
- não duplicar regra de 3 ações em UI/IA;
- não deixar Physics decidir posse/score;
- não deixar Presentation decidir regra;
- não usar legado como dependência de produção;
- não redesenhar Interface e Menu por causa da Unity;
- não criar online fake;
- não declarar teste que não foi executado;
- não inventar benchmarks;
- não transformar números do performance harness em requisito de produção;
- não fazer merge sem autorização explícita.

---

## 17. Próximas fases

### Fase 5 — Advanced Actions

Próxima fase de implementação após o merge/fechamento administrativo da Fase 4.

Entregas normativas:

- chute;
- passe com força reduzida;
- spin/efeito;
- chip/bola por cima 3D;
- menu radial/contextual;
- feedback de força e direção;
- regras de elegibilidade.

Gate:

- teste de regra para cada ação;
- input não contém lógica de resultado;
- parâmetros centralizados;
- comportamento comparado ao legado.

Antes/durante Fase 5:

- especificar **Classic Strike Model** da issue #7;
- manter Modern Control e Classic Simulation separados;
- não decidir OPEN-002/003 silenciosamente.

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

## 18. Próximo passo seguro

No momento desta atualização:

1. confirmar que o PR #10 está no novo head documental final;
2. aguardar/confirmar `Governance` e `Unity Structure` verdes nesse head;
3. se ambos verdes, marcar PR #10 como **Ready for review**;
4. **não fazer merge ainda**;
5. pedir/aguardar autorização explícita do Product Owner para merge;
6. quando autorizada, revalidar head e CI e fazer squash merge;
7. no PC do Product Owner executar:

```powershell
git switch main
git pull --ff-only
git log -1 --oneline
git status --short
```

8. registrar o merge SHA da Fase 4 neste handoff na próxima atualização;
9. iniciar a preparação da Fase 5 pela especificação/arquitetura das Advanced Actions e Classic Strike Model.

---

## 19. Protocolo obrigatório de atualização deste documento

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

## 20. Checklist de handoff

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
- [x] regra de autorização de merge registrada;
- [x] próximo passo explícito;
- [ ] merge SHA do PR #10 — ainda não existe neste momento.

---

## 21. Referências principais

```text
docs/00-PRODUCT_CHARTER.md
docs/01-GAMEPLAY_CONSTITUTION.md
docs/02-VISUAL_CONSTITUTION.md
docs/03-ARCHITECTURE.md
docs/04-DEVELOPMENT_PLAN.md
docs/05-CHANGE_CONTROL.md
docs/10-CODING_STANDARDS.md
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

Issue:

```text
#7 — Specify BFC Classic Simulation mode
```

PRs/merges conhecidos:

```text
#1 Foundation & Governance  -> ae3a87236bf1b0742bb98e131fbd4729909c2724
#3 Phase 1 Unity Bootstrap -> 408a4b1f281b025d991c76303c83f63d6e514672
#4 Phase 2 Physics          -> 43c3f0961805ce177a770d73d56e337fe9b469ae
#5 Phase 3 Match Core       -> c6d636a2ce93461b8034c17db6a01c8518e65ea4
#6 Phase 4 Domain           -> 6b80ee5bfe65d9fb183c9f05db175b864db005e9
#8 Phase 4 Runtime          -> 5ab0db9583aad4c7695dea1aeeb8863208dcfec1
#9 Continuity handoff       -> eb62dc4f93526685e41ab22a96aa591fa465c3c8
#10 Phase 4 final gate      -> OPEN / NOT MERGED
```

---

## 22. Resumo de 30 segundos

```text
Projeto: BFC em Unity 6.3 LTS / 6000.3.21f1.
Referência funcional: BFC legado.
Referência visual: Interface e Menu.
Windows primeiro.
Fases 0, 1, 2 e 3 concluídas e mescladas.
Fase 4: domínio + runtime mesclados; performance gate validado no PR #10.
Product Owner autorizou encerrar a Fase 4 com o escopo atual.
Campo grande = 11 total por time = 10 linha + 1 goleiro.
FormationLab: EditMode 21/21; PlayMode 2/2; inspeção visual aprovada.
Windows performance: 23595 samples, avg 0.424ms, p95 0.527ms, p99 1.012ms, max 275.347ms.
Performance numbers são evidência de harness, não requisito de produção.
OPEN-002/003 continuam abertas.
Classic Simulation está na issue #7 e precisa de Classic Strike Model antes/durante a Fase 5.
PR #10 ainda não foi mesclado.
Nenhum merge sem autorização explícita do Product Owner.
Próximo: CI final do head documental -> Ready for review -> pedir autorização de merge.
```
