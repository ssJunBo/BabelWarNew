using TMPro;
using UnityEngine;

public class TestHyper : MonoBehaviour
{
    public TextMeshProUGUI info;
    
    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            Application.OpenURL("www.baidu.com");
        }
    }
}
