# LAB Playtest Loop — PENDENTE DE VALIDACAO (Codex / Unity)

**Status: PENDENTE DE VALIDACAO. Nao aprovado. Nao integrar a main.**

Este experimento *opt-in* conecta entrada do mouse, elegibilidade de peca,
`MatchController`, fisica planar e detecao geometrica de gol. Sua finalidade e
gerar evidencias sobre espaco, posicionamento e satisfacao das jogadas.

## Dependencia

Este PR depende de `experiments/field-scale-lab` (PR #13). Testar a branch
`experiments/field-playtest-loop` com as duas alteracoes presentes.

## Ativacao

- Unity Editor 6000.3.21f1, cena `Assets/BFC/Scenes/FormationLab.unity`.
- No GameObject com `FormationLabRuntimeBootstrap`, marcar
  `Enable Playable Prototype` no Inspector (usar perfil Tactical para o
  primeiro teste).
- Alternativa: abrir o processo do Unity Editor com os argumentos
  `-bfcPlayableLab -bfcFieldProfile=Tactical`, entao rodar FormationLab.
- Clique numa peca de linha da equipe da vez; arraste na direcao oposta ao
  lancamento; solte para disparar.
- R reinicia o experimento e o placar. Espaco reposiciona todas as pecas
  apos um gol e entrega a posse ao adversario **apenas como convencao do lab**.

## O que realmente esta implementado

- 22 pecas, com goleiros imoveis pelo controle; bola e campo;
- duas equipes alternando por **limite experimental de 3 acoes**;
- bloqueio de segundo lancamento enquanto os corpos ainda estao em movimento;
- teste de gol ao cruzar com a bola inteira a linha de fundo, dentro da boca;
- placar simples em HUD de depuracao;
- campo configuravel via PR #13.

## NAO implementado; nao validar como se estivesse

- regras completas de nenhuma modalidade; selecao dinamica de ruleset;
- detecao de posse por toque, interceptacao ou falta;
- lateral, escanteio, tiro de meta, penalti (OPEN-002/OPEN-003);
- reposicionamentos legais do goleiro e goleiro automatico;
- goleiros que defendem por IA, colisao de discos de precisao, spin, chip;
- relogio correndo, fim de partida, online, audio ou camera imersiva;
- garantia de reproducao deterministica de todas as colisoes PhysX.

**Bola fora:** o experimento para de aceitar comandos e pede R para
reiniciar. Isso evita transformar uma hipotese de reposicao em regra oficial.
**Gol:** para e pede Espaco para reinicio manual; o adversario do marcador
recebe a posse por escolha demonstrativa, nao uma homologacao de OPEN-002.

## Checklist de validacao para Codex

Nao assinalar como aprovado sem executar de fato, em Windows + Unity:

- [ ] Verificar importacao Unity sem erros de compilacao/ASMDEF/assemblies.
- [ ] Rodar `scripts/run-unity-tests.ps1`, guardar `TestResults/*.xml`
      e `Logs/tests-*.log`, informar totais reais de testes EditMode e PlayMode.
- [ ] Confirmar perfis 28x18, 36x23, 42x27, 48x31, sem alteracao da
      boca do gol ou raio da bola; registrar capturas de tela.
- [ ] Tentar mover equipe errada, goleiro, disparo de potencia zero e
      segundo disparo com bola em movimento: todos devem ser recusados.
- [ ] Testar tres lancamentos validos da mesma equipe e verificar
      passagem da vez a outra equipe apos repouso.
- [ ] Testar gol para A e B, inclusive gol contra; placar unico e
      reinicio manual com Espaco. Testar finalizacao fora da boca do gol.
- [ ] Testar bola fora pelas laterais e fundos: nenhuma reposicao
      implicita; bloqueio explicito e reinicio com R.
- [ ] Medir mediana/P95 do deslocamento da bola em contatos reais para
      cada perfil (instrumentacao ainda nao incluida).
- [ ] Executar teste visual de mira, camera e legibilidade em 16:9,
      16:10 e 21:9; registrar hardware/FPS/logs.
- [ ] Classificar achados em bloqueantes, maiores e menores, com passos
      de reproducao, imagens e propostas de ajuste.
- [ ] Manter PR em draft e `main` intacta ate aprovacao expressa.

## Instrucao sugerida para o Codex

```text
No repositorio AiltonSantanaReis/BFC, valide a branch
experiments/field-playtest-loop (PR empilhado sobre #13).
Use o Editor Unity 6000.3.21f1 e execute tests EditMode/PlayMode.
Teste interativamente FormationLab com -bfcPlayableLab e
-bfcFieldProfile=Tactical, cobrindo as condicoes descritas em
docs/experiments/PLAYTEST_LOOP_VALIDATION.md. Nao altere regras
de modalidades, nao feche decisoes OPEN-002/003 e nao integre
PRs na main. Anexe logs, resultados e uma lista de problemas.
Qualquer correcao deve ser entregue em novo commit no PR draft.
```

A validacao de Unity e hardware nao foi executada por este agente.
