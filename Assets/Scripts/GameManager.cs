using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour {
    [SerializeField] private InputController _inputControllerPrefab;
    private InputController _inputController;

    [SerializeField] private List<PlayerController> playerPrefabs;
    private List<PlayerController> _playerControllers;

    [SerializeField] private List<Transform> playerSpawnPositions;
    [SerializeField] private int _playerCount = 4;

    [SerializeField] private UIController _uiController;

    
    [SerializeField] private float _matchDuration = 599;
    private float _currentMatchTime;

    [SerializeField] private GameObject winPanel;
    [SerializeField] private GameObject losePanel;
    [SerializeField] private Animator maskAnimator;
    public Animator MaskAnimator => maskAnimator;

    [SerializeField] private AudioSource musicSource;

    [SerializeField] private AudioClip gameMusic;

    [SerializeField] private AudioClip endMusic;
    [SerializeField] private AudioClip dogMusic;

    [SerializeField] private EnemyPlayer enemyPlayer;

    private float dogMusicTriggeredTime;

    public int PlayerCount => _playerCount;

    private PlayerController _winPlayer;
    public bool IsPlaying => _winPlayer==null;

    public static int PlayersSelected { get; set; } = -1;

    private bool gameFinished = false;

    private void Awake()
    {
        if (PlayersSelected > 0) {
            _playerCount = PlayersSelected;
        } else {
            PlayersSelected = _playerCount;
        }

        _playerControllers = new List<PlayerController>(_playerCount);
        for (int i = 0; i < _playerCount; i++)
        {
            PlayerController playerController = Instantiate(playerPrefabs[i],playerSpawnPositions[i].position,transform.rotation,transform);
            playerController.Initialize(i, this);
            _playerControllers.Add(playerController);
        }

        _inputController = Instantiate(_inputControllerPrefab);
        _inputController.Initialize(this);

        _currentMatchTime = _matchDuration;

        musicSource.clip = gameMusic;
        musicSource.Play();
        _winPlayer = null;
    }

    private void Update()
    {
        _uiController.Refresh(_playerControllers, _currentMatchTime);
        _currentMatchTime -= Time.deltaTime;
        bool winCondition = enemyPlayer.IsDead;
        bool loseCondition = !_playerControllers[0].IsAlive || _currentMatchTime <= 0;
        if (!gameFinished && winCondition)
        {
            EndGame(true);
        }else if (!gameFinished && loseCondition ) {
            EndGame(false);
        }  else if(gameFinished && Input.anyKeyDown) {
            //StartCoroutine(ReturnMenu(0));
        }

        if (dogMusicTriggeredTime > 0 && _currentMatchTime < dogMusicTriggeredTime - dogMusic.length)
        {
            ResumeMusic();
        }
        
        if(enemyPlayer.IsDying && IsPlaying)
        {
            _winPlayer = _playerControllers[0];
        }
        
        
    }

    private void EndGame(bool win)
    {
        PlayerController winner = _playerControllers[0];
        for (int i = 1; i < _playerControllers.Count; i++)
        {
            if (_playerControllers[i].Score > winner.Score)
            {
                winner = _playerControllers[i];
            }
        }

        _winPlayer = winner;

        if (win)
        {
            winPanel.SetActive(true);
            _uiController.ShowWinner(_winPlayer); 
            
        
            musicSource.clip = endMusic;
            musicSource.Play();
        }
        else
        {
            losePanel.SetActive(true);
            musicSource.clip = dogMusic;
            musicSource.Play();
        }

        gameFinished = true;

        Time.timeScale = 0;
        StartCoroutine(ReturnMenu(10));
    }

    public void MovePlayer(int playerNumber, Vector2 direction) {
        _playerControllers[playerNumber].Move(direction);
    }

    public void ActionPlayer(int playerNumber) {
        _playerControllers[playerNumber].Action();
    }
    
    public void JumpPlayer(int playerNumber) {
        _playerControllers[playerNumber].Jump();
    }

    public void ScorePlayer(float score, PlayerController player)
    {
        if (IsPlaying)
        {
            player.Score = Mathf.Max(player.Score + score,0) ;
        }
    }

    public void Attack(PlayerController player, PlayerController target)
    {
        target.ReceiveDamage(player);
    }
    
    public void Damage(int player, float value)
    {
        if (enemyPlayer.IsAlive)
        {
            if (player < 0)
            {
                enemyPlayer.ReceiveDamage(value);
                _playerControllers[0].Score++;
            }
            else
            {
                _playerControllers[player].ReceiveDamage(value);
            }
        }
    }

    public IEnumerator ReturnMenu(float delay) {
        yield return new WaitForSecondsRealtime(delay);
        Time.timeScale = 1;
        SceneManager.LoadScene("Home");
    }
    public void PlayDogMusic()
    {
        musicSource.clip = dogMusic;
        musicSource.Play();
        dogMusicTriggeredTime = _currentMatchTime;
    }

    public void ResumeMusic()
    {
        musicSource.clip = gameMusic;
        musicSource.Play();
        dogMusicTriggeredTime = 0;
    }
}
