using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using BFC.Core.Fields;
using BFC.Core.Matches;
using UnityEngine;

namespace BFC.FormationPlaytestLab
{
    /// <summary>
    /// Observed event at the end of a laboratory action; NEVER a rule/possession verdict.
    /// Distances use project world units in the XZ field plane.
    /// </summary>
    public enum TacticalActionEnd
    {
        Settled = 0,
        Goal = 1,
        OutOfBounds = 2
    }

    public sealed class TacticalActionRecord
    {
        internal TacticalActionRecord(
            int sequence, string fieldId, float fieldLength, float fieldWidth,
            TeamId actor, string pieceId, float power, Vector3 start, Vector3 end,
            float traveled, float maximumForward, float maximumDisplacement,
            int contacts, int friendlyContacts, int opponentContacts, int uniquePieces,
            TeamId lastContactTeam, string lastContactPiece, TacticalActionEnd outcome)
        {
            Sequence = sequence;
            FieldId = fieldId;
            FieldLength = fieldLength;
            FieldWidth = fieldWidth;
            ActingTeam = actor;
            LaunchedPieceId = pieceId;
            NormalizedPower = power;
            BallStart = start;
            BallEnd = end;
            BallPathLength = traveled;
            MaximumForwardProgress = maximumForward;
            MaximumDisplacement = maximumDisplacement;
            ContactEvents = contacts;
            FriendlyContactEvents = friendlyContacts;
            OpponentContactEvents = opponentContacts;
            UniqueContactedPieces = uniquePieces;
            LastContactTeam = lastContactTeam;
            LastContactPieceId = lastContactPiece;
            Outcome = outcome;
        }

        public int Sequence { get; }
        public string FieldId { get; }
        public float FieldLength { get; }
        public float FieldWidth { get; }
        public TeamId ActingTeam { get; }
        public string LaunchedPieceId { get; }
        public float NormalizedPower { get; }
        public Vector3 BallStart { get; }
        public Vector3 BallEnd { get; }
        public float BallPathLength { get; }
        public float MaximumForwardProgress { get; }
        public float MaximumDisplacement { get; }
        public int ContactEvents { get; }
        public int FriendlyContactEvents { get; }
        public int OpponentContactEvents { get; }
        public int UniqueContactedPieces { get; }
        public TeamId LastContactTeam { get; }
        public string LastContactPieceId { get; }
        public TacticalActionEnd Outcome { get; }

        public float NetForwardProgress =>
            (BallEnd.x - BallStart.x) * (ActingTeam == TeamId.TeamA ? 1f : -1f);

        public float PathToFieldLengthRatio => BallPathLength / FieldLength;

        public float MaxForwardToFieldLengthRatio => MaximumForwardProgress / FieldLength;
    }

    /// <summary>
    /// Action telemetry is deliberately observational: an OnCollisionEnter event
    /// is not possession, a legal pass, or a scoring opportunity.
    /// </summary>
    public sealed class TacticalActionTelemetry
    {
        private const int MaxStoredRecords = 500;
        private readonly FieldDefinition _field;
        private readonly List<TacticalActionRecord> _records = new List<TacticalActionRecord>();
        private readonly HashSet<string> _uniqueContacts = new HashSet<string>();

        private bool _active;
        private int _nextSequence = 1;
        private TeamId _team;
        private string _pieceId;
        private float _power;
        private Vector3 _start;
        private Vector3 _previous;
        private float _path;
        private float _maximumForward;
        private float _maximumDisplacement;
        private int _contacts;
        private int _friendlyContacts;
        private int _opponentContacts;
        private TeamId _lastContactTeam;
        private string _lastContactPiece;

        public TacticalActionTelemetry(FieldDefinition field)
        {
            _field = field ?? throw new ArgumentNullException(nameof(field));
        }

        public bool IsRecording => _active;
        public IReadOnlyList<TacticalActionRecord> Records => _records.AsReadOnly();
        public TacticalActionRecord LastRecord => _records.Count == 0 ? null : _records[_records.Count - 1];

        public void Begin(TeamId team, string pieceId, float normalizedPower, Vector3 ballPosition)
        {
            if (_active) throw new InvalidOperationException("An action is already being recorded.");
            if (team != TeamId.TeamA && team != TeamId.TeamB)
                throw new ArgumentOutOfRangeException(nameof(team));
            if (string.IsNullOrWhiteSpace(pieceId))
                throw new ArgumentException("Piece id is required.", nameof(pieceId));
            if (float.IsNaN(normalizedPower) || float.IsInfinity(normalizedPower) ||
                normalizedPower < 0f || normalizedPower > 1f)
                throw new ArgumentOutOfRangeException(nameof(normalizedPower));
            EnsureFinite(ballPosition, nameof(ballPosition));

            _team = team;
            _pieceId = pieceId;
            _power = normalizedPower;
            _start = ballPosition;
            _previous = ballPosition;
            _path = 0f;
            _maximumForward = 0f;
            _maximumDisplacement = 0f;
            _contacts = 0;
            _friendlyContacts = 0;
            _opponentContacts = 0;
            _lastContactTeam = TeamId.None;
            _lastContactPiece = string.Empty;
            _uniqueContacts.Clear();
            _active = true;
        }

