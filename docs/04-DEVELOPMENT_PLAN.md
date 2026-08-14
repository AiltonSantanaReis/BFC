# BFC Development Plan

Status: **NORMATIVE DELIVERY PLAN**

Este plano é orientado por **gates de qualidade**, não apenas por quantidade de features. Uma fase não é considerada concluída porque “funciona no meu PC”; ela precisa atender critérios objetivos e não introduzir regressões conhecidas sem aprovação.

## Fase 0 — Foundation & Governance

Objetivo: impedir que o novo projeto comece acumulando decisões implícitas.

Entregas:

- Product Charter;
- Gameplay Constitution;
- Visual Constitution;
- arquitetura modular;
- política de mudança;
- política de regressão;
- registro de regras legível por máquina;
- versão Unity fixada;
- estrutura inicial de assemblies;
- PR template e CODEOWNERS.

Gate:

- documentos normativos revisados;
- CI de governança ativo;
- ambiguidades críticas registradas como `OPEN`, não resolvidas por suposição.

## Fase 1 — Unity Bootstrap

Objetivo: projeto abre e compila em Unity 6.3 LTS sem conteúdo de gameplay ainda.

Entregas:

- URP configurado;
- Input System configurado;
- cena `Bootstrap`;
- cena `PhysicsLab`;
- assemblies sem dependência circular;
- testes EditMode operacionais;
- build Windows vazio funcional;
- logging básico de desenvolvimento.

Gate:

- zero erros de compilação;
- zero dependências circulares;
- build Windows inicia e encerra corretamente;
- arquitetura validada por referência de assembly.

## Fase 2 — Physics Vertical Slice

Objetivo: provar a sensação central antes de construir o jogo inteiro.

Escopo mínimo:

- campo de laboratório;
- duas peças;
- uma bola;
- seleção;
- drag/aim;
- potência;
- impulso;
- colisão peça×bola;
- colisão peça×peça;
- desaceleração;
- repouso;
- limite de campo de laboratório;
- métricas de benchmark.

Não inclui:

- IA completa;
- loja;
- menus finais;
- progressão;
- 3D de produção;
- formação completa.

Gate:

- física independente de FPS dentro da tolerância definida;
- nenhuma configuração válida acelera indefinidamente;
- resultados de benchmark registrados;
- sensação comparada com BFC legado;
- parâmetros centralizados.

## Fase 3 — Match Core

Objetivo: criar a máquina de estado de uma partida sem depender da UI final.

Entregas:

- MatchState;
- Team/posse;
- comandos de jogador;
- validação de ação;
- até 3 ações por posse onde o ruleset exigir;
- espera de resolução física;
- score;
- gol;
- relógio;
- início/fim de partida;
- eventos de domínio.

Gate:

- testes cobrem transições centrais;
- Presentation não é requisito para testar partida;
- regra de 3 ações não está duplicada na UI/AI.

## Fase 4 — Formation & Field

Objetivo: ampliar de laboratório para campo jogável.

Pré-condição:

- `OPEN-001` resolvido pelo Product Owner.

Entregas:

- quantidade oficial de peças;
- formação inicial;
- goleiro identificado como função distinta;
- área de gol;
- traves;
- limites;
- spawn/reposition seguro;
- suporte arquitetural a formações alternativas para treino.

Gate:

- nenhum número de peças hardcoded fora de definição/ruleset;
- testes de spawn e formação;
- performance estável no target Windows.

## Fase 5 — Advanced Actions

Objetivo: recuperar e superar as mecânicas de habilidade do legado.

Entregas:

- chute;
- passe com força reduzida;
- spin/efeito;
- chip/bola por cima 3D;
- menu radial/contextual;
- feedback de força e direção;
- regras de elegibilidade.

Gate:

- cada ação possui teste de regra;
- input não contém lógica de resultado;
- parâmetros ajustáveis centralizados;
- comportamento comparado ao legado.

## Fase 6 — Goalkeeper & AI

Objetivo: P1 vs COM jogável de ponta a ponta.

Entregas:

- goleiro automático;
- percepção da IA;
- seleção de peça;
- shot planner;
- decisão de força/direção;
- dificuldade inicial;
- limites de tempo de decisão;
- fallback para ação válida.

Gate:

