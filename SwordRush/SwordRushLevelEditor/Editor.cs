using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace SwordRushLevelEditor
{
    //Joshua Leong
    //Purpose: Allows the user to edit, save, and load maps from level files
    //Requirments: uses the form class
    public partial class Editor : Form
    {
        //fields
        private Color selectedColor;
        private PictureBox[,] map;
        private int[,] collisionGrid;
        private int[,] tileGrid;
        private bool collisionLayer;
        private bool saved;
        private int width;
        private int height;
        bool load;

        /// <summary>
        /// parameterized constructor for the editor form
        /// </summary>
        /// <param name="width">width of the map being created</param>
        /// <param name="height">height of the map being created</param>
        /// <param name="load">whether a map be being created or loaded</param>
        public Editor(int width, int height, bool load)
        {

            InitializeComponent();

            //initialize fields
            this.width = width;
            this.height = height;
            this.load = load;
            saved = true;
            map = new PictureBox[width, height];
            tileGrid = new int[width, height];
            collisionGrid = new int[width, height];
            collisionLayer = true;
            selectedColor = Color.Black;

            //set text and color of form and selected box
            this.Text = "Level Editor";
            pictureBoxSelected.BackColor = selectedColor;

            //creates a blank map when a new map is created
            if (!load)
            {
                int tileSize = (groupBoxMap.Size.Height - 25) / height;
                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        //create picture box tiles
                        map[j, i] = new PictureBox();
                        groupBoxMap.Controls.Add(map[j, i]);
                        map[j, i].Location = new Point(j * tileSize + 10, i * tileSize + 20);
                        map[j, i].Size = new Size(tileSize, tileSize);
                        map[j, i].Click += ChangeTileColor;
                        map[j, i].BackColor = selectedColor;
                        map[j, i].BorderStyle = BorderStyle.FixedSingle;

                        //populate data arrays
                        collisionGrid[j, i] = 0;


                    }
                }
            }
            else //loads a new map if load was selected
            {
                LoadMap();
                this.load = false;
            }

            //sets window to auto size
            this.AutoSize = true;
        }

        /// <summary>
        /// changes selected color to green
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonGreen_Click(object sender, EventArgs e)
        {
            selectedColor = Color.Green;
            pictureBoxSelected.BackColor = selectedColor;
        }

        /// <summary>
        /// chnages selected color to gray
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonGray_Click(object sender, EventArgs e)
        {
            selectedColor = Color.LightGray;
            pictureBoxSelected.BackColor = selectedColor;
        }

        /// <summary>
        /// changes selected color to brown
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonBrown_Click(object sender, EventArgs e)
        {
            selectedColor = Color.DarkGoldenrod;
            pictureBoxSelected.BackColor = selectedColor;
        }

        /// <summary>
        /// changes selected color to red
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonRed_Click(object sender, EventArgs e)
        {
            selectedColor = Color.Firebrick;
            pictureBoxSelected.BackColor = selectedColor;
        }

        /// <summary>
        /// changes selected color to light blue
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonBlue_Click(object sender, EventArgs e)
        {
            selectedColor = Color.LightSkyBlue;
            pictureBoxSelected.BackColor = selectedColor;
        }

        /// <summary>
        /// changes selected color to black
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonBlack_Click(object sender, EventArgs e)
        {
            selectedColor = Color.Black;
            pictureBoxSelected.BackColor = selectedColor;
        }

        /// <summary>
        /// sets the click tile to the selected color
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChangeTileColor(object? sender, EventArgs e)
        {
            if (sender is PictureBox)
            {
                PictureBox tileReference = (PictureBox)sender;
                tileReference.BackColor = selectedColor;
                if (!this.Text.Contains("*"))
                {
                    this.Text += "*";
                }
            }
        }

        /// <summary>
        /// opens file dialog and saves current map as a level file in specified location
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonSave_Click(object sender, EventArgs e)
        {
            //open save file dialog
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.Title = "Save a level file.";
            saveDialog.Filter = "Text Files|*.txt";
            saveDialog.FileName = "Level";
            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                using (StreamWriter writer = new StreamWriter(saveDialog.OpenFile()))
                {

                    //write each tile's color
                    for (int i = 0; i < collisionGrid.GetLength(1); i++)
                    {
                        for (int j = 0; j < collisionGrid.GetLength(0); j++)
                        {

                            writer.Write(collisionGrid[j, i]);

                            if (j < collisionGrid.GetLength(0) - 1)
                            {
                                writer.Write(",");
                            }
                        }
                        writer.Write('\n');
                    }

                    writer.Write('\n');


                    for (int i = 0; i < tileGrid.GetLength(1); i++)
                    {
                        for (int j = 0; j < tileGrid.GetLength(0); j++)
                        {

                            writer.Write(tileGrid[j, i]);

                            if (j < tileGrid.GetLength(0) - 1)
                            {
                                writer.Write(",");
                            }
                        }
                        if (i < tileGrid.GetLength(1) - 1)
                        {
                            writer.Write("\n");
                        }
                    }

                }
                //show successful save message
                MessageBox.Show("File saved successfully", "File saved");

                //change window text
                if (Text.Contains("*"))
                {
                    Text = Text.Substring(0, Text.IndexOf("*"));
                }
            }
        }

        /// <summary>
        /// runs load method when load button is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonLoad_Click(object sender, EventArgs e)
        {

            LoadMap();

        }

        /// <summary>
        /// checks if there are unsaved changes when closing form and warns user
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Editor_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!saved)
            {
                DialogResult result = MessageBox.Show("There are unsaved changes. Are you sure you want to quit?",
                "Unsaved changes", MessageBoxButtons.YesNo);
                if (result == DialogResult.No)
                {
                    e.Cancel = true;
                }
            }

        }


        /// <summary>
        /// loads a map from a file and replaces old map
        /// </summary>
        private void LoadMap()
        {


            //open file dialog
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Title = "Open a level file.";
            fileDialog.Filter = "Text Files|*.txt";
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                //delete old array
                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        groupBoxMap.Controls.Remove(map[j, i]);
                        collisionGrid[j, i] = 0;
                        tileGrid[j, i] = 0;
                    }
                }



                //read file
                using (StreamReader reader = new StreamReader(fileDialog.OpenFile()))
                {
                    //create new picture box array
                    map = new PictureBox[width, height];
                    int tileSize = (groupBoxMap.Size.Height - 25) / height;
                    for (int i = 0; i < height; i++)
                    {
                        for (int j = 0; j < width; j++)
                        {
                            map[j, i] = new PictureBox();
                            groupBoxMap.Controls.Add(map[j, i]);
                            map[j, i].Location = new Point(j * tileSize + 10, i * tileSize + 20);
                            map[j, i].Size = new Size(tileSize, tileSize);
                            map[j, i].Click += ChangeTileColor;
                            map[j, i].BorderStyle = BorderStyle.FixedSingle;
                        }
                    }




                    //color new tiles for collision layer
                    string[][] collisions = new string[height][];
                    for (int i = 0; i < height; i++)
                    {
                        collisions[i] = reader.ReadLine()!.Split(",");
                        for (int j = 0; j < width; j++)
                        {
                            if (collisions[i][j] == "4")
                            {
                                map[j, i].BackColor = Color.Green;
                            }
                            else if (collisions[i][j] == "1")
                            {
                                map[j, i].BackColor = Color.LightGray;
                            }
                            else if (collisions[i][j] == "5")
                            {
                                map[j, i].BackColor = Color.DarkGoldenrod;
                            }
                            else if (collisions[i][j] == "2")
                            {
                                map[j, i].BackColor = Color.Firebrick;
                            }
                            else if (collisions[i][j] == "3")
                            {
                                map[j, i].BackColor = Color.LightSkyBlue;
                            }
                            else if (collisions[i][j] == "0")
                            {
                                map[j, i].BackColor = Color.Black;
                            }
                            else if (collisions[i][j] == "6")
                            {
                                map[j, i].BackColor = Color.Purple;
                            }
                            else if (collisions[i][j] == "7")
                            {
                                map[j, i].BackColor = Color.Orange;
                            }
                            else if (collisions[i][j] == "8")
                            {
                                map[j, i].BackColor = Color.Teal;
                            }
                            else if (collisions[i][j] == "9")
                            {
                                map[j, i].BackColor = Color.Gold;
                            }
                            else if (collisions[i][j] == "10")
                            {
                                map[j, i].BackColor = Color.Tan;
                            }
                            else if (collisions[i][j] == "11")
                            {
                                map[j, i].BackColor = Color.Pink;
                            }
                            else if (collisions[i][j] == "12")
                            {
                                map[j, i].BackColor = Color.Aquamarine;
                            }
                            else if (collisions[i][j] == "13")
                            {
                                map[j, i].BackColor = Color.Indigo;
                            }
                            else if (collisions[i][j] == "14")
                            {
                                map[j, i].BackColor = Color.Olive;
                            }

                            collisionGrid[j, i] = Convert.ToInt32(collisions[i][j]);
                        }
                    }

                    reader.ReadLine();


                    //get tile layer
                    string[][] tiles = new string[height][];
                    for (int i = 0; i < height; i++)
                    {
                        tiles[i] = reader.ReadLine()!.Split(",");
                        for (int j = 0; j < width; j++)
                        {
                            tileGrid[j, i] = Convert.ToInt32(tiles[i][j]);
                        }
                    }

                }

                //change window text
                if (Text.Contains("*"))
                {
                    Text = Text.Substring(0, Text.IndexOf("*"));
                }
                if (!Text.Contains("-"))
                {
                    Text += $" - {fileDialog.SafeFileName}";
                }

                //show completion message
                MessageBox.Show("File loaded successfully", "File loaded");

            }
            else if (load)//if load is canceled close form
            {
                this.Close();
            }
        }


        private void label1_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// changes the layer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonLayerChange_Click(object sender, EventArgs e)
        {
            if (collisionLayer)
            {
                buttonGray.Text = "Top Wall";
                buttonGreen.Text = "Bot Wall";
                buttonBrown.Text = "Top Left";
                buttonRed.Text = "Top Right";
                buttonPurple.Text = "Bot Left";
                buttonBlue.Text = "Bot Right";
                buttonOrange.Text = "Left Wall";
                buttonTeal.Text = "Right Wall";
                buttonGold.Text = "lBot corner";
                buttonTan.Text = "rBot corner";
                buttonPink.Text = "Wall Top";
                buttonAqua.Text = "One Square";
                buttonIndigo.Text = "long Bot l";
                buttonOlive.Text = "long Bot r";
                labelLayer.Text = "Tiles";
            }
            else
            {
                buttonGray.Text = "Wall";
                buttonGreen.Text = "Chest";
                buttonBrown.Text = "LR Enemy";
                buttonRed.Text = "SR Enemy";
                buttonPurple.Text = "N/A";
                buttonBlue.Text = "Player";
                buttonOrange.Text = "N/A";
                buttonTeal.Text = "N/A";
                buttonGold.Text = "N/A";
                buttonTan.Text = "N/A";
                buttonPink.Text = "N/A";
                buttonAqua.Text = "N/A";
                buttonIndigo.Text = "N/A";
                buttonOlive.Text = "N/A";
                labelLayer.Text = "Collisions";
            }
            ChangeLayer();

        }

        /// <summary>
        /// updates the map display to 
        /// </summary>
        private void ChangeLayer()
        {
            if (collisionLayer)
            {
                for (int i = 0; i < map.GetLength(1); i++)
                {
                    for (int j = 0; j < map.GetLength(0); j++)
                    {
                        //save collision layer
                        if (map[j, i].BackColor == Color.Green)
                        {
                            collisionGrid[j, i] = 4;
                        }
                        else if (map[j, i].BackColor == Color.LightGray)
                        {
                            collisionGrid[j, i] = 1;
                        }
                        else if (map[j, i].BackColor == Color.DarkGoldenrod)
                        {
                            collisionGrid[j, i] = 5;
                        }
                        else if (map[j, i].BackColor == Color.Firebrick)
                        {
                            collisionGrid[j, i] = 2;
                        }
                        else if (map[j, i].BackColor == Color.LightSkyBlue)
                        {
                            collisionGrid[j, i] = 3;
                        }
                        else if (map[j, i].BackColor == Color.Black)
                        {
                            collisionGrid[j, i] = 0;
                        }
                        else if (map[j, i].BackColor == Color.Purple)
                        {
                            collisionGrid[j, i] = 0;
                        }
                        else if (map[j, i].BackColor == Color.Orange)
                        {
                            collisionGrid[j, i] = 0;
                        }
                        else if (map[j, i].BackColor == Color.Teal)
                        {
                            collisionGrid[j, i] = 0;
                        }
                        else if (map[j, i].BackColor == Color.Gold)
                        {
                            collisionGrid[j, i] = 0;
                        }
                        else if (map[j, i].BackColor == Color.Tan)
                        {
                            collisionGrid[j, i] = 0;
                        }
                        else if (map[j, i].BackColor == Color.Pink)
                        {
                            collisionGrid[j, i] = 0;
                        }
                        else if (map[j, i].BackColor == Color.Aquamarine)
                        {
                            collisionGrid[j, i] = 0;
                        }
                        else if (map[j, i].BackColor == Color.Indigo)
                        {
                            collisionGrid[j, i] = 0;
                        }
                        else if (map[j, i].BackColor == Color.Olive)
                        {
                            collisionGrid[j, i] = 0;
                        }

                        //put out tile layer
                        if (tileGrid[j, i] == 4)
                        {
                            map[j, i].BackColor = Color.Green;
                        }
                        else if (tileGrid[j, i] == 1)
                        {
                            map[j, i].BackColor = Color.LightGray;
                        }
                        else if (tileGrid[j, i] == 5)
                        {
                            map[j, i].BackColor = Color.DarkGoldenrod;
                        }
                        else if (tileGrid[j, i] == 2)
                        {
                            map[j, i].BackColor = Color.Firebrick;
                        }
                        else if (tileGrid[j, i] == 3)
                        {
                            map[j, i].BackColor = Color.LightSkyBlue;
                        }
                        else if (tileGrid[j, i] == 0)
                        {
                            map[j, i].BackColor = Color.Black;
                        }
                        else if (tileGrid[j, i] == 6)
                        {
                            map[j, i].BackColor = Color.Purple;
                        }
                        else if (tileGrid[j, i] == 7)
                        {
                            map[j, i].BackColor = Color.Orange;
                        }
                        else if (tileGrid[j, i] == 8)
                        {
                            map[j, i].BackColor = Color.Teal;
                        }
                        else if (tileGrid[j, i] == 9)
                        {
                            map[j, i].BackColor = Color.Gold;
                        }
                        else if (tileGrid[j, i] == 10)
                        {
                            map[j, i].BackColor = Color.Tan;
                        }
                        else if (tileGrid[j, i] == 11)
                        {
                            map[j, i].BackColor = Color.Pink;
                        }
                        else if (tileGrid[j, i] == 12)
                        {
                            map[j, i].BackColor = Color.Aquamarine;
                        }
                        else if (tileGrid[j, i] == 13)
                        {
                            map[j, i].BackColor = Color.Indigo;
                        }
                        else if (tileGrid[j, i] == 14)
                        {
                            map[j, i].BackColor = Color.Olive;
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < map.GetLength(1); i++)
                {
                    for (int j = 0; j < map.GetLength(0); j++)
                    {
                        //save tile layer
                        if (map[j, i].BackColor == Color.Green)
                        {
                            tileGrid[j, i] = 4;
                        }
                        else if (map[j, i].BackColor == Color.LightGray)
                        {
                            tileGrid[j, i] = 1;
                        }
                        else if (map[j, i].BackColor == Color.DarkGoldenrod)
                        {
                            tileGrid[j, i] = 5;
                        }
                        else if (map[j, i].BackColor == Color.Firebrick)
                        {
                            tileGrid[j, i] = 2;
                        }
                        else if (map[j, i].BackColor == Color.LightSkyBlue)
                        {
                            tileGrid[j, i] = 3;
                        }
                        else if (map[j, i].BackColor == Color.Black)
                        {
                            tileGrid[j, i] = 0;
                        }
                        else if (map[j, i].BackColor == Color.Purple)
                        {
                            tileGrid[j, i] = 6;
                        }
                        else if (map[j, i].BackColor == Color.Orange)
                        {
                            tileGrid[j, i] = 7;
                        }
                        else if (map[j, i].BackColor == Color.Teal)
                        {
                            tileGrid[j, i] = 8;
                        }
                        else if (map[j, i].BackColor == Color.Gold)
                        {
                            tileGrid[j, i] = 9;
                        }
                        else if (map[j, i].BackColor == Color.Tan)
                        {
                            tileGrid[j, i] = 10;
                        }
                        else if (map[j, i].BackColor == Color.Pink)
                        {
                            tileGrid[j, i] = 11;
                        }
                        else if (map[j, i].BackColor == Color.Aquamarine)
                        {
                            tileGrid[j, i] = 12;
                        }
                        else if (map[j, i].BackColor == Color.Indigo)
                        {
                            tileGrid[j, i] = 13;
                        }
                        else if (map[j, i].BackColor == Color.Olive)
                        {
                            tileGrid[j, i] = 14;
                        }

                        //put out collision layer
                        if (collisionGrid[j, i] == 4)
                        {
                            map[j, i].BackColor = Color.Green;
                        }
                        else if (collisionGrid[j, i] == 1)
                        {
                            map[j, i].BackColor = Color.LightGray;
                        }
                        else if (collisionGrid[j, i] == 5)
                        {
                            map[j, i].BackColor = Color.DarkGoldenrod;
                        }
                        else if (collisionGrid[j, i] == 2)
                        {
                            map[j, i].BackColor = Color.Firebrick;
                        }
                        else if (collisionGrid[j, i] == 3)
                        {
                            map[j, i].BackColor = Color.LightSkyBlue;
                        }
                        else if (collisionGrid[j, i] == 0)
                        {
                            map[j, i].BackColor = Color.Black;
                        }
                        else if (collisionGrid[j, i] == 6)
                        {
                            map[j, i].BackColor = Color.Purple;
                        }
                        else if (collisionGrid[j, i] == 7)
                        {
                            map[j, i].BackColor = Color.Orange;
                        }
                        else if (collisionGrid[j, i] == 8)
                        {
                            map[j, i].BackColor = Color.Teal;
                        }
                        else if (collisionGrid[j, i] == 9)
                        {
                            map[j, i].BackColor = Color.Gold;
                        }
                        else if (collisionGrid[j, i] == 10)
                        {
                            map[j, i].BackColor = Color.Tan;
                        }
                        else if (collisionGrid[j, i] == 11)
                        {
                            map[j, i].BackColor = Color.Pink;
                        }
                        else if (collisionGrid[j, i] == 12)
                        {
                            map[j, i].BackColor = Color.Aquamarine;
                        }
                        else if (collisionGrid[j, i] == 13)
                        {
                            map[j, i].BackColor = Color.Indigo;
                        }
                        else if (collisionGrid[j, i] == 14)
                        {
                            map[j, i].BackColor = Color.Olive;
                        }
                    }
                }
            }

            collisionLayer = !collisionLayer;
        }

        private void buttonPurple_Click(object sender, EventArgs e)
        {
            selectedColor = Color.Purple;
            pictureBoxSelected.BackColor = selectedColor;
        }

        private void buttonOrange_Click(object sender, EventArgs e)
        {
            selectedColor = Color.Orange;
            pictureBoxSelected.BackColor = selectedColor;
        }

        private void buttonTeal_Click(object sender, EventArgs e)
        {
            selectedColor = Color.Teal;
            pictureBoxSelected.BackColor = selectedColor;
        }

        private void buttonGold_Click(object sender, EventArgs e)
        {
            selectedColor = Color.Gold;
            pictureBoxSelected.BackColor = selectedColor;
        }

        private void buttonTan_Click(object sender, EventArgs e)
        {
            selectedColor = Color.Tan;
            pictureBoxSelected.BackColor = selectedColor;
        }

        private void buttonPink_Click(object sender, EventArgs e)
        {
            selectedColor = Color.Pink;
            pictureBoxSelected.BackColor = selectedColor;
        }

        private void buttonAqua_Click(object sender, EventArgs e)
        {
            selectedColor = Color.Aquamarine;
            pictureBoxSelected.BackColor = selectedColor;
        }

        private void buttonIndigo_Click(object sender, EventArgs e)
        {
            selectedColor = Color.Indigo;
            pictureBoxSelected.BackColor = selectedColor;
        }

        private void buttonOlive_Click(object sender, EventArgs e)
        {
            selectedColor = Color.Olive;
            pictureBoxSelected.BackColor = selectedColor;
        }
    }
}
