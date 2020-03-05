using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakablePlatform : BasePlatform, IDestroyable
{
    private void EnablePlatform() => gameObject.SetActive(true);

    /// <summary>
    /// Logic for the Interaction with breakable platform.
    /// </summary>
    /// <param name="controller"> Player who interacts with platform. </param>
    public override void Interact(Person controller)
    {
        controller.hasInteracatedWithSlide = false;
        //PrintObjectInteracting(controller, "Breakable");
        if (controller.IsFreeFall)
        {
            //controller.AddFirstMove("DOWN");
            gameObject.SetActive(false);
            Invoke("EnablePlatform", 2f);
            if(_controller != null) _controller.EnterInFreeFall();
        }
        else
        {
            ParentPlayer(controller);
        }

        //base.Interact(controller);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            Interact(collision.gameObject.transform.parent.GetComponent<Person>());
        }
    }

    public override void DestroyObject(string from)
    {
        base.DestroyObject(from);
        Debug.Log("Destroying Breakable platform");
        EnterFreeFall();
        gameObject.SetActive(false);
        Invoke("EnablePlatform", 2f);
    }

    protected override void Start()
    {
        base.Start();
        /*Vector2 parentSize = gameObject.transform.parent.transform.localScale;
        triggerCollider.size = new Vector2(parentSize.x * 0.8f, 0.8f);
        triggerCollider.offset += new Vector2(0, 0.2f);*/
       
    }

}
