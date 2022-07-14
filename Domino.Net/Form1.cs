using DominoPlatform;

namespace Domino.Net
{
    public partial class Form1 : Form
    {

        dynamic board;
        dynamic print;
        dynamic pass;
        dynamic winner;
        dynamic conditions;
        bool robar;

        Form2 Stats;

        DimGame dimGame;

        public Form1()
        {
            
            InitializeComponent();

            //opens the Settings Form
            Stats = new Form2(this);
            Stats.ShowDialog();
            
            dimGame = new DimGame(pictureBox1.Width, pictureBox1.Height, new DimFicha(40, 40));

            pictureBox1.Image = board.Print(print, dimGame);
            
        }

        private void timer1_Tick(object sender, EventArgs e)
        {

            if (board.PlayTurn(pass, robar, winner, conditions))
            {
                pictureBox1.Image = board.Print(print, dimGame);
                button1.Enabled = true;
                button2.Enabled = true;
                button3.Enabled = true;
            }
                

            else
            {
                timer1.Stop();
                button1.Enabled = false;
                button2.Enabled = false;
                button3.Enabled = false;
            }
                

        }

        /// <summary>
        /// Start the Board and store the necesary stuff for later 
        /// </summary>
        /// <typeparam name="F"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="generate"> how to generate the fichas for the game </param>
        /// <param name="distribute"> how to distribute the fichas to players at the beginning </param>
        /// <param name="robar"> play with this rule? </param>
        /// <param name="players"></param>
        /// <param name="print"> how to print the game </param>
        /// <param name="pass"> how to pass the turn </param>
        /// <param name="winner"> how to choose the winner </param>
        /// <param name="InitialHand"> how many fichas at the start hand of each player </param>
        /// <param name="conditions"> game rules </param>
        public void Initialize<F, T>(GenerateFichas<T, Image> generate, Distribute<T, Image> distribute, bool robar, List<PlayFicha<T, Image>> players,
            IBoardPrint<T, Image> print, PassTurn<T, Image> pass, Winner<T, Image> winner, int InitialHand, params IConditions<T, Image>[] conditions) where F: Ficha<T, Image>
        {
            
            List<DominoPlayer<T, Image>> dominoPlayers = new List<DominoPlayer<T, Image>>();

            foreach(var player in players)
            {
                dominoPlayers.Add(new DominoPlayer<T, Image>(player));
            }

            board = Board<T, Image>.StartGame(dominoPlayers, generate, distribute, InitialHand, pass);
            this.print = print;
            this.pass = pass;
            this.winner = winner;
            this.conditions = conditions;
            this.robar = robar;

            dimGame = new DimGame(pictureBox1.Width, pictureBox1.Height, new DimFicha(40, 40));

            timer1.Start();

        }

        private void startNewGameToolStripMenuItem_Click(object sender, EventArgs e)
        {

            bool wasWorking = false;

            wasWorking = timer1.Enabled;

            timer1.Stop();

            this.Stats.ShowDialog();

            if (wasWorking)
                timer1.Start();
        
        }

        //slow plays
        private void button1_Click(object sender, EventArgs e)
        {
            timer1.Interval += timer1.Interval / 2;
            if (!button3.Enabled) button3.Enabled = true;

        }

        //pause or play game
        private void button2_Click(object sender, EventArgs e)
        {
            if (timer1.Enabled)
            {

                timer1.Stop();

                button2.Text = "▶";
                if (!button3.Enabled) button3.Enabled = true;
            }

            else
            {
                timer1.Interval = 2000;
                timer1.Start();
                button2.Text = "⏸";
            }
        }

        //quick game
        private void button3_Click(object sender, EventArgs e)
        {
            timer1.Interval -= timer1.Interval/2;
            if (timer1.Interval <= 100)
            {
                button3.Enabled = false;
            }
        }
    }

}
