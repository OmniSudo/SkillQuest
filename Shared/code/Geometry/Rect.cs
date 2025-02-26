using Godot;
using System;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using Crc32 = System.IO.Hashing.Crc32;

namespace SkillQuest.API.Geometry;

public class Rect : Polygon{
    public Rect(Vector2 min, Vector2 max) : base(
        [
            new Edge(new Vector2(min.X, min.Y), new Vector2(min.X, max.Y)),
            new Edge(new Vector2(min.X, max.Y), new Vector2(max.X, max.Y)),
            new Edge(new Vector2(max.X, max.Y), new Vector2(max.X, min.Y)),
            new Edge(new Vector2(max.X, min.Y), new Vector2(min.X, min.Y))
        ]
    ){ }

    public Vector2 size(){
        return Max - Min;
    }

    public float width(){
        return Max.X - Min.X;
    }

    public float height(){
        return Max.Y - Min.Y;
    }

    public bool Colliding(Vector2 point){
        return (Min.X <= point.X && Max.X >= point.X) && (Min.Y <= point.Y && Max.Y >= point.Y);
    }

    public override string ToString(){
        return $"[ {Min}, {Max} ]";
    }

    public long ConstHashCode(){
        return BitConverter.ToInt64(
            Crc32.Hash(
                BitConverter.GetBytes(Min.X)
                    .Concat(BitConverter.GetBytes(Min.Y))
                    .Concat(BitConverter.GetBytes(Max.X))
                    .Concat(BitConverter.GetBytes(Max.Y))
                    .ToArray()
            )
        );
    }
}
