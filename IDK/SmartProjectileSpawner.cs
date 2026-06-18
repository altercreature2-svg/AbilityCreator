using BitCode.Extensions;
using Landfall.TABS;
using Landfall.TABS.RuntimeCleanup;
using TFBGames;
using UnityEngine;

namespace AC
{
    public class SmartProjectileSpawner
    {
        private static ProjectilesSpawnManager spawnManager;
        private static RuntimeGarbageCollector runtimeGC;
        private static bool init;
        private static void Init()
        {
            if (!init)
                return;
            spawnManager = ServiceLocator.GetService<ProjectilesSpawnManager>();
            runtimeGC = ServiceLocator.GetService<RuntimeGarbageCollector>();
            init = true;
        }
        public static GameObject SpawnProjectile(
            GameObject projectilePrefab,
            GameObject spawnPosition,
            Unit unit,
            Transform spawnRig,
            Transform mainrig,
            float spread,
            Vector3? targetPosition = null,
            Unit target = null,
            bool aimAtTarget = false,
            GameObject pooledProjectile = null)
        {
            Init();
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

            GameObject proj = SpawnProjectile(
                projectilePrefab,
                spawnPosition.transform.position,
                Quaternion.LookRotation(spawnDirection + 0.01f * spread * UnityEngine.Random.insideUnitSphere),
                unit,
                0,
                spawnDirection,
                spawnRig.forward,
                target.data.mainRig,
                spawnRig.forward,
                out var projectile,
                pooledProjectile
            );
            SetTeamHolder(unit, proj, target.data.mainRig, spawnPosition);
            return proj;
        }

        private static Vector3 GetSpawnDirection(Vector3 directionToTarget, Vector3 forcedDirection, Transform mainrig)
        {
            Vector3 result = Vector3.Lerp(directionToTarget, mainrig.forward, new AnimationCurve().Evaluate(Vector3.Angle(directionToTarget, mainrig.forward))).normalized;
            return result;
        }
        // totally not ai (i suck at math man dont judge me)
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

        public static GameObject SpawnProjectile(GameObject prefab, Vector3 position, Quaternion rotation, Unit unit, byte weaponIndex, Vector3 spawnDirection, Vector3 directionToTarget, Rigidbody targetRigidbody, Vector3 shootPositionForward, out Projectile projectile, bool isSpawnedFromPrefabId = false, byte? randomSeed = null, GameObject pooled = null)
        {
            GameObject gameObject;
            if (pooled)
                gameObject = pooled;
            else
            {
                gameObject = UnityEngine.Object.Instantiate(prefab, position, rotation);
                runtimeGC.AddGameObject(gameObject);
            }
            

            if (gameObject == null)
            {
                projectile = null;
                return null;
            }

            projectile = gameObject.GetComponent<Projectile>();
            if (projectile == null)
            {
                Debug.LogErrorFormat("Projectile \"{0}\" does not have the \"{1}\" component.", gameObject.name, "Projectile");
                return gameObject;
            }

            projectile.DestroyedOrReturnedToPool += OnDestroyedOrReturnedToPool;
            if (randomSeed.HasValue)
            {
                projectile.RandomSeed = randomSeed;
            }
            else if (!isSpawnedFromPrefabId)
            {
                if (projectile.GetComponentInChildren<Compensation>() != null || projectile.GetComponentInChildren<MoveTransform>() != null)
                {
                    projectile.RandomSeed = (byte)UnityEngine.Random.Range(0, 32);
                }
                else
                {
                    projectile.RandomSeed = null;
                }
            }
            ((ProjectilesSpawnManager.SpawnedProjectileEventHandler)spawnManager.GetField("SpawnedProjectile")).Invoke(projectile, unit, weaponIndex, spawnDirection, directionToTarget, targetRigidbody, shootPositionForward, isSpawnedFromPrefabId);
            return gameObject;
        }
        private static void OnDestroyedOrReturnedToPool(Projectile projectile)
        {
            if (!(projectile == null))
            {
                projectile.DestroyedOrReturnedToPool -= OnDestroyedOrReturnedToPool;
                ((System.Action<Projectile>)spawnManager.GetField("DestroyedProjectile")).Invoke(projectile);
            }
        }
    }
}