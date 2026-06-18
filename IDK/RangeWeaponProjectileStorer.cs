using Landfall.TABS;
using System.Collections;
using UnityEngine;

namespace AC
{
    public class RangeWeaponProjectileStorer : MonoBehaviour
    {
        public RangeWeapon rangeWeapon;
        public GameObject lastProjectile;
        public void Awake()
        {
            rangeWeapon = GetComponent<RangeWeapon>();
        }
    }
}