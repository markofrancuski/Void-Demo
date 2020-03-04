using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShooterEnemy : MonoBehaviour
{

    [SerializeField] private int spawnPosition;

    [Header("Shoot Settings")]
    public ShootDirection projectileDirection;
    [SerializeField] private float _projectileSpeed;
    [Range(0.1f, 3f)]
    public float fireRate;
    [SerializeField]private float _tempFireRate;

    [SerializeField] private float WarnSignTimer;
    [SerializeField]private float _tempWarnSignTimer;

    [SerializeField] private Transform _projectileSpawnPosition;
    [SerializeField] private GameObject _projectilePrefab;
    [SerializeField] private Quaternion _angle;

    private bool active = true;
    // Start is called before the first frame update
    void Start()
    {
        _tempFireRate = fireRate;
        RotateProjectile();
       
    }

    // Update is called once per frame
    void Update()
    {
        if (!InputManager.Instance.isControllable || !active) return;

        if (_tempFireRate <= 0) FireUpProjectile();
        else _tempFireRate -= Time.deltaTime;
        
    }

    /// <summary>
    /// Fires up a projectile.
    /// </summary>
    public void FireUpProjectile()
    {
        GameObject GO = Instantiate(_projectilePrefab) as GameObject;
        GO.transform.position = _projectileSpawnPosition.position;
        GO.transform.rotation = _angle;

        GO.GetComponent<Projectile>().SetUpProjectile(2, GetMoveDirection(), _projectileSpeed);
        _tempFireRate = fireRate;

    }

    public void TurnActive() => active = !active;

    public bool GetActive() { return active; }

    private void RotateProjectile()
    {
        switch (projectileDirection)
        {
            case ShootDirection.UP:
                _angle = Quaternion.Euler(new Vector3(0, 0, 90));
                break;
            case ShootDirection.DOWN:
                _angle = Quaternion.Euler(new Vector3(0, 0, -90));
                break;
            case ShootDirection.RIGHT:
                _angle = Quaternion.Euler(new Vector3(0, 0, 0));
                break;
            case ShootDirection.LEFT:
                _angle = Quaternion.Euler(new Vector3(0, 0, -180));
                break;
        }
        
    }

    Vector2 GetMoveDirection()
    {
        switch (projectileDirection)
        {
            case ShootDirection.UP: return Vector2.up;
            case ShootDirection.DOWN: return Vector2.down;
            case ShootDirection.RIGHT: return Vector2.right;
            case ShootDirection.LEFT: return Vector2.left;
            default: return Vector2.right;

        }
    }

    public void SetUpShooter(ShooterInfo info)
    {
        projectileDirection = info.shootDirection;
        if(projectileDirection == ShootDirection.DOWN) transform.rotation = Quaternion.Euler(0, 0, -90);
        if(projectileDirection == ShootDirection.UP) transform.rotation = Quaternion.Euler(0, 0, 90);

        _projectileSpeed = info.projectileSpeed;

        transform.position = info.pos;

        _tempFireRate = fireRate = info.fireRate;

    }

    private void WarnSign()
    {

    }

}
