using Godot;
using System.Collections.Generic;
using System.Linq;

namespace SkillQuest.API.Geometry;

public class Polygon : TwoDimensional{
    public Polygon(List<Edge> edges){
        if (edges.Count == 0) {
            Open = true;
            this.Center = null;
            this.Edges = null;
            this.Points = null;
            return;
        }

        Dictionary<Vector2, int> pairs = new();

        foreach (Edge edge in edges) {
            if (edge.PointA == null || edge.PointB == null) {
                Open = true;
                this.Edges = null;
                this.Points = null;
                this.Center = null;
                return;
            }
            var pointA = edge.PointA;
            var pointB = edge.PointB;

            pairs[pointA] = pairs.ContainsKey(pointA) ? pairs[pointA] + 1 : 1;
            pairs[pointB] = pairs.ContainsKey(pointB) ? pairs[pointB] + 1 : 1;
        }

        foreach (var pair in pairs) {
            if (pair.Value != 2) {
                Open = true;
                this.Edges = null;
                this.Points = null;
                this.Center = null;
                return;
            }
        }

        this.Points = pairs.Keys.ToList();
        this.Edges = edges;

        Min = new Vector2(
            Points.Min(vec => vec.X),
            Points.Min(vec => vec.Y)
        );

        Max = new Vector2(
            Points.Max(vec => vec.X),
            Points.Max(vec => vec.Y)
        );
        this.Center = Min + (Max - Min * 0.5f);
        this.Open = pairs.Count != edges.Count;
    }

    public Vector2 Min { get; init; }

    public Vector2 Max { get; init; }

    public Vector2? Center { get; init; }

    public List<Edge> Edges { get; init; }

    public List<Vector2>? Points { get; init; }

    public bool Open { get; init; }

    Rect? _bounds = null;

    public Rect Bounds {
        get {
            return _bounds ??= new Rect(
                new Vector2(
                    Points.AsParallel().Min(vec => vec.X),
                    Points.AsParallel().Min(vec => vec.Y)
                ),
                new Vector2(
                    Points.AsParallel().Max(vec => vec.X),
                    Points.AsParallel().Max(vec => vec.Y)
                )
            );
        }
    }


    public bool Colliding(Vector2 pos){
        return Edges.Where(
            edge => {
                Vector2 vc = edge.PointA;
                Vector2 vn = edge.PointB;

                return ((vc.Y > pos.Y && vn.Y < pos.Y) || (vc.Y < pos.Y && vn.Y > pos.Y)) &&
                       (pos.X < (vn.X - vc.X) * (pos.Y - vc.Y) / (vn.Y - vc.Y) + vc.X);
            }
        ).Count() % 2 == 1;
    }

    public bool Colliding(TwoDimensional? other){
        if (other is null) return false;
        if (this.Points is null) return false;
        if (other.Points is null) return false;

        return
            other.Colliding(this.Points.First()) // Check if inside
            || Colliding(other.Points.First()) // Check if inside
            || Edges.AsParallel().Any( // Check if intersecting
                thisEdge => other.Edges.AsParallel().Any(
                    thisEdge.Intersects
                )
            );
    }

    public bool Intersects(Edge edge) => Edges.AsParallel().Any(edge.Intersects);
}
