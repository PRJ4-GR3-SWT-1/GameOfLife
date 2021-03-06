using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Schema;
using Microsoft.VisualBasic;

namespace GameOfLife
{
    class GameOfLifeTB
    {
        private readonly int _gridSize;
        private const int AliveThreshold = 25;
        private int[,] _curGrid;
        private int[,] _newGrid;


        public GameOfLifeTB(int gridSize)
        {
            _gridSize = gridSize;
            _curGrid = new int[_gridSize, _gridSize];
            _newGrid = new int[_gridSize, _gridSize];

            var rnd = new Random(45);
            for (var row = 0; row < _gridSize; row++)
            {
                for (var col = 0; col < _gridSize; col++)
                {
                    _curGrid[row, col] = (rnd.Next(100) < AliveThreshold) ? 1 : 0;
                }
            }
        }


        public void Run(int numberOfIterations)
        {
        
            int numberOfTask = Environment.ProcessorCount;
            var tasks = new Task[numberOfTask];
            var stepBarrier = new Barrier(numberOfTask, _ => Swap(ref _curGrid, ref _newGrid));
            int chunckSize = _gridSize / numberOfTask;
            var locationsAlive = new int[numberOfTask];
            for (int i = 0; i < numberOfTask; i++)
            {
                int chunckRow = i * chunckSize;
                var localI = i;
                tasks[i] = Task.Run(() =>
                    {

                        while (numberOfIterations > 0)
                        {

                            for (var row = chunckRow; row < chunckRow + chunckSize; row++)
                            {
                                for (var col = 0; col < _gridSize; col++)
                                {
                                    if (ShallLocationBeAlive(row, col))
                                    {
                                        ++locationsAlive[localI];
                                        _newGrid[row, col] = 1;
                                    }
                                    else
                                    {
                                        _newGrid[row, col] = 0;
                                    }
                                }
                            }
                            stepBarrier.SignalAndWait();
                            //Console.WriteLine(stepBarrier.ParticipantsRemaining);
                            //while (stepBarrier.ParticipantsRemaining > 0) { }
                            Console.WriteLine(locationsAlive.Sum());
                            numberOfIterations--;
                        }
                        stepBarrier.RemoveParticipant();
                    }
                );
                
            }

            while (stepBarrier.ParticipantCount>0)
            {
                
            }
                
        }

        private bool ShallLocationBeAlive(int row, int col)
        {
            var liveNeighbors = CalcAliveNeighbors(row, col);

            if (_curGrid[row, col] == 1)
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
                    liveNeighbors = _curGrid[row, col + 1] +
                                       _curGrid[row + 1, col] + _curGrid[row + 1, col + 1];

                else if (col == _gridSize - 1) // Top right-hand corner
                    liveNeighbors = _curGrid[row, col - 1] +
                                       _curGrid[row + 1, col - 1] + _curGrid[row + 1, col];

                else // On top edge
                    liveNeighbors = _curGrid[row, col - 1] + _curGrid[row, col + 1] +
                                       _curGrid[row + 1, col - 1] + _curGrid[row + 1, col] + _curGrid[row + 1, col + 1];
            }
            else if (row == _gridSize - 1) // Bottom row
            {
                if (col == 0) // Bottom left-hand corner
                    liveNeighbors = _curGrid[row - 1, col] + _curGrid[row - 1, col + 1] +
                                                                   _curGrid[row, col + 1];

                else if (col == _gridSize - 1) // Bottom right-hand corner
                    liveNeighbors = _curGrid[row - 1, col - 1] + _curGrid[row - 1, col] +
                                       _curGrid[row, col - 1];

                else // On bottom edge
                    liveNeighbors = _curGrid[row - 1, col - 1] + _curGrid[row - 1, col] + _curGrid[row - 1, col + 1] +
                                    _curGrid[row, col - 1] + _curGrid[row, col + 1];
            }
            else if (col == 0)
            {
                // Must be left edge, since corners are covered above
                liveNeighbors = _curGrid[row - 1, col] + _curGrid[row - 1, col + 1] +
                                                            _curGrid[row, col + 1] +
                                _curGrid[row + 1, col] + _curGrid[row + 1, col + 1];
            }
            else if (col == _gridSize - 1)
            {
                // Must be right edge, since corners are covered above
                liveNeighbors = _curGrid[row - 1, col - 1] + _curGrid[row - 1, col] +
                                  _curGrid[row, col - 1] +
                                  _curGrid[row + 1, col - 1] + _curGrid[row + 1, col];

            }
            else
            {
                // Interior field
                liveNeighbors = _curGrid[row - 1, col - 1] + _curGrid[row - 1, col] + _curGrid[row, col + 1] +
                                    _curGrid[row + 1, col - 1] + _curGrid[row + 1, col] +
                                    _curGrid[row + 1, col - 1] + _curGrid[row + 1, col] + _curGrid[row + 1, col + 1];
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
