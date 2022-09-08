using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager gm;

    public enum GameState
    {
        Ready,
        Run,
        GameOver
    }
    public GameState gState;
    public GameObject gameLabel;
    Text gameText;
    PlayerMove player;

    private void Awake()
    {
        if (gm == null)
        {
            gm = this;
        }
    }

    private void Start()
    {
        gState = GameState.Ready;
        gameText = gameLabel.GetComponent<Text>();

        player = GameObject.Find("Player").GetComponent<PlayerMove>();

        gameText.text = "Ready...";
        gameText.color = new Color32(255, 185, 0, 255);

        StartCoroutine(ReadyToStart());
    }

    IEnumerator ReadyToStart()
    {
        yield return new WaitForSeconds(2f);

        gameText.text = "Go!";
        yield return new WaitForSeconds(0.5f);

        gameLabel.SetActive(false);
        gState = GameState.Run;

    }

    private void Update()
    {
        if(player.hp <= 0)
        {
            player.GetComponentInChildren<Animator>().SetFloat("MoveMotion", 0);

            gameLabel.SetActive(true);
            gameText.text = "Game Over";

            gameText.color = new Color32(255, 0, 0, 255);

            gState = GameState.GameOver;
        }
    }
}
