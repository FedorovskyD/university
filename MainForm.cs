using Serilog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using University.model;

namespace University
{
    public partial class MainForm : Form
    {
        private UniversityContext dbContext;
        public MainForm()
        {
            InitializeComponent();
            InitializeComboboxWithTabelColumnNames(dataGridViewFaculties,comboBoxWithFacultyColumnNames);
            InitializeComboboxWithTabelColumnNames(dataGridViewDepartments,comboBoxDepartmentColumnNames);
            InitializeComboboxWithTabelColumnNames(dataGridViewTeachers,comboBoxTeacherColumnNames);
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            try
            {
                dbContext = new UniversityContext();
                if (!dbContext.Database.Exists())
                {
                    //Создание бд с начальными данными
                    Database.SetInitializer(new UniversityDbInitializer());
                    dbContext.Database.Initialize(true);
                    //Раскомментировать, если нужно создать бд без начальных данных
                    //dbContext.Database.CreateIfNotExists();
                }

                dbContext.Faculties.Load();
                dbContext.Departments.Load();
                dbContext.Teachers.Load();

                teacherBindingSource.DataSource = dbContext.Teachers.Local.ToBindingList();
                departmentBindingSource.DataSource = dbContext.Departments.Local.ToBindingList();
                facultyBindingSource.DataSource = dbContext.Faculties.Local.ToBindingList();


            }
            catch (Exception ex)
            {
                Log.Logger.Error($"Произошла ошибка при загрузке данных: {ex.Message}");
                MessageBox.Show($"Произошла ошибка при загрузке данных: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            Log.CloseAndFlush();
        }

        private void SaveToDiskButton_Click(object sender, EventArgs e)
        {
            if (dbContext == null) return;
            // Получаем данные
            var teachers = dbContext.Teachers.ToList();
            var faculties = dbContext.Faculties.ToList();
            var departments = dbContext.Departments.ToList();

            // Формируем строку для каждого учителя
            string teachersData = string.Join(Environment.NewLine, teachers.Select(t =>
                $"{t.Id}, {t.Name}, {t.LastName}, {t.Address}, {t.Telephone}, {t.Department?.Name}"
            ));

            // Формируем строку для каждого факультета
            string facultiesData = string.Join(Environment.NewLine, faculties.Select(f =>
                $"{f.Id}, {f.Name}, {f.Adress}, {f.Telephone}"
            ));

            // Формируем строку для каждого департамента
            string departmentsData = string.Join(Environment.NewLine, departments.Select(d =>
                $"{d.Id}, {d.Name}, {d.Faculty?.Name}"
            ));
            // Используем SaveFileDialog для выбора места сохранения файла
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
                saveFileDialog.Title = "Выберите место для сохранения файла";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    // Записываем данные в выбранный файл
                    File.WriteAllText(saveFileDialog.FileName,
                        $"Факультеты:{Environment.NewLine}{facultiesData}{Environment.NewLine}{Environment.NewLine}" +
                        $"Кафедры:{Environment.NewLine}{departmentsData}{Environment.NewLine}{Environment.NewLine}" +
                        $"Преподаватели:{Environment.NewLine}{teachersData}{Environment.NewLine}{Environment.NewLine}");
                }
            }
        }
        #region Удаления записей

