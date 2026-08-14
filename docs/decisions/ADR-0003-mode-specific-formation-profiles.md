# ADR-0003 — Perfis de formação por modo

Status: **ACCEPTED**

Date: 2026-08-14

## Contexto

O BFC precisa preservar modos derivados do baseline legado e, ao mesmo tempo, oferecer campos maiores com mais peças. Tratar uma única quantidade de peças como constante global faria Treino, Desafios, modos legados e campo grande competirem pela mesma regra de composição.

## Decisão

Adotar **composição, formação e campo configuráveis por modo/ruleset**.

O perfil de **campo grande** possui **11 peças no total por equipe: 10 de linha + 1 goleiro**. O goleiro está incluído na contagem de 11.

Modos derivados do BFC legado mantêm seus padrões por configuração própria. Treino e Desafios podem fornecer campos e formações específicas de cada cenário, inclusive com quantidades diferentes de peças.

## Regras de arquitetura

- não existe `11` como constante global de toda partida;
- o valor 11 pertence à definição do perfil de campo grande;
- papéis das peças são explícitos no domínio;
- o goleiro não é inferido por nome, cor ou GameObject;
- formação e posição inicial são dados, não lógica duplicada por modo;
- dimensões de campo são fornecidas por definição;
- equipes do mesmo ruleset competitivo usam composição simétrica;
- Core permanece independente de UnityEngine.

## Consequências

A Fase 4 pode implementar uma infraestrutura única para:

- perfis reduzidos derivados do legado;
- campo grande 11 contra 11;
- Treino com poucos elementos;
- Desafios com composição especial;
- futuras formações alternativas.

Nenhuma dessas variações exige copiar um `GameManager` ou criar lógica de spawn específica por cena.

## Não decisões

Este ADR não congela:

- dimensões finais do campo grande;
- uma formação tática única como 4-4-2 ou 4-3-3;
- regras de lateral, escanteio ou tiro de meta;
- regras de falta, vantagem ou pênalti.
