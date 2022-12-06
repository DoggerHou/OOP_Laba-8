using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace OOP_Laba_8
{
    public partial class Form1 : Form
    {
        public Storage myStorage;
        bool controlUp = false;
        Color btn_color = Color.Black;
        bool ellipse = true;
        bool square = false;
        bool triangle = false;
        Point pBox;

        TreeNode tn = new TreeNode();



        public Form1()
        {
            myStorage = new Storage();
            myStorage.addObserver(new TreeObserver());
            InitializeComponent();
        }



        private void pictureBox_Paint(object sender, PaintEventArgs e)//отрисовка объектов
        {
            for (int i = 0; i < myStorage.getStorageSize(); i++)
            {
                myStorage.getObject(i).OnPaint(e);
            }
        }


        private void pictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            this.ActiveControl = null;

            if (ellipse)//Добавление эллипса
                myStorage.addObject(new Circle(e.Location, btn_color, pBox), e, controlUp);

            if (square)//добавление квадрата
                myStorage.addObject(new Square(e.Location, btn_color, pBox), e, controlUp);
            pictureBox.Invalidate();

            if (triangle)//добавление треугольника
                myStorage.addObject(new Triangle(e.Location, btn_color, pBox), e, controlUp);

            treeView1.Nodes.Clear();
            treeView1.Nodes.Add(myStorage.gett());
            pictureBox.Invalidate();
        }


        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Add) //обрабатывает нажатие: Увеличение объектов
                for (int i = 0; i < myStorage.getStorageSize(); i++)
                {
                    if (myStorage.getObject(i).isDetailed())
                        myStorage.getObject(i).IncreaseObjectSize(4);
                }

            if (e.KeyData == Keys.Subtract)//обрабатывает нажатие: Уменьшение объектов
                for (int i = 0; i < myStorage.getStorageSize(); i++)
                {
                    if (myStorage.getObject(i).isDetailed() == true)
                        myStorage.getObject(i).DecreaseObjectSize(4);
                }

            if (e.Control) //обрабатывает нажатие: Выбор объектов
                controlUp = true;

            if (e.KeyData == Keys.Delete)//обрабатывает нажатие: Удаление объектов
            {
                myStorage.DeleteDetailedObjects();
                treeView1.Nodes.Clear();
                treeView1.Nodes.Add(myStorage.gett());
            }
            //Далее идет обработка 4 кнопок движения, ничего интересного и сложного

            if (e.KeyData == Keys.Right)//Движение вправо
            {
                for (int i = 0; i < myStorage.getStorageSize(); i++)
                {
                    if (myStorage.getObject(i).isDetailed() )
                        myStorage.getObject(i).MoveObject(4, 0);
                }
            }
            if (e.KeyData == Keys.Left)//Движение влево
            {
                for (int i = 0; i < myStorage.getStorageSize(); i++)
                {
                    if (myStorage.getObject(i).isDetailed())
                        myStorage.getObject(i).MoveObject(-4, 0);
                }
            }
            if (e.KeyData == Keys.Down)//тут +1, ибо ось Y направлена вниз(а не как в православии, вверх)
            {
                for (int i = 0; i < myStorage.getStorageSize(); i++)
                {
                    if (myStorage.getObject(i).isDetailed())
                        myStorage.getObject(i).MoveObject(0, 4);
                }
            }
            if (e.KeyData == Keys.Up)//тут -1, ибо ось Y направлена вниз(а не как в православии, вверх)
            {
                for (int i = 0; i < myStorage.getStorageSize(); i++)
                {
                    if (myStorage.getObject(i).isDetailed())
                        myStorage.getObject(i).MoveObject(0, -4);
                }
            }
            if (e.Shift)//Группировка
            {
                myStorage.GroupObjects();
                treeView1.Nodes.Clear();
                treeView1.Nodes.Add(myStorage.gett());
            }
            if (e.KeyCode == Keys.A)//Разгруппировка
            {
                treeView1.Nodes.Clear();
                treeView1.Nodes.Add(myStorage.gett());
                myStorage.UngroupObjects();
            }
            if (e.Alt)
            {
                for (int i = 0; i < myStorage.getStorageSize(); i++)
                    if (myStorage.getObject(i).isDetailed())
                    {
                        myStorage.getObject(i).changeSticky_to(true);
                        myStorage.getObject(i).addObserver(new StickyObserver(myStorage));
                    }
            }

            this.KeyPreview = true;
            pictureBox.Invalidate();
        }


        private void Form1_KeyUp(object sender, KeyEventArgs e)//Если кнопка кливиатуры отпущена, значит 
        {                                                      //отпущен и ctrl
            controlUp = false;
        }


        private void button_RED_Click(object sender, EventArgs e)   //Кнопки выбора цвета
        {
            btn_color = ((Button)sender).BackColor;
            for (int i = 0; i < myStorage.getStorageSize(); i++)
            {
                if (myStorage.getObject(i).isDetailed())
                    myStorage.getObject(i).setColor(btn_color);
            }

            pictureBox.Invalidate();
            this.ActiveControl = null;
        }


        private void button_ELLIPSE_Click(object sender, EventArgs e)//Кнопки выбора объекта для отрисовки
        {
            ellipse = false;
            square = false;
            triangle = false;
            button_ELLIPSE.BackColor = Color.DimGray;
            button_SQUARE.BackColor = Color.DimGray;
            button_TRIANGLE.BackColor = Color.DimGray;

            if (sender == button_ELLIPSE)
            {
                ellipse = true;
                button_ELLIPSE.BackColor = Color.Black;
            }

            else if (sender == button_SQUARE)
            {
                square = true;
                button_SQUARE.BackColor = Color.Black;
            }
            else if (sender == button_TRIANGLE)
            {
                triangle = true;
                button_TRIANGLE.BackColor = Color.Black;
            }
            this.ActiveControl = null;
        }


        private void button_ColorDialog_Click(object sender, EventArgs e)//кнопка для ColorDialog
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                btn_color = colorDialog1.Color;
                ((Button)sender).BackColor = colorDialog1.Color;

                for (int i = 0; i < myStorage.getStorageSize(); i++)    //изменяем цвет у всех выбранных объектов
                {
                    if (myStorage.getObject(i).isDetailed())
                        myStorage.getObject(i).setColor(btn_color);
                }
                pictureBox.Invalidate();
            }
            this.ActiveControl = null;
        }

        private void pictureBox_Resize(object sender, EventArgs e)
        {
            pBox.X = pictureBox.Width;
            pBox.Y = pictureBox.Height;
            for (int i = myStorage.getStorageSize() - 1; i >= 0; i--)
            {
                myStorage.getObject(i).setBorders(pBox);
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                StreamWriter save = new StreamWriter(saveFileDialog1.FileName);
                myStorage.SaveStorage(save);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                StreamReader load = new StreamReader(openFileDialog1.FileName);
                MyObjectsFactory factory = new MyObjectsFactory();

                myStorage.LoadStorage(factory, load);
                for (int i = 0; i < myStorage.getStorageSize(); i++)
                    myStorage.getObject(i).setBorders(pBox);
                treeView1.Nodes.Clear();
                treeView1.Nodes.Add(myStorage.gett());
            }
        }


        //Поскольку этот обработчик подписан на все изменения блока catch, он срабатывает при каждом
        //его изменении, в данном случае, если изменяется i блок, срабатывает это событие
        //но т.к. наш TreeNode еще не имеет листьев после i блока, а цикл for идет по всем
        //элементам хранилища, требуется try/catch, дабы выполнится, только если лист уже создан
        private void treeView1_AfterCheck(object sender, TreeViewEventArgs e)
        {
            for(int i = 0; i < myStorage.getStorageSize(); i++)
            {
                if(treeView1.TopNode.LastNode.Index >= i)
                    myStorage.getObject(i).changeDetail_to(treeView1.TopNode.Nodes[i].Checked);
            }
            pictureBox.Invalidate();
        }

        private void treeView1_MouseLeave_1(object sender, EventArgs e)
        {
            this.ActiveControl = null;
            this.KeyPreview = false;
        }
    }
}
