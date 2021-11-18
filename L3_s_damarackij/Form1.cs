using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SQLite;
using System.Globalization;



namespace L3_s_damarackij
{
    public partial class Form1 : Form
    {
        string dbName = "s_damarackij.sqlite";

        // объявление соединения с БД, таблицы с данными в памяти и адаптера, связывающего таблицу БД с таблицей в памяти
        SQLiteConnection sConn;
        DataTable sTable;
        DataTable sTable2;
        SQLiteDataAdapter sAdapter;
        SQLiteDataAdapter sAdapter2;


        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
           
            // Если БД не существует, она будет создана
            sConn = new SQLiteConnection("Data Source=" + dbName);

            sConn.Open();

            SQLiteCommand pragma = new SQLiteCommand("PRAGMA foreign_keys = 1", sConn);
            pragma.ExecuteNonQuery();
            // Создание таблицы Persons, если она не существует (закомментированные строки копировать не обязательно)
            // id_pers - целое автоинкрементное поле - первичный ключ
            // name - текстовое поле
            /*using (SQLiteCommand sCmd = new SQLiteCommand("CREATE TABLE Persons (id_pers INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL, name VARCHAR NOT NULL UNIQUE);", sConn))
            {
            sCmd.ExecuteNonQuery();
            }*/
           

            // Загрузка данных из таблицы Person в элемент DataTable
            sAdapter = new SQLiteDataAdapter("SELECT * FROM Person", sConn);
            sTable = new DataTable();
            sAdapter.Fill(sTable);

            // Автоматическое создание требуемых для обновления таблицы БД
            // команд INSERT, UPDATE, DELETE
            new SQLiteCommandBuilder(sAdapter);

            // Привязка элемента DataTable к элементу DataGridView
            dataGridView1.DataSource = sTable;

         
            


            ///////////////////////////////////////////////////////////
            // Загрузка данных из таблицы Tel в элемент DataTable
            sAdapter2 = new SQLiteDataAdapter("SELECT * FROM Tel", sConn);
            sTable2 = new DataTable();
            sAdapter2.Fill(sTable2);

            // Автоматическое создание требуемых для обновления таблицы БД
            // команд INSERT, UPDATE, DELETE
            SQLiteCommandBuilder dbCommandBuilder= new SQLiteCommandBuilder(sAdapter2);

            dataGridView2.DataSource=sTable2;


            // Переключение на русский язык
            InputLanguage.CurrentInputLanguage = InputLanguage.FromCulture(new CultureInfo("ru-RU"));

            // Запрет изменения значений в столбце id_pers вручную - только для отладки
            //  впоследствии столбец будет скрыт
            dataGridView1.Columns[0].ReadOnly = true;


            dataGridView1.Columns[0].Width = 50;

            dataGridView1.Columns[1].HeaderText = "Абонент";
            dataGridView2.Columns[0].Visible = false;
            dataGridView2.Columns[1].HeaderText = "Абонент";
            dataGridView2.Columns[2].HeaderText = "Номер";
            dataGridView2.Columns[3].HeaderText = "Тип";



            string sid_pers = dataGridView1.CurrentRow.Cells[0].Value.ToString();
            sAdapter2 = new SQLiteDataAdapter("SELECT * FROM Tel WHERE id_pers=" + sid_pers, sConn);
          
        }

        public void showTable()
        {
            dataGridView2.Columns[0].Visible = false;
            dataGridView2.Columns[1].HeaderText = "Абонент";
            dataGridView2.Columns[2].HeaderText = "Номер";
            dataGridView2.Columns[3].HeaderText = "Тип";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            new SQLiteCommandBuilder(sAdapter2);

            dataGridView2.DataSource = sTable2;
            sAdapter.Update(sTable);
            try
            {
                sAdapter2.Update(sTable2);
            }
            catch
            {
                MessageBox.Show("Такого абонента не существует");
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            DataView dataView = sTable.DefaultView;
            dataView.RowFilter = "name LIKE '" + textBox1.Text + "%'";
            dataGridView1.DataSource = dataView;
            showTable();
        }

       

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            // определение текущего значения id_pers
      
            sTable2 = new DataTable();
            string sid_pers;
            try
            {
                sid_pers = dataGridView1.CurrentRow.Cells[0].Value.ToString();

            }
            catch
            {
                sid_pers = "";
            }

            if (sid_pers=="")
            {
                sAdapter2 = new SQLiteDataAdapter("SELECT * FROM Tel", sConn);
                sTable2 = new DataTable();
                sAdapter2.Fill(sTable2);
                showTable();

            }
            else
            {
                sAdapter2 = new SQLiteDataAdapter("SELECT * FROM Tel WHERE id_pers=" + sid_pers, sConn);
                dataGridView2.DataSource = sTable2;
                sAdapter2.Fill(sTable2);
                showTable();

            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            sTable2.Clear();
            sAdapter2 = new SQLiteDataAdapter("SELECT * FROM Tel", sConn);
            sAdapter2.Fill(sTable2);
            showTable();

        }

        private void dataGridView2_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            MessageBox.Show("Некорректный ввод");

        }
    }
}
