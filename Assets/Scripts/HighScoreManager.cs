using UnityEngine;

public class HighScoreManager : MonoBehaviour
{
    private const string HIGH_SCORE_KEY = "HighScore";
    
    public static int GetHighScore()
    {
        return PlayerPrefs.GetInt(HIGH_SCORE_KEY, 0);
    }
    
    public static void SaveHighScore(int score)
    {
        int currentHighScore = GetHighScore();
        if (score > currentHighScore)
        {
            PlayerPrefs.SetInt(HIGH_SCORE_KEY, score);
            PlayerPrefs.Save();
            Debug.Log($"New high score saved: {score}");
        }
    }
    
    public static bool IsNewHighScore(int score)
    {
        return score > GetHighScore();
    }
}
