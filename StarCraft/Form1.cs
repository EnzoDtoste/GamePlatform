using GamePlatform;
using StarCraftPlatform;

namespace StarCraft
{
    public partial class Form1 : Form
    {

        Field<Image> field;

        public Form1()
        {

            InitializeComponent();

            Element<Image>[,] elements = new Element<Image>[100, 100];

            elements[3, 5] = new Trash("Bush", StarCraft.Properties.Resources.Bush);
            elements[8, 3] = new Trash("Rock", StarCraft.Properties.Resources.Rock);
            elements[24, 68] = new Trash("Rock1", StarCraft.Properties.Resources.Roca1);
            elements[18, 30] = new Trash("Rock1", StarCraft.Properties.Resources.Roca2);
            elements[70, 80] = new Trash("Rock1", StarCraft.Properties.Resources.Red_Bush);
            elements[24, 71] = new Trash("Rock1", StarCraft.Properties.Resources.Bush1);

            Image sword = StarCraft.Properties.Resources.espada;
            sword.RotateFlip(RotateFlipType.RotateNoneFlipX);

            Weapon<Image> sunWand = new Weapon<Image>("Sun Wand", 27, 8, 15, StarCraft.Properties.Resources.Vara, PrintObject, FireBallAttack);
            Weapon<Image> goldWand = new Weapon<Image>("Gold Wand", 25, 7, 13, StarCraft.Properties.Resources.Wand, PrintObject, ClassicAttack);
            Weapon<Image> infernalAxe = new Weapon<Image>("Infernal Axe", 35, 2, 10, StarCraft.Properties.Resources.Hacha, PrintObject, ClassicAttack);
            Weapon<Image> mediumAxe = new Weapon<Image>("Medium Axe", 30, 4, 7, StarCraft.Properties.Resources.Axe, PrintObject, FireBallAttack);
            Weapon<Image> redSword = new Weapon<Image>("Red Sword", 29, 5, 8, sword, PrintObject, ClassicAttack);
            Weapon<Image> silverSword = new Weapon<Image>("Silver Sword", 25, 5, 8, StarCraft.Properties.Resources.sword, PrintObject, ClassicAttack);

            LifePotion<Image> blue = new LifePotion<Image>("Blue Potion", 50, StarCraft.Properties.Resources.blue_potion, PrintObject);
            LifePotion<Image> green = new LifePotion<Image>("Green Potion", -75, StarCraft.Properties.Resources.green_potion, PrintObject);
            EnergyPotion<Image> yellow = new EnergyPotion<Image>("Yellow Potion", 30, StarCraft.Properties.Resources.yellow_potion, PrintObject);
            DoublePotion < Image > doublepo = new DoublePotion<Image>("Double Potion", 25, StarCraft.Properties.Resources.double_potion, PrintObject);

            elements[37, 45] = blue;
            elements[13, 15] = green;
            elements[38, 5] = yellow;
            elements[76, 5] = doublepo;
            elements[41, 28] = mediumAxe;
            elements[6, 2] = silverSword;
            elements[86, 89] = redSword;
            elements[50, 50] = infernalAxe;
            elements[24, 15] = goldWand;
            elements[3, 10] = sunWand;

            List<StarPlayer<Image>> starPlayers = new List<StarPlayer<Image>>();

            starPlayers.Add(new StarPlayer<Image>("Witch", new Article<Image>[5] {sunWand, goldWand, blue, doublepo, redSword}, 100, 150, StarCraft.Properties.Resources.witch));
            starPlayers.Add(new StarPlayer<Image>("Lava Monster", new Article<Image>[3] { infernalAxe, green, yellow}, 200, 100, StarCraft.Properties.Resources.lava_monster));
            starPlayers.Add(new StarPlayer<Image>("Rock Monster", new Article<Image>[4] { mediumAxe, blue, green, null }, 110, 100, StarCraft.Properties.Resources.monster_rock));
            starPlayers.Add(new StarPlayer<Image>("Blastoise", new Article<Image>[5] {silverSword, blue, doublepo, green, yellow}, 50, 200, StarCraft.Properties.Resources.blastoise));

            field = new Field<Image>(elements, starPlayers);

            pictureBox1.Image = field.Print(new PrintGame(), StarCraft.Properties.Resources.fondo_tierra_clara, new DimObject(100, 100, true));

        }

