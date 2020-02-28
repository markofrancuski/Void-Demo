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
        //controller.currentState = PersonState.IDLE;

        
    }

    public override void OnCollisionEnter2D(Collision2D collision)
    {
        if (isPositioned) return;
        if (collision.collider.CompareTag("Player") || collision.collider.CompareTag("AI"))
        {

            _controller = collision.collider.transform.parent.GetComponent<Person>();
            StartCoroutine(_WaitForIdleCoroutine());
        }
    }

    public override void OnCollisionExit2D(Collision2D collision)
    {
        if (!isPositioned) return;
        if (collision.collider.CompareTag("Player") || collision.collider.CompareTag("AI"))
        {

            UnParentPlayer(_controller);
            _controller = null;
        }
    }

    public void SetUpSlide(bool value)
    {
        slideRight = value;
    }

    private IEnumerator _WaitForIdleCoroutine()
    {
        yield return new WaitUntil(()=>_controller.currentState == PersonState.IDLE);
        Interact(_controller);
    }
 
}
