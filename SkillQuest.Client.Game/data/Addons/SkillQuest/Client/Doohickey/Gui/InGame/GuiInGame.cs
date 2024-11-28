using System.Numerics;
using ImGuiNET;
using Silk.NET.Maths;
using SkillQuest.API.ECS;
using SkillQuest.API.Geometry;
using SkillQuest.API.Geometry.Random;
using SkillQuest.Client.Engine.Graphics.API;
using SkillQuest.Client.Game.Addons.SkillQuest.Client.Doohickey.Gui.LoginSignup;
using SkillQuest.Shared.Engine.Thing.Character;
using SkillQuest.Shared.Engine.Thing.Universe;

namespace SkillQuest.Client.Game.Addons.SkillQuest.Client.Doohickey.Gui.InGame;

public class GuiInGame : Shared.Engine.ECS.Doohickey, IDrawable{
    public override Uri? Uri { get; set; } = new Uri("ui://skill.quest/ingame");

    private WorldPlayer _localhost;

    public World World;

    public VoronoiPolygons Voronoi = new VoronoiPolygons(0, 25);
    bool initVoronoi = false;

    public GuiInGame(WorldPlayer localhost){
        _localhost = localhost;
        World = new World(_localhost);
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
                ImGuiWindowFlags.NoSavedSettings
            )
        ) {

            if (ImGui.Button($"WIN @ {_localhost.Name}")) {
                Console.WriteLine($"YOU WON {_localhost.Name}");
                _localhost.Connection.Disconnect();
                Stuff.Add(new GuiMainMenu());
                Stuff?.Remove(this);
            }

            if (!initVoronoi) {
                initVoronoi = true;

                Task.Run(() => {
                    var io = ImGui.GetIO();

                    Voronoi.Add(
                        new Rect(
                            new Vector2D<float>(100, 100),
                            new Vector2D<float>(io.DisplaySize.X - 100, io.DisplaySize.Y - 100
                            )
                        )
                    );
                });
            }

            foreach (var polygon in Voronoi.Polygons) {
                foreach (var edge in polygon.Edges) {
                    ImGui.GetWindowDrawList().AddLine(edge.PointA.ToSystem(), edge.PointB.ToSystem(), 0xFFFF00FF);
                }
            }

            ImGui.End();
        }
    }
}
