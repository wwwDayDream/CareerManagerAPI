## <center>[CareerManagerAPI](https://www.nexusmods.com/derailvalley/mods/951)</center>

---

Unifies patching of the Career Manager station to allow other modders to easily add station-specific options to the career manager.


### Usage
After basic installation of a UMM (Unity Mod Manager) mod and reference of the `CareerManagerAPI.dll` you can begin.

Provides a single event (fired on save load) for other modders to bind to and add new screens via the proxy:
```csharp

namespace CareerManagerAPI;

public static class CareerManagerAPI
{
    public static event CareerManagerMainScreenInitialized StationCareerManagerAwake;
    public delegate void CareerManagerMainScreenInitialized(CareerManagerScreenTracker screen, Transform screenRoot, string stationName);
```

You'll need to implement `IDisplayScreen` in a class as the screen your selection will go to if it succeeds.
You can bind to that anywhere after `CareerManagerAPI` is loaded like so:
```csharp
using CareerManagerAPI;

public class MyScreen : IDisplayScreen { ... }

// Defined as a callback to see if we can enter your selection
// Two methods are provided under CareerManagerAPI for your convenience: 
// public static bool AlwaysTrue(out string err)
// public static bool CareerModeOnly(out string err)
public bool CanEnter(out string err){
    if (...) {
        err = "You can't enter this secret menu option!";
        return false;
    } else {
        return true;
    }
}

// Somewhere Later 
CareerManagerAPI.StationCareerManagerAwake += (screen, screensRoot, stationName) => {
    
    // OilWellNorth, OilWellCentral, Farm, IronOreMineWest, CoalMine, IronOreMineEast, 
    // CitySW, GoodsFactory, SteelMill, MachineFactory, FoodFactory, Harbor, Sawmill, 
    // ForestSouth, ForestCentral
    if (stationName == "SteelMill") {
        // bool TryAddToMainScreen(string text, IDisplayScreen screen, CareerManagerAPI.OnCanConfirm? canConfirm = null,
        //      string? after = null, string? before = null)
        // A null CanEnter will default to CareerManagerAPI.AlwaysTrue
        // Defining *a* 'before' OR(not both) 'after' string will place your option before or after another option.  
        screen.TryAddToMainScreen<MyScreen>("Test", CanEnter, before: CareerManagerLocalization.FEES);
    }
};
```