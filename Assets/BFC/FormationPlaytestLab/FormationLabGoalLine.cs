using System;
using BFC.Core.Fields;
using BFC.Core.Matches;
using UnityEngine;

namespace BFC.FormationPlaytestLab
{
    /// <summary>
    /// Laboratory-only, swept whole-ball goal-line test. This is not an
    /// authoritative implementation of any BFC mode's scoring rules.
    /// </summary>
    public static class FormationLabGoalLine
    {
        public static bool TryDetect(
            FieldDefinition field,
            Vector3 previousBallCenter,
            Vector3 currentBallCenter,
            float ballRadius,
            out TeamId scoringTeam)
        {
            if (field == null)
            {
                throw new ArgumentNullException(nameof(field));
            }

            if (float.IsNaN(ballRadius) || float.IsInfinity(ballRadius) || ballRadius <= 0f)
            {
                throw new ArgumentOutOfRangeException(nameof(ballRadius));
            }

            scoringTeam = TeamId.None;
            float usableHalfMouth = (field.GoalMouthWidth * 0.5f) - ballRadius;
            if (usableHalfMouth <= 0f)
            {
                return false;
            }

            float positiveLine = field.HalfLength + ballRadius;
            if (CrossesPlane(previousBallCenter, currentBallCenter, positiveLine, true,
                    usableHalfMouth))
            {
                scoringTeam = TeamId.TeamA;
                return true;
            }

            float negativeLine = -field.HalfLength - ballRadius;
            if (CrossesPlane(previousBallCenter, currentBallCenter, negativeLine, false,
                    usableHalfMouth))
            {
                scoringTeam = TeamId.TeamB;
                return true;
            }

            return false;
        }

        private static bool CrossesPlane(
            Vector3 previous, Vector3 current, float xLine, bool positive,
            float usableHalfMouth)
        {
            bool crossed = positive
                ? previous.x <= xLine && current.x > xLine
                : previous.x >= xLine && current.x < xLine;
            if (!crossed)
            {
                return false;
            }

            float deltaX = current.x - previous.x;
            if (Mathf.Abs(deltaX) <= 0.000001f)
            {
                return false;
            }

            float ratio = (xLine - previous.x) / deltaX;
            float crossingZ = Mathf.Lerp(previous.z, current.z, ratio);
            return Mathf.Abs(crossingZ) <= usableHalfMouth;
        }
    }
}
