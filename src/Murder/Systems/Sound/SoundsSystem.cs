﻿using Bang.Contexts;
using Bang.Systems;
using Bang.Entities;
using Bang;
using System.Collections.Immutable;
using Murder.Services;
using Murder.Components;

namespace Murder.Systems
{
    [Watch(typeof(SoundComponent))]
    internal class SoundsSystem : IReactiveSystem
    {
        public void OnAdded(World world, ImmutableArray<Entity> entities)
        {
            foreach (var e in entities)
            {
                var sound = e.GetSound();
                _ = SoundServices.PlaySound(sound.Guid);

                if (sound.DestroyEntity)
                {
                    e.Destroy();
                }
                else
                {
                    e.RemoveSound();
                }
            }
        }

        public void OnModified(World world, ImmutableArray<Entity> entities)
        { }

        public void OnRemoved(World world, ImmutableArray<Entity> entities)
        { }
    }
}
