using System.Drawing.Drawing2D;
using System.Text;

namespace BubbleBreakerGame
{
    public partial class BubbleBreaker : Form
    {

        enum Colors
        {
            None,
            Red,
            Green,
            Yellow,
            Blue,
            Purple
        };

        const int NUM_BUBBLES = 20;
        const int BUBBLE_SIZE = 40;
        Colors[,] colors;
        Random rand;
        int score;
        bool[,] isSelected;
        int numOfSelectedBubbles;
        Scores scores;


        public BubbleBreaker()
        {
            InitializeComponent();
            rand = new Random();
            numOfSelectedBubbles = 0;
            score = 0;
            colors = new Colors[NUM_BUBBLES, NUM_BUBBLES];
            isSelected = new bool[NUM_BUBBLES, NUM_BUBBLES];
            lblInfo.BackColor = Color.White;

        }

        private void BubbleBreaker_Load(object sender, EventArgs e)
        {
            init();
        }

        private void init()
        {
            SetClientSizeCore(NUM_BUBBLES * BUBBLE_SIZE, NUM_BUBBLES * BUBBLE_SIZE);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            BackColor = Color.Black;
            DoubleBuffered = true;
            txtName.Visible = false;
            btnName.Visible = false;
            btnName.Text = "Enter Your Name: ";

            txtName.Width = ClientSize.Width < 100 ? ClientSize.Width : ClientSize.Width / 2;
            btnName.Width = ClientSize.Width < 100 ? ClientSize.Width : ClientSize.Width / 2;

            txtName.Location = new Point((ClientSize.Width - btnName.Width) / 2, btnName.Height);
            btnName.Location = new Point((ClientSize.Width - btnName.Width) / 2, btnName.Height + 20);

            Start();
        }


        private void GameOver()
        {
            scores = new Scores(score, NUM_BUBBLES);
            StringBuilder sb = new StringBuilder();
            sb.Append("*** GAME OVER***");
            sb.Append("\n");
            sb.Append("*** Top 3 socres***\n");
            sb.Append(scores.GetTopThreeScores() + "\n");
            sb.Append("\n");
            sb.Append(scores.GetScoreMessage());


            MessageBox.Show(sb.ToString());
            txtName.Visible = true;
            btnName.Visible = true;
        }

        private void Start()
        {
            for (int row = 0; row < NUM_BUBBLES; row++)
            {
                for (int col = 0; col < NUM_BUBBLES; col++)
                {
                    colors[row, col] = (Colors)rand.Next(1, 6);
                }
            }
            this.Text = score + " points";
        }

        private void Form_Paint(object sender, PaintEventArgs e)
        {
            for (int row = 0; row < NUM_BUBBLES; row++)
            {
                for (int col = 0; col < NUM_BUBBLES; col++)
                {
                    Color bubbleColor = Color.Empty;
                    var xPos = col;
                    var yPos = row;
                    var isBubble = true;
                    switch (colors[row, col])
                    {
                        case Colors.Red:
                            bubbleColor = Color.Red;
                            break;
                        case Colors.Yellow:
                            bubbleColor = Color.Yellow;
                            break;

                        case Colors.Green:
                            bubbleColor = Color.Green;
                            break;
                        case Colors.Blue:
                            bubbleColor = Color.Blue;
                            break;
                        case Colors.Purple:
                            bubbleColor = Color.Purple;
                            break;
                        default:
                            e.Graphics.FillRectangle(Brushes.Black, xPos * BUBBLE_SIZE,
                                yPos * BUBBLE_SIZE, BUBBLE_SIZE, BUBBLE_SIZE);
                            isBubble = false;
                            break;
                    }
                    if (isBubble)
                    {
                        e.Graphics.FillEllipse(new LinearGradientBrush(
                                               new Point(row * BUBBLE_SIZE, col * BUBBLE_SIZE),
                                               new Point(row * BUBBLE_SIZE + BUBBLE_SIZE, col * BUBBLE_SIZE + BUBBLE_SIZE),
                                               Color.White, bubbleColor),
                                               xPos * BUBBLE_SIZE,
                                               yPos * BUBBLE_SIZE,
                                               BUBBLE_SIZE, BUBBLE_SIZE);

                        if (isSelected[row, col])
                        {
                            // left outline
                            if (col > 0 && colors[row, col] != colors[row, col - 1])
                            {
                                e.Graphics.DrawLine(Pens.White, xPos * BUBBLE_SIZE, yPos * BUBBLE_SIZE,
                                    xPos * BUBBLE_SIZE, yPos * BUBBLE_SIZE + BUBBLE_SIZE);
                            }

                            // right outline
                            if (col < NUM_BUBBLES - 1 && colors[row, col] != colors[row, col + 1])
                            {
                                e.Graphics.DrawLine(Pens.White, xPos * BUBBLE_SIZE + BUBBLE_SIZE, yPos * BUBBLE_SIZE,
                                    xPos * BUBBLE_SIZE + BUBBLE_SIZE, yPos * BUBBLE_SIZE + BUBBLE_SIZE);
                            }
                            // top outline
                            if (row > 0 && colors[row, col] != colors[row - 1, col])
                            {
                                e.Graphics.DrawLine(Pens.White, xPos * BUBBLE_SIZE, yPos * BUBBLE_SIZE,
                                    xPos * BUBBLE_SIZE + BUBBLE_SIZE, yPos * BUBBLE_SIZE);
                            }
                            // bootom outline
                            if (row < NUM_BUBBLES - 1 && colors[row, col] != colors[row + 1, col])
                            {
                                e.Graphics.DrawLine(Pens.White, xPos * BUBBLE_SIZE, yPos * BUBBLE_SIZE + BUBBLE_SIZE,
                                    xPos * BUBBLE_SIZE + BUBBLE_SIZE, yPos * BUBBLE_SIZE + BUBBLE_SIZE);
                            }

                        }
                    }
                }

            }
        }

