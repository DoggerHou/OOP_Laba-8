﻿namespace OOP_Laba_8
{
    public abstract class ModelFactory //Абстрактная фабрика
    {
        public virtual Model CreateObject(string code) {return null;}
    }



    public class MyObjectsFactory : ModelFactory
    {
        public override Model CreateObject(string code)
        {
            Model temp = null;
            switch (code)
            {
                case "Circle":
                    temp = new Circle();
                    break;
                case "Square":
                    temp = new Square();
                    break;
                case "Triangle":
                    temp = new Triangle();
                    break;
                case "Group":
                    temp = new Group();
                    break;
            }
            return temp;
        }
    }
}

   