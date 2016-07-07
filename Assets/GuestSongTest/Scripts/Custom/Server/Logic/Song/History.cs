using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Photon.LoadBalancing.Custom.Server.Logic.Song
{
    public class History
    {
        public string game_id;
        public int round_index;
        public int question_index;
        public int question_id;
        public int user_id;
        public string anwser_text;
        public int? anwser_option;
        public int score;
        public int time_used;
    }
}
