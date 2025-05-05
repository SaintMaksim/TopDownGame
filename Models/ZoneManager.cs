using System.Collections.Generic;
using System.Drawing;

namespace TopDownGame.Models
{
    public class ZoneManager
    {
        public enum Direction { Up, Right, Down, Left }

        private readonly Dictionary<ZoneType, Dictionary<Direction, ZoneTransition>> _transitions = new();

        public void AddTransition(ZoneType from, ZoneType to, Direction direction)
        {
            if (!_transitions.ContainsKey(from))
                _transitions[from] = new Dictionary<Direction, ZoneTransition>();
            _transitions[from][direction] = new ZoneTransition(to, Point.Empty);
        }

        public ZoneTransition GetTransition(ZoneType currentZone, Direction direction, Point currentPosition)
        {
            if (_transitions.TryGetValue(currentZone, out var directions) &&
                directions.TryGetValue(direction, out var transition))
            {
                var spawnPos = GetDefaultSpawnPosition(direction, currentPosition);
                return new ZoneTransition(transition.TargetZone, spawnPos);
            }

            return null;
        }

        private Point GetDefaultSpawnPosition(Direction direction, Point previousPosition)
        {
            return direction switch
            {
                Direction.Right => new Point(5, previousPosition.Y),
                Direction.Left => new Point(795, previousPosition.Y),
                Direction.Up => new Point(previousPosition.X, 595),
                Direction.Down => new Point(previousPosition.X, 5),
                _ => previousPosition
            };
        }
    }

    public class ZoneTransition
    {
        public ZoneType TargetZone { get; }
        public Point SpawnPosition { get; }

        public ZoneTransition(ZoneType targetZone, Point spawnPosition)
        {
            TargetZone = targetZone;
            SpawnPosition = spawnPosition;
        }
    }
}