using UnityEngine;

public class EffectSwapper : MonoBehaviour
{
    [SerializeField] private float defaultEffectDuration = 15f;
    [SerializeField] private bool random;
    [SerializeField] private GameObject[] effects;

    private int effectIndex = -1;
    private TimedEffect timedEffect;
    private float lastChangeTime = 0f;
    private bool EffectDurationElapsed => !timedEffect && (Time.time - lastChangeTime) > defaultEffectDuration;
    private bool TimedEffectIsFinished => timedEffect && timedEffect.IsFinished;
    
    private void Update()
    {
        if (effectIndex == -1 || TimedEffectIsFinished || EffectDurationElapsed)
        {
            Change();
        }
    }

    private void Change()
    {
        int previousEffectIndex = effectIndex;
        effectIndex = random ? Random.Range(0, effects.Length) : effectIndex + 1;
        effectIndex %= effects.Length;
        
        Debug.Log($"Changing from {previousEffectIndex} to {effectIndex}");
        
        for (int index = 0; index < effects.Length; index++)
        {
            GameObject effect = effects[index];
            effect.SetActive(index == effectIndex);

            if (effectIndex == index)
            {
                timedEffect = effect.GetComponent<TimedEffect>();
            }
        }

        lastChangeTime = Time.time;
    }
}