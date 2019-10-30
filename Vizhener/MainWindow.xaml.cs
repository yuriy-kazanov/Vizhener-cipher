using Microsoft.Win32;
using System;
using System.Linq;
using System.Windows;
using System.IO;

namespace Vizhener
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static char[] characters = new char[] { 'а', 'б', 'в', 'г', 'д', 'е', 'ё', 'ж', 'з', 'и',
                                                'й', 'к', 'л', 'м', 'н', 'о', 'п', 'р', 'с',
                                                'т', 'у', 'ф', 'х', 'ц', 'ч', 'ш', 'щ', 'ь', 'ы', 'ъ',
                                                'э', 'ю', 'я', ' ' };
        int N = characters.Length;
        public MainWindow()
        {
            InitializeComponent();
            
        }

        private string Encode(string text, string key)
        {
            string encode = string.Empty;
            text = text.ToLower();
            key = key.ToLower();

            int keyword_index = 0;

            foreach (char symbol in text)
            {
                if (characters.Contains(symbol))
                {
                    int c = (Array.IndexOf(characters, symbol) + Array.IndexOf(characters, key[keyword_index])) % N;
                    encode += characters[c];
                    keyword_index++;
                    if ((keyword_index) == key.Length)
                        keyword_index = 0;
                }
            }

            return encode;
        }

        private string Decode(string text, string key)
        {
            string decode = string.Empty;
            int keyword_index = 0;

            foreach (char symbol in text)
            {
                int p = (Array.IndexOf(characters, symbol) + N -
                    Array.IndexOf(characters, key[keyword_index])) % N;

                decode += characters[p];

                keyword_index++;

                if ((keyword_index) == key.Length)
                    keyword_index = 0;
            }
            return decode;
        }

        private void InputText_Clicked(object sender, RoutedEventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            bool? res = open.ShowDialog();
            if (res == true)
            {
                using (StreamReader r = new StreamReader(open.FileName))
                {
                    string flow = r.ReadToEnd();
                    InputText.Text = flow;
                }
            }
        }

        private void Encode_Click(object sender, RoutedEventArgs e)
        {
            if (Key.Text.Length == 0)
            {
                MessageBox.Show("Введите ключ!", "Ошибка");
                return;
            }
            EncodeText.Text = Encode(InputText.Text, Key.Text);
        }

        private void Decode_Button_Click(object sender, RoutedEventArgs e)
        {
            var decodeWindow = new DecodeWindow(EncodeText.Text);
            decodeWindow.Show();
            DecodeText.Text = Decode(EncodeText.Text, Key.Text);
        }

        private void Clear_Button_Click(object sender, RoutedEventArgs e)
        {
            InputText.Clear();
            Key.Text = "";
            EncodeText.Text = "";
            DecodeText.Text = "";
        }
    }
}
