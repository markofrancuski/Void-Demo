using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalPlatform : BasePlatform
{

    public override void Interact(Person controller)
    {
        DoubleLevel.Instance.Switch();
        //base.Interact(controller);
    }

}
