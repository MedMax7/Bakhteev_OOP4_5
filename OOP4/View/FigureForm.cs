using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Model;
using System.IO;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

namespace View
{
    /// <summary>
    /// Основная форма
    /// </summary>
    public partial class FigureForm : Form
    {

        /// <summary>
        /// Список фигур
        /// </summary>
        private List<FigureBase> _figureList;

        /// <summary>
        /// Инициализация формы
        /// </summary>
        public FigureForm()
        {
            InitializeComponent();
            
        }

        /// <summary>
        /// Загрузка формы
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormLoad(object sender, EventArgs e)
        {
            var figureList = new List<FigureBase>();
            _figureList = figureList;
            LoadData();
        }

        /// <summary>
        /// Выбор фигуры
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListViewType_SelectedIndexChanged
            (object sender, EventArgs e)
        {

            if (listViewType.SelectedItems.Count >= 1)
            {
                int firstIndex = listViewType.SelectedIndices[0];
                dataGridView.Rows.Clear();

               
                switch (firstIndex)
                {
                    case 0:
                    {
                        dataGridView.ColumnCount = 3;
                        dataGridView.Columns[0].Name = "ID";
                        dataGridView.Columns[1].Name = "Radius";
                        dataGridView.Columns[2].Name = "Volume";
                        break;
                    }
                    case 1:
                    {
                        dataGridView.ColumnCount = 4;
                        dataGridView.Columns[0].Name = "ID";
                        dataGridView.Columns[1].Name = "Height";
                        dataGridView.Columns[2].Name = "Base area";
                        dataGridView.Columns[3].Name = "Volume";
                        break;
                    }
                    case 2:
                    {
                        dataGridView.ColumnCount = 5;
                        dataGridView.Columns[0].Name = "ID";
                        dataGridView.Columns[1].Name = "Length";
                        dataGridView.Columns[2].Name = "Width";
                        dataGridView.Columns[3].Name = "Height";
                        dataGridView.Columns[4].Name = "Volume";
                        break;
                    }
                    case 3:
                    {
                        dataGridView.ColumnCount = 4;
                        dataGridView.Columns[0].Name = "ID";
                        dataGridView.Columns[1].Name = "Shape type";
                        dataGridView.Columns[2].Name = "Parameters";
                        dataGridView.Columns[3].Name = "Volume";
                        break;
                    }
                }
                FillingInSheet();
            }
        }

        /// <summary>
        /// Загрузка картинок
        /// </summary>
        private void LoadData()
        {
            listViewType.Items.Clear();
            ImageList imageList = new ImageList
            {
                ImageSize = new Size(50, 50)
            };
            imageList.Images.Add(new Bitmap(Properties.Resources.Ball));
            imageList.Images.Add(new Bitmap(Properties.Resources.triangle));
            imageList.Images.Add(new Bitmap(Properties.Resources.parallelepiped));
            imageList.Images.Add(new Bitmap(Properties.Resources.sdrcgs));
            Bitmap emptyImage = new Bitmap(50, 50);
            imageList.Images.Add(emptyImage);
            listViewType.SmallImageList = imageList;
            for (int i = 0; i < 4; i++)
            {
                ListViewItem listViewItem = new ListViewItem
                    (new string[] { "" })
                {
                    ImageIndex = i
                };
                listViewType.Items.Add(listViewItem);
            }
        }


        /// <summary>
        /// Удаляем элемент из списка
        /// </summary>
        /// <param name="sender"></param>   
        /// <param name="e"></param>
        private void ButtonRemoveFigure_Click(object sender, EventArgs e)
        {           
            if (listViewType.SelectedItems.Count >= 1)
            {
                int firstIndex = listViewType.SelectedIndices[0];
                foreach (DataGridViewRow row in dataGridView.
                    SelectedRows)
                {
                    int index = dataGridView.Rows.IndexOf(row);
                    _figureList.RemoveAt(index);
                    dataGridView.Rows.Remove(row);
                }
            }
        }

