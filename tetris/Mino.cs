using System;
using System.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace tetris;

public class Mino
{
    public bool[,] Shape;
    public int X, Y;
    public int ColorID;

    private string MinoErrorMessage="ミノエラー";

    public Mino(int startX, int startY, int type)
    {
        X = startX;
        Y = startY;
        ColorID = type;
        switch (type)
        {
            case 1:
                Shape = new bool[4, 4]
                {
                    {false,true,false,false},
                    {false,true,false,false},
                    {false,true,false,false},
                    {false,true,false,false}
                };
                break;
            case 2:
                Shape = new bool[3, 3]
                {
                    { true, false, false },
                    { true, true, true },
                    { false, false, false }
                };
                break;
            case 3:
                Shape = new bool[3, 3]
                {
                    { false, false, true },
                    { true, true, true },
                    { false, false, false }
                };
                break;
            case 4:
                Shape = new bool[2, 2]
                {
                    { true, true},
                    { true, true}
                };
                break;
            case 5:
                Shape = new bool[3, 3]
                {
                    { false, true, true },
                    { true, true, false },
                    { false, false, false }
                };
                break;
            case 6:
                Shape = new bool[3, 3]
                {
                    { false, true, false },
                    { true, true, true },
                    { false, false, false }
                };
                break;
            case 7:
                Shape = new bool[3, 3]
                {
                    { true, true, false },
                    { false, true, true },
                    { false, false, false }
                };
                break;
            default:
                Console.WriteLine(MinoErrorMessage);
                break;

        }
    }

    public void Draw(SpriteBatch spriteBatch, Texture2D whiteBlock, Color[] colors, Board board)
    {
        for (int row = 0; row < Shape.GetLength(0); row++)
        {
            for (int col = 0; col < Shape.GetLength(1); col++)
            {
                if (Shape[row, col])
                {
                    spriteBatch.Draw(whiteBlock, board.GetScreenPosition(X + col, Y + row), colors[ColorID]);
                }
            }

        }
    }

    public void Rotate()
    {
        int size = Shape.GetLength(0);
        bool[,] newShape = new bool[size, size];

        for (int r = 0; r < size; r++)
        {
            for (int c = 0; c < size; c++)
            {
                newShape[c, size-1-r] = Shape[r, c];
            }
        }

        Shape = newShape;
    }

    public int GetGhostY(Board board)
    {
        int ghostY = Y;

        while ( true )
        {
            if (!board.IsValidPosition(this, X, ghostY + 1))
            {
                break;
            }
            ghostY++;
        }


        return ghostY;
    }

    public void DrawGhost(SpriteBatch spriteBatch, Texture2D whiteBlock, Color[] colors, Board board)
    {
        int ghostY = GetGhostY(board);

        Color ghostColor = colors[ColorID] * 0.4f;

        for (int row = 0; row < Shape.GetLength(0); row++)
        {
            for (int col = 0; col < Shape.GetLength(1); col++)
            {
                if (Shape[row, col])
                {
                    spriteBatch.Draw(whiteBlock, board.GetScreenPosition(X + col, ghostY + row), ghostColor);
                }
            }
        }
    }
}
