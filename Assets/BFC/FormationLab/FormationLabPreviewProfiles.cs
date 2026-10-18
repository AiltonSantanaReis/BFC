using System;
using BFC.Core.Fields;
using BFC.Core.Formations;

namespace BFC.FormationLab
{
    /// <summary>
    /// Phase 4 preview content only. Numeric field dimensions and this tactical layout are
    /// intentionally not normative competitive values.
    /// </summary>
    /// <summary>
    /// Alternative laboratory field scales. These are experiment presets,
    /// not regulations for any BFC competitive or traditional mode.
    /// </summary>
    public enum FormationLabFieldProfile
    {
        Original = 0,
        Intermediate = 1,
        Tactical = 2,
        Experimental = 3
    }

    public static class FormationLabPreviewProfiles
    {
        public static FieldDefinition CreateLargeFieldPreview()
        {
            // Compatibility alias for existing Phase 4 tests and callers.
            return CreateFieldPreview(FormationLabFieldProfile.Original);
        }

        public static FieldDefinition CreateFieldPreview(FormationLabFieldProfile profile)
        {
            string id;
            float length;
            float width;

            switch (profile)
            {
                case FormationLabFieldProfile.Original:
                    id = "phase4-large-field-preview";
                    length = 28f;
                    width = 18f;
                    break;
                case FormationLabFieldProfile.Intermediate:
                    id = "lab-intermediate-36x23";
                    length = 36f;
                    width = 23f;
                    break;
                case FormationLabFieldProfile.Tactical:
                    id = "lab-tactical-42x27";
                    length = 42f;
                    width = 27f;
                    break;
                case FormationLabFieldProfile.Experimental:
                    id = "lab-experimental-48x31";
                    length = 48f;
                    width = 31f;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(profile), profile, "Unknown laboratory field profile.");
            }

            // Hold goal dimensions constant: first isolate the effect of
            // additional playable space, without making scoring easier.
            return new FieldDefinition(
                id: id,
                length: length,
                width: width,
                goalMouthWidth: 4.4f,
                goalDepth: 1.6f,
                goalAreaLength: 3.5f,
                goalAreaWidth: 8f);
        }

        public static FormationDefinition CreateLargeFieldBalancedPreview()
        {
            TeamCompositionDefinition composition = TeamCompositionDefinition.LargeFieldEleven;
            FormationSlot[] slots =
            {
                new FormationSlot("GK", PieceRole.Goalkeeper, -0.90f, 0f),
                new FormationSlot("D1", PieceRole.Outfield, -0.56f, -0.68f),
                new FormationSlot("D2", PieceRole.Outfield, -0.58f, -0.23f),
                new FormationSlot("D3", PieceRole.Outfield, -0.58f, 0.23f),
                new FormationSlot("D4", PieceRole.Outfield, -0.56f, 0.68f),
                new FormationSlot("M1", PieceRole.Outfield, -0.16f, -0.52f),
                new FormationSlot("M2", PieceRole.Outfield, -0.12f, 0f),
                new FormationSlot("M3", PieceRole.Outfield, -0.16f, 0.52f),
                new FormationSlot("F1", PieceRole.Outfield, 0.28f, -0.56f),
                new FormationSlot("F2", PieceRole.Outfield, 0.34f, 0f),
                new FormationSlot("F3", PieceRole.Outfield, 0.28f, 0.56f)
            };

            return new FormationDefinition(
                "phase4-large-field-balanced-preview",
                composition,
                slots);
        }
    }
}
