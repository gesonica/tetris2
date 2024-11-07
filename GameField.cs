using Godot;
using System;

public partial class GameField : Node2D
{
    [Export] public int FieldWidth = 10;
    [Export] public int FieldHeight = 20;
    [Export] public int CellSize = 32;
    [Export] public Texture BlockTexture;
    [Export] public Color FieldColor = new Color(1, 1, 1);
    [Export] public PackedScene TetrominoScene;

    private bool gameStarted = false; // ゲーム開始フラグ

    public override void _Ready()
    {
        GD.Print($"Field size: {FieldWidth} x {FieldHeight}, Cell size: {CellSize}px");
        CreateCollision();
        QueueRedraw();
    }

    public override void _Process(double delta)
    {
        // スペースキーが押されたらゲームを開始
        if (!gameStarted && Input.IsActionJustPressed("ui_select"))
        {
            gameStarted = true;
            SpawnNewTetromino();
        }
    }

    private void CreateCollision()
    {
        StaticBody2D staticBody = new StaticBody2D();
        AddChild(staticBody);

        // 左側の壁
        CollisionShape2D leftWallCollision = new CollisionShape2D();
        RectangleShape2D leftRect = new RectangleShape2D();
        leftRect.Size = new Vector2(CellSize, FieldHeight * CellSize);
        leftWallCollision.Shape = leftRect;
        leftWallCollision.Position = new Vector2(0, FieldHeight * CellSize / 2);
        staticBody.AddChild(leftWallCollision);

        // 右側の壁
        CollisionShape2D rightWallCollision = new CollisionShape2D();
        RectangleShape2D rightRect = new RectangleShape2D();
        rightRect.Size = new Vector2(CellSize, FieldHeight * CellSize);
        rightWallCollision.Shape = rightRect;
        rightWallCollision.Position = new Vector2(FieldWidth * CellSize, FieldHeight * CellSize / 2);
        staticBody.AddChild(rightWallCollision);

        // 上側の壁
        CollisionShape2D topWallCollision = new CollisionShape2D();
        RectangleShape2D topRect = new RectangleShape2D();
        topRect.Size = new Vector2(FieldWidth * CellSize, CellSize);
        topWallCollision.Shape = topRect;
        topWallCollision.Position = new Vector2(FieldWidth * CellSize / 2, 0);
        staticBody.AddChild(topWallCollision);

        // 下側の壁
        CollisionShape2D bottomWallCollision = new CollisionShape2D();
        RectangleShape2D bottomRect = new RectangleShape2D();
        bottomRect.Size = new Vector2(FieldWidth * CellSize, CellSize);
        bottomWallCollision.Shape = bottomRect;
        bottomWallCollision.Position = new Vector2(FieldWidth * CellSize / 2, FieldHeight * CellSize);
        staticBody.AddChild(bottomWallCollision);
    }

    public override void _Draw()
    {
        for (int x = 0; x < FieldWidth; x++)
        {
            for (int y = 0; y < FieldHeight; y++)
            {
                Vector2 cellPosition = new Vector2(x * CellSize, y * CellSize);
                DrawRect(new Rect2(cellPosition, new Vector2(CellSize, CellSize)), FieldColor, filled: true);
            }
        }
    }

    public void SpawnNewTetromino()
    {
        int offsetY = CellSize; // テトロミノの生成時のYオフセット
        CurrentTetromino newTetromino = TetrominoScene.Instantiate<CurrentTetromino>();
        AddChild(newTetromino);

        // 中央に配置し、Y位置にオフセットを適用
        newTetromino.Position = new Vector2(FieldWidth / 2 * CellSize, offsetY);

        newTetromino.Connect(nameof(CurrentTetromino.ReachedBottom), new Callable(this, nameof(OnTetrominoReachedBottom)));
    }

    private void OnTetrominoReachedBottom()
    {
        SpawnNewTetromino();
    }
}