using SideScrollerPrototype.Core;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SideScrollerPrototype.UI
{
    public class GameUIController : MonoBehaviour
    {
        private GameManager gameManager;
        private Canvas canvas;
        private GameObject menuPanel;
        private Text titleText;
        private Text infoText;
        private Text hudText;
        private Button startButton;
        private Button restartButton;

        public void Initialize(GameManager manager)
        {
            gameManager = manager;
            EnsureEventSystem();
            BuildCanvas();
        }

        public void Refresh(GameUiSnapshot snapshot)
        {
            if (hudText != null)
            {
                hudText.text = snapshot.hudText;
            }

            if (infoText != null)
            {
                infoText.text = snapshot.infoText;
            }

            if (titleText != null)
            {
                titleText.text = snapshot.titleText;
            }

            if (menuPanel != null)
            {
                menuPanel.SetActive(snapshot.showMenu);
            }

            if (startButton != null)
            {
                startButton.gameObject.SetActive(snapshot.showStartButton);
            }

            if (restartButton != null)
            {
                restartButton.gameObject.SetActive(snapshot.showRestartButton);
            }
        }

        private void BuildCanvas()
        {
            canvas = gameObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            gameObject.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            gameObject.AddComponent<GraphicRaycaster>();

            Font font = Resources.GetBuiltinResource<Font>("Arial.ttf");

            hudText = CreateText("HudText", font, new Vector2(16f, -16f), TextAnchor.UpperLeft, 20);
            hudText.alignment = TextAnchor.UpperLeft;
            hudText.rectTransform.anchorMin = new Vector2(0f, 1f);
            hudText.rectTransform.anchorMax = new Vector2(0f, 1f);
            hudText.rectTransform.pivot = new Vector2(0f, 1f);
            hudText.rectTransform.sizeDelta = new Vector2(420f, 220f);

            restartButton = CreateButton("RestartButton", font, "Restart", new Vector2(-20f, -20f), new Vector2(150f, 44f));
            restartButton.onClick.AddListener(gameManager.RestartGame);

            menuPanel = new GameObject("MenuPanel");
            menuPanel.transform.SetParent(canvas.transform, false);

            Image panelImage = menuPanel.AddComponent<Image>();
            panelImage.color = new Color(0.08f, 0.1f, 0.18f, 0.88f);

            RectTransform panelRect = menuPanel.GetComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.5f, 0.5f);
            panelRect.anchorMax = new Vector2(0.5f, 0.5f);
            panelRect.pivot = new Vector2(0.5f, 0.5f);
            panelRect.sizeDelta = new Vector2(460f, 280f);

            titleText = CreateText("TitleText", font, new Vector2(0f, 76f), TextAnchor.MiddleCenter, 30, menuPanel.transform);
            infoText = CreateText("InfoText", font, new Vector2(0f, 18f), TextAnchor.MiddleCenter, 18, menuPanel.transform);
            infoText.rectTransform.sizeDelta = new Vector2(390f, 120f);

            startButton = CreateButton("StartButton", font, "Start", new Vector2(0f, -82f), new Vector2(180f, 52f), menuPanel.transform);
            startButton.onClick.AddListener(gameManager.StartGame);
        }

        private Text CreateText(string objectName, Font font, Vector2 anchoredPosition, TextAnchor anchor, int fontSize, Transform parentOverride = null)
        {
            GameObject textObject = new GameObject(objectName);
            textObject.transform.SetParent(parentOverride == null ? canvas.transform : parentOverride, false);

            Text text = textObject.AddComponent<Text>();
            text.font = font;
            text.fontSize = fontSize;
            text.alignment = anchor;
            text.color = Color.white;

            RectTransform rectTransform = text.GetComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            rectTransform.anchoredPosition = anchoredPosition;
            rectTransform.sizeDelta = new Vector2(700f, 80f);

            return text;
        }

        private Button CreateButton(string objectName, Font font, string label, Vector2 anchoredPosition, Vector2 size, Transform parentOverride = null)
        {
            GameObject buttonObject = new GameObject(objectName);
            buttonObject.transform.SetParent(parentOverride == null ? canvas.transform : parentOverride, false);

            Image image = buttonObject.AddComponent<Image>();
            image.color = new Color(0.2f, 0.55f, 0.88f, 1f);

            Button button = buttonObject.AddComponent<Button>();
            ColorBlock colors = button.colors;
            colors.normalColor = image.color;
            colors.highlightedColor = new Color(0.32f, 0.67f, 0.96f, 1f);
            colors.pressedColor = new Color(0.16f, 0.46f, 0.72f, 1f);
            button.colors = colors;

            RectTransform rectTransform = buttonObject.GetComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(1f, 1f);
            rectTransform.anchorMax = new Vector2(1f, 1f);
            rectTransform.pivot = new Vector2(1f, 1f);
            rectTransform.anchoredPosition = anchoredPosition;
            rectTransform.sizeDelta = size;

            if (parentOverride != null)
            {
                rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
                rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
                rectTransform.pivot = new Vector2(0.5f, 0.5f);
            }

            Text labelText = CreateText(objectName + "Label", font, Vector2.zero, TextAnchor.MiddleCenter, 20, buttonObject.transform);
            labelText.text = label;
            labelText.rectTransform.sizeDelta = size;

            return button;
        }

        private void EnsureEventSystem()
        {
            if (FindObjectOfType<EventSystem>() != null)
            {
                return;
            }

            GameObject eventSystemObject = new GameObject("EventSystem");
            eventSystemObject.AddComponent<EventSystem>();
            eventSystemObject.AddComponent<StandaloneInputModule>();
        }
    }

    public struct GameUiSnapshot
    {
        public string titleText;
        public string infoText;
        public string hudText;
        public bool showMenu;
        public bool showStartButton;
        public bool showRestartButton;
    }
}
