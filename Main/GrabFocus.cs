using Godot;
using System;

public partial class GrabFocus : Button
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		ButtonUp += SetFocus;
		GrabFocus();
	}

    void SetFocus(){
		GD.Print("Click");
		GetParent().GetNode<MarginContainer>("ColorSlider").Visible = true;
		GetParent().GetNode<MarginContainer>("ColorSlider").GetNode<HueGradient>("HBoxContainer").hue_slider.GetParent<Slider>().GrabFocus();
	}
}
