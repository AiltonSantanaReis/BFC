# BFC Gameplay Constitution

Status: **NORMATIVE**

Este documento registra a intenção oficial de gameplay. Ele separa regras bloqueadas, parâmetros ajustáveis e decisões ainda abertas para impedir que detalhes de implementação sejam tratados como autorização para mudar o jogo.

## 1. Classificação

- `LOCKED`: comportamento ou princípio que não pode mudar em PR comum.
- `TUNABLE`: o comportamento é obrigatório, mas números podem ser calibrados sem mudar a natureza da regra.
- `OPEN`: ainda exige decisão explícita antes de ser implementado como regra definitiva.

Os IDs correspondem ao registro em `governance/rules.json`.

## 2. Núcleo de partida

### GAME-001 — Futebol de botão por ações — LOCKED

BFC é um jogo de futebol de botão digital. O jogador escolhe uma peça, define direção e força e executa a jogada. A bola e as peças respondem à física e às colisões.

BFC não deve ser transformado silenciosamente em controle contínuo de personagens, futebol tradicional em tempo real ou outro gênero.

### GAME-002 — Mira por arrasto e soltura — LOCKED

A interação principal deve preservar o conceito do BFC legado:

1. selecionar peça elegível;
2. arrastar para definir vetor e potência;
3. obter feedback visual de direção/força;
4. soltar para executar.

Outros dispositivos podem mapear o mesmo conceito para analógico/toque, mas não podem eliminar a relação entre intenção, direção e potência.

### GAME-003 — Resolução física antes da próxima ação — LOCKED

Depois de uma jogada, a simulação deve resolver o movimento relevante até atingir condição segura de continuação antes da próxima ação tática.

Não deve haver nova jogada competitiva enquanto o estado físico ainda estiver indefinido.

### GAME-004 — Ações por posse competitiva — LOCKED

Os modos competitivos oficiais preservam o conceito de até **3 ações consecutivas por posse**, sujeito às regras explícitas de perda/troca de posse.

A implementação final deve diferenciar claramente `ação` de `toque na bola`; não é permitido assumir silenciosamente que são equivalentes.

### GAME-005 — Modos locais essenciais — LOCKED

O jogo deve suportar pelo menos:

- P1 vs COM;
- P1 vs P2.

Terminologia de interface deve usar esses nomes ou equivalentes aprovados, evitando rótulos técnicos como “dois humanos”.

## 3. Ações de peça

### GAME-010 — Chute — LOCKED

Ação padrão com potência normal configurada pelo jogador.

### GAME-011 — Passe — LOCKED / TUNABLE

Passe é uma variação deliberadamente mais controlada e com potência máxima reduzida em relação ao chute normal.

O percentual exato é parâmetro de balanceamento; o princípio de força reduzida é bloqueado.

### GAME-012 — Efeito / spin — LOCKED / TUNABLE

BFC deve suportar aplicar efeito à bola. A direção/intensidade deve representar intenção do jogador e produzir resultado físico legível.

O algoritmo, intensidade e mapeamento de input são calibráveis.

### GAME-013 — Jogada por cima / chip — LOCKED / TUNABLE

BFC deve preservar a mecânica de levantar a bola para superar determinados obstáculos/interações. Na reconstrução 3D, a implementação deve preferir uma trajetória física real em vez de simulação puramente visual.

Parâmetros de arco, altura e duração são calibráveis.

### GAME-014 — Menu contextual/radial — LOCKED VISUAL-FUNCTIONAL INTENT

As ações especiais devem permanecer acessíveis por uma interação contextual clara, preservando a intenção do menu radial do BFC legado. O input exato varia por mouse, controle e toque.

## 4. Goleiro

### GAME-020 — Goleiro automático competitivo — LOCKED

O goleiro é automático nos modos competitivos oficiais e nos modos que explicitamente adotam esse ruleset.

O jogador não pode obter vantagem competitiva por controle manual exclusivo do goleiro em um modo onde a regra oficial é automática.

### GAME-021 — IA de goleiro — TUNABLE

Tempo de reação, velocidade, alcance e tomada de posição são balanceáveis, mas devem ser simétricos entre lados sob o mesmo ruleset.

## 5. Física

### GAME-030 — Massa, restituição e atrito — LOCKED INTENT / TUNABLE VALUES

Peças, bola e superfície possuem propriedades físicas significativas. Diferentes conteúdos podem ter comportamento distinto apenas onde o ruleset permitir.

Nenhuma combinação válida pode gerar aceleração infinita, impossibilidade de repouso ou travamento de turno por erro numérico.

### GAME-031 — Simulação independente de FPS — LOCKED

A física de gameplay não pode depender da taxa de atualização do monitor.