        public Image PrintObject(Image i, string name, string description, PrintParameters pp)
        {

            DimObject dimo = (DimObject)pp;

            Bitmap b = new Bitmap(dimo.Width, dimo.Height);

            Graphics g = Graphics.FromImage(b);
            g.DrawImage(i, 0, 0, dimo.Width, dimo.Height);

            if (dimo.ShowData)
            {
                g.DrawString(name, new Font("Arial", 8), Brushes.Black, 2, 2);
                g.DrawString(description, new Font("Arial", 8), Brushes.Black, 2, dimo.Height - 20);
            }

            return b;

        }

        void ClassicAttack(int ratio, PrintParameters pp)
        {

            Location l = (Location)pp;

            Image old = field.Print(new PrintGame(), StarCraft.Properties.Resources.fondo_tierra_clara, new DimObject(100, 100, true));

            System.Windows.Forms.Timer t = new System.Windows.Forms.Timer();

            int count = 0;

            t.Interval = 150 / ratio;

            t.Tick += delegate
            {

                if (count < ratio)
                {

                    Image e = new Bitmap(old);

                    Graphics g = Graphics.FromImage(e);

                    g.DrawEllipse(new Pen(Brushes.Red), l.x - count * 25, l.y - count * 25, count * 50, count * 50);

                    pictureBox1.Image = e;
                    pictureBox1.Refresh();

                    count++;

                }

                else
                { t.Stop(); pictureBox1.Image = old; }

            };

            t.Start();

        }

        void FireBallAttack(int ratio, PrintParameters pp)
        {

            Location l = (Location)pp;

            System.Windows.Forms.Timer t = new System.Windows.Forms.Timer();

            List<(int, int, int)> bolas = new List<(int, int, int)>();

            int count = 0;

            t.Interval = 100;

            t.Tick += delegate
            {

                Image e = field.Print(new PrintGame(), StarCraft.Properties.Resources.fondo_tierra_clara, new DimObject(100, 100, true));

                Graphics g = Graphics.FromImage(e);

                Random r = new Random();

                if (count < ratio * 2)
                {
                    bolas.Add((l.x - r.Next(-1 * ratio * 25, ratio * 25), l.y - r.Next(-1 * ratio * 25, ratio * 25), 0));
                    count++;
                }

                foreach (var posBola in bolas)
                    g.DrawImage(StarCraft.Properties.Resources.fire_ball, posBola.Item1, posBola.Item2, 40, 60);

                for (int i = 0; i < bolas.Count; i++)
                {

                    if (bolas[i].Item3 < 3)
                        bolas[i] = (bolas[i].Item1, bolas[i].Item2 + 5, bolas[i].Item3 + 1);
                    else
                    { bolas.RemoveAt(i); i--; }

                }

                pictureBox1.Image = e;
                pictureBox1.Refresh();

                if (bolas.Count == 0)
                { t.Stop(); pictureBox1.Image = field.Print(new PrintGame(), StarCraft.Properties.Resources.fondo_tierra_clara, new DimObject(100, 100, true)); }

            };

            t.Start();

        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {

            switch(e.KeyCode)
            {

                case Keys.A:
                    field.Attack(new Location(375, 250));
                    pictureBox1.Image = field.Print(new PrintGame(), StarCraft.Properties.Resources.fondo_tierra_clara, new DimObject(100, 100, true));
                    pictureBox1.Refresh();
                    break;

                case Keys.Down:
                    field.MovePlayer((1, 0), field.ActualPlayer);
                    pictureBox1.Image = field.Print(new PrintGame(), StarCraft.Properties.Resources.fondo_tierra_clara, new DimObject(100, 100, true));
                    pictureBox1.Refresh();
                    break;

                case Keys.Up:
                    field.MovePlayer((-1, 0), field.ActualPlayer);
                    pictureBox1.Image = field.Print(new PrintGame(), StarCraft.Properties.Resources.fondo_tierra_clara, new DimObject(100, 100, true));
                    pictureBox1.Refresh();
                    break;

                case Keys.Left:
                    field.MovePlayer((0, -1), field.ActualPlayer);
                    pictureBox1.Image = field.Print(new PrintGame(), StarCraft.Properties.Resources.fondo_tierra_clara, new DimObject(100, 100, true));
                    pictureBox1.Refresh();
                    break;

                case Keys.Right:
                    field.MovePlayer((0, 1), field.ActualPlayer);
                    pictureBox1.Image = field.Print(new PrintGame(), StarCraft.Properties.Resources.fondo_tierra_clara, new DimObject(100, 100, true));
                    pictureBox1.Refresh();
                    break;

                case Keys.P:
                    field.NextPlayer();
                    pictureBox1.Image = field.Print(new PrintGame(), StarCraft.Properties.Resources.fondo_tierra_clara, new DimObject(100, 100, true));
                    pictureBox1.Refresh();

                    break;

                case Keys.NumPad1:
                    field.Players[field.ActualPlayer].ApplyArticle(0);
                    pictureBox1.Image = field.Print(new PrintGame(), StarCraft.Properties.Resources.fondo_tierra_clara, new DimObject(100, 100, true));
                    pictureBox1.Refresh();
                    break;
                case Keys.NumPad2:
                    field.Players[field.ActualPlayer].ApplyArticle(1);
                    pictureBox1.Image = field.Print(new PrintGame(), StarCraft.Properties.Resources.fondo_tierra_clara, new DimObject(100, 100, true));
                    pictureBox1.Refresh();
                    break;
                case Keys.NumPad3:
                    field.Players[field.ActualPlayer].ApplyArticle(2);
                    pictureBox1.Image = field.Print(new PrintGame(), StarCraft.Properties.Resources.fondo_tierra_clara, new DimObject(100, 100, true));
                    pictureBox1.Refresh();
                    break;
                case Keys.NumPad4:
                    field.Players[field.ActualPlayer].ApplyArticle(3);
                    pictureBox1.Image = field.Print(new PrintGame(), StarCraft.Properties.Resources.fondo_tierra_clara, new DimObject(100, 100, true));
                    pictureBox1.Refresh();
                    break;
                case Keys.NumPad5:
                    field.Players[field.ActualPlayer].ApplyArticle(4);
                    pictureBox1.Image = field.Print(new PrintGame(), StarCraft.Properties.Resources.fondo_tierra_clara, new DimObject(100, 100, true));
                    pictureBox1.Refresh();
                    break;

            }

        }
    }

