using TMPro;
using UnityEngine;

namespace Animations
{
    public class WavyText : MonoBehaviour
{
    public TMP_Text textComponent;
    
    [Header("Wave Settings")]
    public float waveSpeed = 1.5f;        // Чуть медленнее для плавности
    public float waveHeight = 3f;          // Меньше амплитуда
    public float waveFrequency = 0.4f;     // Более редкие волны
    
    [Header("Color Settings - Pastel")]
    [Range(0f, 1f)]
    public float colorSpeed = 0.7f;        // Медленный перелив
    [Range(0f, 1f)]
    public float pastelSaturation = 0.35f; // Низкая насыщенность = пастель
    [Range(0f, 1f)]
    public float pastelBrightness = 0.9f;  // Светлые тона
    
    [Header("Optional")]
    public bool useFixedPalette = false;   // Включить для фиксированной палитры
    public Color[] pastelPalette = new Color[]
    {
        new Color(1f, 0.7f, 0.7f), // Розовый
        new Color(0.7f, 0.9f, 0.8f), // Мятный
        new Color(0.8f, 0.7f, 1f), // Лаванда
        new Color(1f, 0.9f, 0.6f), // Кремовый
        new Color(0.6f, 0.8f, 1f), // Небесный
        new Color(1f, 0.7f, 0.9f), // Розовый-фламинго
    };
    
    private float phaseOffset = 0f;

    void Start()
    {
        phaseOffset = Random.Range(0f, 100f); // Случайный сдвиг для каждого объекта
    }

    void Update()
    {
        if (textComponent == null) return;
        
        textComponent.ForceMeshUpdate();
        var textInfo = textComponent.textInfo;
        
        if (textInfo.characterCount == 0) return;
        
        var meshInfo = textInfo.meshInfo[0];
        var vertices = meshInfo.vertices;
        var colors32 = meshInfo.colors32;
        
        // Сохраняем оригинальные позиции для плавной анимации
        Vector3[] basePositions = new Vector3[vertices.Length];
        System.Array.Copy(vertices, basePositions, vertices.Length);
        
        for (int i = 0; i < textInfo.characterCount; i++)
        {
            var charInfo = textInfo.characterInfo[i];
            if (!charInfo.isVisible) continue;
            
            int vertIndex = charInfo.vertexIndex;
            
            // === МЯГКАЯ ВОЛНА (синус с плавными переходами) ===
            float time = Time.time * waveSpeed + phaseOffset;
            float wave = Mathf.Sin(time + i * waveFrequency);
            
            // Используем smooth step для более органичного движения
            float smoothWave = Mathf.SmoothStep(-1f, 1f, wave * 0.5f + 0.5f) * 2f - 1f;
            float offsetY = smoothWave * waveHeight;
            
            // Добавляем небольшую горизонтальную вибрацию для живости
            float offsetX = Mathf.Sin(time * 0.7f + i * 0.3f) * waveHeight * 0.15f;
            
            Vector3 offset = new Vector3(offsetX, offsetY, 0);
            
            vertices[vertIndex + 0] += offset;
            vertices[vertIndex + 1] += offset;
            vertices[vertIndex + 2] += offset;
            vertices[vertIndex + 3] += offset;
            
            // === ПАСТЕЛЬНЫЙ ЦВЕТ ===
            Color color;
            
            if (useFixedPalette)
            {
                // Используем фиксированную палитру
                int colorIndex = i % pastelPalette.Length;
                color = pastelPalette[colorIndex];
            }
            else
            {
                // Плавный перелив в пастельных тонах
                float hue = (Time.time * colorSpeed + i * 0.04f + phaseOffset) % 1f;
                color = Color.HSVToRGB(hue, pastelSaturation, pastelBrightness);
            }
            
            // Добавляем вариацию яркости для объема
            float brightnessVariation = 0.85f + 0.15f * Mathf.Sin(Time.time * 0.5f + i * 0.2f);
            color *= brightnessVariation;
            
            Color32 color32 = color;
            
            colors32[vertIndex + 0] = color32;
            colors32[vertIndex + 1] = color32;
            colors32[vertIndex + 2] = color32;
            colors32[vertIndex + 3] = color32;
        }
        
        meshInfo.mesh.vertices = vertices;
        meshInfo.mesh.colors32 = colors32;
        textComponent.UpdateVertexData(TMP_VertexDataUpdateFlags.All);
    }
}
}