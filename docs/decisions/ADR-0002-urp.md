# ADR-0002 — Universal Render Pipeline (URP)

Status: **ACCEPTED**

Date: 2026-08-14

## Contexto

BFC precisa de 3D estilizado, iluminação neon, materiais, pós-processamento moderado, boa performance em Windows e possibilidade de ampliar targets posteriormente.

## Decisão

Adotar **Universal Render Pipeline (URP)** como pipeline gráfico padrão.

Para Unity 6000.3, a família correspondente é URP 17.3.

## Motivos

- adequado a desktop e targets adicionais;
- custo menor que um pipeline focado apenas em high-end;
- Shader Graph e ferramentas suficientes para estética cyber/neon;
- bom equilíbrio entre qualidade, performance e manutenção;
- reduz necessidade de manter pipelines completamente diferentes por plataforma.

## Não objetivos

O uso de URP não autoriza reduzir a identidade visual do `Interface e Menu`.

Também não significa que todos os efeitos serão implementados na primeira fase.

## Consequências

- materiais/shaders de produção devem ser compatíveis com URP;
- assets externos devem ser avaliados quanto a compatibilidade;
- efeitos devem possuir budgets;
- troca para HDRP ou pipeline customizado exige ADR e regressão visual/performance.

## Riscos

- efeitos high-end específicos podem exigir solução customizada;
- diferenças entre plataformas exigirão quality profiles;
- shader excessivamente complexo pode comprometer targets secundários.
