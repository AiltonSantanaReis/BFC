# BFC Definition of Done

Status: **NORMATIVE QUALITY GATE**

Uma tarefa não está concluída apenas porque compila ou parece funcionar.

## DoD geral

Uma mudança é `DONE` quando, quando aplicável:

- compila sem erro;
- não introduz warning novo não justificado;
- possui testes automatizados adequados;
- respeita dependências entre assemblies;
- não duplica regra em UI/AI/Physics;
- cita IDs de regras afetadas no PR;
- atualiza documentação quando muda contrato;
- possui evidência de não regressão;
- não inclui asset sem origem/licença conhecida para produção;
- funciona com mouse/teclado/gamepad quando o escopo exige;
- não expõe debug ao jogador;
- mantém naming BFC;
- não deixa TODO crítico escondido sem issue/RFC.

## DoD de gameplay

Além do geral:

- cenário possui estado inicial reproduzível;
- resultado esperado está documentado/testado;
- física não depende de FPS de render;
- nenhuma condição produz deadlock de turno;
- ação inválida é rejeitada de forma previsível;
- IA e jogador passam pela mesma validação de regra;
- parâmetros de tuning estão centralizados.

## DoD de regra

- regra possui ID;
- comportamento está na constituição/registro;
- teste cobre happy path;
- teste cobre ao menos um limite/erro relevante;
- mudança de `LOCKED` possui RFC aprovada.

## DoD de UI

- screenshot de referência disponível;
- screenshot Unity disponível;
- layout comparado;
- estados normal/hover/focus/pressed/disabled verificados quando existirem;
- navegação por teclado/gamepad verificada;
- textos não vazam conteúdo técnico;
- nenhuma ação final é mock;
- responsividade mínima aprovada;
- dados exibidos vêm do estado real apropriado.

## DoD de save

- schema versionado;
- round-trip save/load testado;
- dados ausentes/antigos tratados;
- corrupção não destrói silenciosamente o único save válido;
- mudança incompatível possui migration.

## DoD de plataforma

- build real na plataforma alvo;
- input real testado;
- resolução/DPI testados;
- ciclo suspend/resume quando aplicável;
- persistência testada;
- performance medida;
- nenhum recurso crítico depende acidentalmente do Editor.

## Não é DONE

Não considerar concluído quando:

- “funciona no Editor” mas não existe build;
- teste foi desabilitado para passar CI;
- regra foi alterada para caber na implementação;
- diferença visual foi ignorada sem registro;
- mock continua atrás de botão final;
- exceção é engolida sem diagnóstico;
- save foi resetado para evitar migration;
- performance não foi medida onde é requisito;
- foi necessário quebrar outra mecânica para fechar a atual.
