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
        public static Texture2D Icon = ContentFinder<Texture2D>.Get("hardhat");
      
        static PlanOnTheFly() 
        {
            var harmony = new Harmony("SOV.PlanOnTheFly");
            harmony.PatchAll();       
        }
        
    }
    [DefOf]
    public static class Hotkeys
    {
        public static KeyBindingDef PlanToggleKey;

        static Hotkeys()
        {         
            DefOfHelper.EnsureInitializedInCtor(typeof(Hotkeys));
        }
    }
    [HarmonyPatch(typeof(RimWorld.GenConstruct), nameof(RimWorld.GenConstruct.PlaceBlueprintForBuild))]
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

            if (row == null || PlanOnTheFly.Icon == null)
                return;   
            row.ToggleableIcon(ref PlanOnTheFlyToggle.Instance.PlanningMode, PlanOnTheFly.Icon, "Click to toggle 'Planning'");


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
        private PlanOnTheFlyToggle(){
            PlanningMode = false;
        }
        public bool PlanningMode;
    }

    [HarmonyPatch(typeof(UIRoot), nameof(UIRoot.UIRootOnGUI))]

    internal static class UIRoot_OnGUI_onKeyPress
    {
        static void Postfix()
        {
            if (Current.ProgramState != ProgramState.Playing || Event.current.type != EventType.KeyDown || Event.current.keyCode == KeyCode.None) return;  
            if (Hotkeys.PlanToggleKey.JustPressed)
            {
                PlanOnTheFlyToggle.Instance.PlanningMode = !PlanOnTheFlyToggle.Instance.PlanningMode;
                Event.current.Use();
            }
        }
    }
}