    public class PrintGame : IFieldPrint<Image>
    {
        public Image Print(in Environment<Element<Image>[,], StarPlayer<Image>, Article<Image>[], Image> environment, PrintParameters pp)
        {

            Field<Image> field = (Field<Image>)environment;

            if (field.Players[field.ActualPlayer].IsDead)
                field.NextPlayer();

            Bitmap b = new Bitmap(750, 500);

            Graphics g = Graphics.FromImage(b);

            for (int i = 0; i <= 20; i++)
            {

                if (field.PlayersPositions[field.ActualPlayer].Item1 - (10 - i) >= 0 && field.PlayersPositions[field.ActualPlayer].Item1 - (10 - i) < field.Collection.GetLength(0))
                {

                    for (int j = 0; j <= 30; j++)
                    {

                        if (field.PlayersPositions[field.ActualPlayer].Item2 - (15 - j) >= 0 && field.PlayersPositions[field.ActualPlayer].Item2 - (15 - j) < field.Collection.GetLength(1))
                        {

                            if (field.Collection[field.PlayersPositions[field.ActualPlayer].Item1 - (10 - i), field.PlayersPositions[field.ActualPlayer].Item2 - (15 - j)] != null)
                                g.DrawImage(field.Collection[field.PlayersPositions[field.ActualPlayer].Item1 - (10 - i), field.PlayersPositions[field.ActualPlayer].Item2 - (15 - j)].Print(pp), j * 25, i * 25);

                            if (field.PlayersPositions.Contains((field.PlayersPositions[field.ActualPlayer].Item1 - (10 - i), field.PlayersPositions[field.ActualPlayer].Item2 - (15 - j))))
                            {

                                int c = field.PlayersPositions[field.ActualPlayer].Item2 - (15 - j);
                                int f = field.PlayersPositions[field.ActualPlayer].Item1 - (10 - i);

                                for(int k = 0; k < field.PlayersPositions.Count; k++)
                                {

                                    if ((f, c) == field.PlayersPositions[k] && !field.Players[k].IsDead)
                                    {

                                        g.DrawImage(field.Players[k].print, j * 25 - 60, i * 25 - 60, 120, 120);

                                        if (field.Players[k].ActualWeapon != null)
                                            g.DrawImage(field.Players[k].ActualWeapon.Print(new DimObject(100, 60, false)), j * 25 + 15, i * 25 - 60);

                                        g.DrawString(field.Players[k].Name, new Font("Arial", 10), Brushes.Black, j * 25 - 60, i * 25 - 95);
                                        g.DrawString("Life: " + field.Players[k].Life + "  Energy: " + field.Players[k].Energy, new Font("Arial", 8), Brushes.Red, j * 25 - 60, i * 25 - 80);

                                    }

                                }

                            }

                        }

                    }

                }

            }

            if (!field.Players[field.ActualPlayer].IsDead)
            {

                g.DrawImage(field.Players[field.ActualPlayer].print, 315, 190, 120, 120);

                if (field.Players[field.ActualPlayer].ActualWeapon != null)
                    g.DrawImage(field.Players[field.ActualPlayer].ActualWeapon.Print(new DimObject(100, 60, false)), 390, 190);

                g.DrawString(field.Players[field.ActualPlayer].Name, new Font("Arial", 10), Brushes.Black, 315, 155);
                g.DrawString("Life: " + field.Players[field.ActualPlayer].Life + "  Energy: " + field.Players[field.ActualPlayer].Energy, new Font("Arial", 8), Brushes.Red, 315, 170);

                g.DrawImage(field.Players[field.ActualPlayer].Print(new PrintPlayer(), pp), 200, 380);

            }

            g.DrawRectangle(new Pen(Brushes.Black), new Rectangle(5, 380, 100, 100));

            for (int i = 0; i < field.PlayersPositions.Count; i++)
            {
                if (!field.Players[i].IsDead)
                    g.DrawEllipse(new Pen(Brushes.Red), 5 + field.PlayersPositions[i].Item2, 380 + field.PlayersPositions[i].Item1, 5, 5);
            }

            return b;

        }

