# Настройка поддержки HighDPI мониторов

## Внесённые изменения

### 1. Обновлены префабы UI с CanvasScaler

Все основные UI префабы обновлены для поддержки масштабирования на HighDPI мониторах:

- `Assets/_Project/Visual/Prefabs/UI/GlobalUiRoot.prefab`
- `Assets/_Project/Visual/Prefabs/UI/GuiServiceRoot.prefab`
- `Assets/_Project/Visual/Prefabs/UI/GuiGameServicePrefab.prefab`
- `Assets/_Project/Visual/Prefabs/UI/HealthView.prefab`
- `Assets/_Project/Visual/Prefabs/UI/HackingSelectionWindow.prefab`
- `Assets/_Project/Visual/UI/HackingView/HackingPrefab.prefab`

**Параметры CanvasScaler:**
- `uiScaleMode`: ScaleWithScreenSize (1)
- `referenceResolution`: 1920x1080
- `screenMatchMode`: MatchWidthOrHeight (1)
- `matchWidthOrHeight`: 0.5 (баланс между шириной и высотой)
- `referencePixelsPerUnit`: 100

### 2. Настройки проекта (ProjectSettings.asset)

- `resolutionScalingMode`: 1 (включено масштабирование разрешения)

### 3. Скрипты редактора

Добавлен скрипт `Assets/_Project/Scripts/Editor/HighDPIConfig.cs` с утилитами для настройки HighDPI:

**Меню:**
- `Tools/HighDPI/Configure Current Scene` - настроить все Canvas в текущей сцене
- `Tools/HighDPI/Enable Project Settings` - включить поддержку HighDPI в настройках проекта
- `Tools/HighDPI/Add CanvasScaler to Selection` - добавить CanvasScaler к выбранному объекту

## Как это работает

1. **ScaleWithScreenSize** - UI автоматически масштабируется под разрешение экрана
2. **Reference Resolution 1920x1080** - базовое разрешение, от которого считается масштаб
3. **MatchWidthOrHeight 0.5** - одинаково учитывает ширину и высоту при масштабировании
   - 0 = только ширина
   - 1 = только высота
   - 0.5 = баланс (рекомендуется)

## Проверка

Для проверки корректности работы HighDPI:

1. Запустите игру в разных разрешениях:
   - 1920x1080 (Full HD)
   - 2560x1440 (2K QHD)
   - 3840x2160 (4K UHD)
   - 1366x768 (HD)

2. UI должен:
   - Сохранять пропорции элементов
   - Оставаться чётким на любых DPI
   - Корректно масштабироваться без искажений

## Примечания

- Для спрайтов рекомендуется использовать атласы с mipmaps
- Шрифты TextMeshPro автоматически масштабируются
- При добавлении новых UI префабов используйте те же настройки CanvasScaler
