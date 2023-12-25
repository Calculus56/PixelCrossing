shader_type canvas_item;
// Old Colors
uniform vec4 HairColor : source_color;
uniform vec4 BackHairColor : source_color;
uniform vec4 SkinColor : source_color;
uniform vec4 BackSkinColor : source_color;
uniform vec4 EyeColor : source_color;
// New Colors
uniform vec4 NewHairColor : source_color;
uniform vec4 NewSkinColor : source_color;
uniform vec4 NewEyeColor : source_color;

void fragment(){
	vec4 curr_color = texture(TEXTURE, UV);
	
	vec4 HairDifference = HairColor - BackHairColor;
	vec4 SkinDifference = SkinColor - BackSkinColor;
	// Floating point makes the colors not exactly the same.
	if(distance(curr_color, HairColor) < 0.1){
		COLOR = NewHairColor;
	}
	else if(distance(curr_color, BackHairColor) < 0.1){
		COLOR = NewHairColor - HairDifference;
	}
	else if(distance(curr_color, SkinColor) < 0.1){
		COLOR = NewSkinColor;
	}
	else if(distance(curr_color, BackSkinColor) < 0.1){
		COLOR = NewSkinColor - SkinDifference;
	}
	else if(distance(curr_color, EyeColor) < 0.1){
		COLOR = NewEyeColor;
	}
	else{
		COLOR = curr_color;
	}
}