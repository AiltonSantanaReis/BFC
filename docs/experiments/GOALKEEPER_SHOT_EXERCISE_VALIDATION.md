# Demonstração de anúncio de chute e goleiro — PENDENTE DE VALIDAÇÃO

PR empilhado sobre #23 → #22 → #21 e anteriores. **Não muda a main**.

## Teste manual (somente FormationLab, P1 vs P2)

- Durante turno esperando ação e com todas as peças paradas, pressione
  **K** para ANUNCIAR chute (exercício fictício do laboratório).
- O adversário ganha uma janela de **6 segundos experimentais** para
  posicionar seu goleiro numa região válida da área.
- Clique na região de destino para preparar a proposta. O goleiro ainda
  não se move; somente pressionar **Enter** confirma.
- Após confirmar ou expirar o prazo, o atacante pode executar sua
  próxima ação como **Shot** normal, sem poder trocar para passe/chip.
- Pressionar R cancela a proposta e reinicia a prévia.
- Goleiro nunca pode ser usado como botão de ataque; jogos com IA
  não recebem movimento manual do goleiro nesta demonstração.

## Limitações

- O gatilho K simula o anúncio de chute, mas NÃO corresponde a um
  evento formal autorizado por todos os rulesets.
- A região temporária é `FieldDefinition.GoalArea`, que pode ser
  diferente da área regulamentar do goleiro em cada modalidade.
- O tempo 6s NÃO representa regra aprovada de nenhuma modalidade.
- Não há UI 3D definitiva, controle seguro de players online,
  IA de goleiro, regras de reposição nem competição real.
- Outras modalidades devem fornecer sua própria política e razões
  antes de permitir qualquer janela.
- A validação deve verificar o goleiro sem sobreposição com bola e
  outras peças, e que a defesa acontece somente após confirmação.

## Checklist Codex

- [ ] Compilar Unity 6000.3.21f1, executar testes EditMode e PlayMode.
- [ ] Exercício só abre após K em P1 vs P2 com todas peças paradas.
- [ ] Bloquear lançamento enquanto goleiro reposiciona.
- [ ] Defender incorreto não confirma; posição ilegal não é aceita.
- [ ] Confirmar movimento só após Enter, dentro de área e longe de peças.
- [ ] Após janela, chute normal fica disponível; chip/passe bloqueados.
- [ ] Quando janela expira, não mover goleiro sem confirmação.
- [ ] Durante IA P1 vs COM, janela manual do defensor não pode abrir.
- [ ] Restaurar com R sem gravidade residual, sem física travada.
- [ ] Avaliar câmera e HUD nos quatro perfis de campo.
- [ ] Comparar com regras reais das modalidades antes de implementar
      eventos oficiais. NÃO marcar regras regulamentares como prontas.
