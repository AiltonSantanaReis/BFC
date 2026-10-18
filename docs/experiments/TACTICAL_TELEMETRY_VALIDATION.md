# Laboratório de telemetria tática — PENDENTE DE VALIDAÇÃO

**PR experimental empilhado sobre #14 → #13. Não aprovado, não integrar à main.**

## Objetivo

Medir a relação entre campo, física e ação real da bola antes de alterar
regras oficiais ou criar assistências artificiais. Um campo grande só traz
estratégia se oferecer mais escolhas interessantes sem criar tempo morto.

## Implementação

- Cada disparo aceito inicia uma amostra. Ela termina no repouso, gol ou
  saída da bola; R durante uma ação descarta apenas a amostra incompleta.
- A trajetória da bola é amostrada no `FixedUpdate`; mede distância
  percorrida, deslocamento máximo, avanço longitudinal e avanço líquido.
- A peça observa eventos `OnCollisionEnter` da bola com botões. Os
  contadores distinguem contatos da equipe que lançou e da adversária,
  contatos totais e número de peças distintas.
- A medição **não** muda física, turnos, posse, placar ou regras de goleiro.
- **C** exporta as amostras concluídas para CSV em
  `Application.persistentDataPath/BFC-Playtests/`.
  O console informa o caminho concreto no computador do teste.
- O HUD de depuração mostra os números da última jogada. A tecla **R**
  reinicia posições/placar, mas preserva as amostras anteriores no mesmo
  processo para permitir séries comparáveis.
- Máximo de 500 amostras em memória para limitar crescimento; exportar
  antes de sessões muito longas.

## Limitações (importantes)

1. **Não é detecção de passe válido.** OnCollisionEnter é somente
   evidência de contato. Pode registrar recontatos, rebotes ou contatos
   consecutivos com a mesma peça. Não determina posse.
2. A trajetória é uma aproximação por amostras no FixedUpdate; colisões
   rápidas e CCD desativado podem causar subcontagem ou tunnelling.
3. Não mede habilidade nem satisfação subjetiva, que precisam de pessoas
   jogando e dando feedback.
4. As distâncias são unidades do mundo do laboratório, não metros reais.
5. Gols, bola fora e limites de ações continuam experimentais conforme #14.
6. O CSV não inclui informações pessoais; equipe e ID de peça são apenas
   identificadores locais de laboratório.

## Checklist para Codex / Unity

- [ ] Compilar com Unity 6000.3.21f1 sem erros de assembly, scripts ou meta.
- [ ] Executar os testes EditMode/PlayMode existentes e os três testes
      `TacticalActionTelemetryTests`; anexar XML e logs reais.
- [ ] Confirmar que os testes de #13 e #14 continuam funcionando.
- [ ] Jogar 10+ lançamentos em cada perfil Original, Intermediate, Tactical
      e Experimental. Guardar um CSV por perfil (tecla C antes de sair).
- [ ] Confirmar distâncias / contatos manualmente em lances simples:
      bola parada, um impacto, ricochetes, contatos de ambas as equipes.
- [ ] Verificar que o HUD e o CSV distinguem avanço de A (+X) e B (-X).
- [ ] Confirmar R interrompe sem registrar jogada não concluída, e não
      apaga as amostras anteriores.
- [ ] Verificar exceções de escrita CSV, nomes de arquivos e permissões.
- [ ] Medir quantos lançamentos não tocam a bola, frequência de contatos
      com equipe adversária e progresso médio por ação.
- [ ] Relatar hardware, FPS, clareza de campo, frustração, tempo entre
      oportunidades e satisfação com passes planejados (feedback humano).
- [ ] Não alterar limites ou regras oficiais com base em amostras pequenas.
- [ ] Manter tudo como PENDENTE DE VALIDAÇÃO até evidências e aprovação.

## Instruções prontas para Codex

```text
Valide o PR de telemetria tática empilhado sobre #14 e #13 no
AiltonSantanaReis/BFC. Use Unity 6000.3.21f1, abra FormationLab com
-bfcPlayableLab -bfcFieldProfile=Tactical. Rode testes e jogue lances
com ambas as equipes. Pressione C para exportar CSV após cada série.
Compare 28x18, 36x23, 42x27, 48x31, mantenha os perfis e regras existentes.
Registre logs, XML de testes, CSVs, screenshots, problemas reproduzíveis,
mudanças sugeridas e parecer de jogabilidade. Não integre à main.
```
