using UnityEngine;
using UnityEngine.UI;
using GameManagement;
using PuzzleData.Save;
using JigsawPuzzles.PuzzleData;

public class PreviewPuzzlePanel : MonoBehaviour
{
    [SerializeField] private Image _previewImage;
    private Toggle _previewToggle;

    private void Awake()
    {
        _previewToggle = GetComponent<Toggle>();
    }

    private void OnEnable()
    {
        LevelManager.LevelStarted += HandleLevelStarted;
        LevelManager.LevelLoaded += HandleLevelLoaded;
    }

    private void OnDisable()
    {
        LevelManager.LevelStarted -= HandleLevelStarted;
        LevelManager.LevelLoaded -= HandleLevelLoaded;
    }

    private void HandleLevelLoaded(Level level, PuzzleSave savedPuzzle)
    {
        Initialize(level.PuzzleData);
    }

    private void HandleLevelStarted(Level level)
    {
        Initialize(level.PuzzleData);
    }

    private void Initialize(PuzzleDocument puzzleData)
    {
        SetInteractable(!puzzleData.IsSecret);
        _previewImage.sprite = puzzleData.FullSizeSprite;
    }

    private void SetInteractable(bool interactable)
    {
        _previewToggle.interactable = interactable;
    }


}
