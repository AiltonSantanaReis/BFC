# Discos e fidelidade dos colisores — PENDENTE DE VALIDAÇÃO

Dependência: PR #24 e todos os anteriores. **Não integrar na main.**

## Discrepância confirmada no código
A malha Cylinder do Unity tem raio local de 0,5. O render visual
usava scale X/Z de `PieceRadius`, dando raio renderizado
`PieceRadius * 0.5`, enquanto o SphereCollider tinha raio
`PieceRadius`. Assim botões pareciam muito menores que as hitboxes.

Este PR dobra a escala horizontal do visual para representar o raio
físico existente corretamente, mantendo **SphereBaseline** por padrão.

## Experimento opcional
- No Editor da Unity, passar `-bfcPieceCollider=ConvexDisc`.
- Em execução com laboratório jogável: combinar com `-bfcPlayableLab`.
- Alternativamente, usar `Default Collider Profile` no Inspector do
  `FormationLabRuntimeBootstrap` antes do Play Mode.
- `ConvexDisc` usa MeshCollider convexo da malha fina do cilindro
  no filho de cada Rigidbody, com dimensões iguais ao visual.
- Não substitui SphereBaseline automaticamente. Hitboxes competitivas
  continuam indefinidas até validação real.

## Checklist Codex/Unity
- [ ] Unity 6000.3.21f1 compila com EditMode e PlayMode.
- [ ] Perfis SphereBaseline e ConvexDisc geram 22 peças e 1 bola.
- [ ] Malhas visuais possuem o mesmo raio real que o colisor escolhido.
- [ ] Confirmar MeshCollider convexo ativo e em Rigidbody pai.
- [ ] Comparar contatos de borda, rebotes, passes, chip, spin e goleiros.
- [ ] Verificar tunnelling, alto impulso e estabilidade em 30/60/120 Hz.
- [ ] Comparar frame time e custo dos colisores nos quatro campos.
- [ ] Registrar satisfação/percepção visual e tática com cada perfil.
- [ ] Escolha oficial de hitbox só após testes e aprovação expressa.

Não há homologação Unity nem árbitro de modalidades neste PR.