        /// <summary>
        /// Поиск строки
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonFind_Click(object sender, EventArgs e)
        {
            FindFigureForm newForm = new FindFigureForm();
            if (newForm.ShowDialog() == DialogResult.OK)
            {
                var condition = newForm.Volume;
                for (int i = 0; i < dataGridView.RowCount ; i++)
                {
                    dataGridView.Rows[i].Selected = false;

                    int firstIndex = listViewType.SelectedIndices[0];
                    var formattedValue = String.Empty;
                    switch (firstIndex) 
                    {
                        case 0:
                        {
                            formattedValue = dataGridView[2, i].FormattedValue.
                                ToString();
                            break;
                        }
                        case 1:
                        {
                            formattedValue = dataGridView[3, i].FormattedValue.
                                ToString();
                            break;
                        }
                        case 2:
                        {
                            formattedValue = dataGridView[4, i].FormattedValue.
                                ToString();
                            break;
                        }
                        case 3:
                        {
                            formattedValue = dataGridView[3, i].FormattedValue.
                                ToString();
                            break;
                        }
                    }

                    switch (newForm.Condition) 
                    {
                        case (ConditionType.More):
                        {
                            if (Convert.ToDouble(formattedValue) >
                                condition)
                            {
                                dataGridView.Rows[i].Selected = true;
                            }
                            break;
                        }

                        case (ConditionType.Less):
                        {
                            if (Convert.ToDouble(formattedValue) <
                                condition)
                            {
                                dataGridView.Rows[i].Selected = true;
                            }
                            break;
                        }

                        case (ConditionType.Equally):
                        {
                            if (Convert.ToDouble(formattedValue) ==
                                condition)
                            {
                                dataGridView.Rows[i].Selected = true;
                            }
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
		/// Добавление фигуры
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ButtonAddFigure_Click(object sender, EventArgs e)
        {
            AddFigureForm newForm = new AddFigureForm();
            const int masterSheetNumber = 3;
            if (newForm.ShowDialog() == DialogResult.OK)
            {
                FigureBase figure = newForm.Figure;
                _figureList.Add(figure);
                MessageBox.Show("Shape added!");
                listViewType.Items[masterSheetNumber].Selected = true;
                ListViewType_SelectedIndexChanged(this, null);
                listViewType.Select();

            }
        }

        /// <summary>
		/// Сохранение листа
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ButtonSave_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                string path = Environment.GetFolderPath(
                    Environment.SpecialFolder.MyDocuments);
                saveFileDialog.InitialDirectory = path;
                saveFileDialog.Filter = "Figure files " +
                    "(*.fig)|*.fig|All files (*.*)|*.*";
                saveFileDialog.FilterIndex = 1;
                saveFileDialog.RestoreDirectory = true;

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    var formatter = new BinaryFormatter();
                    var fileSave = saveFileDialog.FileName;
                    using (var fileStream = new FileStream(
                        fileSave, FileMode.OpenOrCreate))
                    {
                        formatter.Serialize(fileStream, _figureList);
                        MessageBox.Show("File saved!");
                    }
                }
            }
        }

        /// <summary>
		/// Загрузка данных
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ButtonDownload_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                dataGridView.Rows.Clear();
                string path = Environment.GetFolderPath(
                    Environment.SpecialFolder.MyDocuments);
                openFileDialog.InitialDirectory = path;
                openFileDialog.Filter = "Figure files" +
                    " (*.fig)|*.fig|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 1;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    var formatter = new BinaryFormatter();
                    var fileLoad = openFileDialog.FileName;

                    if (Path.GetExtension(fileLoad) == ".fig")
                    {
                        try
                        {
                            using (var fileStream = new FileStream(
                                fileLoad, FileMode.OpenOrCreate))
                            {
                                var newFigures = (List<FigureBase>)
                                    formatter.Deserialize(fileStream);

                                _figureList.Clear();

                                foreach (var figure in newFigures)
                                {
                                    _figureList.Add(figure);
                                }

                                MessageBox.Show("File loaded!");
                            }
                        }

                        catch
                        {
                            MessageBox.Show("File is corrupted, " +
                                "unable to load!");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Incorrect file format " +
                            "(not *.fig)!");
                    }
                }
                
                try
                {
                    int firstIndex = listViewType.SelectedIndices[0];
                }

                catch
                {
                    MessageBox.Show("Select a shape to view options");
                    return;
                }

                FillingInSheet();
                
            }
        }

        /// <summary>
        /// Заполнение листа
        /// </summary>
        private void FillingInSheet()
        {
            int firstIndex = listViewType.SelectedIndices[0];
            dataGridView.Rows.Clear();
            for (int i = 0; i < _figureList.Count; i++)
            {
                string[] info = _figureList[i].GetInfo().
                    Split(new char[] { ';' });

                switch (_figureList[i])
                {
                    
                    case Ball ball:
                    {
                        if (firstIndex == 0)
                        {
                            dataGridView.Rows.Add(i + 1, info[0], info[2]);
                        }
                        break;
                    }
                    case Pyramid piramid:
                    {
                        if (firstIndex == 1)
                        {
                            dataGridView.Rows.Add(i + 1, info[0], info[1], info[2]);
                        }
                        break;
                    }
                    case Parallelepiped parallelepiped:
                    {
                        if (firstIndex == 2)
                        {
                            dataGridView.Rows.Add(i + 1, info[0], info[1], info[2], info[3]);
                        }
                        break;
                    }
                }
                if (firstIndex == 3)
                {
                    
                    switch (_figureList[i].GetType().Name)
                    {
                        case "Ball":
                        {
                            dataGridView.Rows.Add(i + 1,
                                _figureList[i].GetType().Name,
                                "Radius = " + info[0], info[2]);
                            break;
                        }
                        case "Pyramid":
                        {
                            dataGridView.Rows.Add(i + 1,
                                _figureList[i].GetType().Name,
                                "Base area = " + info[0] +
                                "; Height = " + info[1], info[2]);
                            break;
                        }
                        case "Parallelepiped":
                        {
                            dataGridView.Rows.Add(i + 1,
                                _figureList[i].GetType().Name,
                                "Width = " + info[0] +
                                "; Height = " + info[1] +
                                "; Length = " + info[2],
                                info[3]);
                            break;
                        }
                    }
                }
            }
        }
    }
}
