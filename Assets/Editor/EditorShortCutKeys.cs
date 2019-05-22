using UnityEngine;
using UnityEditor;

public class EditorShortCutKeys : ScriptableObject
{
	[MenuItem ("GameObject/Deselect All %#a")]
	static void DeselectAll()
	{
		Selection.objects = new UnityEngine.Object[0];
	}
}