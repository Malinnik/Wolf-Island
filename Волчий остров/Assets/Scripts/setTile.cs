using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;


public class setTile : MonoBehaviour
{
    static public int num = 100;        //Константная переменная хранящяя значение максимума существ
    public Tile groundTile;         //Тайл острова 
    public Tile rabbitTile;             //Тайл Кролика
    public Tile wolfTile;               //Тайл волка
    public Tilemap tilemap;         //Карта тайлов (из неё берётся тайлы для игры)
    bool[] isRabbitAlive = new bool[num+1]; //Жив ли кролик (0;num)
    int[] rabbitX = new int[num+1];   //Массив X координат всех кроликов (0;num)
    int[] rabbitY = new int[num+1];   //Массив Y координат всех кроликов (0;num)
    int[] wolfX = new int[num+1];   //Массив X координат всех волков (0;num)
    int[] wolfY = new int[num+1];   //Массив Y координат всех волков (0;num)
    int[] wolf = new int[num+1];    //Массив типов волков по полу (0-М, 1-Ж) (0;num)
    int[] wolfDelayBorn = new int[num + 1];
    int[] wolfHunger = new int[num + 1];
    bool[] isWolfParent = new bool[num+1];
    bool[] isWolfAlive = new bool[num + 1];
    bool spacePressed;
    bool gameStarted;
    int numOfWolfs = 1;             //Кол-во волков (0;num) //в начале игры должно быть минимум 2 волка, один М, другой Ж
    int numOfRabbits = 0;           //Кол-во кроликов (0;num)
    int currentNumOfRabbits = 1;
    int currentNumOfWolfs = 2;
    Vector3Int cellPos;             //Позиция текущей клетки (курсора установки или удаления тайлов)
    public static float timeScale;
    public static float chanceOfDuplication;   //Шанс размножения кроликов (0-100%)
    public static int startNumOfRabbits;
    public static int startNumOfWolfs;



    void startGame()
    {        
        numOfRabbits = -1;        
        numOfWolfs = -1;
        for (int i = 0; i <= startNumOfRabbits-1; i++)
        {
            if (i == num) break;
            rabbitX[i] = Random.Range(2, 20);
            rabbitY[i] = Random.Range(2, 20);
            cellPos.x = rabbitX[i];
            cellPos.y = rabbitY[i];
            numOfRabbits += 1;
            tilemap.SetTile(cellPos, rabbitTile);
            isRabbitAlive[i] = true;
            
        }
        for (int i = startNumOfRabbits+1; i <= num; i++)
        {
            if (i == num) break;
            rabbitX[i] = -1;
            rabbitY[i] = -1;
            isRabbitAlive[i] = false;
            
        }
        for (int i = 0; i <= startNumOfWolfs - 1; i++)
        {
            if (i == num) break;
            wolfX[i] = Random.Range(2, 19);
            wolfY[i] = Random.Range(2, 19);
            if (i % 2 == 0) wolf[i] = 0; else wolf[i] = 1;
            wolfHunger[i] = 10;
            cellPos.x = wolfX[i];
            cellPos.y = wolfY[i];
            numOfRabbits += 1;
            tilemap.SetTile(cellPos, wolfTile);
            isWolfAlive[i] = true;
            numOfWolfs += 1;
            
        }
        for (int i = startNumOfWolfs + 1; i <= num; i++)
        {
            if (i == num) break;
            wolfX[i] = -3;
            wolfY[i] = -3;
            isWolfAlive[i] = false;
            
        }
        Debug.Log(numOfRabbits);
    }

    void Start()
    {
        Time.timeScale = 1f;
        startGame();
        mapGen();
     //   Time.timeScale = timeScale;  //Задержка выполнения действий, 1f = реальному времени 0.5f = в 2X раза медленее обычного времени
    }

