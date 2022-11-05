﻿using Bang.Contexts;
using Bang.Entities;
using Bang.Systems;
using Bang;
using System.Collections.Immutable;
using Murder.Core.Geometry;
using Murder.Components;
using Murder.Utilities;

namespace Road.Systems
{
    [Filter(typeof(RouteComponent))]
    [Watch(typeof(RouteComponent))]
    public class PathfindRouteSystem : IFixedUpdateSystem, IReactiveSystem
    {
        public ValueTask FixedUpdate(Context context)
        {
            foreach (Entity e in context.Entities)
            {
                //if (!e.HasPathfind())
                //{
                //    continue;
                //}

                // We might have deleted the MoveTo component in MoveToSystem.
                MoveToComponent? moveToComponent = e.TryGetMoveTo();

                PositionComponent position = e.GetPosition().GetGlobalPosition();
                Vector2 currentTarget = moveToComponent?.Target ?? position.ToVector2();

                if (position.IsSameCell(currentTarget.ToPosition()))
                {
                    PathfindComponent pathfindComponent = e.GetPathfind();

                    Vector2 pathfindTarget = pathfindComponent.Target;
                    Point cell = position.CellPoint();

                    if (cell == pathfindTarget.ToGridPoint())
                    {
                        e.SetMoveTo(new MoveToComponent(pathfindTarget, pathfindComponent.MaxSpeed, pathfindComponent.Accel));

                        // We have reached our goal, snap to it!
                        e.RemovePathfind();
                        e.RemoveRoute();

                        continue;
                    }

                    // Look for our next target tile.
                    RouteComponent route = e.GetComponent<RouteComponent>();

                    if (!route.Nodes.ContainsKey(cell))
                    {
                        // for some reason, we went off-route. so just improvise and go somewhere else.
                        // TODO: do something clever than this?
                        PathfindComponent pathfind = pathfindComponent;

                        e.SetPathfind(pathfind);
                        e.RemoveRoute();

                        continue;
                    }

                    TargetEntityTo(e, route.Nodes[cell]);
                }
            }

            return default;
        }

        public ValueTask OnAdded(World world, ImmutableArray<Entity> entities)
        {
            foreach (var e in entities)
            {
                // Did we delete this because we already got to the target position?
                if (e.TryGetRoute() is RouteComponent route)
                {
                    TargetEntityTo(e, route.Nodes[route.Initial]);
                }
            }

            return default;
        }

        public ValueTask OnModified(World world, ImmutableArray<Entity> entities)
        {
            foreach (var e in entities)
            {
                if (e.TryGetRoute() is RouteComponent route)
                {
                    TargetEntityTo(e, route.Nodes[route.Initial]);
                }
            }

            return default;
        }

        public ValueTask OnRemoved(World world, ImmutableArray<Entity> entities)
        {
            return default;
        }

        private void TargetEntityTo(Entity e, Point nextCell)
        {
            if (e.TryGetPathfind() is PathfindComponent pathfind)
            {
                e.SetMoveTo(
                    new MoveToComponent(nextCell.FromCellToVector2CenterPosition(), pathfind.MaxSpeed, pathfind.Accel));
            }
        }
    }
}