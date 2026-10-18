# Passe controlado e corredores táticos — PENDENTE DE VALIDAÇÃO

**Experimento dependente de PR #15 → #14 → #13. Branch e PR em draft.**

O objetivo é aumentar possibilidades de decisão sem transformar o BFC
em futebol controlado automaticamente e sem mudar regras aprovadas.

## Controles no FormationLab com -bfcPlayableLab

- `P`: alterna entre **Shot** e **ControlledPass** quando a ação está livre.
  O passe preserva direção/potência de arrasto, mas utiliza teto de velocidade
  reduzido para **55% da velocidade máxima do chute** (hipótese de tuning).
- `T`: mostra/esconde até quatro corredores geométricos da bola aos botões
  de linha mais próximos do time da vez.
- Cor **verde-água** = sem obstáculo estático no segmento calculado; **âmbar**
  = potencial interseção com o envelope de pelo menos um botão. O destino
  não é considerado obstáculo. Os traços são apenas informação tática.
- `C`: exporta CSV; a coluna `kick_kind` distingue passe e chute.
- Todos os controles anteriores continuam vigentes.

## Não faz

Não altera `MatchController`, regras de modalidade, IA, goleiro, posse,
regras de passe completo, tipo de força físico após colisão ou trajetória
da bola. Não prevê ricochetes, velocidades relativas nem a resposta dinâmica
de outros botões. Uma linha livre **não garante passe certo**. A ferramenta
não escolhe a direção, a força ou o botão para o jogador.

## Checklist Codex

- [ ] Importar sem erros em Unity 6000.3.21f1.
- [ ] Executar Unity Test Runner PlayMode/EditMode, anexar XML/log.
- [ ] Verificar `LabTacticalLaneTests` e regressões `TacticalActionTelemetryTests`.
- [ ] Confirmar passe com mesmo arrasto produz velocidade inicial
      inferior ao chute, sem alterar direção escolhida.
- [ ] Testar as teclas P/T, inclusive durante a animação de resolução.
- [ ] Verificar indicadores geométricos com espaço vazio, obstáculo
      no segmento, peças ao fundo e times alternados.
- [ ] Conferir se o HUD mostra opção atual e se o CSV inclui
      coluna `kick_kind` para comparação dos perfis de campo.
- [ ] Medir se os controles tornam a construção tática mais clara
      sem automatizar a habilidade. Registrar feedback humano.
- [ ] Preservar modalidades existentes. Nada aprovado até testes e revisão.

Este trabalho é uma hipótese instrumental, não representa modalidade
oficial nem prova que o passe ficou divertido.
