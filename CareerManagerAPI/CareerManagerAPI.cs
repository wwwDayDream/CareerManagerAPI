using DV.ServicePenalty.UI;
using DV.UserManagement;
using DV.Utils;
using UnityEngine;

namespace CareerManagerAPI;

public static class CareerManagerAPI
{
    public static event CareerManagerMainScreenInitialized StationCareerManagerAwake;
    public delegate void CareerManagerMainScreenInitialized(CareerManagerScreenTracker screen, string stationName);

    public delegate bool OnCanConfirm(out string displayError);
    public static bool AlwaysTrue(out string err) => (err = string.Empty) == string.Empty;
    public static bool CareerModeOnly(out string err)
    {
        if (SingletonBehaviour<UserManager>.Instance.CurrentUser.CurrentSession.GameMode == "Career")
            return (err = string.Empty) == string.Empty;

        return (err = CareerManagerLocalization.UNAVAILABLE_IN_SANDBOX) == string.Empty;
    }

    public static void OnStationCareerManagerAwake(CareerManagerScreenTracker screen, string stationname)
    {
        StationCareerManagerAwake?.Invoke(screen, stationname);
    }
}