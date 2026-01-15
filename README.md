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
CareerManagerAPI.CareerManagerAwake += (tracker, locationName, station, trainCar) => {
    // station and trainCar are exclusive; A Career Manager can be at a station or on a trainCar.
    // locationName can be any of the following (and maybe more if other mods do weird stuff)
    // Oil Well North, Oil Well Central, Farm, Iron Ore Mine West, Military Base, Coal Mine, Iron Ore Mine East, 
    // Coal Mine South, City West, Goods Factory & Town, Steel Mill, Machine Factory & Town, Food Factory & Town, 
    // Coal Power Plant, Oil Refinery, Harbor & Town, Sawmill, Forest South, Forest Central, City South, 
    // Caboose Red
    if (locationName == "Steel Mill") {
        // bool TryAddToMainScreen(string text, IDisplayScreen screen, CareerManagerAPI.OnCanConfirm? canConfirm = null,
        //      string? after = null, string? before = null)
        // A null CanEnter will default to CareerManagerAPI.AlwaysTrue
        // Defining *a* 'before' OR(not both) 'after' string will place your option before or after another option.  
        tracker.TryAddToMainScreen<MyScreen>("Test", CanEnter, before: CareerManagerLocalization.FEES);
    }
};
```