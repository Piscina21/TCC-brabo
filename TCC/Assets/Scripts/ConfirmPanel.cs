using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ConfirmPanel : MonoBehaviour
{
    [Header("Referências")]
    public CharacterCreatorManager manager;
    public Text summaryText;
    public Button confirmButton;
    public Text feedbackText; // Opcional: exibe mensagem após salvar

    [Header("Evento após confirmar (Ex: carregar cena do jogo)")]
    public UnityEvent onCharacterConfirmed;

    private void OnEnable()
    {
        if (feedbackText != null)
            feedbackText.text = string.Empty;

        if (confirmButton != null)
            confirmButton.onClick.AddListener(OnConfirmClicked);

        BuildSummary();
    }

    private void OnDisable()
    {
        if (confirmButton != null)
            confirmButton.onClick.RemoveListener(OnConfirmClicked);
    }

    private void BuildSummary()
    {
        if (manager == null || summaryText == null) return;

        manager.SyncBuildData();
        CharacterBuild build = manager.currentBuild;

        StringBuilder sb = new StringBuilder();
        sb.AppendLine($"<b>Nome:</b> {build.characterName} ({build.pronoun})");
        sb.AppendLine($"<b>Raça:</b> {build.raceName}  |  <b>Classe:</b> {build.className}");
        sb.AppendLine();

        sb.AppendLine("<b>Atributos:</b>");
        foreach (var attr in build.attributes)
        {
            sb.AppendLine($"• {attr.type}: {attr.totalValue} (Base: {attr.baseValue} + Distribuídos: {attr.allocatedValue})");
        }

        if (build.abilities.Count > 0)
        {
            sb.AppendLine();
            sb.AppendLine("<b>Habilidades:</b> " + string.Join(", ", build.abilities));
        }

        if (build.appearance.Count > 0)
        {
            sb.AppendLine();
            sb.AppendLine("<b>Aparência:</b>");
            foreach (var app in build.appearance)
            {
                sb.AppendLine($"• {app.category}: {app.selectedOptionName}");
            }
        }

        summaryText.text = sb.ToString();
    }

    private void OnConfirmClicked()
    {
        if (manager == null) return;

        manager.FinalizeCharacter();

        if (feedbackText != null)
            feedbackText.text = "Personagem salvo com sucesso!";

        onCharacterConfirmed?.Invoke();
    }
}