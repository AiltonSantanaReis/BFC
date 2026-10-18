# Spin físico experimental da bola — PENDENTE DE VALIDAÇÃO

Dependência: PR #21 e todos os anteriores. Este PR **não** integra a main.

## Como testar

FormationLab com `-bfcPlayableLab` e perfil de campo escolhido.
- `Q`: efeito à esquerda (-1)
- `E`: efeito à direita (+1)
- `X`: efeito neutro (0)
- A direção e potência continuam controladas pelo mouse.
- A escolha é registrada no CSV em `spin_direction`.
- A bola só começa a curvar **após contato físico com a peça que
  foi efetivamente lançada**; golpes sem contato não recebem spin.
- A curvatura vem de aceleração perpendicular à velocidade no plano XZ
  durante os passos físicos. A dissipação é temporal; teste tuning.

## Limitações

É uma aproximação Magnus em duas dimensões sobre Rigidbody PhysX.
Ainda não considera rotação de impacto 3D, efeitos na superfície,
efeito chip, rolamento real ou colisores de disco precisos. Rotação do
botão e efeito físico completo exigirão refinamento. Pode sofrer
diferenças por frequência de timestep e ordem do FixedUpdate, que
precisam ser medidas.

**Não modifica a regra oficial de spin; não afirma que o comportamento
físico final já está validado.**

## Checklist Codex

- [ ] Compilar Unity 6000.3.21f1 e executar todos os testes, salvar XML/log.
- [ ] Rodar `LabBallSpinTests`, testando Q/E/X nos dois lados e
      sem contato com bola (spin nunca ativado).
- [ ] Comparar mesmos golpes com Q/E/X, medir curvatura, velocidade,
      distância final e desvio lateral.
- [ ] Verificar se colisão com peça errada não inicia spin.
- [ ] Confirmar que R, bola fora e gol cancelam o spin e armado anterior.
- [ ] Testar 30/60/120 Hz de fixed timestep: tolerâncias serão definidas
      a partir das medidas, sem alegar determinismo antes disso.
- [ ] Avaliar se o jogador consegue prever e aprender o spin.
- [ ] Preservar modalidades, goleiros e competitividade sem alterações.
