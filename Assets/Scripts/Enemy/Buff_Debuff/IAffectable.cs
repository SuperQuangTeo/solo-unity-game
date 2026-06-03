using UnityEngine;

public interface IAffectable
{
    void ApplyBuff(StatusEffectData statusEffect);
    void ApplyDebuff(StatusEffectData statusEffect);
    void ClearAll(StatusEffectData statusEffect);
}
