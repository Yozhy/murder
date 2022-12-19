﻿using Bang;
using Bang.Contexts;
using Bang.Entities;
using Bang.Systems;
using System.Collections.Immutable;
using Murder.Components;
using Murder.Core.Dialogs;
using Murder.Diagnostics;
using Murder.Messages;
using Murder.Save;
using Murder.Utilities;
using Murder.Assets;

namespace Murder.Systems
{
    [Filter(typeof(RuleWatcherComponent))]
    [Watch(typeof(RuleWatcherComponent))]
    internal class InteractOnRuleMatchSystem : IStartupSystem, IReactiveSystem
    {
        public ValueTask Start(Context context)
        {
            GameLogger.Verify(context.World.TryGetUniqueEntity<RuleWatcherComponent>() is not Entity,
                "Why did we already add an existing rule watcher component!?");

            _ = context.World.AddEntity(new RuleWatcherComponent());
            return default;
        }

        public ValueTask OnModified(World world, ImmutableArray<Entity> entities)
        {
            CheckAndTriggerRules(world);
            return default;
        }

        public ValueTask OnAdded(World world, ImmutableArray<Entity> entities)
        {
            CheckAndTriggerRules(world);
            return default;
        }

        public ValueTask OnRemoved(World world, ImmutableArray<Entity> entities)
        {
            return default;
        }

        private void CheckAndTriggerRules(World world)
        {
            if (MurderSaveServices.TryGetSave() is not SaveData save)
            {
                return;
            }
            
            BlackboardTracker tracker = save.BlackboardTracker;

            // Fetch all entities which might be affected by a rule change.
            ImmutableArray<Entity> interactives = world.GetEntitiesWith(typeof(InteractOnRuleMatchComponent));
            foreach (Entity e in interactives)
            {
                bool match = true;

                // Match each of its requirements.
                InteractOnRuleMatchComponent ruleComponent = e.GetInteractOnRuleMatch();
                foreach (CriterionNode node in ruleComponent.Requirements)
                {
                    if (!tracker.Matches(node.Criterion, /* character */ null, out int weight) && 
                        node.Kind == CriterionNodeKind.And)
                    {
                        // Nope, give up.
                        match = false;
                        break;
                    }
                }

                // If we have a match, trigger the rule and clean up the rule triggers.
                if (match)
                {
                    if (e.HasInteractive())
                    {
                        e.SendMessage(new InteractMessage(e));
                    }
                }
            }
        }
    }
}