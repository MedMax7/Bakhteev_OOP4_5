using Model;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace View
{
    /// <summary>
    /// Форма добавления фигуры
    /// </summary>
    public partial class AddFigureForm : Form
    {
        /// <summary>
        /// Словарь фигур
        /// </summary>		
        private Dictionary<FigureType, string> _figureKey =
            new Dictionary<FigureType, string>
            {
                [FigureType.Ball] = "Ball",
                [FigureType.Pyramid] = "Pyramid",
                [FigureType.Parallelepiped] = "Parallelepiped",
            };

        /// <summary>
        /// Получение FigureBase
        /// </summary>
        public FigureBase Figure
        {
            get; private set;
        }

        /// <summary>
        /// Инициализация формы
        /// </summary>
        public AddFigureForm()
        {
            InitializeComponent();
            Shown += AddFigureForm_Shown;
        }
                
        /// <summary>
		/// Открытие формы добавления фигуры
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
        private void AddFigureForm_Shown(object sender, EventArgs e)
        {
            comboBoxType.Items.Add(_figureKey[FigureType.Ball]);
            comboBoxType.Items.Add(_figureKey[FigureType.Pyramid]);
            comboBoxType.Items.Add(_figureKey[FigureType.Parallelepiped]);

            if (comboBoxType.Text == _figureKey[FigureType.Ball])
            {
                maskedTextBox1.Enabled = true;
            }
            else if (comboBoxType.Text == _figureKey[FigureType.Parallelepiped])
            {
                maskedTextBox1.Enabled = true;
                maskedTextBox2.Enabled = true;
                maskedTextBox3.Enabled = true;
            }
            else
            {
                maskedTextBox2.Enabled = true;
                maskedTextBox3.Enabled = true;
            }
        }

        /// <summary>
        /// Активация спец. полей
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>	
        private void ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            maskedTextBox1.Visible = false;
            maskedTextBox2.Visible = false;
            maskedTextBox3.Visible = false;
            label1.Visible = false;
            label2.Visible = false;
            label3.Visible = false;

            if (comboBoxType.Text == _figureKey[FigureType.Ball])
            {
                label1.Visible = true;
                label1.Text = "Radius:";
                maskedTextBox1.Visible = true;
                maskedTextBox1.Clear();
            }
            else if (comboBoxType.Text == _figureKey[FigureType.Pyramid])
            {
                label2.Visible = true;
                label3.Visible = true;
                label2.Text = "Base area:";
                label3.Text = "Height:";
                maskedTextBox2.Visible = true;
                maskedTextBox3.Visible = true;
                maskedTextBox2.Clear();
                maskedTextBox3.Clear();
            }
            else if (comboBoxType.Text == _figureKey[FigureType.Parallelepiped])
            {
                label1.Visible = true;
                label2.Visible = true;
                label3.Visible = true;
                label2.Text = "Width:";
                label3.Text = "Length:";
                label1.Text = "Height:";
                maskedTextBox1.Visible = true;
                maskedTextBox2.Visible = true;
                maskedTextBox3.Visible = true;
                maskedTextBox1.Clear();
                maskedTextBox2.Clear();
                maskedTextBox3.Clear();
            }
        }

        /// <summary>
		/// Закрыть форму
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
        private void ButtonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Валидация
        /// </summary>
        /// <param name="value"></param>
        /// <param name="nameOfTextBox"></param>
        private double CheckStringNullOrEmpty(string value, string nameOfTextBox)
        {
                if (string.IsNullOrEmpty(value))
                {
                    throw new FormatException($"{nameOfTextBox}" +
                        " - field is empty!");
                }
                else
                {
                    return GetCorrect(value);
                }
        }
                
        /// <summary>
        /// Добавление фигуры
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>		
        private void ButtonOk_Click(object sender, EventArgs e)
        {
            FigureBase figure = null;            
            try
            {
                if (comboBoxType.Text == _figureKey[FigureType.Ball])
                {                    
                    figure = new Ball
                        (CheckStringNullOrEmpty(maskedTextBox1.Text,"Radius"));
                }

                if (comboBoxType.Text == _figureKey[FigureType.Pyramid])
                {
                    figure = new Pyramid
                        (CheckStringNullOrEmpty(maskedTextBox2.Text, "Base area"),
                        CheckStringNullOrEmpty(maskedTextBox3.Text, "Height"));
                }

                if (comboBoxType.Text == _figureKey[FigureType.Parallelepiped])
                {
                    figure = new Parallelepiped
                            (CheckStringNullOrEmpty(maskedTextBox1.Text, "Height"),
                            CheckStringNullOrEmpty(maskedTextBox2.Text, "Width"),
                            CheckStringNullOrEmpty(maskedTextBox3.Text, "Length"));
                }

                Figure = figure ?? throw new ArgumentException("Type not selected!");

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (FormatException ex)
            {
                MessageBox.Show(ex.Message);     
            }
        }

        
        /// <summary>
        /// Проверка на корректность ввода
        /// </summary>
        /// <param name="text">Входная строка</param>
        /// <returns></returns>
        private double GetCorrect( string text)
        {
            try
            {
                double outputNumber;
                double.TryParse(text.Replace('.', ','), NumberStyles.Any,
                    new CultureInfo("ru-RU"), out outputNumber);
                return outputNumber;
            }
            catch (FormatException)
            {
                return default;
            }
        }
        
        
        /// <summary>
        /// Проверяет, существует
        /// ли уже точка в строке
        /// </summary>
        /// <param name="text"></param>
        /// <param name="keyChar"></param>
        /// <returns></returns>
        private bool AlreadyExist(string text, ref char keyChar)
        {
            if (text.IndexOf(',') > -1)
            {
                keyChar = ',';
                return true;
            }

            if (text.IndexOf('.') > -1)
            {
                keyChar = '.';
                return true;
            }
            return false;
        }

        /// <summary>
        /// Разрешает ввод чисел и разделителя
        /// дробной и целой части
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBoxForWage_KeyPress(object sender, KeyPressEventArgs e)
        {
            var tempBox = (MaskedTextBox)sender;

            if (!char.IsControl(e.KeyChar)
                    && !char.IsDigit(e.KeyChar)
                    && e.KeyChar != ',' && e.KeyChar != '.')
            {
                e.Handled = true;
            }

            char separatorChar = 's';
            if (e.KeyChar == ',' || e.KeyChar == '.')
            {
                if (tempBox.Text.Length == 0 ||
                    tempBox.SelectionStart == 0 ||
                    AlreadyExist(tempBox.Text, ref separatorChar))
                {
                    e.Handled = true;
                }
            }

            if (Char.IsDigit(e.KeyChar))
            {
                if (AlreadyExist(tempBox.Text, ref separatorChar))
                {
                    int sepratorPosition = tempBox.Text.IndexOf(separatorChar);
                    string afterSepratorString = tempBox.Text
                        .Substring(sepratorPosition + 1);
                }
            }

        }

       
        /// <summary>
        /// Создать рандомную фигуру
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>		
        private void ButtonRandom_Click(object sender, EventArgs e)
        {
            maskedTextBox1.Text = maskedTextBox2.Text = maskedTextBox3.Text = " ";
            var random = new Random();
            comboBoxType.SelectedIndex = random.Next(0, 3);
            
            if (comboBoxType.Text == _figureKey[FigureType.Ball])
            {
                maskedTextBox1.Text = Convert.ToString(random.Next(1, 50));
            }
            else if (comboBoxType.Text == _figureKey[FigureType.Parallelepiped])
            {
                maskedTextBox1.Text = Convert.ToString(random.Next(1, 50));
                maskedTextBox2.Text = Convert.ToString(random.Next(1, 50));
                maskedTextBox3.Text = Convert.ToString(random.Next(1, 50));
            }
            else
            {
                maskedTextBox2.Text = Convert.ToString(random.Next(1, 50));
                maskedTextBox3.Text = Convert.ToString(random.Next(1, 50));
            }
        }
        
    }
}
