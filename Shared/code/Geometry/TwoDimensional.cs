using Godot;
using System.Collections.Generic;

namespace SkillQuest.API.Geometry;

public interface TwoDimensional{
    public enum Axis{
        X,
        Y
    }

    Vector2? Center { get; init; }

    List<Edge> Edges { get; init; }

    List<Vector2>? Points { get; init; }

    bool Open { get; init; }

    bool Colliding(Vector2 pos);
    bool Colliding(TwoDimensional? shape);
    bool Intersects(Edge edge);

}
