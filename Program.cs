using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace SimpleKeyMan
{
    class Program
    {
        static void Main(string[] args)
        {   
            SetupEnv();
            System.Console.WriteLine("You can enter '#' if you want to use your own key");
            System.Console.WriteLine("type in 'help' for a list of commands");
            bool run = true;
            while(run)
            {
                System.Console.Write(">");
                switch (Console.ReadLine().ToLower())
                {
                    case "help":
                        Help();
                        break;
                    case "list":
                        ListSpaces();
                        break;
                    case "canwrite":
                        CanWrite();
                        break;
                    case "new":
                        NewKey();
                        break;
                    case "update":
                        UpdateKey();
                        break;
                    case "delete":
                        DeleteKey();
                        break;
                    case "setkey":
                        SetAPIKey();
                        break;
                    case "setbaseurl":
                        SetAPIKey();
                        break;
                    case "quit":
                        run = false;
                        break;
                    default:
                        break;
                }
            }
            
        }

        private static void SetupEnv()
        {
            System.Console.WriteLine("Setting up envrioment...");
            
            

            config c = new config();

            Console.Write("Which envrioment: ");
            string env = Console.ReadLine(); 
            

            try
            {
                System.Console.WriteLine("Setting config from file...");
                StreamReader sr = new StreamReader("app."+env+".config");
                string s = sr.ReadToEnd();
                sr.Close();

                c = JsonConvert.DeserializeObject<config>(s);
                c.dataserver = Base64Decode(c.dataserver);
                c.key = Base64Decode(c.key);
                System.Console.WriteLine("Done");

            } 
            catch
            {
                System.Console.WriteLine("No config file found...");
                System.Console.WriteLine("Enter dataserver url (this should end in \"/api/\"):");
                string ds = Console.ReadLine().Trim();

                System.Console.WriteLine("Enter your API key:");
                string k = Console.ReadLine().Trim();
                c.dataserver = ds;
                c.key = k;

                System.Console.WriteLine("Would you like to save this information for later use? (Y/N):");
                if(Console.ReadLine().Trim().ToUpper()=="Y")
                {
                    var config2 = new config();
                    config2.key = Base64Encode(c.key);
                    config2.dataserver = Base64Encode(c.dataserver);
                    StreamWriter sw = new StreamWriter("app."+env+".config");
                    sw.WriteLine(JsonConvert.SerializeObject(config2));
                    sw.Close();
                    System.Console.WriteLine("Configuration saved");
                }
                }
            
            ConfiguredHTTPClient.setupClinet(c.dataserver,c.key);
        }
        static void ListSpaces()
        {
            System.Console.Write("Key: ");
            string key = Console.ReadLine();
            if (key == "#") key = ConfiguredHTTPClient._apiKey;
            System.Console.WriteLine(ConfiguredHTTPClient.ListSpaces(key));
        }
        static void CanWrite()
        {
            System.Console.Write("Key: ");
            string key = Console.ReadLine();
            if (key == "#") key = ConfiguredHTTPClient._apiKey;

            System.Console.Write("Space: ");
            string space = Console.ReadLine();

            System.Console.WriteLine(ConfiguredHTTPClient.CanWrite(key,space));
        }

        static void NewKey()
        {
            System.Console.Write("email: ");
            string email = Console.ReadLine();


            System.Console.WriteLine("Enter Spaces, enter # when done: ");
            List<string> spaces = new List<string>();
            bool run = true;
            while (run)
            {
                string val = Console.ReadLine();
                if (val == "#")
                {
                    run = false;
                }
                else 
                {
                    spaces.Add(val);
                }
            }
            System.Console.WriteLine();

            System.Console.WriteLine("Email: " + email);
            string spaceslist = "";
            spaces.ForEach(s => spaceslist += "["+s+"] ");
            System.Console.WriteLine("Spaces:" + spaceslist);
            System.Console.WriteLine("Are you sure you want to create a new key (Y/N):");
            if(Console.ReadLine().ToUpper()=="Y") run = true;

            if (spaces.Count <= 0)
            {
                run = false;
                System.Console.WriteLine("Key not added as no spaces were defined");
            } 
            if (run)
            {
                System.Console.WriteLine(ConfiguredHTTPClient.NewKey(email,spaces));
            }
        }

        static void DeleteKey()
        {
            System.Console.Write("email: ");
            string email = Console.ReadLine();
            System.Console.WriteLine("Note that this action is IRREVERSIBLE");
            System.Console.WriteLine("Are you sure you want to delete the key associated with '{0}'? (Y/N):",email);
            if(Console.ReadLine().ToUpper()=="Y")
            {
                System.Console.WriteLine(ConfiguredHTTPClient.DeleteKey(email));
            }
        }
        
        static void UpdateKey()
        {
            System.Console.Write("email: ");
            string email = Console.ReadLine();


            System.Console.WriteLine("Enter Spaces, enter # when done: ");
            List<string> spaces = new List<string>();
            bool run = true;
            while (run)
            {
                string val = Console.ReadLine();
                if (val == "#")
                {
                    run = false;
                }
                else 
                {
                    spaces.Add(val);
                }
            }
            System.Console.WriteLine();

            System.Console.WriteLine("Email: " + email);
            string spaceslist = "";
            spaces.ForEach(s => spaceslist += "["+s+"] ");
            System.Console.WriteLine("Spaces:" + spaceslist);
            System.Console.WriteLine("Are you sure you want to update the specified email with the above spaces? (Y/N):");
            if(Console.ReadLine().ToUpper()=="Y") run = true;

            if (spaces.Count <= 0)
            {
                run = false;
                System.Console.WriteLine("Key not updated as no spaces were defined");
            } 
            if (run)
            {
                System.Console.WriteLine(ConfiguredHTTPClient.UpdateKey(email,spaces));
            }
        }
        static void SetAPIKey()
        {
            System.Console.Write("Key: ");
            string key = Console.ReadLine();
            ConfiguredHTTPClient.setAPIKey(key);
        }
        static void SetBaseUri()
        {
            System.Console.Write("Base URI: ");
            string key = Console.ReadLine();
            ConfiguredHTTPClient.setHttpClientBaseURI(key);
        }
        static void Help()
        {
            System.Console.WriteLine("List of commands:");
            System.Console.WriteLine("list \t\t List the spaces a key is valid for");
            System.Console.WriteLine("canwrite \t Check if a key can write to a space");
            System.Console.WriteLine("new \t\t New key");
            System.Console.WriteLine("update \t\t Update spaces for a key");    
            System.Console.WriteLine("quit \t\t Exit the application");    


            System.Console.WriteLine();
            System.Console.WriteLine("You can enter '#' if you want to use your own key");

        }
        public static string Base64Encode(string plainText) {
            byte[] plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
        public static string Base64Decode(string encodedString) {
            byte[] data = Convert.FromBase64String(encodedString);
            return System.Text.Encoding.UTF8.GetString(data);
        }
    
        
    }
    class config
    {
        public string dataserver {get;set;}
        public string key {get;set;}
    }
}
