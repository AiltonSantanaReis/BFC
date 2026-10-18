# BFC — Field-scale experiment (not a new ruleset)

**Status:** experimental; not approved competitive geometry.

## Hypothesis

The player's sense of territory and satisfaction from a constructed play
depends on the relationship between the field dimensions, piece movement,
ball travel, and the number of legal actions per possession — not on field
size in isolation. We want players to see open lanes and choose intentional
passes, rather than defaulting to a maximum-power shot.

## Available profiles

| Profile | Field length | Field width | Surface area vs. Original |
| --- | ---: | ---: | ---: |
| Original | 28 | 18 | 1.00× |
| Intermediate | 36 | 23 | 1.64× |
| Tactical | 42 | 27 | 2.25× |
| Experimental | 48 | 31 | 2.95× |

All dimensions use the project's laboratory world units. Goal mouth, goal
depth, goal area, piece dimensions, ball dimensions, physics coefficients,
and team composition remain unchanged across these four presets. No
traditional or competitive mode is changed by this experiment.

## How to compare

- Open **FormationLab** in Unity. With no argument, the Original
  preview remains the default.
- For an alternate preset, start the Unity Editor with the argument
  `-bfcFieldProfile=Tactical` (also `Original`, `Intermediate`, or
  `Experimental`), then run the FormationLab scene. This is an
  **Editor process argument**, not a command typed inside the Game view.
- As a second option, attach **FormationLabRuntimeBootstrap** to an
  object in the FormationLab scene and change its **Default Field Profile**
  in the Inspector before entering Play Mode. The automatic bootstrap
  will not attach another instance if one already exists.
- The camera backs away proportionally as the field expands so the
  entire field remains inspectable. This camera is *not* the planned
  immersive camera system.

## Observations to record (manual, not yet instrumented)

1. Does the player consider at least two plausible routes to progress?
2. How many actions and how much real time does a normal goal opportunity
   require under each existing ruleset?
3. In real piece-ball collisions, how far does the ball travel after a
   typical strike and a maximum-strength strike? Record median and P95.
4. What fraction of attempts from the defensive half become direct,
   repeatable shots on goal?
5. Does the extra territory create meaningful passes and positional
   defense, or just additional dead time?

The current FormationLab is still a non-interactive formation/physics
preview; none of these gameplay outcomes has been validated by this
change. Do **not** choose a final field size based only on the new visual.

## Technical validation

Run the Unity Test Runner in PlayMode. Tests verify all four profile
dimensions, constant goal geometry, rejection of unknown presets, and
materialization of 22 pieces on the Tactical field. Existing default
formation tests should continue to pass.

Next milestone: connect inputs, physics and match state in a playable
field lab, then measure distance distributions from real collisions and
player decisions. Reevaluate the engine only after that evidence.
