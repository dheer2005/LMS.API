﻿using System.Text.Json.Serialization;

namespace LMS.API.Models
{
    public class Question
    {
        public int Id { get; set; }
        public int QuizId { get; set; }
        public string QuestionText { get; set; }
        public string OptionA { get; set; }
        public string OptionB { get; set; }
        public string OptionC { get; set; }
        public string OptionD { get; set; }
        public string CorrectOption { get; set; }

        [JsonIgnore]
        public Quiz Quiz { get; set; }
    }
}
