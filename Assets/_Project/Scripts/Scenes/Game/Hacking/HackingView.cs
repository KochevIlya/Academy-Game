using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HackingView : MonoBehaviour
{ 
    [SerializeField] private RectTransform _arrowsContainer;
    [SerializeField] private Image _arrowPrefab;
        
    [SerializeField] private Sprite _upSprite;
    [SerializeField] private Sprite _downSprite;
    [SerializeField] private Sprite _leftSprite;
    [SerializeField] private Sprite _rightSprite;

    [SerializeField] private Color _defaultColor = Color.white;
    [SerializeField] private Color _activeColor = Color.green;
    [SerializeField] private Color _errorColor = Color.red;

    private List<Image> _spawnedArrows = new List<Image>();

    public void Show(List<Vector2> sequence)
    {
        gameObject.SetActive(true);
        Clear();

        foreach (var direction in sequence)
        {
            Image arrow = Instantiate(_arrowPrefab, _arrowsContainer, false);
        
            arrow.transform.localScale = Vector3.one;
        
            arrow.sprite = GetSprite(direction);
            arrow.color = _defaultColor;
        
            _spawnedArrows.Add(arrow);
        }
        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(_arrowsContainer.GetComponent<RectTransform>());
    }

    public void UpdateProgress(int currentIndex)
    {
        for (int i = 0; i < _spawnedArrows.Count; i++)
        {
            if (i < currentIndex)
                _spawnedArrows[i].color = _activeColor;
            else
                _spawnedArrows[i].color = _defaultColor;
        }
    }

    public void ShowError()
    {
        foreach (var arrow in _spawnedArrows)
            arrow.color = _errorColor;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        Clear();
    }

    private void Clear()
    {
        foreach (var arrow in _spawnedArrows) Destroy(arrow.gameObject);
        _spawnedArrows.Clear();
    }

    private Sprite GetSprite(Vector2 direction)
    {
        if (direction == Vector2.up) return _upSprite;
        if (direction == Vector2.down) return _downSprite;
        if (direction == Vector2.left) return _leftSprite;
        return _rightSprite;
    }
}
