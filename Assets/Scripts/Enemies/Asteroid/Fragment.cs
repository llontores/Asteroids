using UnityEngine;
using UnityEngine.Events;

public class Fragment : MonoBehaviour
{
    public event UnityAction<Fragment> OnDestroy;
}