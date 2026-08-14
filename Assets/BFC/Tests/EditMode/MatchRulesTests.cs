using System;
using BFC.Core.Rules;
using NUnit.Framework;

namespace BFC.Tests.EditMode
{
    public sealed class MatchRulesTests
    {
        [Test]
        public void Constructor_RejectsNonPositiveActionLimit()
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                () => new MatchRules(0, automaticGoalkeeper: true, normalizeCompetitiveEquipment: true));
        }

        [Test]
        public void Constructor_PreservesRuleValues()
        {
            var rules = new MatchRules(
                maxActionsPerPossession: 3,
                automaticGoalkeeper: true,
                normalizeCompetitiveEquipment: true);

            Assert.That(rules.MaxActionsPerPossession, Is.EqualTo(3));
            Assert.That(rules.AutomaticGoalkeeper, Is.True);
            Assert.That(rules.NormalizeCompetitiveEquipment, Is.True);
        }
    }
}
