using System;
using System.Globalization;
using System.IO;
using Npgsql;
using CsvHelper;

namespace gmc320_correlator
{
    class Program
    {
        static void Main(string[] args)
        {
            string filelocation = args[1];
            string cs = "Host=localhost;Username=postgres;Password=<your_pass>#;Database=nuclear_stl"; // yes yes no cnnection string passwords, truth.
            using NpgsqlConnection con = new NpgsqlConnection(cs);
            con.Open();
            //handle gmc320 csv
            using var streamReader = File.OpenText(filelocation);
            using var csvReader = new CsvReader(streamReader, CultureInfo.CurrentCulture);

            while (csvReader.Read())
            {
                string timestamp = csvReader.GetField(0);
                string geiger_reading = csvReader.GetField(2);
                string sql = $"UPDATE readings set microsievert = {geiger_reading} where timestamp >= '{timestamp}' and timestamp <= '{timestamp}:59'";
                try
                {
                    using NpgsqlCommand cmd = new NpgsqlCommand(sql, con);
                    cmd.ExecuteScalar();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
           
        }
    }
}