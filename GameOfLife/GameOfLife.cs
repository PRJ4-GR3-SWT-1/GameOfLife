using System;

namespace GameOfLife
{
    class GameOfLife
    {
        private readonly int _gridSize;
        private const int AliveThreshold = 25;
        private bool[,] _curGrid;
        private bool[,] _newGrid;


        public GameOfLife(int gridSize)
        {
            _gridSize = gridSize;
            _curGrid = new bool[_gridSize, _gridSize];
            _newGrid = new bool[_gridSize, _gridSize];

            var rnd = new Random();
            for (var row = 0; row < _gridSize; row++)
            {
                for (var col = 0; col < _gridSize; col++)
                {
                    _curGrid[row, col] = (rnd.Next(100) < AliveThreshold) ? true : false;
                }
            }
        }


        public void Run(int numberOfIterations)
        {
            while (numberOfIterations > 0)
            {
                var locationsAlive = 0;
                for (var row = 0; row < _gridSize; row++)
                {
                    for (var col = 0; col < _gridSize; col++)
                    {
                        if (ShallLocationBeAlive(row, col))
                        {
                            ++locationsAlive;
                            _newGrid[row, col] = true;
                        }
                        else
                        {
                            _newGrid[row, col] = false;
                        }
                    }
                }
                Swap(ref _curGrid, ref _newGrid);
                Console.WriteLine(locationsAlive);
                numberOfIterations--;
            }
        }

        private bool ShallLocationBeAlive(int row, int col)
        {
            var liveNeighbors = CalcAliveNeighbors(row, col);

            if (_curGrid[row, col] == true)
            {
                if ((liveNeighbors == 2) || (liveNeighbors == 3))
                    return true;    // Alive and with two or three neighbors - live on
            }
            else if (liveNeighbors == 3)
            {
                return true; // Dead and with exactly three neighbors - live
            }
            return false;   // Die
        }

        private int CalcAliveNeighbors(int row, int col)
        {
            int liveNeighbors;

            // Implementation of GameOfLife-rules
            if (row == 0)   // Top row
            {
                if (col == 0)    // Top left-hand corner
                    liveNeighbors = Convert.ToInt32(_curGrid[row, col + 1]) +
                                    Convert.ToInt32(_curGrid[row + 1, col]) + Convert.ToInt32(_curGrid[row + 1, col + 1]);

                else if (col == _gridSize - 1) // Top right-hand corner
                    liveNeighbors = Convert.ToInt32(_curGrid[row, col - 1]) +
                                    Convert.ToInt32(_curGrid[row + 1, col - 1]) + Convert.ToInt32(_curGrid[row + 1, col]);

                else // On top edge
                    liveNeighbors = Convert.ToInt32(_curGrid[row, col - 1]) + Convert.ToInt32(_curGrid[row, col + 1] )+
                                    Convert.ToInt32(_curGrid[row + 1, col - 1]) + Convert.ToInt32(_curGrid[row + 1, col]) + Convert.ToInt32(_curGrid[row + 1, col + 1]);
            }
            else if (row == _gridSize - 1) // Bottom row
            {
                if (col == 0) // Bottom left-hand corner
                    liveNeighbors = Convert.ToInt32(_curGrid[row - 1, col]) + Convert.ToInt32(_curGrid[row - 1, col + 1]) +
                                    Convert.ToInt32(_curGrid[row, col + 1]);

                else if (col == _gridSize - 1) // Bottom right-hand corner
                    liveNeighbors = Convert.ToInt32(_curGrid[row - 1, col - 1]) + Convert.ToInt32(_curGrid[row - 1, col]) +
                                       Convert.ToInt32(_curGrid[row, col - 1]);

                else // On bottom edge
                    liveNeighbors = Convert.ToInt32(_curGrid[row - 1, col - 1]) + Convert.ToInt32(_curGrid[row - 1, col]) + Convert.ToInt32(_curGrid[row - 1, col + 1]) +
                                    Convert.ToInt32(_curGrid[row, col - 1]) + Convert.ToInt32(_curGrid[row, col + 1]);
            }
            else if (col == 0)
            {
                // Must be left edge, since corners are covered above
                liveNeighbors = Convert.ToInt32(_curGrid[row - 1, col]) + Convert.ToInt32(_curGrid[row - 1, col + 1]) +
                                                            Convert.ToInt32(_curGrid[row, col + 1]) +
                                Convert.ToInt32(_curGrid[row + 1, col]) + Convert.ToInt32(_curGrid[row + 1, col + 1]);
            }
            else if (col == _gridSize - 1)
            {
                // Must be right edge, since corners are covered above
                liveNeighbors = Convert.ToInt32(_curGrid[row - 1, col - 1]) + Convert.ToInt32(_curGrid[row - 1, col]) +
                                  Convert.ToInt32(_curGrid[row, col - 1]) +
                                  Convert.ToInt32(_curGrid[row + 1, col - 1]) + Convert.ToInt32(_curGrid[row + 1, col]);

            }
            else
            {
                // Interior field
                liveNeighbors = Convert.ToInt32(_curGrid[row - 1, col - 1]) + Convert.ToInt32(_curGrid[row - 1, col]) + Convert.ToInt32(_curGrid[row, col + 1]) +
                                    Convert.ToInt32(_curGrid[row + 1, col - 1]) + Convert.ToInt32(_curGrid[row + 1, col]) +
                                    Convert.ToInt32(_curGrid[row + 1, col - 1]) + Convert.ToInt32(_curGrid[row + 1, col]) + Convert.ToInt32(_curGrid[row + 1, col + 1]);
            }


            return liveNeighbors;
        }

        private void Swap<T>(ref T a, ref T b)
        {
            var temp = a;
            a = b;
            b = temp;
        }
    }
}
