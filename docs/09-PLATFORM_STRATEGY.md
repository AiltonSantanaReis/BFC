# BFC Platform Strategy

Status: **STRATEGIC BASELINE**

## 1. Plataforma primária

**Windows PC** é o primeiro target de produção.

Prioridades iniciais:

- mouse;
- teclado;
- controle Xbox-compatible;
- 16:9;
- fullscreen/windowed/borderless conforme configuração final;
- múltiplas resoluções;
- 60 Hz e altas taxas de atualização;
- save local confiável.

## 2. Engine e renderer

- Unity 6.3 LTS;
- C#;
- URP;
- física Unity utilizada por uma camada de gameplay controlada;
- UI reconstruída em Unity sem dependência de browser/webview.

## 3. Input abstraction

Gameplay não deve consultar teclas/botões físicos diretamente fora da camada de input.

Ações lógicas incluem, conforme sistema:

- Navigate;
- Confirm;
- Cancel;
- SelectPiece;
- Aim;
- Power;
- Shoot;
- Pass;
- Spin;
- Chip;
- Pause;
- Camera.

Bindings podem variar por plataforma sem mudar regra.

## 4. Plataforma secundária

Após Windows estável, avaliar por business case e QA:

- Web;
- Linux;
- macOS;
- Android;
- iOS.

A arquitetura deve evitar dependência desnecessária de Win32, mas Windows permanece prioridade inicial.

## 5. Mobile

Mobile exige projeto de input/layout próprio, não apenas build.

Requisitos incluem:

- touch;
- landscape para partida se confirmado;
- safe areas;
- performance térmica;
- suspensão/retorno;
- UI adaptativa;
- legibilidade;
- armazenamento da plataforma.

A identidade visual permanece BFC, mas layout pode adaptar a proporção sem fingir pixel parity com desktop.

## 6. Consoles

Unity torna a arquitetura mais adequada a um futuro port de console, mas suporte técnico da engine não equivale a direito de publicar.

PlayStation, Xbox e Nintendo dependem de:

- aprovação nos programas de desenvolvedor;
- SDKs fechados;
- hardware/devkits conforme fabricante;
- compliance/certificação;
- QA específico;
- eventual plano/licença Unity aplicável.

Nenhum milestone atual promete console até esses requisitos existirem.

## 7. Determinismo e multiplayer futuro

A arquitetura deve evitar decisões que tornem impossível um futuro modo online, mas o projeto inicial não assume que PhysX sozinho será bitwise deterministic entre todas as plataformas.

Se multiplayer competitivo online se tornar requisito:

- servidor autoritativo;
- modelo de sincronização;
- anti-cheat;
- rollback/lockstep ou outra estratégia;
- economia/ranking server-side;

exigirão ADR/RFC específicos.

## 8. Save

Core conhece contrato de persistência, não caminho de arquivo.

Infrastructure decide implementação por plataforma.

## 9. Política de port

Nenhum `#if PLATFORM` deve invadir regras centrais sem justificativa extrema.

Diferenças de plataforma ficam preferencialmente em adapters e composition root.
