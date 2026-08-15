# BFC — Documento Mestre de Continuidade, Status e Handoff

**Versão deste documento:** 1.0  
**Última atualização:** 2026-08-14 20:59 (BRT / UTC-03:00)  
**Repositório:** `AiltonSantanaReis/BFC`  
**Workspace local principal:** `F:\Projetos\BFC`  
**Branch canônica:** `main`  
**HEAD de `main` no momento desta atualização:** `5ab0db9583aad4c7695dea1aeeb8863208dcfec1`  
**Último merge:** PR #8 — `Implement Phase 4 runtime field materialization`  
**Estado local informado pelo Product Owner:** `main` sincronizada com `origin/main` e `git status --short` vazio.

> Este é um documento operacional de continuidade. Ele deve permitir retomar o projeto em uma nova conversa sem depender do histórico da conversa anterior.
>
> Ele **não substitui documentos normativos**. Em caso de conflito, prevalece a ordem de autoridade definida em `docs/00-PRODUCT_CHARTER.md` e nos arquivos de governança do repositório.

---

## 1. Como usar este documento em uma nova conversa

Ao iniciar uma nova conversa sobre o BFC:

1. forneça este documento ao assistente;
2. informe que o repositório oficial é `AiltonSantanaReis/BFC`;
3. peça para o assistente verificar o estado atual de `main`, PRs e issues antes de fazer qualquer alteração;
4. trate este documento como **handoff operacional**, não como substituto de `governance/rules.json`, Product Charter, Gameplay Constitution, Visual Constitution, ADRs e RFCs;
5. se houver divergência entre este documento e o GitHub atual, atualizar primeiro este documento com a realidade do repositório;
6. nunca reconstruir contexto crítico por suposição se ele puder ser conferido no repositório.

### Prompt curto para retomar em uma nova conversa

```text
Estamos continuando o projeto BFC no repositório AiltonSantanaReis/BFC.
Use o arquivo docs/BFC_PROJECT_CONTINUITY.md como handoff operacional.
Antes de alterar código, confira no GitHub o HEAD atual de main, PRs abertos, issues relevantes
e os documentos normativos. Não mude regras LOCKED por conveniência técnica.
Não faça merge de PR sem minha autorização explícita.
Continue a partir da seção "Próximo passo recomendado" do documento de continuidade.
```

---

## 2. Identidade do produto e direção oficial

O produto oficial chama-se **BFC**.

BFC está sendo reconstruído do zero em **Unity**, usando os materiais anteriores como referência, não como dependência de runtime.

### Referências canônicas

**BFC legado / `BFC(1).zip` — referência funcional**

Usar para preservar intenção de:

- mecânicas;
- regras e fluxo de partida;
- física percebida;
- IA;
- goleiro;
- modos;
- progressão;
- economia;
- save;
- desafios;
- missões;
- conquistas.

Bugs, acoplamentos e limitações do legado **não são requisitos**.

**`Interface e Menu.zip` — referência visual oficial**

Usar para preservar:

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

### Missão de produto

BFC deve combinar:

- futebol de botão digital;
- física legível;
- decisões táticas;
- domínio de força/direção;
- passe, spin e jogadas especiais;
- IA para jogo solo;
- P1 vs COM e P1 vs P2;
- modos casual e competitivo;
- Liga Justa e Campeonato;
- Treino e Desafios;
- progressão, missões, conquistas e troféus;
- loja e cosméticos;
- monetização sem pay-to-win.

---

## 3. Ordem de autoridade

Quando existir conflito, seguir esta ordem:

1. decisão explícita mais recente do Product Owner registrada no repositório;
2. `governance/rules.json`;
3. `docs/00-PRODUCT_CHARTER.md`;
4. `docs/01-GAMEPLAY_CONSTITUTION.md`;
5. `docs/02-VISUAL_CONSTITUTION.md`;
6. ADRs aceitos;
7. `docs/04-DEVELOPMENT_PLAN.md`;
8. comportamento das referências legadas;
9. implementação atual.

**Código nunca tem autoridade superior à regra aprovada.**

---

## 4. Baseline técnico fixado

### Engine e pacotes

- Unity: **6.3 LTS**
- Editor exato: **6000.3.21f1**
- Render pipeline: **URP 17.3.0**
- Input System: **1.20.0**
- Unity Test Framework: **1.6.0**
- Plataforma inicial de produção: **Windows x64**
- Projeto: C#

