using System;
using System.Collections.Generic;

namespace FlowerUtils
{
    public class Flower
    {
        /// <summary>
        /// Iniitializes a new <see cref="Flower"/> and puts it in the first stage of growth.
        /// </summary>
        /// <param name="position"></param>
        public Flower(Position position)
        {
            this.Position = position;
            this.Symbol = Stages[0];
        }

        public Position Position { get; set; }
        public char Symbol { get; set; }

        public static IList<char> Stages => new char[] { '-', '+', 'w', 'W', 'o', 'O', 'X' };

        public bool Grow()
        {
            int currentSymbolIndex = Stages.IndexOf(this.Symbol);
            if (currentSymbolIndex >= Stages.Count - 1) return true;

            this.Symbol = Stages[currentSymbolIndex + 1];
            return false;

            // if (currentSymbolIndex >= Stages.Length - 1) throw new FlowerFullyGrownException("This flower cannot grow more.");
            // if (currentSymbolIndex < 0) throw new InvalidOperationException($"Invalid stage; ({nameof(this.Symbol)} = {this.Symbol}) is not part of {nameof(Flower.Stages)}");
        }
    }
}
