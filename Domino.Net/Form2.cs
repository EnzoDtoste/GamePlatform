using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DominoPlatform;
using GamePlatform;

namespace Domino.Net
{
    public partial class Form2 : Form
    {

        Form1 f1;

        public Form2(Form1 f1)
        {

            InitializeComponent();

            this.f1 = f1;

        }

        void Initialize<F, T>(List<T> collection, List<int> values, Func<List<T>, PrintParameters, Image> print) where F: Ficha<T, Image>
        {
            
            List<IConditions<T, Image>> conditions = new List<IConditions<T,Image>>();

            if (checkBox3.Checked)
                conditions.Add(new MePegue<T>((int)numericUpDown2.Value, f1.pictureBox1, checkBox1.Checked));

            conditions.Add(new SeTranco<T>(f1.pictureBox1));

            Distribute<T, Image> distribute;

            if (radioButton1.Checked)
                distribute = RandomDistribute<T>;
            else
                distribute = DistributeInOrder<T>;

            GenerateFichas<T, Image> generate;

            if (radioButton3.Checked)
                generate = new GenerateAllFichas<F, T>(collection, values, (int)numericUpDown3.Value, (int)numericUpDown4.Value, print).Generate;
            else
                generate = new GenerateAllFichas<F, T>(collection, values, (int)numericUpDown3.Value, (int)numericUpDown4.Value, print, (int)numericUpDown5.Value).Generate;

            Winner<T, Image> winner;

            if (radioButton5.Checked)
                winner = ClassicWinner<T>();
            else
                winner = EqualWinner<T>();

            PassTurn<T, Image> pass;

            if (radioButton7.Checked)
                pass = ClassicPass<T>();
            else
                pass = ifDoubleInvert<T>();

            List<PlayFicha<T, Image>> players = new List<PlayFicha<T, Image>>();

            foreach(var player in listBox1.Items)
            {

                if (player.ToString() == "Random Player")
                    players.Add(PlayRandom);
                else if (player.ToString() == "Play First Player")
                    players.Add(PlayFirst);

            }

            f1.Initialize<F, T>(generate, distribute, checkBox2.Checked, players, new PrintGame<T>(), pass, winner, (int)numericUpDown1.Value, conditions.ToArray());

        }

        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {

            if (comboBox1.Text == "Int")
                Initialize<FichaClassic<int>, int>(new List<int>() { 0, 1, 2, 3, 4, 5, 6 }, new List<int>() { 0, 1, 2, 3, 4, 5, 6 }, PrintInt);

            else if(comboBox1.Text == "Int Multiple of 3")
                Initialize<FichaInt3, int>(new List<int>() { 0, 1, 2, 3, 4, 5, 6 }, new List<int>() { 0, 1, 2, 3, 4, 5, 6 }, PrintInt);

            else if (comboBox1.Text == "Color")
                Initialize<FichaClassic<Color>, Color>(new List<Color>() { Color.Red, Color.Black, Color.Blue, Color.Brown, Color.Green, Color.Orange, Color.Violet }, new List<int>() { 0, 1, 2, 3, 4, 5, 6 }, PrintColor);

        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton4.Checked)
                numericUpDown5.Enabled = true;
            else
                numericUpDown5.Enabled = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {

            listBox1.Items.Add(comboBox2.Text);

        }

        (T, Ficha<T, Image>, int) PlayFirst<T>(List<Ficha<T, Image>>[] Rounds, List<T> sides, List<Ficha<T, Image>> hand)
        {

            if (sides == null)
                return (hand[0].sides[0], hand[0], -1);

            foreach (var ficha in hand)
            {

                foreach (var side in sides)
                {

                    var lads = ficha.CanMatch(side);

                    if (lads.Count > 0)
                        return (side, ficha, lads[0]);

                }

            }

            return (hand[0].sides[0], null, -1);

        }

