using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json.Linq;
using System.IO;
//using System.Web.Services;
//using System.Web.UI.WebControls;
using System.Threading;
using System.Threading.Tasks;

namespace JSON
{
    public class Json
    {
        private string jFileName = string.Empty;
        private JObject jObject;

        private static Mutex mux = new Mutex();
        //**************************************************************************************
        //* Json() - Constructor.
        //**************************************************************************************
        public Json(string fileName = "")
        {
            if (File.Exists(fileName))
            {
                jFileName = fileName;
                var json = File.ReadAllText(jFileName);
                jObject = JObject.Parse(json);
            }
        }
        
        public void CreateFile(string fileName)
        {
            jFileName = fileName;
            string Date = "  \"DATE\": \"";
            string d = DateTime.Today.ToString();
            int p = d.IndexOf(' ');
            if (p != -1) d = d.Substring(0, p);
            Date += d;
            Date += "\"";
            string[] lines = { "{", Date, "}" };
            string json = string.Join(System.Environment.NewLine, lines);
            // create jObject
            jObject = JObject.Parse(json);
            //UpdateData(KeyValuePair);
        }
        //**************************************************************************************
        //* GetData() - To retrieve key, value pairs from json file and format into strings
        //              as comma separated values.
        //**************************************************************************************
        public string[] GetData()
        {
            List<string> s = new List<string>();
            //mux.WaitOne();
            //var json = File.ReadAllText(jFileName);
            //mux.ReleaseMutex();
            try
            {
                //jObject = JObject.Parse(json);
                if (jObject != null)
                {
                    foreach (var j in jObject)
                    {
                        string name = j.Key;
                        string value = j.Value.Value<string>();
                        s.Add(name + "|" + value);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Add Error : " + ex.Message.ToString());
            }
            return s.ToArray();
        }
        //**************************************************************************************
        //* GetData(string key) - To retrieve value for the given key.
        //**************************************************************************************
        public string GetData(string key)
        {
            JToken jtoken;
            string value = string.Empty;
            try
            {
                if (jObject != null)
                {
                    jtoken = jObject.GetValue(key);
                    if (jtoken != null)
                        value = jtoken.Parent.First().Value<string>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Add Error : " + ex.Message.ToString());
            }
            return value;
        }
        //**************************************************************************************
        //* GetData() - To retrieve key, value pairs from json file and format into strings
        //              as comma separated values.
        //**************************************************************************************
        public string GetReport()
        {
            mux.WaitOne();
            var json = File.ReadAllText(jFileName);
            mux.ReleaseMutex();
            string s = string.Empty;
            try
            {
                var jObject = JObject.Parse(json);
                if (jObject != null)
                {
                    foreach (var j in jObject)
                    {
                        string name = j.Key;
                        string value;
                        if (name == "DATE") value = j.Value.Value<string>();
                        else value = j.Value.Value<int>().ToString();
                        s += "<br>" + getLineMsg(name) + value;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Add Error : " + ex.Message.ToString());
            }
            return s;
        }
        private string getLineMsg(string key)
        {
            string rc = string.Empty;
            switch (key)
            {
                case "DATE":
                    rc = "Date started = ";
                    break;
                case "VISITORS":
                    rc = "Total number of visitors = ";
                    break;
                case "CONVERSIONS":
                    rc = "Total number of conversions = ";
                    break;
                default:
                    if (key == string.Empty) rc = "Undefined";
                    else
                    {
                        if (key.IndexOf("PLATFORM") != -1 || key.IndexOf("REGION") != -1)
                            rc = key + " users = ";
                        else
                        {
                            if (key.IndexOf(".ZIP") >= 0 || key.IndexOf(".PDF") >= 0)
                                rc = key + " downloads = ";
                            else
                                rc = key + " conversions = ";
                        }
                    }
                    break;
            }
            return rc;
        }
        //**************************************************************************************
        //* UpdateData() - To update the record matching the key json file. It adds a new
        //                 record if the key doesn't exist.
        //**************************************************************************************
        public void UpdateData(string key)
        {
            try
            {
                //string json = File.ReadAllText(jFileName);
                key = key.ToUpper();
                //var jObject = JObject.Parse(json);

                JToken j = jObject[key];
                if (j != null)
                {
                    int count = j.Value<int>();
                    jObject[key] = count + 1;
                }
                else
                    // add new key
                    jObject.Add(key, 1);

                string output = Newtonsoft.Json.JsonConvert.SerializeObject(jObject, Newtonsoft.Json.Formatting.Indented);
                File.WriteAllText(jFileName, output);
            }
            catch (Exception ex)
            {

                Console.WriteLine("Json Update Error : " + ex.Message.ToString());
            }
        }
        //**************************************************************************************
        //* UpdateData() - To update the record matching the key json file. It adds a new
        //                 record if the key doesn't exist.
        //**************************************************************************************
        public void UpdateData(string[] keys)
        {
            try
            {
                foreach(string key in keys)
                {
                    string[] f = key.Split('|');
                    string k = f[0].ToUpper();
                    string v = (f.Count() == 2) ? f[1] : string.Empty;
                    JToken j = jObject[k];
                    if (j != null)
                    {
                        //string value = j.Value<string>();
                        jObject[k] = v;
                    }
                    else
                        // add new key
                        jObject.Add(k, v);
                }
                string output = Newtonsoft.Json.JsonConvert.SerializeObject(jObject, Newtonsoft.Json.Formatting.Indented);
                File.WriteAllText(jFileName, output);
            }
            catch (Exception ex)
            {

                Console.WriteLine("Json Update Error : " + ex.Message.ToString());
            }
        }
        //**************************************************************************************
        //* UpdateData() - To update the record matching the key json file. It adds a new
        //                 record if the key doesn't exist.
        //**************************************************************************************
        public void UpdateSessionStats(string[] keys)
        {
            try
            {
                mux.WaitOne();
                string json = File.ReadAllText(jFileName);
                mux.ReleaseMutex();
                var jObject = JObject.Parse(json);

                foreach (string k in keys)
                {
                    string key = k.ToUpper();
                    JToken j = jObject[key];
                    if (j != null)
                    {
                        int count = j.Value<int>();
                        jObject[key] = count + 1;
                    }
                    else
                        // add new key
                        jObject.Add(key, 1);
                }
                string output = Newtonsoft.Json.JsonConvert.SerializeObject(jObject, Newtonsoft.Json.Formatting.Indented);
                File.WriteAllText(jFileName, output);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Json Update Error : " + ex.Message.ToString());
            }
        }
    }
}