﻿using Bang.Components;
using ImGuiNET;
using Microsoft.Xna.Framework.Input;
using System.Collections.Immutable;
using Murder.Assets;
using Murder.Core;
using Murder.Core.Graphics;
using Murder.Core.Input;
using Murder.Data;
using Murder.Serialization;
using Murder.Editor.ImGuiExtended;
using Murder.Utilities;
using Murder.Diagnostics;
using Murder.Editor.Utilities;
using Murder.Editor.CustomEditors;
using Murder.Editor.Data;
using Murder.Editor.CustomComponents;

namespace Murder.Editor
{
    public partial class EditorScene : Scene
    {
        private ImmutableArray<GameAsset> _selectedAssets = ImmutableArray<GameAsset>.Empty;

        /// <summary>
        /// Asset that has just been selected and is yet to be shown.
        /// </summary>
        private GameAsset? _selectedAsset = null;

        private GameAsset? _assetShown = null;

        /// <summary>
        /// Asset currently open and being shown.
        /// </summary>
        internal GameAsset? AssetShown => _assetShown;

        public readonly Lazy<IntPtr> PreviewTexture = new(Architect.Instance.ImGuiRenderer.GetNextIntPtr);

        public static ImFontPtr EditorFont;
        public static ImFontPtr FaFont;

        /// <summary>
        /// Initialized in <see cref="Start()"/>.
        /// </summary>
        internal IList<Type> ComponentTypes = null!;

        public override MonoWorld? World => null;

        bool _f5Lock = true;
        bool _showingMetricsWindow = false;
        bool _showStyleEditor = false;

        public uint EDITOR_DOCK_ID = 19;

        public override void Start()
        {
            ComponentTypes = new List<Type>();
            foreach (var t in ReflectionHelper.GetAllImplementationsOf<IComponent>())
            {
                ComponentTypes.Add(t);
            }

            foreach (var item in Architect.EditorSettings.OpenedTabs)
            {
                if (Game.Data.TryGetAsset(item) is GameAsset asset)
                {
                    OpenAssetEditor(asset);
                }
            }

            // Start from asset opened in the last session
            int lastSessionTab = Architect.EditorSettings.SelectedTab;
            _f5Lock = true;

            base.Start();
        }

        public override void Draw()
        {
            // We don't need to draw the world when in the editor scene
            // TODO: Pedro fix shader
            // Game.Data.SimpleShader.CurrentTechnique.Passes[0].Apply();
            Game.GraphicsDevice.SetRenderTarget(null);
            Game.GraphicsDevice.Clear(Game.Profile.Theme.Bg.ToXnaColor());
        }

