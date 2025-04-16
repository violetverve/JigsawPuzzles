
using UIS;
using UnityEngine;
using JigsawPuzzles.PuzzleData;
using UI.MenuScene.Puzzle;
using System.Collections.Generic;
using System.Linq;

public class ScrollManager : MonoBehaviour
{

    [SerializeField] private Scroller _scrollList = null;
    [SerializeField] private int _itemsPerRow= 2;
    [SerializeField] private RectTransform _content;
    private int _itemHeight;
    private List<PuzzlePanelUI> _puzzlePanels = new List<PuzzlePanelUI>();

    public List<PuzzlePanelUI> PuzzlePanels => _puzzlePanels;

    private List<PuzzleDocument> _puzzleDocuments = new List<PuzzleDocument>();

    public void Initialize(List<PuzzleDocument> puzzleDocuments)
    {
        if (_scrollList.IsInited)
        {
            _scrollList.RecycleAll();
        }

        if (puzzleDocuments.Count == 0)
        {
            return;
        }

        _puzzleDocuments = puzzleDocuments;

        int itemCount = Mathf.CeilToInt((float)_puzzleDocuments.Count / _itemsPerRow);

        _scrollList.InitData(itemCount);

        var objects = _scrollList.GetAllViews();

        foreach (var obj in objects)
        {
            var panels = obj.GetComponentsInChildren<PuzzlePanelUI>().ToList();

            _puzzlePanels.AddRange(panels);
        }
    }

    private void OnEnable()
    {
        _scrollList.OnFill += OnFillItem;
        _scrollList.OnHeight += OnHeightItem;
    }

    private void OnDisable()
    {
        _scrollList.OnFill -= OnFillItem;
        _scrollList.OnHeight -= OnHeightItem;
    }

    void OnFillItem(int index, GameObject item)
    {
        List<PuzzlePanelUI> puzzlePanels = item.GetComponentsInChildren<PuzzlePanelUI>().ToList();

        int currentIndex = index * _itemsPerRow;

        foreach (PuzzlePanelUI puzzlePanel in puzzlePanels)
        {
            if (currentIndex >= _puzzleDocuments.Count)
            {
                puzzlePanel.SetPlaceholderState();
                return;
            }

            var puzzleDocument = _puzzleDocuments[currentIndex];

            puzzlePanel.LoadPuzzlePanel(puzzleDocument);

            currentIndex++;
        }
    }

    int OnHeightItem(int index)
    {
        if (_itemHeight == 0)
        {
            CalculateItemHeight();
        }

        return _itemHeight;
    }

    private int CalculateItemHeight()
    {
        float prefabItemHeight = 470f;
        float prefabParentWidth = 1080f;

        float scalingFactor = prefabItemHeight / prefabParentWidth;

        _itemHeight = Mathf.RoundToInt(_content.rect.width * scalingFactor);

        return _itemHeight;
    }

    public bool IsInitialized()
    {
        return _scrollList.IsInited;
    }    

    public void UpdatePuzzlePanel(int id)
    {
        var panel = _puzzlePanels.FirstOrDefault(p => p.PuzzleID == id);
        panel?.Reload();
    }

}
