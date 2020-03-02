using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HayPlatform : BasePlatform
{
    [SerializeField] private GameObject _hayParticles;

    public override void Interact(Person controller)
    {
        if(controller.IsFreeFall)
        {
            GameObject go = Instantiate(_hayParticles);
            go.transform.position = transform.position;
        }

        //PrintObjectInteracting(controller, "Hay");
        if (controller.currentState != PersonState.STUNNED && controller.currentState != PersonState.DEAD) controller.currentState = PersonState.IDLE;
        ParentPlayer(controller);
        controller.IsFreeFall = false;
        controller.isDeadFromFall = false;
    }
}
