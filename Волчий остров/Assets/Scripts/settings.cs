using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class settings : MonoBehaviour
{
    [SerializeField] InputField numOfCreatures;
    [SerializeField] InputField numOfRabbits;
    [SerializeField] InputField numOfWolfs;
    [SerializeField] InputField duplicationChance;
    [SerializeField] Slider gameSpeed;
    bool isFullScreen = true;
    private int num;
    private float timeScale;
    private float chanceOfDuplication;
    private int startNumOfRabbits;
    private int startNumOfWolfs;
    public void FullScreenToggle()
    {
        isFullScreen = !isFullScreen;
        Screen.fullScreen = isFullScreen;
    }
    void Start()
    {   
    }
    private void Update()
    {
                num = int.Parse(numOfCreatures.text);
        setTile.num = num;
        startNumOfRabbits = int.Parse(numOfRabbits.text);
        setTile.startNumOfRabbits = startNumOfRabbits;
        startNumOfWolfs = int.Parse(numOfWolfs.text);
        setTile.startNumOfWolfs = startNumOfWolfs;
        chanceOfDuplication = int.Parse(duplicationChance.text);
        setTile.chanceOfDuplication = chanceOfDuplication;
        timeScale = gameSpeed.value / 10000;
        setTile.timeScale = timeScale;

    }
}
