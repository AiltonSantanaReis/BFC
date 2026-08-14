using System;
using BFC.Core.Fields;
using BFC.Core.Formations;
using BFC.Core.Matches;
using NUnit.Framework;

namespace BFC.Tests.EditMode
{
    public sealed class FormationFieldTests
    {
        [Test]
        public void LargeFieldEleven_HasApprovedComposition()
        {
            TeamCompositionDefinition composition = TeamCompositionDefinition.LargeFieldEleven;

            Assert.That(composition.TotalPieces, Is.EqualTo(11));
            Assert.That(composition.OutfieldCount, Is.EqualTo(10));
            Assert.That(composition.GoalkeeperCount, Is.EqualTo(1));
        }

        [Test]
        public void Composition_AllowsScenarioSpecificCounts()
        {
            var trainingComposition = new TeamCompositionDefinition(totalPieces: 4, goalkeeperCount: 0);

            Assert.That(trainingComposition.TotalPieces, Is.EqualTo(4));
            Assert.That(trainingComposition.OutfieldCount, Is.EqualTo(4));
            Assert.That(trainingComposition.GoalkeeperCount, Is.Zero);
        }

        [Test]
        public void Formation_RejectsSlotCountMismatch()
        {
            var composition = new TeamCompositionDefinition(totalPieces: 2, goalkeeperCount: 1);
            var slots = new[]
            {
                new FormationSlot("GK", PieceRole.Goalkeeper, -0.8f, 0f)
            };

            Assert.Throws<ArgumentException>(() => new FormationDefinition("invalid", composition, slots));
        }

        [Test]
        public void Formation_RejectsGoalkeeperCountMismatch()
        {
            var composition = new TeamCompositionDefinition(totalPieces: 2, goalkeeperCount: 1);
            var slots = new[]
            {
                new FormationSlot("A", PieceRole.Outfield, -0.4f, 0f),
                new FormationSlot("B", PieceRole.Outfield, 0.4f, 0f)
            };

            Assert.Throws<ArgumentException>(() => new FormationDefinition("invalid", composition, slots));
        }

        [Test]
        public void Formation_RejectsDuplicateSlotIds()
        {
            var composition = new TeamCompositionDefinition(totalPieces: 2, goalkeeperCount: 1);
            var slots = new[]
            {
                new FormationSlot("same", PieceRole.Goalkeeper, -0.8f, 0f),
                new FormationSlot("same", PieceRole.Outfield, 0.2f, 0f)
            };

            Assert.Throws<ArgumentException>(() => new FormationDefinition("invalid", composition, slots));
        }

        [Test]
        public void SpawnPlanner_MirrorsLongitudinalAxisForTeams()
        {
            FormationDefinition formation = CreateTwoPieceFormation();
            FieldDefinition field = CreateField();

            var teamA = FormationSpawnPlanner.CreateSpawns(formation, field, TeamId.TeamA, safetyMargin: 1f);
            var teamB = FormationSpawnPlanner.CreateSpawns(formation, field, TeamId.TeamB, safetyMargin: 1f);

            Assert.That(teamA.Count, Is.EqualTo(teamB.Count));
            for (int index = 0; index < teamA.Count; index++)
            {
                Assert.That(teamB[index].X, Is.EqualTo(-teamA[index].X).Within(0.0001f));
                Assert.That(teamB[index].Z, Is.EqualTo(teamA[index].Z).Within(0.0001f));
                Assert.That(teamB[index].Role, Is.EqualTo(teamA[index].Role));
            }
        }

        [Test]
        public void SpawnPlanner_RespectsSafetyMargin()
        {
            var composition = new TeamCompositionDefinition(totalPieces: 2, goalkeeperCount: 1);
            var formation = new FormationDefinition(
                "edges",
                composition,
                new[]
                {
                    new FormationSlot("GK", PieceRole.Goalkeeper, -1f, -1f),
                    new FormationSlot("P1", PieceRole.Outfield, 1f, 1f)
                });
            FieldDefinition field = CreateField();

            var spawns = FormationSpawnPlanner.CreateSpawns(
                formation,
                field,
                TeamId.TeamA,
                safetyMargin: 1f);

            foreach (FormationSpawn spawn in spawns)
            {
                Assert.That(Math.Abs(spawn.X), Is.LessThanOrEqualTo(field.HalfLength - 1f + 0.0001f));
                Assert.That(Math.Abs(spawn.Z), Is.LessThanOrEqualTo(field.HalfWidth - 1f + 0.0001f));
            }
        }

        [Test]
        public void FieldDefinition_RejectsInvalidGoalGeometry()
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                () => new FieldDefinition(
                    "invalid",
                    length: 20f,
                    width: 10f,
                    goalMouthWidth: 10f,
                    goalDepth: 1f,
                    goalAreaLength: 3f,
                    goalAreaWidth: 10f));
        }

        private static FormationDefinition CreateTwoPieceFormation()
        {
            var composition = new TeamCompositionDefinition(totalPieces: 2, goalkeeperCount: 1);
            return new FormationDefinition(
                "two-piece",
                composition,
                new[]
                {
                    new FormationSlot("GK", PieceRole.Goalkeeper, -0.8f, 0f),
                    new FormationSlot("P1", PieceRole.Outfield, 0.25f, 0.35f)
                });
        }

        private static FieldDefinition CreateField()
        {
            return new FieldDefinition(
                "test-field",
                length: 20f,
                width: 12f,
                goalMouthWidth: 4f,
                goalDepth: 1f,
                goalAreaLength: 3f,
                goalAreaWidth: 6f);
        }
    }
}