        public override void DrawGui()
        {
            var screenSize = new System.Numerics.Vector2(Architect.Instance.Window.ClientBounds.Width, Architect.Instance.Window.ClientBounds.Height);
            
            var staticWindowFlags = 
                ImGuiWindowFlags.NoResize |  ImGuiWindowFlags.NoDecoration |
                ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoMove | 
                ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoBringToFrontOnFocus | 
                ImGuiWindowFlags.NoDocking ;
            
            float menuHeight;

            ImGui.Begin("Workspace", staticWindowFlags);

            ImGui.BeginMainMenuBar();
            {
                if (ImGui.MenuItem("Quick-Play", "Shift+F5"))
                {
                    SaveEditorState();
                    Architect.Instance.PlayGame(true);
                }

                if (ImGui.MenuItem("Play", "F5"))
                {
                    SaveEditorState();
                    Architect.Instance.PlayGame(false);
                }

                // If there is no lock, the player attempted to play the game.
                if (!_f5Lock && Game.Input.Pressed(MurderInputButtons.PlayGame))
                {
                    Architect.Instance.PlayGame(quickplay: Game.Input.Pressed(Keys.LeftShift) || Game.Input.Pressed(Keys.RightShift));
                }

                if (_f5Lock && !Game.Input.Pressed(MurderInputButtons.PlayGame))
                {
                    _f5Lock = false;
                }

                //if (ImGui.BeginMenu("Edit"))
                //{
                //    if (ImGui.MenuItem("Undo", "Ctrl+Z"))
                //    {
                //        Undo();
                //    }
                //    ImGui.EndMenu();
                //}

                if (ImGui.BeginMenu("Assets"))
                {
                    if (ImGui.MenuItem("Save All Assets", ""))
                    {
                        Architect.EditorData.SaveAllAssets();
                    }

                    ImGui.EndMenu();
                }
                
                if (ImGui.BeginMenu("Reload"))
                {
                    if (ImGui.MenuItem("Atlas only", "F2"))
                    {
                        Architect.PackAtlas();
                        Architect.Data.RefreshAtlas();
                    }
                    if (ImGui.MenuItem("Content and Atlas", "F3"))
                    {
                        Architect.Instance.ReloadContent();
                        AssetsFilter.RefreshCache();
                    }
                    if (ImGui.MenuItem("Window", "F4"))
                    {
                        Architect.Instance.SaveWindowPosition();
                        Architect.Instance.RefreshWindow();
                    }

                    if (ImGui.MenuItem("Shaders", "F6"))
                    {
                        Architect.Instance.ReloadContent();
                    }

                    ImGui.Separator();

                    ImGui.MenuItem("Only Reload Atlas With Changes", "", ref Architect.EditorSettings.OnlyReloadAtlasWithChanges);

                    ImGui.EndMenu();
                }

                if (ImGui.BeginMenu("Util"))
                {
                    ImGui.MenuItem("Show Metrics", "", ref _showingMetricsWindow);
                    ImGui.MenuItem("Show Style Editor", "", ref _showStyleEditor);
                    ImGui.EndMenu();
                }

                if (_showStyleEditor)
                {
                    ImGui.Begin("Style Editor", ref _showStyleEditor,ImGuiWindowFlags.AlwaysAutoResize);
                    if (ImGui.SliderFloat("Editor Scale", ref Architect.EditorSettings.FontScale, 1f, 2f))
                        ImGui.GetIO().FontGlobalScale = Math.Clamp(Architect.EditorSettings.FontScale, 1, 2);
                    
                    ImGui.End();
                }

                if (_showingMetricsWindow)
                    ImGui.ShowMetricsWindow(ref _showingMetricsWindow);

                if (Architect.Input.Shortcut(Keys.F2))
                {
                    Architect.PackAtlas();
                    Architect.Data.RefreshAtlas();
                }
                if (Architect.Input.Shortcut(Keys.F3))
                {
                    Architect.Instance.SaveWindowPosition();
                    Architect.Instance.ReloadContent();
                    Architect.Instance.RefreshWindow();
                }
                if (Architect.Input.Shortcut(Keys.F4))
                {
                    Architect.Instance.SaveWindowPosition();
                    Architect.Instance.RefreshWindow();
                }
                if (Architect.Input.Shortcut(Keys.F6))
                {
                    Architect.Instance.ReloadShaders();
                }

                menuHeight = ImGui.GetItemRectSize().Y;

                ImGui.EndMainMenuBar();
            }

            ImGui.SetWindowPos(new System.Numerics.Vector2(0, 10*ImGui.GetIO().FontGlobalScale));
            ImGui.SetWindowSize(new System.Numerics.Vector2(screenSize.X, screenSize.Y));

            ImGui.BeginChild("Workspace", new System.Numerics.Vector2(-1, -1), false);
            if (ImGui.BeginTable("Workspace", 2, ImGuiTableFlags.Resizable | ImGuiTableFlags.SizingFixedFit))
            {
                ImGui.TableSetupColumn("Explorer", ImGuiTableColumnFlags.NoSort , 300f);
                ImGui.TableSetupColumn("Editor", ImGuiTableColumnFlags.NoResize | ImGuiTableColumnFlags.WidthStretch, -1);

                ImGui.TableNextRow();
                ImGui.TableNextColumn();
                {
                    if (ImGui.BeginChild("explorer_child", ImGui.GetContentRegionAvail() - new System.Numerics.Vector2(0, ImGui.GetStyle().FramePadding.Y)))
                    {
                        ImGui.BeginTabBar("explorer");

                        if (ImGui.BeginTabItem(" Assets"))
                        {
                            DrawAssetsTab();
                            ImGui.EndTabItem();
                        }
                        DrawAtlasTab();
                        DrawSavesTab();

                        ImGui.EndTabBar();

                    }
                    ImGui.EndChild();
                }

                ImGui.TableNextColumn();
                {
                    if (ImGui.BeginChild("docker_child", ImGui.GetContentRegionAvail() - new System.Numerics.Vector2(0, ImGui.GetStyle().FramePadding.Y)))
                    {
                        ImGui.DockSpace(EDITOR_DOCK_ID);
                        // Draw asset editors
                        DrawAssetEditors();
                    }
                    ImGui.EndChild();
                }
                ImGui.EndTable();
            }
            ImGui.EndChild();
            ImGui.End();
        }

        private void Undo()
        {
            throw new NotImplementedException();
        }

        public void SaveEditorState()
        {
            Architect.EditorSettings.OpenedTabs = new Guid[_selectedAssets.Length];
            for (int i = 0; i < _selectedAssets.Length; i++)
            {
                Architect.EditorSettings.OpenedTabs[i] = _selectedAssets[i].Guid;
            }

            ((EditorDataManager)Architect.Data!).SaveAsset(Architect.EditorSettings);
        }