        public (T, Ficha<T, Image>, int) PlayRandom<T>(List<Ficha<T, Image>>[] Rounds, List<T> sides, List<Ficha<T, Image>> hand)
        {

            Random r = new Random();

            if (sides == null)
                return (hand[0].sides[0], hand[0], -1);

            List<(T, Ficha<T, Image>, int)> validMoves = GetValidMoves(sides, hand);
            return validMoves[r.Next(validMoves.Count)];

        }

        IEnumerable<Ficha<T, Image>> RandomDistribute<T>(List<Ficha<T, Image>> FichasCollection, int total)
        {

            Random r = new Random();
            List<int> used_indexes = new List<int>();

            for (int i = 0; i < total; i++)
            {

                int index;

                do
                {

                    index = r.Next(0, FichasCollection.Count);

                } while (used_indexes.Contains(index));

                used_indexes.Add(index);

                yield return FichasCollection[index];

            }

        }

        IEnumerable<Ficha<T, Image>> DistributeInOrder<T>(List<Ficha<T, Image>> FichasCollection, int total)
        {

            for (int i = 0; i < total; i++)
            {
                yield return FichasCollection[i];
            }

        }

        Image PrintColor(List<Color> sides, PrintParameters pp)
        {

            DimFicha df = (DimFicha)pp;

            Image ficha = new Bitmap(df.Width, df.Height);

            Graphics g = Graphics.FromImage(ficha);

            int levels = sides.Count / 2 + sides.Count % 2;

            for (int i = 0; i < levels; i++)
            {

                Bitmap b1 = new Bitmap(10, 10);
                Bitmap b2 = new Bitmap(10, 10);

                for (int j = 0; j < 10; j++)
                {

                    for (int k = 0; k < 10; k++)
                    {
                        b1.SetPixel(j, k, sides[2 * i]);
                        b2.SetPixel(j, k, sides[(2 * i) + 1]);
                    }

                }

                g.DrawImage(b1, 0, i * (ficha.Height / levels), df.Width / 2, df.Height / levels);

                if ((2 * i) + 1 < sides.Count)
                    g.DrawImage(b2, ficha.Width / 2, i * (ficha.Height / levels), df.Width / 2, df.Height / levels);

            }

            return ficha;
        }

        Image PrintInt(List<int> sides, PrintParameters pp)
        {

            DimFicha df = (DimFicha)pp;

            Image ficha = new Bitmap(df.Width, df.Height);

            Graphics g = Graphics.FromImage(ficha);

            int levels = sides.Count / 2 + sides.Count % 2;

            Image[] numbers = new Image[7];

            numbers[0] = Domino.Net.Properties.Resources._0;
            numbers[1] = Domino.Net.Properties.Resources._1;
            numbers[2] = Domino.Net.Properties.Resources._2;
            numbers[3] = Domino.Net.Properties.Resources._3;
            numbers[4] = Domino.Net.Properties.Resources._4;
            numbers[5] = Domino.Net.Properties.Resources._5;
            numbers[6] = Domino.Net.Properties.Resources._6;

            for (int i = 0; i < levels; i++)
            {

                g.DrawImage(numbers[sides[2 * i]], 0, i * (ficha.Height / levels), df.Width / 2, df.Height / levels);

                if ((2 * i) + 1 < sides.Count)
                    g.DrawImage(numbers[sides[(2 * i) + 1]], ficha.Width / 2, i * (ficha.Height / levels), df.Width / 2, df.Height / levels);

            }

            return ficha;

        }

        PassTurn<T, Image> ClassicPass<T>()
        {
            return (x, y) => 1;
        }

        PassTurn<T, Image> ifDoubleInvert<T>()
        {

            return (x, y) =>
            {

                if (x[y].Count > 0 && x[y][x[y].Count - 1] != null)
                {

                    for (int i = 0; i < x[y][x[y].Count - 1].sides.Count - 1; i++)
                    {
                        if (!x[y][x[y].Count - 1].sides[i].Equals(x[y][x[y].Count - 1].sides[i + 1]))
                            return 1;
                    }

                    return -1;

                }

                return 1;

            };

        }

