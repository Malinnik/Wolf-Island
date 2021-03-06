using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class setTile : MonoBehaviour
{
    public Tile groundTile;         //Тайл острова 
    public Tile rabbit;             //Тайл Кролика
    public Tilemap tilemap;         //Карта тайлов (из неё берётся тайлы для игры)
    int[] rabbitX = new int[400];   //Массив X координат всех кроликов (0;399)
    int[] rabbitY = new int[400];   //Массив Y координат всех кроликов (0;399)
    int numOfRabbits = 0;           //Кол-во кроликов (0;399)
    Vector3Int cellPos;             //Позиция текущей клетки (курсора установки или удаления тайлов)



    void Start()
    {   
        rabbitX[0] = 10;        //Создания начального кролика в координате 10 на 10
        rabbitY[0] = 10;
        mapGen();
        Time.timeScale = 0.001f;  //Задержка выполнения действий, 1f = реальному времени 0.5f = в 2X раза медленее обычного времени
    }

    void rabbitMove(int[] rabX,int[] rabY)
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
            tilemap.SetTile(cellPos, rabbit);  //Замена тайла земли на новую позицию кролика


            //Размножение кроликов//Отключено на время создания и теста волков

            /*
            plot = Random.Range(1, 100);  //Шанс размножения кролика = 20%
            if (plot <= 20)
            {
                if (numOfRabbits == 399) numOfRabbits -= 1; //Установка лимита кроликов (кол-во кроликов не будет превышать 399)
                numOfRabbits += 1;  //Увеличение количества кроликов на 1
                rabbitX[numOfRabbits] = rabbitX[i];  //Координаты нового кролика = координатам родителя
                rabbitY[numOfRabbits] = rabbitY[i];
                cellPos.x = rabbitX[numOfRabbits];   //Изменение тайла на тайл кролика 
                cellPos.y = rabbitY[numOfRabbits];  
                tilemap.SetTile(cellPos, rabbit);
            }


            */


        }
    }
    // Генерация карты
    void mapGen()
    {
        cellPos = tilemap.WorldToCell(transform.position);  //Нахождения стартовой клетки
        cellPos.x = 1;  
        cellPos.y = 1;
        Debug.Log(cellPos);
        
        for (int y = 1; y<=21; y++)  //Создания поля 20 на 20
        {
            for (int x = 1; x <= 20; x++)
            {
                tilemap.SetTile(cellPos, groundTile);
                cellPos.x = x;
            }
            cellPos.y = y;
            Debug.Log(cellPos.x + "  " + cellPos.y);
        }


        //координаты левого нижнего угла (1;1)

        //По не понятной мне причине, Y останавливается не доходя до последнего числа в цикле ( <= 21, останавливается на 20), поэтому Y <= 21


        //                      Y ^
        //Система координат XY    | - >
        //                            X
    }

    void FixedUpdate()
    {
        rabbitMove(rabbitX, rabbitY);                               //Передвижение всех кроликов
        Time.fixedDeltaTime = Time.fixedDeltaTime * Time.timeScale; //Задержка перед следующим действием

    }
}
