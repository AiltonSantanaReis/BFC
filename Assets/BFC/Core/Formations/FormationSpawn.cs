namespace BFC.Core.Formations
{
    /// <summary>
    /// Engine-independent world-space spawn coordinate on the gameplay XZ plane.
    /// </summary>
    public readonly struct FormationSpawn
    {
        public FormationSpawn(string pieceId, PieceRole role, float x, float z)
        {
            PieceId = pieceId;
            Role = role;
            X = x;
            Z = z;
        }

        public string PieceId { get; }

        public PieceRole Role { get; }

        public float X { get; }

        public float Z { get; }
    }
}
