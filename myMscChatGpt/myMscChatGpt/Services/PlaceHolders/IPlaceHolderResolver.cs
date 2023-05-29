namespace myMscChatGpt.Services.PlaceHolders;

public interface IPlaceHolderResolver<T>
{
    string ResolvePlaceHolder(T placeHolder);
}