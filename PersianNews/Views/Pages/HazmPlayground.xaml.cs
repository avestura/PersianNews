using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Media;
using NHazm;

namespace PersianNews.Views.Pages
{
    /// <summary>
    /// Interaction logic for HazmPlayground.xaml
    /// </summary>
    public partial class HazmPlayground : Page
    {
        public HazmPlayground()
        {
            InitializeComponent();
        }

        private Normalizer Normalizer { get; } = new Normalizer();
        private WordTokenizer WordTokenizer { get; } = new WordTokenizer();
        private SentenceTokenizer SentenceTokenizer { get; } = new SentenceTokenizer();
        private Stemmer Stemmer { get; } = new Stemmer();
        private Lemmatizer Lemmatizer { get; } = new Lemmatizer();
        private POSTagger Tagger { get; } = new POSTagger();

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (IsLoaded)
                icon.Visibility = Visibility.Visible;
        }

        private void ProcessInput()
        {
            if (!string.IsNullOrWhiteSpace(inputField.Text))
            {
                var tokenizedWords = WordTokenizer.Tokenize(inputField.Text);
                var tokenizedSent = SentenceTokenizer.Tokenize(inputField.Text);

                TokenizedText.Text = tokenizedWords.Aggregate((a, b) => $"{a} | {b}");
                TokenizedSent.Text = tokenizedSent.Aggregate((a, b) => $"{a} | {b}");
                StemField.Text = string.Join(" ", tokenizedWords.Select(w => Stemmer.Stem(w)));
                var tags = Tagger.BatchTag(tokenizedSent.Select(s => WordTokenizer.Tokenize(s)).ToList());
                POSTagField.Inlines.Clear();
                tags.ForEach(taggedSentence =>
                {
                    taggedSentence.ForEach(word =>
                    {
                        var text = word.word();
                        var tag = word.tag();

                        var run = new Run(text);
                        var wrapper = new TextBlock(run)
                        {
                            ToolTip = new TextBlock
                            {
                                Text = $"توکن: {text}\nبرچسب: {tag}",
                                FontFamily = (FontFamily)App.Current.Resources["Fonts.IRMitra"],
                                FontSize = 20
                            }
                        };
                        var popup = new Popup
                        {
                            Child = new Border
                            {
                                BorderThickness = new Thickness(1),
                                BorderBrush = Brushes.Gray,
                                Child = new TextBlock
                                {
                                    Background = Brushes.White,
                                    Text = $"واژه: {text}\nبرچسب: {tag}",
                                    Padding = new Thickness(10)
                                }
                            },
                            PlacementTarget = wrapper,
                            StaysOpen = false
                        };
                        wrapper.MouseDown += (s, ev) => popup.IsOpen = true;
                        wrapper.MouseEnter += (s, ev) => wrapper.Foreground = Brushes.Red;
                        wrapper.MouseLeave += (s, ev) => wrapper.Foreground = Brushes.Black;

                        POSTagField.Inlines.Add(wrapper);
                        POSTagField.Inlines.Add(" ");
                    });
                    POSTagField.Inlines.Add(new LineBreak());
                });
            }

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ProcessInput();
            icon.Visibility = Visibility.Collapsed;
        }
    }
}
