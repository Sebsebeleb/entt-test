using Godot;
using System;
using System.Linq;
using EnTTSharp;
using EnTTSharp.Entities;


/*public class Entity : IEntityKey
{
	public Entity(byte Age, int Key)
	{
		this.Age = Age;
		this.Key = Key;
	}
	public bool Equals(IEntityKey other)
	{
		return this.Age.Equals(other.Age) && this.Key.Equals(other.Key) && this.IsEmpty.Equals(other.IsEmpty);
	}

	public byte Age { get; }
	public int Key { get; }
	public bool IsEmpty { get; }
}*/

public readonly struct Position
{
	public readonly double X;
	public readonly double Y;

	public Position(double x, double y)
	{
		X = x;
		Y = y;
	}

	public override string ToString()
	{
		return $"({X},{Y})";
	}
}

public readonly struct Velocity
{
	public readonly double DeltaX;
	public readonly double DeltaY;

	public Velocity(double deltaX, double deltaY)
	{
		DeltaX = deltaX;
		DeltaY = deltaY;
	}
}


public partial class NewScript : Node2D
{
	private EntityRegistry<EntityKey> registry;
	internal static void UpdatePosition(EntityRegistry<EntityKey> registry, TimeSpan deltaTime)
	{
		// view contains all the entities that have both a position and a velocty component ...
		var view = registry.View<Velocity, Position>();
		foreach (var entity in view)
		{
			if (view.GetComponent(entity, out Position pos) &&
				view.GetComponent(entity, out Velocity velocity))
			{
				var posChanged = new Position(pos.X + velocity.DeltaX * deltaTime.TotalSeconds,
					pos.Y + velocity.DeltaY * deltaTime.TotalSeconds);
				
				registry.ReplaceComponent(entity, in posChanged);
			}
		}
	}
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		registry = new EntityRegistry<EntityKey>(10, (b, i) => new EntityKey(b, i));
		registry.Register<Velocity>();
		registry.Register<Position>();

		int numEntities = 50000;
		for (int x = 0; x < numEntities; x += 1)
		{
			var entity = registry.Create();
			registry.AssignComponent<Position>(entity);
			if ((x % 2) == 0)
			{
				registry.AssignComponent(entity, new Velocity(Random.Shared.NextDouble()*10, Random.Shared.NextDouble()*10));
			}
		}
	}
	
	public static void ClearVelocity(EntityRegistry<EntityKey> registry)
	{
		var view = registry.View<Velocity>();
		foreach (var entity in view)
		{
			registry.ReplaceComponent(entity, new Velocity(0,0));
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		UpdatePosition(registry, TimeSpan.FromSeconds(delta));
		//this.QueueRedraw();
	}

	public override void _Draw()
	{
		foreach (var entity in registry.View<Position>())
		{
			registry.GetComponent(entity, out Position pos);
			DrawCircle(new Vector2((float)pos.X, (float)pos.Y), 12, new Color(0.7f, 0.4f, 0.3f, 0.3f));
		}
	}
}
