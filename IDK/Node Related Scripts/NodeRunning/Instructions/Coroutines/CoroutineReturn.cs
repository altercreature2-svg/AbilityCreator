using AC.NodeScripts;
using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AC.Node_Related_Scripts.NodeRunning.Instructions.Courtines
{
    public struct CoroutineReturn
    {
        public enum CourtineType
        {
            Stop,
            WaitForSeconds,
            WaitForSecondsRealtime,
            PauseFrame,
            ContinueBranch,
            WaitUntil,
        }
        public CoroutineReturn(CourtineType courtineType, object arg0 = default, Func<bool> arg1 = default)
        {
            this.type = courtineType;
            this.arg0 = arg0;
            this.arg1 = arg1;
        }
        public CourtineType type;
        public object arg0;
        public Func<bool> arg1;
    }
}
