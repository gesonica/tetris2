using Godot;
using System;

public partial class Block1 : Node2D
{
    [Export] public Texture2D BlockTexture;

    public override void _Ready()
    {
        // Sprite2Dノードを取得してテクスチャを設定
        if (GetNode("Sprite2D") is Sprite2D sprite)
        {
            sprite.Texture = BlockTexture;
        }

        // CollisionShape2Dノードを取得して形状を設定
        if (GetNode("CollisionShape2D") is CollisionShape2D collisionShape)
        {
            RectangleShape2D rectShape = new RectangleShape2D();
            rectShape.Size = new Vector2(32, 32); // 32x32ピクセルのサイズを設定
            collisionShape.Shape = rectShape;
        }
    }
}