        private void Form_MouseDown(object sender, MouseEventArgs e)
        {
            var x = Convert.ToInt32(e.X / BUBBLE_SIZE);
            var y = Convert.ToInt32(e.Y / BUBBLE_SIZE);
            var row = y;
            var col = x;

            if (isSelected[row, col] && numOfSelectedBubbles > 1)
            {
                score += Convert.ToInt32(lblInfo.Text);
                this.Text = score + " points";
                RemoveBubbles();
                ClearSelected();
                MoveBubblesDown();
                MoveBubblesRight();

                if (!HasMoreMoves())
                {
                    GameOver();
                }
            }
            else
            {
                ClearSelected();
                if (colors[row, col] > Colors.None)
                {
                    HighlightNeighbors(row, col);

                    if (numOfSelectedBubbles > 1)
                    {
                        SetLabel(numOfSelectedBubbles, x, y);
                    }
                }
            }
        }

        private void RemoveBubbles()
        {
            for (int row = 0; row < NUM_BUBBLES; row++)
            {
                for (int col = 0; col < NUM_BUBBLES; col++)
                {
                    if (isSelected[row, col])
                    {
                        colors[row, col] = Colors.None;
                    }
                }
            }

            this.Invalidate();
            Application.DoEvents();

        }


        private void ClearSelected()
        {
            for (int row = 0; row < NUM_BUBBLES; row++)
            {
                for (int col = 0; col < NUM_BUBBLES; col++)
                {
                    isSelected[row, col] = false;
                }
            }

            numOfSelectedBubbles = 0;
            lblInfo.Visible = false;
        }

        private bool HasMoreMoves()
        {
            for (int row = 0; row < NUM_BUBBLES; row++)
            {
                for (int col = 0; col < NUM_BUBBLES; col++)
                {
                    if (colors[row, col] > Colors.None)
                    {
                        if (col < NUM_BUBBLES - 1 && colors[row, col] == colors[row, col + 1])
                        {
                            return true;
                        }

                        if (row < NUM_BUBBLES - 1 && colors[row, col] == colors[row + 1, col])
                            return true;
                    }
                }
            }
            return false;
        }

        private void SetLabel(int numOfBubbles, int x, int y)
        {
            var value = numOfBubbles * (numOfBubbles - 1);
            lblInfo.Text = value.ToString();

            lblInfo.Left = x * BUBBLE_SIZE + BUBBLE_SIZE;
            lblInfo.Top = y * BUBBLE_SIZE + BUBBLE_SIZE;

            if (lblInfo.Left > this.ClientSize.Width / 2)
                lblInfo.Left -= BUBBLE_SIZE;

            if (lblInfo.Top > this.ClientSize.Height / 2)
                lblInfo.Top -= BUBBLE_SIZE;

            lblInfo.Visible = true;
        }


        private void MoveBubblesDown()
        {
            for (int col = 0; col < NUM_BUBBLES; col++)
            {
                var noneColorBubblePosition = NUM_BUBBLES - 1;
                var foundNoneColor = false;

                for (int row = NUM_BUBBLES - 1; row >= 0; row--)
                {
                    if (colors[row, col] == Colors.None)
                        foundNoneColor = true;

                    if (colors[row, col] != Colors.None && !foundNoneColor)
                        noneColorBubblePosition--;

                    if (colors[row, col] != Colors.None && foundNoneColor)
                    {
                        colors[noneColorBubblePosition, col] = colors[row, col];
                        noneColorBubblePosition--;
                    }
                }

                for (int r = noneColorBubblePosition; r >= 0; r--)
                {
                    colors[r, col] = Colors.None;

                }
            }

            this.Invalidate();
            Application.DoEvents();
        }


