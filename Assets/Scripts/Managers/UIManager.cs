using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    protected Image heartsFull;
    protected Image heartsEmpty;
    protected CanvasGroup dead;
    protected CanvasGroup reward;
    protected Canvas uiCanvas;
    protected Text level;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        uiCanvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        heartsFull = GameObject.Find("HeartsFull").GetComponent<Image>();
        heartsEmpty = GameObject.Find("HeartsEmpty").GetComponent<Image>();
        level = GameObject.Find("Level").GetComponent<Text>();
        dead = GameObject.Find("GameOverCanvas").GetComponent<CanvasGroup>();
        dead.alpha = 0;
        dead.blocksRaycasts = false;
        dead.interactable = false;
        reward = GameObject.Find("RewardCanvas").GetComponent<CanvasGroup>();
        HideRewardSelect();
    }

    void Update()
    {
        heartsFull.fillAmount = GameManager.Instance.lives * (1.0f / 11.0f);
        heartsEmpty.fillAmount = GameManager.Instance.maxLives * (1.0f / 11.0f);
        level.text = GameManager.Instance.level.ToString();
    }

    public void GameOver()
    {
        uiCanvas.enabled = false;
        dead.alpha = 1;
        dead.interactable = true;
        dead.blocksRaycasts = true;
    }

    public void HideRewardSelect()
    {
        reward.alpha = 0;
        reward.interactable = false;
        reward.blocksRaycasts = false;
    }

    public void RewardSelect()
    {
        reward.alpha = 1;
        reward.interactable = true;
        reward.blocksRaycasts = true;
    }
}