### Regras técnicas importantes

- `BFC.Core` deve permanecer sem dependência de `UnityEngine`;
- lógica de regra deve ser testável sem UI;
- lógica central não deve depender de nomes de GameObject ou cena;
- física usa passo fixo;
- regra competitiva não pode depender de FPS;
- Bootstrap é o composition root;
- não adotar ECS/DOTS ou arquitetura mais complexa sem necessidade mensurável;
- multiplayer real futuro exigirá autoridade de backend/server; o projeto atual não deve fingir online real.

### Ambiente local utilizado nas validações

- Windows 11;
- repositório em `F:\Projetos\BFC`;
- PowerShell;
- Git;
- Unity Editor 6000.3.21f1.

---

## 5. Arquitetura aprovada

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

### Responsabilidades

**BFC.Core**

- entidades;
- value objects;
- identificadores;
- contratos;
- eventos;
- definições de domínio;
- estado lógico independente da engine.

**BFC.Gameplay**

- lifecycle da partida;
- turno;
- posse;
- comandos;
- validação;
- score;
- relógio;
- transições;
- reinícios quando ruleset estiver definido.

**BFC.Physics**

- corpos físicos;
- impulso;
- colisão;
- desaceleração;
- condição de repouso;
- spin;
- bola aérea;
- bridge com PhysX.

A física **não decide** vencedor, posse oficial, recompensa ou regra de modo.

**BFC.AI**

- percepção;
- avaliação;
- seleção de peça;
- planejamento;
- direção/força;
- dificuldade.

A IA deve produzir os mesmos tipos de comandos válidos disponíveis ao jogador.

**BFC.Modes**

Composição de rulesets:

- P1 vs COM;
- P1 vs P2;
- Liga Justa;
- Campeonato;
- Treino;
- Desafios;
- futuros perfis Classic Simulation.

**BFC.Progression**

- perfil;
- inventário;
- moedas;
- missões;
- conquistas;
- troféus;
- recompensas;
- vitrine;
- ownership/equipamento.

**BFC.Presentation**

- Interface e Menu;
- HUD;
- navegação;
- câmeras;
- VFX;
- feedback.

A UI observa estado e emite comandos/intents; não possui regra autoritativa.

**BFC.Infrastructure**

- save;
- armazenamento;
- plataforma;
- serviços externos futuros;
- telemetria/rede futuras.

**BFC.Bootstrap**

Único composition root autorizado.

---

## 6. Regras de gameplay que já precisam ser preservadas

### Núcleo

- futebol de botão por ações;
- seleção + drag/aim + força + release no controle moderno;
- resolução física antes de nova ação;
- competitivo oficial: até **3 ações por posse**, quando o ruleset usar essa regra;
- `ação` não é automaticamente igual a `toque na bola`;
- P1 vs COM;
- P1 vs P2.

### Ações

- chute;
- passe com força máxima reduzida em relação ao chute;
- spin/efeito;
- chip/bola por cima 3D;
- menu radial/contextual.

### Goleiro

- goleiro automático em competitivo oficial;
- reação/velocidade/alcance são tuning, não mudança de regra;
- controle manual de goleiro não pode surgir silenciosamente em ruleset competitivo onde a regra é automática.

### Física

- massa, restituição e atrito significativos;
- repouso deve ser alcançável;
- física independente de FPS;
- no competitivo, cosmético não altera hitbox, massa, atrito, potência, spin ou qualquer vantagem.

### Competitivo e progressão

- Liga Justa;
- Campeonato;
- recompensas também podem existir em derrota;
- sem pay-to-win competitivo;
- perfil persistente;
- missões;
- troféus/vitrine;
- loja com estados locked / available / owned / equipped.

---

## 7. OPEN-001 — decisão resolvida e bloqueada

Em 2026-08-14 o Product Owner resolveu a composição de equipe.

### Decisão

BFC **não possui uma contagem universal de peças para todos os modos**.

O perfil oficial de **campo grande** possui:

- **11 peças no total por equipe**;
- **10 jogadores de linha**;
- **1 goleiro**;
- o goleiro está incluído nas 11 peças.

Além disso:

- modos derivados do legado preservam seus próprios padrões de campo/composição por configuração;
- Treino e Desafios podem variar campo, quantidade e formação por cenário;
- lógica central não deve possuir número mágico global de peças.

### Registros