        private void MoveBubblesRight()
        {
            for (int row = 0; row < NUM_BUBBLES; row++)
            {
                var noneColorBubblePosition = NUM_BUBBLES - 1;
                var foundNoneColor = false;

                for (int col = NUM_BUBBLES - 1; col >= 0; col--)
                {
                    if (colors[row, col] == Colors.None)
                        foundNoneColor = true;

                    if (colors[row, col] != Colors.None && !foundNoneColor)
                        noneColorBubblePosition--;

                    if (colors[row, col] != Colors.None && foundNoneColor)
                    {
                        colors[row, noneColorBubblePosition] = colors[row, col];
                        noneColorBubblePosition--;
                    }
                }

                for (int c = noneColorBubblePosition; c >= 0; c--)
                {
                    colors[row, c] = Colors.None;

                }
            }

            this.Invalidate();
            Application.DoEvents();
            GenerateBubbles();
        }



        private void GenerateBubbles()
        {
            if (colors[NUM_BUBBLES - 1, 0] == Colors.None)
            {
                for (int row = NUM_BUBBLES - 1; row >= 0; row--)
                {
                    colors[row, 0] = (Colors)rand.Next(1, 6);

                }

                this.Invalidate();
                Application.DoEvents();
                MoveBubblesRight();

            }

        }

        private void HighlightNeighbors(int row, int col)
        {
            isSelected[row, col] = true;
            numOfSelectedBubbles++;
            int[,] positionTracking = new int[NUM_BUBBLES, NUM_BUBBLES];
            var positionCounter = 1;
            positionTracking[row, col] = positionCounter;
            var rowIndex = row;
            var colIndex = col;

            while (positionCounter > 0)
            {   //move up
                if (rowIndex > 0 && colors[rowIndex, colIndex] == colors[rowIndex - 1, colIndex] &&
                    !isSelected[rowIndex - 1, colIndex])
                {
                    isSelected[rowIndex - 1, colIndex] = true;
                    numOfSelectedBubbles++;
                    positionCounter++;
                    positionTracking[rowIndex - 1, colIndex] = positionCounter;
                    rowIndex--;
                }
                //move down
                else if (rowIndex < NUM_BUBBLES - 1 && colors[rowIndex, colIndex] == colors[rowIndex + 1, colIndex] &&
                    !isSelected[rowIndex + 1, colIndex])
                {
                    isSelected[rowIndex + 1, colIndex] = true;
                    numOfSelectedBubbles++;
                    positionCounter++;
                    positionTracking[rowIndex + 1, colIndex] = positionCounter;
                    rowIndex++;
                }

                // move left
                else if (colIndex > 0 && colors[rowIndex, colIndex] == colors[rowIndex, colIndex - 1] &&
                    !isSelected[rowIndex, colIndex - 1])
                {
                    isSelected[rowIndex, colIndex - 1] = true;
                    numOfSelectedBubbles++;
                    positionCounter++;
                    positionTracking[rowIndex, colIndex - 1] = positionCounter;
                    colIndex--;
                }

                //move right
                else if (colIndex < NUM_BUBBLES - 1 && colors[rowIndex, colIndex] == colors[rowIndex, colIndex + 1] &&
                   !isSelected[rowIndex, colIndex + 1])
                {
                    isSelected[rowIndex, colIndex + 1] = true;
                    numOfSelectedBubbles++;
                    positionCounter++;
                    positionTracking[rowIndex, colIndex + 1] = positionCounter;
                    colIndex++;
                }
                else
                {
                    positionCounter--;
                    for (int r = 0; r < NUM_BUBBLES; r++)
                    {
                        for (int c = 0; c < NUM_BUBBLES; c++)
                        {
                            if (positionTracking[r, c] == positionCounter + 1)
                                positionTracking[r, c] = 0;

                            if (positionTracking[r, c] == positionCounter)
                            {
                                rowIndex = r;
                                colIndex = c;
                            }
                        }
                    }
                }
            }
        }

        private void btnName_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Please enter a valid name");
                return;
            }
            scores.WriteScore(txtName.Text);
            numOfSelectedBubbles = 0;
            txtName.Text = "";
            score = 0;
            init();
            
        }
    }
}