    // Передвижение всех кроликов
    void rabbitMove()
    {
        int x, y;       //X и Y координаты кролика 
        int move,plot;  //Рандомные переменные, определяют движение и возможность размножения кролика


        for (int i = 0; i <= numOfRabbits; i++) //Цикл передвижения всех кроликов 
        {
            if (isRabbitAlive[i] == false) { continue; }
            else
            {
                x = rabbitX[i]; //начальные координаты [i] кролика
                y = rabbitY[i]; //начальные координаты [i] кролика      // 1 2 3
                                                                        // 4 5 6
                move = Random.Range(1, 10);  //путь движения кролика    // 7 8 9
                switch (move)
                {
                    case 1:
                        rabbitX[i] = rabbitX[i] - 1;
                        rabbitY[i] = rabbitY[i] + 1;
                        break;
                    case 2:
                        rabbitX[i] = rabbitX[i];
                        rabbitY[i] = rabbitY[i] + 1;
                        break;
                    case 3:
                        rabbitX[i] = rabbitX[i] + 1;
                        rabbitY[i] = rabbitY[i] + 1;
                        break;
                    case 4:
                        rabbitX[i] = rabbitX[i] - 1;
                        rabbitY[i] = rabbitY[i];
                        break;
                    case 5:
                        rabbitX[i] = rabbitX[i];
                        rabbitY[i] = rabbitY[i];
                        break;
                    case 6:
                        rabbitX[i] = rabbitX[i] + 1;
                        rabbitY[i] = rabbitY[i];
                        break;
                    case 7:
                        rabbitX[i] = rabbitX[i] - 1;
                        rabbitY[i] = rabbitY[i] - 1;
                        break;
                    case 8:
                        rabbitX[i] = rabbitX[i];
                        rabbitY[i] = rabbitY[i] - 1;
                        break;
                    case 9:
                        rabbitX[i] = rabbitX[i] + 1;
                        rabbitY[i] = rabbitY[i] - 1;
                        break;
                }

                //ограничения передвижения кролика (квадрат 20 на 20)
                if (rabbitX[i] < 1)  rabbitX[i] = 1;  //Если вышел за левую грань   
    
                if (rabbitX[i] > 20) rabbitX[i] = 20;  //Если вышел за правую грань
      
                if (rabbitY[i] < 1)  rabbitY[i] = 1;  //Если вышел за нижнюю грань
    
                if (rabbitY[i] > 20) rabbitY[i] = 20;  //Если вышел за верхнюю грань
  
                //                      Y ^
                //Система координат XY    | - >
                //                            X

                cellPos.x = x;
                cellPos.y = y;
                tilemap.SetTile(cellPos, groundTile); //Замена текущего тайла кролика на землю
                cellPos.x = rabbitX[i];
                cellPos.y = rabbitY[i];
                tilemap.SetTile(cellPos, rabbitTile);  //Замена тайла земли на новую позицию кролика



                //Размножение кроликов

                plot = Random.Range(0, 100);  //Шанс размножения кролика = chaceOfDuplication%
                if (plot <= chanceOfDuplication)
                {
                    for (int j = 0; j <= num; j++)
                    {
                        
                        if (isRabbitAlive[j] == false)
                        {
                            rabbitX[j] = rabbitX[i];
                            rabbitY[j] = rabbitY[i];
                            isRabbitAlive[j] = true;
                            break;
                        }
                        else if (j == numOfRabbits)
                        {
                          /* numOfLiving += 1;
                            if (numOfLiving == numOfRabbits) isThereDead = false;
                            if (isThereDead == false)
                            { */
                                if (numOfRabbits == num) numOfRabbits -= 1; //Установка лимита кроликов (кол-во кроликов не будет превышать num)
                                numOfRabbits += 1;                      //Увеличение количества кроликов на 1
                                rabbitX[numOfRabbits] = rabbitX[i];  //Координаты нового кролика = координатам родителя
                                rabbitY[numOfRabbits] = rabbitY[i];
                                isRabbitAlive[numOfRabbits] = true;
                            break;
                           // }
                        }
                    }
                }                         
            }
        }
    }
        
    // Оставить в живых начального кролика (Update) (Не учтена смерть от волков)
    void killRabits()
    {
         numOfRabbits = 0;
         for(int i = 1; i < num; i++)
         {
            isRabbitAlive[i] = false; 
            cellPos.x = rabbitX[i];
            cellPos.y = rabbitY[i];
            tilemap.SetTile(cellPos, groundTile);
            rabbitX[i] = -1;
            rabbitY[i] = -1;  
         }    
         rabbitX[0] = 10;
         rabbitY[0] = 10;
         cellPos.x = rabbitX[0];
         cellPos.y = rabbitY[0];
         tilemap.SetTile(cellPos, groundTile);
         isRabbitAlive[0] = true;
    }

    // Генерация карты
    void mapGen()
    {
        cellPos = tilemap.WorldToCell(transform.position);  //Нахождения стартовой клетки
        cellPos.x = 1;  
        cellPos.y = 1;
        for (int y = 1; y<=21; y++)  //Создания поля 20 на 20
        {
            for (int x = 1; x <= 20; x++)
            {
                tilemap.SetTile(cellPos, groundTile);
                cellPos.x = x;
            }
            cellPos.y = y;
        }


        //координаты левого нижнего угла (1;1)

        //По не понятной мне причине, Y останавливается не доходя до последнего числа в цикле ( <= 21, останавливается на 20), поэтому Y <= 21


        //                      Y ^
        //Система координат XY    | - >
        //                            X
    }

