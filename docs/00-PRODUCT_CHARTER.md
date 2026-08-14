# BFC Product Charter

Status: **NORMATIVE / LOCKED**

## 1. Produto oficial

O nome oficial do produto é **BFC**.

Nomes de protótipos, nomes internos antigos ou variações de marca não substituem `BFC` em produto, código público, UI, builds, documentação de usuário ou materiais oficiais sem uma decisão formal de marca.

## 2. Missão

BFC é um jogo competitivo de futebol de botão digital que combina:

- física legível e satisfatória;
- decisões táticas por turnos/posse;
- domínio mecânico de força, direção, passe, efeito e jogadas especiais;
- IA funcional para jogo solo;
- modos casual e competitivo;
- progressão, desafios, troféus e cosméticos sem vantagem competitiva paga;
- identidade visual cyber/neon definida pela referência `Interface e Menu`.

## 3. Referências oficiais

Há duas referências canônicas com funções diferentes.

### 3.1 BFC legado — referência funcional

É a referência para intenção e comportamento de:

- mecânicas;
- regras;
- fluxo de partida;
- física percebida;
- IA;
- goleiro automático;
- modos;
- progressão;
- economia;
- save;
- desafios;
- missões;
- conquistas.

Bugs conhecidos do legado não são requisitos.

### 3.2 Interface e Menu — referência visual

É a referência visual oficial para:

- layout;
- composição;
- cores;
- estilo;
- hierarquia;
- cards;
- modais;
- fundos;
- vitrine;
- linguagem visual;
- animação e atmosfera.

Trocar de engine não autoriza redesenhar essa identidade.

## 4. Ordem de autoridade

Quando documentos ou implementações entrarem em conflito, a ordem é:

1. decisão explícita mais recente do Product Owner registrada no repositório;
2. `governance/rules.json`;
3. este Product Charter;
4. Gameplay Constitution;
5. Visual Constitution;
6. ADRs aceitos;
7. Development Plan;
8. comportamento das referências legadas;
9. implementação atual.

**Código nunca é autoridade superior à regra aprovada.**

## 5. Não negociáveis

### P-001 — Nome

O produto é BFC.

### P-002 — Identidade visual

A estética da referência `Interface e Menu` deve ser preservada. Melhorias técnicas podem corrigir responsividade, performance, acessibilidade e resolução sem descaracterizar a composição aprovada.

### P-003 — Gameplay antes de cosmética

Decisões de arquitetura e arte não podem degradar clareza, resposta ou consistência da jogabilidade.

### P-004 — Sem pay-to-win competitivo

Dinheiro real, moeda premium ou aquisição cosmética não podem conceder vantagem de atributo no ambiente competitivo oficial.

### P-005 — Regras não mudam silenciosamente

Nenhuma tarefa de implementação, refatoração, otimização, port ou correção autoriza mudar uma regra `LOCKED` sem seguir `docs/05-CHANGE_CONTROL.md`.

### P-006 — Referências não são dependência de runtime

O novo BFC pode reproduzir conceitos aprovados, mas não deve depender do código legado em produção.

### P-007 — Testabilidade

Regras centrais devem ser verificáveis por teste sem exigir a UI completa.

### P-008 — Plataforma primária

Windows é a plataforma primária de produção inicial. Arquitetura deve evitar bloqueios desnecessários para outras plataformas, mas nenhuma plataforma secundária pode comprometer a qualidade da versão Windows sem decisão formal.

## 6. Product Owner

A autoridade final para mudança de regra, escopo de produto, identidade ou direção é o proprietário do repositório/produto.

Automação, IA, contributor, engine, plugin ou ferramenta não pode promover uma hipótese de design a regra oficial sem aprovação registrada.

## 7. Regra de ambiguidade

Quando uma referência antiga e uma intenção de produto divergirem, a implementação deve **parar naquela decisão específica**, registrar a questão em `OPEN_DECISIONS.md`/RFC e manter o restante do trabalho que não dependa dela.

É proibido resolver ambiguidade estrutural por conveniência técnica.