- `OPEN-001` em `governance/rules.json`: `locked`;
- `docs/changes/RFC-0001-phase4-team-composition.md`;
- `docs/decisions/ADR-0003-mode-specific-formation-profiles.md`;
- `docs/OPEN_DECISIONS.md`;
- `docs/01-GAMEPLAY_CONSTITUTION.md`.

### O que NÃO foi congelado

- dimensão final do campo grande;
- formação tática única obrigatória;
- números específicos de todos os perfis legados;
- regras de bola fora/reposição;
- faltas/pênaltis;
- tuning físico final.

---

## 8. Decisões ainda abertas

### OPEN-002 — bola fora e reposições

Ainda precisa definir:

- quais rulesets permitem saída;
- transferência de posse;
- lateral/escanteio ou ausência deles;
- tiro de meta;
- reposicionamento.

Bloqueia a definição final da Fase 7.

### OPEN-003 — faltas, vantagem e pênaltis

Ainda precisa definir:

- o que constitui falta;
- vantagem;
- pênalti;
- posicionamento de cobrança;
- elegibilidade;
- impacto em posse/turno.

### OPEN-004 — baseline numérico competitivo

Duração, limite de gols, rating, entradas, recompensas etc. ainda são benchmark/tuning.

### OPEN-005 — tecnologia final de UI

Ainda deve haver spike de fidelidade para decidir entre:

- UI Toolkit;
- uGUI;
- combinação controlada.

A ferramenta escolhida não pode obrigar redesign da Interface e Menu.

---

## 9. BFC Classic Simulation — direção de produto registrada

Issue oficial: **#7 — `Specify BFC Classic Simulation mode`**.

Esse modo **não substitui** o controle moderno.

### Princípio central

No Classic Simulation o jogador controla **como o golpe é aplicado à peça**, não a trajetória final da bola.

Conceitualmente:

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

### Resultado desejado

- ponto de impacto importa;
- ângulo importa;
- força importa;
- erro emerge principalmente da execução física, não de RNG artificial;
- a bola não é diretamente “controlada” depois da execução;
- o modo deve reduzir a sensação de “bilhar” e se aproximar da lógica física de futebol de botão.

### Goleiro no Classic

O goleiro poderá ser reposicionado em **janelas explícitas de posicionamento**, abertas apenas quando um ruleset autorizar.

Futuro conceito arquitetural:

```text
GoalkeeperPlacementWindow
- Team
- AllowedArea
- MaxDisplacement
- CanRotate
- TimeLimit
- ConfirmationRequired
```

Não existe autorização para mover o goleiro livremente o tempo inteiro.

### Timing

- intenção registrada agora;
- não interrompe a Fase 4;
- especificação do **Classic Strike Model** deve ser preparada antes ou durante a Fase 5;
- situações exatas de reposicionamento do goleiro dependem de regras futuras de reinício/falta/pênalti.

---

## 10. Histórico de fases, PRs e resultados

### Fase 0 — Foundation & Governance

**Status:** concluída e mesclada.

- PR #1: `Establish BFC Unity foundation and governance`
- merge SHA: `ae3a87236bf1b0742bb98e131fbd4729909c2724`
- Unity 6000.3.21f1 fixada;
- documentos normativos;
- arquitetura;
- governance/rules.json;
- PR template;
- CODEOWNERS;
- CI de governança;
- OPEN DECISIONS.
- PR #2 foi um PR noop/acidental e foi fechado sem relevância de produto.

### Fase 1 — Unity Bootstrap

**Status:** concluída e mesclada.

- PR #3: `Implement Phase 1 Unity bootstrap`
- merge SHA: `408a4b1f281b025d991c76303c83f63d6e514672`

Validação real:

- Unity abre/compila;
- Input System novo ativo;
- URP materializado;
- EditMode: **2/2**;
- PlayMode: **1/1**;
- Windows x64 build gerado;
- `BFC.exe` permaneceu em execução por mais de 5 s no smoke test;
- CI Governance verde;
- CI Unity Structure verde.

### Fase 2 — Physics Vertical Slice

**Status:** concluída e mesclada.

- PR #4: `Implement Phase 2 physics vertical slice`
- merge SHA: `43c3f0961805ce177a770d73d56e337fe9b469ae`

Implementado/validado:

