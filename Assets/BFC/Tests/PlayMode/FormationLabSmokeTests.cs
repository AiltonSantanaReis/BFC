using System.Collections;
using System.Linq;
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
