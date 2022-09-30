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
    public static class PlanOnTheFly
    {
        public static Texture2D Icon = ContentFinder<Texture2D>.Get("HeatMap");
        static PlanOnTheFly() //our constructor
        {
           
            Log.Message("Hello World!"); //Outputs "Hello World!" to the dev console.
            var harmony = new Harmony("SOV.PlanOnTheFly");
            Harmony.DEBUG = true;
            harmony.PatchAll();
            // var blah = new RimWorld.ForbidUtility().SetForbidden(t, true)
        }

    }
    [HarmonyPatch(typeof(RimWorld.GenConstruct), nameof(RimWorld.GenConstruct.PlaceBlueprintForBuild_NewTemp))]
    public static class PlanOnFlyPatch
    {
        static void Postfix(ref Blueprint_Build __result)
        {
            if (Wrapper.Instance.PlanningMode)
            {
                Log.Message($"Patched : {__result.Label}");
                __result.SetForbidden(true);
            }
        }
    }

    [HarmonyPatch(typeof(RimWorld.PlaySettings), nameof(RimWorld.PlaySettings.DoPlaySettingsGlobalControls))]
    public static class PlanOnFlyToggle
    {
        static void Postfix(WidgetRow row, bool worldView)
        {
            
            if (worldView)
                return;

            if (row == null || PlanOnTheFly.Icon == null)
                return;
            bool test = Wrapper.Instance.PlanningMode;
            row.ToggleableIcon(ref test, PlanOnTheFly.Icon, "IT WORKED");

            Wrapper.Instance.PlanningMode = test;

        }
    }

    public sealed class Wrapper
    {
        private Wrapper _instance;

        private static readonly Lazy<Wrapper> lazy = new Lazy<Wrapper>(() => new Wrapper());
        public static Wrapper Instance { get
            {
               return lazy.Value;
            } 
        }
        private Wrapper()
        {

        }
        public bool PlanningMode { get; set; }
    }
}
