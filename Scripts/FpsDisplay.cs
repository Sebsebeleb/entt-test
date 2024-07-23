using Godot;

namespace entttest.Scripts;

public partial class FpsDisplay : Label
{
    public override void _Process(double delta)
    {
        this.Text = Engine.GetFramesPerSecond().ToString();
    }
}