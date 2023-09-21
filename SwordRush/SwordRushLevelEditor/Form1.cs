namespace SwordRushLevelEditor
{
    //Joshua Leong
    //Purpose: Creates a window that allows the user to choose whether they want to create a new level or edit an existing one from a file
    //Requirements: uses the form class
    public partial class Form1 : Form
    {
        //fields
        private int width;
        private int height;

        /// <summary>
        /// constructor for the form1 class that sets width and height as well as the text of the form
        /// </summary>
        public Form1()
        {
            InitializeComponent();
            width = 20;
            height = 12;
            Text = "Level Editor";
        }

        /// <summary>
        /// checks if the width and height given in the text boxes are valid and then if there are no errors creates a level editor of the specified size
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonCreate_Click(object sender, EventArgs e)
        {
            //open editor
            Editor editor = new Editor(width, height, false);
            editor.Show();
        }

        /// <summary>
        /// opens a file dialog and allows the user to open a level editor with the chosen file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonLoad_Click(object sender, EventArgs e)
        {
            //open editor
            Editor editor = new Editor(width, height, true);
            if (!editor.IsDisposed) //checks if load was canceled or not
            {
                editor.Show();
            }


        }
    }
}