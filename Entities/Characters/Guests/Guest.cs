using Godot;

public partial class Guest : CharacterBody3D
{
	[Export]
	private Node3D hand;

	public override void _PhysicsProcess(double delta)
	{
		
	}

	public void TakeDrink(Drink drink)
	{
		drink.Reparent(hand, keepGlobalTransform: false);
		drink.Position = Vector3.Zero;
	}
}
