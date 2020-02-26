using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlidePlatform : BasePlatform
{

    [SerializeField] private bool slideRight;

    public delegate void OnSlidePlatformInteract(string str);
    public static event OnSlidePlatformInteract OnSlidePlatformInteractEvent;

    public override void Interact(Person controller)
    {
        controller.hasInteracatedWithSlide = true;
        PrintObjectInteracting(controller, "Slide");

        //ParentPlayer(controller);

        CheckFallingFromHeight();

        if (slideRight)
        {
            controller.SlideMove("RIGHT");
            //Move Right
            //OnSlidePlatformInteractEvent?.Invoke("RIGHT");
        }
        else
        {
            controller.SlideMove("LEFT");
            //Move Left
            //OnSlidePlatformInteractEvent?.Invoke("LEFT");
        }
        controller.IsFreeFall = false;
        controller.currentState = PersonState.IDLE;

        
    }

    public void SetUpSlide(bool value)
    {
        slideRight = value;
    }
 
}
