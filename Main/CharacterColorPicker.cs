using Godot;
using System;

public partial class CharacterColorPicker : ColorPickerButton
{
	// @HBoxContainer@3
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}
	// H = [0, 360]
	/// <summary>
	/// Switch statement state.
	/// r starts at 255, increase g untill it hits 255, start decreasing r until it hits 0,
	///  
	/// </summary>
	void HandleHue(){

	}
	// Hue and value affects saturation slider.
	void HandleSaturation(){

	}
	// Hue and saturation affects value slider.
	void HandleValue(){

	}
}
