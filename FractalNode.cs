using Godot;
using System;

public partial class FractalNode : Node2D
{
	[Signal]
	public delegate void PressedEventHandler();
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void _Pressed() {
		GD.Print("FractalNode Pressed");
		EmitSignal(SignalName.Pressed);
	}
}
