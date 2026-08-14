namespace BFC.Gameplay.Matches
{
    public enum PlayerActionRejectionReason
    {
        None = 0,
        MatchNotStarted = 1,
        MatchFinished = 2,
        PhysicalResolutionPending = 3,
        RestartPending = 4,
        TeamNotInPossession = 5
    }

    public readonly struct PlayerActionSubmissionResult
    {
        private PlayerActionSubmissionResult(bool accepted, PlayerActionRejectionReason rejectionReason)
        {
            Accepted = accepted;
            RejectionReason = rejectionReason;
        }

        public bool Accepted { get; }

        public PlayerActionRejectionReason RejectionReason { get; }

        public static PlayerActionSubmissionResult Accept()
        {
            return new PlayerActionSubmissionResult(true, PlayerActionRejectionReason.None);
        }

        public static PlayerActionSubmissionResult Reject(PlayerActionRejectionReason reason)
        {
            return new PlayerActionSubmissionResult(false, reason);
        }
    }
}
