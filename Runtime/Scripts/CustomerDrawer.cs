using UnityEditor;
using UnityEngine;
using static Level;

[CustomPropertyDrawer(typeof(Customer))]
public class CustomerDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // 1. Find the properties inside the struct
        SerializedProperty nameProp = property.FindPropertyRelative("name");
        SerializedProperty typeProp = property.FindPropertyRelative("customerType");

        // 2. Determine the color based on the enum value
        Color labelColor = Color.white;
        if (typeProp.enumValueIndex == (int)CustomerType.Line)
        {
            labelColor = Color.yellow;
        }
        else if (typeProp.enumValueIndex == (int)CustomerType.Table)
        {
            labelColor = Color.green;
        }

        // 3. Create a custom style for the label
        GUIStyle labelStyle = new GUIStyle(EditorStyles.foldout);
        labelStyle.normal.textColor = labelColor;
        labelStyle.fontStyle = FontStyle.Bold;

        // 4. Create the display text (Name + Type)
        string displayName = string.IsNullOrEmpty(nameProp.stringValue) ? "Customer" : nameProp.stringValue;
        string headerText = $"{displayName} ({((CustomerType)typeProp.enumValueIndex)})";

        // 5. Draw the foldout with the custom color
        property.isExpanded = EditorGUI.Foldout(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight),
            property.isExpanded, new GUIContent(headerText), true, labelStyle);

        // 6. If the foldout is open, draw the children fields
        if (property.isExpanded)
        {
            EditorGUI.indentLevel++;
            float lineHeight = EditorGUIUtility.singleLineHeight + 2;
            Rect fieldRect = new Rect(position.x, position.y + lineHeight, position.width, EditorGUIUtility.singleLineHeight);

            EditorGUI.PropertyField(fieldRect, typeProp);
            fieldRect.y += lineHeight;
            EditorGUI.PropertyField(fieldRect, property.FindPropertyRelative("spawnTime"));
            EditorGUI.indentLevel--;
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        // Calculate height: If closed, 1 line. If open, 4 lines (header + 3 fields)
        if (property.isExpanded)
            return (EditorGUIUtility.singleLineHeight + 2) * 4;

        return EditorGUIUtility.singleLineHeight;
    }
}