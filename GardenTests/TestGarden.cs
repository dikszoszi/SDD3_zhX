using System;
using System.Linq;
using System.Threading;
using FlowerUtils;
using NUnit.Framework;

[assembly: CLSCompliant(false)]
namespace GardenTests
{
    public class TestGarden
    {
        private static readonly Random r = new ();

        [TestCase(1, 1)]
        [TestCase(10000, 20000)]
        public void MovePlayerBehaves(int dx, int dy)
        {
            int width = r.Next(100), heigth = r.Next(100);
            CancellationTokenSource cts = new ();
            Garden g = new (heigth, width, cts);
            int oldX = g.PlayerPosition.X, oldY = g.PlayerPosition.Y;

            g.MovePlayer(dx, dy);

            Assert.That(g.PlayerPosition.X, Is.LessThan(width));
            Assert.That(g.PlayerPosition.Y, Is.LessThan(heigth));
            Assert.That(g.PlayerPosition.X, Is.EqualTo(dx % width + oldX));
            Assert.That(g.PlayerPosition.Y, Is.EqualTo(dy % heigth + oldY));
            cts.Dispose();
        }

        [Test]
        public void PlantFlowerPutsNewSeedInPlayerPosition()
        {
            int width = r.Next(100), heigth = r.Next(100);
            CancellationTokenSource cts = new ();
            Garden g = new (heigth, width, cts);
            g.MovePlayer(r.Next(width), r.Next(heigth));
            g.PlantFlower();
            char[,] field = g.GetField();

            Assert.IsTrue(Flower.Stages.Any(flowr => flowr.Equals(field[g.PlayerPosition.Y, g.PlayerPosition.X])));
            cts.Dispose();
        }

        // TO DO: other tests
    }
}
