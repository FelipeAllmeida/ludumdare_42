using UnityEngine;

public abstract class MapObject : MonoBehaviour
{
    public int X { get; protected set; }
    public int Y { get; protected set; }

    public virtual void Initialize(int p_x, int p_y)
    {
        X = p_x;
        Y = p_y;
    }

    public virtual void SetLocalScale(Vector3 p_localScale)
    {

    }

    public virtual void SetPosition(Vector3 p_position)
    {

    }
}
