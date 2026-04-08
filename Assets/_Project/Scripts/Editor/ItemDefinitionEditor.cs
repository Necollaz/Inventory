using UnityEditor;
using UnityEngine;
using _Project.Data;

[CustomEditor(typeof(ItemDefinition))]
    public sealed class ItemDefinitionEditor : Editor
    {
        private const float PREVIEW_MAX_SIZE_PIXELS = 96f;
        
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            SerializedProperty idProperty = serializedObject.FindProperty("_id");
            SerializedProperty typeProperty = serializedObject.FindProperty("_type");
            SerializedProperty weightProperty = serializedObject.FindProperty("_weight");
            SerializedProperty maxStackProperty = serializedObject.FindProperty("_maxStack");
            SerializedProperty inventorySlotSpriteProperty = serializedObject.FindProperty("_inventorySlotSprite");
            SerializedProperty protectionProperty = serializedObject.FindProperty("_protection");
            SerializedProperty ammoIdProperty = serializedObject.FindProperty("_ammoId");
            SerializedProperty damageProperty = serializedObject.FindProperty("_damage");
            
            DrawInventorySlotSpritePreview(inventorySlotSpriteProperty);
            
            EditorGUILayout.PropertyField(inventorySlotSpriteProperty);
            EditorGUILayout.Space(4f);
            EditorGUILayout.PropertyField(idProperty);
            EditorGUILayout.PropertyField(typeProperty);
            EditorGUILayout.PropertyField(weightProperty);
            EditorGUILayout.PropertyField(maxStackProperty);
            
            ItemType itemType = (ItemType)typeProperty.enumValueIndex;
            
            EditorGUILayout.Space(4f);
            
            switch (itemType)
            {
                case ItemType.Head:
                case ItemType.Torso:
                    EditorGUILayout.LabelField("Armor", EditorStyles.boldLabel);
                    EditorGUILayout.PropertyField(protectionProperty);
                    break;
                
                case ItemType.Weapon:
                    EditorGUILayout.LabelField("Weapon", EditorStyles.boldLabel);
                    EditorGUILayout.PropertyField(ammoIdProperty);
                    EditorGUILayout.PropertyField(damageProperty);
                    break;
                
                case ItemType.Ammo:
                    break;
                
                default:
                    EditorGUILayout.HelpBox($"Неизвестный ItemType: {itemType}", MessageType.Warning);
                    break;
            }
            
            serializedObject.ApplyModifiedProperties();
        }
        
        private void DrawInventorySlotSpritePreview(SerializedProperty spriteProperty)
        {
            Sprite sprite = spriteProperty.objectReferenceValue as Sprite;
            
            if (sprite == null)
                return;
            
            Texture previewTexture = AssetPreview.GetAssetPreview(sprite);
            
            if (previewTexture == null)
                previewTexture = sprite.texture;
            
            if (previewTexture == null)
                return;
            
            float width = Mathf.Min(PREVIEW_MAX_SIZE_PIXELS, previewTexture.width);
            float height = Mathf.Min(PREVIEW_MAX_SIZE_PIXELS, previewTexture.height);
            float aspect = previewTexture.width > 0 ? (float)previewTexture.height / previewTexture.width : 1f;
            height = width * aspect;
            
            Rect rect = GUILayoutUtility.GetRect(width, height, GUILayout.ExpandWidth(false));
            GUI.DrawTexture(rect, previewTexture, ScaleMode.ScaleToFit);
        }
    }