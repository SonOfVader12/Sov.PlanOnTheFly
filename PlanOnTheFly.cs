using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;
using HarmonyLib;
using UnityEngine;

namespace Sov.PlanOnTheFly
{
    [StaticConstructorOnStartup]
    public static class InitPlanOnTheFly
    {
        public static Texture2D Icon = ContentFinder<Texture2D>.Get("hardhat");
        static InitPlanOnTheFly() 
        {
            var harmony = new Harmony("SOV.PlanOnTheFly");
            harmony.PatchAll();
        }

    }
    [HarmonyPatch(typeof(RimWorld.GenConstruct), nameof(RimWorld.GenConstruct.PlaceBlueprintForBuild_NewTemp))]
    public static class BluePrint_Patch
    {
        static void Postfix(ref Blueprint_Build __result)
        {
            if (PlanOnTheFlyToggle.Instance.PlanningMode)
            {
                __result.SetForbidden(true);
            }
        }
    }

    [HarmonyPatch(typeof(RimWorld.PlaySettings), nameof(RimWorld.PlaySettings.DoPlaySettingsGlobalControls))]
    public static class ToggleIcon_Patch
    {
        static void Postfix(WidgetRow row, bool worldView)
        {
            
            if (worldView)
                return;

            if (row == null || InitPlanOnTheFly.Icon == null)
                return;   
            row.ToggleableIcon(ref PlanOnTheFlyToggle.Instance.PlanningMode, InitPlanOnTheFly.Icon, "Click to toggle 'Planning'");


        }
    }

    public sealed class PlanOnTheFlyToggle
    {
       

        private static readonly Lazy<PlanOnTheFlyToggle> lazy = new Lazy<PlanOnTheFlyToggle>(() => new PlanOnTheFlyToggle());
        public static PlanOnTheFlyToggle Instance { get
            {
               return lazy.Value;
            } 
        }
        private PlanOnTheFlyToggle(){ }
        public bool PlanningMode;
    }
}
