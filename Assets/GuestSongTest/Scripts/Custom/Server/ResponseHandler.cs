#if !UNITY_5_3_OR_NEWER
using ExitGames.Logging;
using Photon.LoadBalancing.Custom.Common;
using Photon.LoadBalancing.Custom.Models;
using Photon.LoadBalancing.Custom.Server.Database;
using Photon.LoadBalancing.Custom.Server.Operations.Requests;
using Photon.LoadBalancing.Custom.Server.Operations.Responses;
using Photon.LoadBalancing.MasterServer;
using Photon.SocketServer;
using System;
using System.Linq;
using Photon.Hive;
using Photon.LoadBalancing.Custom.Server.Logic;
using Photon.LoadBalancing.GameServer;
using System.Collections.Generic;

namespace Photon.LoadBalancing.Custom.Server
{
    public class ResponseHandler
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        public static void SendResponse(PeerBase peer, BaseResponse response, SendParameters sendParameters)
        {
            peer.SendOperationResponse(new OperationResponse(MessageTag.KINGPLAY_OPERATION_CODE, response), sendParameters);
        }

        //public static OperationResponse ListGame(PeerBase peer, OperationRequest operationRequest, SendParameters sendParameters)
        //{
        //    // validate the operation request 
        //    OperationResponse response;
        //    var operation = new ListGameRequest(peer.Protocol, operationRequest);
        //    if (OperationHelper.ValidateOperation(operation, log, out response) == false)
        //    {
        //        return response;
        //    }

        //    try
        //    {
        //        using (var db = new DbContext())
        //        {
        //            IQueryable<Game> games = db.Games.Where(x => x.Status == 1);

        //            if (games.Count<Game>() == 0)
        //            {
        //                List<Game> sample_data = new List<Game>();

        //                sample_data.Add(new Game { Code = 1, Name = "Ba cây", Status = 1 });
        //                sample_data.Add(new Game { Code = 2, Name = "Phỏm", Status = 1 });
        //                sample_data.Add(new Game { Code = 3, Name = "Tiến lên Miền Nam", Status = 1 });
        //                sample_data.Add(new Game { Code = 4, Name = "Sâm", Status = 1 });
        //                sample_data.Add(new Game { Code = 5, Name = "Tiến lên Miền Bắc", Status = 1 });
        //                sample_data.Add(new Game { Code = 6, Name = "Poker", Status = 1 });
        //                sample_data.Add(new Game { Code = 7, Name = "Cờ tướng", Status = 1 });
        //                sample_data.Add(new Game { Code = 8, Name = "Xì tố", Status = 1 });

        //                db.Games.AddRange(sample_data);
        //                db.SaveChanges();
        //            }

        //            List<string> listName = new List<string>();
        //            List<int> listCode = new List<int>();
        //            List<int> listMaxPlayer = new List<int>();

        //            foreach (Game game in games)
        //            {
        //                listName.Add(game.Name);
        //                listCode.Add(game.Code);
        //                listMaxPlayer.Add(game.MaxPlayerInBoard);
        //            }

        //            peer.SendOperationResponse(
        //                new OperationResponse(
        //                    MessageTag.KINGPLAY_OPERATION_CODE,
        //                    new ListGameResponse
        //                    {
        //                        Name = listName.ToArray(),
        //                        Code = listCode.ToArray(),
        //                        MaxPlayerInBoard = listMaxPlayer.ToArray(),
        //                    }
        //                ),
        //                sendParameters
        //            );
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        log.Error(ex);
        //    }

        //    return null;
        //}

