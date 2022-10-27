# SaveData

**Namespace:** Murder.Assets \
**Assembly:** Murder.dll

```csharp
public class SaveData : GameAsset
```

Tracks a saved game with all the player status.

**Implements:** _[GameAsset](/Murder/Assets/GameAsset.html)_

### ⭐ Constructors
```csharp
protected SaveData(BlackboardTracker tracker)
```

**Parameters** \
`tracker` [BlackboardTracker](/Murder/Save/BlackboardTracker.html) \

### ⭐ Properties
#### BlackboardTracker
```csharp
public readonly BlackboardTracker BlackboardTracker;
```

**Returns** \
[BlackboardTracker](/Murder/Save/BlackboardTracker.html) \
#### CanBeCreated
```csharp
public virtual bool CanBeCreated { get; }
```

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
#### CanBeDeleted
```csharp
public virtual bool CanBeDeleted { get; }
```

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
#### CanBeRenamed
```csharp
public virtual bool CanBeRenamed { get; }
```

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
#### CanBeSaved
```csharp
public virtual bool CanBeSaved { get; }
```

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
#### CustomPath
```csharp
public virtual string CustomPath { get; }
```

**Returns** \
[string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
#### DynamicAssets
```csharp
public Dictionary<TKey, TValue> DynamicAssets { get; private set; }
```

These are all the dynamic assets within the game session.

**Returns** \
[Dictionary\<TKey, TValue\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Generic.Dictionary-2?view=net-7.0) \
#### EditorColor
```csharp
public virtual Vector4 EditorColor { get; }
```

**Returns** \
[Vector4](https://learn.microsoft.com/en-us/dotnet/api/System.Numerics.Vector4?view=net-7.0) \
#### EditorFolder
```csharp
public virtual string EditorFolder { get; }
```

**Returns** \
[string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
#### FileChanged
```csharp
public bool FileChanged;
```

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
#### FilePath
```csharp
public string FilePath { get; public set; }
```

**Returns** \
[string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
#### Guid
```csharp
public Guid Guid { get; protected set; }
```

**Returns** \
[Guid](https://learn.microsoft.com/en-us/dotnet/api/System.Guid?view=net-7.0) \
#### Icon
```csharp
public virtual char Icon { get; }
```

**Returns** \
[char](https://learn.microsoft.com/en-us/dotnet/api/System.Char?view=net-7.0) \
#### Name
```csharp
public string Name { get; public set; }
```

**Returns** \
[string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
#### SaveDataRelativeDirectoryPath
```csharp
public string SaveDataRelativeDirectoryPath { get; public set; }
```

This is save path, used by its assets.

**Returns** \
[string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
#### SavedWorlds
```csharp
public ImmutableDictionary<TKey, TValue> SavedWorlds { get; private set; }
```

This maps
             [World Guid -&gt; Saved World Guid]
            that does not belong to a run and should be persisted.

**Returns** \
[ImmutableDictionary\<TKey, TValue\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Immutable.ImmutableDictionary-2?view=net-7.0) \
#### SaveLocation
```csharp
public virtual string SaveLocation { get; }
```

**Returns** \
[string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
#### SaveName
```csharp
public string SaveName { get; public set; }
```

This is the name used in-game, specified by the user.

**Returns** \
[string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
#### StoreInDatabase
```csharp
public virtual bool StoreInDatabase { get; }
```

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
#### TaggedForDeletion
```csharp
public bool TaggedForDeletion;
```

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
### ⭐ Methods
#### TryGetDynamicAssetImpl(out T&)
```csharp
protected virtual bool TryGetDynamicAssetImpl(T& value)
```

**Parameters** \
`value` [T&]() \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### SaveWorld(Guid, MonoWorld)
```csharp
protected void SaveWorld(Guid worldGuid, MonoWorld world)
```

This saves a world that should be persisted across several runs.
            For now, this will be restricted to the city.

**Parameters** \
`worldGuid` [Guid](https://learn.microsoft.com/en-us/dotnet/api/System.Guid?view=net-7.0) \
`world` [MonoWorld](/Murder/Core/MonoWorld.html) \

#### Duplicate(string)
```csharp
public GameAsset Duplicate(string name)
```

**Parameters** \
`name` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \

**Returns** \
[GameAsset](/Murder/Assets/GameAsset.html) \

#### GetSimplifiedName()
```csharp
public string GetSimplifiedName()
```

**Returns** \
[string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \

#### TryGetDynamicAsset()
```csharp
public T TryGetDynamicAsset()
```

**Returns** \
[T]() \

#### TryLoadLevel(Guid)
```csharp
public virtual SavedWorld TryLoadLevel(Guid guid)
```

Get a world asset to instantiate in the game.

**Parameters** \
`guid` [Guid](https://learn.microsoft.com/en-us/dotnet/api/System.Guid?view=net-7.0) \

**Returns** \
[SavedWorld](/Murder/Assets/SavedWorld.html) \

#### SynchronizeWorld(Guid, MonoWorld)
```csharp
public virtual void SynchronizeWorld(Guid worldGuid, MonoWorld world)
```

Save a world state.

**Parameters** \
`worldGuid` [Guid](https://learn.microsoft.com/en-us/dotnet/api/System.Guid?view=net-7.0) \
`world` [MonoWorld](/Murder/Core/MonoWorld.html) \

#### MakeGuid()
```csharp
public void MakeGuid()
```

#### RemoveDynamicAsset(Type)
```csharp
public void RemoveDynamicAsset(Type t)
```

**Parameters** \
`t` [Type](https://learn.microsoft.com/en-us/dotnet/api/System.Type?view=net-7.0) \

#### SaveDynamicAsset(Guid)
```csharp
public void SaveDynamicAsset(Guid guid)
```

**Parameters** \
`guid` [Guid](https://learn.microsoft.com/en-us/dotnet/api/System.Guid?view=net-7.0) \



⚡