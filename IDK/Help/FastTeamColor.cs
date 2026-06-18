using Landfall.TABS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

namespace IDK.Help
{
    public class FastTeamColor
    {
        public static void SetTeamColors(GameObject go, Team team)
        {
            TeamColor[] componentsInChildren = go.GetComponentsInChildren<TeamColor>();
            for (int i = 0; i < componentsInChildren.Length; i++)
            {
                componentsInChildren[i].SetTeamColor(team);
            }
        }
    }
}