- IA não viola regra para agir;
- goleiros simétricos no mesmo ruleset;
- IA conclui partidas sem deadlock;
- seeds/test fixtures permitem reproduzir cenários críticos.

## Fase 7 — Rules & Restarts

Objetivo: consolidar regras de bola fora, reposições, faltas e demais eventos.

Pré-condição:

- `OPEN-002` e `OPEN-003` resolvidos ou explicitamente adiados.

Entregas conforme decisões aprovadas:

- bola fora;
- reposicionamento;
- tiro de meta;
- faltas;
- vantagem;
- pênaltis;
- demais reinícios.

Gate:

- cada regra tem ID e teste;
- nenhuma regra depende do texto da UI;
- regras não são inferidas de collider names ou scene names.

## Fase 8 — Modes

Objetivo: transformar a partida base em experiências oficiais.

Entregas:

- P1 vs COM;
- P1 vs P2;
- Liga Justa;
- Campeonato;
- Treino;
- Desafios;
- padronização competitiva.

Gate:

- rulesets são composição de dados/serviços, não cópia de GameManager;
- competitivo normaliza atributos de gameplay;
- testes comprovam ausência de vantagem cosmética.

## Fase 9 — Progression, Economy & Save

Objetivo: reconstruir metajogo sem contaminar o motor de partida.

Entregas:

- perfil;
- save versionado;
- inventário;
- moedas/fichas/tokens conforme especificação aprovada;
- loja;
- missões;
- conquistas;
- troféus;
- vitrine;
- recompensas por resultado;
- migração futura preparada.

Gate:

- save possui schema version;
- corrupção possui fallback seguro;
- loja distingue locked/available/owned/equipped;
- economia competitiva não altera stats oficiais.

## Fase 10 — Official Interface & Menu

Objetivo: reconstruir em Unity a referência visual oficial.

Entregas:

- shell principal;
- navegação;
- telas;
- cards;
- modais;
- loja;
- perfil/vitrine;
- settings;
- integração com estado real;
- navegação por mouse/teclado/gamepad.

Gate:

- screenshot comparison por tela;
- nenhuma ação é mock se a tela é marcada como final;
- nenhuma informação técnica aparece ao jogador;
- nomes oficiais BFC;
- fidelidade visual aceita.

## Fase 11 — Match Presentation

Objetivo: transformar o motor funcional em experiência de produção.

Entregas:

- HUD;
- câmeras;
- feedback de turno/posse;
- power meter;
- radial final;
- VFX;
- replay/câmera de gol se aprovado;
- animações de resultado.

Gate:

- HUD não obstrui campo indevidamente;
- feedback não altera regras;
- câmera não prejudica input/precisão;
- perf budget medido.

## Fase 12 — Audio & Feel

Entregas:

- impactos;
- bola;
- peças;
- gol;
- torcida;
- UI;
- música;
- mix;
- sliders reais;
- feedback háptico quando suportado.

Gate:

- volumes configuráveis;
- configurações persistem;
- áudio não depende de frame rate;
- nenhuma faixa/asset comercial sem licença registrada.

## Fase 13 — Windows Production Build

Objetivo: primeiro target de produto.

Entregas:

- fullscreen/windowed/borderless conforme decisão;
- resolução;
- gamepad;
- teclado/mouse;
- save;
- crash logging local apropriado;
- build/versioning;
- pacote de distribuição.

Gate:

- smoke test em máquina limpa;
- 60/120/144 Hz testados;
- save survives update;
- zero blocker conhecido.

## Fase 14 — Optimization & Release Candidate

Entregas:

- profiling CPU/GPU;
- memória;
- loading;
- regressão visual;
- regressão de física;
- regressão de regras;
- acessibilidade mínima aprovada;
- release checklist.

Gate:

- todos os blockers fechados;
- documentação de versão;
- Product Owner sign-off.

## Fase 15 — Secondary Platforms

Somente após estabilidade Windows.

Avaliar separadamente:

- Web;
- Linux;
- macOS;
- Android;
- iOS;
- consoles quando houver acesso a programas/SDKs adequados.

Nenhum port é “grátis”. Cada plataforma recebe performance/input/UI/save QA próprios.

## Regra de sequenciamento

Feature visual não pode forçar arquitetura de gameplay.

Feature de gameplay não pode alterar constituição visual.

O plano pode ser reordenado por RFC, mas gates não podem ser removidos apenas para acelerar entrega.
