using System;
using System.Collections.Generic;
using BFC.Core.Fields;
using BFC.Core.Formations;
using BFC.Core.Matches;
using BFC.Physics;
using UnityEngine;
using UnityEngine.Rendering;

namespace BFC.FormationLab
{
    public static class FormationLabRuntimeBuilder
    {
        public const string FixturesName = "Runtime Fixtures";
        public const string FieldSurfaceName = "Field Surface";
        public const string BallName = "Ball";

        private const float SurfaceThickness = 0.16f;
        private const float MarkingThickness = 0.045f;
        private const float GoalHeight = 0.95f;

        public static Transform Build(Transform host)
        {
            if (host == null)
            {
                throw new ArgumentNullException(nameof(host));
            }

            Transform existing = host.Find(FixturesName);
            if (existing != null)
            {
                return existing;
            }

            FieldDefinition field = FormationLabPreviewProfiles.CreateLargeFieldPreview();
            FormationDefinition formation = FormationLabPreviewProfiles.CreateLargeFieldBalancedPreview();

            Camera camera = EnsureCamera();
            EnsureDirectionalLight();

            Transform fixtures = new GameObject(FixturesName).transform;
            fixtures.SetParent(host, false);

            Material fieldMaterial = CreateMaterial(new Color(0.025f, 0.11f, 0.085f));
            Material markingMaterial = CreateMaterial(new Color(0.72f, 0.95f, 1f));
            Material teamAMaterial = CreateMaterial(new Color(0.04f, 0.82f, 1f));
            Material teamBMaterial = CreateMaterial(new Color(1f, 0.07f, 0.66f));
            Material goalkeeperMaterial = CreateMaterial(new Color(0.92f, 0.95f, 1f));
            Material ballMaterial = CreateMaterial(new Color(0.96f, 0.98f, 1f));
            PhysicsMaterial collisionMaterial = CreateCollisionMaterial();

            CreateField(field, fixtures, fieldMaterial, markingMaterial);
            CreateGoals(field, fixtures, markingMaterial);

            float safetyMargin = PhysicsLabTuning.PieceRadius + 0.2f;
            IReadOnlyList<FormationSpawn> teamASpawns = FormationSpawnPlanner.CreateSpawns(
                formation,
                field,
                TeamId.TeamA,
                safetyMargin);
            IReadOnlyList<FormationSpawn> teamBSpawns = FormationSpawnPlanner.CreateSpawns(
                formation,
                field,
                TeamId.TeamB,
                safetyMargin);

            CreateTeam(
                TeamId.TeamA,
                teamASpawns,
                fixtures,
                teamAMaterial,
                goalkeeperMaterial,
                collisionMaterial);
            CreateTeam(
                TeamId.TeamB,
                teamBSpawns,
                fixtures,
                teamBMaterial,
                goalkeeperMaterial,
                collisionMaterial);

            CreateBall(fixtures, ballMaterial, collisionMaterial);

            camera.transform.SetPositionAndRotation(
                new Vector3(0f, 23f, -21f),
                Quaternion.Euler(52f, 0f, 0f));
            camera.fieldOfView = 48f;

            Debug.Log(
                "[BFC FormationLab] Large-field preview materialized: 11 pieces per team " +
                "(10 outfield + 1 goalkeeper). Preview dimensions and tactical layout are tunable, not final rules.");

            return fixtures;
        }

        private static Camera EnsureCamera()
        {
            Camera camera = Camera.main;
            if (camera != null)
            {
                return camera;
            }

            GameObject cameraObject = new GameObject("Main Camera");
            cameraObject.tag = "MainCamera";
            camera = cameraObject.AddComponent<Camera>();
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(0.003f, 0.007f, 0.018f);
            return camera;
        }

        private static void EnsureDirectionalLight()
        {
            Light[] lights = UnityEngine.Object.FindObjectsByType<Light>(FindObjectsSortMode.None);
            for (int index = 0; index < lights.Length; index++)
            {
                if (lights[index].type == LightType.Directional)
                {
                    return;
                }
            }

            GameObject lightObject = new GameObject("Directional Light");
            Light light = lightObject.AddComponent<Light>();
            light.type = LightType.Directional;
            light.color = new Color(0.88f, 0.94f, 1f);
            light.intensity = 1.15f;
            light.shadows = LightShadows.Soft;
            lightObject.transform.rotation = Quaternion.Euler(55f, -35f, 0f);
        }

