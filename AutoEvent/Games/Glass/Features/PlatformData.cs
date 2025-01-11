namespace AutoEvent.Games.Glass.Features;
public struct PlatformData
{
    public PlatformData(bool leftSideIsDangerous, int placement)
    {
        LeftSideIsDangerous = leftSideIsDangerous;
        Placement = placement;
    }
    public int Placement { get; set; }
    public bool LeftSideIsDangerous { get; set; }
}