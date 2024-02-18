using AutoEvent.Interfaces;

namespace AutoEvent.Games.Example;
public class ExampleTranslation : EventTranslation
{
    public override string Name { get; set; } = "Example";
    public override string Description { get; set; } = "Example message mini-game";
    public override string CommandName { get; set; } = "example";
    public string AnyText { get; set; } = "<size=100><color=red>Any text</color></size>";
}