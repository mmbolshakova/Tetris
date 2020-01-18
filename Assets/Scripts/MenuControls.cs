using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class MenuControls : MonoBehaviour
{
    public Canvas ratingCanvas;
    [SerializeField] private Button play;
    [SerializeField] private Button rat;
    [SerializeField] private Button exit;
    public Text ratingText;
    private string loadRating;
    private bool flag;
    public void PlayPressed()
    {
        string sceneGame = "Game";
        Time.timeScale = 1;
        SceneManager.LoadScene(sceneGame);
    }

    private void DisplayButtons()
    {
        play.gameObject.SetActive(flag);
        rat.gameObject.SetActive(flag);
        exit.gameObject.SetActive(flag);
        ratingCanvas.gameObject.SetActive(!flag);
    }
    public void RatingPressed()
    {
        flag = false;
        DisplayButtons();
        loadRating = PlayerPrefs.GetString("New");
        string[] display = loadRating.Split(',');
        ratingText.text = "1. " + display[0] + "\n" + "2." + display[1] + "\n" + "3." + display[2] + "\n" +
                       "4." + display[3] + "\n" + "5." + display[4];
    }
    public void ExitPressed()
    {
        Application.Quit();
    }

    public void MenuPausePressed()
    {
        flag = true;
        DisplayButtons();
    }
}
