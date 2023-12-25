using Godot;
using System;

public partial class GoBack : MarginContainer
{
	[Export] public ColorPreset preset_color;
	[Export] ColorRect colorRect;

    public override void _Ready()
    {
		GD.Print(colorRect);
    }
    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
		if(Input.IsActionJustPressed("ui_cancel") && Visible){
			GetParent().GetNode<Button>("Button").GrabFocus();
			Visible = false;
		}
	}
	
	public void UpdateColor(Color color){
		colorRect.Color = color;
		GetOwner<CharacterCreater>().UpdateShader();
	}
}
