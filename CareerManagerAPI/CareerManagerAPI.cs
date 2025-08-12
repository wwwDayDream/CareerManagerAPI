using System;
using DV.ServicePenalty.UI;
using DV.UserManagement;
using DV.Utils;
using UnityEngine;

namespace CareerManagerAPI;

public static class CareerManagerAPI
{
    public static event CareerManagerInitialized CareerManagerAwake;
    public delegate void CareerManagerInitialized(CareerManagerScreenTracker tracker, string locationName, StationController? station, TrainCar? trainCar);
    
    [Obsolete($"{nameof(StationCareerManagerAwake)} is deprecated, please use {nameof(CareerManagerAwake)} instead.")]
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

    public static void OnStationCareerManagerAwake(CareerManagerScreenTracker tracker, StationController? station, TrainCar? trainCar, string locationName)
    {
        StationCareerManagerAwake?.Invoke(tracker, locationName);
        CareerManagerAwake?.Invoke(tracker, locationName, station, trainCar);
    }
}