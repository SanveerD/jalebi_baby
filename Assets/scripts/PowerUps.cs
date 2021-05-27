using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PowerUps : MonoBehaviour
{

    // Public 
    public AudioSource levelUpSound;

    public Text jumpPowerText;
    public Text speedPowerText;
    public Text levelText;

    // Private
    private int jumpCoolDown = 4;
    private int speedCoolDown = 3;
    private int level;

    void Awake()
    {
        level = 1;
    }

    void OnEnable()
    {
        PlayerMovement.OnJumpPowerUp += OnJumpPowerUp;
        PlayerMovement.OnSpeedPowerUp += OnSpeedPowerUp;
        PlayerMovement.OnLevelUp += OnLevelUp;
        PlayerMovement.OnPlayerDied += OnPlayerDied;

    }

    void OnDisable()
    {
        PlayerMovement.OnJumpPowerUp -= OnJumpPowerUp;
        PlayerMovement.OnSpeedPowerUp -= OnSpeedPowerUp;
        PlayerMovement.OnLevelUp -= OnLevelUp;
        PlayerMovement.OnPlayerDied -= OnPlayerDied;
    }

    void OnLevelUp()
    {
        levelUpSound.Play();

        levelText.gameObject.SetActive(true);
        level++;
        levelText.text = "Level " + level.ToString();
        StartCoroutine("LevelCountDown");
    }

    void OnJumpPowerUp() 
    {
        jumpPowerText.gameObject.SetActive(true);
        StopCoroutine("JumpCountDown");
        StartCoroutine("JumpCountDown");

    }

    void OnSpeedPowerUp()
    {
        speedPowerText.gameObject.SetActive(true);
        StopCoroutine("SpeedCountDown");
        StartCoroutine("SpeedCountDown");
    }

    IEnumerator JumpCountDown()
    {
        
        for (int i = 0;  i < jumpCoolDown; i++)
        {
            jumpPowerText.text = "Super Jump: " + (jumpCoolDown - i).ToString() + "s";
            yield return new WaitForSeconds(1);
        }
        jumpPowerText.gameObject.SetActive(false);
    }

    IEnumerator SpeedCountDown()
    {

        for (int i = 0; i < speedCoolDown; i++)
        {
            speedPowerText.text = "Super Speed: " + (speedCoolDown - i).ToString() + "s";
            yield return new WaitForSeconds(1);
        }
        speedPowerText.gameObject.SetActive(false);
    }

    IEnumerator LevelCountDown()
    {
        int count = 3;
        for (int i = 0; i < count; i++)
        {
            yield return new WaitForSeconds(1);
        }
        levelText.gameObject.SetActive(false);
    }

    void OnPlayerDied()
    {
        level = 1;
        speedPowerText.gameObject.SetActive(false);
        levelText.gameObject.SetActive(false);
        jumpPowerText.gameObject.SetActive(false);
    }
}
