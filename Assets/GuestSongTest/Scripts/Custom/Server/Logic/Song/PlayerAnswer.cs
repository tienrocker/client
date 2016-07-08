using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Photon.LoadBalancing.Custom.Server.Logic.Song
{
    public class PlayerAnswer
    {
        public int question_id;
        public int user_id;
        public string anwser_text;
        public int? anwser_option;
        public int time_server;
        public int time_answer;
        public int score;
    }
}