        Winner<T, Image> ClassicWinner<T>()
        {
            return (in List<DominoPlayer<T, Image>> x) =>
            {

                int menor = x[0].TotalValues();
                List<int> indexes = new List<int>() { 0 };

                for (int i = 1; i < x.Count; i++)
                {
                    if (x[i].TotalValues() == menor)
                        indexes.Add(i);
                    else if (x[i].TotalValues() < menor)
                    { menor = x[i].TotalValues(); indexes = new List<int>() { i }; }
                }

                return indexes;

            };
        }

        Winner<T, Image> EqualWinner<T>()
        {

            return (in List<DominoPlayer<T, Image>> x) =>
            {

                int equal = 0;
                List<int> indexes = new List<int>();

                for (int i = 0; i < x.Count; i++)
                {

                    Dictionary<T, int> equals = new Dictionary<T, int>();

                    foreach (var ficha in x[i].Collection)
                    {

                        foreach (T side in ficha.sides)
                        {

                            if (equals.ContainsKey(side))
                                equals[side]++;
                            else
                                equals.Add(side, 1);

                        }

                    }

                    if(!(equals.Values.Count == 0 && equal != 0) && ((equals.Values.Count == 0 && equal == 0) || equal == equals.Values.Max()))
                        indexes.Add(i);

                    else if (equal < equals.Values.Max())
                    { equal = equals.Values.Max(); indexes = new List<int>() { i }; }

                }

                return indexes;

            };

        }

        public static List<(T, Ficha<T, Image>, int)> GetValidMoves<T>(List<T> sides, List<Ficha<T, Image>> hand)
        {
            
            List<(T, Ficha<T, Image>, int)> validMoves = new List<(T, Ficha<T, Image>, int)>();
            
            foreach (var ficha in hand)
            {
                foreach (var side in sides)
                {
                    
                    var playableSides = ficha.CanMatch(side);
                    
                    for (int i = 0; i < playableSides.Count; i++)
                    {
                        validMoves.Add((side, ficha, playableSides[i]));
                    }

                }
            }
            
            if (validMoves.Count == 0) validMoves.Add((hand[0].sides[0], null, -1));
            
            return validMoves;
        
        }

    }

    public class GenerateAllFichas<F, T> where F : Ficha<T, Image>
    {

        List<T> collection;
        List<int> values;
        int sides;
        int play_bySides;
        Func<List<T>, PrintParameters, Image> print;
        int top;

        public GenerateAllFichas(List<T> collection, List<int> values, int sides, int play_bySides, Func<List<T>, PrintParameters, Image> print)
        { this.collection = collection; this.values = values; this.sides = sides; this.play_bySides = play_bySides; this.print = print; top = -1; }

        public GenerateAllFichas(List<T> collection, List<int> values, int sides, int play_bySides, Func<List<T>, PrintParameters, Image> print, int top)
        { this.collection = collection; this.values = values; this.sides = sides; this.play_bySides = play_bySides; this.print = print; this.top = top; }


        public IEnumerable<F> Generate()
        {

            if (top == -1)
                return RecGenerate(0, new List<T>());

            return RandomGenerate();

        }

        private IEnumerable<F> RecGenerate(int index, List<T> chain)
        {

            if (chain.Count == sides)
            {

                try
                { return new List<F>() { (F)Activator.CreateInstance(typeof(F), new List<T>(chain.ToArray()), values, play_bySides, print) }; }

                catch
                {
                    return new List<F>() { (F)Activator.CreateInstance(typeof(F), new List<T>(chain.ToArray()), values, play_bySides) };
                }

            }

            List<F> fichas = new List<F>();

            for (int i = index; i < collection.Count; i++)
            {
                chain.Add(collection[i]);
                fichas.AddRange(RecGenerate(i, chain));
                chain.RemoveAt(chain.Count - 1);
            }

            return fichas;

        }

        private IEnumerable<F> RandomGenerate()
        {

            List<F> fichas = new List<F>();

            List<F> allfichas = new List<F>(RecGenerate(0, new List<T>()));

            Random r = new Random();

            for (int i = 0; i < top; i++)
            {

                int index = r.Next(allfichas.Count);

                fichas.Add(allfichas[index]);

                allfichas.RemoveAt(index);

            }

            return fichas;

        }

    }

