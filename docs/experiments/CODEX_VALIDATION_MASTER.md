# Guia consolidado para Codex — PENDENTE DE VALIDAÇÃO

**Experimentos empilhados #13 → #14 → #15 → #16 → #17 → #18 → #19 → #21 → #22 → #23 → #24 → #25.**
Última branch contém as alterações anteriores, mas **nenhuma
foi aprovada**. Validar na branch, não na `main`.

## Ordem recomendada

1. Checkout `experiments/disc-collider-fidelity`.
2. Validar scripts:
   - `python scripts/validate_unity_structure.py`
   - `python scripts/validate_governance.py`
   - `python -m unittest discover -s scripts/tests -p "test_*.py" -v`
3. Usar Unity **6000.3.21f1** com dependências da `manifest.json`.
4. Rodar `scripts/run-unity-tests.ps1` no Windows; guardar XML/log.
5. Abrir FormationLab com parâmetros de processo
   `-bfcPlayableLab -bfcFieldProfile=Tactical`.
6. Comparar quatro escalas e controles P/T/V/I/R/C/Espaço, com os
   mesmos parâmetros de física. Validar gols, ações e bola fora.
7. Na mesma instalação, testar P1 vs P2 (padrão) e P1 vs COM (tecla I).
   Confirmar que IA da Equipe B usa somente peças de linha, respeita
   o turno, passa pelo `TryLaunch` e não movimenta goleiro manualmente.
   Guardar CSVs de **sessões separadas** para humano vs IA, porque o
   relatório atual não agrupa por tipo de controlador.
8. Capturar 10+ ações por combinação campo × tipo de ação; preferir
   >30 amostras quando possível, com histórico de hardware e opinião
   dos jogadores. Exportar CSV pela tecla C antes de sair da cena.
9. Gerar relatório com arquivos CSV:

```powershell
python scripts/report_field_telemetry.py test-csv/campo28.csv test-csv/campo42.csv --json-out TestResults/field-metrics.json --min-samples 10
```

10. Não declarar 42×27 vencedor apenas por mediana, gols ou FPS.
   Avaliar sensação de espaço, criatividade, tempo morto e satisfação
   com passes planejados.
11. Corrigir erros no PR correto. Não integrar à `main` nem marcar
    como validado antes de obter logs, capturas e aprovação explícita.

## O relatório calcula (sem decidir balanceamento)

Por campo e tipo de ação: quantidade, mediana/P95 de deslocamento
percorrido pela bola, distância normalizada, avanço líquido, percentual
de ações sem contato, bola fora e gols do protótipo. Grupos pequenos
recebem o sinalizador `exploratory_only`.

Amostras podem ser correlacionadas entre si. O relatório não estima
intervalos de confiança nem qualidade competitiva. Gols e bola fora
vêm de detectores experimentais, não da arbitragem definitiva.

## Novas validações de física, goleiro e geometria (#21–#25)

- [ ] Testar `GoalkeeperControlPolicyTests`: modos automáticos
      bloqueiam reposicionamento manual; janelas permitidas somente
      por eventos explícitos com tempo, equipe e área válidos.
- [ ] Exercício K no laboratório **P1 vs P2**, nunca no P1 vs COM:
      click prepara proposta e Enter confirma; goleiro não se move
      fora da janela. Tempo de seis segundos e área são temporários.
- [ ] Comparar **Q/E/X** para spin esquerdo/direito/neutro. Bola não
      deve ganhar spin antes do contato com a peça realmente lançada.
- [ ] Testar **H** para Chip: bola com gravidade real após contato,
      retorno ao plano antes da próxima jogada, sem gol acima da trave.
- [ ] Comparar `-bfcPieceCollider=SphereBaseline` (padrão)
      com `-bfcPieceCollider=ConvexDisc` (experimental).
      Medir contatos rasantes, sensação visual de diâmetro,
      desempenho e rebotes nos quatro campos.
- [ ] Verificar se alterações em spin, chip ou collider interferem
      nas regras existentes, gols, AI, telemetria e câmera.
- [ ] Documentar falhas e gráficos reais; sem homologação antes do
      editor Unity, hardware e avaliação humana.

## Pendências de produto não concluídas

- Regras finais por modalidade e integração das janelas do goleiro;
- Goleiro automático competitivo (ainda não implementado);
- posse por toque, spin/chip de produção e passe normativo;
- reposições/faltas/pênaltis (OPEN-002/003);
- P1 vs COM, rede, áudio, replays e arte final;
- decisão futura Unity/Unreal.

## Prompt para Codex

```text
No repositorio AiltonSantanaReis/BFC, valide a branch
experiments/disc-collider-fidelity, nao a main. Execute validadores
Python, Unity 6000.3.21f1 e scripts/run-unity-tests.ps1. Teste
FormationLab -bfcPlayableLab nos campos Original, Intermediate,
Tactical, Experimental, com comandos P/T/V/I/R/C/Espaco. Teste o adversario P1 vs COM sem alterar regras. Compare Q/E/X
para spin, H para chip, K para janela de goleiro em P1 vs P2 e
SphereBaseline versus ConvexDisc com -bfcPieceCollider=ConvexDisc. Gere CSVs por
perfil, analise com scripts/report_field_telemetry.py e anexe logs,
XML, telas e falhas reproduziveis. Nao faca merges, nem aprove regras
sem evidencia humana e tecnica.
```

**STATUS DO CONJUNTO:** PENDENTE DE VALIDAÇÃO (Unity/Codex). Os checks
GitHub "Governance" e "Unity Structure" não executam Unity. Os PRs
continuam draft e nenhuma das melhorias foi integrada à main.
