﻿using System;
using Assets.Scripts.Models.Towers;
using Assets.Scripts.Simulation.Input;
using Assets.Scripts.Simulation.Towers;
using Assets.Scripts.Simulation.Towers.Behaviors;
using Assets.Scripts.Unity.Bridge;
using BTD_Mod_Helper.Extensions;
using HarmonyLib;
using Il2CppSystem.Collections.Generic;
using MelonLoader;
using BuffQuery = Assets.Scripts.Simulation.Towers.Buffs.BuffQuery;

namespace IndustrialFarmer.Patches
{
    public class Discounts
    {
        public const string IndustrialFarmerDiscount = "IndustrialFarmerDiscount";
        
        
        private static TowerModel discountingTower;


        [HarmonyPatch(typeof(UnityToSimulation), nameof(UnityToSimulation.GetUpgradeCost))]
        internal class UnityToSimulation_GetUpgradeCost
        {
            [HarmonyPrefix]
            internal static void Prefix(UnityToSimulation __instance, int id)
            {
                var towerFromId = __instance.GetTowerFromId(id);
                if (towerFromId != null)
                {
                    discountingTower = towerFromId.towerModel;
                }
            }
        }



        
        [HarmonyPatch(typeof(TowerInventory), nameof(TowerInventory.GetTowerDiscount))]
        internal class TowerInventory_GetTowerDiscount
        {
            [HarmonyPrefix]
            internal static void Prefix(TowerModel def)
            {
                discountingTower = def;
            }
        }


        [HarmonyPatch(typeof(TowerManager), nameof(TowerManager.GetZoneDiscount))]
        internal class TowerManager_GetZoneDiscount
        {
            [HarmonyPostfix]
            internal static void Postfix(TowerManager __instance, ref Dictionary<string, List<DiscountZone>> __result)
            {
                if (__result != null && discountingTower != null &&
                    discountingTower.baseId != TowerType.BananaFarm)
                {
                    if (__result.ContainsKey(IndustrialFarmerDiscount))
                    {
                        __result[IndustrialFarmerDiscount].Clear();
                    }
                }
                
                discountingTower = null;
            }
        }
        
        
        [HarmonyPatch(typeof(Tower), nameof(Tower.AddDiscountZoneBuffs))]
        internal class Tower_AddDiscountZoneBuffs
        {
            [HarmonyPostfix]
            internal static void Postfix(Tower __instance, ref List<BuffQuery> __result)
            {
                if (__result != null && __instance.towerModel.baseId != TowerType.BananaFarm)
                {
                    foreach (var buffQuery in __result)
                    {
                        if (buffQuery != null && buffQuery.buffIndicator.name.Contains(nameof(IndustrialFarmer)))
                        {
                            buffQuery.canCurrentlyBuff = false;
                        }   
                    }
                }
            }
        }
    }
}