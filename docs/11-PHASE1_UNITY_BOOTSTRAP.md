# Phase 1 — Unity Bootstrap

Status: **IMPLEMENTED IN REPOSITORY / EDITOR EXECUTION PENDING**

## Purpose

Turn the approved BFC foundation into an actual Unity 6.3 LTS project skeleton without introducing gameplay changes or visual redesign.

## Pinned toolchain

- Unity: `6000.3.21f1`
- Universal Render Pipeline: `17.3.0`
- Input System: `1.20.0`
- Test Framework: `1.6.0`
- Primary player target: Windows x64

## Repository deliverables

- `Bootstrap.unity`: first build scene and composition-root host.
- `PhysicsLab.unity`: isolated laboratory scene reserved for Phase 2 physics benchmarking.
- `BfcProjectSetup`: idempotent Editor setup that creates and assigns the BFC URP asset on first Editor open.
- `BfcBuild`: deterministic Windows x64 build entry point.
- EditMode and PlayMode test assemblies.
- PowerShell entry points for tests and Windows build.
- Static CI validation of the Unity project structure.

## URP bootstrap behavior

The repository deliberately does **not** hand-author version-sensitive URP `.asset` serialization. On the first successful Unity Editor load, `BfcProjectSetup` creates `Assets/BFC/Settings/BFC_URP.asset` and its built-in Universal Renderer through Unity's own API, then assigns the pipeline in Graphics Settings. This prevents a stale or malformed hand-written render-pipeline asset.

The generated URP assets must be committed after the first successful Editor validation so all future machines share the same serialized pipeline configuration.

## Windows build

Local build command from PowerShell:

```powershell
./scripts/build-windows.ps1
```

Expected output:

```text
Builds/Windows/BFC.exe
```

The build script calls `BfcProjectSetup` first and builds only the `Bootstrap` scene. `PhysicsLab` is intentionally excluded from the player build.

## Tests

Run:

```powershell
./scripts/run-unity-tests.ps1
```

This runs EditMode followed by PlayMode tests and writes results under `TestResults/`.

## Acceptance gate

Phase 1 is fully complete only when all items below are true on a machine with Unity `6000.3.21f1` installed and licensed:

1. project opens with no compile errors;
2. package resolution produces a stable `Packages/packages-lock.json`;
3. URP assets are generated and assigned;
4. `Bootstrap.unity` opens cleanly;
5. `PhysicsLab.unity` opens cleanly;
6. EditMode tests pass;
7. PlayMode tests pass;
8. `scripts/build-windows.ps1` produces a launchable `Builds/Windows/BFC.exe`;
9. generated Unity assets/settings are reviewed and committed;
10. no gameplay rule or Interface-and-Menu visual rule changed during bootstrap.

## Current execution limitation

The repository changes were produced from an environment without the Unity Editor binary or a Unity license. Therefore no claim is made that a Windows `.exe`, `packages-lock.json`, or generated URP asset has already been produced in this environment. The build/test commands are implemented and reproducible, but the Editor-dependent acceptance items remain pending until executed with the pinned Unity Editor.

This distinction is intentional: a build is not considered successful until Unity itself produces the player and reports `BuildResult.Succeeded`.
