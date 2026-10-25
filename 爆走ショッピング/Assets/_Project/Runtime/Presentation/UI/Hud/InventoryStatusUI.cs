using UnityEngine;
using UnityEngine.UI;

/// <summary>カートの中身の量を四段階の短い状態で HUD に表示します。</summary>
public class InventoryStatusUI : MonoBehaviour
{
    [Header("参照")]
    [SerializeField] private PlayerInventory inventory;
    private PlayerInventory subscribedInventory;

    [Header("表示のしきい値（個数）")]
    [Tooltip("この個数以上で「多め」と表示します。")]
    [SerializeField, Min(2)] private int manyItemsThreshold = 5;
    [Tooltip("この個数以上で「満杯」と表示します。表示専用で、取得数は制限しません。")]
    [SerializeField, Min(3)] private int fullItemsThreshold = 10;

    [Header("レイアウト")]
    [SerializeField] private Vector2 anchoredPosition = new Vector2(24f, 24f);
    [SerializeField, Min(220f)] private float panelWidth = 300f;
    [SerializeField, Min(8)] private int titleFontSize = 24;

    [Header("見た目")]
    [SerializeField] private Color backgroundColor = new Color(0f, 0f, 0f, 0.45f);
    [SerializeField] private Color titleColor = Color.white;

    private RectTransform panelRect;
    private Text statusText;

    // 呼び出し元の初期化契約を保ち、表示に必要な所持品だけを受け取ります。
    public void Initialize(PlayerInventory configuredInventory, InventoryInfluenceSettings configuredInfluenceSettings)
    {
        UnsubscribeInventory();
        inventory = configuredInventory;
        if (isActiveAndEnabled) SubscribeInventory();
        RefreshText();
    }

    private void Awake()
    {
        CreateRuntimePanel();
        RefreshText();
    }

    private void OnEnable()
    {
        SubscribeInventory();
        RefreshText();
    }

    private void OnDisable()
    {
        UnsubscribeInventory();
    }

    private void SubscribeInventory()
    {
        UnsubscribeInventory();
        subscribedInventory = inventory;
        if (subscribedInventory != null) subscribedInventory.InventoryChanged += RefreshText;
    }

    private void UnsubscribeInventory()
    {
        if (subscribedInventory != null) subscribedInventory.InventoryChanged -= RefreshText;
        subscribedInventory = null;
    }

    private void CreateRuntimePanel()
    {
        if (panelRect != null) return;

        Canvas canvas = FindAnyObjectByType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasObject = new GameObject("Inventory Status Canvas");
            canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObject.AddComponent<CanvasScaler>();
            canvasObject.AddComponent<GraphicRaycaster>();
        }

        GameObject panelObject = new GameObject("Inventory Status Panel", typeof(RectTransform));
        panelObject.transform.SetParent(canvas.transform, false);
        panelRect = panelObject.GetComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.zero;
        panelRect.pivot = Vector2.zero;
        panelRect.anchoredPosition = anchoredPosition;
        panelRect.sizeDelta = new Vector2(panelWidth, titleFontSize + 28f);

        Image background = panelObject.AddComponent<Image>();
        background.color = backgroundColor;
        background.raycastTarget = false;

        GameObject textObject = new GameObject("Cart Status", typeof(RectTransform));
        textObject.transform.SetParent(panelObject.transform, false);
        RectTransform textRect = textObject.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = new Vector2(16f, 8f);
        textRect.offsetMax = new Vector2(-16f, -8f);

        statusText = textObject.AddComponent<Text>();
        statusText.font = JapaneseUIFont.Get(titleFontSize);
        statusText.fontSize = titleFontSize;
        statusText.fontStyle = FontStyle.Bold;
        statusText.color = titleColor;
        statusText.alignment = TextAnchor.MiddleLeft;
        statusText.horizontalOverflow = HorizontalWrapMode.Overflow;
        statusText.verticalOverflow = VerticalWrapMode.Truncate;
        statusText.raycastTarget = false;
    }

    private void RefreshText()
    {
        if (statusText == null) return;

        int count = inventory != null ? inventory.CarriedItems.Count : 0;
        string status = count == 0 ? "空"
            : count >= fullItemsThreshold ? "満杯"
            : count >= manyItemsThreshold ? "多め"
            : "少なめ";
        statusText.text = "カート：" + status;
    }

    private void OnValidate()
    {
        manyItemsThreshold = Mathf.Max(2, manyItemsThreshold);
        fullItemsThreshold = Mathf.Max(manyItemsThreshold + 1, fullItemsThreshold);
        panelWidth = Mathf.Max(220f, panelWidth);
        titleFontSize = Mathf.Max(8, titleFontSize);
    }
}
