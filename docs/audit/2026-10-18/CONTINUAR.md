# Retomada local do BFC

O projeto clonado está em `C:\BFC`. A branch local `audit/project-assessment-2026-10-18` parte da cadeia experimental `experiments/disc-collider-fidelity`, commit `1b734bb3f433fb598c2db55d655a2b04f581a584`. `main` permanece disponível e não recebeu merges.

Ler `AUDITORIA.md` e `VALIDACAO.md` desta pasta antes de interpretar o handoff antigo. As imagens de `art/` são propostas e não a identidade aprovada do produto.

## Editor

Foi baixado o instalador oficial Unity 6000.3.21f1 e verificada sua assinatura Authenticode válida, de Unity Technologies SF. O editor foi preparado localmente em:

`C:\BFC\Temp\Dependencies\Unity-6000.3.21f1\Editor\Unity.exe`

O instalador de 4.092.408.560 bytes permanece em `C:\BFC\Temp\Dependencies\UnitySetup64-6000.3.21f1.exe`. Esses arquivos e o cache Unity ficam fora do Git. Não depender dessa pasta Temp em uma máquina nova: usar a instalação oficial dessa mesma versão ou informar o caminho correto ao script.

Fonte do download: https://unity.com/releases/editor/whats-new/6000.3.21f1

## Testes

No PowerShell, a partir de `C:\BFC`:

```powershell
python -B scripts/validate_governance.py
python -B scripts/validate_unity_structure.py
python -B -c "import sys,unittest; sys.path.insert(0,'.'); result=unittest.TextTestRunner(verbosity=2).run(unittest.defaultTestLoader.discover('scripts/tests',pattern='test_*.py')); raise SystemExit(not result.wasSuccessful())"
```

O terceiro comando contorna o Python local com `python313._pth` que omite o diretório atual; num Python convencional, o comando de unittest documentado no repositório pode funcionar diretamente.

Para usar o runner existente com o editor baixado:

```powershell
& .\scripts\run-unity-tests.ps1 -UnityPath 'C:\BFC\Temp\Dependencies\Unity-6000.3.21f1\Editor\Unity.exe'
```

Ele verifica XML e testes efetivamente executados; o exit code sozinho não basta. Consultar `VALIDACAO.md` para resultados desta sessão, pois o runner acima produz novos arquivos.

## Laboratório

Abrir a cena `Assets/BFC/Scenes/FormationLab.unity`. O bridge de input é opt-in e exige iniciar o processo com `-bfcPlayableLab`; o perfil pode ser selecionado por `-bfcFieldProfile=Tactical` e `-bfcPieceCollider=ConvexDisc`. Usar um editor por projeto para evitar bloqueio de arquivos.

```powershell
& 'C:\BFC\Temp\Dependencies\Unity-6000.3.21f1\Editor\Unity.exe' -projectPath 'C:\BFC' -bfcPlayableLab -bfcFieldProfile=Tactical
```

No editor, abrir FormationLab e entrar em Play. A cena é construída em runtime. Arrastar/soltar lança; P troca passe/chute; Q/E/X selecionam efeito; H seleciona chip; K abre o exercício de goleiro em hot-seat; Enter confirma; I alterna adversário; V alterna câmera; C exporta CSV; Espaço reinicia após gol; R reseta. Esses controles pertencem ao laboratório, não à interface final.

O build Windows padrão do repositório usa Bootstrap. Para construir FormationLab, existe `BFC.Editor.Build.BfcBuild.BuildFormationLabWindows64`; é uma entrada separada. Não confundir build de produção vazio com regressão do input do laboratório.

## Próxima implementação recomendada

Usar os problemas A1–A7 do relatório como backlog técnico. Consolidar controles e física com testes de interação, depois entregar partida curta completa e um slice audiovisual da proposta 2. As referências legadas ausentes e as medições de mesa real continuam necessárias para comprovar fidelidade e realismo. Não há autorização inferida para merge ou publicação.
