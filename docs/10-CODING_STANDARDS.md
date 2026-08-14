# BFC Coding Standards

Status: **ENGINEERING BASELINE**

## 1. Linguagem

- Código: inglês.
- Documentação de produto/arquitetura: português, salvo necessidade externa.
- Namespace raiz: `BFC`.
- Uma classe pública principal por arquivo, salvo tipos pequenos e fortemente relacionados.

## 2. Naming C#

- Types, methods, properties: `PascalCase`.
- Private fields: `_camelCase`.
- Locals/parameters: `camelCase`.
- Interfaces: `IName`.
- Constantes: `PascalCase` salvo convenção externa.
- Evitar abreviações ambíguas.

## 3. MonoBehaviour

`MonoBehaviour` é adapter de engine, não local padrão para lógica de negócio.

Evitar classes gigantes que acumulam:

- input;
- regras;
- física;
- UI;
- save;
- áudio.

## 4. Dependências

Preferir dependências explícitas por construtor/factory/composition root para classes puras.

Em componentes Unity, referências serializadas são aceitáveis quando representam composição de cena/prefab e não service locator oculto.

## 5. Proibido por padrão

- `FindObjectOfType` como DI;
- `GameObject.Find` em gameplay frequente;
- singleton global mutável sem ADR;
- `PlayerPrefs` fora de Infrastructure;
- strings mágicas para estados/modos;
- regras duplicadas em UI;
- `Update()` para física autoritativa;
- swallow de exception vazio;
- `async void` fora de event handlers apropriados;
- código morto comentado em produção.

## 6. Time

Diferenciar explicitamente:

- simulation time;
- game clock;
- wall clock;
- UI animation time.

Não usar `DateTime.Now` diretamente em regra testável; usar uma porta/clock quando tempo externo importar.

## 7. Random

Aleatoriedade de IA/gameplay deve ser injetável ou seedable onde reprodutibilidade for necessária.

Evitar `UnityEngine.Random` espalhado em regra central.

## 8. Floating point

Comparações físicas usam tolerância explícita.

Evitar igualdade exata de `float` quando o domínio não garantir representação exata.

## 9. ScriptableObject

Usar preferencialmente como definição/configuração de conteúdo.

Não usar como banco global mutável de estado de partida sem decisão arquitetural.

## 10. Serialização/save

Persistência usa DTO/schema explícito e versionado.

Não serializar grafo arbitrário de GameObjects para representar domínio persistente.

## 11. Logs

Logs de desenvolvimento devem:

- indicar subsistema;
- evitar spam por frame;
- não conter segredo;
- poder ser reduzidos em build de produção.

## 12. Testes

Toda correção de bug reproduzível deve preferencialmente adicionar teste que falhava antes da correção.

Regras `LOCKED` importantes devem possuir teste associado assim que implementadas.

## 13. Comentários

Comentar intenção e restrição, não traduzir literalmente a linha de código.

Quando código implementa regra normativa, citar ID quando isso melhorar rastreabilidade, por exemplo:

```csharp
// GAME-004: competitive possession allows up to three valid actions.
```

## 14. TODO

TODO crítico deve ter issue/RFC ou contexto suficiente. Evitar TODO genérico que vire dívida invisível.
