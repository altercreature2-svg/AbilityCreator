using BitCode.Extensions;
using Landfall.TABS;
using TFBGames;
using UnityEngine;

namespace IDK
{
    public class SmartProjectileSpawner
    {
        public static GameObject SpawnProjectile(
            GameObject projectilePrefab,
            GameObject spawnPosition,
            Unit unit,
            Transform spawnRig,
            Transform mainrig,
            float spread,
            Vector3? targetPosition = null,
            Unit target = null,
            bool aimAtTarget = false)
        {
            Vector3 spawnDirection;
            if (aimAtTarget && targetPosition.HasValue)
            {
                var moveTransform = projectilePrefab.GetComponent<MoveTransform>();
                if (moveTransform != null)
                {
                    Vector3 velocity = moveTransform.worldImpulse + moveTransform.transform.TransformDirection(moveTransform.selfImpulse);
                    float gravity = moveTransform.gravity;
                    spawnDirection = CalculateBallisticDirection(
                        spawnPosition.transform.position,
                        targetPosition.Value,
                        velocity.magnitude,
                        gravity,
                        mainrig.up
                    );
                }
                else
                {
                    spawnDirection = GetSpawnDirection(spawnRig.forward, spawnRig.forward, mainrig);
                }
            }
            else
            {
                spawnDirection = GetSpawnDirection(spawnRig.forward, spawnRig.forward, mainrig);
            }

            GameObject proj = ServiceLocator.GetService<ProjectilesSpawnManager>().SpawnProjectile(
                projectilePrefab,
                spawnPosition.transform.position,
                Quaternion.LookRotation(spawnDirection + 0.01f * spread * UnityEngine.Random.insideUnitSphere),
                unit,
                0,
                spawnDirection,
                spawnRig.forward,
                target.data.mainRig,
                spawnRig.forward,
                out var projectile
            );
            SetTeamHolder(unit, proj, target.data.mainRig, spawnPosition);
            return proj;
        }

        private static Vector3 GetSpawnDirection(Vector3 directionToTarget, Vector3 forcedDirection, Transform mainrig)
        {
            Vector3 result = Vector3.Lerp(directionToTarget, mainrig.forward, new AnimationCurve().Evaluate(Vector3.Angle(directionToTarget, mainrig.forward))).normalized;
            return result;
        }

        // Ballistic trajectory calculation (returns normalized direction)
        // Assumes projectile is fired from 'origin' to hit 'target' with given velocity and gravity.
        // Returns Vector3.zero if no valid solution.
        private static Vector3 CalculateBallisticDirection(
            Vector3 origin,
            Vector3 target,
            float velocity,
            float gravity,
            Vector3 up)
        {
            Vector3 delta = target - origin;
            Vector3 deltaXZ = Vector3.ProjectOnPlane(delta, up);
            float y = Vector3.Dot(delta, up);
            float xz = deltaXZ.magnitude;
            float g = Mathf.Abs(gravity);
            float v2 = velocity * velocity;
            float gxz = g * xz;

            // Quadratic formula for angle: v^4 - g(g*x^2 + 2*y*v^2)
            float discriminant = v2 * v2 - g * (g * xz * xz + 2 * y * v2);
            if (discriminant < 0.0f || Mathf.Approximately(xz, 0f))
                return delta.normalized; // fallback: direct

            float sqrt = Mathf.Sqrt(discriminant);
            // Two possible angles (high/low arc), pick low arc for most games
            float angle = Mathf.Atan2(v2 - sqrt, gxz);
            // Direction in XZ plane
            Vector3 dirXZ = deltaXZ.normalized;
            // Compose direction vector
            Vector3 result = Quaternion.AngleAxis(Mathf.Rad2Deg * angle, Vector3.Cross(dirXZ, up)) * dirXZ;
            return result.normalized;
        }
        public static void SetTeamHolder(Unit unit, GameObject spawnedObject, Rigidbody targetRig, GameObject spawner)
        {
            if (unit)
            {
                if ((bool)unit.data)
                {

                    TeamHolder orAddComponent = spawnedObject.GetOrAddComponent<TeamHolder>();
                    orAddComponent.team = unit.data.team;
                    orAddComponent.spawner = unit.transform.root.gameObject;

                    orAddComponent.spawnerWeapon = spawner;


                    orAddComponent.target = targetRig;
                }
            }
        }

    }
}