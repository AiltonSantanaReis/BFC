#!/usr/bin/env python3
"""Validate BFC governance files and guard protected normative documents.

The script always validates governance/rules.json. In pull requests it also
requires an RFC when an already-established protected document is modified.
"""

from __future__ import annotations

import json
import os
import subprocess
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
RULES_PATH = ROOT / "governance" / "rules.json"

PROTECTED_PATHS = {
    "docs/00-PRODUCT_CHARTER.md",
    "docs/01-GAMEPLAY_CONSTITUTION.md",
    "docs/02-VISUAL_CONSTITUTION.md",
    "docs/05-CHANGE_CONTROL.md",
    "governance/rules.json",
}

ALLOWED_STATUSES = {"locked", "locked-tunable", "open", "deprecated"}
ALLOWED_CHANGE_AUTHORITIES = {
    "product-owner",
    "product-owner-for-concept",
}


def fail(message: str) -> None:
    print(f"GOVERNANCE ERROR: {message}", file=sys.stderr)
    raise SystemExit(1)


def run_git(*args: str) -> str:
    return subprocess.check_output(
        ["git", *args], cwd=ROOT, text=True, stderr=subprocess.DEVNULL
    ).strip()


def validate_rules() -> None:
    if not RULES_PATH.exists():
        fail("governance/rules.json is missing")

    try:
        payload = json.loads(RULES_PATH.read_text(encoding="utf-8"))
    except json.JSONDecodeError as exc:
        fail(f"rules.json is invalid JSON: {exc}")

    if payload.get("product") != "BFC":
        fail("rules.json product must be BFC")

    rules = payload.get("rules")
    if not isinstance(rules, list) or not rules:
        fail("rules.json must contain a non-empty rules array")

    seen: set[str] = set()
    required = {"id", "category", "status", "title", "statement", "changeAuthority"}

    for index, rule in enumerate(rules):
        if not isinstance(rule, dict):
            fail(f"rule at index {index} must be an object")

        missing = required - set(rule)
        if missing:
            fail(f"rule at index {index} is missing fields: {sorted(missing)}")

        rule_id = str(rule["id"]).strip()
        if not rule_id:
            fail(f"rule at index {index} has an empty id")
        if rule_id in seen:
            fail(f"duplicate rule id: {rule_id}")
        seen.add(rule_id)

        if rule["status"] not in ALLOWED_STATUSES:
            fail(f"{rule_id}: unsupported status {rule['status']!r}")

        if rule["changeAuthority"] not in ALLOWED_CHANGE_AUTHORITIES:
            fail(
                f"{rule_id}: unsupported change authority "
                f"{rule['changeAuthority']!r}"
            )

        if not str(rule["statement"]).strip():
            fail(f"{rule_id}: statement cannot be empty")

    print(f"Governance registry OK: {len(rules)} rules, {len(seen)} unique IDs.")


def path_exists_at(base_sha: str, path: str) -> bool:
    try:
        subprocess.check_call(
            ["git", "cat-file", "-e", f"{base_sha}:{path}"],
            cwd=ROOT,
            stdout=subprocess.DEVNULL,
            stderr=subprocess.DEVNULL,
        )
        return True
    except subprocess.CalledProcessError:
        return False


def validate_protected_changes() -> None:
    base_sha = os.getenv("BASE_SHA", "").strip()
    if not base_sha:
        print("BASE_SHA not set; protected-file diff check skipped.")
        return

    changed = set(run_git("diff", "--name-only", f"{base_sha}...HEAD").splitlines())
    established_protected_changes = {
        path
        for path in (changed & PROTECTED_PATHS)
        if path_exists_at(base_sha, path)
    }

    if not established_protected_changes:
        print("No established protected governance files modified.")
        return

    title = os.getenv("PR_TITLE", "").strip()
    valid_prefix = title.startswith("[RULE CHANGE]") or title.startswith(
        "[GOVERNANCE CHANGE]"
    )
    if not valid_prefix:
        fail(
            "protected governance files changed without PR title prefix "
            "[RULE CHANGE] or [GOVERNANCE CHANGE]: "
            + ", ".join(sorted(established_protected_changes))
        )

    rfc_files = [
        path
        for path in changed
        if path.startswith("docs/changes/RFC-")
        and path != "docs/changes/RFC-TEMPLATE.md"
        and path.endswith(".md")
    ]
    if not rfc_files:
        fail("protected governance change requires a concrete docs/changes/RFC-*.md")

    print("Protected governance change has required PR prefix and RFC file.")


def main() -> None:
    validate_rules()
    validate_protected_changes()


if __name__ == "__main__":
    main()
