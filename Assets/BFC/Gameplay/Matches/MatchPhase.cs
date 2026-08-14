namespace BFC.Gameplay.Matches
{
    /// <summary>
    /// Authoritative logical phase of a match. Presentation state must not be encoded here.
    /// </summary>
    public enum MatchPhase
    {
        NotStarted = 0,
        AwaitingAction = 1,
        ResolvingAction = 2,
        AwaitingRestart = 3,
        Finished = 4
    }
}
