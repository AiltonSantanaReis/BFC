using System.Collections;
using System.Linq;
using BFC.Core.Fields;
using BFC.Core.Formations;
using BFC.FormationLab;
using BFC.Physics;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace BFC.Tests.PlayMode
{
    public sealed class FormationLabSmokeTests
    {
        [TestCase(FormationLabFieldProfile.Original, 28f, 18f)]
        [TestCase(FormationLabFieldProfile.Intermediate, 36f, 23f)]
        [TestCase(FormationLabFieldProfile.Tactical, 42f, 27f)]
        [TestCase(FormationLabFieldProfile.Experimental, 48f, 31f)]
        public void FieldProfiles_ChangeOnlyThePlayableScale(
            FormationLabFieldProfile profile, float length, float width)
        {
            FieldDefinition field = FormationLabPreviewProfiles.CreateFieldPreview(profile);

            Assert.That(field.Length, Is.EqualTo(length));
            Assert.That(field.Width, Is.EqualTo(width));
            Assert.That(field.GoalMouthWidth, Is.EqualTo(4.4f));
            Assert.That(field.GoalAreaLength, Is.EqualTo(3.5f));
            Assert.That(field.GoalAreaWidth, Is.EqualTo(8f));
        }

        [Test]
        public void FieldProfiles_RejectUnknownValues()
        {
            Assert.Throws<System.ArgumentOutOfRangeException>(() =>
                FormationLabPreviewProfiles.CreateFieldPreview((FormationLabFieldProfile)999));
        }

        [UnityTest]
        public IEnumerator TacticalFieldPreviewMaterializesExpectedDimensionsAndTeamCount()
        {
            GameObject host = new GameObject("Tactical Field Test Host");
            Transform fixtures = FormationLabRuntimeBuilder.Build(
                host.transform, FormationLabFieldProfile.Tactical);

            yield return null;

            Transform surface = fixtures.Find(FormationLabRuntimeBuilder.FieldSurfaceName);
            Assert.That(surface, Is.Not.Null);
            Assert.That(surface.localScale.x, Is.EqualTo(42f).Within(0.001f));
            Assert.That(surface.localScale.z, Is.EqualTo(27f).Within(0.001f));

            FormationPieceRuntime[] pieces = fixtures.GetComponentsInChildren<FormationPieceRuntime>();
            Assert.That(pieces.Length, Is.EqualTo(22));
            Assert.That(pieces.Count(piece => piece.Role == PieceRole.Goalkeeper), Is.EqualTo(2));

            Object.Destroy(host);
            yield return null;
        }

        [UnityTest]
        public IEnumerator LargeFieldPreviewMaterializesTwentyTwoPiecesAndBall()
        {
            GameObject host = new GameObject("FormationLab Test Host");
            Transform fixtures = FormationLabRuntimeBuilder.Build(host.transform);

            yield return null;

            Assert.That(fixtures, Is.Not.Null);
            Assert.That(fixtures.name, Is.EqualTo(FormationLabRuntimeBuilder.FixturesName));
            Assert.That(fixtures.Find(FormationLabRuntimeBuilder.FieldSurfaceName), Is.Not.Null);
            Assert.That(fixtures.Find(FormationLabRuntimeBuilder.BallName), Is.Not.Null);

            FormationPieceRuntime[] pieces = fixtures.GetComponentsInChildren<FormationPieceRuntime>();
            Assert.That(pieces.Length, Is.EqualTo(22));
            Assert.That(pieces.Count(piece => piece.Role == PieceRole.Goalkeeper), Is.EqualTo(2));
            Assert.That(pieces.Count(piece => piece.Role == PieceRole.Outfield), Is.EqualTo(20));
            Assert.That(pieces.All(piece => piece.transform.localScale == Vector3.one), Is.True);

            PlanarKineticBody[] bodies = fixtures.GetComponentsInChildren<PlanarKineticBody>();
            Assert.That(bodies.Length, Is.EqualTo(23));
            Assert.That(bodies.Count(body => body.Kind == PhysicsBodyKind.Ball), Is.EqualTo(1));

            Object.Destroy(host);
            yield return null;
        }
    }
}
