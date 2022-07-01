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
        
        public Form1()
        {
            
            InitializeComponent();

            Stats = new Form2(this);
            Stats.ShowDialog();
            
            pictureBox1.Image = board.Print(print, new DimFicha(80, 47));
            timer1.Start();

        }

        private void timer1_Tick(object sender, EventArgs e)
        {

            if (board.PlayTurn(pass, robar, winner, conditions))
            {
                pictureBox1.Image = board.Print(print, new DimFicha(80, 47));
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

        public void Initialize<F, T>(GenerateFichas<T, Image> generate, Distribute<T, Image> distribute, bool robar, List<PlayFicha<T, Image>> players, IBoardPrint<T, Image> print, PassTurn<T, Image> pass, Winner<T, Image> winner, int InitialHand, params IConditions<T, Image>[] conditions) where F: Ficha<T, Image>
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

        }

        private void startNewGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Stats.ShowDialog();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            timer1.Interval += timer1.Interval / 2;
            if (!button3.Enabled) button3.Enabled = true;

        }

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

        private void button3_Click(object sender, EventArgs e)
        {
            timer1.Interval -= timer1.Interval/2;
            if (timer1.Interval <= 250)
            {
                button3.Enabled = false;
            }
        }
    }

}
