public interface IEnemyHearing
{
    // Any class that implements this must have a HearSound method
    void HearSound(UnityEngine.Vector3 soundPos, float volume);
}
