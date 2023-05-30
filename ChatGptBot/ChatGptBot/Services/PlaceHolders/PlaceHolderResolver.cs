using System.Text.Json;
using System.Text.RegularExpressions;
using ChatGptBot.Ioc;

namespace ChatGptBot.Services.PlaceHolders;

public interface IPlaceHolderResolverService
{
    string ReplacePlaceHolders(string answerFromAi);
}

public class PlaceHolderResolverService : IPlaceHolderResolverService, ISingletonScope
{
    private readonly IPlaceHolderResolver<HtmlLinkPlaceHolder> _htmlLinkPlaceHolderResolver;

    public PlaceHolderResolverService(IPlaceHolderResolver<HtmlLinkPlaceHolder> htmlLinkPlaceHolderResolver)
    {
        _htmlLinkPlaceHolderResolver = htmlLinkPlaceHolderResolver;
    }

    public static readonly string PlaceHolderMarker = "#placeholder#";
    private readonly Regex _regex = new Regex($"(?s)(?<={PlaceHolderMarker}).*?(?={PlaceHolderMarker})", RegexOptions.Compiled);

    public string ReplacePlaceHolders(string answerFromAi)
    {
        if (string.IsNullOrEmpty(answerFromAi))
        {
            return "";

        }

        var ret = answerFromAi;
        while (true)
        {
            var match = _regex.Match(ret);
            if (!match.Success)
            {
                break;
            }

            var doc = JsonDocument.Parse(match.Value);
            var category = doc.RootElement.GetProperty("Category").GetString();
            if (category != null)
            {
                if (category == PlaceHolderBase.HTmlLinkCategory)
                {
                    var htmlLinkPlaceHolder = JsonSerializer.Deserialize<HtmlLinkPlaceHolder>(match.Value);
                    if (htmlLinkPlaceHolder != null)
                    {
                        ret = ReplaceText(ret, match.Value,
                            _htmlLinkPlaceHolderResolver.ResolvePlaceHolder(htmlLinkPlaceHolder));
                    }
                    else
                    {
                        ret = ReplaceText(ret, match.Value, "");
                    }
                }
                else
                {
                    ret = ReplaceText(ret, match.Value, "");
                }
            }
        }


        return ret;



    }

    private string ReplaceText(string text, string originalValue, string newValue)
    {
        return text.Replace(PlaceHolderMarker + originalValue + PlaceHolderMarker, newValue);
    }
}