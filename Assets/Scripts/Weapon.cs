using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Weapon : MonoBehaviour
{
    private PlayerInputManager _player;

    [SerializeField] private Transform _spawnPoint;
    [SerializeField] private WeaponData _weaponData;

    public WeaponData WeaponData { get { return _weaponData; } }
    public Transform SpawnPoint { get { return _spawnPoint; } }

    private void OnEnable()
    {
        _player = GetComponentInParent<PlayerInputManager>();
        _player.FireWeapon += Fire;
    }

    private void Fire(Vector3 target)
    {
        Projectile bullet = Instantiate(_weaponData._bulletPrefab, _spawnPoint.transform.position, _spawnPoint.transform.rotation);
        bullet.LoadProjectile(target, _weaponData);
    }
}
