using System.Reflection.Metadata.Ecma335;
using myMscChatGpt.Services.PlaceHolders;
using static myMscChatGpt.Services.PlaceHolders.HtmlLinkPlaceHolderResolver;

namespace myMscChatGpt.Services.PlaceHolders;

public enum LinkCategory
{
    None, Booking, ShippingInstruction,Quote
}

    public class HtmlLinkPlaceHolder : PlaceHolderBase
{
    public LinkCategory LinkCategory { get; init; } 
    public string Value { get; init; } = "";
}