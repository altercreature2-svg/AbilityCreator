using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace AC
{
    public class SurfaceEvent : ProjectileSurfaceEffect
    {
        [Serializable]
        public class OnHitEvent : UnityEvent<GameObject>
        {
        }

        public OnHitEvent onBlock = new OnHitEvent();

        public override bool DoEffect(HitData hit, GameObject projectile)
        {
            Debug.Log("BLOCKING");
            onBlock?.Invoke(projectile);
            return true;
        }
    }
}