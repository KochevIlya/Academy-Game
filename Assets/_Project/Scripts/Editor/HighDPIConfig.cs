using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Скрипт для настройки поддержки HighDPI мониторов в Unity.
/// </summary>
public class HighDPIConfig
{
    /// <summary>
    /// Включает поддержку HighDPI для всех Canvas в текущей сцене.
    /// </summary>
    [MenuItem("Tools/HighDPI/Configure Current Scene")]
    public static void ConfigureCurrentScene()
    {
        var canvases = Object.FindObjectsOfType<Canvas>();
        int configuredCount = 0;

        foreach (var canvas in canvases)
        {
            var canvasGroup = canvas.GetComponent<CanvasScaler>();
            if (canvasGroup == null)
            {
                canvasGroup = canvas.gameObject.AddComponent<CanvasScaler>();
            }

            // Настройка CanvasScaler для поддержки HighDPI
            canvasGroup.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasGroup.referenceResolution = new Vector2(1920, 1080);
            canvasGroup.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            canvasGroup.matchWidthOrHeight = 0.5f; // Баланс между шириной и высотой
            canvasGroup.referencePixelsPerUnit = 100;

            // Включаем пиксель-перфект для чёткости на любых DPI
            canvas.pixelPerfect = false; // Отключаем для плавного масштабирования

            configuredCount++;
            Debug.Log($"Настроен Canvas: {canvas.gameObject.name}");
        }

        Debug.Log($"Всего настроено Canvas: {configuredCount}");
    }

    /// <summary>
    /// Включает поддержку HighDPI в настройках проекта.
    /// </summary>
    [MenuItem("Tools/HighDPI/Enable Project Settings")]
    public static void EnableInProjectSettings()
    {
        // Для Windows Standalone включаем поддержку HighDPI через PlayerSettings
        PlayerSettings.SetPropertyBool("highdpi_support", true, BuildTargetGroup.Standalone);
        
        Debug.Log("HighDPI поддержка включена в настройках проекта");
        Debug.Log("Для Windows: Убедитесь, что в PlayerSettings > Resolution and Presentation > 'Run In Window' включено");
    }

    /// <summary>
    /// Добавляет компонент CanvasScaler с правильными настройками к выбранному GameObject.
    /// </summary>
    [MenuItem("Tools/HighDPI/Add CanvasScaler to Selection")]
    public static void AddCanvasScalerToSelection()
    {
        if (Selection.activeGameObject == null)
        {
            Debug.LogWarning("Выберите GameObject с Canvas");
            return;
        }

        var canvas = Selection.activeGameObject.GetComponent<Canvas>();
        if (canvas == null)
        {
            Debug.LogWarning("Выбранный объект не имеет Canvas");
            return;
        }

        var canvasScaler = canvas.GetComponent<CanvasScaler>();
        if (canvasScaler == null)
        {
            canvasScaler = canvas.gameObject.AddComponent<CanvasScaler>();
        }

        canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasScaler.referenceResolution = new Vector2(1920, 1080);
        canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        canvasScaler.matchWidthOrHeight = 0.5f;
        canvasScaler.referencePixelsPerUnit = 100;

        Debug.Log($"Добавлен CanvasScaler к {canvas.gameObject.name}");
    }
}
