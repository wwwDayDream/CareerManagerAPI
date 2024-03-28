using DV.ServicePenalty.UI;

namespace CareerManagerAPI;

public struct CareerManagerMainScreenEntry(string text, IDisplayScreen screen, CareerManagerAPI.OnCanConfirm canEnter, 
    string? after = null, string? before = null)
{
    public readonly string Text = text;
    public readonly CareerManagerAPI.OnCanConfirm CanEnter = canEnter;
    public readonly IDisplayScreen Screen = screen;
    public readonly string? DisplayAfter = after;
    public readonly string? DisplayBefore = before;
}