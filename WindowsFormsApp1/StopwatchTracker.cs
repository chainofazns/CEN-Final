using System;
using System.Windows.Forms;
using System.Data.SQLite;

namespace MyFitTimer_1
{
    public partial class StopwatchTracker : Form
    {
        //Define time variables
        int sec = 0;
        int min = 0;
        int hrs = 0;
        DateTime startTime = new DateTime();
        DateTime endTime = new DateTime();
        //create sql connection
        SQLiteConnection sql_conn;

        public StopwatchTracker()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //on start, connect to table, check if its made, set start time
            sql_conn = CreateConnection();
            CreateTable(sql_conn);
            startTime = DateTime.Now;
            button2.Visible = true;
            button1.Visible = false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //on end, find end time, calculate time elapsed. 
            endTime = DateTime.Now;
            TimeSpan timeElapsed = endTime.Subtract(startTime);
            string msg = "Starting time: " + startTime.TimeOfDay.ToString("hh\\:mm\\:ss") + "\nTime Elapsed: " + timeElapsed.ToString("hh\\:mm\\:ss");
            //add time elapsed to our database
            sec = timeElapsed.Seconds;
            min = timeElapsed.Minutes;
            hrs = timeElapsed.Hours;
            InsertTime(sql_conn, hrs, min, sec);
            //pull past times and add to our message
            string past = ReadData(sql_conn, "\nPast Times:\n");
            MessageBox.Show(msg + past);
            button2.Visible = false;
            button1.Visible = true;
        }
        //SQL comamnds
        static SQLiteConnection CreateConnection() {
            SQLiteConnection sqlite_conn;
            //create database if it doesnt exist
            //sqlite_conn = new SQLiteConnection("Data Source=database.db; Version = 3; New = True; Compress = True; ");
            sqlite_conn = new SQLiteConnection("Data Source=database.db;");
            //connect to it
            try
                {
                    sqlite_conn.Open();
                }
                catch (Exception ex)
                {
                    
                }
                return sqlite_conn;
        }
        static void CreateTable(SQLiteConnection conn) {
            SQLiteCommand cmd;
            //create table if it doesnt exist
            string createTable = "CREATE TABLE IF NOT EXISTS Stopwatch (Col1 INT, Col2 INT, Col3 INT)";
            cmd = conn.CreateCommand();
            cmd.CommandText = createTable;
            cmd.ExecuteNonQuery();
        }
        static void InsertTime(SQLiteConnection conn, int hr, int min, int sec) {
            //insert time as integers
            SQLiteCommand cmd;
            cmd = conn.CreateCommand();
            cmd.CommandText = "INSERT INTO Stopwatch (Col1, Col2, Col3) VALUES (" + hr + ", " + min + ", " + sec + ");";
            cmd.ExecuteNonQuery();
        }
        static String ReadData(SQLiteConnection conn, string message) {
            //pull integers from our database and convert it to a string
            SQLiteDataReader dataReader;
            SQLiteCommand cmd;
            cmd = conn.CreateCommand();
            cmd.CommandText =  "SELECT * FROM Stopwatch";
            dataReader = cmd.ExecuteReader();
            while (dataReader.Read()) {
                int readHours = dataReader.GetInt32(0);
                int readMinutes = dataReader.GetInt32(1);
                int readSeconds = dataReader.GetInt32(2);
                message = message + readHours.ToString("00") + ":" + readMinutes.ToString("00") + ":" + readSeconds.ToString("00") + "\n";
            }
            conn.Close();
            return message;
        }
    }
}
