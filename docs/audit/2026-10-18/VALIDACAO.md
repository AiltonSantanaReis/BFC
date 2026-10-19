# BFC — evidências da validação local

Data: 18/10/2026. Branch local: `audit/project-assessment-2026-10-18`, derivada do commit experimental `1b734bb3f433fb598c2db55d655a2b04f581a584`. Nenhum merge, push, aprovação de PR ou alteração de regra foi realizado.

## Ambiente e dependências

- Unity executada: **6000.3.21f1**, revisão **c02631ffc030**, confirmada pelo log do próprio editor.
- O caminho inicialmente anunciado pelo Unity Hub em Program Files continha só metadados. Foi baixado o instalador oficial, verificada assinatura Authenticode válida de Unity Technologies SF e preparado o editor em `C:\BFC\Temp\Dependencies\Unity-6000.3.21f1`.
- O editor importou e resolveu os pacotes do projeto. A versão da engine do projeto foi preservada.
- Testes Unity executados em batch mode com `-nographics`. Isso verifica código e testes de simulação, sem comprovar qualidade visual ou desempenho com renderização.
- Inventário antes dos testes: 60 arquivos C# e 3 cenas Unity.

## Verificações já concluídas

| Verificação | Resultado | Evidência |
|---|---|---|
| Registro de governança | 27 regras, 27 IDs únicos | `governance.txt` |
| Estrutura Unity | Passou | `structure.txt` |
| Testes Python de telemetria | 4/4 passaram | `python-tests.txt` |
| Unity na ponta original | Falha de compilação CS0104 | `C:\BFC\Logs\audit-editmode-baseline.log` |
| Após primeira correção | Falha de compilação CS0246 | `C:\BFC\Logs\audit-EditMode.log` |
| Após segunda correção | Falha de compilação CS0103 | `C:\BFC\Logs\audit-EditMode-fixed.log` |
| EditMode após as três correções | **25/25 passaram**, 0 ignorados | `C:\BFC\TestResults\Audit-EditMode-final.xml` |
| PlayMode antes do ajuste dos testes | 32/34 passaram; 2 falhas de leitura imediata do Transform | `C:\BFC\TestResults\Audit-PlayMode-final.xml` |
| Prova isolada do goleiro | Corpo físico correto em X=20; Transform ainda em X=18,225 no mesmo frame | `C:\BFC\TestResults\Audit-Keeper-probe.xml` |
| PlayMode após ajuste temporal dos testes | **34/34 passaram**, 0 ignorados | `C:\BFC\TestResults\Audit-PlayMode-verified.xml` |

O validador de governança foi executado sem `BASE_SHA`: a verificação do registro passou, mas o próprio script informa que pulou o diff de arquivos protegidos. Não apresentar esse resultado como aprovação de mudanças de regras.

O Python instalado usa um arquivo `python313._pth` e não adiciona o diretório atual ao caminho de importação. O comando convencional de unittest falhou com `ModuleNotFoundError: scripts`. Repetido com inserção explícita da raiz do projeto em `sys.path`, os quatro testes passaram. Trata-se de uma particularidade do ambiente local, sem necessidade demonstrada de modificar o relatório Python.

## Correções locais

1. `FormationLabRuntimeBootstrap.cs`: qualificação de `UnityEngine.Object`, eliminando CS0104.
2. `FormationLabPlaytestController.cs`: import de `BFC.FormationLab`, eliminando CS0246.
3. `FormationLabGoalLine.cs`: import do mesmo namespace, eliminando CS0103.
4. Dois testes PlayMode de posicionamento do goleiro: verificar `Rigidbody.position` imediatamente e `Transform.position` após o próximo passo de física e frame. As restrições de equipe, janela, área e confirmação continuam verificadas.

Nenhum tuning de física, collider, coeficiente, regra de posse ou goleiro foi alterado para fazer os testes passarem.

### Por que os testes do goleiro falharam

A prova adicionou uma asserção da posição física antes da asserção visual original. A primeira passou; a segunda falhou. Isso separa uma aplicação física correta de uma expectativa temporal incorreta do teste. `Rigidbody.position` só publica a posição no Transform após a simulação seguinte, conforme a [documentação Unity](https://docs.unity3d.com/6000.3/Documentation/ScriptReference/Rigidbody-position.html). A interpolação também precisa ser considerada ao observar apresentação. Não foi necessário mover o goleiro por um segundo caminho, remover asserções ou liberar movimento fora da janela.

## Resultado final do PlayMode

A execução após o ajuste temporal passou: **34 testes executados, 34 aprovados, 0 falhas e 0 ignorados**, com exit code 0 e XML verificado. Evidências: `C:\BFC\TestResults\Audit-PlayMode-verified.xml` e `C:\BFC\Logs\audit-playmode-verified.log`.

Com os **25 EditMode**, são **59 testes Unity aprovados** nesta base local. Somam-se quatro testes Python. O resultado é da branch experimental com as correções descritas acima, não da ponta remota original, que não compilava. Os cinco arquivos alterados contêm 13 linhas adicionadas e uma removida; três correções de namespace e dois ajustes de testes. Continuam pendentes os cenários físicos e a avaliação humana descritos abaixo.

## O que estes testes não comprovam

- Física calibrada contra mesa, palheta e bola reais.
- Interação spin + chip: há um problema de apagamento de velocidade Y identificado no código, sem cobertura na suíte existente.
- Rebote na trave, aterrissagem realista, regras completas de todas as modalidades, partida com reposições e fim.
- Desempenho com arte, som, efeitos e renderização finais; não foi repetida a medição histórica de milhares de FPS do harness vazio.
- Funcionamento visual de mouse/câmera/gamepad por avaliação humana, interesse dos jogadores ou procura por cosméticos.
- Fidelidade aos arquivos legados `BFC(1).zip` e `Interface e Menu.zip`, ausentes.
- Build Windows standalone nesta rodada.

## Arte

As três imagens de `art/` foram geradas pela ferramenta integrada ImageGen e inspecionadas. Arquivos e prompts foram preservados no workspace. Elas são propostas de direção artística, sem representar capturas do runtime ou homologação de equipamentos/regras.

## Preservação das evidências

XMLs e logs completos permanecem em `TestResults/` e `Logs/`, ignorados pelo Git. `prs-snapshot.json` guarda a consulta pública dos estados e branches de PRs no momento da auditoria. O relatório distingue código existente, falhas reproduzidas e sugestões; não substitui validação humana para integração ou lançamento.
