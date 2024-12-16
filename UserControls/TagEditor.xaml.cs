using Prot1.Valorant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Valorant;

namespace Prot1.UserControls
{
    public class TextTag : Button
    {
        public static readonly DependencyProperty TagNameProperty =
            DependencyProperty.Register("TagName", typeof(string), typeof(TextTag));

        public string TagName
        {
            get { return (string)GetValue(TagNameProperty); }
            set { SetValue(TagNameProperty, value); }
        }

        static TextTag()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TextTag), new FrameworkPropertyMetadata(typeof(TextTag)));
        }
    }

    public partial class TagEditor : UserControl
    {
        string _key;
        public TagEditor(string key, params string[] tags)
        {
            InitializeComponent();
            _key = key;
            this.Loaded += (sender, e) =>
            {
                string msg;
                if (!Globals.UserSettings.PlayerMessages.TryGetValue(key, out msg))
                    return;

                tagsPanel.Children.Clear();
                foreach (var kvp in Globals.TagNames.Where(x => tags.Contains(x.Key)))
                {
                    var item = new TextTag()
                    {
                        TagName = kvp.Value,
                        Tag = kvp.Key
                    };

                    item.Click += (_s, _e) =>
                    {
                        object tag = (_s as FrameworkElement)?.Tag;

                        if (tag is string tstr && Globals.TagNames.TryGetValue(tstr, out string tagName))
                        {
                            InlineUIContainer inlineUIContainer = new InlineUIContainer(new TextTag { TagName = tagName, Tag = tstr })
                            {
                                BaselineAlignment = BaselineAlignment.Center,
                                Tag = tstr
                            };

                            if(richTextBox.CaretBrush.Transform != null)
                                richTextBox.CaretPosition.Paragraph.Inlines.Add(inlineUIContainer);
                        }
                    };

                    tagsPanel.Children.Add(item);
                }

                if (msg != null && msg != string.Empty)
                {
                    RenderText(msg);
                }
            };
        }

        public const string pattern = @"\{%([^%]+)%\}";
        private void RenderText(string text)
        {
            var p = richTextBox.CaretPosition.Paragraph;
            p.Inlines.Clear();

            var matches = Regex.Matches(text, pattern, RegexOptions.IgnoreCase | RegexOptions.Multiline);

            int lastIndex = 0;
            foreach (Match match in matches)
            {
                if (match.Index > lastIndex)
                {
                    p.Inlines.Add(new Run(text.Substring(lastIndex, match.Index - lastIndex)));
                }

                if (Globals.TagNames.TryGetValue(match.Groups[1].Value, out string name))
                {
                    p.Inlines.Add(new InlineUIContainer(new TextTag()
                    {
                        TagName = name,
                        Tag = match.Groups[1].Value
                    }) { BaselineAlignment = BaselineAlignment.Center, Tag = match.Groups[1].Value });
                }

                lastIndex = match.Index + match.Length;
            }

            // Add any remaining text after the last tag
            if (lastIndex < text.Length)
            {
                p.Inlines.Add(new Run(text.Substring(lastIndex)));
            }
            else if (matches.Count == 0)
            {
                // If no tags were found, add the entire text as a single Run
                p.Inlines.Add(new Run(text));
            }
        }

        public delegate void OnClosedEventHandler();
        public event OnClosedEventHandler OnClosed;

        // CANCEL
        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            OnClosed?.Invoke();
        }

        // SAVE
        private void Border_MouseLeftButtonDown_1(object sender, MouseButtonEventArgs e)
        {
            if (Globals.UserSettings != null)
            {
                if (!Globals.UserSettings.PlayerMessages.ContainsKey(_key))
                {
                    OnClosed?.Invoke();
                    return;
                }

                Globals.UserSettings.PlayerMessages[_key] = GetMessage(richTextBox.Document);
                Settings.SaveSettings(ref Globals.UserSettings);

                OnClosed?.Invoke();
            }
        }

        private string GetMessage(FlowDocument document)
        {
            var textBuilder = new StringBuilder();

            foreach (var block in document.Blocks)
            {
                if (block is Paragraph paragraph)
                {
                    foreach (var inline in paragraph.Inlines)
                    {
                        if (inline is InlineUIContainer uiContainer)
                        {
                            if (uiContainer.Tag != null)
                            {
                                textBuilder.Append("{%" + uiContainer.Tag.ToString() + "%}");
                            }
                        }
                        else if (inline is Run run)
                        {
                            textBuilder.Append(run.Text);
                        }
                        else if (inline is LineBreak)
                        {
                            textBuilder.AppendLine();
                        }
                    }
                }
            }

            return textBuilder.ToString();
        }
    }
}
