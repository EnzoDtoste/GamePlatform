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

        /// <summary>
        /// Invokes the Initialize method at the Form1
        /// </summary>
        /// <typeparam name="F"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"> Collection sides for the ficha </param>
        /// <param name="values"> Value represented in a Int, of each side </param>
        /// <param name="print"> how the ficha print it self </param>
        public void Initialize<F, T>(List<T> collection, List<int> values, Func<List<T>, List<int>, PrintParameters, Image> print) where F: Ficha<T, Image>
        {
            
            List<IConditions<T, Image>> conditions = new List<IConditions<T,Image>>();

            if (checkBox2.Checked)
                conditions.Add(new Robar<T>());

            if (checkBox3.Checked)
                conditions.Add(new MePegue<T>((int)numericUpDown2.Value, f1.pictureBox1, checkBox1.Checked));

            conditions.Add(new SeTranco<T>(f1.pictureBox1));

            if (checkBox4.Checked)
                conditions.Add(new Plin<T>());

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

            f1.Initialize<F, T>(generate, distribute, players, new PrintGame<T>(), pass, winner, (int)numericUpDown1.Value, conditions.ToArray());

        }
        
        //Remove player from list
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

        //Start the game
        private void button2_Click(object sender, EventArgs e)
        {

            if (listBox1.Items.Count >= 2)
            {

                List<int> values = new List<int>();

                for (int i = 0; i <= numericUpDown6.Value; i++)
                    values.Add(i);

                List<int> nums = new List<int>();

                for (int i = 0; i <= numericUpDown6.Value; i++)
                    nums.Add(i);

                if (comboBox1.Text == "Int")
                    Initialize<FichaClassic<int>, int>(nums, values, PrintInt);

                else if (comboBox1.Text == "Int Multiple of 3")
                    Initialize<FichaInt3, int>(nums, values, PrintInt);

                else if (comboBox1.Text == "Color")
                {

                    List<Color> colors = new List<Color>();

                    Random r = new Random();

                    for(int i = 0; i <= numericUpDown6.Value; i++)
                        colors.Add(Color.FromArgb(r.Next(255), r.Next(255), r.Next(255)));

                    Initialize<FichaClassic<Color>, Color>(colors, values, PrintColor);

                }

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

        private void numericUpDown6_ValueChanged(object sender, EventArgs e)
        {
            if ((CollectionCount() / listBox1.Items.Count) < (int)numericUpDown1.Value)
                numericUpDown1.Value--;
            if (numericUpDown5.Enabled && (int)numericUpDown5.Value / listBox1.Items.Count < numericUpDown1.Value)
            {
                numericUpDown1.Value = (int)numericUpDown5.Value / listBox1.Items.Count;
            }
        }

        /// <summary>
        /// how many fichas will be generated with the actual settings
        /// </summary>
        /// <returns></returns>
        public long CollectionCount()
        {
            int sides = (int)numericUpDown3.Value;
            return Factorial((int)numericUpDown6.Value + 1 + sides - 1) / (Factorial(sides) * Factorial((int)numericUpDown6.Value));
        }

        Dictionary<int, long> facs = new Dictionary<int, long>();

        /// <summary>
        /// Factorial of a natural number
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public long Factorial(int num)
        {

            if (num == 0)
                return 1;

            if (facs.ContainsKey(num))
                return facs[num];

            facs.Add(num, num * Factorial(num - 1));
            return facs[num];

        }

        //add player to list
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
        
        //Player that plays the first valid move that it founds
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

        //Player who plays a random valid move
        public (T, Ficha<T, Image>, int) PlayRandom<T>(List<Ficha<T, Image>>[] Rounds, List<T> sides, List<Ficha<T, Image>> hand)
        {

            Random r = new Random();

            if (sides == null)
                return (hand[0].sides[0], hand[0], -1);

            List<(T, Ficha<T, Image>, int)> validMoves = GetValidMoves(sides, hand);
            return validMoves.Count > 0 ? validMoves[r.Next(validMoves.Count)] : (sides[0], null, -1);

        }

        //Player who plays the valid move that represents the ficha with higher value
        public (T, Ficha<T, Image>, int) BotaGorda<T>(List<Ficha<T, Image>>[] Rounds, List<T> sides, List<Ficha<T, Image>> hand)
        {
           return Botador(true, sides, hand);
        }
        
        //Player who plays the valid move that represents the ficha with lower value
        public (T, Ficha<T, Image>, int) BotaSuave<T>(List<Ficha<T, Image>>[] Rounds, List<T> sides, List<Ficha<T, Image>> hand)
        {
            return Botador(false, sides, hand);
        }

        private (T, Ficha<T, Image>, int) Botador<T>(bool botador, List<T> sides, List<Ficha<T, Image>> hand)
        {
           
            //if the game ain´t started
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

            foreach (var play in validMoves.Skip(1))
            {
                
                var ficha = play.Item2;
                
                //if true I keep the higher, if false I keep the lower
                if (botador? ficha.TotalValue() > Best.Item2.TotalValue() : ficha.TotalValue() < Best.Item2.TotalValue()) 
                    Best = play;

            }

            return Best;
        }

        //Player who plays according to some heuristic
        public (T, Ficha<T, Image>, int) PlaySmart<T>(List<Ficha<T, Image>>[] Rounds, List<T> sides, List<Ficha<T, Image>> hand)
        {

            if (sides == null)
            {

                int max = 0;
                Ficha<T, Image> best = null;

                //I play the ficha with the most repeat side in the hand
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

            //I play the ficha that contains the side that repeats the most on the board promediated with how many other fichas I contain with that side
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


        //given a number and a collection, return randomly that number of elements from the collection
        IEnumerable<Ficha<T, Image>> RandomDistribute<T>(List<Ficha<T, Image>> FichasCollection, int total)
        {

            Random r = new Random();
            var randomized = FichasCollection.OrderBy(x => r.Next());

            return randomized.Take(total);

        }

        //given a number and a collection, return the firsts elements from the collection
        IEnumerable<Ficha<T, Image>> DistributeInOrder<T>(List<Ficha<T, Image>> FichasCollection, int total)
        {

            for (int i = 0; i < total; i++)
            {
                yield return FichasCollection[i];
            }

        }


        //how to print a ficha of Color type side
        public Image PrintColor(List<Color> sides, List<int> used_sides, PrintParameters pp)
        {

            DimFicha df = (DimFicha)pp;

            //how many levels of height will have the ficha
            int levels = sides.Count / 2 + sides.Count % 2;

            Image ficha = new Bitmap(df.Width * 2, df.Height * levels);

            Graphics g = Graphics.FromImage(ficha);

            for (int i = 0; i < levels; i++)
            {

                //create the color blocks
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

                //paint the block and the one beside him
                g.DrawImage(b1, 0, i * (ficha.Height / levels), df.Width, df.Height);

                if (used_sides.Contains(2 * i))
                    g.DrawRectangle(new Pen(sides[2 * i] == Color.Red ? Color.Black : Color.Red, 3), 0, i * (ficha.Height / levels), df.Width, df.Height);

                if ((2 * i) + 1 < sides.Count)
                {

                    g.DrawImage(b2, ficha.Width / 2, i * (ficha.Height / levels), df.Width, df.Height);

                    if (used_sides.Contains(2 * i + 1))
                        g.DrawRectangle(new Pen(sides[2 * i + 1] == Color.Red ? Color.Black : Color.Red, 3), ficha.Width / 2, i * (ficha.Height / levels), df.Width, df.Height);

                }

            }

            return ficha;
        }

        //how to print a ficha of Int type side
        public Image PrintInt(List<int> sides, List<int> used_sides, PrintParameters pp)
        {

            DimFicha df = (DimFicha)pp;

            //how many levels of height will have the ficha
            int levels = sides.Count / 2 + sides.Count % 2;

            Image ficha = new Bitmap(df.Width * 2, df.Height * levels);

            Graphics g = Graphics.FromImage(ficha);

            Image[] numbers = new Image[10];

            numbers[0] = Domino.Net.Properties.Resources._0;
            numbers[1] = Domino.Net.Properties.Resources._1;
            numbers[2] = Domino.Net.Properties.Resources._2;
            numbers[3] = Domino.Net.Properties.Resources._3;
            numbers[4] = Domino.Net.Properties.Resources._4;
            numbers[5] = Domino.Net.Properties.Resources._5;
            numbers[6] = Domino.Net.Properties.Resources._6;
            numbers[7] = Domino.Net.Properties.Resources._7;
            numbers[8] = Domino.Net.Properties.Resources._8;
            numbers[9] = Domino.Net.Properties.Resources._9;

            for (int i = 0; i < levels; i++)
            {

                //paint the block and the one beside him
                if (sides[2 * i] < numbers.Length)
                    g.DrawImage(numbers[sides[2 * i]], 0, i * (ficha.Height / levels), df.Width, df.Height);

                //if I don´t have image for him, I paint the number
                else
                    g.DrawString(sides[2 * i].ToString(), new Font("Arial", df.Height / 3, FontStyle.Bold) , Brushes.Black, 10, i * (ficha.Height / levels) + 5);

                if (used_sides.Contains(2 * i))
                    g.DrawRectangle(new Pen(Brushes.Red, 3), 0, i * (ficha.Height / levels), df.Width, df.Height);

                if ((2 * i) + 1 < sides.Count)
                {

                    if (sides[(2 * i) + 1] < numbers.Length)
                        g.DrawImage(numbers[sides[(2 * i) + 1]], ficha.Width / 2, i * (ficha.Height / levels), df.Width, df.Height);
                    else
                        g.DrawString(sides[2 * i + 1].ToString(), new Font("Arial", df.Height / 3, FontStyle.Bold), Brushes.Black, ficha.Width / 2 + 10, i * (ficha.Height / levels) + 5);

                    if(used_sides.Contains(2 * i + 1))
                        g.DrawRectangle(new Pen(Brushes.Red, 3), ficha.Width / 2, i * (ficha.Height / levels), df.Width, df.Height);

                }

            }

            return ficha;

        }


        //classic way of passing the turn
        PassTurn<T, Image> ClassicPass<T>()
        {
            return (x, y) => 1;
        }

        //if the player plays a ficha with all sides equals, the direction of the game gets turned
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


        //classic way of winning the game, the players with the lowest sum of values of the fichas at their hand, wins
        Winner<T, Image> ClassicWinner<T>()
        {
            return (in List<DominoPlayer<T, Image>> x) =>
            {

                int menor = x[0].Collection.Count > 0 ? x[0].TotalValues() : -1;
                List<int> indexes = new List<int>() { 0 };
                
                for (int i = 1; i < x.Count; i++)
                {

                    int total = x[i].Collection.Count > 0 ? x[i].TotalValues() : -1;

                    if (total == menor)
                        indexes.Add(i);

                    else if (total < menor)
                    { menor = total; indexes = new List<int>() { i }; }

                }

                return indexes;

            };
        }

        //wins the players with the highest number of ficha´s side repeat 
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


        //returns all the valid moves for that hand
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
        Func<List<T>, List<int>, PrintParameters, Image> print;
        int top;

        public GenerateAllFichas(List<T> collection, List<int> values, int sides, int play_bySides, Func<List<T>, List<int>, PrintParameters, Image> print)
        { this.collection = collection; this.values = values; this.sides = sides; this.play_bySides = play_bySides; this.print = print; top = -1; }

        public GenerateAllFichas(List<T> collection, List<int> values, int sides, int play_bySides, Func<List<T>, List<int>, PrintParameters, Image> print, int top)
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
    
    public class Plin<T> : IConditions<T, Image>
    {
        
        public bool Finish(Board<T, Image> board, Winner<T, Image> winner)
        {

            board.NextPlayer(board.pass);
            return false;

        }

        public bool isValid(Board<T, Image> board)
        {

            var f = board.Rounds[board.ActualPlayer][board.Rounds[board.ActualPlayer].Count - 1];

            if (f != null) 
            {
                
                for(int i = 0; i < f.values.Count; i++)
                {
                    if (f.values[i] == 5 && !f.used_sides.Contains(i))
                        return true;
                }

            }

            return false;

        }

    }
    public class Robar<T> : IConditions<T, Image>
    {
        public bool Finish(Board<T, Image> board, Winner<T, Image> winner)
        {

            if (board.FichasAfuera.Count > 0)
            {

                Random r = new Random();

                var f = board.FichasAfuera[r.Next(board.FichasAfuera.Count)];
                Board<T, Image>.Remove(board.FichasAfuera, f);

                board.Players[board.ActualPlayer].Collection.Add(f);

                board.pass = 0;

            }

            return false;
        
        }

        public bool isValid(Board<T, Image> board)
        {
            return board.Rounds[board.ActualPlayer][board.Rounds[board.ActualPlayer].Count - 1] == null;
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

        public bool Finish(Board<T, Image> board, Winner<T, Image> winner)
        {
            
            if (endgame)
            {
                Image i = new Bitmap(pictureBox.Width, pictureBox.Height);

                Graphics g = Graphics.FromImage(i);
                RectangleF table = new RectangleF(0, 0, pictureBox.Width, pictureBox.Height);
                Rectangle win = new Rectangle(pictureBox.Width / 2 - 25, 85, 80, 60);
                g.DrawImage(Properties.Resources.table, table);
                g.DrawIcon(Properties.Resources.icons8_Win, win);
                g.DrawString("Player " + board.ActualPlayer + " se pegó", new Font("Arial", 15, FontStyle.Bold), Brushes.Black, pictureBox.Width / 3 + 30, 150);
                System.Media.SoundPlayer estoyPegao = new System.Media.SoundPlayer(Properties.Resources.untitled_2022_06_30_23_15_09_REC__consolidated_);
                estoyPegao.Play();
                var winners = winner(board.Players);

                for (int j = 0; j < winners.Count; j++)
                    g.DrawString("Player " + winners[j] + " ganó", new Font("Arial", 15, FontStyle.Bold), Brushes.Black, pictureBox.Width / 3 + 30, 200 + j * 40);

                pictureBox.Image = i;
                pictureBox.Refresh();


                return true;
            }

            board.Players[board.ActualPlayer].Enabled = false;
            return false;

        }

        public bool isValid(Board<T, Image> board)
        {

            if (!board.Players[board.ActualPlayer].Enabled)
                return false;

            if (FinishHand >= 0)
                return board.Players[board.ActualPlayer].Collection.Count == FinishHand;

            else
            {

                int count = 0;

                foreach (var f in board.Rounds[board.ActualPlayer])
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

        public bool Finish(Board<T, Image> board, Winner<T, Image> winner)
        {
            
            Image i = new Bitmap(pictureBox.Width, pictureBox.Height);

            Graphics g = Graphics.FromImage(i);
            RectangleF table = new RectangleF(0, 0, pictureBox.Width, pictureBox.Height);
            Rectangle win = new Rectangle(pictureBox.Width / 2 - 25, 85, 80, 60);
            g.DrawImage(Properties.Resources.table, table);
            g.DrawIcon(Properties.Resources.icons8_Win, win);
            g.DrawString("Se Tranco", new Font("Arial", 15, FontStyle.Bold), Brushes.Black, pictureBox.Width / 3 + 30, 150);
            System.Media.SoundPlayer applause = new System.Media.SoundPlayer(Properties.Resources.applause___Part_1);
            applause.Play();

            var winners = winner(board.Players);

            for (int j = 0; j < winners.Count; j++)
                g.DrawString("Player " + winners[j] + " ganó", new Font("Arial", 15, FontStyle.Bold), Brushes.Black, pictureBox.Width / 3 + 30, 200 + j * 40);

            pictureBox.Image = i;

            return true;

        }

        public bool isValid(Board<T, Image> board)
        {
            
            for (int i = 0; i < board.Rounds.Length; i++)
            {
                if ((board.Rounds[i].Count == 0 || board.Rounds[i][board.Rounds[i].Count - 1] != null) && board.Players[i].Enabled)
                    return false;
            }

            return true;
        
        }

    }
    
    public class PrintGame<T> : IBoardPrint<T, Image>
    {

        DimFicha dimFicha;

        public Image Print(in Environment<TreeN<T, Image>, DominoPlayer<T, Image>, List<Ficha<T, Image>>, Image> environment, PrintParameters pp)
        {

            Board<T, Image> board = (Board<T, Image>)environment;

            DimGame dimGame = (DimGame)pp;

            Image image = new Bitmap(dimGame.Width, dimGame.Height);

            Graphics g = Graphics.FromImage(image);
            RectangleF table = new RectangleF(0, 0, dimGame.Width, dimGame.Height);
            g.DrawImage(Properties.Resources.table,table);

            if (board.Collection != null)
            {
               
                int deep = board.Collection.Deep();

                if (dimFicha == null)
                    dimFicha = new DimFicha(dimGame.s_ficha.Width, dimGame.s_ficha.Height);

                while(Fit(deep, board.Collection.Level(0)[0].Print(dimFicha).Height, dimGame.Height))
                {

                    dimFicha.Width = (int)(dimFicha.Width / 1.25);
                    dimFicha.Height = (int)(dimFicha.Height / 1.25);

                }

                for (int i = 0; i < deep; i++)
                {

                    g.DrawImage(PrintFranja(board.Collection.Level(i), dimGame.Width, dimGame.Height, dimFicha), 0, i * (dimGame.Height / deep));

                }

            }

            Image hand = board.Players[board.ActualPlayer].Print(new PrintHand<T>(), dimGame.s_ficha);

            if (hand.Width > dimGame.Width)
                g.DrawImage(hand, 0, dimGame.Height - hand.Height - 20, dimGame.Width, (int)(hand.Height / ((double)hand.Width / dimGame.Width)));

            else
                g.DrawImage(hand, dimGame.Width / 2 - hand.Width / 2, dimGame.Height - hand.Height - 20);
            
           if(board.Collection != null && Form2.GetValidMoves(board.Collection.AvailableSides(),board.Players[board.ActualPlayer].Collection).Count == 0)
           {
                Rectangle passE = new Rectangle(600, 340, 40, 40);
                Rectangle passH = new Rectangle(594, 361, 52, 52);
                g.DrawIcon(Properties.Resources.icons8_Explosion, passE);
                g.DrawIcon(Properties.Resources.icons8_Hand_Rock, passH);
                System.Media.SoundPlayer tocarMesa = new System.Media.SoundPlayer(Properties.Resources.mesa_de_noche_3__consolidated_);
                tocarMesa.Play();
                
           }

           g.DrawString("Player " + board.ActualPlayer, new Font("Arial", 15, FontStyle.Bold), Brushes.Black, dimGame.Width - 200, dimGame.Height - 70);

           return image;

        }

        private bool Fit(int deep, int f_h, int g_h)
        {
            return deep * f_h > g_h;
        }

        public Image Print(in Environment<TreeN<T, Image>, DominoPlayer<T, Image>, List<Ficha<T, Image>>, Image> environment, Image Background, PrintParameters pp)
        {


            DimGame dimGame = (DimGame)pp;

            Image image = new Bitmap(dimGame.Width, dimGame.Height);

            Graphics g = Graphics.FromImage(image);
            RectangleF table = new RectangleF(0, 0, dimGame.Width, dimGame.Height);
            g.DrawImage(Properties.Resources.table, table);

            g.DrawImage(Print(environment, pp), 0, 0);

            return image;

        }

        private Image PrintFranja(List<Ficha<T, Image>> fichas, int width, int height, DimFicha dimFicha)
        {

            Bitmap image = new Bitmap(width, height);

            Graphics g = Graphics.FromImage(image);

            for (int i = 0; i < fichas.Count; i++)
            {

                if (fichas[i] != null)
                    g.DrawImage(fichas[i].Print(dimFicha), i * (width / fichas.Count), 0);
                
            }

            return image;

        }

    }
    public class PrintHand<T> : IDominoPlayerPrint<T, Image>
    {
        public Image Print(List<Ficha<T, Image>> Collection, GamePlatform.PrintParameters pp)
        {

            if (Collection.Count == 0)
                return new Bitmap(0, 0);

            DimFicha df = (DimFicha)pp;

            var f = Collection[0].Print(pp);

            bool b = f.Width > f.Height;

            Image i = new Bitmap(b ? (f.Height + 2) * Collection.Count : (f.Width + 2) * Collection.Count, b ? f.Width : f.Height);

            Graphics g = Graphics.FromImage(i);

            for (int j = 0; j < Collection.Count; j++)
            {

                Image ficha = Collection[j].Print(pp);

                if (b)
                    ficha.RotateFlip(RotateFlipType.Rotate90FlipNone);

                g.DrawImage(ficha, j * (b ? f.Height + 2 : f.Width + 2), 0);
            }

            return i;

        }
    }
    
    public class FichaClassic<T> : Ficha<T, Image>
    {

        Func<List<T>, List<int>, PrintParameters, Image> print;

        public FichaClassic(List<T> sides, List<int> values, int plays_bySide, Func<List<T>, List<int>, PrintParameters, Image> print) : base(sides, values, plays_bySide)
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
            return print(sides, used_sides, pp);
        }

    }
    public class FichaInt3 : Ficha<int, Image>
    {

        Func<List<int>, List<int>, PrintParameters, Image> print;

        public FichaInt3(List<int> sides, List<int> values, int plays_bySide, Func<List<int>, List<int>, PrintParameters, Image> print) : base(sides, values, plays_bySide)
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
            return print(sides, used_sides, pp);
        }

    }
   
    public class DimGame: PrintParameters
    {

        public int Height;
        public int Width;
        public DimFicha s_ficha;

        public DimGame(int width, int height, DimFicha dimFicha)
        {
            this.Height = height;
            this.Width = width;
            this.s_ficha = dimFicha;
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
