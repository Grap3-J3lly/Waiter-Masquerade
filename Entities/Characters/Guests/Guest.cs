using Godot;

public partial class Guest : CharacterBody3D
{
	[Export]
	private Node3D hand;

	[Export]
	private Mask mask;

	public Mask Mask { get => mask; }

	public override void _Ready()
	{
		mask.Setup();
	}

	public void TakeDrink(Drink drink)
	{
		drink.Reparent(hand, keepGlobalTransform: false);
		drink.Position = Vector3.Zero;
		drink.DisableInteractions();
		drink.BeginDespawning();
	}
}
