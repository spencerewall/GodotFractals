using Godot;
using System;

public partial class Main : Control
{
	private Color _circleColor = new Color(0, 1, 0); // Green color
    private float _circleRadius = 50f; // Radius of the circle
    private PackedScene _fractalNodeFactory;
    public override void _Ready()
    {
        _fractalNodeFactory = GD.Load<PackedScene>("res://fractal_node.tscn");
		// draw the first one in the middle of the screen
		Vector2 screenSize = GetViewportRect().Size;
		
        FractalNode centerNode = _fractalNodeFactory.Instantiate() as FractalNode;
        centerNode.Position = screenSize / 2;
        this.AddChild(centerNode);

        GD.Print(centerNode.Position);
        Action centerClickHandler = BuildClickHandler(centerNode.Position, new Vector2(0,0), screenSize, centerNode.Scale * .75f, true);
        centerNode.Pressed += centerClickHandler.Invoke;
    }

    // every time a node is clicked, this method will be called on it
    // todo: first instance
    // todo: fix for every other instance
    private Action BuildClickHandler(Vector2 position, Vector2 minPosition, Vector2 maxPosition, Vector2 scale, bool growHorizontal) {
        float growVal = growHorizontal ? position.X : position.Y;
        float growMin = growHorizontal ? minPosition.X : minPosition.Y;
        float growMax = growHorizontal ? maxPosition.X : maxPosition.Y;
        float newLeftVal = (growVal + growMin) / 2;
        float newRightVal = (growVal + growMax) / 2;

        Vector2 leftPosition = growHorizontal ? new Vector2(newLeftVal, position.Y) : new Vector2(position.X, newLeftVal);
        Vector2 rightPosition = growHorizontal ? new Vector2(newRightVal, position.Y) : new Vector2(position.X, newRightVal);
        
        bool newGrowDirection = !growHorizontal;
        
        return () => {
            GD.Print("Position", position);
            GD.Print("Left position", leftPosition);
            GD.Print("Right position", rightPosition);

            FractalNode leftInstance = _fractalNodeFactory.Instantiate() as FractalNode;
            FractalNode rightInstance = _fractalNodeFactory.Instantiate() as FractalNode;
        
            leftInstance.Position = leftPosition;
            rightInstance.Position = rightPosition;
            leftInstance.Scale = scale;
            rightInstance.Scale = scale;
            this.AddChild(leftInstance);
            this.AddChild(rightInstance);

            // ** Calculate the next values and lazily generate the next click handler step to be executed recursively
            // when growing horizontally: 
            //   y values stay the same - left x max goes to parent position - right x min goes to parent position
            // when growing vertically, similar pattern follows: 
            //   x values stay the same - left y max goes to parent position - right y min goes to parent position
            Vector2 leftMin;
            Vector2 rightMin;
            Vector2 leftMax;
            Vector2 rightMax;
            if (growHorizontal) {
                leftMin = minPosition;
                leftMax = new Vector2(position.X, maxPosition.Y);
                rightMin = new Vector2(position.X, minPosition.Y);
                rightMax = maxPosition;
            } else {
                leftMin = minPosition;
                leftMax = new Vector2(maxPosition.X, position.Y);
                rightMin = new Vector2(minPosition.X, position.Y);
                rightMax = maxPosition;
            }

            Action leftClickHandler = BuildClickHandler(leftPosition, leftMin, leftMax, scale * .75f, newGrowDirection);
            Action rightClickHandler = BuildClickHandler(rightPosition, rightMin, rightMax, scale * .75f, newGrowDirection);
            leftInstance.Pressed += leftClickHandler.Invoke;
            rightInstance.Pressed += rightClickHandler.Invoke;
        };
    }
}