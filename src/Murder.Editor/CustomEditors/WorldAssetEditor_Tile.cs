using ImGuiNET;
using Murder.Assets.Graphics;
using Murder.Components;
using Murder.Diagnostics;
using Murder.Editor.ImGuiExtended;
using Murder.Editor.Stages;
using Murder.Editor.Utilities;
using Murder.Prefabs;

namespace Murder.Editor.CustomEditors
{
    internal partial class WorldAssetEditor
    {
        private bool DrawTileEditor(Stage stage)
        {
            GameLogger.Verify(_world is not null);

            bool modified = false;

            if (ImGui.Button("Add room!"))
            {
                modified = true;
            }

            IList<IEntity> rooms = stage.FindTileEntities();
            if (rooms.Count > 0)
            {
                using TableMultipleColumns table = new("editor_settings", flags: ImGuiTableFlags.SizingFixedFit | ImGuiTableFlags.BordersOuter,
                (ImGuiTableColumnFlags.WidthFixed, -1), (ImGuiTableColumnFlags.WidthStretch, -1));

                ImGui.TableNextRow();
                ImGui.TableNextColumn();

                foreach (IEntity room in rooms)
                {
                    ImGui.Text("Tileset");
                    ImGui.TableNextColumn();

                    MapThemeComponent map = (MapThemeComponent)room.GetComponent(typeof(MapThemeComponent));

                    TilesetAsset? tileset = Game.Data.TryGetAsset<TilesetAsset>(map.Tileset);
                    if (tileset is not null)
                    {
                        AssetsHelpers.DrawPreview(tileset);
                    }

                    ImGui.TableNextRow();

                    ImGui.TableNextColumn();
                    ImGui.Text("Floor");
                    ImGui.TableNextColumn();

                    AsepriteAsset? floor = Game.Data.TryGetAsset<AsepriteAsset>(map.Floor);
                    if (floor is not null)
                    {
                        AssetsHelpers.DrawPreview(floor);
                    }

                    ImGui.TableNextRow();
                }
            }

            return modified;
        }
    }
}