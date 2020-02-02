using UnityEngine;

public class PlatformActivate : MonoBehaviour
{
    public bool ActivateOnAndroid;
    private void Awake()
    {
#if UNITY_ANDROID
        gameObject.SetActive(ActivateOnAndroid);
#else
        gameObject.SetActive(!ActivateOnAndroid);
#endif
    }
}