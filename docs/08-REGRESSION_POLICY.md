# BFC Regression Policy

Status: **NORMATIVE**

## 1. Princípio

Uma melhoria técnica não é sucesso se remove comportamento aprovado, piora sensação central sem decisão ou descaracteriza a interface oficial.

## 2. Tipos de regressão

### Funcional

Uma mecânica deixa de existir, muda resultado ou passa a falhar.

### Física

Mudam de forma não aprovada:

- distância;
- tempo de parada;
- rebote;
- spin;
- trajetória;
- repouso;
- colisão;
- resposta do goleiro.

### Regra

Turno, posse, score, modo ou eligibility mudam sem RULE CHANGE.

### Visual

Layout, cor, hierarquia, estilo ou estado interativo divergem da referência sem VISUAL CHANGE.

### Persistência

Save antigo deixa de carregar ou perde conteúdo.

### Input

Mouse, teclado, gamepad ou toque suportado perde funcionalidade.

### Performance

Frame time, loading ou memória degradam além do budget sem aprovação.

## 3. Golden Master

Antes de substituir sistema existente, produzir baseline suficiente para detectar diferença.

O baseline pode ser:

- teste;
- fixture;
- screenshot;
- vídeo;
- benchmark;
- save de compatibilidade;
- log estruturado.

## 4. Física

Cada milestone de física deve manter um conjunto de cenários determinísticos.

Comparar ao menos:

- distância final;
- posição final;
- tempo até repouso;
- velocidade máxima;
- desvio angular;
- resultado de colisão.

Tolerâncias devem ser explícitas. “Parecido” não é critério automatizável.

## 5. Frame rate

Testar gameplay com renderização em frequências diferentes.

O resultado da simulação deve permanecer equivalente dentro da tolerância definida.

## 6. Visual

Telas migradas devem possuir baseline visual.

Diferenças permitidas precisam ser classificadas:

- correção técnica;
- adaptação de aspect ratio;
- acessibilidade;
- mudança aprovada.

## 7. Save

Manter fixtures de versões anteriores assim que o primeiro schema de produção existir.

Nunca corrigir incompatibilidade apagando saves de teste sem entender o impacto.

## 8. Bug legado vs comportamento desejado

Ao detectar diferença com o protótipo:

1. confirmar se é comportamento desejado;
2. confirmar se é bug conhecido;
3. consultar regra;
4. se não estiver claro, registrar decisão aberta.

Não copiar bug apenas para obter igualdade binária.

## 9. Regressão aceita

Uma regressão só pode ser aceita quando:

- é conhecida;
- está descrita no PR;
- possui impacto avaliado;
- existe motivo de produto/arquitetura;
- possui aprovação apropriada;
- existe plano de correção quando temporária.

## 10. Stop-the-line

Regressões em regra `LOCKED`, save de produção ou corrupção de progressão são bloqueadoras de release por padrão.
