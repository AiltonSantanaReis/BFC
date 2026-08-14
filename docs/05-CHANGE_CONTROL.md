# BFC Change Control

Status: **NORMATIVE / GOVERNANCE**

## 1. Objetivo

Evitar três classes de desvio:

1. **drift de regra** — implementação muda comportamento sem decisão de produto;
2. **drift visual** — redesign acontece durante port/refatoração;
3. **drift arquitetural** — atalhos locais viram arquitetura permanente sem revisão.

## 2. Classes de mudança

### Classe A — Implementação compatível

Exemplos:

- corrigir bug sem mudar regra;
- otimizar alocação;
- melhorar teste;
- refatorar nome interno;
- corrigir responsividade mantendo layout.

Não exige RFC de regra, mas exige testes aplicáveis.

### Classe B — Tuning

Exemplos:

- potência;
- atrito;
- velocidade do goleiro;
- atraso da IA;
- recompensa numérica.

Permitido apenas para propriedades marcadas `TUNABLE`.

Exige benchmark antes/depois e justificativa no PR.

### Classe C — Arquitetura

Exemplos:

- novo sistema de DI;
- troca de renderer;
- mudança de save backend;
- troca de UI framework;
- mudança de versão major/minor da engine;
- multiplayer authoritative.

Exige ADR.

### Classe D — RULE CHANGE

Qualquer mudança que altere uma regra `LOCKED`, remova mecânica aprovada ou reinterprete comportamento normativo.

Exige RFC e aprovação explícita do Product Owner.

### Classe E — VISUAL CHANGE

Qualquer mudança material na constituição visual.

Exige RFC visual, comparativos e aprovação explícita do Product Owner.

## 3. Regra de ouro

> Se uma tarefa não diz que está mudando a regra, ela não pode mudar a regra.

“Foi necessário para implementar”, “a engine funciona diferente”, “é mais simples” e “fica melhor” não constituem aprovação.

## 4. Fluxo obrigatório para RULE CHANGE

1. Criar branch `rule-change/<assunto>`.
2. Criar `docs/changes/RFC-XXXX-<assunto>.md`.
3. Identificar IDs de regras impactados.
4. Descrever comportamento atual.
5. Descrever comportamento proposto.
6. Explicar motivo.
7. Listar regressões possíveis.
8. Listar impacto em UI, IA, física, save, progressão, competitivo e plataformas.
9. Atualizar testes propostos.
10. Obter aprovação explícita do Product Owner no PR.
11. Somente depois atualizar `governance/rules.json` e constituições.

## 5. Proibição de delete silencioso

Regra bloqueada não deve simplesmente desaparecer do registro.

Quando uma mudança aprovada aposenta uma regra:

- marcar como `deprecated`;
- registrar `supersededBy`;
- referenciar RFC;
- manter histórico.

## 6. Mudança arquitetural

Toda decisão arquitetural material recebe ADR em `docs/decisions/` contendo:

- contexto;
- decisão;
- alternativas;
- benefícios;
- regressões/limitações;
- consequências;
- reversibilidade.

## 7. Pull Request

Todo PR deve responder:

- Que problema resolve?
- Que módulos toca?
- Quais regras IDs são afetadas?
- Muda comportamento observável?
- Muda visual?
- Muda save?
- Muda física?
- Muda plataforma?
- Quais testes provam ausência de regressão?

## 8. Automação

O workflow de governança deve falhar quando arquivos protegidos mudarem sem os sinais mínimos de processo exigidos.

Automação é guarda adicional, não substitui revisão humana.

## 9. Hotfix

Hotfix de produção pode ser acelerado apenas para corrigir defeito grave.

Se o hotfix precisar alterar uma regra observável, a mudança deve ser registrada retrospectivamente em RFC antes de ser considerada baseline permanente.

## 10. Autoridade

A aprovação final de mudança de regra/identidade pertence ao Product Owner.

Aprovação de código por si só não equivale a aprovação de design.
