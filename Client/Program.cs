using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

class Program
{
    static void Main(string[] args)
    {
        // Get the application directory
        string appDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

        // Read IP address, port, and logging settings from file
        string ip;
        int port;
        bool loggingEnabled;
        if (File.Exists(Path.Combine(appDirectory, "config.txt")))
        {
            string[] lines = File.ReadAllLines(Path.Combine(appDirectory, "config.txt"));
            ip = lines[0];
            port = int.Parse(lines[1]);
            loggingEnabled = bool.Parse(lines[2]);
        }
        else
        {
            // Create config.txt file with example data
            ip = "127.0.0.1";
            port = 1234;
            loggingEnabled = true;
            string[] lines = { ip, port.ToString(), loggingEnabled.ToString() };
            File.WriteAllLines(Path.Combine(appDirectory, "config.txt"), lines);
            Console.WriteLine("Created config.txt file with example data.");
            return;
        }

        try
        {
            // Connect to host
            TcpClient client = new TcpClient();
            client.Connect(ip, port);

            // Send data
            string message = args[0]; // Data from command line arguments
            byte[] data = Encoding.ASCII.GetBytes(message);
            using (NetworkStream stream = client.GetStream())
            {
                stream.Write(data, 0, data.Length);

                // Read response from the server
                byte[] buffer = new byte[1024];
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                string response = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                Console.WriteLine("Server response: " + response);

                // Write data to log file if logging is enabled
                if (loggingEnabled)
                {
                    string logFile = Path.Combine(appDirectory, "log.txt");
                    using (StreamWriter writer = File.AppendText(logFile))
                    {
                        writer.WriteLine(DateTime.Now + " - " + message);
                    }
                }
            }

            client.Close();
        }
        catch (Exception ex)
        {

            if (loggingEnabled)
            {
                string logFile = Path.Combine(appDirectory, "error.log");
                using (StreamWriter writer = File.AppendText(logFile))
                {
                    writer.WriteLine(DateTime.Now + " - " + ex.Message);
                }
            }

            Console.WriteLine("Error: " + ex.Message);
        }
    }
}
