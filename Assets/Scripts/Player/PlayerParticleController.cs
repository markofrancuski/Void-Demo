using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerParticleController : MonoBehaviour
{
    [SerializeField] private Transform _particleTransform;

    private void OnEnable()
    {
        gameObject.GetComponent<PlayerController>().RotateParticle += RotateParticleObject;
    }

    private void OnDisable()
    {
        gameObject.GetComponent<PlayerController>().RotateParticle -= RotateParticleObject;
    }

    private void RotateParticleObject(string where)
    {
        Debug.Log($"_particleTransform.rotation before: {_particleTransform.rotation}");
        switch (where)
        {
            case "RIGHT":
                _particleTransform.rotation = Quaternion.Euler(0, 0, 0);
                break;
            case "LEFT":
                _particleTransform.rotation = Quaternion.Euler(0, 0, 180);
                break;
            case "UP":
                _particleTransform.rotation = Quaternion.Euler(0, 0, 90);
                break;
            case "DOWN":
                _particleTransform.rotation = Quaternion.Euler(0, 0, 270);
                break;
            default:
                break;
        }
        Debug.Log($"_particleTransform.rotation after: {_particleTransform.rotation}");
    }
}