        private static void CreateField(
            FieldDefinition field,
            Transform parent,
            Material fieldMaterial,
            Material markingMaterial)
        {
            GameObject surface = GameObject.CreatePrimitive(PrimitiveType.Cube);
            surface.name = FieldSurfaceName;
            surface.transform.SetParent(parent, false);
            surface.transform.position = new Vector3(0f, -SurfaceThickness * 0.5f, 0f);
            surface.transform.localScale = new Vector3(field.Length, SurfaceThickness, field.Width);
            surface.GetComponent<Renderer>().sharedMaterial = fieldMaterial;
            surface.GetComponent<Collider>().enabled = false;

            float halfLength = field.HalfLength;
            float halfWidth = field.HalfWidth;
            float lineY = 0.012f;

            CreateMarking("Touchline Near", new Vector3(0f, lineY, -halfWidth), new Vector3(field.Length, 0.02f, MarkingThickness), parent, markingMaterial);
            CreateMarking("Touchline Far", new Vector3(0f, lineY, halfWidth), new Vector3(field.Length, 0.02f, MarkingThickness), parent, markingMaterial);
            CreateMarking("Goal Line A", new Vector3(-halfLength, lineY, 0f), new Vector3(MarkingThickness, 0.02f, field.Width), parent, markingMaterial);
            CreateMarking("Goal Line B", new Vector3(halfLength, lineY, 0f), new Vector3(MarkingThickness, 0.02f, field.Width), parent, markingMaterial);
            CreateMarking("Halfway Line", new Vector3(0f, lineY, 0f), new Vector3(MarkingThickness, 0.02f, field.Width), parent, markingMaterial);

            CreateGoalAreaMarkings(field, TeamId.TeamA, parent, markingMaterial);
            CreateGoalAreaMarkings(field, TeamId.TeamB, parent, markingMaterial);
        }

        private static void CreateGoalAreaMarkings(
            FieldDefinition field,
            TeamId team,
            Transform parent,
            Material material)
        {
            float sign = team == TeamId.TeamA ? -1f : 1f;
            float goalLineX = sign * field.HalfLength;
            float innerX = goalLineX - (sign * field.GoalAreaLength);
            float centerX = (goalLineX + innerX) * 0.5f;
            float halfAreaWidth = field.GoalAreaWidth * 0.5f;
            float lineY = 0.013f;

            CreateMarking(
                $"{team} Goal Area Inner",
                new Vector3(innerX, lineY, 0f),
                new Vector3(MarkingThickness, 0.02f, field.GoalAreaWidth),
                parent,
                material);
            CreateMarking(
                $"{team} Goal Area Near",
                new Vector3(centerX, lineY, -halfAreaWidth),
                new Vector3(field.GoalAreaLength, 0.02f, MarkingThickness),
                parent,
                material);
            CreateMarking(
                $"{team} Goal Area Far",
                new Vector3(centerX, lineY, halfAreaWidth),
                new Vector3(field.GoalAreaLength, 0.02f, MarkingThickness),
                parent,
                material);
        }

        private static void CreateGoals(FieldDefinition field, Transform parent, Material material)
        {
            CreateGoal(field, TeamId.TeamA, parent, material);
            CreateGoal(field, TeamId.TeamB, parent, material);
        }

        private static void CreateGoal(
            FieldDefinition field,
            TeamId team,
            Transform parent,
            Material material)
        {
            float sign = team == TeamId.TeamA ? -1f : 1f;
            float goalLineX = sign * field.HalfLength;
            float rearX = goalLineX + (sign * field.GoalDepth);
            float halfMouth = field.GoalMouthWidth * 0.5f;
            Transform goal = new GameObject($"Goal {team}").transform;
            goal.SetParent(parent, false);

            CreateMarking(
                "Post Near",
                new Vector3(goalLineX, GoalHeight * 0.5f, -halfMouth),
                new Vector3(0.09f, GoalHeight, 0.09f),
                goal,
                material);
            CreateMarking(
                "Post Far",
                new Vector3(goalLineX, GoalHeight * 0.5f, halfMouth),
                new Vector3(0.09f, GoalHeight, 0.09f),
                goal,
                material);
            CreateMarking(
                "Crossbar",
                new Vector3(goalLineX, GoalHeight, 0f),
                new Vector3(0.09f, 0.09f, field.GoalMouthWidth),
                goal,
                material);
            CreateMarking(
                "Rear Bar",
                new Vector3(rearX, 0.05f, 0f),
                new Vector3(0.09f, 0.09f, field.GoalMouthWidth),
                goal,
                material);

            GameObject volume = new GameObject("Goal Volume");
            volume.transform.SetParent(goal, false);
            volume.transform.position = new Vector3((goalLineX + rearX) * 0.5f, GoalHeight * 0.3f, 0f);
            BoxCollider trigger = volume.AddComponent<BoxCollider>();
            trigger.isTrigger = true;
            trigger.size = new Vector3(field.GoalDepth, GoalHeight * 0.6f, field.GoalMouthWidth);
        }

