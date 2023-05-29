﻿namespace myMscChatGpt.Services;

public class Embedding
{

    public Guid Id { get; init; } = Guid.NewGuid();

    public int Tokens { get; init; } = 0;

    public string Text { get; init; } = "";

    public List<float> VectorValues { get; init; } = new();
}