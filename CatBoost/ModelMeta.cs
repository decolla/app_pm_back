using System.Collections.Generic;

namespace teste_catboost_cs;

public class ModelMeta
{
    public required List<string> cat_features { get; init; }
    public required List<string> text_features { get; init; }
    public required List<string> features { get; init; }
}