    public class MePegue<T> : IConditions<T, Image>
    {

        int FinishHand;
        PictureBox pictureBox;
        bool endgame;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="FinishHand"> negative if it is the number fichas played, >= 0 if it is the number of fichas remaining </param>
        /// <param name="pictureBox"></param>
        /// <param name="endgame"> if game must over </param>
        public MePegue(int FinishHand, PictureBox pictureBox, bool endgame)
        {
            this.FinishHand = FinishHand;
            this.pictureBox = pictureBox;
            this.endgame = endgame;
        }

        public bool Finish(in List<DominoPlayer<T, Image>> players, int actualPlayer, Winner<T, Image> winner)
        {

            Image i = new Bitmap(750, 450);

            Graphics g = Graphics.FromImage(i);

            g.DrawString("Player " + actualPlayer + " se pegó", new Font("Arial", 15, FontStyle.Bold), Brushes.Black, 200, 220);

            var winners = winner(players);

            for (int j = 0; j < winners.Count; j++)
                g.DrawString("Player " + winners[j] + " ganó", new Font("Arial", 15, FontStyle.Bold), Brushes.Black, 200, 260 + j * 40);

            pictureBox.Image = i;
            pictureBox.Refresh();

            if (endgame)
                return true;

            players[actualPlayer].Enabled = false;
            return false;

        }

        public bool isValid(in List<Ficha<T, Image>>[] Rounds, in List<DominoPlayer<T, Image>> players, int actualPlayer)
        {

            if (FinishHand >= 0)
                return players[actualPlayer].Collection.Count == FinishHand;

            else
            {

                int count = 0;

                foreach (var f in Rounds[actualPlayer])
                {
                    if (f != null)
                        count++;
                }

                return Math.Abs(FinishHand) == count;

            }

        }
    }

    public class SeTranco<T> : IConditions<T, Image>
    {

        PictureBox pictureBox;
        public SeTranco(PictureBox pictureBox)
        {
            this.pictureBox = pictureBox;
        }

        public bool Finish(in List<DominoPlayer<T, Image>> players, int actualPlayer, Winner<T, Image> winner)
        {
            Image i = new Bitmap(750, 450);

            Graphics g = Graphics.FromImage(i);

            g.DrawString("Se Tranco", new Font("Arial", 15, FontStyle.Bold), Brushes.Black, 200, 220);

            var winners = winner(players);

            for (int j = 0; j < winners.Count; j++)
                g.DrawString("Player " + winners[j] + " ganó", new Font("Arial", 15, FontStyle.Bold), Brushes.Black, 200, 260 + j * 40);

            pictureBox.Image = i;

            return true;
        }

        public bool isValid(in List<Ficha<T, Image>>[] Rounds, in List<DominoPlayer<T, Image>> players, int actualPlayer)
        {
            for (int i = 0; i < Rounds.Length; i++)
            {
                if ((Rounds[i].Count == 0 || Rounds[i][Rounds[i].Count - 1] != null) && players[i].Enabled)
                    return false;
            }

            return true;
        }
    }

    public class PrintGame<T> : IBoardPrint<T, Image>
    {

        public Image Print(in Environment<TreeN<T, Image>, DominoPlayer<T, Image>, List<Ficha<T, Image>>, Image> environment, PrintParameters pp)
        {

            Board<T, Image> board = (Board<T, Image>)environment;

            Image image = new Bitmap(750, 450);

            Graphics g = Graphics.FromImage(image);

            if (board.Collection != null)
            {

                int deep = board.Collection.Deep();

                for (int i = 0; i < deep; i++)
                {

                    g.DrawImage(PrintFranja(board.Collection.Level(i), 750, ((DimFicha)pp).Height, pp), 0, i * (450 / deep));

                }

            }

            g.DrawImage(board.Players[board.ActualPlayer].Print(new PrintHand<T>(), pp), 200, 350);
            g.DrawString("Player " + board.ActualPlayer, new Font("Arial", 15, FontStyle.Bold), Brushes.Black, 550, 400);

            return image;

        }

