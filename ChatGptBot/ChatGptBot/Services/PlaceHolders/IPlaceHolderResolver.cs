﻿namespace ChatGptBot.Services.PlaceHolders;

public interface IPlaceHolderResolver<T>
{
    string ResolvePlaceHolder(T placeHolder);
}