        public static OperationResponse ProfileResponse(PeerBase peer, OperationRequest operationRequest, SendParameters sendParameters)
        {
            // validate the operation request 
            OperationResponse response;
            var operation = new ProfileRequest(peer.Protocol, operationRequest);
            if (OperationHelper.ValidateOperation(operation, log, out response) == false)
            {
                return response;
            }

            try
            {
                using (var db = new DbContext())
                {
                    ProfileResponse _response = new ProfileResponse();
                    ModelUser user = db.Users.SingleOrDefault(x => x.id == operation.UserId);
                    if (user != null)
                    {
                        _response.Id = user.id;
                        _response.Username = user.username;
                        _response.Nickname = user.nickname;
                    }

                    SendResponse(peer, _response, sendParameters);
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }

            return null;
        }

        public static OperationResponse PlaylistResponse(PeerBase peer, OperationRequest operationRequest, SendParameters sendParameters)
        {
            // validate the operation request 
            OperationResponse response;
            var operation = new PlaylistRequest(peer.Protocol, operationRequest);
            if (OperationHelper.ValidateOperation(operation, log, out response) == false)
            {
                return response;
            }

            try
            {
                using (var db = new DbContext())
                {
                    PlaylistResponse _response = new PlaylistResponse();

                    ModelPlayList[] play_lists = db.PlayLists.Where(x => x.enable == true).ToArray();
                    if (play_lists.Length != 0)
                    {
                        _response.Id = new int[play_lists.Length];
                        _response.Name = new string[play_lists.Length];
                        _response.Slug = new string[play_lists.Length];
                        _response.Enable = new bool[play_lists.Length];
                        _response.Start = new int[play_lists.Length];
                        _response.End = new int[play_lists.Length];
                        _response.Type = new int[play_lists.Length];
                        _response.Price = new int[play_lists.Length];

                        for (int i = 0; i < play_lists.Length; i++)
                        {
                            _response.Id[i] = play_lists[i].id;
                            _response.Name[i] = play_lists[i].name;
                            _response.Slug[i] = play_lists[i].slug;
                            _response.Enable[i] = play_lists[i].enable;
                            _response.Start[i] = play_lists[i].start;
                            _response.End[i] = play_lists[i].end;
                            _response.Type[i] = (int)play_lists[i].type;
                            _response.Price[i] = play_lists[i].price;
                        }
                    }

                    SendResponse(peer, _response, sendParameters);
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }

            return null;
        }

        public static void GameStateResponse(List<HivePeer> peers, Common.Quiz.SongGameState state) { foreach (var peer in peers) { GameStateResponse(peer, state); } }

        public static void GameStateResponse(PeerBase peer, Common.Quiz.SongGameState state)
        {
            try
            {
                SendResponse(peer, new GameStateResponse() { State = state }, new SendParameters());
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        public static void QuestionListResponse(List<HivePeer> peers, SongGame data, int playedRound, int totalRound)
        {
            try
            {
                using (var db = new DbContext())
                {
                    db.Database.Log = log.Debug;

                    QuestionListResponse _response = new QuestionListResponse();

                    int question_number = data.round_index == 0 ? Rules.ROUND_1_SONG_NUMBER : (data.round_index == 2 ? Rules.ROUND_2_SONG_NUMBER : Rules.ROUND_3_SONG_NUMBER); // get round question number
                    List<ModelQuestion> questions = db.Questions.Where(x => x.playlist_id == data.PlayListId).Take(question_number).ToList();

                    if (questions.Count != 0)
                    {
                        _response.Id = new int[questions.Count];
                        _response.Question = new string[questions.Count];
                        _response.Option1 = new string[questions.Count];
                        _response.Option2 = new string[questions.Count];
                        _response.Option3 = new string[questions.Count];
                        _response.Option4 = new string[questions.Count];
                        _response.Url = new string[questions.Count];
                        _response.BundleName = new string[questions.Count];
                        _response.AssetName = new string[questions.Count];
                        _response.PlayedRound = playedRound;
                        _response.TotalRound = totalRound;

                        for (int i = 0; i < questions.Count; i++)
                        {
                            _response.Id[i] = questions[i].id;
                            _response.Question[i] = questions[i].question;
                            _response.Option1[i] = questions[i].option1;
                            _response.Option2[i] = questions[i].option2;
                            _response.Option3[i] = questions[i].option3;
                            _response.Option4[i] = questions[i].option4;

                            // get song, normaly we will take from redis db for better performance
                            ModelSong song = db.Songs.Find(questions[i].song_id);
                            if (song == null)
                            {
                                questions.Remove(questions[i]);
                                throw new Exception("Invalid song id in question: " + questions[i].id);
                            }
                            _response.Url[i] = song.url;
                            _response.BundleName[i] = song.bundle_name;
                            _response.AssetName[i] = song.asset_name;
                        }
                    }

                    data.questions = questions;
                    foreach (PeerBase peer in peers) SendResponse(peer, _response, new SendParameters());
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        public static void ReadyPlayersResponse(List<HivePeer> peers, List<int> ReadyPlayerIds)
        {
            ReadyPlayersResponse _response = new ReadyPlayersResponse() { Id = ReadyPlayerIds.ToArray() };
            foreach (PeerBase peer in peers) SendResponse(peer, _response, new SendParameters());
        }

        public static void AnwserBuzzResponse(List<HivePeer> peers, int PlayerId, int Time)
        {
            AnwserBuzzResponse _response = new AnwserBuzzResponse() { Id = PlayerId };
            foreach (PeerBase peer in peers) SendResponse(peer, _response, new SendParameters());
        }

        public static void AnwserTextResponse(List<HivePeer> peers, int PlayerId, string Text)
        {
            AnwserTextResponse _response = new AnwserTextResponse() { Id = PlayerId, Text = Text };
            foreach (PeerBase peer in peers) SendResponse(peer, _response, new SendParameters());
        }

    }
}
#endif