using Godot;
using System;
using System.Collections.Generic;

public partial class CurrentTetromino : CharacterBody2D
{
    [Signal]
public delegate void ReachedBottomEventHandler(); // シンプルにする


    [Export] public Texture2D BlockTexture;
    private Vector2 moveVector = Vector2.Zero;
    private const int CellSize = 32;
    private Random random = new Random();
    private bool isFixed = false;

    private readonly Dictionary<string, int[,]> tetrominoShapes = new Dictionary<string, int[,]>
    {
        { "I", new int[,] { { 1, 1, 1, 1 } } },
        { "O", new int[,] { { 1, 1 }, { 1, 1 } } },
        { "T", new int[,] { { 0, 1, 0 }, { 1, 1, 1 } } },
        { "S", new int[,] { { 0, 1, 1 }, { 1, 1, 0 } } },
        { "Z", new int[,] { { 1, 1, 0 }, { 0, 1, 1 } } },
        { "J", new int[,] { { 1, 0, 0 }, { 1, 1, 1 } } },
        { "L", new int[,] { { 0, 0, 1 }, { 1, 1, 1 } } }
    };

    public override void _Ready()
    {
        CreateRandomTetromino();
    }

    private void CreateRandomTetromino()
    {
        List<string> keys = new List<string>(tetrominoShapes.Keys);
        string randomKey = keys[random.Next(keys.Count)];
        int[,] shape = tetrominoShapes[randomKey];

        for (int y = 0; y < shape.GetLength(0); y++)
        {
            for (int x = 0; x < shape.GetLength(1); x++)
            {
                if (shape[y, x] == 1)
                {
                    Sprite2D block = new Sprite2D();
                    block.Texture = BlockTexture;
                    block.Position = new Vector2(x * CellSize, y * CellSize);
                    AddChild(block);

                    // 動的にコリジョンを設定
                    CollisionShape2D collisionShape = new CollisionShape2D();
                    RectangleShape2D rectShape = new RectangleShape2D();
                    rectShape.Size = new Vector2(CellSize, CellSize);
                    collisionShape.Shape = rectShape;
                    collisionShape.Position = block.Position; // ブロックの位置に基づいて設定
                    block.AddChild(collisionShape);
                }
            }
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        if (isFixed) return;

        moveVector = Vector2.Zero;

        if (Input.IsActionJustPressed("ui_left"))
        {
            moveVector.X = -CellSize;
        }
        else if (Input.IsActionJustPressed("ui_right"))
        {
            moveVector.X = CellSize;
        }
        else if (Input.IsActionJustPressed("ui_down"))
        {
            moveVector.Y = CellSize;
        }

        Velocity = moveVector / (float)delta;
        MoveAndSlide();

        if (IsAtBottom())
        {
            FixTetromino();
            // シグナルの発行
            EmitSignal(nameof(ReachedBottom));
        }
    }

    private void FixTetromino()
    {
        isFixed = true;
        SetPhysicsProcess(false); 
    }

    private bool IsAtBottom()
    {
        return IsOnFloor();
    }
}
