# Phase 2 — Physics Vertical Slice

Status: **FIRST RUNTIME SLICE IMPLEMENTED / UNITY VALIDATION PENDING**

## Purpose

Prove the central button-football feel before match rules, full formations, final UI, AI, economy, or production visuals are built.

The Phase 2 scope follows `docs/04-DEVELOPMENT_PLAN.md`: laboratory field, two pieces, one ball, selection, drag/aim, power, impulse, piece×ball and piece×piece collision, slowdown, rest, laboratory boundaries, and benchmark metrics.

## Architecture

- `BFC.Physics` owns planar motion, impulse application, speed bounds, slowdown math, and benchmark math.
- `BFC.PhysicsLab` is a development-only runtime harness. It owns temporary lab geometry, mouse interaction, aim feedback, reset input, and benchmark logging.
- `BFC.Physics` does **not** reference the Input System. The lab assembly depends on both `BFC.Physics` and `Unity.InputSystem`.
- Match rules remain outside the physics layer.

## Controls

- Left click a resting piece to select it.
- Drag opposite the intended travel direction.
- Drag distance maps to normalized power.
- Release to apply an instantaneous planar impulse.
- Press `R` to restore Piece A, Piece B, and Ball to the baseline fixture positions.

## Physics model

Motion is constrained to the XZ plane with Rigidbody constraints. Gravity is disabled. PhysX owns collision resolution. After collisions, `PlanarKineticBody` applies a bounded constant-deceleration rule in `FixedUpdate` and clamps speed to the configured laboratory maximum.

The launch is represented as an instantaneous impulse (`J = m * Δv`) and converted directly to the corresponding planar velocity change. This avoids frame-dependent accumulation of launch force.

## Centralized calibration

All initial Phase 2 values live in `PhysicsLabTuning`. They are **laboratory calibration defaults, not final competitive tuning**.

Current first-pass values include:

- lab field: 17 × 11 units;
- piece radius: 0.55;
- ball radius: 0.24;
- piece mass: 1.0;
- ball mass: 0.35;
- piece slowdown: 4.8 units/s²;
- ball slowdown: 2.4 units/s²;
- maximum launch speed: 8.5 units/s;
- fixed-step stop-distance spread tolerance: 2%.

Changing these numbers later is `TUNING`, not a gameplay-rule decision, unless a normative rule is affected.

## FPS/fixed-step benchmark

`PhysicsBenchmark` simulates the same constant-deceleration step used by runtime motion. The first automated gate compares stop distance at 30 Hz, 60 Hz, and 120 Hz fixed steps and requires relative spread ≤ 2%.

This is the first benchmark gate, not the final proof of full collision determinism. Collision scenarios and render-FPS runs will be added and measured before Phase 2 is closed.

## Explicit non-decisions

Phase 2 does not resolve:

- `OPEN-001` final number of pieces / goalkeeper counting;
- `OPEN-002` final out-of-bounds and restart rules;
- `OPEN-003` fouls, advantage, and penalties.

The four lab boundaries only keep benchmark bodies inside the laboratory. They do not define official match restart rules.

## Validation gate for this slice

Before this first Phase 2 slice is considered validated:

1. Unity `6000.3.21f1` imports with zero compile errors;
2. `PhysicsLab.unity` enters Play Mode and materializes Surface Fixture, Piece A, Ball, Piece B, and four boundaries;
3. drag/aim/release launches a selected stationary piece;
4. piece×ball and piece×piece collisions are observable;
5. bodies decelerate to rest without self-acceleration;
6. EditMode physics benchmark tests pass;
7. existing Phase 1 EditMode/PlayMode tests continue to pass;
8. generated `.meta` files are reviewed and committed.
