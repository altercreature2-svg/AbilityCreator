using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

namespace IDK
{
    public class Reapeter : MonoBehaviour
    {
        public class RepeaterTask
        {
            public int timesLeft;
            public float interval;
            public float timeToNext;
            public Action action;
            public RepeaterTask(int times, float interval, Action action)
            {
                timesLeft = times;
                this.interval = Mathf.Max(0f, interval);
                timeToNext = this.interval;
                this.action = action;
            }
        }

        public List<RepeaterTask> queue = new List<RepeaterTask>();
        public void AddTask(int times, float interval, Action action)
        {
            queue.Add(new RepeaterTask(times, interval, action));
        }
        public void Update()
        {
            float dt = Time.deltaTime;
            for (int i = queue.Count - 1; i >= 0; i--)
            {
                var task = queue[i];
                task.timeToNext -= dt;
                while (task.timeToNext <= 0f)
                {
                    try
                    {
                        task.action?.Invoke();
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
