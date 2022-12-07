﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;
using System.IO;

namespace OOP_Laba_8
{
    static class Program 
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }



    public abstract class Model : Observerable
    {
        protected int RADIX; //Радиус окружности/Вписанной в квадрат окружности/Длина стороны треугольника
        protected Point location; //Точка центра объекта
        protected bool detail;    //Маркер выделенности
        protected Color object_color;//Цвет объекта
        protected Point pictureBox_Border; //Границы формы для рисования
        protected bool sticky;


        public virtual void changeDetail_to(bool d) { detail = d; } //Снимает выделение c объекта


        public virtual void changeSticky_to(bool d) { sticky = d; }

        public virtual bool isDetailed() { return detail; } //Достает информацию о Выделенности объекта


        public virtual void OnPaint(PaintEventArgs e) { }//Отрисовка


        public virtual bool isPicked(MouseEventArgs e, bool controlUp) { return false; } //Попали ли мы в объект


        public virtual void setColor(Color color) //Установить цвет объекта
        {
            this.object_color = color;
        }

        //установить границы формы
        public virtual void setBorders(Point pbox_border)
        {
            pictureBox_Border.X = pbox_border.X;
            pictureBox_Border.Y = pbox_border.Y;
        }


        //Можем ли сдвинуть объект
        public virtual bool isMovable(int point_X, int point_Y) 
        {
            if ((location.X + point_X - RADIX >= 0) & (location.X + point_X + RADIX <= pictureBox_Border.X) &
                (location.Y + point_Y - RADIX >= 0) & (location.Y + point_Y + RADIX <= pictureBox_Border.Y))
                return true;
            return false;
        }


        //можем ли увеличить размер 
        public virtual bool isIncreasable(int _radix) 
        {
            if (((location.X - _radix - RADIX >= 0) & (location.X + _radix + RADIX <= pictureBox_Border.X) &
                (location.Y - _radix - RADIX >= 0) & (location.Y + _radix + RADIX <= pictureBox_Border.Y)))
                return true;
            return false;
        }

        //Увеличить размер
        public virtual void IncreaseObjectSize(int _radix)
        {
            if (isIncreasable(_radix))
            {
                RADIX += _radix;
            }
        }


        //Уменьшить размер
        public virtual void DecreaseObjectSize(int _radix)
        {
            if (RADIX >= _radix)
                RADIX -= _radix;
        }


        //Изменить местоположение объекта
        public virtual void MoveObject(int _X, int _Y) 
        {
            if(sticky)
                notifyObjects( _X, _Y);

            if (isMovable(_X, _Y))
            {
                location.X += _X;
                location.Y += _Y;
            }
        }


        //Получить границы объекта
        public virtual (int, int, int, int) getGroupBoards()
        {
            var tuple = (location.X - RADIX, location.X + RADIX, location.Y - RADIX, location.Y + RADIX);
            return tuple;
        }



        public virtual void SaveObject(StreamWriter save) 
        {
            save.Write(location.X.ToString() + " ");
            save.Write(location.Y.ToString() + " ");
            save.Write(RADIX.ToString() + " ");
            save.Write(object_color.ToArgb() + " ");
            save.WriteLine(detail.ToString());
        }


        public virtual void LoadObject(StreamReader load, ModelFactory factory) 
        {
            string[] s = load.ReadLine().Split(' ');
            location.X = int.Parse(s[0]);
            location.Y = int.Parse(s[1]);
            RADIX = int.Parse(s[2]);
            object_color = Color.FromArgb(int.Parse(s[3]));
            detail = bool.Parse(s[4]);
        }


        public virtual bool isSticked(Model o)
        {
            if (Math.Sqrt(Math.Pow(o.location.X - location.X, 2) +
                Math.Pow(o.location.Y - location.Y, 2)) <= o.RADIX + RADIX)
                return true;
            return false;
        }
    }



    public class Circle : Model
    {
        public Circle() { }


        public Circle(Point location, Color color, Point pb)
        {
            RADIX = 40;
            this.location = location;
            this.pictureBox_Border = pb;
            detail = true;
            object_color = color;
        }

        public override void OnPaint(PaintEventArgs e)//Отрисовка Эллипса
        {
            Pen pen = new Pen(object_color);
            if (sticky)
                pen.Width = 15;
            else if (detail)
                pen.Width = 7;
            else
                pen.Width = 4;

            e.Graphics.DrawEllipse(pen, location.X - RADIX, location.Y - RADIX, RADIX * 2, RADIX * 2);
        }


        public override bool isPicked(MouseEventArgs e, bool controlUp)//попали ли мы в объект
        {
            if (Math.Pow(location.X - e.X, 2) + Math.Pow(location.Y - e.Y, 2) <= Math.Pow(RADIX, 2)
                & controlUp)
            {
                detail = !detail; //Инвертируем выделенность
                return true;
            }
            return false;//не попал в круг - ниче не делает
        }


        //Такой вот типа Template method
        public override void SaveObject(StreamWriter save)
        {
            save.WriteLine("Circle");
            base.SaveObject(save);
        }
    }
    


    public class Square : Model
    {
        public Square() { }


        public Square(Point location, Color color, Point pb)
        {
            RADIX = 40;                 // RADIX - Радиус вписанного в квадрат круга
            this.location = location;   //(или половина стороны квадрата)
            this.pictureBox_Border = pb;
            detail = true;
            object_color = color;
        }


        public override void OnPaint(PaintEventArgs e)//Отрисовка Квадрата
        {
            Pen pen = new Pen(object_color);
            if (sticky)
                pen.Width = 15;
            else if (detail)
                pen.Width = 7;
            else
                pen.Width = 4;

            e.Graphics.DrawRectangle(pen, location.X - RADIX, location.Y - RADIX, RADIX * 2, RADIX * 2);
        }


        public override bool isPicked(MouseEventArgs e, bool controlUp)//попали ли мы в объект
        {
            if ((e.X <= location.X + RADIX) & (e.X >= location.X - RADIX) &
                (e.Y <= location.Y + RADIX) & (e.Y >= location.Y - RADIX) &
                controlUp)
            {
                detail = !detail; //Инвертируем выделенность
                return true;
            }
            return false;
        }


        public override void SaveObject(StreamWriter save)
        {
            save.WriteLine("Square");
            base.SaveObject(save);
        }
    }



    public class Triangle : Model
    {
        private Point A; //Координаты Трех точек треугольника
        private Point B;
        private Point C;



        public Triangle() { }

        int _RADIX;
        public Triangle(Point location, Color color, Point pb)
        {
            RADIX = 50; //Радиус описанной вокруг треугольника окружности
            int _RADIX;//Радиус описанной окружности
            this.location = location;
            this.pictureBox_Border = pb;
            detail = true;
            object_color = color;

            refreshTriangle();
        }


        //Возвращает площадь треугольника
        private double getTriangle_Square(double aX, double aY, double bX, double bY, double cX, double cY)
        {
            return Math.Abs(bX * cY - cX * bY - aX * cY + cX * aY + aX * bY - bX * aY);
        }


        //обновляем координаты точек треугольника после каких-то изменений длины стороны
        private void refreshTriangle() 
        {
            int a = (int)(3 * RADIX / Math.Sqrt(3));
            _RADIX = (int)(Math.Sqrt(3) * a / 6);

            A.X = location.X - a / 2;
            A.Y = location.Y + _RADIX;

            B.X = location.X + a / 2;
            B.Y = location.Y + _RADIX;

            C.X = location.X;
            C.Y = location.Y - RADIX;
        }


        public override void OnPaint(PaintEventArgs e)//состоит из отрисовки трех линий
        {
            Pen pen = new Pen(object_color);
            pen.StartCap = System.Drawing.Drawing2D.LineCap.Round; //Чтобы линии были более закругленными

            if (sticky)
                pen.Width = 15;
            else if (detail)
                pen.Width = 7;
            else
                pen.Width = 4;

            e.Graphics.DrawLine(pen, A.X, A.Y, B.X, B.Y);
            e.Graphics.DrawLine(pen, A.X, A.Y, C.X, C.Y);
            e.Graphics.DrawLine(pen, B.X, B.Y, C.X, C.Y);

        }


        public override bool isPicked(MouseEventArgs e, bool controlUp)//по методу сравнения площадей
        {
            if ((getTriangle_Square(A.X, A.Y, B.X, B.Y, e.X, e.Y) + 
                getTriangle_Square(A.X, A.Y, e.X, e.Y, C.X, C.Y) +
                getTriangle_Square(e.X, e.Y, B.X, B.Y, C.X, C.Y) - 
                getTriangle_Square(A.X, A.Y, B.X, B.Y, C.X, C.Y) <= 0.01) &
                controlUp)
            {
                detail = !detail; //Инвертируем выделенность
                return true;
            }
            return false;
        }


        //Проверяет, вышел ли треугольник за границы при движении
        public override bool isMovable(int point_aX, int point_cY) //к примеру, point_aX - координата 
        {                                                          //точки А по оси Х
            if ((A.X + point_aX >= 0) & (B.X + point_aX  <= pictureBox_Border.X) &
                (C.Y + point_cY >= 0) & (A.Y + point_cY <= pictureBox_Border.Y))
                return true;
            return false;
        }

        //Проверяет, вышел ли треугольник за границы при Увеличении
        public override bool isIncreasable(int _radix)
        {
            int a = (int)(3 * (RADIX +_radix)/ Math.Sqrt(3));
            _RADIX = (int)(Math.Sqrt(3) * a / 6);

            int Ax = location.X - a / 2;
            int Ay = location.Y + _RADIX;
            int Bx = location.X + a / 2;
            int Cy = location.Y - (RADIX + _radix);


            if ((Ax >= 0) & 
                (Bx <= pictureBox_Border.X) &
                (Cy >= 0) & 
                (Ay <= pictureBox_Border.Y))
                return true;
            return false;
        }


        public override void IncreaseObjectSize(int _radix)
        {
            if (isIncreasable(_radix))
            {
                RADIX += _radix;
                refreshTriangle();
            }
        }


        public override void DecreaseObjectSize(int _radix)
        {
            if (RADIX >= _radix)
                RADIX -= _radix;
            else
                RADIX = 0;
            refreshTriangle();
        }


        public override void MoveObject(int _X, int _Y)
        {
            if (sticky)
                notifyObjects(_X, _Y);

            if (isMovable(_X, _Y))
            {
                location.X += _X;
                location.Y += _Y;
                refreshTriangle();
            }
        }


        public override (int, int, int, int) getGroupBoards()
        {
            var tuple = (A.X, B.X, C.Y, A.Y);
            return tuple;
        }


        public override void SaveObject(StreamWriter save)
        {
            save.WriteLine("Triangle");
            base.SaveObject(save);
        }

        public override void LoadObject(StreamReader load, ModelFactory factory)
        {
            base.LoadObject(load, factory);
            refreshTriangle();
        }
    }



    public class Storage : Observerable
    {
        private List<Model> objects;

        public Storage()
        {
            objects = new List<Model>();
        }


        //возвращает размер Списка
        public int getStorageSize() 
        { 
            return objects.Count(); 
        }


        //возвращает объект Model
        public Model getObject(int index) 
        { 
            return objects[index];
        }


        //добавляет объект
        public void addObject(Model temp_object, MouseEventArgs e, bool controlUp)
        {
            for (int i = 0; i < objects.Count(); i++) //Если попали в объект, то выходим
                if (objects[i].isPicked(e, controlUp)) //(чтобы не рисовать новый)
                {
                    notifyTree();
                    return;
                }

            objects.Add(temp_object); //если не попали, добавляем новый объект
            for (int i = 0; i < objects.Count() - 1; i++)//снимаем выделение со всех предыдущих
                if(!(objects[i] is Group))
                    objects[i].changeDetail_to(false);

            notifyTree();
        }


        //удаляет все "помеченные" объекты
        public void DeleteDetailedObjects()
        {
            for (int i = objects.Count() - 1; i >= 0; i--)
                if (objects[i].isDetailed() )
                    objects.RemoveAt(i);
            notifyTree();
        }


        //Группирует объекты
        public void GroupObjects()
        {
            Group g = new Group();
            for (int i = objects.Count - 1; i >= 0; i--)
            {
                if (objects[i].isDetailed())
                {
                    g.add_in_Group(objects[i]);
                    objects.RemoveAt(i);
                }
            }
            if (g.getGroup().Count < 2)
                objects.AddRange(g.getGroup());
            else
                objects.Add(g);

            notifyTree();
        }


        //Разгруппировывает объекты
        public void UngroupObjects()
        {
            for(int i = objects.Count-1; i >= 0; i--)
            {
                if(objects[i] is Group & objects[i].isDetailed())
                {
                    objects.AddRange(((Group)objects[i]).getGroup());
                    objects.RemoveAt(i);
                }
            }
            notifyTree();
        }


        public void SaveStorage(StreamWriter save)
        {
            //StreamWriter save = new StreamWriter("Storage.txt", false);
            save.WriteLine(objects.Count.ToString());
            foreach (var obj in objects)
            {
                obj.SaveObject(save);
                save.Flush();
            }
            save.Close();
        }


        public void LoadStorage(ModelFactory factory, StreamReader load)
        {
            Model tempModel;
            int count;

            try
            {
                count = int.Parse(load.ReadLine());
            }
            catch
            {
                load.Close();
                return;
            }

            for(int i = 0; i < count; i++)
            {
                tempModel = factory.CreateObject(load.ReadLine());
                if (tempModel != null)
                {
                    tempModel.LoadObject(load, factory);
                    objects.Add(tempModel);
                }
            }
            load.Close();
            notifyTree();
        }

    }
}