A mesma entrada e o mesmo estado inicial devem produzir resultados equivalentes dentro das tolerâncias definidas, independentemente de 60, 120, 144 ou 240 Hz de renderização.

### GAME-032 — Competitive parity — LOCKED

No competitivo oficial, atributos de campo, bola e peças que alterem gameplay devem ser padronizados ou normalizados pelo ruleset.

Cosméticos não alteram massa, potência, atrito, restituição, spin, hitbox, visibilidade competitiva ou qualquer atributo de vantagem.

## 6. IA

### GAME-040 — IA funcional P1 vs COM — LOCKED

P1 vs COM deve usar IA capaz de selecionar peça, avaliar bola/gol e executar jogadas válidas sem depender de scripts específicos de uma única cena.

### GAME-041 — Evolução de IA — TUNABLE

Dificuldade, erro, planejamento, defesa, passes e agressividade são calibráveis. Uma melhoria de IA não pode quebrar regras de turno, input ou física.

## 7. Competitivo

### GAME-050 — Liga Justa — LOCKED MODE IDENTITY

`Liga Justa` é um modo competitivo com regras padronizadas e sem pay-to-win.

O baseline legado de entrada, duração, limite de gols, rating e recompensa deve ser registrado como benchmark antes de calibração definitiva.

### GAME-051 — Campeonato — LOCKED MODE IDENTITY

`Campeonato` é um modo competitivo distinto da Liga Justa, com progressão/estrutura de campeonato e recompensas próprias.

### GAME-052 — Recompensa mesmo em derrota — LOCKED

Participação válida pode gerar progresso/recompensa mesmo em derrota. O valor pode ser menor que vitória/empate, mas derrota não zera automaticamente todo progresso de sessão.

### GAME-053 — Monetização sem vantagem competitiva — LOCKED

Itens monetizados são cosméticos ou conteúdo sem vantagem estatística no ruleset competitivo oficial.

## 8. Progressão

### GAME-060 — Perfil persistente — LOCKED

BFC deve persistir perfil, configurações e progressão relevantes entre sessões.

### GAME-061 — Missões — LOCKED SYSTEM INTENT

Missões diárias/semanais/mensais podem orientar progressão e retenção sem exigir comportamento abusivo ou pay-to-win.

### GAME-062 — Troféus e vitrine — LOCKED SYSTEM INTENT

Conquistas/troféus obtidos devem poder alimentar uma vitrine/perfil visual.

### GAME-063 — Loja cosmética — LOCKED SYSTEM INTENT

A loja deve distinguir claramente:

- bloqueado;
- disponível para adquirir;
- possuído;
- equipado/selecionado.

## 9. Formação, saída e faltas

### OPEN-001 — Composição por modo e campo grande 11 peças — RESOLVED / LOCKED

BFC não possui uma única contagem global de peças para todos os modos.

- modos derivados do baseline legado preservam seus padrões de campo e composição por definição/ruleset;
- o perfil oficial de **campo grande** possui **11 peças no total por equipe: 10 jogadores de linha + 1 goleiro**;
- o goleiro está incluído na contagem de 11, não é uma peça adicional;
- Treino e Desafios podem definir campos, quantidades de peças e formações próprias conforme o cenário;
- lógica central não pode codificar uma quantidade fixa de peças fora da definição/ruleset.

A decisão está detalhada em `RFC-0001` e `ADR-0003`.

Os pontos abaixo continuam deliberadamente **OPEN** e não devem ser resolvidos por suposição.

### OPEN-002 — Bola fora / reposições

A implementação definitiva de bola fora, reposicionamento, tiro de meta, lateral/escanteio ou ausência deles deve ser formalizada antes do Rules Milestone.

Até essa decisão, nenhum comportamento temporário pode ser documentado como regra oficial.

### OPEN-003 — Faltas, vantagem e pênaltis

São capacidades desejadas para a evolução do ruleset, mas suas condições exatas ainda precisam de especificação própria antes da implementação definitiva.

## 10. Parâmetros não são regras novas

Valores como:

- potência máxima;
- coeficiente de atrito;
- duração da partida;
- limite de gols;
- velocidade de goleiro;
- atraso da IA;
- recompensa numérica;

podem ser calibrados quando classificados como `TUNABLE`, desde que:

1. o comportamento conceitual não mude;
2. haja benchmark antes/depois;
3. testes sejam atualizados conscientemente;
4. balanceamento competitivo permaneça simétrico.

## 11. Proibição de regressão disfarçada

É proibido remover uma mecânica porque “a engine nova não faz igual”. Se a engine exigir solução diferente, deve-se preservar a intenção ou abrir RFC de mudança de regra.
