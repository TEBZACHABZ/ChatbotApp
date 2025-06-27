using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace ChatbotApp
{
    public partial class MainWindow : Window
    {
        private int currentQuestion = 0;
        private int score = 0;

        private List<(string question, string[] options, string answer)> quiz = new()
        {
            ("What is phishing?", new[] { "Fishing activity", "Online scam", "Password tool", "Antivirus" }, "Online scam"),
            ("True or False: Password123 is secure.", new[] { "True", "False" }, "False")
        };

        // Keyword to response mapping for chatbot
        private readonly Dictionary<string, string> keywordResponses = new(StringComparer.OrdinalIgnoreCase)
        {
            { "phishing", "Phishing is a type of online scam where attackers trick you into giving out personal information. Always check the sender's email and avoid clicking suspicious links." },
            { "password", "Use strong, unique passwords for each account. Consider using a password manager and avoid common passwords like 'Password123'." },
            { "safe browsing", "Safe browsing means keeping your browser and software updated, avoiding suspicious websites, and not downloading files from untrusted sources." },
            { "remind", "Would you like to set a task or reminder?" },
            { "task", "Would you like to set a task or reminder?" },
            { "quiz", "Go to the Quiz tab to test your knowledge!" }
        };

        public MainWindow()
        {
            InitializeComponent();
            LoadQuizQuestion();
            LoadActivityLog();
        }

        // Navigation
        private void Chatbot_Click(object sender, RoutedEventArgs e) => ShowGrid(ChatbotGrid);
        private void Tasks_Click(object sender, RoutedEventArgs e) => ShowGrid(TasksGrid);
        private void Quiz_Click(object sender, RoutedEventArgs e) => ShowGrid(QuizGrid);
        private void Log_Click(object sender, RoutedEventArgs e) => ShowGrid(LogGrid);
        private void Exit_Click(object sender, RoutedEventArgs e) => Application.Current.Shutdown();

        private void ShowGrid(Grid gridToShow)
        {
            ChatbotGrid.Visibility = TasksGrid.Visibility = QuizGrid.Visibility = LogGrid.Visibility = Visibility.Collapsed;
            gridToShow.Visibility = Visibility.Visible;
        }

        // Chatbot Logic with multiple keyword recognition
        private void Send_Click(object sender, RoutedEventArgs e)
        {
            string input = UserInputBox.Text.Trim();
            ChatList.Items.Add("You: " + input);

            var responses = new List<string>();

            foreach (var pair in keywordResponses)
            {
                if (input.Contains(pair.Key, StringComparison.OrdinalIgnoreCase))
                {
                    // Avoid duplicate responses for synonyms like "remind" and "task"
                    if (!responses.Contains(pair.Value))
                        responses.Add(pair.Value);
                }
            }

            string response;
            if (responses.Count > 0)
            {
                response = string.Join(Environment.NewLine, responses);
            }
            else
            {
                response = "Sorry, I didn’t understand that.";
            }

            ChatList.Items.Add("Bot: " + response);
            ActivityList.Items.Add($"Chatbot interaction: {input}");
            UserInputBox.Clear();
        }

        // Tasks
        private void AddTask_Click(object sender, RoutedEventArgs e)
        {
            string title = TaskTitleBox.Text;
            string desc = TaskDescriptionBox.Text;
            string reminder = ReminderDatePicker.SelectedDate?.ToShortDateString() ?? "No reminder";

            string task = $"Task: {title} - {desc} (Reminder: {reminder})";
            TaskList.Items.Add(task);
            ActivityList.Items.Add($"Task added: {title}");

            TaskTitleBox.Clear();
            TaskDescriptionBox.Clear();
            ReminderDatePicker.SelectedDate = null;
        }

        // Quiz
        private void LoadQuizQuestion()
        {
            if (currentQuestion >= quiz.Count)
            {
                QuestionText.Text = $"Quiz Complete! Your score: {score}/{quiz.Count}";
                OptionA.Visibility = OptionB.Visibility = OptionC.Visibility = OptionD.Visibility = Visibility.Collapsed;
                return;
            }

            var q = quiz[currentQuestion];
            QuestionText.Text = q.question;

            OptionA.Content = q.options[0];
            OptionB.Content = q.options[1];

            OptionC.Visibility = OptionD.Visibility = Visibility.Collapsed;
            if (q.options.Length > 2)
            {
                OptionC.Content = q.options[2];
                OptionC.Visibility = Visibility.Visible;
            }
            if (q.options.Length > 3)
            {
                OptionD.Content = q.options[3];
                OptionD.Visibility = Visibility.Visible;
            }

            OptionA.IsChecked = OptionB.IsChecked = OptionC.IsChecked = OptionD.IsChecked = false;
        }

        private void SubmitQuiz_Click(object sender, RoutedEventArgs e)
        {
            string selected = OptionA.IsChecked == true ? OptionA.Content.ToString() :
                              OptionB.IsChecked == true ? OptionB.Content.ToString() :
                              OptionC.IsChecked == true ? OptionC.Content.ToString() :
                              OptionD.IsChecked == true ? OptionD.Content.ToString() : "";

            if (selected == quiz[currentQuestion].answer)
            {
                FeedbackText.Text = "Correct!";
                score++;
                ActivityList.Items.Add($"Quiz Q{currentQuestion + 1}: Correct");
            }
            else
            {
                FeedbackText.Text = $"Incorrect. Answer: {quiz[currentQuestion].answer}";
                ActivityList.Items.Add($"Quiz Q{currentQuestion + 1}: Incorrect");
            }

            currentQuestion++;
            LoadQuizQuestion();
        }

        // Log
        private void LoadActivityLog()
        {
            ActivityList.Items.Add("App started");
        }
    }
}

