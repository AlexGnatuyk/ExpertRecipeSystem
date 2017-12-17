using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ExpertRecipeSystem
{
    public class CSVExpertSystem 
    {
        private const int CurrentState = 0;
        private const int NextState = 1;
        private const int EndOfSearching = 2;
        private const int Answer = 3;
        private const int Output = 4;

        private string _pathToStateTableFile;
        private string _pathToStateQuestionMapFile;
        private List<string[]> _stateTableList;
        private Dictionary<int, string> _questionList;
        private int _currentState = -1;
        private List<Tuple<string, string>> _history;
        private bool _isEndOfSearching;

        public string NextQuestion(string answerForPreviousQuestion, out bool isEndOfSearching)
        {
            string question;
            isEndOfSearching = _isEndOfSearching;
            if (_currentState == -1)
            {
                _currentState = 0;
                question = _questionList[_currentState];
                _history.Add(new Tuple<string, string>(question, ""));
                return question;
            }

            if (_stateTableList.FindIndex(tokens => tokens[Answer] == answerForPreviousQuestion) == -1)
                return null;

            _history.Add(new Tuple<string, string>(_questionList[_currentState], answerForPreviousQuestion));

            var transition = _stateTableList
                .Where(tokens => int.Parse(tokens[CurrentState]) == _currentState)
                .Where(tokens => tokens[Answer] == answerForPreviousQuestion)
                .FirstOrDefault();

            _currentState = int.Parse(transition[NextState]);

            _isEndOfSearching = transition[EndOfSearching] == "0" ? false : true;
            isEndOfSearching = _isEndOfSearching;

            if (isEndOfSearching)
                return transition[Output];

            return _questionList[_currentState];
        }

        public void Save(string path)
        {
            using (var sw = new StreamWriter(path, false, Encoding.GetEncoding(1251)))
            {
                sw.WriteLine("Вопрос;Ответ");
                foreach (var historyItem in _history)
                {
                    sw.WriteLine(historyItem.Item1 + ";" + historyItem.Item2);
                }
            }
        }



        private void ParseFiles()
        {
            _stateTableList = File.ReadLines(_pathToStateTableFile, Encoding.GetEncoding(1251))
                .Skip(1)
                .Select(line => line.Split(';'))
                .ToList();


            var pairs = File.ReadLines(_pathToStateQuestionMapFile, Encoding.GetEncoding(1251))
                .Skip(1)
                .Select(line => line.Split(';'))
                .Select(tokens => new { Key = int.Parse(tokens[0]), Value = tokens[1] });
            foreach (var pair in pairs)
            {
                _questionList.Add(pair.Key, pair.Value);
            }
        }

        public void Clear()
        {
            _history.Clear();
            _currentState = -1;
            _isEndOfSearching = false;
        }

        public string GetHelp()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Доступные ответы на вопросы:" + Environment.NewLine);
            var questionAnswers = _stateTableList.Select(tokens =>
                _questionList[int.Parse(tokens[CurrentState])] + " - " + tokens[Answer]
            )
            .Distinct();
            foreach (var questionAnswer in questionAnswers)
            {
                sb.AppendLine(questionAnswer);
            }
            return sb.ToString();
        }

        public List<Tuple<string, string>> GetHistory()
        {
            return _history;
        }

        public CSVExpertSystem(string pathToStateTableFile, string pathToStateQuestionMapFile)
        {
            _pathToStateQuestionMapFile = pathToStateQuestionMapFile;
            _pathToStateTableFile = pathToStateTableFile;
            _stateTableList = new List<string[]>();
            _history = new List<Tuple<string, string>>();
            _questionList = new Dictionary<int, string>();
            _isEndOfSearching = false;
            ParseFiles();
        }
    }
}