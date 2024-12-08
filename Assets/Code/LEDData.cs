using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(fileName = "LightData", menuName = "Create LightData")]
public class LEDData : ScriptableObject
{
    [SerializeField] private List<Vector2> normalisedPositions;
    [Space(30)]
    [SerializeField] private TextAsset lightDataCSV;

    private List<Vector2> backupPositions;

    public Vector2 GetPosition(int ledId) => normalisedPositions[ledId];

    public int LightCount => normalisedPositions.Count;
    
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
