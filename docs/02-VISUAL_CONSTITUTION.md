# BFC Visual Constitution

Status: **NORMATIVE / LOCKED VISUAL DIRECTION**

## 1. Fonte visual oficial

O projeto denominado **Interface e Menu** é a referência visual oficial do BFC.

A reconstrução em Unity deve preservar a identidade aprovada em vez de reinterpretá-la livremente.

## 2. Elementos bloqueados

A menos que exista RFC visual aprovada, devem permanecer reconhecíveis e fiéis à referência:

- layout geral;
- composição das telas;
- navegação e hierarquia visual;
- paleta cyber/neon;
- relação entre cyan, magenta, fundos escuros e brilhos;
- cards e painéis;
- estilo de bordas;
- fundos cinematográficos;
- modais;
- vitrine/troféus;
- densidade visual;
- estilo de botões;
- atmosfera tecnológica;
- proporção e ritmo dos espaços;
- linguagem de motion/feedback.

## 3. Fidelidade não significa copiar defeitos técnicos

É permitido corrigir:

- responsividade;
- resolução;
- legibilidade;
- acessibilidade;
- navegação por teclado/gamepad;
- safe areas;
- performance;
- text wrapping;
- clipping;
- scaling;
- z-order;
- suporte ultrawide;
- suporte a DPI.

Essas correções não autorizam trocar a direção de arte.

## 4. Desktop é a referência primária

A primeira meta visual é Windows em 16:9.

A reconstrução deve ser comparada por screenshot com a referência em uma resolução canônica definida pelo time. Adaptações para outras proporções devem preservar hierarquia e identidade, não obrigatoriamente coordenadas absolutas.

## 5. BFC como marca

Textos e elementos de marca devem usar **BFC** como nome oficial.

Variações legadas não devem reaparecer por cópia de assets ou código antigo.

## 6. Conteúdo técnico não pertence à UI de jogador

Informações como:

- nomes de componentes;
- estados internos de sistema;
- mensagens de debug;
- nomes técnicos de arquitetura;
- indicadores de desenvolvimento;

não devem aparecer em telas de jogador, salvo em ferramentas de desenvolvimento explicitamente separadas.

## 7. UI em Unity

A tecnologia de UI pode mudar desde que a saída visual permaneça fiel.

A implementação deve separar:

- estrutura/layout;
- styling;
- dados;
- navegação;
- animação;
- binding com estado do jogo.

Nenhum componente visual deve possuir regra de gameplay.

## 8. Processo de validação visual

Uma tela só pode ser considerada migrada quando existir:

1. captura da referência;
2. captura da implementação Unity na mesma proporção;
3. checklist de layout;
4. checklist de cores/contraste;
5. checklist de conteúdo;
6. checklist de interação;
7. registro das diferenças intencionais.

Diferença não documentada é regressão até prova em contrário.

## 9. Assets

Assets provenientes dos protótipos devem ser tratados como referência/licenciamento a verificar antes de uso comercial definitivo.

A arquitetura não deve depender de caminhos frágeis ou assets externos não versionados.

## 10. Mudanças visuais

Uma mudança de estética exige RFC quando alterar materialmente:

- paleta;
- layout principal;
- posição/hierarquia dos blocos;
- estilo de navegação;
- estilo dos cards;
- tipografia principal;
- formato da vitrine;
- linguagem de animação;
- identidade cyber/neon.

O argumento “fica mais moderno” não é justificativa suficiente para desviar da referência aprovada.