- drag/release;
- potência;
- peça → bola;
- peça → peça;
- peça → parede;
- desaceleração;
- repouso;
- parâmetros centralizados;
- fixed-step;
- EditMode total: **5/5**;
- PlayMode: **1/1**;
- CI final verde.

Caveats importantes:

- `PhysicsLabTuning` ainda é provisório;
- movimento principal é planar XZ;
- desaceleração é fixed-step;
- collision detection atual do laboratório ficou **Discrete**, não congelada como decisão final;
- não inventar benchmarks numéricos que não foram capturados;
- o laboratório antigo possuía geometria aceitável para a fase, mas não era o padrão final de root/collider.

### Fase 3 — Match Core

**Status:** concluída, validada e mesclada.

- PR #5: `Implement Phase 3 match core`
- merge SHA: `c6d636a2ce93461b8034c17db6a01c8518e65ea4`

Implementação principal:

- `MatchState`;
- fases da partida;
- posse;
- `PlayerActionCommand`;
- validação de equipe/posse;
- `ResolvingAction`;
- espera por resolução física;
- contador de ações;
- limite vindo de ruleset;
- score;
- gol;
- `AwaitingRestart`;
- `ResumeAfterRestart(nextPossession)`;
- relógio lógico;
- eventos de domínio;
- fim explícito.

Regras preservadas:

- `GAME-003`;
- `GAME-004`;
- nenhuma UI necessária para testar;
- nenhuma decisão de OPEN-002/003.

Validação:

- 8 testes próprios do MatchController;
- EditMode total: **13/13**;
- PlayMode: **1/1**;
- metadados revisados;
- CI final verde.

### Fase 4 — Formation & Field

**Status geral:** **ATIVA**. Duas fatias importantes já concluídas e mescladas. A fase ainda não deve ser declarada encerrada até o gate restante ser formalmente fechado.

#### Fase 4 — primeira fatia: domínio

- PR #6: `[GOVERNANCE CHANGE] Implement Phase 4 formation and field`
- merge SHA: `6b80ee5bfe65d9fb183c9f05db175b864db005e9`

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
- ADR-0003;
- atualização de governança.

Validação:

- EditMode: **21/21**;
- PlayMode: **1/1**;
- 10 `.meta` revisados;
- CI final verde.

#### Fase 4 — segunda fatia: runtime materialization

- PR #8: `Implement Phase 4 runtime field materialization`
- merge SHA: `5ab0db9583aad4c7695dea1aeeb8863208dcfec1`

Cria o `FormationLab` e materializa em runtime:

- campo;
- linhas;
- áreas;
- gols;
- volumes de gol;
- bola;
- Team A;
- Team B;
- **22 peças**;
- **2 goleiros**;
- papel explícito em `FormationPieceRuntime`;
- posições calculadas pelo `FormationSpawnPlanner`;
- `PlanarKineticBody` nas peças e bola.

A composição mostrada no preview:

- 10 outfield + 1 GK por equipe;
- layout balanceado de preview;
- **não normativo**.

Campo de preview:

- comprimento: 28;
- largura: 18;
- demais dimensões do preview são tuning de laboratório;
- **não são medidas oficiais congeladas**.

Correção arquitetural relevante:

- root da peça fica em escala 1;
- Rigidbody/collider ficam no root;
- cilindro visual é filho escalado;
- evita repetir o problema de collider sob escala não uniforme.

Validação real da segunda fatia:

- primeira execução encontrou erros reais de compilação:
  - dependência desnecessária de `AudioListener`;
  - uso incorreto de `FormationSpawn.SlotId` em vez de `PieceId`;
- correção commitada em `6ab8355`;
- EditMode: **21/21**;
- PlayMode: **2/2**;
- inspeção visual do FormationLab aprovada;
- campo inteiro enquadrado;
- 11 peças por lado;
- goleiros visíveis;
- bola central;
- gols e áreas visíveis;
- Console sem erro/warning visível na evidência do FormationLab;
- 6 `.meta` gerados, GUIDs presentes e únicos;
- commit local de metadata: `0be852b`;
- Governance verde no head final;
- Unity Structure verde no head final;
- PR #8 mesclado por squash;
- `main` atual: `5ab0db9583aad4c7695dea1aeeb8863208dcfec1`.

Build settings:

- `Bootstrap` continua como entrada de produção;
- `FormationLab` é harness de desenvolvimento e fica fora do build de produção;
- Unity 6 serializou `m_UseUCBPForAssetBundles: 0`.

