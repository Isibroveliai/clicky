using TMPro;
using UnityEngine;
using UnityEngine.UI;

// TODO: Make info about upgrade be visible in editor.

public class UpgradeButton : MonoBehaviour
{
    [SerializeField]
    public Upgrade upgrade;

    private TMP_Text label;
    private Button button;

    void Start()
    {
        GameManager manager = GameManager.Instance;
        label = transform.Find("Label").GetComponent<TMP_Text>();
        button = GetComponent<Button>();

        label.text = upgrade.name;
        button.onClick.AddListener(() => upgrade.Buy());
    }

    void FixedUpdate()
    {
        // TODO: Refactor, to not check can buy every frame.
        // Add "OnClick" event to ClickerManager to solve this problem.
        button.interactable = upgrade.CanBuy();
    }
}
