using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

namespace AC
{
    public class Reapeter : MonoBehaviour
    {
        public class RepeaterTask
        {
            public int timesLeft;
            public float interval;
            public float timeToNext;
            public Action<int> action;
            public RepeaterTask(int times, float interval, Action<int> action)
            {
                timesLeft = times;
                this.interval = Mathf.Max(0f, interval);
                timeToNext = this.interval;
                this.action = action;
            }
        }

        public List<RepeaterTask> queue = new List<RepeaterTask>();
        public void AddTask(int times, float interval, Action<int> action)
        {
            queue.Add(new RepeaterTask(times, interval, action));
        }
        public void Update()
        {
            float dt = Time.deltaTime;
            for (int i = 0; i < queue.Count; i++)
            {
                var task = queue[i];
                task.timeToNext -= dt;
                while (task.timeToNext <= 0f)
                {
                    try
                    {
                        task.action?.Invoke(i);
                    }
                    catch (Exception ex)
                    {
                        Debug.LogException(ex);
                    }

                    if (task.timesLeft > 0)
                        task.timesLeft--;

                    if (task.timesLeft == 0)
                    {
                        queue.RemoveAt(i);
                        break;
                    }

                    task.timeToNext += task.interval;
                }
            }
        }
    }
}
