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

        public void Initialize<F, T>(List<T> collection, List<int> values, Func<List<T>, PrintParameters, Image> print) where F: Ficha<T, Image>
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
                else if (player.ToString() == "Bota Gorda Player")
                    players.Add(BotaGorda);
                else if (player.ToString() == "Bota Suave Player")
                    players.Add(BotaSuave);
                else if (player.ToString() == "Smart Player")
                    players.Add(PlaySmart);
            }

            f1.Initialize<F, T>(generate, distribute, checkBox2.Checked, players, new PrintGame<T>(), pass, winner, (int)numericUpDown1.Value, conditions.ToArray());

        }

        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {

           
        }
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

            listBox1.Items.Remove(listBox1.SelectedItem);

            if (listBox1.Items.Count > 0)
            {
                long all = CollectionCount();
                if(numericUpDown1.Value > all / listBox1.Items.Count)
                {
                    numericUpDown1.Value = all/ listBox1.Items.Count;
                }
            } 
                

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (listBox1.Items.Count > 1)
            {
                
                if (comboBox1.Text == "Int")
                    Initialize<FichaClassic<int>, int>(new List<int>() { 0, 1, 2, 3, 4, 5, 6 }, new List<int>() { 1, 2, 3, 4, 5, 6, 7 }, PrintInt);

                else if (comboBox1.Text == "Int Multiple of 3")
                    Initialize<FichaInt3, int>(new List<int>() { 0, 1, 2, 3, 4, 5, 6 }, new List<int>() { 1, 2, 3, 4, 5, 6, 7 }, PrintInt);

                else if (comboBox1.Text == "Color")
                    Initialize<FichaClassic<Color>, Color>(new List<Color>() { Color.Red, Color.Black, Color.Blue, Color.Brown, Color.Green, Color.Orange, Color.Violet }, new List<int>() { 1, 2, 3, 4, 5, 6, 7 }, PrintColor);
                this.Hide();
            }
            else MessageBox.Show("There must be at least two players‼");
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton4.Checked)
            {
                numericUpDown5.Enabled = true;
                numericUpDown5.Value = listBox1.Items.Count;
                numericUpDown1.Value = 1;
            }
                
            else
                numericUpDown5.Enabled = false;
        }

        private void numericUpDown5_ValueChanged(object sender, EventArgs e)
        {

            long all = CollectionCount();
            if (numericUpDown5.Value > all)
                numericUpDown5.Value = all;
            if(numericUpDown5.Value/ listBox1.Items.Count < numericUpDown1.Value)
            {
                numericUpDown1.Value = numericUpDown5.Value/ listBox1.Items.Count;
            }
        }

        private void numericUpDown3_ValueChanged(object sender, EventArgs e)
        {
            if (numericUpDown3.Value > 10) numericUpDown3.Value = 10;
            long all = CollectionCount();
            if (radioButton4.Checked && all < (int)numericUpDown5.Value)
            {
                numericUpDown5.Value = all;
            }
            if((all / listBox1.Items.Count) < (int)numericUpDown1.Value)
            {
                numericUpDown1.Value = all / listBox1.Items.Count;
            }
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            if ((CollectionCount() / listBox1.Items.Count) < (int)numericUpDown1.Value)
                numericUpDown1.Value--;
            if (numericUpDown5.Enabled && (int)numericUpDown5.Value/listBox1.Items.Count < numericUpDown1.Value)
            {
                numericUpDown1.Value = (int)numericUpDown5.Value/ listBox1.Items.Count;
            }
        }

        public long CollectionCount()
        {
            int sides = (int)numericUpDown3.Value;
            return Factorial(7 + sides - 1) / (Factorial(sides) * Factorial(6));
        }

        public long Factorial(int num)
        {
            long result = 1;
            for (int i = 2; i <= num; i++)
            {
                result *= i;
            }
            return result;
        }

        private void button1_Click(object sender, EventArgs e)
        {

            listBox1.Items.Add(comboBox2.Text);

            long all = CollectionCount();

            if (numericUpDown1.Value > all / listBox1.Items.Count)
            {
                if (all / listBox1.Items.Count == 0)
                    listBox1.Items.RemoveAt(listBox1.Items.Count - 1);

                else
                    numericUpDown1.Value = all / listBox1.Items.Count;
            }

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
            
            return (sides[0], null, -1);

        }

        public (T, Ficha<T, Image>, int) PlayRandom<T>(List<Ficha<T, Image>>[] Rounds, List<T> sides, List<Ficha<T, Image>> hand)
        {

            Random r = new Random();

            if (sides == null)
                return (hand[0].sides[0], hand[0], -1);

            List<(T, Ficha<T, Image>, int)> validMoves = GetValidMoves(sides, hand);
            return validMoves.Count > 0 ? validMoves[r.Next(validMoves.Count)] : (sides[0], null, -1);

        }

        public (T, Ficha<T, Image>, int) BotaGorda<T>(List<Ficha<T, Image>>[] Rounds, List<T> sides, List<Ficha<T, Image>> hand)
        {
           return Botador(true, Rounds, sides, hand);
        }
        public (T, Ficha<T, Image>, int) BotaSuave<T>(List<Ficha<T, Image>>[] Rounds, List<T> sides, List<Ficha<T, Image>> hand)
        {
            return Botador(false, Rounds, sides, hand);
        }

        private (T, Ficha<T, Image>, int) Botador<T>(bool botador,List<Ficha<T, Image>>[] Rounds, List<T> sides, List<Ficha<T, Image>> hand)
        {
           
            if (sides == null)
            {
                Ficha<T, Image> best = hand[0];
                foreach (var ficha in hand)
                {
                    if (botador? ficha.TotalValue() > best.TotalValue() : ficha.TotalValue() < best.TotalValue()) best = ficha;
                }
                return (hand[0].sides[0], best, -1);
            }

            List<(T, Ficha<T, Image>, int)> validMoves = GetValidMoves(sides, hand);

            if (validMoves.Count == 0) return (sides[0], null, -1);

            (T, Ficha<T, Image>, int) Best = validMoves[0];

            foreach (var play in validMoves)
            {
                var ficha = play.Item2;
                if (ficha == null) return play;


                if (botador? ficha.TotalValue() > Best.Item2.TotalValue() : ficha.TotalValue() < Best.Item2.TotalValue()) Best = play;


            }

            return Best;
        }

        public (T, Ficha<T, Image>, int) PlaySmart<T>(List<Ficha<T, Image>>[] Rounds, List<T> sides, List<Ficha<T, Image>> hand)
        {

            if (sides == null)
            {

                int max = 0;
                Ficha<T, Image> best = null;

                foreach (var f in hand)
                {

                    int count = 0;

                    foreach (var ficha in hand)
                    {

                        foreach (var side in f.sides)
                        {

                            if (ficha.sides.Contains(side))
                                count++;

                        }

                    }

                    if(count > max)
                    {
                        max = count;
                        best = f;
                    }

                }

                return (hand[0].sides[0], best, -1);

            }

            var moves = GetValidMoves(sides, hand);

            (T, Ficha<T, Image>, int) rarest = (sides[0], null, -1);
            double ind = -1;

            foreach (var play in moves)
            {

                Dictionary<T, int> data = new Dictionary<T, int>();

                List<int> rareza = new List<int>();

                for (int i = 0; i < play.Item2.sides.Count; i++)
                {

                    if (i != play.Item3)
                    {

                        if (data.ContainsKey(play.Item2.sides[i]))
                            rareza.Add(data[play.Item2.sides[i]]);

                        else
                        {

                            int count = 0;

                            foreach (var item in Rounds)
                            {

                                foreach (var f in item)
                                {

                                    if (f != null && f.sides.Contains(play.Item2.sides[i]))
                                        count++;

                                }

                            }

                            data.Add(play.Item2.sides[i], count);
                            rareza.Add(count);

                        }

                    }

                }

                List<int> ihave = new List<int>();

                for (int i = 0; i < play.Item2.sides.Count; i++)
                {

                    if (i != play.Item3)
                    {

                        int count = 0;

                        foreach (var ficha in hand)
                        {

                            if (!ficha.EqualsTo(play.Item2))
                            {

                                if (ficha.sides.Contains(play.Item2.sides[i]))
                                    count++;

                            }

                        }

                        ihave.Add(count);

                    }

                }

                double prom = (((double)rareza.Sum() / (double)rareza.Count) + ((double)ihave.Sum() / (double)ihave.Count)) / 2.0;

                if (prom > ind)
                {
                    ind = prom;
                    rarest = play;
                }

            }

            return rarest;

        }
        
        IEnumerable<Ficha<T, Image>> RandomDistribute<T>(List<Ficha<T, Image>> FichasCollection, int total)
        {

            Random r = new Random();
            var randomized = FichasCollection.OrderBy(x => r.Next());
            foreach(var item in randomized.Take(total))
                yield return item;


        }

        IEnumerable<Ficha<T, Image>> DistributeInOrder<T>(List<Ficha<T, Image>> FichasCollection, int total)
        {

            for (int i = 0; i < total; i++)
            {
                yield return FichasCollection[i];
            }

        }

        public Image PrintColor(List<Color> sides, PrintParameters pp)
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

        public Image PrintInt(List<int> sides, PrintParameters pp)
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

                    else if (equals.Count > 0 && equal < equals.Values.Max())
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
                List<int> value = new List<int>();
                foreach(T t in chain)
                {
                    value.Add(values[collection.IndexOf(t)]);
                }
                try
                { return new List<F>() { (F)Activator.CreateInstance(typeof(F), new List<T>(chain.ToArray()), value, play_bySides, print) }; }

                catch
                {
                    return new List<F>() { (F)Activator.CreateInstance(typeof(F), new List<T>(chain.ToArray()), value, play_bySides) };
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
            if (endgame)
            {
                Image i = new Bitmap(750, 450);

                Graphics g = Graphics.FromImage(i);
                RectangleF table = new RectangleF(0, 0, 750, 450);
                Rectangle win = new Rectangle(350, 85, 80, 60);
                g.DrawImage(Properties.Resources.table, table);
                g.DrawIcon(Properties.Resources.icons8_Win, win);
                g.DrawString("Player " + actualPlayer + " se pegó", new Font("Arial", 15, FontStyle.Bold), Brushes.Black, 280, 150);
                System.Media.SoundPlayer estoyPegao = new System.Media.SoundPlayer(Properties.Resources.untitled_2022_06_30_23_15_09_REC__consolidated_);
                estoyPegao.Play();
                var winners = winner(players);
                
                for (int j = 0; j < winners.Count; j++)
                    g.DrawString("Player " + winners[j] + " ganó", new Font("Arial", 15, FontStyle.Bold), Brushes.Black, 280, 200 + j * 40);

                pictureBox.Image = i;
                pictureBox.Refresh();


                return true;
            }

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
            RectangleF table = new RectangleF(0, 0, 750, 450);
            Rectangle win = new Rectangle(350, 85, 80, 60);
            g.DrawImage(Properties.Resources.table, table);
            g.DrawIcon(Properties.Resources.icons8_Win, win);
            g.DrawString("Se Tranco", new Font("Arial", 15, FontStyle.Bold), Brushes.Black, 280, 150);
            System.Media.SoundPlayer applause = new System.Media.SoundPlayer(Properties.Resources.applause___Part_1);
            applause.Play();

            var winners = winner(players);

            for (int j = 0; j < winners.Count; j++)
                g.DrawString("Player " + winners[j] + " ganó", new Font("Arial", 15, FontStyle.Bold), Brushes.Black, 280, 200 + j * 40);

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
            RectangleF table = new RectangleF(0, 0, 750, 450);
            g.DrawImage(Properties.Resources.table,table);
            if (board.Collection != null)
            {
               
                int deep = board.Collection.Deep();

                for (int i = 0; i < deep; i++)
                {

                    g.DrawImage(PrintFranja(board.Collection.Level(i), 750, ((DimFicha)pp).Height, pp), 0, i * (450 / deep));

                }

            }

            g.DrawImage(board.Players[board.ActualPlayer].Print(new PrintHand<T>(), pp), 200, 350);
            
           if(board.Collection != null && Form2.GetValidMoves(board.Collection.AvailableSides(),board.Players[board.ActualPlayer].Collection).Count == 0)
           {
                Rectangle passE = new Rectangle(650, 340, 40, 40);
                Rectangle passH = new Rectangle(644, 361, 52, 52);
                g.DrawIcon(Properties.Resources.icons8_Explosion, passE);
                g.DrawIcon(Properties.Resources.icons8_Hand_Rock, passH);
                System.Media.SoundPlayer tocarMesa = new System.Media.SoundPlayer(Properties.Resources.mesa_de_noche_3__consolidated_);
                tocarMesa.Play();
                
           }
            g.DrawString("Player " + board.ActualPlayer, new Font("Arial", 15, FontStyle.Bold), Brushes.Black, 550, 380);

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
