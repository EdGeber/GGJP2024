using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameEvents
{
    public static ViewEvent CurrentView => GameEventContainer.Get<ViewEvent>("CurrentView");

    public static InteractiveEvent ObjectFocusChanged => GameEventContainer.Get<InteractiveEvent>("ObjectFocusChanged");

    public static InteractiveEvent InteractionAttempted => GameEventContainer.Get<InteractiveEvent>("InteractionAttempted");

}
