# Chip experimental com gravidade e colisão 3D — PENDENTE DE VALIDAÇÃO

Dependente de #22 → #21 e anteriores. O objetivo é permitir ao jogador
imaginar uma trajetória por cima, sem transformá-la num efeito apenas visual.

## Controles
- `H` ativa o modo **Chip** antes de lançar (P volta para passe/chute).
- O comando seleciona o teto experimental de velocidade da peça;
  direção e potência continuam do mouse.
- A bola só recebe velocidade vertical após **contato real** com o
  botão lançado. Em seguida seu Rigidbody passa a ter gravidade.
- O detector de gols rejeita trajetórias acima do travessão.
- A bola pousa quando o centro atravessa a altura do raio da bola;
  depois volta à simulação planar.

## Limitações a resolver antes de produção
- Pouso inelástico de laboratório, sem física avançada de quique,
  rolagem, atrito em diferentes alturas ou física de campo real.
- Peças ainda usam colisores esféricos e permanecem no plano;
  o travessão e as traves da prévia são marcadores visuais.
- Necessário testar tunnelling, frame rate, CCD e casos de gol na
  borda e durante a trajetória alta.
- Não altera regras de modalidades, goleiro, reposições ou arbitragem
  oficial. Sem homologação de chip como mecânica final.

## Codex / Unity (pendente)
- [ ] Unity 6000.3.21f1 sem erros, EditMode e PlayMode todos aprovados
      com XML/log e frames documentados.
- [ ] Chip H e lance sem contato não levantam a bola.
- [ ] Chip de ambos os lados ultrapassa peça baixa sem teletransporte.
- [ ] Ball Y sobe e retorna ao raio; nova ação apenas após pouso.
- [ ] Bola por cima do travessão não registra gol.
- [ ] R, gol e bola fora não deixam a bola com gravidade ligada.
- [ ] Comparar Original e Tactical, FPS e qualidade do quique.
- [ ] Não aprovar ou fazer merge até revisão de gameplay.
