using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MinesweeperCS
{
    enum Minefield
    {
        EMPTY,
        ALL_CLEAR,
        MINE
    }
    struct Position
    {
        public Position(int x, int y)
        {
            X = x;
            Y = y;
        }
        public int X { get; set; }
        public int Y { get; set; }
    }

    struct Velocity
    {
        public Velocity(int x, int y)
        {
            X = x;
            Y = y;
        }
        public int X { get; set; }
        public int Y { get; set; }
    }
    class Program
    {
        static void Main(string[] args)
        {
            Position player = new Position(3, 3);
            string msg="";


            Minefield[,] minefield = new Minefield[27, 27];
            Console.WriteLine("Welcom to Minesweeper 2021\n");
            Console.WriteLine("By Alex Mair");
            Thread.Sleep(1000);
            Console.WriteLine("Choose level: easy - 1 hard - 2");
            int op=0;
            while(op!=1 && op!=2)
            {
                op = Convert.ToInt32(Console.ReadLine());
                if(op==1)
                {
                    LayMines(100, ref minefield);
                }
                if (op==2)
                {
                    LayMines(200, ref minefield);
                }
            }

            
            //showMines(player.X, player.Y, ref minefield);
            while (true)
            {
                
                ShowGame(ref minefield, ref player);
                Console.Write(msg);
                
                
                if(!SetPlayerPosition(ref player, ref minefield, ref msg))
                    break;
            }
            Console.WriteLine("Game Over");
            
        }
        static bool SetPlayerPosition(ref Position player, ref Minefield[,] minefield, ref string msg)
        {
            Console.WriteLine("Enter coordinates");
            string coordinates = Console.ReadLine();
            if (coordinates.Length==2 && Char.IsLetter(coordinates[coordinates.Length-1])
                && Char.IsLetter(coordinates[0]))
            {
                player.Y = Convert.ToInt32(coordinates[coordinates.Length - 1])-96;
                player.X= Convert.ToInt32(coordinates[0]) - 96;
                if (minefield[player.X, player.Y] == Minefield.MINE)
                {
                    Console.Clear();
                    ShowMines(player.X, player.Y, ref minefield);
                    return false;
                }
                else
                {
                    CheckIfClear(player.X, player.Y, ref minefield);
                }
            }
            else if(coordinates.Length<2)
            {
                msg = "input must be at least two letters\n";
            }
            else if(coordinates.Length>2)
            {
                msg="You've typed too many characters\n";
            }
            else if(!Char.IsLetter(coordinates[0]) || !Char.IsLetter(coordinates[1]))
            {
                msg = "Coordinates must consist of two letters\n";
            }
            Console.Clear();
            return true;
        }
        static void ShowGame(ref Minefield[,] minefield, ref Position player)
        {

            for (int i = 0; i < minefield.GetLength(0); i++)
            {
                for (int j = 0; j < minefield.GetLength(1); j++)
                {
                    if (j == 0 && i == 0)
                    {
                        Console.Write("[O]");
                    }

                    else if (j == 0 && i > 0 && i < 28)
                    {
                        
                            Console.Write(" "+Convert.ToChar(i + 96)+" ");                     
                        
                    }

                    else if (i == 0 && j > 0 && j < 28)
                    {
                        Console.Write(" "+Convert.ToChar(j + 96)+" ");
                    }

                    else if (i==player.Y && j==player.X)
                    {
                        Console.Write(">-0");
                    }
                    else if (Neighborhood(i, j, ref minefield) > 0 && minefield[i,j] == Minefield.ALL_CLEAR)
                    {
                        Console.Write(" " + Neighborhood(i, j, ref minefield)+ " ");
                    }
                    else if(minefield[i,j]==Minefield.ALL_CLEAR)
                    {
                        Console.Write("   ");
                    }
                    else
                    {
                        Console.Write("[ ]");
                    }

                }
                Console.WriteLine();
            }
        }
        static void ShowMines(int x, int y, ref Minefield[,] minefield)
        {
            for (int i = 0; i < minefield.GetLength(0); i++)
            {
                for (int j = 0; j < minefield.GetLength(1); j++)
                {
                    if (minefield[i , j] == Minefield.MINE && x == j && y == i)
                    {
                        Console.Write( "\\|/");
                    }
                    else if (minefield[i, j] == Minefield.MINE)
                    {
                        Console.Write( "(X)");
                    }
                    else
                    {
                        Console.Write( "[ ]");
                    }
                }
                Console.WriteLine();
            }
        }
        static void CheckIfClear(int x, int y, ref Minefield[,] minefield)
        {

            int n = 1;
            bool mine_nearby = false;
            while (/*x + n < minefield.GetLength(1)
                && x - n > 0
                && y + n < minefield.GetLength(0)
                && y - n > 0
                &&*/ !mine_nearby)
            {
                for (int i = y - n; i < y + n; i++)
                {
                    for (int j = x - n; j <= x + n; j++)
                    {
                        if (((i >= y - n || i <= y + n) && (j == x + n || j == x - n)
                            || (j <= x + n || j >= x - n) && (i == y - n || i == y + n))
                            && j>0 && j<minefield.GetLength(0)
                            && i>0 && i<minefield.GetLength(0))
                        {
                            if(minefield[i, j] == Minefield.MINE)
                            {
                                mine_nearby = true;
                            }
                            else
                            {
                                minefield[i, j] = Minefield.ALL_CLEAR;
                            }

                        }
                        
                    }
                }
                n++;
            }


        }
        
        static Velocity Gravity(Velocity v)
        {
            v.X -= 2;
            v.Y += 2;
            return v;
        }

        static void LayMines(int iterations, ref Minefield [,] minefield)
        {
            Position p = new Position(0, 20);
            Velocity v = new Velocity(-5, 2);
            
            for (int i = 0; i < iterations; i++)
            {
                p=Inertia(p, v, ref minefield);
                v=Gravity(v);
                Seeder(ref minefield, p);

            }
        }
        static Position Inertia(Position p, Velocity v, ref Minefield[,] minefield)
        {
            if (p.X + v.X <= 0 || p.X + v.X > 27)
            {
                v.X = -v.X;
            }

            if (p.Y + v.Y <= 0 || p.Y + v.Y > 27)
            {
                v.Y = -v.Y;
            }

            p.X += v.X;
            p.Y += v.Y;
            return p;
        }

        static void Seeder(ref Minefield[,] minefield, Position p)
        {
            for (int i = 0; i < minefield.GetLength(0); i++)
            {
                for (int j = 0; j < minefield.GetLength(1); j++)
                {
                    if (i == p.X && j == p.Y)
                    {
                        minefield[i, j] = Minefield.MINE;
                    }
                }
            }
        }
        
        static int Neighborhood(int i, int j, ref Minefield[,] minefield)
        {
            int neighbors = 0;

            if (i > 0 && i < minefield.GetLength(0) - 1 && j > 0 && j < minefield.GetLength(1) - 1)
            {
                for (int k = i - 1; k <= i + 1; k++)
                {
                    for (int l = j - 1; l <= j + 1; l++)
                    {
                        if (minefield[k, l] == Minefield.MINE && !(k == i && l == j))
                        {

                            neighbors++;
                        }
                    }
                }
            }
            return neighbors;
        }


    }
}
