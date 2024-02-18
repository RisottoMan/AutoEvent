namespace AutoEvent.Interfaces;

public interface IEventTranslation
{
    string Name { get; set; }
    string Description { get; set; }
    string CommandName { get; set; }
}