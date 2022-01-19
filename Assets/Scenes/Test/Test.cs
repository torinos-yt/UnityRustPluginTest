using System.Runtime.InteropServices;
using UnityEngine;

public class Test : MonoBehaviour
{
    [DllImport("plugin_test", EntryPoint="test_add")]
    static extern int Add(int a, int b);

    [SerializeField] int _a = 0;
    [SerializeField] int _b = 0;

    void Start()
    {
        Debug.Log("Rust Plugin Test");
        Debug.Log($"Add : a + b = {Add(_a, _b)}");
    }
}
