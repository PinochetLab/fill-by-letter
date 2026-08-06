using UnityEngine;
using TMPro;

[RequireComponent(typeof(TMP_Text))]
public class WavyText : MonoBehaviour
{
    private TMP_Text m_TextComponent;

    [Header("Настройки волны")]
    [Tooltip("Скорость движения волны")]
    public float waveSpeed = 5f;
    
    [Tooltip("Высота волны (амплитуда)")]
    public float waveHeight = 10f;
    
    [Tooltip("Частота (расстояние между пиками волны)")]
    public float waveFrequency = 5f;

    [Header("Настройки цвета (Темные пастельные)")]
    [Tooltip("Скорость смены цветовых оттенков")]
    public float colorSpeed = 2f;

    void Awake()
    {
        m_TextComponent = GetComponent<TMP_Text>();
    }

    // Этот метод вызывается каждый раз, когда объект активируется
    void OnEnable()
    {
        // Если компонент уже есть, сразу применяем анимацию,
        // чтобы избежать белого кадра.
        if (m_TextComponent != null)
        {
            AnimateText();
        }
    }

    void Update()
    {
        AnimateText();
    }

    // Основная логика анимации вынесена в отдельный метод
    private void AnimateText()
    {
        // Принудительно обновляем данные текста, если они изменились
        m_TextComponent.ForceMeshUpdate();
        TMP_TextInfo textInfo = m_TextComponent.textInfo;

        int characterCount = textInfo.characterCount;

        if (characterCount == 0) return;

        // Проходим по всем символам
        for (int i = 0; i < characterCount; i++)
        {
            TMP_CharacterInfo charInfo = textInfo.characterInfo[i];

            // Пропускаем невидимые символы
            if (!charInfo.isVisible)
                continue;

            // Получаем индексы для доступа к массивам вершин и цветов
            int materialIndex = charInfo.materialReferenceIndex;
            int vertexIndex = charInfo.vertexIndex;

            // Кэшируем ссылки на массивы (это быстрее, чем получать их каждый раз в цикле)
            Vector3[] vertices = textInfo.meshInfo[materialIndex].vertices;
            Color32[] vertexColors = textInfo.meshInfo[materialIndex].colors32;

            // --- Расчет смещения Y ---
            float timeOffset = Time.time * waveSpeed + i * (waveFrequency * 0.1f);
            float offsetY = Mathf.Sin(timeOffset) * waveHeight;

            // --- Расчет цвета ---
            float hue = Mathf.Repeat(Time.time * colorSpeed * 0.1f + i * 0.05f, 1f);
            // Пастельные темные тона: S=0.35, V=0.6
            Color32 pastelDarkColor = Color.HSVToRGB(hue, 0.35f, 0.6f);

            // Применяем изменения к 4 вершинам каждого символа
            for (int j = 0; j < 4; j++)
            {
                int currentVertexIndex = vertexIndex + j;
                
                // Позиция
                Vector3 orig = vertices[currentVertexIndex];
                vertices[currentVertexIndex] = new Vector3(orig.x, orig.y + offsetY, orig.z);

                // Цвет
                vertexColors[currentVertexIndex] = pastelDarkColor;
            }
        }

        // Передаем измененные данные обратно в меш компонента
        for (int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            // Важно: помечаем массивы как измененные, чтобы Unity обновила буферы
            textInfo.meshInfo[i].mesh.vertices = textInfo.meshInfo[i].vertices;
            textInfo.meshInfo[i].mesh.colors32 = textInfo.meshInfo[i].colors32;
            m_TextComponent.UpdateGeometry(textInfo.meshInfo[i].mesh, i);
        }
    }
}