# Câmera de ação em campo — PENDENTE DE VALIDAÇÃO

**PR experimental empilhado sobre #16 → #15 → #14 → #13. Não aprovar sem Unity e playtest.**

## Experiência

- A mira continua na câmera de visão tática para não interferir com a
  precisão de escolha da peça, direção e potência.
- Após disparo aceito, quando habilitada, a câmera se aproxima da bola e
  acompanha a ação. Ao resolver por repouso, retorna à visão tática.
- Após gol detectado pelo protótipo, permanece aproximadamente 1,25 s
  em nível baixo antes de voltar à visão do campo.
- `V` habilita ou desabilita os momentos cinematográficos (padrão ligado).
- `R` e reinício após gol restauram imediatamente a perspectiva tática.
- Durante o retorno da câmera, novos cliques não iniciam mira até o
  enquadramento se estabilizar; observar se o atraso é confortável.
- A câmera não interfere nas forças, peças, bola, placar ou `MatchController`.

## Limitações

Não há público, celebrações, áudio, replay salvo ou câmera totalmente
polida. É uma experiência provisória de enquadramento para validar
o efeito emocional e a legibilidade. Não é um sistema de câmera final.

## Checklist Codex

- [ ] Compilar Unity 6000.3.21f1 sem erros; salvar logs.
- [ ] Executar EditMode/PlayMode completos, incluindo
      `LabCinematicActionCameraTests`, e arquivar XML.
- [ ] Seleção e mira sempre legíveis antes de chutar, nos quatro campos.
- [ ] Testar V desligado/ligado, antes e durante o movimento.
- [ ] Confirmar que nenhum Rigidbody foi movimentado pela câmera.
- [ ] Câmera regressa na hora certa ao repouso, ao gol e com R/Espaço.
- [ ] A câmera não fica presa se a bola sai, sofre colisões ou é lançada
      mais de uma vez consecutivamente.
- [ ] Testar campos em proporção 16:9, 16:10 e 21:9 e FPS 60.
- [ ] Registrar feedback: emoção dos momentos próximos, orientação
      espacial, enjoo de movimento e eventuais atrasos na próxima ação.
- [ ] Não integrar à main ou homologar o comportamento sem aprovação.

## Instrução para Codex

```text
Valide o PR da camera imersiva sobre as branches experimentais do BFC.
Abra FormationLab com -bfcPlayableLab -bfcFieldProfile=Tactical no Unity
6000.3.21f1 e execute EditMode/PlayMode. Teste a tecla V, lances
normais, bolas fora, gols, reinicios, resolucao lenta e ajustes da camera
em campos 28x18, 36x23, 42x27 e 48x31. Anexe logs, capturas e
gravacoes de jogadas e avalie conforto/legibilidade da mira.
Corrija defeitos em commits no PR, sem alterar main ou regras oficiais.
```