        private void DataGridViewDepartments_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                ConfirmDeleteRows(dataGridViewDepartments, "Кафедры");
            }
        }

        private void DataGridViewTeachers_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                ConfirmDeleteRows(dataGridViewTeachers, "Преподаватели");
            }
        }

        private void DataGridViewFaculties_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                ConfirmDeleteRows(dataGridViewFaculties, "Факультеты");
            }
        }
        private void ConfirmDeleteRows(DataGridView dataGridView, string tableName)
        {
            int selectedRowCount = dataGridView.SelectedRows.Count;

            DialogResult result = MessageBox.Show($"Вы уверены, что хотите удалить {selectedRowCount} строк(и) из таблицы {tableName}?",
                "Подтверждение удаления", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.No)
            {
                dataGridView.CancelEdit();
            }
        }
        #endregion

        #region Валидация записей

        private void DataGridViewFaculties_RowValidating(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (!DataGridView_RowValidating(sender, e, "columnFacultyName", "Введите название факультета"))
            {
                MessageBox.Show("Завершите редактирование записи в таблице Факультеты", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void DataGridViewDepartments_RowValidating(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (!DataGridView_RowValidating(sender, e, "columnFacultyOfDepartment", "Выберите факультет") |
                !DataGridView_RowValidating(sender, e, "columnDepartmentName", "Введите название кафедры"))
            {
                MessageBox.Show("Завершите редактирование записи в таблице Кафедры", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void DataGridViewTeachers_RowValidating(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (
            !DataGridView_RowValidating(sender, e, "columnDepartmentOfTeacher", "Выберите кафедру") |
            !DataGridView_RowValidating(sender, e, "columnTeacherName", "Введите имя") |
            !DataGridView_RowValidating(sender, e, "columnTeacherLastName", "Введите Фамилию"))
            {
                MessageBox.Show("Завершите редактирование записи в таблице Учителя", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private bool DataGridView_RowValidating(object sender, DataGridViewCellCancelEventArgs e, string columnName, string errorMessage)
        {
            DataGridViewRow row = ((DataGridView)sender).Rows[e.RowIndex];

            if (!row.IsNewRow)
            {
                var cellValue = row.Cells[columnName].Value;

                if (cellValue == null || (cellValue is int intValue && intValue <= 0) || (cellValue is string strValue && string.IsNullOrEmpty(strValue)))
                {
                    row.Cells[columnName].ErrorText = errorMessage;
                    ((DataGridView)sender).EndEdit();
                    e.Cancel = true;
                    return false;
                }
                else
                {
                    row.Cells[columnName].ErrorText = string.Empty;
                }
            }
            return true;
        }
        private void RowValidated(object sender, DataGridViewCellEventArgs e)
        {
            //Сохранение записей, когда строка прошла валидацию
            try
            {
                dbContext?.SaveChanges();
            }
            catch(Exception ex)
            {
                Log.Logger.Error($"Произошла ошибка при сохранении данных: {ex.Message}");
                MessageBox.Show($"Произошла ошибка при сохранении данных: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
            }
        }

        #endregion

        #region Поиск записей
        private void InitializeComboboxWithTabelColumnNames(DataGridView dgv, ComboBox cmb)
        {
            if (dgv == null) return;
            if (cmb == null) return;
            // Заполнение ComboBox названиями столбцов
            foreach (DataGridViewColumn column in dgv.Columns)
            {
                cmb.Items.Add(column.HeaderText);
            }
            if (cmb.Items.Count > 0)
            {
                cmb.SelectedIndex = 0;
            }
        }

        private void TextBoxFacultyWithKeyValue_TextChanged(object sender, EventArgs e)
        {
            FilterDataGridView(dataGridViewFaculties, comboBoxWithFacultyColumnNames, textBoxFacultyWithKeyValue);
        }

        private void ComboBoxFacultyColumns_SelectedIndexChanged(object sender, EventArgs e)
        {
            textBoxFacultyWithKeyValue.Clear();
        }
        private void ComboBoxDepartmentColumnNames_SelectedIndexChanged(object sender, EventArgs e)
        {
            textBoxDepartmentWithKeyValue.Clear();
        }

        private void ComboBoxTeacherColumnNames_SelectedIndexChanged(object sender, EventArgs e)
        {
            textBoxTeacherWithKeyValue.Clear();
        }

        private void TextBoxDepartmentWithKeyValue_TextChanged(object sender, EventArgs e)
        {
            FilterDataGridView(dataGridViewDepartments, comboBoxDepartmentColumnNames, textBoxDepartmentWithKeyValue);
        }

        private void TextBoxTeacherWithKeyValue_TextChanged(object sender, EventArgs e)
        {
            FilterDataGridView(dataGridViewTeachers, comboBoxTeacherColumnNames, textBoxTeacherWithKeyValue);
        }

        private void FilterDataGridView(DataGridView dgv, ComboBox comboBox, TextBox textBox)
        {
            string searchText = textBox.Text.ToLower();

            // Скрываем все строки в DataGridView
            foreach (DataGridViewRow row in dgv.Rows)
            {
                if (!row.IsNewRow)
                {
                    dgv.CurrentCell = null;
                    row.Visible = false;
                }
            }

            // Если в ComboBox выбрано "все", выполняем поиск по всем столбцам
            if (comboBox.SelectedItem.ToString() == "все")
            {
                foreach (DataGridViewRow row in dgv.Rows)
                {
                    foreach (DataGridViewCell cell in row.Cells)
                    {
                        if (cell.Value != null && cell.Value.ToString().ToLower().Contains(searchText) )
                        {
                            row.Visible = true;
                            break; // Выход из цикла при первом совпадении
                        }
                    }
                }
            }
            else
            {
                // Выполнение поиска по выбранному столбцу
                string columnName = comboBox.SelectedItem.ToString();

                if (!string.IsNullOrEmpty(columnName))
                {
                    // Находим индекс столбца по его заголовку
                    int columnIndex = -1;
                    foreach (DataGridViewColumn column in dgv.Columns)
                    {
                        if (column.HeaderText == columnName)
                        {
                            columnIndex = column.Index;
                            break;
                        }
                    }

                    // Показываем только строки, соответствующие условиям поиска
                    foreach (DataGridViewRow row in dgv.Rows)
                    {
                        if (row.Cells[columnIndex].Value != null &&
                            row.Cells[columnIndex].Value.ToString().IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            row.Visible = true;
                        }
                    }
                }
            }
        }
        #endregion

        private void DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            DataGridView dgv = (DataGridView)sender;
            if (!dgv.Rows[e.RowIndex].IsNewRow)
            {
                int columnIndex = e.ColumnIndex;
                int rowIndex = e.RowIndex;

                if (columnIndex >= 0 && columnIndex < dgv.Columns.Count && rowIndex >= 0 && rowIndex < dgv.Rows.Count)
                {
                    if (dgv.Rows[rowIndex].Cells[columnIndex] is DataGridViewComboBoxCell)
                    {
                        e.Cancel = true;
                    }
                    else
                    {
                        if (MessageBox.Show("Таблица " + dgv.Name + "\nОшибка: ячейка " +
                            rowIndex + "x" + columnIndex + "\n" +
                            e.Exception.Message, "Внимание", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK)
                        {
                            e.Cancel = true;
                        }
                    }
                }
            }
        }

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}

