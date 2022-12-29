# EffectsServices

**Namespace:** Murder.Services \
**Assembly:** Murder.dll

```csharp
public static class EffectsServices
```

### ⭐ Methods
#### FadeIn(World, float, Color, bool)
```csharp
public void FadeIn(World world, float time, Color color, bool destroyAfterFinished)
```

Add an entity which will apply a "fade-in" effect. Darkening the screen to black.

**Parameters** \
`world` [World](/Bang/World.html) \
`time` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`color` [Color](/Murder/Core/Graphics/Color.html) \
`destroyAfterFinished` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### FadeOut(World, float, Color, float, bool)
```csharp
public void FadeOut(World world, float time, Color color, float delay, bool destroyAfterFinished)
```

Add an entity which will apply a "fade-out" effect. Clearing the screeen.

**Parameters** \
`world` [World](/Bang/World.html) \
`time` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`color` [Color](/Murder/Core/Graphics/Color.html) \
`delay` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`destroyAfterFinished` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \



⚡