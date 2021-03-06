﻿using System;

namespace FlowerUtils
{
    /// <summary>
    /// Represents coordinates in 2-D Cartesian coordinate system.
    /// </summary>
    public struct Position : IEquatable<Position>
    {
        /// <summary>
        /// Position on the horizontal axis.
        /// </summary>
        public int X { get; private set; }
        /// <summary>
        /// Position on the vertical axis.
        /// </summary>
        public int Y { get; private set; }

        /// <summary>
        /// Initializes a new instance of <see cref="Position"/>.
        /// </summary>
        /// <param name="X">Horizontal position</param>
        /// <param name="Y">Vertical position</param>
        public Position(int X, int Y)
        {
            this.X = X;
            this.Y = Y;
        }

        public override bool Equals(object obj)
        {
            return obj is Position other
                && this.X == other.X
                && this.Y == other.Y;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y);
        }

        public static bool operator ==(Position left, Position right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Position left, Position right)
        {
            return !(left == right);
        }

        public bool Equals(Position other)
        {
            return this.X == other.X && this.Y == other.Y;
        }
    }
}
