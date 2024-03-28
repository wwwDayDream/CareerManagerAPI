using System;
using DV.ServicePenalty.UI;
using HarmonyLib;
using TMPro;
using UnityEngine;

namespace CareerManagerAPI.Patches;

[HarmonyPatch(typeof(CareerManagerMainScreen), nameof(CareerManagerMainScreen.Awake))]
public static class CareerManagerMainScreenAwake
{
    public static void Postfix(CareerManagerMainScreen __instance)
    {
        __instance.gameObject.AddComponent<CareerManagerScreenTracker>().MainScreen = __instance;
    }
}
[HarmonyPatch(typeof(CareerManagerMainScreen), nameof(CareerManagerMainScreen.Activate))]
public static class CareerManagerMainScreenActivate
{
    public static void Postfix(CareerManagerMainScreen __instance)
    {
        __instance.gameObject.GetComponent<CareerManagerScreenTracker>()?.ShowAllTexts();
    }
}
[HarmonyPatch(typeof(CareerManagerMainScreen), nameof(CareerManagerMainScreen.Disable))]
public static class CareerManagerMainScreenDisable
{
    public static void Postfix(CareerManagerMainScreen __instance)
    {
        __instance.gameObject.GetComponent<CareerManagerScreenTracker>()?.HideAllTexts();
    }
}
[HarmonyPatch(typeof(CareerManagerMainScreen), nameof(CareerManagerMainScreen.HandleInputAction))]
public static class CareerManagerMainScreenHandleInputAction
{
    public static bool Prefix(CareerManagerMainScreen __instance, InputAction input)
    {
        __instance.gameObject.GetComponent<CareerManagerScreenTracker>()?.InputAction(input);
        return false;
    }
}