        public Image Print(in Environment<TreeN<T, Image>, DominoPlayer<T, Image>, List<Ficha<T, Image>>, Image> environment, Image Background, PrintParameters pp)
        {
            throw new NotImplementedException();
        }

        private Image PrintFranja(List<Ficha<T, Image>> fichas, int width, int height, PrintParameters pp)
        {

            Image image = new Bitmap(width, height);

            Graphics g = Graphics.FromImage(image);

            for (int i = 0; i < fichas.Count; i++)
            {

                if (fichas[i] != null)
                    g.DrawImage(fichas[i].Print(new DimFicha(100, 50)), i * (width / fichas.Count), 0);

            }

            return image;

        }

    }

    public class PrintHand<T> : IDominoPlayerPrint<T, Image>
    {
        public Image Print(List<Ficha<T, Image>> Collection, GamePlatform.PrintParameters pp)
        {

            DimFicha df = (DimFicha)pp;

            Image i = new Bitmap(df.Height * Collection.Count + 1, df.Width);

            Graphics g = Graphics.FromImage(i);

            for (int j = 0; j < Collection.Count; j++)
            {

                Image ficha = Collection[j].Print(pp);

                ficha.RotateFlip(RotateFlipType.Rotate90FlipNone);

                g.DrawImage(ficha, j * df.Height, 0);
            }

            return i;

        }
    }

    public class FichaClassic<T> : Ficha<T, Image>
    {

        Func<List<T>, PrintParameters, Image> print;

        public FichaClassic(List<T> sides, List<int> values, int plays_bySide, Func<List<T>, PrintParameters, Image> print) : base(sides, values, plays_bySide)
        {
            this.print = print;
        }

        public override List<int> CanMatch(T side)
        {

            List<int> indexes = new List<int>();

            for (int i = 0; i < sides.Count; i++)
            {
                if (sides[i].Equals(side))
                    indexes.Add(i);
            }

            return indexes;

        }

        public override int TotalValue()
        {
            return values.Sum();
        }

        public override bool EqualsTo(Element<Image> other)
        {

            FichaClassic<T> f = (FichaClassic<T>)other;

            return this.sides.Count == f.sides.Count && UEqualsTo(this, f) && UEqualsTo(f, this);

        }

        private bool UEqualsTo(FichaClassic<T> f1, FichaClassic<T> f2)
        {

            foreach (T side in f1.sides)
            {

                if (!f2.sides.Contains(side))
                    return false;

            }

            return true;

        }

        public override Image Print(PrintParameters pp)
        {
            return print(sides, pp);
        }

    }

    public class FichaInt3 : Ficha<int, Image>
    {

        Func<List<int>, PrintParameters, Image> print;

        public FichaInt3(List<int> sides, List<int> values, int plays_bySide, Func<List<int>, PrintParameters, Image> print) : base(sides, values, plays_bySide)
        {
            this.print = print;
        }

        public override List<int> CanMatch(int side)
        {

            List<int> indexes = new List<int>();

            for(int i = 0; i < sides.Count; i++)
            {

                if ((sides[i] + side) % 3 == 0)
                    indexes.Add(i);

            }

            return indexes;

        }

        public override int TotalValue()
        {
            return values.Sum();
        }

        public override bool EqualsTo(Element<Image> other)
        {

            FichaInt3 f = (FichaInt3)other;

            return this.sides.Count == f.sides.Count && UEqualsTo(this, f) && UEqualsTo(f, this);

        }

        private bool UEqualsTo(FichaInt3 f1, FichaInt3 f2)
        {

            foreach (int side in f1.sides)
            {

                if (!f2.sides.Contains(side))
                    return false;

            }

            return true;

        }

        public override Image Print(PrintParameters pp)
        {
            return print(sides, pp);
        }

    }

    public class DimFicha : PrintParameters
    {

        public int Height;
        public int Width;

        public DimFicha(int width, int height)
        {
            this.Height = height;
            this.Width = width;
        }

    }

}
