using System.Collections.Concurrent;
using System.Net;
using System.Numerics;
using ImGuiNET;
using Silk.NET.OpenGL;
using SkillQuest.API.ECS;
using SkillQuest.Client.Engine.Graphics.API;
using SkillQuest.Client.Engine.Input;

namespace SkillQuest.Client.Game.Addons.SkillQuest.Client.Doohickey.Gui.InGame;

public class GuiEditor : Shared.Engine.ECS.Doohickey, IDrawable {
    public override Uri? Uri { get; set; } = new Uri("ui://skill.quest/editor");

    public GuiEditor(){
        Stuffed += (stuff, thing) => {
            Stuff.ThingAdded += StuffOnThingAdded;
            Stuff.ThingRemoved += StuffOnThingRemoved;
            foreach (var t in Stuff.Things) {
                StuffOnThingAdded(t.Value);
            }
        };

        Unstuffed += (stuff, thing) => {
            Tree.Clear();
            Stuff.ThingAdded -= StuffOnThingAdded;
            Stuff.ThingRemoved -= StuffOnThingRemoved;
        };
    }

    private class DirNode{
        public string Name;
        public ConcurrentDictionary<string, DirNode> Children;
        public IThing? Thing;
    }
    
    ConcurrentDictionary<string, DirNode> Tree = new ConcurrentDictionary<string, DirNode>();
    
    void StuffOnThingAdded(IThing thing){
        var root = thing.Uri.Scheme + "://" + thing.Uri.Host;
        if (!Tree.ContainsKey(root)) Tree[root] = new DirNode() { Name = root };
        var tree = Tree[root];

        foreach (var path in thing.Uri.Segments) {
            string p = path.Trim('/');
            if (p.Length == 0) continue;
            if (tree.Children is null) tree.Children = new();
            if (!tree.Children.ContainsKey(p)) tree.Children[p] = new DirNode() { Name = p };
            tree = tree.Children[p];
            
        }
        tree.Thing = thing;
    }

    void StuffOnThingRemoved(IThing thing){
        var root = thing.Uri.Scheme + "://" + thing.Uri.Host;
        if (!Tree.ContainsKey(root)) return;
        var tree = Tree[root];

        foreach (var path in thing.Uri.Segments.Reverse().Skip(1).Reverse()) {
            string p = path.Trim('/');
            if (p.Length == 0) continue;
            if (tree.Children is null) return;
            if (!tree.Children.ContainsKey(p)) return;
            tree = tree.Children[p];
        }
        tree.Children.Remove(thing.Uri.Segments.Last(), out _);
    }

    public void Draw(DateTime now, TimeSpan delta){
        ImGui.SetNextWindowSize(ImGui.GetIO().DisplaySize);
        ImGui.SetNextWindowPos(new Vector2(0, 0));

        if (
            ImGui.Begin(
                Uri.ToString(),
                ImGuiWindowFlags.NoTitleBar |
                ImGuiWindowFlags.NoResize |
                ImGuiWindowFlags.NoCollapse |
                ImGuiWindowFlags.NoSavedSettings |
                ImGuiWindowFlags.NoMove
            )
        ) {
            ImGui.BeginChild( "#explorer", new Vector2(250, -1), ImGuiChildFlags.Border );
            foreach (var node in Tree) {
                DrawNode(node.Value);
            }
            ImGui.End();
        }
    }

    private void DrawNode(DirNode node){
        if (ImGui.TreeNodeEx(node.Name, (node.Children?.Count() ?? 0 ) == 0 ? ImGuiTreeNodeFlags.Leaf : 0)) {
            if (node.Children is not null) {
                foreach (var child in node.Children.OrderBy(child => child.Value.Name)) {
                    DrawNode(child.Value);
                }
            }

            ImGui.TreePop();
        }
    }
}