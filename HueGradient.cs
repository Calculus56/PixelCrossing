using Godot;
using Godot.NativeInterop;
using System;
using System.Linq;

public partial class HueGradient : HBoxContainer
{
	[Export] TextureRect hue_slider, saturation_slider, value_slider;
	[Export] ColorRect final_color;
	int maxOffsets = 7, pos = 1;
	// Need to be from 0 to 1.
	float[] huePositions = new float[7];
	Color[] hueColors = new Color[7];
	Color color = Colors.White; 
	Color red = Colors.Red;
	float hue = 0.5f, saturation = 0.5f, value = 0.5f;
	Gradient hueGradient, saturationGradient, valueGradient;
	
	public override void _Ready()
	{
		hueGradient = InitializeGradient(hue_slider);
		saturationGradient = InitializeGradient(saturation_slider);
		valueGradient = InitializeGradient(value_slider);
		SetUpHueSlider();
		SetUpSaturationSlider();
	}

	Gradient InitializeGradient(TextureRect texture){
		return (texture.Texture as GradientTexture1D).Gradient;
	}

	// How Hue Works
	// Color has to be between 0 and 1, use rgb_value/255. If you use 255 as the value you will end up with a value of 65025.
	// rgb(255,0,0) --> rgb(255,255,0) --> rgb(0,255,0) --> rgb(0,255,255) --> rgb(0,0,255) --> rgb(255,0,255) --> rgb(255,0,0)
	void SetUpHueSlider(){
		for(int i = 0; i < maxOffsets; i++){
			huePositions[i] = (float)i / (maxOffsets - 1);
			if(i != 0) RGBSwitch(i % 2);
			hueColors[i] = red;
		}
		
		hueGradient.Offsets = huePositions;
		hueGradient.Colors = hueColors;
		UpdateColors(hue_slider, hue, "Hue");
		ResourceSaver.Save(hueGradient, $"res://Tres/HueBase.tres");
	}

	// How saturation works.
	// Use greyscale (R + G + B)/3 for all 3 values. This is 100% greyscale.
	// RGB(255, 0, 0), GreyScale(128, 128, 128), Difference (128, -128, -128)
	// RGB - Greyscale = Difference
	// Substitute
	// RGB - ((RGB - Greyscale) * greyscale_percent) = Saturatioon
	void SetUpSaturationSlider(){
		float rgb_avg = (color.R + color.G + color.B) / 3;
		Color greyscale = new Color(rgb_avg, rgb_avg, rgb_avg);
		float[] saturationPos = {0,1};
		Color[] saturationColors = {greyscale, color};
		saturationGradient.Offsets = saturationPos;
		saturationGradient.Colors = saturationColors;
		GD.Print(greyscale);
	}
	// How value works.
	// The lower the value, the closer the color is to balck RGB(0, 0, 0).
	// We take the value and that is the percentage of the hue and saturation.
	void SetUpValueSlider(){

	}

	// We don't change the values of the other sliders we only update the color rectangles.
	void UpdateColors(TextureRect texture, float val, string name){
		// Saves resources to disk only when user stops draging.
		
		// Final RGB Value
		// (Hue * Saturation) / Value
		color = InitializeGradient(texture).Sample(val);
		Color editted_color = color * saturation / value;
		final_color.Color = editted_color;
		
		ResourceSaver.Save(texture.Texture as GradientTexture1D, $"res://Tres/{name}Texture.tres");
	}
	// Used to make an array of hue colors for a gradient.
	void RGBSwitch(int i){
		switch(i){
			// Remove Behind
			case 0:
				red[(pos - 1) % 3] = 0;
				pos += 1;
				break;
			// Add Next
			case 1:
				red[pos % 3] = 1;
				break;
		}
	}


	void OnHueSliderValueChanged(float val){
		hue = val;
		UpdateColors(hue_slider, val, "Hue");
		SetUpSaturationSlider();
	}

	void OnSaturationSliderValueChanged(float val){
		saturation = val;
		UpdateColors(saturation_slider, val, "Saturation");
	}
}