        public void ObserveBallPosition(Vector3 position)
        {
            if (!_active) return;
            EnsureFinite(position, nameof(position));

            float dx = position.x - _previous.x;
            float dz = position.z - _previous.z;
            _path += Mathf.Sqrt(dx * dx + dz * dz);
            _previous = position;

            float fullDx = position.x - _start.x;
            float fullDz = position.z - _start.z;
            _maximumDisplacement = Mathf.Max(_maximumDisplacement,
                Mathf.Sqrt(fullDx * fullDx + fullDz * fullDz));

            float signedForward = fullDx * (_team == TeamId.TeamA ? 1f : -1f);
            _maximumForward = Mathf.Max(_maximumForward, signedForward);
        }

        public void ObserveBallContact(TeamId team, string pieceId)
        {
            if (!_active || (team != TeamId.TeamA && team != TeamId.TeamB) ||
                string.IsNullOrWhiteSpace(pieceId))
            {
                return;
            }

            _contacts++;
            if (team == _team) _friendlyContacts++;
            else _opponentContacts++;
            _lastContactTeam = team;
            _lastContactPiece = pieceId;
            _uniqueContacts.Add(((int)team).ToString(CultureInfo.InvariantCulture) + ":" + pieceId);
        }

        public TacticalActionRecord Complete(TacticalActionEnd outcome, Vector3 finalBallPosition)
        {
            if (!_active) throw new InvalidOperationException("No active action.");
            if (!Enum.IsDefined(typeof(TacticalActionEnd), outcome))
                throw new ArgumentOutOfRangeException(nameof(outcome));
            ObserveBallPosition(finalBallPosition);

            var record = new TacticalActionRecord(
                _nextSequence++, _field.Id, _field.Length, _field.Width,
                _team, _pieceId, _power, _start, finalBallPosition,
                _path, _maximumForward, _maximumDisplacement, _contacts,
                _friendlyContacts, _opponentContacts, _uniqueContacts.Count,
                _lastContactTeam, _lastContactPiece, outcome);
            if (_records.Count == MaxStoredRecords) _records.RemoveAt(0);
            _records.Add(record);
            _active = false;
            return record;
        }

        /// <summary>Aborted shots are not counted as successful/completed samples.</summary>
        public void CancelActive()
        {
            _active = false;
            _uniqueContacts.Clear();
        }

        public string ToCsv()
        {
            var builder = new StringBuilder();
            builder.AppendLine("sequence,field_id,field_length,field_width,team,piece,power,start_x,start_z,end_x,end_z,ball_path,maximum_displacement,max_forward_progress,net_forward_progress,path_field_ratio,contacts,friendly_contacts,opponent_contacts,unique_contacted_pieces,last_contact_team,last_contact_piece,outcome");
            foreach (TacticalActionRecord item in _records)
            {
                builder.Append(item.Sequence).Append(',')
                    .Append(Csv(item.FieldId)).Append(',')
                    .Append(Number(item.FieldLength)).Append(',')
                    .Append(Number(item.FieldWidth)).Append(',')
                    .Append(item.ActingTeam).Append(',')
                    .Append(Csv(item.LaunchedPieceId)).Append(',')
                    .Append(Number(item.NormalizedPower)).Append(',')
                    .Append(Number(item.BallStart.x)).Append(',')
                    .Append(Number(item.BallStart.z)).Append(',')
                    .Append(Number(item.BallEnd.x)).Append(',')
                    .Append(Number(item.BallEnd.z)).Append(',')
                    .Append(Number(item.BallPathLength)).Append(',')
                    .Append(Number(item.MaximumDisplacement)).Append(',')
                    .Append(Number(item.MaximumForwardProgress)).Append(',')
                    .Append(Number(item.NetForwardProgress)).Append(',')
                    .Append(Number(item.PathToFieldLengthRatio)).Append(',')
                    .Append(item.ContactEvents).Append(',')
                    .Append(item.FriendlyContactEvents).Append(',')
                    .Append(item.OpponentContactEvents).Append(',')
                    .Append(item.UniqueContactedPieces).Append(',')
                    .Append(item.LastContactTeam).Append(',')
                    .Append(Csv(item.LastContactPieceId)).Append(',')
                    .Append(item.Outcome)
                    .AppendLine();
            }

            return builder.ToString();
        }

        private static string Number(float value) =>
            value.ToString("0.######", CultureInfo.InvariantCulture);

        private static string Csv(string text) =>
            "\"" + (text ?? string.Empty).Replace("\"", "\"\"") + "\"";

        private static void EnsureFinite(Vector3 vector, string argument)
        {
            if (float.IsNaN(vector.x) || float.IsNaN(vector.y) || float.IsNaN(vector.z) ||
                float.IsInfinity(vector.x) || float.IsInfinity(vector.y) ||
                float.IsInfinity(vector.z))
                throw new ArgumentOutOfRangeException(argument);
        }
    }
}