---

## 11. Estado atual do repositório

Após o merge do PR #8, o Product Owner executou:

```powershell
git switch main
git pull --ff-only
git status --short
```

Resultado:

- `main` avançou de `6b80ee5` para `5ab0db9`;
- 19 arquivos da segunda fatia foram incorporados;
- 780 inserções / 17 remoções;
- `git status --short` final ficou vazio.

Portanto, no instante de criação deste documento:

```text
branch local: main
origin/main: sincronizada
working tree: limpa
HEAD main: 5ab0db9583aad4c7695dea1aeeb8863208dcfec1
```

---

## 12. FormationLab — arquivos principais atuais

```text
Assets/BFC/FormationLab/
├── BFC.FormationLab.asmdef
├── FormationLabPreviewProfiles.cs
├── FormationLabRuntimeBootstrap.cs
├── FormationLabRuntimeBuilder.cs
└── FormationPieceRuntime.cs

Assets/BFC/Scenes/
└── FormationLab.unity

Assets/BFC/Tests/PlayMode/
└── FormationLabSmokeTests.cs
```

Objetivos do harness:

- provar que definições de domínio geram objetos Unity;
- materializar 11×11;
- testar papéis;
- testar spawn;
- verificar integração física;
- não servir como tela final do produto.

---

## 13. Processo padrão de desenvolvimento que estamos seguindo

### 13.1 Antes de começar uma fatia/fase

1. verificar `git status --short`;
2. buscar `origin`;
3. sincronizar `main`;
4. confirmar HEAD;
5. criar branch específica;
6. abrir PR Draft cedo quando a fatia possuir trabalho significativo;
7. documentar objetivo e não-decisões;
8. não tocar em regras OPEN fora do escopo.

Exemplo:

```powershell
cd F:\Projetos\BFC
git status --short
git fetch origin
git switch main
git pull --ff-only
```

### 13.2 Durante implementação

- código em inglês;
- docs de produto/arquitetura em português;
- regras centrais fora de MonoBehaviour;
- dependências explícitas;
- parâmetros centralizados;
- testes junto das regras;
- sem números mágicos duplicados;
- sem UI como autoridade;
- sem física decidindo regra;
- sem IA violando regra.

### 13.3 Validação local Unity

Comando padrão:

```powershell
.\scripts\run-unity-tests.ps1
```

O script:

- executa EditMode;
- espera processo terminar;
- valida XML;
- executa PlayMode;
- valida XML;
- falha se Unity retornar erro.

Validador estrutural:

```powershell
python .\scripts\validate_unity_structure.py
```

### 13.4 Arquivos `.meta`

Quando Unity gera novos `.meta`:

1. não commitá-los antes de validar código;
2. conferir `git status --short`;
3. verificar GUIDs;
4. garantir GUIDs não vazios e únicos;
5. fazer stage;
6. rodar `git diff --cached --check`;
7. trailing whitespace padrão do Unity em `.meta` não deve ser “corrigido” manualmente;
8. confirmar ausência de problemas fora de `.meta`:

```powershell
git diff --cached --check -- . ':(exclude)*.meta'
```

### 13.5 Antes de considerar um PR pronto

Exigir, quando aplicável:

- Unity real compila;
- EditMode verde;
- PlayMode verde;
- inspeção visual se houver cena/apresentação;
- metadata revisada;
- CI Governance verde;
- CI Unity Structure verde;
- documentação atualizada;
- PR body com evidências;
- working tree local limpa.

### 13.6 Merge

**Regra operacional desta colaboração: nenhum PR deve ser mesclado sem autorização explícita do Product Owner.**

Quando autorizado:

- revalidar head do PR;
- revalidar status mergeable/draft;
- confirmar CI;
- preferir squash conforme padrão adotado;
- usar expected head SHA quando possível;
- depois sincronizar `main` localmente;
- atualizar este documento de continuidade.

---

## 14. Governança de mudanças

### Implementação compatível

Bug fix/refactor/teste que não muda regra.

### Tuning

Só para propriedades marcadas como TUNABLE.

Exige:

- benchmark;
- justificativa;
- antes/depois quando possível.

### Mudança arquitetural

Exige ADR.

### RULE CHANGE

Se altera regra LOCKED:

- RFC;
- aprovação explícita do Product Owner;
- atualização de regra;
- testes;
- PR adequado;
- arquivos protegidos exigem sinais de governança.

