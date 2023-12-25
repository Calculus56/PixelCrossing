using Godot;
using System;

[GlobalClass]
public partial class ColorPreset : Resource
{
    [Export] public Color color{private set; get;}
}
