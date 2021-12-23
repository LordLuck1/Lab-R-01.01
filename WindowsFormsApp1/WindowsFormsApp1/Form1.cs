using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        bool drawing;
        GraphicsPath currentPath;
        Point oldLocation;
        Color historyColor;
        int historyCounter;
        public Pen currentPen;       
        List<Image> History;


        public Form1()
        {
            InitializeComponent();
            drawing = false;
            currentPen = new Pen(Color.Black);     
            currentPen.Width = trackBarPen.Value;
            History = new List<Image>();
        }
        private void PicDrawingSurface_MouseDown(object sender, MouseEventArgs e)
        {
            
            if (picDrawingSurface.Image == null)
            {
                MessageBox.Show("Сначала создайте новый файл!"); return;
            }
            if (e.Button == MouseButtons.Left)
            {
                drawing = true; oldLocation = e.Location;
                currentPath = new GraphicsPath();
            }

        }

        private void SaveToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            SaveFileDialog SaveDlg = new SaveFileDialog();
            SaveDlg.Filter = "JPEG Image|*.jpg|Bitmap Image|*.bmp|GIF Image|*.gif|PNG Image|*.png";
            SaveDlg.Title = "Save an Image File";
            SaveDlg.FilterIndex = 4;
            SaveDlg.ShowDialog();
            Graphics g = Graphics.FromImage(picDrawingSurface.Image);

            g.Clear(Color.White);
            g.DrawImage(picDrawingSurface.Image, 0, 0, 750, 500);


            if (SaveDlg.FileName != "")
            {
                System.IO.FileStream fs = (System.IO.FileStream)SaveDlg.OpenFile();

                switch (SaveDlg.FilterIndex)
                {
                    case 1: this.picDrawingSurface.Image.Save(fs, ImageFormat.Jpeg); break;
                    case 2: this.picDrawingSurface.Image.Save(fs, ImageFormat.Bmp); break;
                    case 3:
                        this.picDrawingSurface.Image.Save(fs, ImageFormat.Gif); break;
                    case 4:
                        this.picDrawingSurface.Image.Save(fs, ImageFormat.Png); break;
                }
                fs.Close();
            }
            if (picDrawingSurface.Image != null)
            {
                var result = MessageBox.Show("Сохранить текущее изображение перед созданием нового рисунка?", "Предупреждение", MessageBoxButtons.YesNoCancel);

                switch (result)
                {
                    case DialogResult.No: break;
                    case DialogResult.Yes: SaveToolStripMenuItem1_Click(sender, e); break;
                    case DialogResult.Cancel: return;
                }
            }

        }

        private void OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog OP = new OpenFileDialog();
            OP.Filter = "JPEG Image|*.jpg|Bitmap Image|*.bmp|GIF Image|*.gif|PNG Image|*.png";
            OP.Title = "Open an Image File";
            OP.FilterIndex = 1; //По умолчанию будет выбрано первое расширение *.jpg И, когда пользователь укажет нужный путь к картинке, ее нужно будет загрузить в PictureBox: 
            if (OP.ShowDialog() != DialogResult.Cancel) picDrawingSurface.Load(OP.FileName);
            picDrawingSurface.AutoSize = true;

        }

        private void PicDrawingSurface_MouseUp(object sender, MouseEventArgs e)
        {
            drawing = false; try
            {
                currentPath.Dispose();
            }
            catch { }
            History.RemoveRange(historyCounter + 1, History.Count - historyCounter - 1);
            History.Add(new Bitmap(picDrawingSurface.Image)); if (historyCounter + 1 < 10) historyCounter++;
            if (History.Count - 1 == 10) History.RemoveAt(0);
            drawing = false; try
            {
                currentPath.Dispose();
            }
            catch { };


        }

        private void PicDrawingSurface_MouseMove(object sender, MouseEventArgs e)
        {
            if (drawing)
            {
                Graphics g = Graphics.FromImage(picDrawingSurface.Image); currentPath.AddLine(oldLocation, e.Location);
                g.DrawPath(currentPen, currentPath); oldLocation = e.Location;
                g.Dispose();
                picDrawingSurface.Invalidate();
            }
            label1_XY.Text = e.X.ToString() + ", " + e.Y.ToString();
        }

        private void TrackBar1_Scroll(object sender, EventArgs e)
        {
            currentPen.Width = trackBarPen.Value;
        }

        private void NewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            History.Clear(); historyCounter = 0;
            Bitmap pic = new Bitmap(750, 500);
            picDrawingSurface.Image = pic;
            History.Add(new Bitmap(picDrawingSurface.Image));
        }

        private void UndoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (History.Count != 0 && historyCounter != 0)
            {
                picDrawingSurface.Image = new Bitmap(History[--historyCounter]);
            }
            else MessageBox.Show("История пуста");

        }

        private void RenoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (historyCounter < History.Count - 1)
            {
                picDrawingSurface.Image = new Bitmap(History[++historyCounter]);
            }
            else MessageBox.Show("История пуста");

        }

        private void SolidToolStripMenuItem_Click(object sender, EventArgs e)
        {
            currentPen.DashStyle = DashStyle.Solid;
            solidStyleMenu.Checked = true;
            dotStyleMenu.Checked = false;
            dashDotDotStyleMenu.Checked = false;

        }
    }
}