### VISUAL CHANGE

Mudança material da constituição visual exige RFC visual e aprovação.

### Arquivos protegidos

A governança atual protege pelo menos:

- `docs/00-PRODUCT_CHARTER.md`;
- `docs/01-GAMEPLAY_CONSTITUTION.md`;
- `docs/02-VISUAL_CONSTITUTION.md`;
- `docs/05-CHANGE_CONTROL.md`;
- `governance/rules.json`.

Mudanças estabelecidas nesses arquivos devem seguir o processo de governança e prefixo de PR apropriado.

---

## 15. CI e proteção de `main`

`main` está protegida para fluxo por PR.

Regras já utilizadas:

- não trabalhar diretamente em `main` para feature;
- Governance obrigatório;
- Unity Structure obrigatório;
- force-push/deleção de `main` bloqueados.

Os workflows estáticos **não substituem** Unity real.

Já ocorreu um exemplo importante:

- CI estático estava verde;
- Unity real detectou erros de compilação no FormationLab;
- só após corrigir e reexecutar Unity os gates foram considerados válidos.

Portanto, sempre distinguir:

```text
static CI green
≠
runtime/Unity validation complete
```

---

## 16. Erros e aprendizados relevantes

### Phase 2

- collision detection ContinuousSpeculative foi alterado para Discrete durante estabilização do laboratório;
- não tratar isso como decisão de produção final;
- não inventar benchmark numérico ausente.

### Phase 4 runtime

Primeira execução real encontrou:

- referência ao módulo de áudio por AudioListener, apesar de o harness não precisar de áudio;
- `FormationSpawn.SlotId` inexistente, pois o tipo usa `PieceId`.

Correção:

- remover dependência desnecessária de AudioListener no harness;
- usar `PieceId`.

### Unity Build Settings

Unity 6000.3.21f1 removeu o FormationLab de EditorBuildSettings.asset quando ele não precisava ser build scene e adicionou:

```text
m_UseUCBPForAssetBundles: 0
```

Decisão adotada:

- FormationLab fora do player build;
- Bootstrap como entrada de produção;
- validador estrutural atualizado para refletir a serialização real.

---

## 17. O que NÃO fazer

- não decidir OPEN-002/003 por conveniência;
- não congelar 28×18 como dimensão oficial;
- não transformar a formação de preview em formação obrigatória;
- não implementar 11 jogadores de linha + goleiro extra: campo grande é 11 total;
- não forçar 11 peças em todos os modos;
- não duplicar regra de 3 ações em UI e IA;
- não deixar Physics decidir posse/score;
- não deixar Presentation decidir regra;
- não usar código legado como dependência de produção;
- não redesenhar a Interface e Menu por causa da Unity;
- não criar “online” fake;
- não declarar teste que não foi executado;
- não inventar números de benchmark;
- não fazer merge sem autorização explícita.

---

## 18. Próximas fases do plano

### Fase 4 — ainda ativa

Objetivo do gate original:

- quantidade por definição;
- formação;
- goleiro distinto;
- área;
- traves;
- limites;
- spawn/reposition seguro;
- formações alternativas;
- performance estável no target Windows.

Duas fatias foram concluídas. Antes de declarar a Fase 4 formalmente encerrada, revisar:

- evidência de performance no target Windows;
- se é necessária uma fatia adicional de integração com fluxo de partida;
- se perfis adicionais de campo/formação precisam entrar antes do gate;
- atualizar `docs/14-PHASE4_FORMATION_FIELD.md` para refletir as validações já concluídas.

### Fase 5 — Advanced Actions

Planejada para:

- chute;
- passe;
- spin;
- chip;
- menu radial;
- feedback força/direção;
- elegibilidade.

Antes/durante essa fase:

- escrever especificação do **Classic Strike Model**;
- proteger separação entre Modern Control e Classic Simulation.

### Fase 6 — Goalkeeper & AI

- goleiro automático;
- IA P1 vs COM;
- planner;
- dificuldade;
- fallback;
- ausência de deadlock.

### Fase 7 — Rules & Restarts

Depende de decisão sobre OPEN-002 e OPEN-003.

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

## 19. Próximo passo recomendado no momento desta atualização

O repositório acabou de receber o merge do PR #8 e está limpo em main.

A sequência mais segura é:

