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
            aihealue.SelectedIndex = questionStartIndex;
            SetTxtBoxReadonly();

        }


        // TODO make these not global
        const int questionStartIndex = 0;
        const string connectionString =
@"Data Source=..\..\..\database\db_tietovisa_vba.db;Version=3;ApplicationIntent=ReadOnly";

        private void New_Question_Click(object sender, EventArgs e)
        {

            var selectedSubject = aihealue.Text;

            // Muodostaa yhteyden tietokantaan. Muista valita oma reitti.
            using (var connection = new SQLiteConnection(connectionString))
            {
                // Hakee kysymyksen aiheen mukaan ja tuo sen vastaukset ja oikea vastaus numeron.
                connection.Open();
                var query =
"SELECT Kysymys, Vastaus1, Vastaus2, Vastaus3 FROM tbl_tietovisa " +

$"{(selectedSubject == "Kaikki" ?
string.Empty :
$"WHERE Aihe = '{selectedSubject}' OR aihe = 'Kaikki'")} " +

"ORDER BY RANDOM() LIMIT 1";

                // MessageBox.Show(query);
                using (var command = new SQLiteCommand(query, connection))
                {

                    var reader = command.ExecuteReader();

                    if (reader.Read())
                    {

                        kysymys.Text = reader["Kysymys"].ToString();
                        Vastaus1.Text = reader["Vastaus1"].ToString();
                        Vastaus2.Text = reader["Vastaus2"].ToString();
                        Vastaus3.Text = reader["vastaus3"].ToString();

                    }
                }
            }

            // Käynnistää valintapainikkeet
            TurnOnButtons();
            SetTxtBoxReadonly();
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
                var query =
$"SELECT Oikea_vastaus_nro FROM tbl_tietovisa WHERE Kysymys = '{kysymys.Text}'";

                using (var command = new SQLiteCommand(query, connection))
                {

                    if (int.TryParse(command.ExecuteScalar().ToString(), out int correctAnswer))
                    {
                        MessageBox.Show(selectedAnswer == correctAnswer ?
                                        "Oikein!" : "Väärin!");
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
            Vastaus1.IsReadOnly =
            Vastaus2.IsReadOnly =
            Vastaus3.IsReadOnly =
            kysymys.IsReadOnly =
            kysymysOtsikko.IsReadOnly =
            vastaus3Txt.IsReadOnly =
            vastaus2Txt.IsReadOnly =
            vastaus1Txt.IsReadOnly = true;

        private void SetTxtBoxWritable() =>
            Vastaus1.IsReadOnly =
            Vastaus2.IsReadOnly =
            Vastaus3.IsReadOnly =
            kysymys.IsReadOnly =
            kysymysOtsikko.IsReadOnly =
            vastaus3Txt.IsReadOnly =
            vastaus2Txt.IsReadOnly =
            vastaus1Txt.IsReadOnly = true;

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
