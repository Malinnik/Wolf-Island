﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class setTile : MonoBehaviour
{
    const int num = 399;  //Константная переменная хранящяя значение максимума существ
    public Tile groundTile;         //Тайл острова 
    public Tile rabbitTile;             //Тайл Кролика
    public Tile wolfTile;               //Тайл волка
    public Tilemap tilemap;         //Карта тайлов (из неё берётся тайлы для игры)
    int[] rabbitX = new int[num+1];   //Массив X координат всех кроликов (0;num)
    int[] rabbitY = new int[num+1];   //Массив Y координат всех кроликов (0;num)
    int[] wolfX = new int[num + 1];   //Массив X координат всех волков (0;num)
    int[] wolfY = new int[num + 1];   //Массив Y координат всех волков (0;num)
    int[] wolf = new int[num + 1];    //Массив типов волков по полу (0-М, 1-Ж) (0;num)
    int numOfWolfs = 1;             //Кол-во волков (0;num) //в начале игры должно быть минимум 2 волка, один М, другой Ж
    int numOfRabbits = 0;           //Кол-во кроликов (0;num)
    [SerializeField] float timeScale;
    [SerializeField] int chanceOfDuplication = 20;   //Шанс размножения кроликов (0-100%)
    
    Vector3Int cellPos;             //Позиция текущей клетки (курсора установки или удаления тайлов)



    void Start()
    {   
        rabbitX[0] = 10;        //Создания начального кролика в координате 10 на 10
        rabbitY[0] = 10;
        wolfX[0] = 1;
        wolfY[0] = 1;
        wolfX[1] = 20;
        wolfY[1] = 1;
        wolf[0] = 0;
        wolf[1] = 1;
        mapGen();
        Time.timeScale = timeScale;  //Задержка выполнения действий, 1f = реальному времени 0.5f = в 2X раза медленее обычного времени
    }

    // Передвижение всех кроликов
    void rabbitMove()
    {
        int x, y;       //X и Y координаты кролика 
        int move,plot;  //Рандомные переменные, определяют движение и возможность размножения кролика


        for (int i = 0; i <= numOfRabbits; i++) //Цикл передвижения всех кроликов 
        {
            x = rabbitX[i]; //начальные координаты [i] кролика
            y = rabbitY[i]; //начальные координаты [i] кролика     // 1 2 3
                                                                   // 4 5 6
            move = Random.Range(1, 9);  //путь движения кролика    // 7 8 9
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
            if (rabbitX[i] == 0)     //Если вышел за левую грань   
            {
                rabbitX[i] = 1;   
            }
            if (rabbitX[i] == 21)   //Если вышел за правую грань
            {
                rabbitX[i] = 20;
            }
            if (rabbitY[i] == 0)    //Если вышел за нижнюю грань
            {
                rabbitY[i] = 1;
            }
            if (rabbitY[i] == 21)  //Если вышел за верхнюю грань
            {
                rabbitY[i] = 20;
            }
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

            plot = Random.Range(1, 100);  //Шанс размножения кролика = 20%
            if (plot <= chanceOfDuplication)
            {
                if (numOfRabbits == num) numOfRabbits -= 1; //Установка лимита кроликов (кол-во кроликов не будет превышать 399)
                numOfRabbits += 1;  //Увеличение количества кроликов на 1
                rabbitX[numOfRabbits] = rabbitX[i];  //Координаты нового кролика = координатам родителя
                rabbitY[numOfRabbits] = rabbitY[i];
                cellPos.x = rabbitX[numOfRabbits];   //Изменение тайла на тайл кролика 
                cellPos.y = rabbitY[numOfRabbits];  
                tilemap.SetTile(cellPos, rabbitTile);
            }


            


        }
    }

    // Оставить в живых начального кролика (Update) (Не учтена смерть от волков)
    void killRabits()
    {
         numOfRabbits = 0;
         for(int i = 1; i <= num; i++)
         {
            rabbitX[i] = 0;
            rabbitY[i] = 0;
         }
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
        int move;

        for(int i = 0; i <= numOfWolfs; i++)
        {
            x = wolfX[i];
            y = wolfY[i];


            move = Random.Range(1, 9);  //путь движения волка
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
            if (wolfX[i] == 0)     //Если вышел за левую грань   
            {
                wolfX[i] = 1;
            }
            if (wolfX[i] == 21)   //Если вышел за правую грань
            {
                wolfX[i] = 20;
            }
            if (wolfY[i] == 0)    //Если вышел за нижнюю грань
            {
                wolfY[i] = 1;
            }
            if (wolfY[i] == 21)  //Если вышел за верхнюю грань
            {
                wolfY[i] = 20;
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
        }
        
    }

    void FixedUpdate()
    {
        rabbitMove();                               //Передвижение всех кроликов
        Time.fixedDeltaTime = Time.fixedDeltaTime * Time.timeScale; //Задержка перед следующим действием 
        wolfMove();
        Debug.Log(numOfRabbits+1);
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.F))
        {
            killRabits();
            mapGen();
        }
       
    }
}
