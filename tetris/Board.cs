using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace tetris;

public class Board
{
    public const int Rows = 20;
    public const int Cols = 10;
    private int[,] _cells = new int[Rows, Cols];

    private List<int> _flashingRows = new List<int>();
    private double _flashTimer = 0;
    private const double FlashDuration = 0.15;

    private const int BlockSize = 32;
    private readonly Vector2 benchmark = new Vector2(481, 41);

    public void PlaceBlock(int x, int y, int type)
    {
        if (x >= 0 && x < Cols && y >= 0 && y < Rows)
        {
            _cells[y, x] = type;
        }
    }

    public Vector2 GetScreenPosition(int x, int y)
    {
        float X = x * BlockSize + benchmark.X;
        float Y = y * BlockSize + benchmark.Y;
        return new Vector2(X, Y);
    }

    public void Draw(SpriteBatch spriteBatch, Texture2D whiteBlock, Color[] colors)
    {
        for (int i = 0; i < Rows; i++)
        {
            bool isFlashing = _flashingRows.Contains(i);
            for (int j = 0; j < Cols; j++)
            {
                if (_cells[i, j] > 0)
                {
                    Color drawColor = isFlashing ? Color.White : colors[_cells[i, j]];
                    spriteBatch.Draw(whiteBlock, GetScreenPosition(j, i), drawColor);
                }
            }
        }
    }

    public bool IsValidPosition(Mino mino, int nextX, int nextY)
    {
        int rows = mino.Shape.GetLength(0);
        int cols = mino.Shape.GetLength(1);

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                if (mino.Shape[r, c])
                {
                    int boardX = nextX + c;
                    int boardY = nextY + r;

                    if (boardX < 0 || boardX >= Cols || boardY >= Rows)
                    {
                        return false;
                    }

                    if (boardY >= 0 && _cells[boardY, boardX] > 0)
                    {
                        return false;
                    }
                }
            }
        }
        return true;
    }

    public void LockMino(Mino mino)
    {
        int rows = mino.Shape.GetLength(0);
        int cols = mino.Shape.GetLength(1);

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                if (mino.Shape[r, c])
                {
                    int boardX = mino.X + c;
                    int boardY = mino.Y + r;

                    if (boardY >= 0 && boardY < Rows && boardX >= 0 && boardX < Cols)
                    {
                        _cells[boardY, boardX] = mino.ColorID;
                    }
                }
            }
        }
    }

    public List<int> FindFullLines()
    {
        var fullRows = new List<int>();
        for (int y = 0; y < Rows; y++)
        {
            bool isFull = true;
            for (int x = 0; x < Cols; x++)
            {
                if (_cells[y, x] == 0) { isFull = false; break; }
            }
            if (isFull) fullRows.Add(y);
        }
        return fullRows;
    }

    public void StartFlash(List<int> rows)
    {
        _flashingRows = rows;
        _flashTimer = FlashDuration;
    }

    public bool UpdateFlash(double deltaTime)
    {
        _flashTimer -= deltaTime;
        return _flashTimer <= 0;
    }

    public void ClearFlashingRows()
    {
        _flashingRows.Sort((a, b) => b.CompareTo(a));
        foreach (int y in _flashingRows)
        {
            for (int i = y; i > 0; i--)
                for (int x = 0; x < Cols; x++)
                    _cells[i, x] = _cells[i - 1, x];

            for (int x = 0; x < Cols; x++)
                _cells[0, x] = 0;
        }
        _flashingRows.Clear();
    }
}
