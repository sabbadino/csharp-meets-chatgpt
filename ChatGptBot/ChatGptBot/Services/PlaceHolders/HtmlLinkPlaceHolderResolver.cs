using ChatGptBot.Ioc;
using ChatGptBot.Settings;
using Microsoft.Extensions.Options;

namespace ChatGptBot.Services.PlaceHolders;

public class HtmlLinkPlaceHolderResolver : IPlaceHolderResolver<HtmlLinkPlaceHolder>, ISingletonScope
{
    private readonly PlaceholdersInformation _placeholdersInformation;

    public HtmlLinkPlaceHolderResolver(IOptions<PlaceholdersInformation> placeholdersInformation)
    {
        _placeholdersInformation = placeholdersInformation.Value;
    }

    //
    public string ResolvePlaceHolder(HtmlLinkPlaceHolder placeHolder)
    {
        var ret = placeHolder switch {
            { LinkCategory : LinkCategory.Booking } => new Uri($"{_placeholdersInformation.LinkRoot}/booking/{placeHolder.Value}").ToString(),
            { LinkCategory: LinkCategory.ShippingInstruction } => new Uri($"{_placeholdersInformation.LinkRoot}/ShippingInstructions/{placeHolder.Value}").ToString(),
            { LinkCategory: LinkCategory.Quote} => new Uri($"{_placeholdersInformation.LinkRoot}/quote/{placeHolder.Value}").ToString(),
            _ => throw new ArgumentException($"Unknown LinkCategory {placeHolder.LinkCategory}"),
        };
        return ret;
    }
}