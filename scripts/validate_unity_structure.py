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
        "Assets/BFC/Tests/EditMode/BFC.Core.EditMode.Tests.asmdef",
        "Assets/BFC/Tests/PlayMode/BFC.Bootstrap.PlayMode.Tests.asmdef",
        "ProjectSettings/EditorBuildSettings.asset",
        "scripts/build-windows.ps1",
        "scripts/run-unity-tests.ps1",
    ]
    for relative in required_paths:
        require((ROOT / relative).exists(), f"Missing Phase 1 file: {relative}")

    for asmdef in (ROOT / "Assets/BFC").rglob("*.asmdef"):
        json.loads(asmdef.read_text(encoding="utf-8"))

    for scene_name in ("Bootstrap.unity", "PhysicsLab.unity"):
        scene = (ROOT / "Assets/BFC/Scenes" / scene_name).read_text(encoding="utf-8")
        require(scene.startswith("%YAML 1.1"), f"{scene_name} is not a Unity text scene")
        require("SceneRoots:" in scene, f"{scene_name} has no SceneRoots block")

    bootstrap_scene = (ROOT / "Assets/BFC/Scenes/Bootstrap.unity").read_text(encoding="utf-8")
    require("5a6a5d84f37a4bef93031ec3fc0a9d11" in bootstrap_scene, "Bootstrap scene lost BfcBootstrap reference")

    build_settings = (ROOT / "ProjectSettings/EditorBuildSettings.asset").read_text(encoding="utf-8")
    require("Assets/BFC/Scenes/Bootstrap.unity" in build_settings, "Bootstrap scene missing from build settings")
    require("Assets/BFC/Scenes/PhysicsLab.unity" in build_settings, "PhysicsLab scene missing from build settings")

    build_script = (ROOT / "Assets/BFC/Editor/Build/BfcBuild.cs").read_text(encoding="utf-8")
    require("BuildTarget.StandaloneWindows64" in build_script, "Windows x64 build target missing")

    print("Unity Phase 1 structure validation passed.")


if __name__ == "__main__":
    main()