1. criar/mesclar uma atualização documental que:
   - introduza este documento de continuidade no repositório;
   - corrija o status antigo de `docs/13-PHASE3_MATCH_CORE.md`;
   - atualize `docs/14-PHASE4_FORMATION_FIELD.md` com os resultados reais do PR #8;
2. em seguida definir a próxima fatia da Fase 4, com foco no **fechamento formal do gate**, evitando iniciar prematuramente a Fase 5;
3. registrar evidência mínima de performance do FormationLab no Windows;
4. decidir se o gate de Fase 4 requer integração adicional com o Match Core ou se a materialização atual + testes já é suficiente;
5. só então declarar Fase 4 concluída;
6. preparar a especificação do Classic Strike Model antes/durante o início da Fase 5.

---

## 20. Protocolo obrigatório de atualização deste documento

Este arquivo deve ser atualizado **sempre que uma etapa, fatia ou fase relevante for concluída**.

Antes de encerrar um milestone/PR importante, atualizar:

### Cabeçalho

- data/hora;
- HEAD de main;
- branch ativa se ainda não mesclado;
- PR atual;
- status local.

### Histórico

Registrar:

- número do PR;
- título;
- head final;
- merge SHA;
- testes reais;
- CI;
- inspeções;
- metadata;
- build;
- decisões aprovadas.

### Estado atual

Atualizar:

- fase;
- fatia;
- blockers;
- OPEN DECISIONS;
- issues relevantes;
- próximos passos.

### Caveats

Registrar fatos que o próximo assistente não deve esquecer.

### Finalização

Depois de merge:

```powershell
git switch main
git pull --ff-only
git status --short
git log -1 --oneline
```

O resultado deve ser incorporado neste documento.

---

## 21. Checklist para handoff entre conversas

Antes de abandonar uma conversa longa:

- [ ] este documento foi atualizado;
- [ ] HEAD de main registrado;
- [ ] PR atual registrado;
- [ ] branch atual registrada;
- [ ] testes mais recentes registrados;
- [ ] CI registrado;
- [ ] decisões do Product Owner registradas;
- [ ] OPEN DECISIONS atualizadas;
- [ ] erros/caveats registrados;
- [ ] próximo passo explícito;
- [ ] nenhum merge pendente foi assumido como autorizado.

---

## 22. Referências principais do repositório

Documentos normativos/operacionais importantes:

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

docs/changes/RFC-0001-phase4-team-composition.md
docs/decisions/ADR-0003-mode-specific-formation-profiles.md

governance/rules.json
scripts/validate_unity_structure.py
scripts/run-unity-tests.ps1
scripts/build-windows.ps1
```

Issue de produto relevante:

```text
#7 — Specify BFC Classic Simulation mode
```

PRs/merges relevantes:

```text
#1 Foundation & Governance  -> ae3a87236bf1b0742bb98e131fbd4729909c2724
#3 Phase 1 Unity Bootstrap -> 408a4b1f281b025d991c76303c83f63d6e514672
#4 Phase 2 Physics          -> 43c3f0961805ce177a770d73d56e337fe9b469ae
#5 Phase 3 Match Core       -> c6d636a2ce93461b8034c17db6a01c8518e65ea4
#6 Phase 4 Domain           -> 6b80ee5bfe65d9fb183c9f05db175b864db005e9
#8 Phase 4 Runtime          -> 5ab0db9583aad4c7695dea1aeeb8863208dcfec1
```

---

## 23. Resumo de 30 segundos

```text
Projeto: BFC, reconstrução em Unity 6.3 LTS.
Referência funcional: BFC legado.
Referência visual: Interface e Menu.
Windows primeiro.
Core/Gameplay sem dependência desnecessária da engine.
Fases 0, 1, 2 e 3 concluídas.
Fase 4 ativa: domínio + runtime 11×11 já mesclados.
Campo grande = 11 total por time = 10 linha + 1 goleiro.
Treino/Desafios podem ter quantidades e campos próprios.
OPEN-002/003 continuam abertos.
FormationLab validado: EditMode 21/21, PlayMode 2/2, inspeção visual aprovada.
main atual = 5ab0db9583aad4c7695dea1aeeb8863208dcfec1.
Classic Simulation registrado na issue #7; não implementar como target direto da bola.
Nenhum PR deve ser mesclado sem autorização explícita do Product Owner.
Próximo objetivo: fechar formalmente o gate restante da Fase 4 e manter este documento atualizado.
```
