using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Acoross.Game2d;

namespace TestWinform
{
    public partial class Form1 : Form
    {
        class MyObject : Acoross.Game2d.GameObject
        {
            public CircleCollider collider { get; }

            public MyObject(Vector2d position, float radius)
                : base(position, Vector2d.X)
            {
                this.radius = radius;
                collider = new CircleCollider(this, 0, 0, radius);
            }

            public override void OnColliding(Collider collider)
            {
                isColliding = true;
            }

            public void draw(Graphics g)
            {
                Pen circlePen = Pens.Black;
                if (isColliding)
                {
                    circlePen = Pens.Red;
                    isColliding = false;
                }

                var x = this.position.x - radius;
                var y = this.position.y - radius;
                var width = radius * 2;
                var height = radius * 2;

                var dirEnd = position + rotation * radius;

                g.DrawArc(circlePen, x, y, width, height, 0, 360);
                g.DrawLine(new Pen(Color.Red), position.x, position.y, dirEnd.x, dirEnd.y);
            }

            public bool isColliding { get; private set; } = false;
            public float radius { get; }
        }

        HashSet<MyObject> objectSet = new HashSet<MyObject>();
        MyObject player;
        ColliderWorld colworld = new ColliderWorld();

        public Form1()
        {
            InitializeComponent();
            
            var obj = new MyObject(new Vector2d(100, 100), 10);
            player = obj;
            objectSet.Add(obj);

            colworld.Add(obj.collider);
            colworld.Add(new LineCollider(1, 0, 0));
            colworld.Add(new LineCollider(-1, 0, -200));
            colworld.Add(new LineCollider(0, 1, 0));
            colworld.Add(new LineCollider(0, -1, -200));

            timer1.Interval = 33;
            timer1.Start();
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Up:
                    player.Move(Vector2d.Y * -10);
                    break;
                case Keys.Down:
                    player.Move(Vector2d.Y * 10);
                    break;
                case Keys.Right:
                    player.Move(Vector2d.X * 10);
                    break;
                case Keys.Left:
                    player.Move(Vector2d.X * -10);
                    break;
                default:
                    break;
            }
        }

        void update()
        {
            pictureBox1.Image?.Dispose();
            var bmp = new Bitmap(pictureBox1.Size.Width, pictureBox1.Size.Height);
            using (var graphics = Graphics.FromImage(bmp))
            {
                graphics.DrawRectangle(Pens.Black, 0, 0, 200, 200);

                foreach (var o in objectSet)
                {
                    o.Rotate((float)Math.PI / 12.0f);
                    o.draw(graphics);
                }
            }

            pictureBox1.Image = bmp;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            colworld.CheckCollision();
            colworld.ResolveCollisions();
            update();
        }
    }
}
