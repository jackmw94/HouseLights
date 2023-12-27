using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LightData", menuName = "Create LightData")]
public class LEDData : ScriptableObject
{
    [SerializeField] private List<Vector2> normalisedPositions;
    [Space(30)]
    [SerializeField] private TextAsset lightDataCSV;

    private List<Vector2> backupPositions;

    public Vector2 GetPosition(int index) => normalisedPositions[index];

    public int LightCount => normalisedPositions.Count;

    [ContextMenu(nameof(LoadLightDataFromFile))]
    private void LoadLightDataFromFile()
    {
        string csv = lightDataCSV.text;
        string[] lines = csv.Split('\n');
        normalisedPositions = new List<Vector2>(lines.Length);

        foreach (string line in lines)
        {
            if (string.IsNullOrEmpty(line))
            {
                continue;
            }

            string[] values = line.Split(',');
            Debug.Log($"Line '{line}' has #{values.Length} values");

            float x = float.Parse(values[0]);
            float y = 1f - float.Parse(values[1]);
            Vector2 position = new Vector2(x, y);

            normalisedPositions.Add(position);
        }
    }

    public void Backup()
    {
        backupPositions = new List<Vector2>(normalisedPositions);
    }

    public void LoadBackup()
    {
        normalisedPositions = new List<Vector2>(backupPositions);
    }

    public void SetLEDPositions(Vector2[] positions)
    {
        normalisedPositions = new List<Vector2>(positions.Length);
        foreach (Vector2 pos in positions) 
        {
            normalisedPositions.Add(pos);
        }
    }
}
