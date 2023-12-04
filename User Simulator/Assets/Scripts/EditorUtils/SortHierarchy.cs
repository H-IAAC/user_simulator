//https://gist.github.com/AShim3D/d76e2026c5655b3b34e2
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

public class SortHierarchy
{
	[MenuItem("Tools/SortChildrenByName")]

	public static void SortChildrenByName()
	{
		foreach (Transform t in Selection.transforms)
		{
			List<Transform> children = t.Cast<Transform>().ToList();
			children.Sort((Transform t1, Transform t2) => { return t1.name.CompareTo(t2.name); });
			for (int i = 0; i < children.Count; ++i)
			{
				Undo.SetTransformParent(children[i], children[i].parent, "Sort Children");
				children[i].SetSiblingIndex(i);
			}
		}
	} // SortChildrenByName()

} // class TempTools