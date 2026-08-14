# RFC-0001 — Composição por modo e perfil de campo grande

Status: **ACCEPTED**

Date: 2026-08-14

## Contexto

`OPEN-001` bloqueava a Fase 4 porque o histórico do BFC contém dois objetivos que não devem ser confundidos:

- preservar os padrões de formação/campo dos modos derivados do BFC legado;
- adicionar modos de campo maior com formação ampliada.

Treino e Desafios também precisam poder variar campo, quantidade de peças e formação conforme o cenário.

## Decisão aprovada pelo Product Owner

1. BFC não terá uma única contagem global de peças válida para todos os modos.
2. O perfil oficial de **campo grande** usa **11 peças no total por equipe**.
3. Nesse total estão incluídos **10 jogadores de linha + 1 goleiro**.
4. O goleiro não é uma 12ª peça adicional.
5. Modos derivados do BFC legado preservam suas composições e padrões de campo por definição/ruleset, em vez de serem automaticamente convertidos para 11 peças.
6. Treino e Desafios podem usar campos, quantidades de peças e formações diferentes conforme cada cenário.
7. Contagem, papéis e formação devem ser dados configuráveis; lógica central não pode depender de um número mágico de peças.

## Consequências

- `OPEN-001` deixa de bloquear a Fase 4;
- o perfil 11 peças é específico de campo grande e não redefine todos os modos;
- a arquitetura deve permitir múltiplos `TeamCompositionDefinition`, `FormationDefinition` e `FieldDefinition`;
- cenários de Treino/Desafios podem omitir o perfil 11 peças sem quebrar o Match Core;
- competitivo deve manter simetria entre as equipes dentro do mesmo ruleset;
- dimensões finais de cada campo e formação tática específica continuam sendo conteúdo/tuning de modo, salvo regra posterior.

## Fora de escopo

Este RFC não resolve:

- `OPEN-002` — bola fora e reposições;
- `OPEN-003` — faltas, vantagem e pênaltis;
- dimensões finais dos campos;
- formação tática única obrigatória para o perfil de campo grande.
