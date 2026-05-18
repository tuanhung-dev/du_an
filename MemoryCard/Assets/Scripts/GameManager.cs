using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Card Settings")]
    public GameObject cardPrefab;
    public Sprite[] cardSprites;
    public Sprite backSprite;

    [Header("Grid Settings")]
    public int columns = 2;
    public float spacing = 1.8f;

    [Header("UI Settings")]
    public TextMeshProUGUI scoreText;
    public GameObject scoreUI;
    public GameObject endPanel;
    public TextMeshProUGUI finalScoreText;

    private int matchedPairs = 0;
    private List<Card> cards = new List<Card>();
    private Card firstCard;
    private Card secondCard;
    private bool canClick = true;
    private int score = 0;

    void Start()
    {
        SpawnCards();

        if (endPanel != null)
            endPanel.SetActive(false);
    }

    void SpawnCards()
    {
        if (cardPrefab == null)
        {
            Debug.LogError("CHƯA GÁN CARD PREFAB!");
            return;
        }

        List<(Sprite sprite, int id)> spawnList = new List<(Sprite, int)>();
        int idCounter = 0;

        foreach (Sprite s in cardSprites)
        {
            if (s == null) continue;
            spawnList.Add((s, idCounter));
            spawnList.Add((s, idCounter));
            idCounter++;
        }

        for (int i = 0; i < spawnList.Count; i++)
        {
            var temp = spawnList[i];
            int rand = Random.Range(i, spawnList.Count);
            spawnList[i] = spawnList[rand];
            spawnList[rand] = temp;
        }

        int rows = Mathf.CeilToInt((float)spawnList.Count / columns);
        float width = (columns - 1) * spacing;
        float height = (rows - 1) * spacing;

        for (int i = 0; i < spawnList.Count; i++)
        {
            GameObject obj = Instantiate(cardPrefab);
            var data = spawnList[i];
            Card card = obj.GetComponent<Card>();
            card.Init(this, data.sprite, backSprite, data.id);

            int col = i % columns;
            int row = i / columns;
            float x = col * spacing - width / 2f;
            float y = height / 2f - row * spacing;
            obj.transform.position = new Vector3(x, y, 0);

            cards.Add(card);
        }
    }

    public bool CanClick()
    {
        return canClick;
    }

    public void OnCardClicked(Card card)
    {
        if (!canClick) return;

        if (firstCard == null)
        {
            firstCard = card;
        }
        else if (secondCard == null && card != firstCard)
        {
            secondCard = card;
            StartCoroutine(CheckMatch());
        }
    }

    IEnumerator CheckMatch()
    {
        canClick = false;
        yield return new WaitForSeconds(0.5f);

        if (firstCard.id == secondCard.id)
        {
            score += 10;
            firstCard.Hide();
            secondCard.Hide();
            matchedPairs++;

            UpdateScoreUI();

            if (matchedPairs >= cardSprites.Length)
            {
                EndGame();
            }
        }
        else
        {
            score -= 2;
            firstCard.Flip();
            secondCard.Flip();
            UpdateScoreUI();
        }

        firstCard = null;
        secondCard = null;
        canClick = true;
    }

    void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
        }
    }

    void EndGame()
    {
        Debug.Log("🏆 END GAME - Score: " + score);

        if (scoreUI != null)
            scoreUI.SetActive(false);

        if (endPanel != null)
        {
            endPanel.SetActive(true);
            Debug.Log("📦 EndPanel active: " + endPanel.activeSelf);
        }

        if (finalScoreText != null)
        {
            finalScoreText.text = "Score: " + score;
        }

        // 🔥 KHÔNG GÁN CODE NỮA - ĐỂ GÁN BẰNG TAY
    }

    // 🔥 HÀM RESTART - PUBLIC
    public void RestartGame()
    {
        Debug.Log("RESTART CLICK");

        Time.timeScale = 1f;

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}