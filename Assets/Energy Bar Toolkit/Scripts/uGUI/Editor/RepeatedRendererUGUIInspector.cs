using System;
using UnityEditor.AnimatedValues;
using UnityEngine;
using UnityEditor;
using System.Collections;

namespace EnergyBarToolkit {

[CustomEditor(typeof(RepeatedRendererUGUI))]
public class RepeatedRendererUGUIInspector : EnergyBarUGUIInspectorBase {

    #region Fields

    private SerializedProperty spriteIcon;
    private SerializedProperty spriteSlot;

    private SerializedProperty repeatCount;
    private SerializedProperty repeatPositionDelta;

    private SerializedProperty growType;
    private SerializedProperty growDirection;

    private SerializedProperty effectBlink;
    private SerializedProperty effectBlinkValue;
    private SerializedProperty effectBlinkRatePerSecond;
    private SerializedProperty effectBlinkColor;
    private SerializedProperty effectBlinkOperator;

    private readonly AnimBool blinkEffectAnimBool = new AnimBool();

    #endregion

    #region Methods

    public override void OnEnable() {
        base.OnEnable();

        spriteIcon = serializedObject.FindProperty("spriteIcon");
        spriteSlot = serializedObject.FindProperty("spriteSlot");

        repeatCount = serializedObject.FindProperty("repeatCount");
        repeatPositionDelta = serializedObject.FindProperty("repeatPositionDelta");

        growType = serializedObject.FindProperty("growType");
        growDirection = serializedObject.FindProperty("growDirection");

        effectBlink = serializedObject.FindProperty("effectBlink");
        effectBlinkValue = serializedObject.FindProperty("effectBlinkValue");
        effectBlinkRatePerSecond = serializedObject.FindProperty("effectBlinkRatePerSecond");
        effectBlinkColor = serializedObject.FindProperty("effectBlinkColor");
        effectBlinkOperator = serializedObject.FindProperty("effectBlinkOperator");

        blinkEffectAnimBool.value = effectBlink.boolValue;
    }

    public override void OnInspectorGUI() {
        serializedObject.UpdateIfDirtyOrScript();

        SectionTextures();
        SectionAppearance();
        SectionEffects();
        SectionDebugMode();

        serializedObject.ApplyModifiedProperties();
    }

    private void SectionTextures() {
        GUILayout.Label("Textures", "HeaderLabel");
        using (MadGUI.Indent()) {
            FieldSprite(spriteIcon, "Icon", MadGUI.ObjectIsSet);

#if !UNITY_5
            EnsureReadable(spriteIcon.FindPropertyRelative("sprite"));
#endif

            using (MadGUI.Indent()) {
                MadGUI.PropertyField(spriteIcon.FindPropertyRelative("material"), "Material");
            }

            EditorGUILayout.Space();

            FieldSprite(spriteSlot, "Slot");

            using (MadGUI.Indent()) {
                MadGUI.PropertyField(spriteSlot.FindPropertyRelative("material"), "Material");
            }
        }
    }

    private void SectionAppearance() {
        GUILayout.Label("Appearance", "HeaderLabel");
        using (MadGUI.Indent()) {
            FieldSetNativeSize();
            EditorGUILayout.Space();

            MadGUI.PropertyField(repeatCount, "Repeat Count");
            MadGUI.PropertyFieldVector2(repeatPositionDelta, "Icons Distance");

            EditorGUILayout.Space();

            MadGUI.PropertyFieldEnumPopup(growType, "Grow Type");
            using (MadGUI.EnabledIf(growType.enumValueIndex == (int) RepeatedRendererUGUI.GrowType.Fill)) {
                var index = growDirection.enumValueIndex;
                EditorGUI.BeginChangeCheck();
                index = MadGUI.DynamicPopup(index, "Fill Direction",
                    Enum.GetNames(typeof (EnergyBarBase.GrowDirection)).Length - 1,
                    i => {
                        return SplitByCamelCase(Enum.GetNames(typeof (EnergyBarBase.GrowDirection))[i]);
                    });
                if (EditorGUI.EndChangeCheck()) {
                    growDirection.enumValueIndex = index;
                }
            }

            FieldLabel2();
        }
    }

    private void SectionEffects() {
        GUILayout.Label("Effects", "HeaderLabel");
        using (MadGUI.Indent()) {
            FieldSmoothEffect();

            blinkEffectAnimBool.target = effectBlink.boolValue;
            MadGUI.PropertyField(effectBlink, "Blink");
            if (EditorGUILayout.BeginFadeGroup(blinkEffectAnimBool.faded)) {
                MadGUI.Indent(() => {
                    MadGUI.PropertyFieldEnumPopup(effectBlinkOperator, "Operator");
                    MadGUI.PropertyField(effectBlinkValue, "Value");
                    MadGUI.PropertyField(effectBlinkRatePerSecond, "Rate (per second)");
                    MadGUI.PropertyField(effectBlinkColor, "Color");
                });
                EditorGUILayout.Space();
            }
            EditorGUILayout.EndFadeGroup();
        }
        
    }

    #endregion
}

} // namespace