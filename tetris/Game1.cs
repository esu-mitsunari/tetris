using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace tetris;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    private KeyboardState _previousKeyState;
    private KeyboardState _currentKeyState;

    private Mino currentMino;

    private Random _random = new Random();
    private int _previousMinoType = -1;

    private Texture2D WhiteBlock, UIFrame;

    private Color[] color;
    private Board gameBoard;

    private double _dropTimer = 0;
    private double _dropInterval = 1.0;
    private bool _isGameOver = false;
    private bool _isFlashing = false;

    private double _shakeTimer = 0;
    private const double ShakeDuration = 0.2;
    private const float ShakeMagnitude = 6f;
    private Vector2 _shakeOffset = Vector2.Zero;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;

        _graphics.PreferredBackBufferWidth = 1280;
        _graphics.PreferredBackBufferHeight = 720;
    }

    protected override void Initialize()
    {
        gameBoard = new Board();
        currentMino = new Mino(4, 0, 1);

        color = new Color[8];
        color[0] = Color.Transparent;
        color[1] = Color.Cyan;
        color[2] = Color.DeepSkyBlue;
        color[3] = Color.Orange;
        color[4] = Color.Yellow;
        color[5] = Color.GreenYellow;
        color[6] = Color.MediumPurple;
        color[7] = Color.Red;
        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        WhiteBlock = Content.Load<Texture2D>("images/WhiteBlock");
        UIFrame = Content.Load<Texture2D>("images/UIFrame");

    }

    protected override void Update(GameTime gameTime)
    {

        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        if (_isGameOver) return;

        if (_shakeTimer > 0)
        {
            _shakeTimer -= gameTime.ElapsedGameTime.TotalSeconds;

            float mag = ShakeMagnitude * (float)(_shakeTimer / ShakeDuration);
            _shakeOffset = new Vector2(
                (_random.NextSingle() * 2f - 1f) * mag,
                (_random.NextSingle() * 2f - 1f) * mag
            );
            if (_shakeTimer <= 0) _shakeOffset = Vector2.Zero;
        }

        if (_isFlashing)
        {
            if (gameBoard.UpdateFlash(gameTime.ElapsedGameTime.TotalSeconds))
            {
                _isFlashing = false;
                gameBoard.ClearFlashingRows();
                SpawnNextMino();
            }
            base.Update(gameTime);
            return;
        }

        _dropTimer += gameTime.ElapsedGameTime.TotalSeconds;

        if (_dropTimer >= _dropInterval)
        {
            _dropTimer -= _dropInterval;

            DropMino();
        }

        _previousKeyState = _currentKeyState;
        _currentKeyState = Keyboard.GetState();

        if (_currentKeyState.IsKeyDown(Keys.Right) && _previousKeyState.IsKeyUp(Keys.Right))
        {
            if (gameBoard.IsValidPosition(currentMino, currentMino.X + 1, currentMino.Y))
            {
                currentMino.X++;
            }
        }

        if (_currentKeyState.IsKeyDown(Keys.Left) && _previousKeyState.IsKeyUp(Keys.Left))
        {
            if (gameBoard.IsValidPosition(currentMino, currentMino.X - 1, currentMino.Y))
            {
                currentMino.X--;
            }
        }

        if (_currentKeyState.IsKeyDown(Keys.Down) && _previousKeyState.IsKeyUp(Keys.Down))
        {
            DropMino();
        }

        if (_currentKeyState.IsKeyDown(Keys.Up) && _previousKeyState.IsKeyUp(Keys.Up))
        {
            int ghostY = currentMino.GetGhostY(gameBoard);

            currentMino.Y = ghostY;

            DropMino();

            _dropTimer = 0;
        }

        if (_currentKeyState.IsKeyDown(Keys.Space) && _previousKeyState.IsKeyUp(Keys.Space))
        {
            bool[,] oldShape = (bool[,])currentMino.Shape.Clone();

            currentMino.Rotate();

            if (!gameBoard.IsValidPosition(currentMino, currentMino.X, currentMino.Y))
            {
                currentMino.Shape = oldShape;
            }
        }

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);
        Matrix shakeMatrix = Matrix.CreateTranslation(_shakeOffset.X, _shakeOffset.Y, 0);
        _spriteBatch.Begin(transformMatrix: shakeMatrix);

        _spriteBatch.Draw(UIFrame, Vector2.Zero, Color.White);

        gameBoard.Draw(_spriteBatch, WhiteBlock, color);

        currentMino.Draw(_spriteBatch, WhiteBlock, color, gameBoard);
        currentMino.DrawGhost(_spriteBatch, WhiteBlock, color, gameBoard);

        _spriteBatch.End();
        base.Draw(gameTime);
    }

    private void DropMino()
    {
        if (gameBoard.IsValidPosition(currentMino, currentMino.X, currentMino.Y + 1))
        {
            currentMino.Y++;
        }
        else
        {
            gameBoard.LockMino(currentMino);
            _shakeTimer = ShakeDuration;

            var fullRows = gameBoard.FindFullLines();
            if (fullRows.Count > 0)
            {
                gameBoard.StartFlash(fullRows);
                _isFlashing = true;
            }
            else
            {
                SpawnNextMino();
            }
        }
    }

    private void SpawnNextMino()
    {
        int nextType;
        do
        {
            nextType = _random.Next(1, 8);
        }
        while (nextType == _previousMinoType);
        currentMino = new Mino(4, 0, nextType);
        _previousMinoType = nextType;

        if (!gameBoard.IsValidPosition(currentMino, currentMino.X, currentMino.Y))
        {
            _isGameOver = true;
            Console.WriteLine("GAME OVER");
        }
    }
}
