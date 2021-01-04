﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FlowerUtils
{
    public class Garden
    {
        private readonly object fieldLock = new object();
        private readonly object listLock = new object();
        private char[,] field;
        private CancellationTokenSource cts;
        public List<Flower> Flowers { get; private set; }
        public Position PlayerPosition { get; private set; }
        public Position Size
        {
            get
            {
                lock (fieldLock) return new Position(this.field.GetLength(1), this.field.GetLength(0));
            }
        }

        public Garden(int height, int width, CancellationTokenSource cts)
        {
            this.field = new char[height, width];
            this.cts = cts;
            this.Flowers = new List<Flower>();
        }

        /// <summary>
        /// This has to move the player with the specified vector (e.g. dx=0, dy=-1 - move up).
        /// We should not be allowed to step off the playing field (the playing field is the console window).
        /// </summary>
        /// <param name="dx">Vector of the horizontal (X) axis.</param>
        /// <param name="dy">Vector of the vertical (Y) axis.</param>
        public void MovePlayer(int dx, int dy)
        {
            if (dx == 0 || dy == 0) return;
            lock (fieldLock)
            {
                int posX = this.PlayerPosition.X;
                int posY = this.PlayerPosition.Y;
                int height = this.field.GetLength(0);
                int width = this.field.GetLength(1);

                if (dx > 0)
                {
                    if ((posX += dx % width) >= width) posX -= width;
                    //else posX += dx;
                }
                else if (dx < 0)
                {
                    if ((posX += dx % width) < 0) posX += width;
                    //else posX += dx;
                }

                if (dy > 0)
                {
                    if ((posY += dy % height) >= height) posY -= height;
                    //else posY += dy;
                }
                else if (dy < 0)
                {
                    if ((posY += dy % height) < 0) posY += height;
                    //else posY += dy;
                }
                this.PlayerPosition = new Position(posX, posY);
            }
        }

        private void UpdateField()
        {
            lock (fieldLock)
            {
                for (int i = 0; i < this.field.GetLength(0); i++)
                {
                    for (int j = 0; j < this.field.GetLength(1); j++)
                    {
                        this.field[i, j] = '.';
                    }
                }
                lock (listLock)
                {
                    this.Flowers.ForEach(flowr => this.field[flowr.Position.Y, flowr.Position.X] = flowr.Symbol);
                }
            }
        }

        /// <summary>
        /// This puts a new seed down at the location of the player if there is no flower there.
        /// For the new seed, start a new <see cref="Task"/>, that must "grow" the flower, so the character representing the flower should be changing, one change per second.
        /// </summary>
        public void PlantFlower()
        {
            Task task = null;
            UpdateField();
            lock (fieldLock)
            {
                lock (listLock)
                {
                    if (!Flower.Stages.Any(flower => flower.Equals(this.field[this.PlayerPosition.Y, this.PlayerPosition.X])))
                        task = new Task(() => this.GrowFlower(this.cts.Token), this.cts.Token, TaskCreationOptions.LongRunning);
                }
            }
            if (task != null) task.Start();
        }

        /// <summary>
        /// If there is a fully openned flower in the position of the player, then we collect the flower (remove it from the list), otherwise it should throw an exception.
        /// </summary>
        public void CollectFlower()
        {
            try
            {
                lock (fieldLock)
                {
                    char character = this.field[this.PlayerPosition.Y, this.PlayerPosition.X];
                    lock (listLock)
                    {
                        if (Flower.Stages.Any(flower => flower.Equals(character)))
                            if (Flower.Stages.Last().Equals(character))
                                this.Flowers.Remove(this.Flowers.Single(flwr => flwr.Position.Equals(this.PlayerPosition)));
                            else throw new InvalidOperationException($"The flower at ({this.PlayerPosition.X},{this.PlayerPosition.Y}) is {this.field[this.PlayerPosition.Y, this.PlayerPosition.X]} thus not fully openned.");
                    }
                }
            }
            finally
            {
                UpdateField();
            }

        }

        /// <summary>
        /// This should ask the tasks to stop, and it should wait for the tasks to complete.
        /// </summary>
        public void CancelAll()
        {
            this.cts.Cancel();
        }

        private void GrowFlower(CancellationToken ct)
        {
            Flower newFlower = new Flower(this.PlayerPosition);
            lock (listLock) this.Flowers.Add(newFlower);
            Thread.Sleep(1000);
            bool isFinished = false;

            while (!ct.IsCancellationRequested)
            {
                lock (listLock) isFinished = this.Flowers.Single(flwr => flwr.Position.Equals(newFlower.Position)).Grow();
                Thread.Sleep(1000);
                UpdateField();
                if (isFinished) break;
            }

        }

        public char[,] GetField()
        {
            Thread.Sleep(1500);
            UpdateField();
            lock (fieldLock) return this.field;
        }

        public override string ToString()
        {
            UpdateField();
            string output = string.Empty;
            lock (fieldLock)
            {
                for (int i = 0; i < this.field.GetLength(0); i++)
                {
                    for (int j = 0; j < this.field.GetLength(1); j++)
                    {
                        output += this.field[i, j];
                    }
                    output += "\n";
                }
            }
            return output;
        }
    }
}
