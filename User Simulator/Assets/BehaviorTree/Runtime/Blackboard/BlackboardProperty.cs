using System;

[Serializable]
public abstract class BlackboardProperty
{
    public string PropertyName;

    public abstract string PropertyTypeName{ get; }
}