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
    FloatVariable towerCost;

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
    }

    void Update()
    {
        slider.value = goalScript.currentHealth;

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
                    if (goalScript.currentHealth > towerCost.Value)
                    {
                        towerGhostPosition.y = 0.0f;
                        GameObject newTower = Instantiate(towerPrefab, towerGhostPosition, Quaternion.identity);
                        newTower.transform.SetParent(towersParent.transform);
                        TowerScript towerScript = newTower.GetComponent<TowerScript>();
                        towerScript.Init(!Input.GetKey(KeyCode.LeftShift));
                        towers.Add(towerScript);
                        goalScript.currentHealth -= towerCost.Value;
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

        foreach (SpawnScript spawn in spawns)
        {
            spawn.gameObject.SetActive(false);
        }

        foreach (AgentScript agent in agents)
        {
            if (agent != null)
            {
                agent.navMeshAgent.isStopped = true;
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

        tryAgainButton.onClick.AddListener(() => SceneManager.LoadScene("MainScene"));

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

        StartCoroutine(cogo1);
        StartCoroutine(cogo2);
        StartCoroutine(cogo3);
        StartCoroutine(coquit);
        StartCoroutine(coquit2);
        StartCoroutine(cotry);
        StartCoroutine(cotry2);
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
}

