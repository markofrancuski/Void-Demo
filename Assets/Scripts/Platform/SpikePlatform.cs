using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikePlatform : BasePlatform
{

    public override void Interact(Person controller)
    {
        if (controller.PlayerWon) return;
        controller.hasInteracatedWithSlide = false;
        //ParentPlayer(controller);
        //PrintObjectInteracting(controller, "Spike");
        controller.Death("Spike Platform");
    }
}
