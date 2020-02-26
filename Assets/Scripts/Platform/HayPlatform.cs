using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HayPlatform : BasePlatform
{

    public override void Interact(Person controller)
    {
        //PrintObjectInteracting(controller, "Hay");
        controller.hasInteracatedWithSlide = false;
        if (controller.currentState != PersonState.STUNNED && controller.currentState != PersonState.DEAD) controller.currentState = PersonState.IDLE;
        ParentPlayer(controller);
        controller.IsFreeFall = false;
        controller.isDeadFromFall = false;
    }
}
