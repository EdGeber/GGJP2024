using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameEvents
{
    public static ViewEvent CurrentView => GameEventContainer.Get<ViewEvent>("CurrentView");

    public static InteractiveEvent ObjectFocusChanged => GameEventContainer.Get<InteractiveEvent>("ObjectFocusChanged");

    public static InteractiveEvent InteractionAttempted => GameEventContainer.Get<InteractiveEvent>("InteractionAttempted");

    public static BoolEvent PlayerIsLookingDown => GameEventContainer.Get<BoolEvent>("PlayerIsLookingDown");

    public static IntEvent CurrentLevel => GameEventContainer.Get<IntEvent>("CurrentLevel");

    public static IntEvent MaxLevelReached => GameEventContainer.Get<IntEvent>("MaxLevelReached");

    public static IntEvent CurrentTabletBattery => GameEventContainer.Get<IntEvent>("CurrentTabletBattery");

    public static IntEvent MaxTabletBattery => GameEventContainer.Get<IntEvent>("MaxTabletBattery");

    public static IntEvent TabletBatteryLevel0 => GameEventContainer.Get<IntEvent>("TabletBatteryLevel0");

    public static IntEvent TabletBatteryLevel1 => GameEventContainer.Get<IntEvent>("TabletBatteryLevel1");

    public static IntEvent TabletBatteryLevel2 => GameEventContainer.Get<IntEvent>("TabletBatteryLevel2");

}
