namespace ChatGptBot.Services.PlaceHolders;

public abstract class PlaceHolderBase
{
    public string Category { get; init; } = "";

    public static string HTmlLinkCategory { get; } = "HTmlLink";
}