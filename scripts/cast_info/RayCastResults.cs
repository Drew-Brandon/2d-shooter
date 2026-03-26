using Godot;
using Godot.Collections;

public partial struct RayCastResults
{
    public int Shape = -1;

    public Rid ColliderRid = new Rid();

    public Vector2 Position = Vector2.Zero;

    public Vector2 Normal = Vector2.Zero;

    public GodotObject Collider = null;

    public RayCastResults(Dictionary resultsDict)
    {
        Shape = resultsDict["shape"].AsInt32();
        ColliderRid = resultsDict["rid"].AsRid();
        Position = resultsDict["position"].AsVector2();
        Normal = resultsDict["normal"].AsVector2();
        Collider = resultsDict["collider"].AsGodotObject();
    }
}
