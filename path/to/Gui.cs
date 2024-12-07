// entire file content ...
public class Gui {
    // ... rest of code ...

    public void Draw(DateTime now, TimeSpan delta){
        if (ImGui.BeginChild("#explorer", new Vector2(250, -1), ImGuiChildFlags.Border)) {
            foreach (var node in Tree.OrderBy(n => n.Value.Name)) {
                DrawNode(node.Value);
            }
            ImGui.EndChild();
        }
    }

    private void DrawNode(DirNode node){
        ImGui.TreePush("#test");
        ImGui.PushID(node.Name);

        if (ImGui.Button( " " ) ) {
            node.Opened = !node.Opened;
        }

        ImGui.SameLine();
        ImGui.Text(node.Name);

        // Add this line to display the text label at the top of the tree
        if (node == Tree.Values.First().Value) {
            ImGui.SameLine();
            ImGui.Text("Test");
        }

        if (node.Opened) {
            if (node.Children is not null) {
                foreach (var child in node.Children.OrderBy(child => child.Value.Name)) {
                    DrawNode(child.Value);
                }
            }
        }

        ImGui.PopID();
        ImGui.TreePop();
    }
}