        private static void CreateTeam(
            TeamId team,
            IReadOnlyList<FormationSpawn> spawns,
            Transform parent,
            Material teamMaterial,
            Material goalkeeperMaterial,
            PhysicsMaterial collisionMaterial)
        {
            Transform teamRoot = new GameObject(team.ToString()).transform;
            teamRoot.SetParent(parent, false);

            for (int index = 0; index < spawns.Count; index++)
            {
                FormationSpawn spawn = spawns[index];
                Material visualMaterial = spawn.Role == PieceRole.Goalkeeper
                    ? goalkeeperMaterial
                    : teamMaterial;
                CreatePiece(team, spawn, teamRoot, visualMaterial, collisionMaterial);
            }
        }

        private static void CreatePiece(
            TeamId team,
            FormationSpawn spawn,
            Transform parent,
            Material visualMaterial,
            PhysicsMaterial collisionMaterial)
        {
            GameObject root = new GameObject($"{team} {spawn.PieceId}");
            root.transform.SetParent(parent, false);
            root.transform.position = new Vector3(spawn.X, PhysicsLabTuning.PieceHeight * 0.5f, spawn.Z);

            SphereCollider collider = root.AddComponent<SphereCollider>();
            collider.radius = PhysicsLabTuning.PieceRadius;
            collider.material = collisionMaterial;

            PlanarKineticBody body = root.AddComponent<PlanarKineticBody>();
            body.Configure(
                PhysicsBodyKind.Piece,
                PhysicsLabTuning.PieceMass,
                PhysicsLabTuning.PieceDeceleration,
                PhysicsLabTuning.PieceMaxSpeed,
                PhysicsLabTuning.RestSpeed);

            FormationPieceRuntime identity = root.AddComponent<FormationPieceRuntime>();
            identity.Initialize(team, spawn.PieceId, spawn.Role);

            GameObject visual = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            visual.name = "Visual";
            visual.transform.SetParent(root.transform, false);
            visual.transform.localScale = new Vector3(
                PhysicsLabTuning.PieceRadius,
                PhysicsLabTuning.PieceHeight * 0.5f,
                PhysicsLabTuning.PieceRadius);
            visual.GetComponent<Renderer>().sharedMaterial = visualMaterial;
            visual.GetComponent<Collider>().enabled = false;
        }

        private static void CreateBall(
            Transform parent,
            Material material,
            PhysicsMaterial collisionMaterial)
        {
            GameObject root = new GameObject(BallName);
            root.transform.SetParent(parent, false);
            root.transform.position = new Vector3(0f, PhysicsLabTuning.BallRadius, 0f);

            SphereCollider collider = root.AddComponent<SphereCollider>();
            collider.radius = PhysicsLabTuning.BallRadius;
            collider.material = collisionMaterial;

            PlanarKineticBody body = root.AddComponent<PlanarKineticBody>();
            body.Configure(
                PhysicsBodyKind.Ball,
                PhysicsLabTuning.BallMass,
                PhysicsLabTuning.BallDeceleration,
                PhysicsLabTuning.BallMaxSpeed,
                PhysicsLabTuning.RestSpeed);

            GameObject visual = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            visual.name = "Visual";
            visual.transform.SetParent(root.transform, false);
            visual.transform.localScale = Vector3.one * (PhysicsLabTuning.BallRadius * 2f);
            visual.GetComponent<Renderer>().sharedMaterial = material;
            visual.GetComponent<Collider>().enabled = false;
        }

        private static void CreateMarking(
            string name,
            Vector3 position,
            Vector3 scale,
            Transform parent,
            Material material)
        {
            GameObject marking = GameObject.CreatePrimitive(PrimitiveType.Cube);
            marking.name = name;
            marking.transform.SetParent(parent, false);
            marking.transform.position = position;
            marking.transform.localScale = scale;
            marking.GetComponent<Renderer>().sharedMaterial = material;
            marking.GetComponent<Collider>().enabled = false;
        }

        private static Material CreateMaterial(Color color)
        {
            RenderPipelineAsset pipeline = GraphicsSettings.currentRenderPipeline;
            if (pipeline == null)
            {
                pipeline = GraphicsSettings.defaultRenderPipeline;
            }

            Material template = pipeline != null ? pipeline.defaultMaterial : null;
            if (template == null)
            {
                throw new InvalidOperationException(
                    "FormationLab could not resolve the active render pipeline default material.");
            }

            Material material = new Material(template)
            {
                hideFlags = HideFlags.DontSave
            };
            material.color = color;
            return material;
        }

        private static PhysicsMaterial CreateCollisionMaterial()
        {
            return new PhysicsMaterial("BFC FormationLab No Bounce")
            {
                dynamicFriction = 0f,
                staticFriction = 0f,
                bounciness = 0f,
                frictionCombine = PhysicsMaterialCombine.Minimum,
                bounceCombine = PhysicsMaterialCombine.Minimum,
                hideFlags = HideFlags.DontSave
            };
        }
    }
}