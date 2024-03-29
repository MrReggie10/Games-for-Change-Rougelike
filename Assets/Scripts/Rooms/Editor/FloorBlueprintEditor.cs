using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

//UNUSED

//[CustomEditor(typeof(FloorBlueprint))]
public class FloorBlueprintEditor : Editor
{
    public override VisualElement CreateInspectorGUI()
    {
        SerializedProperty floorBounds = serializedObject.FindProperty("floorBounds");

        VisualElement inspector = new VisualElement();
        PropertyField floorBoundsField = new PropertyField(floorBounds);
        inspector.Add(floorBoundsField);
        floorBoundsField.RegisterCallback<FocusOutEvent>(delegate { UpdateFloorMatrix(); });
        Debug.Log("Editor Called");
        return inspector;

        void UpdateFloorMatrix()
        {
            Debug.Log(floorBounds.vector2IntValue);
        }
    }
}