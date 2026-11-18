// AttributePanel.cs
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AttributePanel : MonoBehaviour
{
    [Header("Referências")]
    public CharacterCreatorManager manager;
    public Transform rowsContainer;
    public AttributeRowUI rowPrefab;
    public Text remainingPointsText;

    private readonly List<AttributeRowUI> spawnedRows = new List<AttributeRowUI>();

    private void Awake()
    {
        BuildRows();
    }

    private void OnEnable()
    {
        manager.onAttributesChanged.AddListener(UpdateUI);
        UpdateUI();
    }

    private void OnDisable()
    {
        manager.onAttributesChanged.RemoveListener(UpdateUI);
    }

    private void BuildRows()
    {
        foreach (Transform child in rowsContainer)
            Destroy(child.gameObject);

        spawnedRows.Clear();

        foreach (AttributeType attr in Enum.GetValues(typeof(AttributeType)))
        {
            AttributeRowUI row = Instantiate(rowPrefab, rowsContainer);
            row.Setup(attr, manager);
            spawnedRows.Add(row);
        }
    }

    private void UpdateUI()
    {
        foreach (var row in spawnedRows)
            row.Refresh();

        if (remainingPointsText != null)
            remainingPointsText.text = "Pontos restantes: " + manager.GetRemainingPoints();
    }
}