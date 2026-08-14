# BFC Reference Baseline

Status: **NORMATIVE REFERENCE PROCEDURE**

## 1. Objetivo

Os protótipos anteriores são referências executáveis, não dependências de produção.

A reconstrução deve capturar o que é valioso neles antes de substituir implementação.

## 2. Referência funcional — BFC legado

Antes de implementar um sistema equivalente, registrar quando aplicável:

- fluxo de entrada;
- estado inicial;
- parâmetros observáveis;
- resultado;
- captura de tela/vídeo;
- valores de configuração relevantes;
- comportamento correto;
- bugs conhecidos que **não** devem ser reproduzidos.

### Matriz mínima por mecânica

| Mecânica | Entrada | Resultado legado | Resultado Unity | Diferença aprovada? |
|---|---|---|---|---|
| Chute | vetor + força | TBD benchmark | TBD | - |
| Passe | vetor + força | TBD benchmark | TBD | - |
| Efeito | direção/intensidade | TBD benchmark | TBD | - |
| Chip | ação especial | TBD benchmark | TBD | - |
| Troca de turno | ações/posse | TBD benchmark | TBD | - |
| Goleiro | ataque válido | TBD benchmark | TBD | - |

`TBD benchmark` deve ser substituído por dados medidos durante a fase correspondente.

## 3. Baseline de física

Criar cenários reproduzíveis para pelo menos:

- chute 25%;
- chute 50%;
- chute 75%;
- chute 100%;
- passe em potência equivalente;
- colisão frontal;
- colisão oblíqua;
- rebote em parede quando o ruleset usar parede;
- spin esquerdo/direito;
- chip;
- tempo de repouso;
- goleiro contra trajetória direta.

Medir:

- distância;
- tempo;
- velocidade inicial quando disponível;
- desvio;
- posição final;
- resultado de colisão.

O objetivo não é copiar bugs matemáticos. É preservar a **sensação e intenção aprovadas** com uma implementação estável.

## 4. Referência visual — Interface e Menu

Para cada tela final:

1. capturar referência em resolução canônica;
2. registrar dimensões/aspect ratio;
3. identificar blocos estruturais;
4. registrar paleta medida em vez de estimada;
5. registrar tipografia/alternativa licenciada;
6. registrar spacing e alinhamentos;
7. registrar estados hover/focus/pressed;
8. registrar modal/overlay;
9. registrar motion;
10. capturar implementação Unity equivalente.

## 5. Golden references

Quando assets de referência forem adicionados ao repositório, usar pasta dedicada fora do runtime, por exemplo:

```text
References/
├── Functional/
├── Visual/
└── Benchmarks/
```

Esses arquivos não devem ser carregados pelo jogo em produção.

## 6. Evidência obrigatória

Uma afirmação como “já está igual” não é evidência suficiente.

Usar conforme o caso:

- teste automatizado;
- benchmark;
- screenshot comparativo;
- vídeo curto;
- log reproduzível;
- fixture;
- tabela antes/depois.

## 7. Divergência intencional

Quando a versão Unity divergir do legado por correção ou melhoria aprovada:

- documentar a divergência;
- citar regra/ADR/RFC;
- registrar por que o comportamento novo é desejado;
- atualizar baseline somente após aprovação.

## 8. Bugs conhecidos do legado

Não promover bugs conhecidos a requisito apenas porque são reproduzíveis.

Exemplos já identificados na auditoria e que não devem ser copiados como regra:

- física dependente da frequência de renderização;
- combinações de atrito que podem aumentar velocidade indefinidamente;
- ordem de integração/colisão inconsistente;
- estados de UI mockados como se fossem sistemas reais;
- atributos declarados sem efeito na mecânica.
