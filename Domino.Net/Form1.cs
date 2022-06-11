using DominoPlatform;
using GamePlatform;

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

        public Form1()
        {
            
            InitializeComponent();

            Form2 f2 = new Form2(this);
            f2.ShowDialog();

            pictureBox1.Image = board.Print(print, new DimFicha(80, 47));
            timer1.Start();

        }

        private void timer1_Tick(object sender, EventArgs e)
        {

            if (board.PlayTurn(pass, robar, winner, conditions))
                pictureBox1.Image = board.Print(print, new DimFicha(80, 47));

            else
                timer1.Stop();

        }

        public void Initialize<F, T>(GenerateFichas<T, Image> generate, Distribute<T, Image> distribute, bool robar, List<PlayFicha<T, Image>> players, IEnvironmentPrint<TreeN<T, Image>, DominoPlayer<T, Image>, List<Ficha<T, Image>>, Image> print, PassTurn<T, Image> pass, Winner<T, Image> winner, int InitialHand, params IConditions<T, Image>[] conditions) where F: Ficha<T, Image>
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

    }

}
