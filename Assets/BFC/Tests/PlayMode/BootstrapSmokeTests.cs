using System.Collections;
using BFC.Bootstrap;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace BFC.Tests.PlayMode
{
    public sealed class BootstrapSmokeTests
    {
        [UnityTest]
        public IEnumerator BootstrapComponent_CanInitialize()
        {
            var root = new GameObject("Bootstrap Test Root");
            var bootstrap = root.AddComponent<BfcBootstrap>();

            yield return null;

            Assert.That(bootstrap, Is.Not.Null);
            Assert.That(bootstrap.gameObject.activeInHierarchy, Is.True);
            Object.Destroy(root);
        }
    }
}
