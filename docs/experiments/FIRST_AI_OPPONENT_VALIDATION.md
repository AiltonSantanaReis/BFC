# Primeiro P1 vs COM experimental — PENDENTE DE VALIDAÇÃO

Este protótipo adiciona apenas um adversário local **heurístico** ao
laboratório de campo, sobre PR #18 → #17 → #16 → #15 → #14 → #13.

- Padrão segue **P1 vs P2**.
- Pressione **I** enquanto espera uma ação para alternar **P1 vs COM**.
- A CPU controla a **Equipe B**, escolhe somente jogador de linha, estima
  a distância para bola, posição relativa e possíveis bloqueios retos,
  seleciona entre chute/passe experimental e lança via o MESMO
  `TryLaunch` e `MatchController` do humano.
- Não existe acerto automático nem teletransporte. Não move goleiros,
  não modifica a bola diretamente e espera física e câmera concluírem.
- Se não houver candidato elegível, registra aviso no Console; a sessão
  pode ser reiniciada com R ou retornar a P1 vs P2 com I.

**Não é IA de produção, não garante bom jogo nem implementa P1 vs COM
em qualquer modalidade oficial.** A escolha é uma heurística determinística
simples; não prevê colisões, não considera a posse física da bola e não
otimiza sequências de passes. GAME-040 requer evolução e testes de cena.

## Validar via Codex + Unity 6000.3.21f1

- [ ] Compilar e executar EditMode/PlayMode, inclusive
      `LabTacticalOpponentPlannerTests`; salvar logs/XML.
- [ ] Confirmar P1 vs P2 por padrão, alternância I só quando permitido.
- [ ] Confirmar CPU só age na posse da B e após repouso.
- [ ] Confirmar nenhum disparo extra, nenhum disparo de goleiro, nenhuma
      diferença de massa, potência ou física por ser CPU.
- [ ] Confirmar leitura correta de oclusão, seleção atrás da bola,
      times espelhados e casos sem candidatos.
- [ ] Com câmera V ligada/desligada, executar 10+ ciclos de CPU sem
      travar interação do humano.
- [ ] Comparar resultados no campo Original e Tactical.
- [ ] Identificar jogadas ruins, repetitivas, demasiado fortes e situações
      em que a CPU não consegue aproximar-se da bola.
- [ ] Documentar comportamentos frustrantes e propor evolução, SEM
      transformar este algoritmo provisório em regra definitiva.
- [ ] Não fazer merge antes de validação humana explícita.

## Prompt para Codex

```text
Valide a branch experiments/first-ai-opponent do BFC, empilhada
sobre as anteriores. Em Unity 6000.3.21f1, execute todos os testes;
abra FormationLab -bfcPlayableLab -bfcFieldProfile=Tactical e
ative I para P1 vs COM. Verifique timing, seletor de pecas,
seguranca do goleiro, disparo por MatchController, retorno da
camera e ausencias de travamento. Teste P1 vs P2 (default) e
alternancia, salve logs, CSV e videos. Registre falhas e corrija no
PR em draft, sem alterar main e sem declarar IA homologada.
```
