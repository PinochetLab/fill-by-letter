using TMPro;
using UnityEngine;

namespace Animations
{
    public class WavyText : MonoBehaviour
    {
        public TMP_Text textComponent;
        public float waveSpeed = 2f;
        public float waveHeight = 5f;
        public float waveFrequency = 0.5f;
        
        [Header("Color Settings")]
        public float colorSpeed = 1f;
        public float colorSaturation = 1f;
        public float colorValue = 1f;

        void Update()
        {
            textComponent.ForceMeshUpdate();
            var textInfo = textComponent.textInfo;
        
            // Берем первый меш (обычно он один)
            var meshInfo = textInfo.meshInfo[0];
            var vertices = meshInfo.vertices;
            var colors32 = meshInfo.colors32; // ← Правильное имя: colors32
            
            // Проходим по всем видимым символам
            for (int i = 0; i < textInfo.characterCount; i++)
            {
                var charInfo = textInfo.characterInfo[i];
                if (!charInfo.isVisible) continue;

                // Получаем индекс первой вершины символа
                int vertIndex = charInfo.vertexIndex;
            
                // === АНИМАЦИЯ ВОЛНЫ ===
                float offsetY = Mathf.Sin(Time.time * waveSpeed + i * waveFrequency) * waveHeight;
                Vector3 offset = new Vector3(0, offsetY, 0);
            
                vertices[vertIndex + 0] += offset;
                vertices[vertIndex + 1] += offset;
                vertices[vertIndex + 2] += offset;
                vertices[vertIndex + 3] += offset;
                
                // === ПЕРЕЛИВ ЦВЕТА ===
                float hue = (Time.time * colorSpeed + i * 0.05f) % 1f;
                Color color = Color.HSVToRGB(hue, colorSaturation, colorValue);
                Color32 color32 = color;
                
                // Применяем цвет ко всем 4 вершинам буквы
                colors32[vertIndex + 0] = color32;
                colors32[vertIndex + 1] = color32;
                colors32[vertIndex + 2] = color32;
                colors32[vertIndex + 3] = color32;
            }
        
            // Обновляем меш
            meshInfo.mesh.vertices = vertices;
            meshInfo.mesh.colors32 = colors32; // ← Правильное имя
            
            // Правильные флаги: используем All или конкретные
            textComponent.UpdateVertexData(TMP_VertexDataUpdateFlags.All);
            // Или так (если нужно только позиции и цвета):
            // textComponent.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices);
            // Для цветов отдельного флага нет — они обновляются через All
        }
    }
}