    void wolfMove()
    {
        int x, y;
        bool[] isRabbitHere = new bool[9];
        bool[] isWolfHere = new bool[9];
        int move,sex;

        for(int i = 0; i <= numOfWolfs; i++)
        {
            x = wolfX[i];
            y = wolfY[i];
            if (isWolfAlive[i])
            {
                wolfHunger[i] -= 1;
                if (wolfHunger[i] < 1)
                {
                    isWolfAlive[i] = false;
                    cellPos.x = x;
                    cellPos.y = y;
                    tilemap.SetTile(cellPos, groundTile);
                    wolfX[i] = -3;
                    wolfY[i] = -3;
                }
                if (isWolfAlive[i])
                {

                    if (isWolfParent[i])
                    {
                        wolfDelayBorn[i] += 1;
                        if (wolfDelayBorn[i] % 10 == 0) isWolfParent[i] = false;
                    }


                    move = Random.Range(1, 10);  //путь движения волка

                    for (int j = 0; j <= numOfRabbits; j++)
                    {
                        if ((rabbitX[j] == wolfX[i] - 1) && (rabbitY[j] == wolfY[i] + 1)) isRabbitHere[0] = true;
                        if ((rabbitX[j] == wolfX[i]) && (rabbitY[j] == wolfY[i] + 1)) isRabbitHere[1] = true;
                        if ((rabbitX[j] == wolfX[i] + 1) && (rabbitY[j] == wolfY[i] + 1)) isRabbitHere[2] = true;

                        if ((rabbitX[j] == wolfX[i] - 1) && (rabbitY[j] == wolfY[i])) isRabbitHere[3] = true;
                        if ((rabbitX[j] == wolfX[i]) && (rabbitY[j] == wolfY[i])) isRabbitHere[4] = true;
                        if ((rabbitX[j] == wolfX[i] + 1) && (rabbitY[j] == wolfY[i])) isRabbitHere[5] = true;

                        if ((rabbitX[j] == wolfX[i] - 1) && (rabbitY[j] == wolfY[i] - 1)) isRabbitHere[6] = true;
                        if ((rabbitX[j] == wolfX[i]) && (rabbitY[j] == wolfY[i] - 1)) isRabbitHere[7] = true;
                        if ((rabbitX[j] == wolfX[i] + 1) && (rabbitY[j] == wolfY[i] - 1)) isRabbitHere[8] = true;
                    }

                    if (isRabbitHere[4])
                    {
                        move = 5;
                        for (int j = 0; j <= numOfRabbits; j++)
                        {
                            if ((rabbitX[j] == wolfX[i]) && (rabbitY[j] == wolfY[i]))
                            {
                                cellPos.x = rabbitX[j];
                                cellPos.y = rabbitY[j];
                                tilemap.SetTile(cellPos, wolfTile);
                                rabbitX[j] = -1;
                                rabbitY[j] = -1;
                                isRabbitAlive[j] = false;
                                wolfHunger[i] += 10;
                                if (wolfHunger[i] > 80) wolfHunger[i] = 80;
                            }
                        }

                    }
                    else if (isRabbitHere[0]) move = 1;
                    else if (isRabbitHere[1]) move = 2;
                    else if (isRabbitHere[2]) move = 3;
                    else if (isRabbitHere[3]) move = 4;
                    else if (isRabbitHere[5]) move = 6;
                    else if (isRabbitHere[6]) move = 7;
                    else if (isRabbitHere[7]) move = 8;
                    else if (isRabbitHere[8]) move = 9;



                    if (isWolfParent[i] == false)
                    {
                        if ((isRabbitHere[0] == false) && (isRabbitHere[1] == false) && (isRabbitHere[2] == false) && (isRabbitHere[3] == false) && (isRabbitHere[4] == false) && (isRabbitHere[5] == false)
                                && (isRabbitHere[6] == false) && (isRabbitHere[7] == false) && (isRabbitHere[8] == false))
                        {
                            for (int j = 0; j <= numOfWolfs; j++)
                            {
                                if ((wolfX[j] == wolfX[i] - 1) && (wolfY[j] == wolfY[i] + 1) && (wolf[j] != wolf[i]) && (isWolfParent[j] == false)) isWolfHere[0] = true;
                                if ((wolfX[j] == wolfX[i]) && (wolfY[j] == wolfY[i] + 1) && (wolf[j] != wolf[i]) && (isWolfParent[j] == false)) isWolfHere[1] = true;
                                if ((wolfX[j] == wolfX[i] + 1) && (wolfY[j] == wolfY[i] + 1) && (wolf[j] != wolf[i]) && (isWolfParent[j] == false)) isWolfHere[2] = true;

                                if ((wolfX[j] == wolfX[i] - 1) && (wolfY[j] == wolfY[i]) && (wolf[j] != wolf[i]) && (isWolfParent[j] == false)) isWolfHere[3] = true;
                                if ((wolfX[j] == wolfX[i]) && (wolfY[j] == wolfY[i]) && (wolf[j] != wolf[i]) && (isWolfParent[j] == false)) isWolfHere[4] = true;
                                if ((wolfX[j] == wolfX[i] + 1) && (wolfY[j] == wolfY[i]) && (wolf[j] != wolf[i]) && (isWolfParent[j] == false)) isWolfHere[5] = true;

                                if ((wolfX[j] == wolfX[i] - 1) && (wolfY[j] == wolfY[i] - 1) && (wolf[j] != wolf[i]) && (isWolfParent[j] == false)) isWolfHere[6] = true;
                                if ((wolfX[j] == wolfX[i]) && (wolfY[j] == wolfY[i] - 1) && (wolf[j] != wolf[i]) && (isWolfParent[j] == false)) isWolfHere[7] = true;
                                if ((wolfX[j] == wolfX[i] + 1) && (wolfY[j] == wolfY[i] - 1) && (wolf[j] != wolf[i]) && (isWolfParent[j] == false)) isWolfHere[8] = true;
                            }
                        }


                        if (isWolfHere[4])
                        {
                            move = 5;
                            for (int l = 0; l <= numOfWolfs; l++)
                            {

                                if (isWolfAlive[l] == false)
                                {
                                    wolfX[l] = wolfX[i];
                                    wolfY[l] = wolfY[i];

                                    sex = Random.Range(0, 2);
                                    if (sex == 0) wolf[l] = 0;
                                    if (sex == 1) wolf[l] = 1;

                                    wolfHunger[l] = 10;
                                    isWolfAlive[l] = true;

                                    
                                    for (int j = 0; j <= numOfWolfs; j++)
                                    {
                                        if ((wolfX[j] == wolfX[i]) && (wolfY[j] == wolfY[i]))
                                        {
                                            isWolfParent[j] = true;
                                        }
                                    }
                                    cellPos.x = wolfX[l];
                                    cellPos.y = wolfY[l];
                                //    tilemap.SetTile(cellPos, wolfTile);
                                    break;
                                   
                                }
                                else if (l == numOfWolfs)
                                {                                  
                                    if (numOfWolfs == num) numOfWolfs -= 1;
                                    numOfWolfs += 1;
                                    wolfX[numOfWolfs] = wolfX[i];
                                    wolfY[numOfWolfs] = wolfY[i];
                                    sex = Random.Range(0, 2);
                                    if (sex == 0) wolf[numOfWolfs] = 0;
                                    if (sex == 1) wolf[numOfWolfs] = 1;
                                    isWolfAlive[numOfWolfs] = true;
                                    for (int j = 0; j <= numOfWolfs; j++)
                                    {
                                        if ((wolfX[j] == wolfX[i]) && (wolfY[j] == wolfY[i]))
                                        {
                                            isWolfParent[j] = true;
                                        }
                                    }
                                    cellPos.x = wolfX[numOfWolfs];
                                    cellPos.y = wolfY[numOfWolfs];
                                //    tilemap.SetTile(cellPos, wolfTile);
                                    break;
                                }
                            }
                        }
                        else if (isWolfHere[0]) move = 1;
                        else if (isWolfHere[1]) move = 2;
                        else if (isWolfHere[2]) move = 3;
                        else if (isWolfHere[3]) move = 4;
                        else if (isWolfHere[5]) move = 6;
                        else if (isWolfHere[6]) move = 7;
                        else if (isWolfHere[7]) move = 8;
                        else if (isWolfHere[8]) move = 9;
                    }



                    switch (move)
                    {
                        case 1:
                            wolfX[i] = wolfX[i] - 1;
                            wolfY[i] = wolfY[i] + 1;
                            break;
                        case 2:
                            wolfX[i] = wolfX[i];
                            wolfY[i] = wolfY[i] + 1;
                            break;
                        case 3:
                            wolfX[i] = wolfX[i] + 1;
                            wolfY[i] = wolfY[i] + 1;
                            break;
                        case 4:
                            wolfX[i] = wolfX[i] - 1;
                            wolfY[i] = wolfY[i];
                            break;
                        case 5:
                            wolfX[i] = wolfX[i];
                            wolfY[i] = wolfY[i];
                            break;
                        case 6:
                            wolfX[i] = wolfX[i] + 1;
                            wolfY[i] = wolfY[i];
                            break;
                        case 7:
                            wolfX[i] = wolfX[i] - 1;
                            wolfY[i] = wolfY[i] - 1;
                            break;
                        case 8:
                            wolfX[i] = wolfX[i];
                            wolfY[i] = wolfY[i] - 1;
                            break;
                        case 9:
                            wolfX[i] = wolfX[i] + 1;
                            wolfY[i] = wolfY[i] - 1;
                            break;
                    }

                    //ограничения передвижения волка (квадрат 20 на 20)
                    if (wolfX[i] < 1) wolfX[i] = 1;   //Если вышел за левую грань   

                    if (wolfX[i] > 20) wolfX[i] = 20;  //Если вышел за правую грань

                    if (wolfY[i] < 1) wolfY[i] = 1;  //Если вышел за нижнюю грань

                    if (wolfY[i] > 20) wolfY[i] = 20; //Если вышел за верхнюю грань


                    for (int j = 0; j <= numOfRabbits; j++)
                    {
                        if ((rabbitX[j] == wolfX[i]) && (rabbitY[j] == wolfY[i])) isRabbitHere[4] = true;
                    }

                    if (isRabbitHere[4])
                    {
                        for (int j = 0; j <= numOfRabbits; j++)
                        {
                            if ((rabbitX[j] == wolfX[i]) && (rabbitY[j] == wolfY[i]))
                            {
                                cellPos.x = rabbitX[j];
                                cellPos.y = rabbitY[j];
                                tilemap.SetTile(cellPos, wolfTile);
                                rabbitX[j] = -1;
                                rabbitY[j] = -1;
                                isRabbitAlive[j] = false;
                                wolfHunger[i] += 10;
                                if (wolfHunger[i] > 80) wolfHunger[i] = 80;

                            }
                        }

                    }
                    //                      Y ^
                    //Система координат XY    | - >
                    //                            X

                    cellPos.x = x;
                    cellPos.y = y;
                    tilemap.SetTile(cellPos, groundTile); //Замена текущего тайла волка на землю
                    cellPos.x = wolfX[i];
                    cellPos.y = wolfY[i];
                    tilemap.SetTile(cellPos, wolfTile);  //Замена тайла земли на новую позицию волка
                    for (int j = 0; j < 9; j++) isRabbitHere[j] = false;
                    for (int j = 0; j < 9; j++) isWolfHere[j] = false;
                }
            }

            
        }
    }