        public Image Print(in Environment<Element<Image>[,], StarPlayer<Image>, Article<Image>[], Image> environment, Image Background, PrintParameters pp)
        {

            Image i = new Bitmap(750, 500);

            Graphics g = Graphics.FromImage(i);

            g.DrawImage(Background, 0, 0, 754, 504);

            g.DrawImage(Print(environment, pp), 0, 0);

            return i;

        }
    
    }
    public class PrintPlayer : IStarPlayerPrint<Image>
    {
        public Image Print(Article<Image>[] Collection, PrintParameters pp)
        {
            
            Image barra = new Bitmap(Collection.Length * 90 + 2 * (Collection.Length + 1), 94);

            Graphics g = Graphics.FromImage(barra);

            for (int i = 0; i <= Collection.Length; i++)
            {

                g.DrawLine(new Pen(Brushes.Black), i * 92, 0, i * 92, 93);
                g.DrawLine(new Pen(Brushes.Black), (i * 92) + 1, 0, (i * 92) + 1, 93);

                if (i < Collection.Length && Collection[i] != null)
                    g.DrawImage(Collection[i].Print(new DimObject(90, 90, true)), (i * 92) + 2, 2);
                

            }

            g.DrawLine(new Pen(Brushes.Black), 0, 0, Collection.Length * 90 + 2 * (Collection.Length + 1) - 1, 0);
            g.DrawLine(new Pen(Brushes.Black), 0, 1, Collection.Length * 90 + 2 * (Collection.Length + 1) - 1, 1);
            g.DrawLine(new Pen(Brushes.Black), 0, 92, Collection.Length * 90 + 2 * (Collection.Length + 1) - 1, 92);
            g.DrawLine(new Pen(Brushes.Black), 0, 93, Collection.Length * 90 + 2 * (Collection.Length + 1) - 1, 93);

            return barra;

        }
    }
    public class Trash : GamePlatform.Element<Image>
    {

        public string Name { get; private set; }
        public Image Pic { get; private set; }

        public Trash(string name, Image image)
        {
            this.Name = name;
            this.Pic = image;
        }

        public override bool EqualsTo(Element<Image> other)
        {
            return other is Trash && Name == ((Trash)other).Name;
        }

        public override Image Print(PrintParameters pp)
        {

            DimObject dimo = (DimObject)pp;

            Bitmap b = new Bitmap(dimo.Width, dimo.Height);

            Graphics g = Graphics.FromImage(b);
            g.DrawImage(Pic, 0, 0, dimo.Width, dimo.Height);

            return b;

        }
    }
    public class DimObject: GamePlatform.PrintParameters
    {
        public int Height { get; set; }
        public int Width { get; set; }

        public bool ShowData { get; set; }
        
        public DimObject(int height, int width, bool showData)
        {
            Height = height;
            Width = width;
            ShowData = showData;
        }

    }
    public class Location: PrintParameters
    {
        public int x;
        public int y;

        public Location(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

    }

}