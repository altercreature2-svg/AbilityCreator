using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace IDK.Help_Componets
{
    public class HoldPosition : MonoBehaviour
    {
        public PoseHandler poseHandler;
        public SetGlobalJointSettings globalJointSettings;
        public DataHandler dataHandler;
        public Rigidbody[] bodyParts;
        public ConfigurableJoint[] joints;
        public Vector3[] offsets = new Vector3[] { };
        public float time;
        public float dampen = 2;
        private float timer;
        private bool working;
        public bool free;
        public bool x;
        public bool y;
        public bool z;
        public void Update()
        {
            if (working)
            {
                if (timer > time)
                {
                    Destroy(this);
                    dataHandler.playingStaticAnim = false;
                    for (int i = 0; i < joints.Length; i++)
                    {
                        if (x)
                            joints[i].angularXMotion = ConfigurableJointMotion.Limited;
                        if (y) 
                            joints[i].angularYMotion = ConfigurableJointMotion.Limited ;
                        if (z) 
                            joints[i].angularZMotion = ConfigurableJointMotion.Limited;
                    }
                    return;             
                }
                timer += Time.deltaTime;
               

                for (int i = 0; i < joints.Length; i++)
                {
                    if (x)
                        joints[i].angularXMotion = ConfigurableJointMotion.Locked ;
                    if (y)
                        joints[i].angularYMotion = ConfigurableJointMotion.Locked ;
                    if (z)
                        joints[i].angularZMotion = ConfigurableJointMotion.Locked ;
                }
            }
        }
        public void Awake()
        {
            poseHandler = transform.root.GetComponentInChildren<PoseHandler>();
            globalJointSettings = transform.root.GetComponentInChildren<SetGlobalJointSettings>();
            dataHandler = transform.root.GetComponentInChildren<DataHandler>();
        }
        public void Go(float time, Rigidbody[] rigidbodies, bool x, bool y, bool z)
        {
            this.time = time;
            bodyParts = rigidbodies.Where(n => n).ToArray();
            joints = rigidbodies.Select(n => n.GetComponent<ConfigurableJoint>()).ToArray();
            working = true;
            this.x = x;
            this.y = y;
            this.z = z;
        }
    }
}