        private string _atlasSearchBoxTmp = string.Empty;
        private void DrawAtlasTab()
        {
            if (ImGui.BeginTabItem("Atlas"))
            {
                ImGui.SetNextItemWidth(-1);
                ImGui.InputText("##Search", ref _atlasSearchBoxTmp, 256);
                ImGui.BeginChildFrame(891237, new System.Numerics.Vector2(-1,-1));
                foreach (var atlas in Enum.GetValues(typeof(AtlasId)))
                {
                    if ((AtlasId)atlas == AtlasId.None)
                    {
                        if (ImGui.TreeNode("No Atlas"))
                        {
                            foreach (var texture in Game.Data.AvailableUniqueTextures.Where(t=>t.Contains(_atlasSearchBoxTmp)))
                            {
                                ImGui.Selectable(FileHelper.GetPathWithoutExtension(texture), false);
                                if (ImGui.IsItemHovered())
                                {
                                    ImGui.BeginTooltip();
                                    Architect.ImGuiTextureManager.DrawPreviewImage(texture, 256, null);
                                    ImGui.EndTooltip();
                                }
                            }
                            ImGui.TreePop();
                        }
                    }
                    else
                    {

                        DrawAtlasImageList((AtlasId)atlas);
                    }
                }
                ImGui.EndChildFrame();
                ImGui.EndTabItem();
            }
        }

        private void DrawAtlasImageList(AtlasId atlasId)
        {
            var atlas = Architect.Data.FetchAtlas(atlasId);

            if (ImGui.TreeNode(atlasId.GetDescription()))
            {
                foreach (var item in atlas.GetAllEntries().Where(t => t.Name.Contains(_atlasSearchBoxTmp)))
                {
                    ImGui.Selectable(item.Name);
                    if (ImGui.IsItemHovered())
                    {
                        ImGui.BeginTooltip();
                        Architect.ImGuiTextureManager.DrawPreviewImage(item.Name, 256, atlas);
                        ImGui.EndTooltip();
                    }
                }
                ImGui.Separator();
                ImGui.TreePop();
            }
        }

        int _selectedAssetToCreate = 0;

        private void DrawAssetsTab()
        {
            // Get all assets
            var assets = Architect.EditorData.GetAllAssets();

            // Draw asset tree
            DrawAssetFolder("#\uf07b", Architect.Profile.Theme.White, typeof(GameAsset), assets);
            DrawAssetInList(Architect.EditorData.EditorSettings, Game.Profile.Theme.White, Architect.EditorData.EditorSettings.Name);
            DrawAssetInList(Architect.EditorData.GameProfile, Game.Profile.Theme.White, Architect.EditorData.GameProfile.Name);

            // Button to add a new asset
            CreateAssetButton(typeof(GameAsset));
        }

        private void DrawSavesTab()
        {
            if (ImGui.BeginTabItem("Save data"))
            {
                // Get all assets
                var assets = Architect.EditorData.GetAllSaveAssets();

                // Draw asset tree
                DrawAssetFolder("#\uf07b", Architect.Profile.Theme.White, typeof(GameAsset), assets);

                if (ImGuiHelpers.FadedSelectableWithIcon($"Kill all saves", '\uf54c', false))
                {
                    Architect.EditorData.DeleteAllSaves();
                }

                ImGui.EndTabItem();
            }
        }
        
        private void DrawSelectedAtlasImage(AtlasTexture selectedAtlasImage)
        {
            ImGui.BeginGroup();
            {
                ImGui.Text(selectedAtlasImage.Name);
                ImGui.Image(PreviewTexture.Value, selectedAtlasImage.SourceRectangle.Size.ToVector2());
            }
            ImGui.EndGroup();
        }

        private bool DrawRenameModal(GameAsset? asset)
        {
            var closed = false;
            if (asset is not null)
            {
                if (ImGui.BeginPopup("Asset Name"))
                {
                    ImGui.SetWindowSize(new System.Numerics.Vector2(400, 100));
                    ImGui.Text("What's the new name?");

                    if (ImGui.IsWindowAppearing())
                        ImGui.SetKeyboardFocusHere();   
                    ImGui.InputText("", ref _newAssetName, 64, ImGuiInputTextFlags.AutoSelectAll);

                    if (ImGui.Button("Rename") || Architect.Input.Pressed(Keys.Enter))
                    {
                        asset.Name = _newAssetName;
                        asset.Rename = true;
                        ImGui.CloseCurrentPopup();

                        closed = true;
                    }
                    ImGui.SameLine();
                    if (ImGui.Button("Cancel") || Architect.Input.Pressed(Keys.Escape))
                    {
                        ImGui.CloseCurrentPopup();
                        closed = true;
                    }
                    ImGui.EndPopup();
                }
            }
            return closed;
        }
        private bool DrawDeleteModal(GameAsset asset)
        {
            var closed = false;
            if (ImGui.BeginPopup("Delete?"))
            {
                ImGui.Text("Are you sure you want to delete this asset?");
                if (ImGui.Button("OK"))
                {
                    Architect.EditorData.RemoveAsset(asset);
                    _selectedAssets = _selectedAssets.Remove(asset);
                    _selectedAsset = null;
                    ImGui.CloseCurrentPopup();
                    closed = true;
                }
                ImGui.SameLine();
                if (ImGui.Button("Cancel"))
                {
                    ImGui.CloseCurrentPopup();
                    closed = true;
                }
                ImGui.EndPopup();
            }
            
            return closed;
        }
    }
}
