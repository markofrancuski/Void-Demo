using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scalable : MonoBehaviour
{
    [SerializeField] private Vector3 startingScale;
    [SerializeField] private Vector3 newScale;
    [SerializeField] private bool scale = true;
    // Start is called before the first frame update
    void Start()
    {
        startingScale = newScale = transform.localScale;
        if(scale)
        {
            newScale.x = newScale.x + (newScale.x * Globals.Instance.scaleSize.x);
            newScale.y = newScale.y + (newScale.y * Globals.Instance.scaleSize.x);
            transform.localScale = newScale;
        }

    }


}
