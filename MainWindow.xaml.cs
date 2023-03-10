using System;
using System.Data.SQLite;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace WpfApp61
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>


    public partial class MainWindow : Window
    {
        DispatcherTimer timer;
        public MainWindow()
        {

            InitializeComponent();
            aihealue.SelectedIndex = 0; //questionStartIndex
            SetTxtBoxReadonly();

        }


        // TODO make these not global
        //N: Got rid of unnecessary variable (const int questionStartIndex = 0;)
        const string connectionString = @"Data Source=..\..\..\database\db_tietovisa_vba.db;Version=3;ApplicationIntent=ReadOnly";
        UInt32 correctCounter = 0; //N: correct answers counter
        UInt32 summCounter = 0; //N: all answers counter

        private void New_Question_Click(object sender, EventArgs e) //Start game button
        {
            GoToNextQuestion(); //N: Moved everything to GoToNextQuestion() function
            
            StartGame_Button.IsEnabled = false;
            aihealue.IsEnabled = false; 
            //N: Disable Start game and topic selection

            // Käynnistää valintapainikkeet
            TurnOnButtons();
            SetTxtBoxReadonly();
        }

        private void GoToNextQuestion()
        {
            var selectedSubject = aihealue.Text;

            // Muodostaa yhteyden tietokantaan. Muista valita oma reitti.
            using (var connection = new SQLiteConnection(connectionString))
            {
                // Hakee kysymyksen aiheen mukaan ja tuo sen vastaukset ja oikea vastaus numeron.
                connection.Open();
                var query = "SELECT Kysymys, Vastaus1, Vastaus2, Vastaus3 FROM tbl_tietovisa " + $"{(selectedSubject == "Kaikki" ? string.Empty : $"WHERE Aihe = '{selectedSubject}' OR aihe = 'Kaikki'")} " + "ORDER BY RANDOM() LIMIT 1";

                using (var command = new SQLiteCommand(query, connection))
                {

                    var reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        kysymys.Text = reader["Kysymys"].ToString();
                        valinta1.Content = reader["Vastaus1"].ToString();
                        valinta2.Content = reader["Vastaus2"].ToString();
                        valinta3.Content = reader["vastaus3"].ToString();
                        //N: Write answers on the buttons instead of using TextBoxes
                    }
                }
            }
        }

        private void BtnClick(object sender, RoutedEventArgs e)
        {

            var btn = (Button)sender;
            // since the buttons are named with numbers we can use it to check for the right answer
            CheckAnswer(uint.Parse(btn.Name[^1].ToString()));
        }

        private void CheckAnswer(uint selectedAnswer)
        {
            // Yhdistää tietokannan
            using (var connection = new SQLiteConnection(connectionString))
            {

                /* Hakee Oikea_vastaus_nro kysymyksen kohdalta joka haettiin aikaisemmin.
                *  Katsoo onko valitsemasi vastaus sama yllä olevissa kohdissa
                *  laittaa vastauspainikkeet pois käytöstä
                */

                connection.Open();
                var query = $"SELECT Oikea_vastaus_nro FROM tbl_tietovisa WHERE Kysymys = '{kysymys.Text}'";

                using (var command = new SQLiteCommand(query, connection))
                {

                    if (int.TryParse(command.ExecuteScalar().ToString(), out int correctAnswer))
                    {
                        GoToNextQuestion(); //N: Going to the next question instead of showing message and requiring user to press the button manually
                    }
                    else
                    {
                        MessageBox.Show("Vastauksen varmentamisessa tapahtui ongelma.");
                    }
                }
            }
        }

        private void TurnOffButtons() =>
            valinta1.IsEnabled =
            valinta2.IsEnabled =
            valinta3.IsEnabled = false;

        private void TurnOnButtons() =>
            valinta1.IsEnabled =
            valinta2.IsEnabled =
            valinta3.IsEnabled = true;

        private void SetTxtBoxReadonly() =>
            kysymys.IsReadOnly = true;

        private void SetTxtBoxWritable() =>
            kysymys.IsReadOnly = true;

        // unused methods
        private void Subject_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        private void aihealue_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
        }
        private void Vastaus1_TextChanged(object sender, TextChangedEventArgs e)
        {
        }
        private void Vastaus2_TextChanged(object sender, TextChangedEventArgs e)
        {
        }
        private void Vastaus3_TextChanged(object sender, TextChangedEventArgs e)
        {
        }
    }
}
