using Godot;
using Godot.NativeInterop;
using System;
using System.Linq;

public partial class HueGradient : HBoxContainer
{
	[Export] public TextureRect hue_slider, saturation_slider, value_slider;
	[Export] ColorRect final_color;
	int maxOffsets = 7, pos = 1;
	// Need to be from 0 to 1.
	float[] huePositions = new float[7];
	Color[] hueColors = new Color[7];
	Color color = Colors.White, red = Colors.Red, full_greyscale;
	float hue = 0f, saturation = 1f, value = 0.5f;
	Gradient hueGradient, saturationGradient, valueGradient;
	
	public override void _Ready()
	{
		hueGradient = InitializeGradient(hue_slider);
		saturationGradient = InitializeGradient(saturation_slider);
		valueGradient = InitializeGradient(value_slider);
		hue = (Owner as GoBack).preset_color.color.H;
		saturation = (Owner as GoBack).preset_color.color.S;
		value = (Owner as GoBack).preset_color.color.V;
		
		full_greyscale = new Color(0.5f * value * 2, 0.5f * value * 2, 0.5f * value * 2);
		
		SetUpHueSlider();
		SetUpSaturationSlider();
		SetUpValueSlider();
		// Signals will be called to everthing.
		// SaveBaseColors();
		hue_slider.GetParent<Slider>().Value = hue;
		saturation_slider.GetParent<Slider>().Value = saturation;
		value_slider.GetParent<Slider>().Value = value;
	}

	void SaveBaseColors(){
		ResourceSaver.Save(hue_slider.Texture as GradientTexture1D, $"res://Tres/HueTexture.tres");
		ResourceSaver.Save(saturation_slider.Texture as GradientTexture1D, $"res://Tres/SaturationTexture.tres");
		ResourceSaver.Save(value_slider.Texture as GradientTexture1D, $"res://Tres/ValueTexture.tres");
	}

	Gradient InitializeGradient(TextureRect texture){
		// Instantiated objects are 
		texture.Texture = new GradientTexture1D();
		(texture.Texture as GradientTexture1D).Gradient = new Gradient();
		return (texture.Texture as GradientTexture1D).Gradient;
	}

	// How Hue Works
	// Color has to be between 0 and 1, use rgb_value/255. If you use 255 as the value you will end up with a value of 65025.
	// rgb(255,0,0) --> rgb(255,255,0) --> rgb(0,255,0) --> rgb(0,255,255) --> rgb(0,0,255) --> rgb(255,0,255) --> rgb(255,0,0)
	void SetUpHueSlider(){
		for(int i = 0; i < maxOffsets; i++){
			huePositions[i] = (float)i / (maxOffsets - 1);
			if(i != 0) RGBSwitch(i % 2);
			Color sat_difference = red - new Color(value, value, value);
			Color saturated_color = red - ((1 - saturation) * sat_difference);
			hueColors[i] = saturated_color - ((1 - value) * saturated_color);

			// Having value be a decimal allows us to use multiplication, if value had a range of 1-100, we would have to divide by it.
			// This would result in a DivideByZero Error and we have to use 'try and catch' for everytime we use value.
			// We wary of the aplha value multiplying by a deciaml will decrease it.
			hueColors[i].A = 1;
		}
		
		hueGradient.Offsets = huePositions;
		hueGradient.Colors = hueColors;
		UpdateColors(hue_slider, hue, "Hue");
	}

	// How saturation works.
	// Use greyscale (R + G + B)/3 for all 3 values. This is 100% greyscale.
	// RGB(255, 0, 0), GreyScale(128, 128, 128), Difference (128, -128, -128)
	// RGB - Greyscale = Difference
	// Substitute
	// RGB - ((RGB - Greyscale) * greyscale_percent) = Saturation
	void SetUpSaturationSlider(){
		//float rgb_avg = (color.R + color.G + color.B) / 3;
		//Color greyscale = new Color(rgb_avg, rgb_avg, rgb_avg);
		Color greyscale = new Color(0.5f * value * 2, 0.5f * value * 2, 0.5f * value * 2);
		float[] saturationPos = {0,1};
		Color[] saturationColors = {greyscale, color};
		saturationGradient.Offsets = saturationPos;
		saturationGradient.Colors = saturationColors;
		UpdateColors(saturation_slider, saturation, "Saturation");
	}
	// How value works.
	// The lower the value, the closer the color is to balck RGB(0, 0, 0).
	// We take the value and that is the percentage of the hue and saturation.
	void SetUpValueSlider(){
		Color greyscale = new Color(0.5f * value * 2, 0.5f * value * 2, 0.5f * value * 2);
		Color sat_difference = color - greyscale;
		Color saturated_color = color - ((1 - value) * sat_difference);
		float[] valuePos = {0, 1};
		Color[] valueColors = {Colors.Black, saturated_color};
		valueGradient.Offsets = valuePos;
		valueGradient.Colors = valueColors;
		UpdateColors(value_slider, value, "Value");
	}

	// We don't change the values of the other sliders we only update the final color rectangle.
	void UpdateColors(TextureRect texture, float val, string name){
		// Saves resources to disk only when user stops draging.
		
		// We get the color of the hue slider because we change it with the saturation and value already.
		color = (hue_slider.Texture as GradientTexture1D).Gradient.Sample(hue);
		//Color difference = color - full_greyscale;
		//Color editted_color = (color - ((1 - saturation) * difference)) / value;
		final_color.Color = color;
		(Owner as GoBack).UpdateColor(final_color.Color);
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
		SetUpHueSlider();
		SetUpSaturationSlider();
		SetUpValueSlider();
	}

	void OnSaturationSliderValueChanged(float val){
		saturation = val;
		UpdateColors(saturation_slider, val, "Saturation");
		SetUpHueSlider();
		SetUpSaturationSlider();
		SetUpValueSlider();
	}

	void OnValueSliderValueChanged(float val){
		value = val;
		UpdateColors(value_slider, val, "Value");
		SetUpValueSlider();
		SetUpHueSlider();
		SetUpSaturationSlider();
	}
	
}
