# ADR-0001 — Unity 6.3 LTS

Status: **ACCEPTED**

Date: 2026-08-14

## Contexto

O BFC será reconstruído do zero usando os protótipos anteriores como referência funcional e visual. Como o código legado não precisa ser preservado, a engine deve ser escolhida pelo ciclo de vida do produto, suporte 3D, tooling, input, UI, build e possibilidade futura de múltiplas plataformas.

## Decisão

Adotar **Unity 6.3 LTS** como família de editor para a fundação do projeto.

Baseline inicial do repositório:

- Editor: `6000.3.21f1`
- Changeset: `c02631ffc030`

## Motivos

- linha LTS adequada a produção estabilizada;
- suporte de longo prazo superior à linha Update para um baseline travado;
- C# e ecossistema maduros;
- boa adequação a física 3D de pequena/média escala;
- suporte a Windows como target primário;
- caminho técnico melhor para targets adicionais do que a arquitetura web anterior;
- tooling de profiling, input, assets, build e testes.

## Consequências positivas

- uma única engine para gameplay, física, UI, áudio e build;
- bola e peças podem usar 3D real;
- câmeras e efeitos não exigem renderer web próprio;
- melhor isolamento de gameplay de frame rate quando implementado corretamente;
- arquitetura preparada para ports futuros.

## Regressões e limitações

- código TypeScript legado não é reutilizado diretamente;
- período inicial terá menos features que o BFC legado;
- assets/UI precisam ser reconstruídos;
- Unity é tecnologia proprietária e sujeita a licenciamento/planos vigentes;
- ports de console continuam exigindo aprovação/SDKs dos fabricantes;
- PhysX não deve ser assumido como bitwise deterministic entre todas as plataformas;
- upgrades de Editor podem alterar física, renderer ou serialização e portanto exigem validação.

## Política de upgrade

Patch dentro de 6.3 LTS pode ser proposto por PR técnico com release notes analisadas e smoke tests.

Mudança para outra linha (por exemplo 6.4/6.5/novo LTS) exige novo ADR ou superseding ADR e regressão completa de:

- física;
- input;
- UI;
- build;
- save;
- render.

## Alternativas consideradas

### Godot

Excelente para PC/open source, mas menos direta para a estratégia potencial de consoles.

### Unreal

Capacidade gráfica muito acima da necessidade inicial e custo/complexidade maiores para o escopo do BFC.

### Web/Tauri

Maximizaria reaproveitamento do protótipo, mas deixaria de ser a melhor escolha quando a premissa passou a ser reconstrução do zero em engine.
