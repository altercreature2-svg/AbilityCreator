using AC.Node_Related_Scripts.NodeRunning.Instructions.Courtines;
using AC.NodeScripts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AC.Node_Related_Scripts.NodeRunning.Instructions
{
    public struct NodeInstruction
    {
        public NodeInstruction(VirtualNode node, INode nodeFunction)
        {
            this.node = node;
            this.nodeFunction = nodeFunction;
        }
        public INode nodeFunction;
        public VirtualNode node;
        public IEnumerator Execute(NodeEnv nodeEnv, NodeProgram.NodeCore nodeInstructions, Action continueBranchAction = null)
        {
            IEnumerator<CoroutineReturn> courtine = nodeFunction.Execute(nodeEnv);
            if (courtine == null)
            {
                yield break; // it already shat
            }
            bool procced = true;
            while (procced)
            {
                switch (courtine.Current.type)
                {
                    case (CoroutineReturn.CourtineType.WaitForSecondsRealtime):
                        yield return new WaitForSecondsRealtime((float)courtine.Current.arg0);
                        break;
                    case (CoroutineReturn.CourtineType.WaitForSeconds):
                        yield return new WaitForSeconds((float)courtine.Current.arg0);
                        break;
                    case (CoroutineReturn.CourtineType.WaitUntil):
                        yield return new WaitUntil(courtine.Current.arg1);
                        break;
                    case (CoroutineReturn.CourtineType.PauseFrame):
                        yield return null;
                        break;
                    case (CoroutineReturn.CourtineType.ContinueBranch):
                        if (continueBranchAction == null)
                            nodeInstructions.Next();
                        else
                            continueBranchAction();

                        yield break;
                    default:
                        break;
                }
                procced = courtine.MoveNext();
            }
        }
    }
}
