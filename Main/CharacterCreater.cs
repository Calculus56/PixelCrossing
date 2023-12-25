using Godot;
using System;

public partial class CharacterCreater : CanvasLayer
{
    [Export] TextureRect texture;
    [Export] ColorRect Hair, Eyes, Skin;

    public void UpdateShader(){
        (texture.Material as ShaderMaterial).SetShaderParameter("NewHairColor", Hair.Color);
        (texture.Material as ShaderMaterial).SetShaderParameter("NewEyeColor", Eyes.Color);
        (texture.Material as ShaderMaterial).SetShaderParameter("NewSkinColor", Skin.Color);
    }
}
