using System.Collections.Generic;
using UnityEngine;

//Utility
//Ordena filhos pela utilidade (memoried: apenas no start, memoryless: always)
//
//Abordagens
//Executa 1º filho com maior utilidade
//Seleciona aleatoriamente ponderado pela utilidade
//Seleciona aleatoriamente acima de certa utilidade
//

public enum UtilityPropagationMethod
{
    BIGGEST,
    SMALLEST,
    ALL_SUCESS_PROBABILITY,
    AT_LEAST_ONE_SUCESS_PROBABILITY
}

public enum UtilitySelectionMethod
{
    BIGGEST,
    WEIGHT_RANDOM,
    RANDOM_THRESHOULD
}

public abstract class CompositeNode : Node
{
    [HideInInspector] 
    public List<Node> children = new();

    [SerializeField] UtilityPropagationMethod utilityPropagationMethod = UtilityPropagationMethod.BIGGEST;
    [SerializeField] UtilitySelectionMethod utilitySelectionMethod = UtilitySelectionMethod.BIGGEST;
    [SerializeField] float utilityThreshould = 0f;

    public CompositeNode(MemoryMode memoryMode = MemoryMode.Memoryless) : base(memoryMode)
    {

    }

    public override Node Clone()
    {
        CompositeNode node = Instantiate(this);

        node.children = new List<Node>();

        foreach(Node child in children)
        {
            node.children.Add(child.Clone());
        }

        return node;
    }

    public override void AddChild(Node child)
    {
        children.Add(child);
        child.parent = this;
    }

    public override void RemoveChild(Node child)
    {
        children.Remove(child);
    }

    public override List<Node> GetChildren()
    {
        return children;
    }

    protected void SortChildrenByUtility()
    {
        children.Sort(SortByUtility);
    }

    private int SortByUtility(Node left, Node right)
    {
        if(left.GetUtility() < right.GetUtility())
        {
            return 1;
        }
        return -1;
    }

    public override float GetUtility()
    {
        switch (utilityPropagationMethod)
        {
            case UtilityPropagationMethod.BIGGEST:
                {
                    float biggest = 0;
                    foreach (Node child in children)
                    {
                        float u = child.GetUtility();
                        if (u > biggest)
                        {
                            biggest = u;
                        }
                    }

                    return biggest;
                    break;
                }
            case UtilityPropagationMethod.SMALLEST:
                {
                    float smallest = 1;
                    foreach (Node child in children)
                    {
                        float u = child.GetUtility();
                        if (u < smallest)
                        {
                            smallest = u;
                        }
                    }

                    return smallest;
                    break;
                }

            case UtilityPropagationMethod.ALL_SUCESS_PROBABILITY:
                {
                    float p = 1;
                    foreach (Node child in children)
                    {
                        p *= child.GetUtility();
                    }

                    return p;
                    break;
                }

            case UtilityPropagationMethod.AT_LEAST_ONE_SUCESS_PROBABILITY:
                {
                    if (children.Count == 0)
                    {
                        return 0;
                    }

                    float p = 1;
                    foreach (Node child in children)
                    {
                        p *= 1 - child.GetUtility();
                    }

                    return 1 - p;
                    break;
                }

            default:
                return 0;
                break;
        }
    }

    protected Node NextNode(List<Node> exclude = null)
    {
        //Nodes are already sorted by utility

        if(exclude == null)
        {
            exclude = new();
        }

        
        switch (utilitySelectionMethod)
        {
            case UtilitySelectionMethod.BIGGEST:
                foreach(Node node in children)
                {
                    if(!exclude.Contains(node))
                    {
                        return node;
                    }
                }

                break;

            case UtilitySelectionMethod.WEIGHT_RANDOM:
                {
                    float weightTotal = 0;
                    List<Node> nodes = new();
                    foreach (Node node in children)
                    {
                        if (!exclude.Contains(node) && node.GetUtility() > utilityThreshould)
                        {
                            nodes.Add(node);
                            weightTotal += node.GetUtility();
                        }
                    }

                    if (nodes.Count > 0)
                    {
                        int result;
                        float total = 0;
                        float randVal = Random.Range(0, weightTotal);
                        for (result = 0; result < nodes.Count; result++)
                        {
                            total += nodes[result].GetUtility();
                            if (total > randVal) break;
                        }
                        return nodes[result];
                    }

                    break;
                }
            case UtilitySelectionMethod.RANDOM_THRESHOULD:
                {
                    List<Node> nodes = new();
                    foreach (Node node in children)
                    {
                        if (!exclude.Contains(node) && node.GetUtility() > utilityThreshould)
                        {
                            nodes.Add(node);
                        }
                    }

                    if (nodes.Count > 0)
                    {
                        int index = Random.Range(0, nodes.Count);
                        return nodes[index];
                    }

                    break;
                }
        }

        return null;
    }
}