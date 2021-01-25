using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointInTime
{
    public Vector2 position;
    public Sprite sprite;
    public Vector2 scale;
    public Quaternion rotation;


    public PointInTime(Vector2 _position, Sprite _sprite, Vector2 _scale, Quaternion _rotation)
    {
        position = _position;
        sprite = _sprite;
        scale = _scale;
        rotation = _rotation;
    }
}
