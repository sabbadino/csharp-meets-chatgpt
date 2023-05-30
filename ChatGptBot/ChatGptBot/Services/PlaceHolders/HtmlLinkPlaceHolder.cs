namespace ChatGptBot.Services.PlaceHolders;

public enum LinkCategory
{
    None, Booking, ShippingInstruction,Quote
}

    public class HtmlLinkPlaceHolder : PlaceHolderBase
{
    public LinkCategory LinkCategory { get; init; } 
    public string Value { get; init; } = "";
}