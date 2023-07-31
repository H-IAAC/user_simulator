using System;

[Serializable]
public class StringBlackboardProperty : BlackboardProperty
{
    public string value;

    public override string PropertyTypeName { get { return "string"; } }
}