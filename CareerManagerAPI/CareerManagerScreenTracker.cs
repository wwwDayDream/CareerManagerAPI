using System;
using System.Collections.Generic;
using System.Linq;
using DV.ServicePenalty.UI;
using JetBrains.Annotations;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CareerManagerAPI;

public class CareerManagerScreenTracker : MonoBehaviour
{
    public CareerManagerMainScreen? MainScreen { get; set; } = null;
    private List<CareerManagerMainScreenEntry> MainScreenEntries { get; set; } = [];
    private string StationLocation => MainScreen?.transform.parent?.parent?.parent?.name ?? "";

    public void Start()
    {
        if (MainScreen == null || !MainScreen)
            throw new ArgumentException("MainScreen wasn't supplied!", nameof(MainScreen));

        MainScreen.selectableText = [];

        
        MainScreenEntries.Add(new CareerManagerMainScreenEntry(
            CareerManagerLocalization.FEES, MainScreen.feesScreen, CareerManagerAPI.AlwaysTrue));
        MainScreenEntries.Add(new CareerManagerMainScreenEntry(
            CareerManagerLocalization.LICENSES, MainScreen.licensesScreen, CareerManagerAPI.CareerModeOnly));
        MainScreenEntries.Add(new CareerManagerMainScreenEntry(
            CareerManagerLocalization.OWNED_VEHICLES, MainScreen.ownedVehiclesScreen, CareerManagerAPI.CareerModeOnly));
        MainScreenEntries.Add(new CareerManagerMainScreenEntry(
            CareerManagerLocalization.STATS, MainScreen.statsScreen, CareerManagerAPI.AlwaysTrue));
        
        CareerManagerAPI.OnStationCareerManagerAwake(this, StationLocation);
        
        Plugin.Logger?.Log($"Initialized Career Manager @ {StationLocation}");

        MainScreenUpdated();
    }
    [UsedImplicitly]
    public bool TryAddToMainScreen<TDisplayScreen>(string text, CareerManagerAPI.OnCanConfirm? canConfirm = null,
        string? after = null, string? before = null) where TDisplayScreen : IDisplayScreen
    {
        if (MainScreenEntries.Exists(entry => entry.Text == text)) return false;

        var newGo = new GameObject(text, typeof(TDisplayScreen))
        {
            transform =
            {
                parent = MainScreen!.feesScreen.transform.parent
            }
        };

        MainScreenEntries.Add(new CareerManagerMainScreenEntry(text, (IDisplayScreen)newGo.GetComponent(typeof(TDisplayScreen)), canConfirm ?? CareerManagerAPI.AlwaysTrue, after, before));
        return true;
    }

    internal void ShowAllTexts()
    {
        MainScreen!.fees.text = MainScreen.licenses.text = MainScreen.ownedVehicles.text = MainScreen.stats.text = string.Empty;
        MainScreenUpdated();
    }

    internal void HideAllTexts()
    {
        MainScreenUpdated(true);
    }

    internal void InputAction(InputAction input)
    {
        var currentSelected = MainScreen!.selector.current;
        switch (input)
        {
            case DV.ServicePenalty.UI.InputAction.Up:
                
                MainScreen!.HighlightSelected(MainScreen!.selector.Previous(), currentSelected);
                break;
            case DV.ServicePenalty.UI.InputAction.Down:
                MainScreen!.HighlightSelected(MainScreen!.selector.Next(), currentSelected);
                break;
            case DV.ServicePenalty.UI.InputAction.Cancel:
                break;
            case DV.ServicePenalty.UI.InputAction.Confirm:
                if (MainScreen!.selector.current < MainScreenEntries.Count)
                {
                    var entry = MainScreenEntries[MainScreen!.selector.current];
                    if (entry.CanEnter(out var err))
                        MainScreen!.screenSwitcher.SetActiveDisplay(entry.Screen);
                    else
                    {
                        MainScreen!.infoScreen.SetInfoData(MainScreen!, entry.Text, err);
                        MainScreen!.screenSwitcher.SetActiveDisplay(MainScreen!.infoScreen);
                    }
                }
                else
                    Debug.LogError(string.Format("Unhandled case in {0}: {1}", "CareerManagerMainScreen", MainScreen!.selector.Current));
                break;
            default:
                break;
        }
    }
    private void MainScreenUpdated(bool hide = false)
    {
        foreach (var textMeshPro in MainScreen!.selectableText)
            if (textMeshPro != MainScreen.fees && textMeshPro != MainScreen.licenses && textMeshPro != MainScreen.ownedVehicles && textMeshPro != MainScreen.stats)
                Destroy(textMeshPro);

        List<CareerManagerMainScreenEntry> toAdd = [];
        for (var idx = 0; idx < MainScreenEntries.Count; idx++)
        {
            var entry = MainScreenEntries[idx];
            var beforeIdx = -1;
            var afterIdx = -1;
            if (entry.DisplayBefore != null && (beforeIdx = toAdd.FindIndex(en => en.Text == entry.DisplayBefore)) >= 0)
                toAdd.Insert(beforeIdx, entry);
            else if (entry.DisplayAfter != null && (afterIdx = toAdd.FindIndex(en => en.Text == entry.DisplayAfter)) >= 0)
                toAdd.Insert(afterIdx, entry);
            else if (entry.DisplayBefore != null || entry.DisplayAfter != null)
            {
                MainScreenEntries.RemoveAt(idx);
                MainScreenEntries.Add(entry);
                idx--;
            } else
                toAdd.Add(entry);
        }

        MainScreenEntries = toAdd;

        var yDiff = MainScreen.fees.transform.localPosition.y - MainScreen.licenses.transform.localPosition.y;
        var index = 0;
        MainScreen!.selectableText = toAdd.Select(entry =>
        {
            var obj = Object.Instantiate(MainScreen!.fees, MainScreen!.fees.transform.parent, true)!;
            obj.renderer.enabled = true;
            obj.color = MainScreen.CurrentSelection == index ? MainScreen.screenSwitcher.HIGHLIGHTED_COLOR : MainScreen.screenSwitcher.REGULAR_COLOR;
            obj.text = hide ? string.Empty : entry.Text;
            obj.transform.localPosition = new Vector3(obj.transform.localPosition.x, MainScreen.fees.transform.localPosition.y - index * yDiff,
                obj.transform.localPosition.z);
            index++;
            return obj;
        }).ToArray();
        
        MainScreen!.selector.Length = MainScreen!.selectableText.Length;
    }
}