    void checkCurrentNumOfLiving()
    {
        currentNumOfWolfs = num +1;
        currentNumOfRabbits = num + 1;
            for (int j = 0; j <= num; j++)
            {
                if (!isWolfAlive[j]) currentNumOfWolfs -= 1;
               // else currentNumOfWolfs -= 1;

                if (!isRabbitAlive[j]) currentNumOfRabbits -= 1;
              //  else currentNumOfRabbits -= 1;
            }
        
    }

    void FixedUpdate()
    {
        
        rabbitMove();   //Передвижение всех кроликов
        wolfMove();
        checkCurrentNumOfLiving();
        Time.fixedDeltaTime = Time.fixedDeltaTime * Time.timeScale; //Задержка перед следующим действием 
        if (!gameStarted) gameStarted = true;
        if (gameStarted) Time.timeScale = timeScale;
        Debug.Log("Кролики: " + (currentNumOfRabbits) + "   Волки: " + (currentNumOfWolfs) + "   " + isWolfParent[0] + "    " + isWolfAlive[0] + " : " + wolfHunger[0]);
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.F))
        {
            killRabits();
            mapGen();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            startGame();
            mapGen();           
        }
        if (Input.GetKeyDown(KeyCode.Escape)) SceneManager.LoadScene("Menu");

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (spacePressed)
            {
                Time.timeScale = timeScale;
                spacePressed = false;
            }
            else
            {
                Time.timeScale = 0;
                spacePressed = true;
            }
        }

    }
}
