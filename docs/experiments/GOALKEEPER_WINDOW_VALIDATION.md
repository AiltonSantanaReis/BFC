# Goleiro: autoridade por janela explícita — PENDENTE DE VALIDAÇÃO

Este PR introduz **somente a autoridade de regras e geometria**, separada
da Unity, sem alterar o comportamento das partidas atuais.

## Princípios
- `AutomaticCompetitive`: nenhum jogador pode solicitar reposicionamento manual.
- `ManualLocked`: bloqueio até que o ruleset defina permissões.
- `GoalkeeperControlPolicy` recebe contextos **aprovados pelo ruleset**;
  não presume que uma modalidade permite todo tipo de movimentação.
- `GoalkeeperPlacementWindow` só abre com evento explicitamente autorizado,
  peça física em repouso, equipe válida e período configurado.
- Candidato deve caber inteiramente dentro da área definida como
  `GoalArea` de seu time; confirmação verifica a equipe correta.
- Expiração cancela posição não confirmada. A posição da peça física
  não muda até o consumidor processar `TryCommit`.

## Contextos simbólicos (não direitos universais)
`ShotDeclared`, `SetPiece`, `TacticalAdjustment`. Cada modalidade
decidirá qual contexto é permitido, se existe reposicionamento, duração
e a geometria aplicável. **Não ativar janelas em Classic nem nos
competitivos por mera suposição.**

## Atenção de engenharia
O `GoalArea` de `FieldDefinition` foi reutilizado como envelope
de exemplo, mas algumas modalidades têm uma região de posicionamento
do goleiro diferente. Antes de integrar ao gameplay real, substituir
essa simplificação por volumes específicos de cada ruleset/peça.

## Testes Codex/Unity
- [ ] Compilar o Core sem dependência da Unity.
- [ ] Executar `GoalkeeperControlPolicyTests` em EditMode.
- [ ] Confirmar automático sempre bloqueado e validade de ambos os lados.
- [ ] Confirmar temporizador cancelando commit após expiração.
- [ ] Revisar manualmente as regras de cada modalidade e definir eventos.
- [ ] Implementar adaptação de eventos e UI em PR separado (não feito aqui).
- [ ] Não aprovar regras ainda abertas; não fazer merge na main.

**Este PR não habilita reposicionamento manual no jogo.** Cria uma
base segura para fazê-lo quando suas modalidades forem conectadas.
