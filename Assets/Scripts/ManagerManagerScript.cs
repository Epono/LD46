using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

public class ManagerManagerScript : MonoBehaviour
{
    private static ManagerManagerScript _instance;
    public static ManagerManagerScript Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }

    }

    public List<AgentScript> agents = new List<AgentScript>();
    public List<SpawnScript> spawns = new List<SpawnScript>();
    public List<IntermediaryGoalScript> intermediaryGoals = new List<IntermediaryGoalScript>();
    public List<TowerScript> towers = new List<TowerScript>();

    [SerializeField]
    public Transform goal;
    public GoalScript goalScript;

    [SerializeField]
    public GameObject towerGhostPrefab;
    [SerializeField]
    public GameObject towerPrefab;

    [SerializeField]
    FloatVariable towerCostSimple;

    [SerializeField]
    FloatVariable towerCostDouble;

    private TowerGhostScript towerGhostScript;
    private GameObject towerGhost;

    [SerializeField]
    LayerMask layerMask;

    [SerializeField]
    Slider slider;

    [SerializeField]
    Text healthText;

    GameObject towersParent;

    // Game over
    public bool gameOver = false;
    public bool playerWon = false;

    [SerializeField]
    GameObject gameOverPanel;

    [SerializeField]
    Text gameOverText1;

    [SerializeField]
    Text gameOverText2;

    [SerializeField]
    Text gameOverText3;

    [SerializeField]
    Button tryAgainButton;

    [SerializeField]
    Button quitButton;

    // Spéciale dédicace à Lucas
    [SerializeField]
    Slider sliderVictory;

    [SerializeField]
    FloatVariable playerVictoryTimer;

    public float elapsedTime = 0;

    //
    [SerializeField]
    GameObject infoPanel;

    [SerializeField]
    Text towerSimpleDPS;

    [SerializeField]
    Text towerSimpleLifespan;

    [SerializeField]
    Text towerSimpleCost;

    [SerializeField]
    Text towerDoubleDPS;

    [SerializeField]
    Text towerDoubleLifespan;

    [SerializeField]
    Text towerDoubleCost;

    //

    [SerializeField]
    FloatVariable timeBetweenShotsSimple;
    [SerializeField]
    FloatVariable damageSimple;
    [SerializeField]
    FloatVariable lifeSpanSimple;

    //
    [SerializeField]
    FloatVariable timeBetweenShotsDouble;
    [SerializeField]
    FloatVariable damageDouble;
    [SerializeField]
    FloatVariable lifeSpanDouble;

    void Start()
    {
        towerGhost = Instantiate(towerGhostPrefab, new Vector3(100, 100, 100), Quaternion.identity);
        towerGhostScript = towerGhost.GetComponent<TowerGhostScript>();

        goalScript = goal.GetComponent<GoalScript>();
        slider.maxValue = goalScript.maxHealth.Value;

        towersParent = GameObject.Find("Towers");

        gameOverPanel.SetActive(false);
        //tryAgainButton.gameObject.SetActive(false);
        //quitButton.gameObject.SetActive(false);

        var tryAgainButtonImage = tryAgainButton.GetComponent<Image>();
        tryAgainButtonImage.color = new Color(tryAgainButtonImage.color.r, tryAgainButtonImage.color.g, tryAgainButtonImage.color.b, 0);

        var tryAgainButtonText = tryAgainButton.GetComponentInChildren<Text>();
        tryAgainButtonText.color = new Color(tryAgainButtonText.color.r, tryAgainButtonText.color.g, tryAgainButtonText.color.b, 0);

        var quitButtonImage = quitButton.GetComponentInChildren<Image>();
        quitButtonImage.color = new Color(quitButtonImage.color.r, quitButtonImage.color.g, quitButtonImage.color.b, 0);

        var quitButtonText = quitButton.GetComponentInChildren<Text>();
        quitButtonText.color = new Color(quitButtonText.color.r, quitButtonText.color.g, quitButtonText.color.b, 0);

        sliderVictory.maxValue = playerVictoryTimer.Value;
        infoPanel.SetActive(false);

        towerSimpleDPS.text = "" + Math.Round(damageSimple.Value / timeBetweenShotsSimple.Value, 1) + " DPS";
        towerSimpleLifespan.text = lifeSpanSimple.Value + " s";
        towerSimpleCost.text = "" + towerCostSimple.Value;

        towerDoubleDPS.text = "" + Math.Round((damageDouble.Value / timeBetweenShotsDouble.Value), 1) + " DPS";
        towerDoubleLifespan.text = lifeSpanDouble.Value + " s";
        towerDoubleCost.text = "" + towerCostDouble.Value;
    }

    void Update()
    {
        slider.value = goalScript.currentHealth;

        elapsedTime += Time.deltaTime;
        sliderVictory.value = elapsedTime;

        if (elapsedTime >= playerVictoryTimer.Value && !gameOver)
        {
            GameOver(true);
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            infoPanel.SetActive(true);
        }
        if (Input.GetKeyUp(KeyCode.Tab))
        {
            infoPanel.SetActive(false);
        }
        if (Input.GetKeyDown(KeyCode.Keypad5))
        {
            infoPanel.SetActive(!infoPanel.active);
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }


        healthText.text = (int)goalScript.currentHealth + " / " + (int)goalScript.maxHealth.Value;

        bool clicked = Input.GetMouseButtonDown(0);

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            towerGhostScript.ChangeSimple(false);
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            towerGhostScript.ChangeSimple(true);
        }
        bool shiftDown = Input.GetKey(KeyCode.LeftShift);

        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, 100, layerMask) && !gameOver)
        {
            Vector3 towerGhostPosition = hit.point;
            towerGhostPosition.x = (float)Math.Round(towerGhostPosition.x + 0.5);
            towerGhostPosition.y = 0.0f;
            towerGhostPosition.z = (float)Math.Round(towerGhostPosition.z + 0.5);
            towerGhostScript.transform.position = towerGhostPosition;

            if (clicked)
            {
                if (IsValidPosition(towerGhostPosition))
                {
                    if (!shiftDown && goalScript.currentHealth > towerCostSimple.Value + 5
                        || shiftDown && goalScript.currentHealth > towerCostDouble.Value + 5)
                    {
                        towerGhostPosition.y = 0.0f;
                        GameObject newTower = Instantiate(towerPrefab, towerGhostPosition, Quaternion.identity);
                        newTower.transform.SetParent(towersParent.transform);
                        TowerScript towerScript = newTower.GetComponent<TowerScript>();
                        towerScript.Init(!shiftDown);
                        towers.Add(towerScript);

                        goalScript.currentHealth -= shiftDown ? towerCostDouble.Value : towerCostSimple.Value;

                        SoundManagerScript.instance.PlayOneShotSound(SoundManagerScript.AudioClips.TowerPlaced);
                    }
                    else
                    {
                        SoundManagerScript.instance.PlayOneShotSound(SoundManagerScript.AudioClips.TowerNotEnoughMoney);
                    }
                }
                else
                {
                    SoundManagerScript.instance.PlayOneShotSound(SoundManagerScript.AudioClips.TowerInvalidLocation);
                }
            }
        }
    }

    private bool IsValidPosition(Vector3 position)
    {
        if (towerGhostScript.collisions.Count != 0)
            return false;

        foreach (TowerScript towerScript in towers)
        {
            if (Vector3.Distance(position, towerScript.transform.position) < 1.9)
            {
                return false;
            }
        }

        return true;
    }

    private void Reset()
    {
        agents = new List<AgentScript>();
        spawns = new List<SpawnScript>();
        intermediaryGoals = new List<IntermediaryGoalScript>();
        towers = new List<TowerScript>();

        //[SerializeField]
        //public Transform goal;
        //public GoalScript goalScript;

        //[SerializeField]
        //public GameObject towerGhostPrefab;
        //[SerializeField]
        //public GameObject towerPrefab;

        //[SerializeField]
        //FloatVariable towerCost;

        //private TowerGhostScript towerGhostScript;
        //private GameObject towerGhost;

        //[SerializeField]
        //LayerMask layerMask;

        //[SerializeField]
        //Slider slider;

        //[SerializeField]
        //Text healthText;


        //GameObject towersParent;

        //public bool isPaused = false;
    }

    public void GameOver(bool playerWon = false)
    {
        gameOver = true;
        this.playerWon = playerWon;
        towerGhost.transform.position = new Vector3(100, 100, 100);
        slider.gameObject.SetActive(false);
        sliderVictory.gameObject.SetActive(false);

        foreach (SpawnScript spawn in spawns)
        {
            spawn.gameObject.SetActive(false);
        }

        for (int i = agents.Count - 1; i >= 0; i--)
        //foreach (AgentScript agent in agents)
        {
            AgentScript agent = agents[i];
            if (playerWon)
            {
                agent.TakeDamage(1000);
            }
            else
            {
                if (agent != null)
                {
                    agent.navMeshAgent.isStopped = true;
                }
            }
        }

        foreach (TowerScript tower in towers)
        {
            //tower.gameObject.SetActive(false);
            tower.timeBetweenShots = 1000;
            tower.timeSinceCreated = -1000;
            tower.timeBeforeNextShot = 1000;
            tower.particlesTweener.Pause();
            tower.transformTweener.Pause();
        }

        gameOverPanel.SetActive(true);

#if UNITY_EDITOR
        quitButton.onClick.AddListener(() => UnityEditor.EditorApplication.isPlaying = false);
#else
        quitButton.onClick.AddListener(() => Application.Quit());
#endif

        tryAgainButton.onClick.AddListener(() => StartCoroutine(LoadScene()));

        if (playerWon)
        {
            gameOverText1.text = "You kept the fire alive";
            gameOverText2.text = "Gwyn, Lord Of Cinder is happy";
            gameOverText3.text = ":)";
        }
        else
        {
            gameOverText1.text = "You let the fire die";
            gameOverText2.text = "Gwyn, Lord Of Cinder is not happy";
            gameOverText3.text = ":(";
        }

        gameOverText1.color = new Color(gameOverText1.color.r, gameOverText1.color.g, gameOverText1.color.b, 0.0f);
        gameOverText2.color = new Color(gameOverText2.color.r, gameOverText2.color.g, gameOverText2.color.b, 0.0f);
        gameOverText3.color = new Color(gameOverText3.color.r, gameOverText3.color.g, gameOverText3.color.b, 0.0f);

        IEnumerator cogo1 = FadeInAfterDelay(gameOverText1, 0.0f, 1.5f);
        IEnumerator cogo2 = FadeInAfterDelay(gameOverText2, 1.5f, 1.5f);
        IEnumerator cogo3 = FadeInAfterDelay(gameOverText3, 3.0f, 1.5f);
        IEnumerator coquit = FadeInAfterDelay(tryAgainButton.GetComponent<Image>(), 4.5f, 0.5f);
        IEnumerator coquit2 = FadeInAfterDelay(tryAgainButton.GetComponentInChildren<Text>(), 4.5f, 0.5f);
        IEnumerator cotry = FadeInAfterDelay(quitButton.GetComponent<Image>(), 4.5f, 0.5f);
        IEnumerator cotry2 = FadeInAfterDelay(quitButton.GetComponentInChildren<Text>(), 4.5f, 0.5f);

        IEnumerator coSound = PlaySoundAfterDelay(3.0f);

        StartCoroutine(cogo1);
        StartCoroutine(cogo2);
        StartCoroutine(cogo3);
        StartCoroutine(coquit);
        StartCoroutine(coquit2);
        StartCoroutine(cotry);
        StartCoroutine(cotry2);
        StartCoroutine(coSound);
    }

    IEnumerator PlaySoundAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (playerWon)
        {
            SoundManagerScript.instance.PlayOneShotSound(SoundManagerScript.AudioClips.GameWin);
        }
        else
        {
            SoundManagerScript.instance.PlayOneShotSound(SoundManagerScript.AudioClips.GameLose);
        }
    }

    IEnumerator FadeInAfterDelay(Text text, float delay, float duration)
    {
        yield return new WaitForSeconds(delay);
        DOTween.To(() => text.color, x => text.color = x, new Color(text.color.r, text.color.g, text.color.b, 1), duration);
    }

    IEnumerator FadeInAfterDelay(Image image, float delay, float duration)
    {
        yield return new WaitForSeconds(delay);
        DOTween.To(() => image.color, x => image.color = x, new Color(image.color.r, image.color.g, image.color.b, 1), duration);
    }

    IEnumerator DisplayAfterDelay(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        obj.SetActive(true);
    }

    IEnumerator LoadScene()
    {
        SoundManagerScript.instance.PlayOneShotSound(SoundManagerScript.AudioClips.TryAgain);
        yield return new WaitForSeconds(1.0f);
        SceneManager.LoadScene("MainScene");
    }
}

