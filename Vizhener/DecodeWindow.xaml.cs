using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Vizhener
{
    /// <summary>
    /// Логика взаимодействия для DecodeWindow.xaml
    /// </summary>
    public partial class DecodeWindow : Window
    {
        static char[] characters = new char[] { 'а', 'б', 'в', 'г', 'д', 'е', 'ё', 'ж', 'з', 'и',
                                                'й', 'к', 'л', 'м', 'н', 'о', 'п', 'р', 'с',
                                                'т', 'у', 'ф', 'х', 'ц', 'ч', 'ш', 'щ', 'ь', 'ы', 'ъ',
                                                'э', 'ю', 'я', ' ' };
        string _text;
        public DecodeWindow(string text)
        {
            _text = text;
            InitializeComponent();
        }

        public List<int> SupposedKeyLengths(string text)
        {
            List<double> matchIndexKeys = new List<double>() { 0, 0 };
            Dictionary<char, int> dictionary;
            for (int i = 2; i < 10; i++)
            {
                string s = string.Empty;

                for (int j = 0; j < text.Length; j++)
                {
                    if (j % i == 0)
                        s += text[j];
                }
                dictionary = new Dictionary<char, int>();
                foreach (var letter in s)
                {
                    if (dictionary.ContainsKey(letter))
                        dictionary[letter]++;
                    else
                        dictionary.Add(letter, 1);
                }
                double matchIndex = 0;
                foreach (var num in dictionary.Values)
                {
                    matchIndex += num * (num - 1);
                }
                matchIndex /= s.Length * (s.Length - 1);

                matchIndexKeys.Add(matchIndex);
            }
            List<int> possibleKeyLengths = new List<int>();
            for (int i = 2; i < matchIndexKeys.Count; i++)
            {
                if (matchIndexKeys[i] - matchIndexKeys[i - 1] > 0.005)
                    possibleKeyLengths.Add(i);
            }
            return possibleKeyLengths;
        }
        
        public List<string> SupossedKeys(int keyLength, string text)
        {
            List<string> supossedKeys = new List<string>();
            List<string> allTexts = new List<string>();
            List<int> steps = new List<int>();

            for (int i = 0; i < keyLength; i++)
            {
                string s = string.Empty;

                for (int j = i; j < text.Length; j += keyLength)
                {
                    s += text[j];
                }
                allTexts.Add(s);
            }

            for (int i = 1; i < allTexts.Count; i++)
            {
                List<double> indexes = new List<double>();

                for (int j = 1; j < characters.Length + 1; j++)
                {
                    string s = string.Empty;

                    for (int g = 0; g < allTexts[i].Length; g++)
                    {
                        char ch = allTexts[i][g];
                        int position = Array.IndexOf(characters, ch);
                        int step = (position + j) % characters.Length;
                        s += characters[step];
                    }
                    string ideal = allTexts[0];
                    Dictionary<char, double> dictionary1 = new Dictionary<char, double>();
                    Dictionary<char, double> dictionary2 = new Dictionary<char, double>();
                    for (int k = 0; k < ideal.Length; k++)
                    {
                        if (dictionary1.ContainsKey(ideal[k]))
                            dictionary1[ideal[k]]++;
                        else
                            dictionary1.Add(ideal[k], 1);
                    }

                    for (int k = 0; k < s.Length; k++)
                    {
                        if (dictionary2.ContainsKey(s[k]))
                            dictionary2[s[k]]++;
                        else
                            dictionary2.Add(s[k], 1);
                    }

                    double sum = 0;
                    foreach (var kvp in dictionary1)
                    {
                        if (dictionary2.ContainsKey(kvp.Key))
                        {
                            sum += kvp.Value * dictionary2[kvp.Key];
                        }
                    }

                    double itemIndex = sum / (allTexts[0].Length * s.Length);

                    indexes.Add(itemIndex);
                }
                steps.Add(Array.IndexOf(indexes.ToArray(), indexes.Max()));
            }

            for (int i = 0; i < characters.Length; i++)
            {
                string s = string.Empty + characters[i];
                foreach (var step in steps)
                {
                    if (i - step - 1 < 0)
                    {
                        s += characters[(i + characters.Length - step - 1) % characters.Length];
                    }
                    else
                    {
                        s += characters[(i - step - 1) % characters.Length];
                    }
                }
                supossedKeys.Add(s);
            }
            return supossedKeys;
        }
        
        private string Decode(string text, string key)
        {
            string decode = string.Empty;
            int keyword_index = 0;

            foreach (char symbol in text)
            {
                int p = (Array.IndexOf(characters, symbol) + characters.Length -
                    Array.IndexOf(characters, key[keyword_index])) % characters.Length;

                decode += characters[p];

                keyword_index++;

                if ((keyword_index) == key.Length)
                    keyword_index = 0;
            }
            return decode;
        }

        private void Button_Click_KeyLength(object sender, RoutedEventArgs e)
        {
            var listLengOfKey = SupposedKeyLengths(_text);
            foreach (int lengthKey in listLengOfKey)
            {
                KeyLengthBox.Items.Add(lengthKey);
            }
        }


        private void Button_Click_Key(object sender, RoutedEventArgs e)
        {
            var keys = SupossedKeys(Convert.ToInt32(KeyLengthBox.SelectedValue), _text);
            foreach (string key in keys)
            {
                KeyBox.Items.Add(key);
            }
        }

        private void Button_Click_Decode(object sender, RoutedEventArgs e)
        {
            DecodeText.Text = Decode(_text, KeyBox.SelectedValue.ToString());
        }
        private void Button_Click_Clear(object sender, RoutedEventArgs e)
        {
            KeyLengthBox.Items.Clear();
            KeyBox.Items.Clear();
            DecodeText.Clear();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            List<int> keyLengths = SupposedKeyLengths(_text);
            List<string> AllKeys = new List<string>();
            foreach (var keyLength in keyLengths)
            {
                List<string> suppossedKeys = SupossedKeys(keyLength, _text);
                foreach (var sup in suppossedKeys)
                {
                    AllKeys.Add(sup);
                }
            }
            foreach (var key in AllKeys)
            {
                //DecodeText.Text += key + '\n' + '\r' + Decode(_text, key) + '\n' + '\r';
                string decode = Decode(_text, key);
                if (decode.Contains(" который "))
                {
                    DecodeText.Text += "Ключ: " + key + '\n' + '\r' + decode + '\n' + '\r';
                }

            }
        }
    }
}
