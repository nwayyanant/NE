        private string configItemUpdate(string appConfigContent, string key, string newValue)
        {
            string searchedString = "<add key=\"" + key+ "\" value=\"";
            int index = appConfigContent.IndexOf(searchedString) + searchedString.Length;
            string currentValue = appConfigContent.Substring(index, appConfigContent.IndexOf("\"", index) - index);
            if (currentValue == newValue) return appConfigContent;
            return appConfigContent.Replace(searchedString + currentValue, searchedString+newValue);
        }
		private void buttonQuit_Click(object sender, EventArgs e)
        {
            try
            {
                string appConfigContent = File.ReadAllText(ConfigFile);
                //string searchedString = "<add key=\"LAYOUT\" value=\"";
                //int index = appConfigContent.IndexOf(searchedString) + searchedString.Length;
                //string currentValue = appConfigContent.Substring(index, appConfigContent.IndexOf("\"", index) - index);
                //string newValue = (layOutHorizontal) ? "Horizontal" : "Vertical";
                //var newContent = appConfigContent.Replace("{searchedString}{currentValue}\"", "{searchedString}{newValue}\"");
                
                //string newContent = appConfigContent.Replace(currentValue, newValue);
                string newContent = configItemUpdate(appConfigContent, "LAYOUT", (layOutHorizontal) ? "Horizontal" : "Vertical");
                newContent = configItemUpdate(newContent, "SRC", comboBox1.Text);
                newContent = configItemUpdate(newContent, "TGT", comboBox2.Text);
                File.WriteAllText(ConfigFile, newContent);
            }
            catch (FileNotFoundException)
            {
                MessageBox.Show("File not found.");
            }
            Application.Exit();
        }
        private string configItemValue(string appConfigContent, string key)
        {
            string searchString = "<add key=\"" + key + "\" value=\"";
            int index = appConfigContent.IndexOf(searchString) + searchString.Length;
            string currentValue = appConfigContent.Substring(index, appConfigContent.IndexOf("\"", index) - index);
            return currentValue;
        }
		private string readConfig()
		{
			string appConfigContent = File.ReadAllText(ConfigFile);
		}