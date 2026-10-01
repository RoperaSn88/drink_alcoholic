using UnityEngine;
using UnityEngine.InputSystem;

namespace DrinkAlcoholic
{
    public class MainGameController : MonoBehaviour
    {
        [SerializeField] GameConfig config;
        [SerializeField] PlayerController player;
        [SerializeField] DrinkerTarget target;
        [SerializeField] AlcoholGaugeView gaugeView;
        [SerializeField] MainGameHud hud;
        [SerializeField] GameMenuPanel gameMenuPanel;
        [SerializeField] GameOverPanel gameOverPanel;

        [Header("Audio（任意）")]
        [SerializeField] AudioClip bgm;
        [SerializeField] AudioClip startSe;
        [SerializeField] AudioClip bottleEmptySe;
        [SerializeField] AudioClip gameOverSe;

        readonly ScoreCounter score = new ScoreCounter();

        public GameState State { get; private set; }

        void Awake()
        {
            if (!ValidateReferences())
            {
                enabled = false;
                return;
            }

            hud.OnStartClicked += StartParty;
            gameMenuPanel.OnContinueClicked += ResumeGame;
            gameMenuPanel.OnExitClicked += SceneLoader.LoadTitle;
            score.OnChanged += hud.SetScore;
            player.OnBottleEmptied += HandleBottleEmptied;
        }

        void Start()
        {
            gameOverPanel.Hide();
            gameMenuPanel.Hide();
            score.Reset();
            SetState(GameState.Ready);
            AudioManager.Instance.PlayBgm(bgm);
        }

        public void StartParty()
        {
            if (State != GameState.Ready) return;

            target.Initialize(config);
            gaugeView.Bind(target.Gauge);
            gaugeView.ShowFor(config.gaugeVisibleSeconds);
            score.Reset();
            player.TakeNewBottle();

            AudioManager.Instance.PlaySe(startSe);
            SetState(GameState.Playing);
        }

        void Update()
        {
            if (Keyboard.current?.escapeKey.wasPressedThisFrame == true)
            {
                if (State == GameState.Playing)
                {
                    SetState(GameState.Paused);
                    gameMenuPanel.Show();
                }
                else if (State == GameState.Paused)
                {
                    ResumeGame();
                }
            }

            if (State != GameState.Playing) return;

            float dt = Time.deltaTime;
            if (hud.IsPouring) player.PourTo(target, config.pourRateMlPerSec, dt);
            target.Gauge.Tick(dt);

            if (target.Gauge.IsOverflow) HandleGameOver();
        }

        void HandleBottleEmptied()
        {
            // ゲージを超えたのと同じフレームで飲み切った1本もカウントする
            score.AddBottle();
            AudioManager.Instance.PlaySe(bottleEmptySe);
            player.TakeNewBottle();
        }

        void HandleGameOver()
        {
            SetState(GameState.GameOver);
            AudioManager.Instance.PlaySe(gameOverSe);
            gameOverPanel.Show(score.BottleCount);
        }

        void ResumeGame()
        {
            gameMenuPanel.Hide();
            SetState(GameState.Playing);
        }

        bool ValidateReferences()
        {
            bool valid = true;
            valid &= Require(config, nameof(config));
            valid &= Require(player, nameof(player));
            valid &= Require(target, nameof(target));
            valid &= Require(gaugeView, nameof(gaugeView));
            valid &= Require(hud, nameof(hud));
            valid &= Require(gameMenuPanel, nameof(gameMenuPanel));
            valid &= Require(gameOverPanel, nameof(gameOverPanel));
            return valid;
        }

        bool Require(UnityEngine.Object reference, string fieldName)
        {
            if (reference != null) return true;
            Debug.LogError($"{nameof(MainGameController)} の {fieldName} が設定されていません。", this);
            return false;
        }

        void SetState(GameState state)
        {
            State = state;
            hud.SetState(state);
        }
    }
}
