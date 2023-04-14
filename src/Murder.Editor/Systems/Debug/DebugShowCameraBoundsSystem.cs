﻿using Bang.Contexts;
using Bang.Systems;
using Murder.Core.Geometry;
using Murder.Core.Graphics;
using Murder.Core.Input;
using Murder.Editor.Components;
using Murder.Editor.Utilities;
using Murder.Services;
using Murder.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Murder.Editor.Systems.Debug
{
    public class DebugShowCameraBoundsSystem : IMonoRenderSystem, IUpdateSystem
    {
        
        public void Draw(RenderContext render, Context context)
        {
            EditorHook? editorHook = context.World.TryGetUnique<EditorComponent>()?.EditorHook;
            if (editorHook is null || editorHook.DrawCameraBounds is not EditorHook.CameraBoundsInfo info)
            {
                return;
            }

            Rectangle cameraRect = new(render.Camera.Position.X + render.Camera.HalfWidth - Game.Profile.GameWidth / 2f + info.Offset.X,
                render.Camera.Position.Y + render.Camera.Height / 2f - Game.Profile.GameHeight / 2f + info.Offset.Y, Game.Profile.GameWidth, Game.Profile.GameHeight);

            render.GameUiBatch.DrawRectangleOutline(cameraRect + new Point(0, 1), Game.Profile.Theme.Bg, 3, 0.5f);
            render.GameUiBatch.DrawRectangleOutline(cameraRect, Game.Profile.Theme.HighAccent, 2, 0.45f);

            render.GameUiBatch.DrawRectangle(new Rectangle(
                render.Camera.Bounds.X, render.Camera.Bounds.Y,
                render.Camera.Bounds.Width + 2,
                cameraRect.Y - render.Camera.Bounds.Y), Game.Profile.Theme.Bg * 0.4f, 0.995f);

            render.GameUiBatch.DrawRectangle(new Rectangle(
                render.Camera.Bounds.X, cameraRect.Y,
                cameraRect.X - render.Camera.Bounds.X,
                cameraRect.Height), Game.Profile.Theme.Bg * 0.4f, 0.995f);

            render.GameUiBatch.DrawRectangle(new Rectangle(
                cameraRect.X + cameraRect.Width, cameraRect.Y,
                render.Camera.Bounds.Width,
                cameraRect.Height), Game.Profile.Theme.Bg * 0.4f, 0.995f);

            render.GameUiBatch.DrawRectangle(new Rectangle(
                render.Camera.Bounds.X,cameraRect.Y + cameraRect.Height,
                render.Camera.Bounds.Width + 2,
                render.Camera.Bounds.Height - cameraRect.Height - cameraRect.Y + render.Camera.Bounds.Y + 2), Game.Profile.Theme.Bg * 0.4f, 0.995f);

            Point handleSize = new Point(98, 12);
            info.HandleArea = new(cameraRect.TopLeft - new Point(0, handleSize.Y), handleSize);
            render.GameUiBatch.DrawRectangle(info.HandleArea.Value, Game.Profile.Theme.HighAccent, 0.45f);
            Game.Data.PixelFont.Draw(render.GameUiBatch, "CAMERA REAL SIZE", 1, cameraRect.TopLeft - new Point(-4, handleSize.Y - 4), 0.44f, Game.Profile.Theme.Bg);
        }

        public void Update(Context context)
        {
            EditorHook? editorHook = context.World.TryGetUnique<EditorComponent>()?.EditorHook;
            if (editorHook is null || editorHook.DrawCameraBounds is not EditorHook.CameraBoundsInfo info)
            {
                return;
            }

            if (info.HandleArea.HasValue)
            {
                if (info.ResetCameraBounds)
                {
                    info.ResetCameraBounds = false;
                    info.CenterOffset = Point.Zero;
                    info.Offset = Point.Zero;
                }

                if (info.HandleArea.Value.Contains(editorHook.CursorWorldPosition))
                {
                    editorHook.Cursor = EditorHook.CursorStyle.Hand;
                    if (Game.Input.Pressed(MurderInputButtons.LeftClick))
                    {
                        info.CenterOffset = editorHook.CursorWorldPosition - info.Offset;
                        info.Dragging = true;
                        editorHook.UsingCursor = true;
                    }
                }

                if (!Game.Input.Down(MurderInputButtons.LeftClick))
                {
                    editorHook.UsingCursor = false;
                    info.Dragging = false;
                }

                if (info.Dragging)
                {
                    info.Offset = editorHook.CursorWorldPosition - info.CenterOffset;
                }
            }
        }
    }
}