#!/usr/bin/env python3
import json
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]


def require(condition: bool, message: str) -> None:
    if not condition:
        raise SystemExit(message)


def main() -> None:
    project_version = (ROOT / "ProjectSettings/ProjectVersion.txt").read_text(encoding="utf-8")
    require("6000.3.21f1" in project_version, "Unity version must remain pinned to 6000.3.21f1")

    manifest = json.loads((ROOT / "Packages/manifest.json").read_text(encoding="utf-8"))
    dependencies = manifest["dependencies"]
    require(dependencies.get("com.unity.render-pipelines.universal") == "17.3.0", "URP must be 17.3.0")
    require(dependencies.get("com.unity.inputsystem") == "1.20.0", "Input System must be 1.20.0")
    require(dependencies.get("com.unity.test-framework") == "1.6.0", "Test Framework must be 1.6.0")

    required_paths = [
        "Assets/BFC/Scenes/Bootstrap.unity",
        "Assets/BFC/Scenes/PhysicsLab.unity",
        "Assets/BFC/Editor/ProjectSetup/BfcProjectSetup.cs",
        "Assets/BFC/Editor/Build/BfcBuild.cs",
        "Assets/BFC/Physics/PlanarMotionMath.cs",
        "Assets/BFC/Physics/PhysicsBenchmark.cs",
        "Assets/BFC/Physics/PhysicsLabTuning.cs",
        "Assets/BFC/Physics/PlanarKineticBody.cs",
        "Assets/BFC/PhysicsLab/BFC.PhysicsLab.asmdef",
        "Assets/BFC/PhysicsLab/PhysicsLabRuntimeBootstrap.cs",
        "Assets/BFC/Gameplay/Matches/MatchPhase.cs",
        "Assets/BFC/Gameplay/Matches/MatchFinishReason.cs",
        "Assets/BFC/Gameplay/Matches/MatchScore.cs",
        "Assets/BFC/Gameplay/Matches/MatchState.cs",
        "Assets/BFC/Gameplay/Matches/PlayerActionCommand.cs",
        "Assets/BFC/Gameplay/Matches/PlayerActionSubmissionResult.cs",
        "Assets/BFC/Gameplay/Matches/PhysicalActionResolution.cs",
        "Assets/BFC/Gameplay/Matches/MatchDomainEvent.cs",
        "Assets/BFC/Gameplay/Matches/MatchController.cs",
        "Assets/BFC/Core/Fields/FieldDefinition.cs",
        "Assets/BFC/Core/Formations/PieceRole.cs",
        "Assets/BFC/Core/Formations/TeamCompositionDefinition.cs",
        "Assets/BFC/Core/Formations/FormationSlot.cs",
        "Assets/BFC/Core/Formations/FormationDefinition.cs",
        "Assets/BFC/Core/Formations/FormationSpawn.cs",
        "Assets/BFC/Core/Formations/FormationSpawnPlanner.cs",
        "Assets/BFC/Tests/EditMode/BFC.Core.EditMode.Tests.asmdef",
        "Assets/BFC/Tests/EditMode/FormationFieldTests.cs",
        "Assets/BFC/Tests/PlayMode/BFC.Bootstrap.PlayMode.Tests.asmdef",
        "Assets/BFC/Tests/PhysicsEditMode/BFC.Physics.EditMode.Tests.asmdef",
        "Assets/BFC/Tests/PhysicsEditMode/PhysicsBenchmarkTests.cs",
        "Assets/BFC/Tests/GameplayEditMode/BFC.Gameplay.EditMode.Tests.asmdef",
        "Assets/BFC/Tests/GameplayEditMode/MatchControllerTests.cs",
        "ProjectSettings/EditorBuildSettings.asset",
        "scripts/build-windows.ps1",
        "scripts/run-unity-tests.ps1",
        "docs/12-PHASE2_PHYSICS_VERTICAL_SLICE.md",
        "docs/13-PHASE3_MATCH_CORE.md",
        "docs/14-PHASE4_FORMATION_FIELD.md",
        "docs/changes/RFC-0001-phase4-team-composition.md",
        "docs/decisions/ADR-0003-mode-specific-formation-profiles.md",
    ]
    for relative in required_paths:
        require((ROOT / relative).exists(), f"Missing Unity project file: {relative}")

    for asmdef in (ROOT / "Assets/BFC").rglob("*.asmdef"):
        json.loads(asmdef.read_text(encoding="utf-8"))

    for scene_name in ("Bootstrap.unity", "PhysicsLab.unity"):
        scene = (ROOT / "Assets/BFC/Scenes" / scene_name).read_text(encoding="utf-8")
        require(scene.startswith("%YAML 1.1"), f"{scene_name} is not a Unity text scene")
        require("SceneRoots:" in scene, f"{scene_name} has no SceneRoots block")

    bootstrap_scene = (ROOT / "Assets/BFC/Scenes/Bootstrap.unity").read_text(encoding="utf-8")
    require("5a6a5d84f37a4bef93031ec3fc0a9d11" in bootstrap_scene, "Bootstrap scene lost BfcBootstrap reference")

    physics_lab_scene = (ROOT / "Assets/BFC/Scenes/PhysicsLab.unity").read_text(encoding="utf-8")
    require("c7a6e6315b7a4f02a12d914b8c5e60f1" in physics_lab_scene, "PhysicsLab scene lost runtime bootstrap reference")

    build_settings = (ROOT / "ProjectSettings/EditorBuildSettings.asset").read_text(encoding="utf-8")
    require("Assets/BFC/Scenes/Bootstrap.unity" in build_settings, "Bootstrap scene missing from build settings")
    require("Assets/BFC/Scenes/PhysicsLab.unity" in build_settings, "PhysicsLab scene missing from build settings")

    build_script = (ROOT / "Assets/BFC/Editor/Build/BfcBuild.cs").read_text(encoding="utf-8")
    require("BuildTarget.StandaloneWindows64" in build_script, "Windows x64 build target missing")

    core_asmdef = json.loads((ROOT / "Assets/BFC/Core/BFC.Core.asmdef").read_text(encoding="utf-8"))
    require(core_asmdef.get("noEngineReferences") is True, "BFC.Core must remain engine-independent")

    physics_asmdef = json.loads((ROOT / "Assets/BFC/Physics/BFC.Physics.asmdef").read_text(encoding="utf-8"))
    require("Unity.InputSystem" not in physics_asmdef.get("references", []), "BFC.Physics must not depend on input")

    lab_asmdef = json.loads((ROOT / "Assets/BFC/PhysicsLab/BFC.PhysicsLab.asmdef").read_text(encoding="utf-8"))
    lab_refs = lab_asmdef.get("references", [])
    require("BFC.Physics" in lab_refs, "PhysicsLab must reference BFC.Physics")
    require("Unity.InputSystem" in lab_refs, "PhysicsLab must reference the Input System")

    gameplay_asmdef = json.loads((ROOT / "Assets/BFC/Gameplay/BFC.Gameplay.asmdef").read_text(encoding="utf-8"))
    gameplay_refs = gameplay_asmdef.get("references", [])
    require("BFC.Core" in gameplay_refs, "BFC.Gameplay must reference BFC.Core")
    require(gameplay_asmdef.get("noEngineReferences") is True, "BFC.Gameplay must remain engine-independent")
    require("Unity.InputSystem" not in gameplay_refs, "BFC.Gameplay must not depend on the Input System")

    gameplay_tests = json.loads(
        (ROOT / "Assets/BFC/Tests/GameplayEditMode/BFC.Gameplay.EditMode.Tests.asmdef").read_text(encoding="utf-8")
    )
    gameplay_test_refs = gameplay_tests.get("references", [])
    require("BFC.Core" in gameplay_test_refs, "Gameplay EditMode tests must reference BFC.Core")
    require("BFC.Gameplay" in gameplay_test_refs, "Gameplay EditMode tests must reference BFC.Gameplay")

    match_controller = (ROOT / "Assets/BFC/Gameplay/Matches/MatchController.cs").read_text(encoding="utf-8")
    require("MaxActionsPerPossession" in match_controller, "Match Core must source the possession action limit from rules")
    require("MatchPhase.ResolvingAction" in match_controller, "Match Core must represent pending physical resolution")
    require("ResumeAfterRestart" in match_controller, "Match Core must keep restart possession explicit")
    require("DateTime.Now" not in match_controller, "Match Core must not use wall clock directly")

    formation_paths = [
        ROOT / "Assets/BFC/Core/Fields/FieldDefinition.cs",
        ROOT / "Assets/BFC/Core/Formations/PieceRole.cs",
        ROOT / "Assets/BFC/Core/Formations/TeamCompositionDefinition.cs",
        ROOT / "Assets/BFC/Core/Formations/FormationSlot.cs",
        ROOT / "Assets/BFC/Core/Formations/FormationDefinition.cs",
        ROOT / "Assets/BFC/Core/Formations/FormationSpawn.cs",
        ROOT / "Assets/BFC/Core/Formations/FormationSpawnPlanner.cs",
    ]
    for path in formation_paths:
        source = path.read_text(encoding="utf-8")
        require("UnityEngine" not in source, f"Phase 4 domain file must remain engine-independent: {path.name}")

    composition_source = (ROOT / "Assets/BFC/Core/Formations/TeamCompositionDefinition.cs").read_text(encoding="utf-8")
    require("LargeFieldEleven" in composition_source, "Large-field eleven profile is missing")
    require("totalPieces: 11, goalkeeperCount: 1" in composition_source, "Large-field profile must remain 11 total including one goalkeeper")

    rules = json.loads((ROOT / "governance/rules.json").read_text(encoding="utf-8"))["rules"]
    open_001 = next((rule for rule in rules if rule.get("id") == "OPEN-001"), None)
    require(open_001 is not None, "OPEN-001 governance record is missing")
    require(open_001.get("status") == "locked", "OPEN-001 must remain resolved/locked for Phase 4")

    print("Unity Phase 4 structure validation passed.")


if __name__ == "__main__":
    main()
